module Transition

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition

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