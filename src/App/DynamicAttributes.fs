module DynamicAttributes

// Adapted from
// https://svelte.dev/examples

open Sutil
let src = "https://i.gifer.com/K9s.gif"
let name = "Minion"

let view() =
    Html.img [
        Attr.src src
        Attr.alt $"{name} playing guitar"
    ]
