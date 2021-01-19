module Nested

open Sutil
open Sutil.DOM

let Nested() =
    Styling.withStyle [] <| Html.p [ text "...don't affect this element" ]

