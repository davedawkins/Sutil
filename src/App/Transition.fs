module Transition

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Transition

let visible = Store.make true

let view() =
    Html.div [
        Html.label [
            Html.input [
                type' "checkbox"
                bindAttr "checked" visible
            ]
            text "visible"
        ]
        transition (Both(fade, [])) visible <|
            Html.p [ text "Fades in and out" ]
    ]