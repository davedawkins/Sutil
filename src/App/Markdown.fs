module Sutil.Markdown

open Sutil.Styling
open Sutil.DOM

let style = [
    rule "h1" [ Css.allRevert ]
    rule "h2" [ Css.allRevert ]
    rule "h3" [ Css.allRevert ]
    rule "h4" [ Css.allRevert ]
    rule "h5" [ Css.allRevert ]
    rule "ul" [ Css.allRevert ]
    rule "li" [ Css.allRevert ]
]

let withMarkdown s =
    s @ style