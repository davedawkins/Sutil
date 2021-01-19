module Sutil.Markdown

open Sutil.Styling
open Sutil.DOM
open Sutil.Attr

let style = [
    rule "h1" [ all "revert" ]
    rule "h2" [ all "revert" ]
    rule "h3" [ all "revert" ]
    rule "h4" [ all "revert" ]
    rule "h5" [ all "revert" ]
    rule "ul" [ all "revert" ]
    rule "li" [ all "revert" ]
]

let withMarkdown s =
    s @ style