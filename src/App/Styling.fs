module Styling

open Sveltish
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM

let css = [
        rule "p" [
            color "purple"
            fontFamily "Comic Sans MS', cursive"
            fontSize "2em"
        ]
    ]

let view() =
    style css <| Html.p [
        text "Styled!"
    ]