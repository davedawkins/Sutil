module CheckboxInputs

open Sveltish
open Sveltish.DOM
open Sveltish.Attr

let yes = Store.make(false)

let view() =
    Html.div [

        Html.div [
            class' "block"
            Html.label [
                //class' "checkbox"
                Html.input [
                    type' "checkbox"
                    Bindings.bindAttr "checked" yes
                ]
                text " Enable ejector seat"
            ]
        ]

        Html.div [
            class' "block"
            Bindings.showElse yes
                (Html.p  [ text "You are ready for launch" ])
                (Html.p  [ text "You won't be going anywhere unless you enable the ejector seat" ])
        ]

        Html.div [
            class' "block"
            Html.button [
                Bindings.bindAttrIn "disabled" (yes |> Store.map not)
                text "Launch"
            ]
        ]
    ]