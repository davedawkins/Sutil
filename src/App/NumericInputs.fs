module NumericInputs

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM
open Sutil.Attr

let view() =
    let a = Store.make(1)
    let b = Store.make(2)

    Html.div [
        disposeOnUnmount [ a; b ]

        Html.div [
            class' "block"
            Html.input [
                type' "number"
                Bindings.bindAttr "value" a
                min "0"
                max "10"
            ]
            Html.input [
                type' "range"
                Bindings.bindAttr "value" a
                min "0"
                max "10"
            ]
        ]
        Html.div [
            class' "block"
            Html.input [
                type' "number"
                Bindings.bindAttr "value" b
                min "0"
                max "10"
            ]
            Html.input [
                type' "range"
                Bindings.bindAttr "value" b
                min "0"
                max "10"
            ]
        ]
        Html.p [
            class' "block"
            Bindings.bind2 a b (fun (a',b') -> text $"{a'} + {b'} = {a' + b'}")
        ]
    ]
