module AnchorTest

// Tests for the comment-anchor binding model (fsimgo #896). The red-first case is
// "last-child binding keeps a constant element count", which fails on the group
// implementation because ReplaceChild appends when insertBefore is null (fsimgo #895).

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.Core
open Sutil.CoreElements
open System
open Browser.CssExtensions

// An observable that records whether its subscription was disposed.
type private TrackedObservable<'T>(source: IObservable<'T>) =
    member val Unsubscribed = false with get, set

    interface IObservable<'T> with
        member this.Subscribe(observer) =
            let d = source.Subscribe(observer)

            { new IDisposable with
                member _.Dispose() =
                    this.Unsubscribed <- true
                    d.Dispose()
            }

let private findBindAnchor (parent: Browser.Types.Node) : Browser.Types.Node =
    DomHelpers.children parent
    |> Seq.tryFind (fun n -> DomHelpers.isCommentNode n && n.textContent = "bind")
    |> Option.defaultValue null

describe "Sutil.Anchor" <| fun () ->

    it "last-child binding keeps a constant element count across rebuilds" <| fun () -> promise {
        // fsimgo #895 stated positively: the bind is the last child of its parent, and
        // repeated store updates must not grow the child list.
        let store = Store.make 0

        let app =
            Html.div [
                Html.span [ text "first" ]
                Bind.el (store, fun n -> Html.div [ text (string n) ])
            ]

        mountTestApp app

        Expect.queryNumChildren "div" 2
        let baselineNodes = (currentEl.querySelector "div").childNodes.length

        for _ in 1..3 do
            store |> Store.modify ((+) 1)

        // Comments and text count too: no node of any kind may accumulate (fsimgo #895).
        Expect.queryNumChildren "div" 2
        Expect.areEqual ((currentEl.querySelector "div").childNodes.length, baselineNodes)
        Expect.queryText "div div" "3"
        return ()
    }

    it "a timed fade completes and hides the node" <| fun () -> promise {
        let visible = Store.make true

        let app =
            Html.div [
                Transition.transition
                    [ InOut (Transition.withProps [ Duration 40.0 ] TransitionFunctions.fade) ]
                    visible
                    (Html.p [ text "fady" ])
            ]

        // Animations only run on attached elements, so this test mounts into document.body.
        let host = Browser.Dom.document.createElement "div"
        Browser.Dom.document.body.appendChild host |> ignore
        currentEl <- host
        let mounted = Sutil.Program.mount (host, app)

        let display () =
            (host.querySelector ("p") :?> Browser.Types.HTMLElement).style.display

        Expect.areEqual (display (), "")

        Store.set visible false
        do! Promise.sleep 500
        Expect.areEqual (display (), "none")

        Store.set visible true
        do! Promise.sleep 500
        Expect.areEqual (display (), "")

        mounted.Dispose()
        Browser.Dom.document.body.removeChild host |> ignore
        return ()
    }

    it "binding that renders nothing holds its position between static siblings" <| fun () -> promise {
        let store = Store.make (Some "middle")

        let app =
            Html.div [
                Html.span [ text "A" ]
                Bind.el (
                    store,
                    fun v ->
                        match v with
                        | Some s -> Html.p [ text s ]
                        | None -> fragment []
                )
                Html.span [ text "B" ]
            ]

        mountTestApp app

        Expect.queryText "div :nth-child(1)" "A"
        Expect.queryText "div :nth-child(2)" "middle"
        Expect.queryText "div :nth-child(3)" "B"

        Store.set store None
        Expect.queryNumChildren "div" 2

        Store.set store (Some "back")
        Expect.queryText "div :nth-child(1)" "A"
        Expect.queryText "div :nth-child(2)" "back"
        Expect.queryText "div :nth-child(3)" "B"
        return ()
    }

    it "multi-root binding replaces all of its nodes on the next value" <| fun () -> promise {
        let store = Store.make 1

        let app =
            Html.div [
                Bind.el (
                    store,
                    fun n ->
                        fragment [
                            Html.p [ text (sprintf "%da" n) ]
                            Html.p [ text (sprintf "%db" n) ]
                        ]
                )
            ]

        mountTestApp app

        Expect.queryNumChildren "div" 2
        Expect.queryText "div :nth-child(1)" "1a"
        Expect.queryText "div :nth-child(2)" "1b"

        Store.set store 2

        Expect.queryNumChildren "div" 2
        Expect.queryText "div :nth-child(1)" "2a"
        Expect.queryText "div :nth-child(2)" "2b"
        return ()
    }

    it "removing a binding's anchor disposes its subscription" <| fun () -> promise {
        let store = Store.make 5
        let tracked = TrackedObservable(store)

        let app = Html.div [ Bind.el (tracked, fun n -> Html.p [ text (string n) ]) ]

        mountTestApp app

        Expect.queryText "div p" "5"
        Expect.areEqual (tracked.Unsubscribed, false)

        let host = currentEl.querySelector ("div")
        let anchor = findBindAnchor host
        Expect.assertTrue (not (isNull anchor)) "bind anchor exists in the DOM"

        DomHelpers.removeNode anchor

        Expect.areEqual (tracked.Unsubscribed, true)
        Expect.queryNumChildren "div" 0
        return ()
    }

    it "unmounting an ancestor disposes a nested binding's subscription" <| fun () -> promise {
        let store = Store.make 5
        let tracked = TrackedObservable(store)

        let app = Html.div [ Html.div [ Bind.el (tracked, fun n -> Html.p [ text (string n) ]) ] ]

        mountTestApp app
        Expect.areEqual (tracked.Unsubscribed, false)

        DomHelpers.clear currentEl

        Expect.areEqual (tracked.Unsubscribed, true)
        return ()
    }

    it "outer rebuild disposes an inner binding rooted at the content top level" <| fun () -> promise {
        let switch = Store.make true
        let store = Store.make 5
        let tracked = TrackedObservable(store)

        let app =
            Html.div [
                Bind.el (
                    switch,
                    fun flag ->
                        if flag then
                            Bind.el (tracked, fun n -> Html.p [ text (string n) ])
                        else
                            fragment []
                )
            ]

        mountTestApp app

        Expect.queryText "div p" "5"
        Expect.areEqual (tracked.Unsubscribed, false)

        switch |> Store.modify not

        Expect.areEqual (tracked.Unsubscribed, true)
        Expect.queryNumChildren "div" 0
        return ()
    }

    it "a foreign wipe of one binding's region leaves sibling subscribers alive" <| fun () -> promise {
        // Red-team finding: destroying an anchor with non-Sutil DOM code must not turn
        // later store updates into exceptions that starve the store's other subscribers.
        let store = Store.make 0

        let app =
            Html.div [
                Html.div [ Attr.id "regionA"; Bind.el (store, fun n -> Html.p [ text (sprintf "A%d" n) ]) ]
                Html.div [ Attr.id "regionB"; Bind.el (store, fun n -> Html.p [ text (sprintf "B%d" n) ]) ]
            ]

        mountTestApp app

        Expect.queryText "#regionA p" "A0"
        Expect.queryText "#regionB p" "B0"

        (currentEl.querySelector ("#regionA") :?> Browser.Types.HTMLElement).innerHTML <- ""

        Store.set store 1
        Expect.queryText "#regionB p" "B1"

        Store.set store 2
        Expect.queryText "#regionB p" "B2"
        return ()
    }

    it "showIf works over fragment-rooted content" <| fun () -> promise {
        let visible = Store.make true

        let app = Html.div [ Transition.showIf visible (fragment [ Html.p [ text "frag" ] ]) ]

        mountTestApp app

        let display () =
            (currentEl.querySelector ("p") :?> Browser.Types.HTMLElement).style.display

        Expect.queryText "div p" "frag"
        Expect.areEqual (display (), "")

        Store.set visible false
        Expect.areEqual (display (), "none")

        Store.set visible true
        Expect.areEqual (display (), "")
        return ()
    }

    it "each renders a single-child fragment item root without an error node" <| fun () -> promise {
        let items = Store.make [| 1; 2 |]

        let app =
            Html.div [
                BindArray.each (items, (fun (n: int) -> fragment [ Html.p [ text (string n) ] ]), (fun (n: int) -> n))
            ]

        mountTestApp app

        Expect.assertFalse (Expect.getInnerText().Contains("sutil-error")) "no error node rendered"
        Expect.queryText "div :nth-child(1)" "1"
        Expect.queryText "div :nth-child(2)" "2"
        return ()
    }

    it "showIf toggles visibility both ways on the built node" <| fun () -> promise {
        let visible = Store.make true

        let app = Html.div [ Transition.showIf visible (Html.p [ text "content" ]) ]

        mountTestApp app

        let isHidden () =
            let p = currentEl.querySelector ("p") :?> Browser.Types.HTMLElement
            Expect.assertTrue (not (isNull p)) "p stays in the DOM"
            p.style.display = "none"

        Expect.areEqual (isHidden (), false)

        Store.set visible false
        Expect.areEqual (isHidden (), true)

        Store.set visible true
        Expect.areEqual (isHidden (), false)
        return ()
    }

    it "showIfElse alternates between the two branches" <| fun () -> promise {
        let flag = Store.make true

        let app =
            Html.div [
                Transition.showIfElse
                    flag
                    (Html.p [ Attr.id "yes"; text "yes" ])
                    (Html.p [ Attr.id "no"; text "no" ])
            ]

        mountTestApp app

        let displayOf id =
            (currentEl.querySelector ("#" + id) :?> Browser.Types.HTMLElement).style.display

        Expect.areEqual (displayOf "yes", "")
        Expect.areEqual (displayOf "no", "none")

        Store.set flag false
        Expect.areEqual (displayOf "yes", "none")
        Expect.areEqual (displayOf "no", "")

        Store.set flag true
        Expect.areEqual (displayOf "yes", "")
        Expect.areEqual (displayOf "no", "none")
        return ()
    }

    it "transitionList over three predicates shows exactly the matching node" <| fun () -> promise {
        let sel = Store.make 0

        let option (id: string) (label: string) = Html.p [ Attr.id id; text label ]

        let app =
            Html.div [
                Transition.transitionMatch sel [
                    (=) 0, option "p0" "zero", []
                    (=) 1, option "p1" "one", []
                    (=) 2, option "p2" "two", []
                ]
            ]

        mountTestApp app

        let displayOf id =
            match currentEl.querySelector ("#" + id) with
            | null -> "unbuilt"
            | el -> (el :?> Browser.Types.HTMLElement).style.display

        Expect.areEqual (displayOf "p0", "")
        Expect.areEqual (displayOf "p1", "none")
        Expect.areEqual (displayOf "p2", "none")

        Store.set sel 1
        Expect.areEqual (displayOf "p0", "none")
        Expect.areEqual (displayOf "p1", "")
        Expect.areEqual (displayOf "p2", "none")

        Store.set sel 2
        Expect.areEqual (displayOf "p0", "none")
        Expect.areEqual (displayOf "p1", "none")
        Expect.areEqual (displayOf "p2", "")
        return ()
    }
