module BindApiTest

// Coverage for the Bind API surface outside Bind.el / Bind.each. The lifecycle of a binding
// is well covered by AnchorTest and BindingTest; what these tests pin is what each binding
// family actually does to the DOM, and — for the form bindings — that data flows back out
// of the DOM into the store.

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.Core
open Sutil.CoreElements
open System
open Fable.Core
open Browser.Types
open Browser.CssExtensions

// Form bindings read the document (bindGroup queries by input name), so these tests mount
// into document.body rather than the detached container the default harness uses.
let private mountInBody (app: SutilElement) =
    let host = Browser.Dom.document.createElement "div"
    Browser.Dom.document.body.appendChild host |> ignore
    currentEl <- host
    let mounted = Sutil.Program.mount (host, app)

    let dispose () =
        mounted.Dispose()
        Browser.Dom.document.body.removeChild host |> ignore

    host, dispose

/// The user typing is an "input" event; every two-way binding in Bindings.fs listens for it.
let private fireInput (el: Node) =
    el.dispatchEvent (Interop.customEvent "input" {|  |}) |> ignore

let private asEl (n: Node) = n :?> HTMLElement
let private asInput (n: Node) = n :?> HTMLInputElement
let private asSelect (n: Node) = n :?> HTMLSelectElement

describe "Sutil.BindApi" <| fun () ->

    // ---------------------------------------------------------------- form bindings

    it "typing into an input writes through Bind.attr into its store" <| fun () -> promise {
        let store = Store.make "start"
        let host, dispose = mountInBody (Html.input [ Bind.attr ("value", store) ])

        let input = asInput (host.querySelector "input")
        Expect.areEqual (input.value, "start", "store seeds the input")

        input.value <- "typed"
        fireInput input

        Expect.areEqual (Store.get store, "typed", "input event writes back to the store")

        Store.set store "from store"
        Expect.areEqual (input.value, "from store", "store change writes into the input")

        dispose ()
        return ()
    }

    it "Bind.value dispatches the element's value on input" <| fun () -> promise {
        let store = Store.make "a"
        let mutable dispatched = []
        let host, dispose = mountInBody (Html.input [ Bind.value (store, fun v -> dispatched <- dispatched @ [ v ]) ])

        let input = asInput (host.querySelector "input")
        input.value <- "b"
        fireInput input
        input.value <- "c"
        fireInput input

        Expect.areEqual (dispatched, [ "b"; "c" ], "every input event dispatches the current value")

        dispose ()
        return ()
    }

    it "Bind.isChecked dispatches the checkbox state on input" <| fun () -> promise {
        let store = Store.make false
        let mutable dispatched = []
        let host, dispose =
            mountInBody (Html.input [ attr ("type", "checkbox"); Bind.isChecked (store, fun v -> dispatched <- dispatched @ [ v ]) ])

        let input = asInput (host.querySelector "input")
        Expect.areEqual (input.``checked``, false, "store seeds the checkbox")

        input.``checked`` <- true
        fireInput input
        Expect.areEqual (dispatched, [ true ], "checking dispatches true")

        Store.set store true
        Expect.areEqual (input.``checked``, true, "store change checks the box")

        dispose ()
        return ()
    }

    it "Bind.radioValue checks the radio matching the store and reports the clicked one" <| fun () -> promise {
        let store = Store.make "red"

        let radio value =
            Html.input [ attr ("type", "radio"); attr ("value", value); Bind.radioValue store ]

        let host, dispose = mountInBody (Html.div [ radio "red"; radio "green" ])

        let inputs = host.querySelectorAll "input"
        let red = asInput inputs.[0]
        let green = asInput inputs.[1]

        Expect.areEqual (red.``checked``, true, "store selects red")
        Expect.areEqual (green.``checked``, false, "green is not selected")

        green.``checked`` <- true
        fireInput green
        Expect.areEqual (Store.get store, "green", "clicking green writes green to the store")
        Expect.areEqual (red.``checked``, false, "red is deselected by the store update")

        dispose ()
        return ()
    }

    it "Bind.checkboxGroup collects every checked value in the group" <| fun () -> promise {
        let store = Store.make [ "b" ]

        let box' value =
            Html.input [ attr ("type", "checkbox"); attr ("value", value); Bind.checkboxGroup store ]

        let host, dispose = mountInBody (Html.div [ box' "a"; box' "b"; box' "c" ])

        let inputs = host.querySelectorAll "input"
        let a = asInput inputs.[0]
        let b = asInput inputs.[1]

        Expect.areEqual (a.``checked``, false, "a starts unchecked")
        Expect.areEqual (b.``checked``, true, "the store's initial value checks b")

        a.``checked`` <- true
        fireInput a

        // The group reports every checked box, not just the one that changed.
        Expect.areEqual (Store.get store |> List.sort, [ "a"; "b" ], "checking a adds it alongside b")

        dispose ()
        return ()
    }

    it "Bind.selectSingle selects the store's option and reports a new selection" <| fun () -> promise {
        let store = Store.make "two"

        let app =
            Html.select [
                Bind.selectSingle store
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")

        Expect.areEqual (select.selectedIndex, 1, "store selects the second option")

        select.selectedIndex <- 0
        fireInput select
        Expect.areEqual (Store.get store, "one", "selecting the first option writes it to the store")

        dispose ()
        return ()
    }

    it "Bind.selectMultiple round-trips the whole selection" <| fun () -> promise {
        let store = Store.make [ "one" ]

        let app =
            Html.select [
                attr ("multiple", true)
                Bind.selectMultiple store
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")
        let options = host.querySelectorAll "option"

        Expect.areEqual ((options.[0] :?> HTMLOptionElement).selected, true, "store selects option one")

        (options.[1] :?> HTMLOptionElement).selected <- true
        fireInput select
        Expect.areEqual (Store.get store |> List.sort, [ "one"; "two" ], "both selections reach the store")

        dispose ()
        return ()
    }

    // ------------------------------------------------------- attribute / class / style

    it "Bind.attr updates a plain attribute when the store changes" <| fun () -> promise {
        let store = Store.make "first"
        let app = Html.div [ Bind.attr ("title", store :> IObservable<string>) ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.getAttribute "title", "first", "initial attribute")

        Store.set store "second"
        Expect.areEqual (div.getAttribute "title", "second", "attribute follows the store")
        return ()
    }

    it "Bind.booleanAttr adds and removes the attribute" <| fun () -> promise {
        let store = Store.make false
        let app = Html.input [ Bind.booleanAttr ("disabled", store) ]

        mountTestApp app
        let input = asInput (currentEl.querySelector "input")
        Expect.areEqual (input.hasAttribute "disabled", false, "false leaves the attribute off")

        Store.set store true
        Expect.areEqual (input.hasAttribute "disabled", true, "true adds the attribute")

        Store.set store false
        Expect.areEqual (input.hasAttribute "disabled", false, "false removes it again")
        return ()
    }

    it "Bind.className replaces the element's class" <| fun () -> promise {
        let store = Store.make "alpha"
        let app = Html.div [ Bind.className store ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.assertTrue (div.classList.contains "alpha") "initial class applied"

        Store.set store "beta"
        Expect.assertTrue (div.classList.contains "beta") "new class applied"
        Expect.assertFalse (div.classList.contains "alpha") "old class removed"
        return ()
    }

    it "Bind.classNames applies every class in the sequence" <| fun () -> promise {
        let store = Store.make [ "one"; "two" ]
        let app = Html.div [ Bind.classNames store ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.assertTrue (div.classList.contains "one") "first class applied"
        Expect.assertTrue (div.classList.contains "two") "second class applied"

        Store.set store [ "three" ]
        Expect.assertTrue (div.classList.contains "three") "replacement class applied"
        Expect.assertFalse (div.classList.contains "one") "replaced class removed"
        return ()
    }

    it "Bind.toggleClass swaps between the active and inactive class" <| fun () -> promise {
        let store = Store.make true
        let app = Html.div [ Bind.toggleClass (store, "on", "off") ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.assertTrue (div.classList.contains "on") "true gives the active class"
        Expect.assertFalse (div.classList.contains "off") "inactive class absent"

        Store.set store false
        Expect.assertTrue (div.classList.contains "off") "false gives the inactive class"
        Expect.assertFalse (div.classList.contains "on") "active class removed"
        return ()
    }

    it "Bind.style writes name/value pairs into the style attribute" <| fun () -> promise {
        let store = Store.make "red"
        let app = Html.div [ Bind.style (store |> Store.map (fun c -> [ "color", box c ])) ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.color, "red", "initial style")

        Store.set store "blue"
        Expect.areEqual (div.style.color, "blue", "style follows the store")
        return ()
    }

    it "Bind.style with an updater writes through the CSS declaration" <| fun () -> promise {
        let store = Store.make 2.0
        let app =
            Html.div [
                Bind.style (store, fun (style: CSSStyleDeclaration) (v: float) -> style.opacity <- string v)
            ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.opacity, "2", "updater ran with the initial value")

        Store.set store 3.0
        Expect.areEqual (div.style.opacity, "3", "updater ran again on change")
        return ()
    }

    it "Bind.effect hands the element itself to the updater" <| fun () -> promise {
        let store = Store.make "hello"
        let mutable seen = []

        let app =
            Html.div [
                Bind.effect (store, fun (e: HTMLElement) v ->
                    seen <- seen @ [ v ]
                    e.setAttribute ("data-seen", v))
            ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.getAttribute "data-seen", "hello", "effect saw the element")

        Store.set store "again"
        Expect.areEqual (div.getAttribute "data-seen", "again", "effect re-ran")
        Expect.areEqual (seen, [ "hello"; "again" ], "effect ran once per value")
        return ()
    }

    // ------------------------------------------------------------- geometry / visibility

    it "Bind.visibility hides and shows the element" <| fun () -> promise {
        let visible = Store.make true
        let app = Html.div [ Bind.visibility visible (Html.p [ text "peekaboo" ]) ]

        mountTestApp app
        let p () = asEl (currentEl.querySelector "p")
        Expect.areEqual ((p ()).style.display, "", "visible by default")

        Store.set visible false
        Expect.areEqual ((p ()).style.display, "none", "hidden when the store goes false")

        Store.set visible true
        Expect.areEqual ((p ()).style.display, "", "shown again")
        return ()
    }

    it "Bind.widthHeight sizes the element, zero included" <| fun () -> promise {
        let size = Store.make (0.0, 0.0)
        let app = Html.div [ Bind.widthHeight size ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.width, "0px", "zero is a real size, not a missing one")
        Expect.areEqual (div.style.height, "0px", "zero is a real size, not a missing one")

        Store.set size (40.0, 25.0)
        Expect.areEqual (div.style.width, "40px", "width applied")
        Expect.areEqual (div.style.height, "25px", "height applied")

        Store.set size (0.0, 25.0)
        Expect.areEqual (div.style.width, "0px", "collapsing one axis to zero is applied")
        Expect.areEqual (div.style.height, "25px", "the other axis is untouched")
        return ()
    }

    it "Bind.xywh sets position and size together" <| fun () -> promise {
        let box' = Store.make (1.0, 2.0, 3.0, 4.0)
        let app = Html.div [ Bind.xywh box' ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.left, "1px", "left")
        Expect.areEqual (div.style.top, "2px", "top")
        Expect.areEqual (div.style.width, "3px", "width")
        Expect.areEqual (div.style.height, "4px", "height")

        Store.set box' (0.0, 0.0, 0.0, 0.0)
        Expect.areEqual (div.style.left, "0px", "zero origin applied")
        Expect.areEqual (div.style.top, "0px", "zero origin applied")
        Expect.areEqual (div.style.width, "0px", "zero size applied")
        Expect.areEqual (div.style.height, "0px", "zero size applied")
        return ()
    }

    // ------------------------------------------------------------------------- async

    it "Bind.promise shows the waiting view and then the result" <| fun () -> promise {
        let mutable resolve : string -> unit = ignore
        let p = Promise.create (fun res _ -> resolve <- res)

        let app =
            Html.div [
                Bind.promise (p, (fun (v: string) -> Html.p [ text v ]), Html.span [ text "waiting" ], (fun (x: exn) -> Html.b [ text x.Message ]))
            ]

        mountTestApp app
        Expect.queryText "div span" "waiting"

        resolve "done"
        do! Promise.sleep 50

        Expect.queryText "div p" "done"
        Expect.areEqual (currentEl.querySelectorAll("div span").length, 0, "waiting view is gone")
        return ()
    }

    it "Bind.promise shows the error view when the promise rejects" <| fun () -> promise {
        let mutable reject : exn -> unit = ignore
        let p : JS.Promise<string> = Promise.create (fun _ rej -> reject <- rej)

        let app =
            Html.div [
                Bind.promise (p, (fun (v: string) -> Html.p [ text v ]), Html.span [ text "waiting" ], (fun (x: exn) -> Html.b [ text x.Message ]))
            ]

        mountTestApp app
        Expect.queryText "div span" "waiting"

        reject (Exception "boom")
        do! Promise.sleep 50

        Expect.queryText "div b" "boom"
        return ()
    }

    // -------------------------------------------------------------- list / option forms

    it "Bind.eachi renders each item with its index" <| fun () -> promise {
        let items = Store.make [ "a"; "b" ]
        let app = Html.div [ Bind.eachi (items, fun (i, s) -> Html.p [ text (sprintf "%d:%s" i s) ]) ]

        mountTestApp app
        Expect.queryText "div p:nth-child(1)" "0:a"
        Expect.queryText "div p:nth-child(2)" "1:b"

        Store.set items [ "x"; "y"; "z" ]
        Expect.queryNumChildren "div" 3
        Expect.queryText "div p:nth-child(3)" "2:z"
        return ()
    }

    it "Bind.some renders the view for Some and the fallback for None" <| fun () -> promise {
        let store = Store.make (Some "here")
        let app = Html.div [ Bind.some (store, (fun v -> Html.p [ text v ]), Html.span [ text "nothing" ]) ]

        mountTestApp app
        Expect.queryText "div p" "here"

        Store.set store None
        Expect.queryText "div span" "nothing"
        Expect.areEqual (currentEl.querySelectorAll("div p").length, 0, "Some view removed")

        Store.set store (Some "back")
        Expect.queryText "div p" "back"
        return ()
    }

    it "Bind.el2 rebuilds when either observable changes" <| fun () -> promise {
        let a = Store.make 1
        let b = Store.make 10
        let app = Html.div [ Bind.el2 a b (fun (x, y) -> Html.p [ text (sprintf "%d-%d" x y) ]) ]

        mountTestApp app
        Expect.queryText "div p" "1-10"

        Store.set a 2
        Expect.queryText "div p" "2-10"

        Store.set b 20
        Expect.queryText "div p" "2-20"

        Expect.queryNumChildren "div" 1
        return ()
    }

    // ------------------------------------------------- remaining Bind surface

    it "Bind.attrInit seeds the attribute and dispatches it on input" <| fun () -> promise {
        let mutable dispatched = []
        let host, dispose = mountInBody (Html.input [ Bind.attrInit ("value", "seed", fun v -> dispatched <- dispatched @ [ v ]) ])

        let input = asInput (host.querySelector "input")
        Expect.areEqual (input.value, "seed", "initial value written to the element")

        input.value <- "edited"
        fireInput input
        Expect.assertTrue (dispatched |> List.contains "edited") "input event dispatches the current value"

        dispose ()
        return ()
    }

    it "Bind.prop binds a property in both directions" <| fun () -> promise {
        let store = Store.make "one"
        let mutable dispatched = []
        let host, dispose =
            mountInBody (Html.div [ Bind.prop ("title", store, fun v -> dispatched <- dispatched @ [ v ]) ])

        let div = asEl (host.querySelector "div")
        Expect.areEqual (div.title, "one", "store writes into the property")

        Store.set store "two"
        Expect.areEqual (div.title, "two", "property follows the store")

        div.title <- "typed"
        fireInput div
        Expect.assertTrue (dispatched |> List.contains "typed") "input event dispatches the property value"

        dispose ()
        return ()
    }

    it "Bind.selectMultiple clears every option when the selection empties" <| fun () -> promise {
        let store = Store.make [ "one"; "two" ]

        let app =
            Html.select [
                attr ("multiple", true)
                Bind.selectMultiple store
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")
        let options = host.querySelectorAll "option"

        Expect.areEqual (select.selectedOptions.length, 2, "both options start selected")

        Store.set store []

        Expect.areEqual (select.selectedOptions.length, 0, "an empty selection deselects everything")
        Expect.areEqual ((options.[0] :?> HTMLOptionElement).selected, false, "first option deselected")
        Expect.areEqual ((options.[1] :?> HTMLOptionElement).selected, false, "second option deselected")

        dispose ()
        return ()
    }

    it "Bind.selectOptional round-trips Some through the selection" <| fun () -> promise {
        let store = Store.make (Some "two")

        let app =
            Html.select [
                Bind.selectOptional store
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")
        Expect.areEqual (select.selectedIndex, 1, "Some selects the matching option")

        select.selectedIndex <- 0
        fireInput select
        Expect.areEqual (Store.get store, Some "one", "selecting writes Some back to the store")

        dispose ()
        return ()
    }

    it "Bind.selectOptional clears the element when the store goes to None" <| fun () -> promise {
        // A non-multiple <select> re-selects its first option the moment everything is deselected,
        // so the binding has to say "nothing is selected" explicitly or the store's None and the
        // visible selection diverge.
        let store = Store.make (Some "two")

        let app =
            Html.select [
                Bind.selectOptional store
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")

        Store.set store None

        Expect.areEqual (Store.get store, None, "the store holds None")
        Expect.areEqual (select.selectedIndex, -1, "and the element shows no selection")

        dispose ()
        return ()
    }

    it "Bind.selected dispatches the whole selection" <| fun () -> promise {
        let selection = Store.make [ "one" ]
        let mutable dispatched = []

        let app =
            Html.select [
                attr ("multiple", true)
                Bind.selected (selection :> IObservable<string list>, fun v -> dispatched <- dispatched @ [ v ])
                Html.option [ attr ("value", "one"); text "One" ]
                Html.option [ attr ("value", "two"); text "Two" ]
            ]

        let host, dispose = mountInBody app
        let select = asSelect (host.querySelector "select")
        let options = host.querySelectorAll "option"

        Expect.areEqual ((options.[0] :?> HTMLOptionElement).selected, true, "observable seeds the selection")

        (options.[1] :?> HTMLOptionElement).selected <- true
        fireInput select
        Expect.areEqual (dispatched, [ [ "one"; "two" ] ], "the whole selection is dispatched once")

        dispose ()
        return ()
    }

    it "Bind.elementStyle hands both the element and its style to the updater" <| fun () -> promise {
        let store = Store.make "red"

        let app =
            Html.div [
                Bind.elementStyle (store, fun (e: HTMLElement) (style: CSSStyleDeclaration) (v: string) ->
                    style.color <- v
                    e.setAttribute ("data-colour", v))
            ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.color, "red", "style written through the declaration")
        Expect.areEqual (div.getAttribute "data-colour", "red", "updater also saw the element")

        Store.set store "blue"
        Expect.areEqual (div.style.color, "blue", "style follows the store")
        Expect.areEqual (div.getAttribute "data-colour", "blue", "element attribute follows too")
        return ()
    }

    it "Bind.leftTop positions the element, zero included" <| fun () -> promise {
        let xy = Store.make (0.0, 0.0)
        let app = Html.div [ Bind.leftTop xy ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.left, "0px", "zero is a real coordinate, not a missing one")
        Expect.areEqual (div.style.top, "0px", "zero is a real coordinate, not a missing one")

        Store.set xy (12.0, 34.0)
        Expect.areEqual (div.style.left, "12px", "left applied")
        Expect.areEqual (div.style.top, "34px", "top applied")

        Store.set xy (0.0, 34.0)
        Expect.areEqual (div.style.left, "0px", "moving back to the left edge is applied")
        Expect.areEqual (div.style.top, "34px", "the other axis is untouched")
        return ()
    }

    it "Bind.rightTop positions from the right edge" <| fun () -> promise {
        let xy = Store.make (5.0, 6.0)
        let app = Html.div [ Bind.rightTop xy ]

        mountTestApp app
        let div = asEl (currentEl.querySelector "div")
        Expect.areEqual (div.style.right, "5px", "right applied")
        Expect.areEqual (div.style.top, "6px", "top applied")
        Expect.areEqual (div.style.left, "", "left is left alone")

        Store.set xy (0.0, 0.0)
        Expect.areEqual (div.style.right, "0px", "zero pins it to the right edge")
        Expect.areEqual (div.style.top, "0px", "zero pins it to the top edge")
        return ()
    }

    it "Bind.fragment rebuilds its content on each value" <| fun () -> promise {
        let store = Store.make 1
        let app = Html.div [ Bind.fragment store (fun n -> Html.p [ text (string n) ]) ]

        mountTestApp app
        Expect.queryText "div p" "1"

        Store.set store 2
        Expect.queryText "div p" "2"
        Expect.queryNumChildren "div" 1
        return ()
    }

    it "Bind.fragment2 rebuilds when either value changes" <| fun () -> promise {
        let a = Store.make 1
        let b = Store.make 10
        let app = Html.div [ Bind.fragment2 a b (fun (x, y) -> Html.p [ text (sprintf "%d/%d" x y) ]) ]

        mountTestApp app
        Expect.queryText "div p" "1/10"

        Store.set a 2
        Expect.queryText "div p" "2/10"

        Store.set b 20
        Expect.queryText "div p" "2/20"
        Expect.queryNumChildren "div" 1
        return ()
    }

    it "Bind.delay swaps its placeholder for the real view" <| fun () -> promise {
        let app = Html.div [ Bind.delay (Html.p [ text "late" ]) ]

        mountTestApp app
        Expect.areEqual (currentEl.querySelectorAll("div p").length, 0, "the real view is not built synchronously")

        do! Promise.sleep 100

        Expect.queryText "div p" "late"
        Expect.queryNumChildren "div" 1
        return ()
    }

    it "Bind.promises follows the newest promise" <| fun () -> promise {
        let mutable resolve1 : string -> unit = ignore
        let p1 = Promise.create (fun res _ -> resolve1 <- res)
        let mutable resolve2 : string -> unit = ignore
        let p2 = Promise.create (fun res _ -> resolve2 <- res)

        let source = Store.make p1

        let app =
            Html.div [
                Bind.promises (source, (fun (v: string) -> Html.p [ text v ]), Html.span [ text "waiting" ], (fun (x: exn) -> Html.b [ text x.Message ]))
            ]

        mountTestApp app
        Expect.queryText "div span" "waiting"

        resolve1 "first"
        do! Promise.sleep 50
        Expect.queryText "div p" "first"

        Store.set source p2
        Expect.queryText "div span" "waiting"

        resolve2 "second"
        do! Promise.sleep 50
        Expect.queryText "div p" "second"
        Expect.areEqual (currentEl.querySelectorAll("div p").length, 1, "only the newest promise's view is rendered")
        return ()
    }

    it "BindArray.eachs updates an item in place without remounting it" <| fun () -> promise {
        let items = Store.make [| (1, "a"); (2, "b") |]
        let mutable unmounts = 0

        let app =
            Html.div [
                BindArray.eachs (
                    items,
                    (fun (s: IReadOnlyStore<int * string>) ->
                        Html.p [
                            unsubscribeOnUnmount [ fun _ -> unmounts <- unmounts + 1 ]
                            Bind.el (s, fun (_, v) -> text v)
                        ]),
                    fst
                )
            ]

        mountTestApp app
        Expect.queryText "div p:nth-child(1)" "a"
        Expect.queryText "div p:nth-child(2)" "b"
        Expect.areEqual (unmounts, 0, "nothing unmounted on first render")

        // Same keys, changed values: the per-item store updates the existing views.
        Store.set items [| (1, "A"); (2, "B") |]
        Expect.queryText "div p:nth-child(1)" "A"
        Expect.queryText "div p:nth-child(2)" "B"
        Expect.areEqual (unmounts, 0, "changing a value must not remount the item")
        return ()
    }

let init () = ()
