module NestedComponents

open Sveltish
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM
open Nested

let css = [
        rule "p" [
            color "orange"
            fontFamily "'Comic Sans MS', cursive"
            fontSize "2em"
        ]
    ]

let view() =
    style css <| Html.div [
        Html.p [
            text "These styles..."
        ]
        Nested()
    ]
