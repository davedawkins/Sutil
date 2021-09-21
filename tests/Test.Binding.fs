module Test.Binding

open TestFramework
open Sutil
open Sutil.DOM

let tests = testList "Sutil.Binding" [


  testCase "Bind counter" <| fun () ->
        let store = Store.make 0
        let app =
            Html.div [
                Bind.el(store, Html.div)
            ]

        mountTestApp app

        Expect.queryNumChildren "div" 1
        Expect.queryText "div>div" "0"

        store |> Store.modify ((+)1)

        Expect.queryNumChildren "div" 1
        Expect.queryText "div>div" "1"


  testCase "Bind" <| fun () ->
        let store = Store.make 0
        let mutable disposed = 0
        let app =
            Html.div [
                Bind.el(store, fun n ->
                    Html.div [
                        unsubscribeOnUnmount [ (fun _ -> disposed <- disposed + 1) ]
                        text (n.ToString())
                    ]
                )
            ]

        mountTestApp app

        Expect.queryText "div>div" "0"
        Expect.assertTrue (disposed = 0) "Not yet disposed"

        store |> Store.modify ((+)1)

        Expect.queryText "div>div" "1"
        Expect.areEqual disposed 1



]
