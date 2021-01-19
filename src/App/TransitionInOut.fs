module TransitionInOut

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

        let flyIn = (fly, [ Duration 2000.0; Y 200.0 ])
        let fadeOut = (fade, [])

        transition (InOut(flyIn, fadeOut)) visible <|
            Html.p [ text "Flies in and fades out" ]
    ]
