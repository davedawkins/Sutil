module Sutil.Markdown

open Sutil.Styling
open Sutil.DOM
open Sutil.Attr

let style = [
    rule "h1" [ Css.all "revert" ]
    rule "h2" [ Css.all "revert" ]
    rule "h3" [ Css.all "revert" ]
    rule "h4" [ Css.all "revert" ]
    rule "h5" [ Css.all "revert" ]
    rule "ul" [ Css.all "revert" ]
    rule "li" [ Css.all "revert" ]
]

let withMarkdown s =
    s @ style