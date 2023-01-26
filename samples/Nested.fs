module Nested

// Adapted from
// https://svelte.dev/examples

open Sutil.Html
open Sutil.Core
open Sutil.CoreElements
open Sutil.Styling

let Nested() =
   Html.p [ text "...don't affect this element" ] |> withStyle []

