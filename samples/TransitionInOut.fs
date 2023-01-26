module TransitionInOut

// Adapted from
// https://svelte.dev/examples

open Sutil

open Sutil.Core
open Sutil.CoreElements
open Sutil.Transition

let view() =
    let visible = Store.make true

    Html.div [
        disposeOnUnmount [visible]

        Html.label [
            Html.input [
                type' "checkbox"
                Bind.attr ("checked", visible)
            ]
            text " visible"
        ]

        let flyIn = fly |> withProps [ Duration 2000.0; Y 200.0 ]

        transition [ In flyIn; Out fade ] visible <|
            Html.p [ text "Flies in and fades out" ]
    ]

view() |> Program.mountElement "sutil-app"
