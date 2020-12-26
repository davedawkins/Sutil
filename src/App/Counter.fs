module Counter

open Sveltish
open Sveltish.Bindings
open Sveltish.DOM
open Sveltish.Attr

let Counter() =
    let count = Store.make 0
    Html.div [

        bind count (fun n -> text $"Counter = {n}")

        Html.div [
            Html.button [
                onClick (fun _ -> count <~= (fun n -> n-1))
                text "-"
            ]

            Html.button [
                onClick (fun _ -> count <~= (fun n -> n+1))
                text "+"
            ]
        ]
    ]
