module Counter

open Sveltish
open Sveltish.Bindings
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Stores

let Counter() =
    let count = makeStore 0
    Html.div [

        count |=> fun n -> sprintf "Counter = %d" n |> text

        Html.div [
            Html.button [
                onClick (fun _ -> count <~= (fun n -> n+1))
                text "-"
            ]

            Html.button [
                onClick (fun _ -> count <~= (fun n -> n-1))
                text "+"
            ]
        ]
    ]
