module Transition

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
                Bind.attr ("checked",visible)
            ]
            text " visible"
        ]
        transition [InOut fade] visible <|
            Html.p [ text "Fades in and out" ]
    ]
