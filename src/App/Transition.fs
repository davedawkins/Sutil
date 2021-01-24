module Transition

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition


let view() =
    let visible = Store.make true

    Html.div [
        disposeOnUnmount [visible]

        Html.label [
            Html.input [
                type' "checkbox"
                bindAttr "checked" visible
            ]
            text " visible"
        ]
        transition [InOut fade] visible <|
            Html.p [ text "Fades in and out" ]
    ]