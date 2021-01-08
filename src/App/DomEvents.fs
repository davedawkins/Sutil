module DomEvents

open Sveltish
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Styling
open Sveltish.Bindings
open Browser.Types

let m = Store.make (0.0,0.0)

let handleMousemove (e:MouseEvent) =
    m <~ (e.clientX, e.clientY)

let view() =
    Html.div [
        onMouseMove handleMousemove []
        bind m <| fun (x,y) -> text $"The mouse position is {x} x {y}"
    ] |> withStyle [
        rule "div" [
            width "100vw"
            height "100vh"
        ]
    ]


