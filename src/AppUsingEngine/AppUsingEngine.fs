module AppUsingEngine

open Sutil.DOM
open Sutil.Feliz
open Sutil.Styling

let style1 = [
    rule "div" [
        Css.fontFamily "Arial"
    ]
]

let view() =
    Html.div "Hello world from Feliz.Engine"
        |> withStyle style1

mountElement "sutil-app" (view())