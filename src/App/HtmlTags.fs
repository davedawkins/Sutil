module HtmlTags

open Sveltish
open Sveltish.DOM

let stringOfHtml = "here's some <strong>HTML!!!</strong>"

let view() = Html.p [
    html stringOfHtml
]