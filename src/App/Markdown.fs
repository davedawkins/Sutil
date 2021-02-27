module Sutil.Markdown

open Sutil.Styling
open Sutil.Attr

let style = [
    rule "h1" [ CssXs.all "revert" ]
    rule "h2" [ CssXs.all "revert" ]
    rule "h3" [ CssXs.all "revert" ]
    rule "h4" [ CssXs.all "revert" ]
    rule "h5" [ CssXs.all "revert" ]
    rule "ul" [ CssXs.all "revert" ]
    rule "li" [ CssXs.all "revert" ]
]

let withMarkdown s =
    s @ style