module LogicElseIf

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

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