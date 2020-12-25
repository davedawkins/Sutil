module CheckboxInputs

open Sveltish
open Sveltish.Attr
open Sveltish.DOM

let yes = Store.make(true)

let view() =
    Html.div [
        Html.label [
            Html.input [
                attr ("type","checkbox")
                Bindings.bindAttrBoth "checked" yes
            ]
            text "Yes! Send me regular email spam"
        ]

        Bindings.showElse yes
            (Html.p  [ text "Thank you. We will bombard your inbox and sell your personal details." ])
            (Html.p  [ text "You must opt in to continue. If you're not paying, you're the product." ])

        Html.button [
            Bindings.bindAttrIn "disabled" (yes |> Store.map not)
            text "Subscribe"
        ]
    ]