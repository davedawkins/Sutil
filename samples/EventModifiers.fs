module EventModifiers

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM
open Sutil.Attr
open Browser.Dom

let handleClick _ =
    window.alert("no more alerts")

let view() = Html.button [
    onClick handleClick [Once]
    text "Click me"
]
view() |> Program.mountElement "sutil-app"
