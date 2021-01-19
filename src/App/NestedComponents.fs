module NestedComponents

open Sutil
open Sutil.Styling
open Sutil.Attr
open Sutil.DOM
open Nested

let css = [
        rule "p" [
            color "orange"
            fontFamily "'Comic Sans MS', cursive"
            fontSize "2em"
        ]
    ]

let view() =
    withStyle css <| Html.div [
        Html.p [
            text "These styles..."
        ]
        Nested()
    ]
