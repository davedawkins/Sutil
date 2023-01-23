module BindingTest


open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.Core
open Sutil.CoreElements

type Record = {
    Id : int
    Value : string
}

let viewItem (r : Record) =
    Html.div [
        text r.Value
    ]

let viewItemO (r : System.IObservable<Record>) =
    Bind.el( r, viewItem )

describe "Sutil.Binding" <| fun () ->


    it "Doesn't dispose internal state for observable view function" <| fun () -> promise {
        let items = Store.make [|
                { Id = 0; Value = "Apples" }
                { Id = 1; Value = "Oranges" }
                { Id = 2; Value = "Pears" }
            |]

        let app =
            BindArray.each( items, viewItemO, (fun x -> x.Id))

        mountTestApp app

        Expect.queryTextContains "div:nth-child(1)" "Apples"
        Expect.queryTextContains "div:nth-child(2)" "Oranges"
        Expect.queryTextContains "div:nth-child(3)" "Pears"

        items.Update( fun _ -> [|
                { Id = 0; Value = "Bananas" }
                { Id = 1; Value = "Oranges" }
                { Id = 2; Value = "Pears" }
            |]
        )

        Expect.queryTextContains "div:nth-child(1)" "Bananas"
        Expect.queryTextContains "div:nth-child(2)" "Oranges"
        Expect.queryTextContains "div:nth-child(3)" "Pears"


        items.Update( fun _ -> [|
                { Id = 0; Value = "Pineapples" }
                { Id = 1; Value = "Oranges" }
                { Id = 2; Value = "Pears" }
            |]
        )

        Expect.queryTextContains "div:nth-child(1)" "Pineapples"
        Expect.queryTextContains "div:nth-child(2)" "Oranges"
        Expect.queryTextContains "div:nth-child(3)" "Pears"

        return ()
    }

    it "Shows exception if binding fails" <| fun () -> promise {
        let data = Store.make 0
        let app =
            Bind.el( data, fun n -> failwith "expected-exception")
        mountTestApp app
        Expect.queryTextContains "div" "expected-exception"
        return ()
    }

    it "Doesn't dispose items when re-ordering" <| fun () -> promise {
        let sort = Store.make false

        let items = [ 3; 4; 2; 1 ]

        let sortedList useSort =
            items |> List.sortBy (fun n -> if useSort then n else 0)

        let mutable unmountCount = 0
        let app =
            Html.div [
                Bind.each(
                    (sort |> Store.map sortedList),
                    (fun (n : int) ->
                        Html.div [
                            unsubscribeOnUnmount [fun _ -> unmountCount <- unmountCount + 1]
                            text (string n)
                        ]),
                    (fun n -> n)
                )
            ]

        mountTestApp app

        Expect.queryText "div>div:nth-child(1)" "3"
        Expect.queryText "div>div:nth-child(2)" "4"
        Expect.queryText "div>div:nth-child(3)" "2"
        Expect.queryText "div>div:nth-child(4)" "1"

        Expect.areEqual(unmountCount, 0)

        true |> Store.set sort

        Expect.queryText "div>div:nth-child(1)" "1"
        Expect.queryText "div>div:nth-child(2)" "2"
        Expect.queryText "div>div:nth-child(3)" "3"
        Expect.queryText "div>div:nth-child(4)" "4"

        Expect.areEqual(unmountCount, 0)

        return ()
    }

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
