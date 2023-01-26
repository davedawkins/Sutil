module CheckboxInputs

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Core
open Sutil.CoreElements

open Sutil.Transition

let yes = Store.make(false)

let view() =
    Html.div [

        Html.div [
            class' "block"
            Html.label [
                Html.input [
                    type' "checkbox"
                    Bind.attr ("checked",yes)
                ]
                text " Enable ejector seat"
            ]
        ]

        Html.div [
            class' "block"
            showIfElse yes
                (Html.p  [ text "You are ready for launch" ])
                (Html.p  [ text "You won't be going anywhere unless you enable the ejector seat" ])
        ]

        Html.div [
            class' "block"
            Html.button [
                Bind.attr ("disabled", yes |> Store.map not)
                text "Launch"
            ]
        ]
    ]
view() |> Program.mountElement "sutil-app"
