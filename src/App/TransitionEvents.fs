module TransitionEvents

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Transition

let visible = Store.make true
let status  = Store.make "Waiting..."
let view() =
    Html.div [
        Html.p [
            class' "block"
            text "status: "
            bind status text
        ]
        Html.label [
            Html.input [
                type' "checkbox"
                bindAttr "checked" visible
            ]
            text " visible"
        ]
        transition (Both(fly, [ Duration 2000.0; Y 200.0 ])) visible <|
            Html.p [
                on "introstart" (fun _ -> status <~ "intro started") []
                on "introend" (fun _ -> status <~ "intro ended") []
                on "outrostart" (fun _ -> status <~ "outro started") []
                on "outroend" (fun _ -> status <~ "outro ended") []
                text "Flies in and out"
            ]
    ]
