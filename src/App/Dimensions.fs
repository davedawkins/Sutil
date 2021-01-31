module Dimensions

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Styling
open Sutil.Bindings

let style = Bulma.withBulmaHelpers [
    rule "input" [
        Css.display "block"
        Css.width "50%"
    ]
    rule "div.resizing" [
        Css.display "inline-block"
        Css.border "1pt solid #dddddd"
        Css.resize "both"
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
            Html.input [ type' "range"; bindAttr "value" size ]
        ]
        Html.div [
            class' "block"
            Html.input [ type' "text"; bindAttr "value" text ]
        ]

        Html.div [
            bind2 w h <| fun (w',h') -> DOM.text $"Size: {w'}px x {h'}px"
        ]

        Html.div [
            class' "resizing block"
            bindPropOut "clientWidth" w
            bindPropOut "clientHeight" h
            Html.span [
                bindAttrIn "style" (size |> Store.map (fun n -> $"font-size: {n}px"))
                bind text DOM.text
            ]
        ]
    ] |> withStyle style