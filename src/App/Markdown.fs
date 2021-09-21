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
        Css.custom("-webkit-mask-image", "linear-gradient(to bottom, black 50%, transparent 100%)")
        Css.transition "max-height 0.5s cubic-bezier(0, 1, 0, 1)" // the transition to this state
     ]
    rule "code.full" [
        Css.maxHeight (px 1000)
        Css.transition "max-height 0.5s cubic-bezier(0, 1, 0, 1)" // the transition to this state
        //Css.transition "max-height 1s ease-in-out"
        //Css.transition "-webkit-mask-image 4s ease-in-out"
        //Css.transition "mask-image 4s ease-in-out"
        Css.custom("-webkit-mask-image", "linear-gradient(to bottom, black 100%, black 100%)")
        Css.custom("mask-image", "linear-gradient(to bottom, black 100%, black 100%)")
    ]
    rule "code.full .more-button" [
        Css.displayNone
    ]
]

let withMarkdown s =
    s @ style
