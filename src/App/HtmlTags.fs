module HtmlTags

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM

let stringOfHtml = "here's some <strong>HTML!!!</strong>"

let view() = Html.p [
    html stringOfHtml
]