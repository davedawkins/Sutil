module TextInputs

open Sutil
open Sutil.Attr
open Sutil.DOM

let view() =
    let name = Store.make("")

    let nameOrStranger s = if s = "" then "stranger" else s

    Html.div [
        disposeOnUnmount [ name ]

        Html.input [
            type' "text"
            Bindings.bindAttr "value" name
            placeholder "Enter your name"
        ]
        Html.p [
            class' "block"
            Bindings.bind name (fun s -> text $"Hello {nameOrStranger s}")
        ]
    ]