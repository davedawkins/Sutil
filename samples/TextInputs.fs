module TextInputs

// Adapted from
// https://svelte.dev/examples

open Sutil

open Sutil.Core
open Sutil.CoreElements

let view() =
    let name = Store.make("")

    let nameOrStranger s = if s = "" then "stranger" else s

    Html.div [
        disposeOnUnmount [ name ]

        Html.input [
            type' "text"
            Bind.attr ("value",name)
            Attr.placeholder "Enter your name"
        ]
        Html.p [
            class' "block"
            Bind.el(name, fun s -> text $"Hello {nameOrStranger s}")
        ]
    ]

view() |> Program.mountElement "sutil-app"
