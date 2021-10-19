module BindingTest


open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.DOM

describe "Sutil.Binding" <| fun () ->

  it "Bind counter" <| fun () -> promise {
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
  }

  it "Bind dispose div" <| fun () ->promise {
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
        Expect.areEqual(disposed,1)
  }


  it "Bind disposal nestx2" <| fun () ->promise {
        let storeInner = Store.make 0
        let storeOuter = Store.make 0
        let mutable disposed = 0
        let app =
            Html.div [
                Bind.el(storeOuter, fun n ->
                    Html.div [
                        Bind.el(storeInner, fun n ->
                            Html.div [
                                unsubscribeOnUnmount [ (fun _ -> disposed <- disposed + 1) ]
                                text (n.ToString())
                            ]
                        )
                    ])
            ]

        mountTestApp app

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1)
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1)

        storeOuter |> Store.modify ((+)1)

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1)
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1)
        Expect.areEqual(disposed,1)
  }

  it "Bind disposal nestx3" <| fun () ->promise {
        let storeInner = Store.make 0
        let storeOuter = Store.make 0
        let storeOuter2 = Store.make 0
        let mutable disposed = 0
        let mutable numRenders = 0

        let reset() =
            numRenders <- 0

        let render() =
            numRenders <- numRenders + 1

        let app() =
            Html.div [
                do render()

                Bind.el(storeOuter2, fun n1 ->
                    do render()
                    Html.div [
                        Bind.el(storeOuter, fun n2 ->
                            do render()
                            Html.div [
                                Bind.el(storeInner, fun n3 ->
                                    do render()
                                    Html.div [
                                        unsubscribeOnUnmount [ (fun _ -> disposed <- disposed + 1) ]
                                        text (sprintf "%d %d %d" n1 n2 n3)
                                    ]
                                )
                            ])
                    ])
            ]

        reset()
        mountTestApp (app())

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(disposed,0,"disposed")
        Expect.areEqual(numRenders,4,"numRenders #1")

        reset()
        storeOuter2 |> Store.modify ((+)1)

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(disposed,1,"disposed")
        Expect.areEqual(numRenders,3,"numRenders #2")

        reset()
        storeOuter2 |> Store.modify ((+)1)

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(disposed,2,"disposed")
        Expect.areEqual(numRenders,3,"numRenders #3")

        reset()
        storeOuter |> Store.modify ((+)1)

        Expect.areEqual(storeInner.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(storeOuter.Debugger.NumSubscribers,1,"NumSubscribers")
        Expect.areEqual(disposed,3,"disposed")
        Expect.areEqual(numRenders,2,"numRenders #4")
  }
let init() = ()
