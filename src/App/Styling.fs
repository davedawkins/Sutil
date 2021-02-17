module StylingExample

// Adapted from
// https://svelte.dev/examples

open Sutil.Styling
open Sutil.Attr
open Sutil.DOM

let css = [
        rule "p" [
            Css.color "purple"
            Css.fontFamily "'Comic Sans MS', cursive"
            Css.fontSize (Em 2.0)
        ]
    ]

let view() =
    withStyle css <| Html.p [
        text "Styled!"
    ]