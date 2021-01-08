module EventModifiers

open Sveltish
open Sveltish.DOM
open Sveltish.Attr
open Browser.Dom

let handleClick _ =
    window.alert("no more alerts")

let view() = Html.button [
    onClick handleClick [Once]
    text "Click me"
]