module Sutil.Markdown

open Sutil.Styling
open Sutil.DOM

open type Feliz.length
let style = [
    rule "h1" [ Css.allRevert ]
    rule "h2" [ Css.allRevert ]
    rule "h3" [ Css.allRevert ]
    rule "h4" [ Css.allRevert ]
    rule "h5" [ Css.allRevert ]
    rule "ul" [ Css.allRevert ]
    rule "li" [ Css.allRevert ]
    rule "p" [ Css.marginTop (em 1); Css.marginBottom (em 1)]
]

let withMarkdown s =
    s @ style
