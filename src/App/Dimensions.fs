module Dimensions

// Adapted from
// https://svelte.dev/examples

open Feliz
open type Feliz.length
open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Styling

let style = Bulma.withBulmaHelpers [
    rule "input" [
        Css.displayBlock
        Css.width (percent 50)
    ]
    rule "div.resizing" [
        Css.displayInlineBlock
        Css.border( pt 1, borderStyle.solid, "#dddddd")
        Css.resizeBoth
    ]
]

let view() =
    Html.div [
        let w = Store.make 0.0
        let h = Store.make 0.0
        let size = Store.make 42.0
        let text = Store.make "Edit me, slide him ↑"

        DOM.disposeOnUnmount [w; h; size; text ]

        Html.div [
            class' "block"
            Html.input [ type' "range"; Bind.prop("value",size) ]
        ]
        Html.div [
            class' "block"
            Html.input [ type' "text"; Bind.prop("value",text) ]
        ]

        Html.div [
            Bind.fragment2 w h <| fun (w',h') -> DOM.text $"Size: {w'}px x {h'}px"
        ]

        Html.div [
            class' "resizing"
            bindPropStoreOut "clientWidth" w
            bindPropStoreOut "clientHeight" h
            Html.span [
                Bind.attr("style", size |> Store.map (fun n -> $"font-size: {n}px") )
                Bind.fragment text DOM.text
            ]
        ]
    ] |> withStyle style