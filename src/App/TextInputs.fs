module TextInputs

open Sveltish
open Sveltish.Attr
open Sveltish.DOM

let name = Store.make("")

let view() =
    let nameOrStranger s = if s = "" then "stranger" else s

    Html.div [
        Html.input [
            class' "input"
            Bindings.bindAttr "value" name
            placeholder "enter your name"
        ]
        Html.p [
            Bindings.bind name (fun s -> text $"Hello {nameOrStranger s}")
        ]
    ]