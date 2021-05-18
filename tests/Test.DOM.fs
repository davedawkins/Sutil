module Test.DOM

open TestFramework
open Sutil
open Sutil.DOM

let tests = testList "Sutil.DOM" [

    // Simplest case
    testCase "Hello World" <| fun () ->
        let app =
            Html.div "Hello World"

        mountTestApp app

        Expect.queryText "div" "Hello World"

    // Basic fragment
    testCase "Fragment" <| fun () ->
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

    // Basic fragment
    testCase "Adjacent Fragments" <| fun () ->
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

    testCaseP "Delay" <| fun () ->
        promise {
            let app1 = Html.div "Delay: Waiting"
            let app2 = Html.div "Delay: Done"
            mountTestApp app1
            do! Promise.sleep(40)
            mountTestApp app2
        }

    testCaseP "Animation frame" <| fun () ->
        promise {
            let app1 = Html.div "Frame: Waiting"
            let app2 = Html.div "Frame: Done"
            mountTestApp app1
            for t in [1..1] do
                do! waitAnimationFrame()
            mountTestApp app2
        }

    testCase "Binding" <| fun () ->
        let store = Store.make 0
        let app =
            fragment [
                Bind.fragment store <| fun n ->
                    Html.div (string n)
            ]
        mountTestApp app
        Expect.queryText "div" "0"
        store |> Store.modify ((+)1)
        Expect.queryText "div" "1"


    testCase "Consecutive Bindings" <| fun () ->
        let store1 = Store.make 10
        let store2 = Store.make 20
        let app =
            fragment [
                Bind.fragment store1 <| fun n ->
                    Html.div (string n)
                Bind.fragment store2 <| fun n ->
                    Html.div (string n)
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
//    ]

//let tests = testList "Sutil.DOM" [

    testCase "Consecutive Binding Fragments" <| fun () ->
        let store1 = Store.make 10
        let store2 = Store.make 20
        let app =
            Html.div [
                Bind.fragment store1 <| fun n ->
                    fragment [
                        Html.div "Binding 1"
                        Html.div (string n)
                    ]
                Bind.fragment store2 <| fun n ->
                    fragment [
                        Html.div "Binding 2"
                        Html.div (string n)
                    ]
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

    testCase "Each" <| fun () ->
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                each store1 (fun item -> Html.div item) []
                Html.div "Footer"
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "Footer"

        store1 |> Store.modify (cons "A")

        Expect.queryText "div>div:nth-child(1)" "Header"
        Expect.queryText "div>div:nth-child(2)" "A"
        Expect.queryText "div>div:nth-child(3)" "Footer"

//]

//let tests = testList "Sutil.DOM" [

    testCase "Consecutive Each" <| fun () ->
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                each store1 (fun item -> Html.div item) []
                each store2 (fun item -> Html.div item) []
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

    testCase "Consecutive Each Update Sequence #2" <| fun () ->
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                each store1 (fun item -> Html.div item) []
                each store2 (fun item -> Html.div item) []
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

    testCase "Separated each" <| fun () ->
        let cons x xs = x :: xs
        let store1 = Store.make ([ ] : string list)
        let store2 = Store.make ([ ] : string list)
        let app =
            Html.div [
                Html.div "Header"
                each store1 (fun item -> Html.div item) []
                Html.div "Middle"
                each store2 (fun item -> Html.div item) []
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

    //testCaseP "400ms" <| fun () ->
    //    promise {
    //        do! Promise.sleep(400)
    //    }


]
