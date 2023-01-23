module HtmlTags

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Core
open Sutil.CoreElements

let stringOfHtml = "here's some <strong>HTML!!!</strong>"

let view() = Html.p [
    Html.parse stringOfHtml
]
