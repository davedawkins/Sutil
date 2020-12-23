module Nested

open Sveltish
open Sveltish.DOM

let Nested() =
    Styling.style [] <| Html.p [ text "...don't affect this element" ]

