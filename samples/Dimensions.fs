module Dimensions

// Adapted from
// https://svelte.dev/examples

open type Feliz.length
open type Feliz.borderStyle
open Sutil

open Sutil.Core
open Sutil.CoreElements
open Sutil.Styling

let style = Bulma.withBulmaHelpers [
    rule "input" [
        Css.displayBlock
        Css.width (percent 50)
    ]
    rule "div.resizing" [
        Css.displayInlineBlock
        Css.border( pt 1, solid, "#dddddd")
        Css.resizeBoth
    ]
]

let view() =
    Html.div [
        let w = Store.make 0.0
        let h = Store.make 0.0
        let size = Store.make 42.0
        let text = Store.make "Edit me, slide him ↑"

        Core.disposeOnUnmount [w; h; size; text ]

        Html.div [
            class' "block"
            Html.input [ type' "range"; Bind.attr("value",size) ]
        ]
        Html.div [
            class' "block"
            Html.input [ type' "text"; Bind.attr("value",text) ]
        ]

        Html.div [
            Bind.el2 w h <| fun (w',h') -> Core.text $"Size: {w'}px x {h'}px"
        ]

        Html.div [
            class' "resizing"
            bindPropOut "clientWidth" w
            bindPropOut "clientHeight" h
            Html.span [
                Bind.attr( "style", size |> Store.map (fun n -> $"font-size: {n}px") )
                Bind.el(text,Core.text)
            ]
        ]
    ] |> withStyle style

view() |> Program.mountElement "sutil-app"
