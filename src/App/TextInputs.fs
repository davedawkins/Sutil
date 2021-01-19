module TextInputs

open Sutil
open Sutil.Attr
open Sutil.DOM

let name = Store.make("")

let view() =
    let nameOrStranger s = if s = "" then "stranger" else s

    Html.div [
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