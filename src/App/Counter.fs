module Counter

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

module GlobalStore =
    // A global store
    // - Never disposed (should not be disposed by Counter, could be disposed by global cleanup though)
    // - Can make multiple Counters (each will share the same value)
    //

    let count = Store.make 0

    let Counter() =
        Html.div [
            Html.div [
                class' "block"
                bind count <| fun n -> text $"Counter = {n}"
            ]

            Html.div [
                class' "block"
                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n-1)) []
                    text "-"
                ]

                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n+1)) []
                    text "+"
                ]
            ]
        ]

module DisposeOnUnmount =
    // - Private counter
    // - Disposed when unmounted
    // - Multiple store dependencies : just declare more Store.make and include them in the disposeOnUmount
    // - Safe when Counter() is used to instantiate new instances
    //   of the component. (Sutil internally promises not to use the same NodeFactory more than once)

    let Counter() =
        let count = Store.make 0

        Html.div [
            disposeOnUnmount [ count ]

            Html.div [
                class' "block"
                bind count <| fun n -> text $"Counter = {n}"
            ]

            Html.div [
                class' "block"
                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n-1)) []
                    text "-"
                ]

                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n+1)) []
                    text "+"
                ]
            ]
        ]



module BindStore =
    //
    // Store is private to Counter
    // Store will be disposed when Counter unmounted
    // Safe management of store
    // Dependency on multiple stores requires chaining bindStores
    // E.g. bindStore 0 <| fun count -> bindStore true -> show -> ...
    //
    let Counter() =
        Html.div [
            bindStore 0 <| fun count -> fragment [
                Html.div [
                    class' "block"
                    bind count <| fun n -> text $"Counter = {n}"
                ]

                Html.div [
                    class' "block"
                    Html.button [
                        onClick (fun _ -> count <~= (fun n -> n-1)) []
                        text "-"
                    ]

                    Html.button [
                        onClick (fun _ -> count <~= (fun n -> n+1)) []
                        text "+"
                    ]
                ]]
        ]

module DeclareResource =
    //
    // Generic resource management
    // Store is private to each instance of Counter
    // Store will be disposed when Counter unmounted
    // Safe: instantiated and cleaned up no matter how NodeFactory is used
    // Works for any type of IDisposable
    //
    //
    let Counter() =
        let mutable count : Store<int> = Unchecked.defaultof<_>

        Html.div [
            declareResource (fun () -> Store.make 0) (fun s -> count <- s)

            Html.div [
                class' "block"
                bind count <| fun n -> text $"Counter = {n}"
            ]

            Html.div [
                class' "block"
                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n-1)) []
                    text "-"
                ]

                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n+1)) []
                    text "+"
                ]
            ]
        ]

let Counter = DeclareResource.Counter