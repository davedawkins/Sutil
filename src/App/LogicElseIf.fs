module LogicElseIf

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM

let x = 7;

let view() =
    Html.div [
        if x > 10 then
            Html.p [ text $"{x} is greater than 10" ]
        else if 5 > x then
            Html.p [ text $"{x} is less than 5" ]
        else
            Html.p [ text $"{x} is between 5 and 10" ]
    ]