module TransitionParameters

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
            text " visible"
        ]
        transition (Both(fly |> withProps [ Duration 2000.0; Y 200.0 ])) visible <|
            Html.p [ text "Flies in and out" ]
    ]
