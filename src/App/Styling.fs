module StylingExample

open Sutil
open Sutil.Styling
open Sutil.Attr
open Sutil.DOM

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