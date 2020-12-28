module StylingExample

open Sveltish
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM

let css = [
        rule "p" [
            color "purple"
            fontFamily "'Comic Sans MS', cursive"
            fontSize "2em"
        ]
    ]

let view() =
    withStyle css <| Html.p [
        text "Styled!"
    ]