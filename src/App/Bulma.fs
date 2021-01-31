module Sutil.Bulma

open Sutil.Styling
open Sutil.DOM
open Sutil.Attr

let styleHelpers = [
    rule "h1" [ addClass "title"; addClass "is-1" ]
    rule "h2" [ addClass "title"; addClass "is-2" ]
    rule "h3" [ addClass "title"; addClass "is-3" ]
    rule "h4" [ addClass "title"; addClass "is-4" ]
    rule "h5" [ addClass "title"; addClass "is-5" ]
    rule "button" [ addClass "button" ]

    rule "input[type='file']" [ addClass "file-cta" ]

    rule "input[type='text']" [
        addClass "input"
    ]

    rule "input[type='radio']" [
        addClass "radio"
    ]

    rule "input[type='checkbox']" [
        addClass "checkbox"
    ]

    rule "input[type='number']" [
        addClass "input"
        addClass "is-small"
        Css.maxWidth "50%"
    ]

    rule "input[type='range']" [
        addClass "input"
        addClass "is-small"
        Css.maxWidth "50%"
    ]
]

let withBulmaHelpers s =
    s @ styleHelpers