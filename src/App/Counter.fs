module Counter

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

let Counter() =
    let count = Store.make 0
    Html.div [

        Html.div [
            class' "block"
            bind count (fun n -> text $"Counter = {n}")
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
