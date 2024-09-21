module DOMTest

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.Core
open Sutil.CoreElements
open Sutil.DomHelpers

let log s = Fable.Core.JS.console.log s

open Fable.Core.JsInterop

describe "DOM" <| fun () ->

    // Simplest case
    it "Hello World" <| fun () -> promise {
        let app =
            Html.div "Hello World"

        mountTestApp app

        Expect.queryText "div" "Hello World"
    }

    // Mount
    it "Mount is called once" <| fun () -> promise {
        let counters = Array.zeroCreate 6

        let countMount i opts =
            onMount (fun e -> counters.[i] <- counters.[i] + 1; log(sprintf "mount %d: %s" i (DomHelpers.nodeStrShort (!!e.target)))) opts

        let app =
            Html.div [
                countMount 0 []
                Html.h1 [
                    countMount 1 []
                    fragment [ text "Hello" ]
                ]
                fragment [
                    countMount 2 [ ] //  target will be <div> (parent)
                    Html.p [
                        countMount 3 []
                    ]
                    Html.span [
                        countMount 4 []
                    ]
                ]
                fragment [
                    countMount 5 [] // target will be <div> (parent)
                ]
            ]

        mountTestApp app

        Expect.areEqual(counters.[0],1)
        Expect.areEqual(counters.[1],1)
        Expect.areEqual(counters.[2],1)
        Expect.areEqual(counters.[3],1)
        Expect.areEqual(counters.[4],1)
        Expect.areEqual(counters.[5],1)
    }



    // Basic fragment
    it "Fragment" <| fun () -> promise {
        let app =
            Html.div [
                Html.div "Header"
                fragment [
                    Html.div "Body"
                ]
                Html.div "Footer"
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Body"
        Expect.queryText "div>div:nth-child(3)" "Footer"
    }

    // Basic fragment
    it "Adjacent Fragments" <| fun () -> promise {
        let app =
            Html.div [
                fragment [
                    Html.h2 "Section 1"
                    Html.div "Item 1.a"
                ]
                fragment [
                    Html.h2 "Section 2"
                    Html.div "Item 2.a"
                ]
            ]

        mountTestApp app

        Expect.queryText "div>h2:nth-child(1)" "Section 1"
        Expect.queryText "div>div:nth-child(2)" "Item 1.a"
        Expect.queryText "div>h2:nth-child(3)" "Section 2"
        Expect.queryText "div>div:nth-child(4)" "Item 2.a"
    }

    it "Delay" <| fun () -> promise {
        let app1 = Html.div "Delay: Waiting"
        let app2 = Html.div "Delay: Done"
        mountTestApp app1
        do! Promise.sleep(40)
        mountTestApp app2
    }

    it "Animation frame" <| fun () -> promise {
        let app1 = Html.div "Frame: Waiting"
        let app2 = Html.div "Frame: Done"
        mountTestApp app1
        for t in [1..1] do
            do! BrowserFramework.waitAnimationFrame()
        mountTestApp app2
    }

    it "Binding" <| fun () -> promise {
        let store = Store.make 0
        let app =
            fragment [
                Bind.el(store, Html.div)
            ]
        mountTestApp app
        Expect.queryText "div" "0"
        store |> Store.modify ((+)1)
        Expect.queryText "div" "1"
    }

    it "Consecutive Bindings" <| fun () -> promise {
        let store1 = Store.make 10
        let store2 = Store.make 20
        let app =
            fragment [
                Bind.el(store1, Html.div)
                Bind.el(store2, Html.div)
            ]
        mountTestApp app

        Expect.queryText "div:nth-child(1)" "10"
        Expect.queryText "div:nth-child(2)" "20"

        store1 |> Store.modify ((+)1)

        Expect.queryText "div:nth-child(1)" "11"
        Expect.queryText "div:nth-child(2)" "20"

        store2 |> Store.modify ((+)1)

        Expect.queryText "div:nth-child(1)" "11"
        Expect.queryText "div:nth-child(2)" "21"
    }

    it "Consecutive Binding Fragments" <| fun () -> promise {
        let store1 = Store.make 10
        let store2 = Store.make 20
        let app =
            Html.div [
                Bind.el(store1,fun n ->
                    fragment [
                        Html.div "Binding 1"
                        Html.div (string n)
                    ])
                Bind.el(store2,fun n ->
                    fragment [
                        Html.div "Binding 2"
                        Html.div (string n)
                    ])
            ]
        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Binding 1"
        Expect.queryText "div>div:nth-child(2)" "10"
        Expect.queryText "div>div:nth-child(3)" "Binding 2"
        Expect.queryText "div>div:nth-child(4)" "20"

        store1 |> Store.modify ((+)1)

        Expect.queryText "div>div:nth-child(1)" "Binding 1"
        Expect.queryText "div>div:nth-child(2)" "11"
        Expect.queryText "div>div:nth-child(3)" "Binding 2"
        Expect.queryText "div>div:nth-child(4)" "20"

        store2 |> Store.modify ((+)1)

        Expect.queryText "div>div:nth-child(1)" "Binding 1"
        Expect.queryText "div>div:nth-child(2)" "11"
        Expect.queryText "div>div:nth-child(3)" "Binding 2"
        Expect.queryText "div>div:nth-child(4)" "21"
    }

    it "Each" <| fun () -> promise {
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                Bind.each(store1, Html.div)
                Html.div "Footer"
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Footer"

        store1 |> Store.modify (cons "A")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Footer"
    }


    it "Consecutive Each" <| fun () -> promise {
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                Bind.each(store1, Html.div)
                Bind.each(store2, Html.div)
                Html.div "Footer"
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Footer"

        store1 |> Store.modify (cons "A")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Footer"

        store1 |> Store.modify (cons "B")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "Footer"

        store2 |> Store.modify (cons "X")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "X"
        Expect.queryText "div>div:nth-child(5)" "Footer"

        store2 |> Store.modify (cons "Y")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "Y"
        Expect.queryText "div>div:nth-child(5)" "X"
        Expect.queryText "div>div:nth-child(6)" "Footer"
    }

    it "Consecutive Each Update Sequence #2" <| fun () -> promise {
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                Bind.each(store1, Html.div)
                Bind.each(store2, Html.div)
                Html.div "Footer"
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Footer"

        store1 |> Store.modify (cons "A")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Footer"

        store2 |> Store.modify (cons "X")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "X"
        Expect.queryText "div>div:nth-child(4)" "Footer"

        store1 |> Store.modify (cons "B")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "X"
        Expect.queryText "div>div:nth-child(5)" "Footer"

        store2 |> Store.modify (cons "Y")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "Y"
        Expect.queryText "div>div:nth-child(5)" "X"
        Expect.queryText "div>div:nth-child(6)" "Footer"
    }

    it "Separated each" <| fun () -> promise {
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                Bind.each(store1, Html.div)
                Html.div "Middle"
                Bind.each(store2, Html.div)
                Html.div "Footer"
            ]

        mountTestApp app
        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Middle"
        Expect.queryText "div>div:nth-child(3)" "Footer"

        store1 |> Store.modify (cons "A")
        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Middle"
        Expect.queryText "div>div:nth-child(4)" "Footer"

        store2 |> Store.modify (cons "X")
        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Middle"
        Expect.queryText "div>div:nth-child(4)" "X"
        Expect.queryText "div>div:nth-child(5)" "Footer"

        store1 |> Store.modify (cons "B")
        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "Middle"
        Expect.queryText "div>div:nth-child(5)" "X"
        Expect.queryText "div>div:nth-child(6)" "Footer"

        store2 |> Store.modify (cons "Y")
        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "B"
        Expect.queryText "div>div:nth-child(3)" "A"
        Expect.queryText "div>div:nth-child(4)" "Middle"
        Expect.queryText "div>div:nth-child(5)" "Y"
        Expect.queryText "div>div:nth-child(6)" "X"
        Expect.queryText "div>div:nth-child(7)" "Footer"
    }

    it "disposes conditional div" <| fun _ -> promise {
        let switch = Store.make true
        let mutable disposed = false
        let mutable unsubbed = false

        let app =
            Bind.el( switch, fun flag ->
                if flag then
                    Html.div [
                        unsubscribeOnUnmount [
                            (fun _ -> unsubbed <- true)
                        ]
                        disposeOnUnmount [
                         { new System.IDisposable with member _.Dispose() = disposed <- true }
                        ]
                    ]
                else
                    fragment []
            )

        mountTestApp app

        Expect.areEqual( disposed, false )
        Expect.areEqual( unsubbed, false )

        switch |> Store.modify not

        Expect.areEqual( disposed, true )
        Expect.areEqual( unsubbed, true )

        return ()
    }

    it "cleans up div"<| fun _ -> promise {
        let mutable node1 = Unchecked.defaultof<_>
        let mutable node2 = Unchecked.defaultof<_>
        let mutable node2_unsubbed = false

        let app =
            Html.div [
                hookParent (fun n -> node1 <- n)

                Html.div [
                    hookParent (fun n -> node2 <- n)
                    text "node2"
                    unsubscribeOnUnmount [
                        (fun _ -> node2_unsubbed <- true)
                    ]
                ]
            ]

        mountTestApp app

        Expect.queryText "div" "node2"
        Expect.queryText "div>div" "node2"

        DomEdit.removeChild node1 node2

        Expect.queryText "div" ""
        Expect.areEqual(node2_unsubbed,true)

        return ()
    }

    it "cleans up fragment"<| fun _ -> promise {
        let mutable node1 = Unchecked.defaultof<_>
        let mutable node2 = Unchecked.defaultof<_>
        let mutable node2_fragment_unsubbed = 0

        let app =
            Html.div [
                hookParent (fun n -> node1 <- n)

                Html.div [
                    hookParent (fun n -> node2 <- n)
                    fragment [
                        text "node2"
                        unsubscribeOnUnmount [
                            (fun _ -> node2_fragment_unsubbed <- node2_fragment_unsubbed + 1)
                        ]
                    ]
                ]
            ]

        mountTestApp app

        Expect.queryText "div" "node2"
        Expect.queryText "div>div" "node2"

        DomEdit.removeChild node1 node2

        Expect.queryText "div" ""
        Expect.areEqual(node2_fragment_unsubbed,1)

        return ()
    }

    it "cleans up fragment"<| fun _ -> promise {
        let mutable node1 = Unchecked.defaultof<_>
        let mutable node2 = Unchecked.defaultof<_>
        let mutable node2_fragment_unsubbed = 0

        let app =
            Html.div [
                hookParent (fun n -> node1 <- n)

                Html.div [
                    hookParent (fun n -> node2 <- n)
                    fragment [
                        text "node2"
                        unsubscribeOnUnmount [
                            (fun _ -> node2_fragment_unsubbed <- node2_fragment_unsubbed + 1)
                        ]
                    ]
                ]
            ]

        mountTestApp app

        Expect.queryText "div" "node2"
        Expect.queryText "div>div" "node2"

        DomEdit.removeChild node1 node2

        Expect.queryText "div" ""
        Expect.areEqual(node2_fragment_unsubbed,1)

        return ()
    }

    it "disposes conditional fragment" <| fun _ -> promise {
        let switch = Store.make true
        let mutable disposed = false
        let mutable unsubbed = false

        let app =
            Bind.el( switch, fun flag ->
                if flag then
                    fragment [
                        text "frag001"
                        unsubscribeOnUnmount [
                            (fun _ -> unsubbed <- true)
                        ]
                        disposeOnUnmount [
                        { new System.IDisposable with member _.Dispose() = disposed <- true }
                        ]
                    ]
                else
                    fragment []
            )

        mountTestApp app

        Expect.areEqual(Expect.getInnerText(), "frag001")
        Expect.areEqual( disposed, false )
        Expect.areEqual( unsubbed, false )

        switch |> Store.modify not

        Expect.areEqual(Expect.getInnerText(), "")
        Expect.areEqual( disposed, true )
        Expect.areEqual( unsubbed, true )

        return ()
    }

    // Issue #91
    it "input readonly=false is writable" <| fun _ -> promise {
        let mutable readOnly = true

        let app =
            Html.input [
                Attr.readOnly false
                Ev.onMount( fun e ->
                    let ipEl = e.target :?> Browser.Types.HTMLInputElement
                    readOnly <- ipEl.readOnly
                )
            ]

        mountTestApp app

        Expect.assertFalse readOnly "input should not be readonly"

        return ()
    }

    it "input readonly=true is read-only" <| fun _ -> promise {
        let mutable readOnly = false

        let app =
            Html.input [
                Attr.readOnly true
                Ev.onMount( fun e ->
                    let ipEl = e.target :?> Browser.Types.HTMLInputElement
                    readOnly <- ipEl.readOnly
                )
            ]

        mountTestApp app

        Expect.assertTrue readOnly "input should be readonly"

        return ()
    }


    //testCaseP "400ms" <| fun () ->
    //    promise {
    //        do! Promise.sleep(400)
    //    }

let init() =
    ()
