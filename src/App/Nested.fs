module Nested

open Sveltish
open Sveltish.DOM

let Nested() =
    Styling.withStyle [] <| Html.p [ text "...don't affect this element" ]

