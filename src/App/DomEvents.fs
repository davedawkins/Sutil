module DomEvents

// Adapted from
// https://svelte.dev/examples

open Sutil
open Feliz
open type Feliz.length
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Browser.Types
open System

let inner() =
    let r = Random()
    let clickHandler (e: Event) =
        let props: CustomDispatch<string> list = [Bubbles true; Detail(Some $"Hello there! %i{r.Next(1000)}")]
        CustomDispatch.dispatch<string>(e,"on-custom-click", props)

    Html.button [
        onClick clickHandler []
        text "I will dispatch a 'on-custom-click' event"
    ]

let view() =
    Html.div [
        let m = Store.make (0.0,0.0)
        let m2 = Store.make ""

        let handleMousemove (e:MouseEvent) =
            m <~ (e.clientX, e.clientY)

        disposeOnUnmount [m; m2]

        onMouseMove handleMousemove []
        Bind.fragment m <| fun (x,y) -> text $"The mouse position is {x} x {y}"

        onCustomEvent<string> "on-custom-click" (fun (e: CustomEvent<string>) -> m2 <~ (e.detail |> Option.defaultValue "")) []
        Html.div [
            class' "my-class"
            Html.p "Custom Events!"
            inner()
            Bind.fragment m2 <| fun s -> text $"Got: [{s}]"
        ] |> withStyle [
            rule "my-class" [ Css.displayFlex; Css.flexDirectionColumn ]
        ]

    ] |> withStyle [
        rule "div" [
            Css.width (vw 100)
            Css.height (vh 100)
        ]
    ]


