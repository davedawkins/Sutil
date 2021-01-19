module TransitionParameters

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
            text " visible"
        ]
        transition (Both(fly, [ Duration 2000.0; Y 200.0 ])) visible <|
            Html.p [ text "Flies in and out" ]
    ]
