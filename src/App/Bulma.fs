module Sveltish.Bulma

open Sveltish.Styling
open Sveltish.DOM
open Sveltish.Attr

let style = [
    rule "h1" [ addClass "title"; addClass "is-1" ]
    rule "h2" [ addClass "title"; addClass "is-2" ]
    rule "h3" [ addClass "title"; addClass "is-3" ]
    rule "h4" [ addClass "title"; addClass "is-4" ]
    rule "h5" [ addClass "title"; addClass "is-5" ]
    rule "button" [ addClass "button" ]
]

let withBulma s =
    s @ style