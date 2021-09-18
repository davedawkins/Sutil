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
    rule "code.more" [
        Css.maxHeight (rem 12)
        Css.overflowYHidden
        Css.custom("mask-image", "linear-gradient(to bottom, black 50%, transparent 100%)")
        Css.transition "max-height 0.5s cubic-bezier(0, 1, 0, 1)"
     ]
    rule "code.full" [
        Css.maxHeight (px 1000)
        Css.transition "max-height 1s ease-in-out"
    ]
    rule "code.full .more-button" [
        Css.displayNone
    ]
]

let withMarkdown s =
    s @ style
