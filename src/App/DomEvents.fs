module DomEvents

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Bindings
open Browser.Types

let view() =
    Html.div [
        let m = Store.make (0.0,0.0)

        let handleMousemove (e:MouseEvent) =
            m <~ (e.clientX, e.clientY)

        disposeOnUnmount [m]

        onMouseMove handleMousemove []
        bind m <| fun (x,y) -> text $"The mouse position is {x} x {y}"
    ] |> withStyle [
        rule "div" [
            Css.width "100vw"
            Css.height "100vh"
        ]
    ]


