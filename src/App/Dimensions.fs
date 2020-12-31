module Dimensions

open Sveltish
open Sveltish.Attr
open Sveltish.Bindings
open Sveltish.Styling
open Sveltish.Html

let w = Store.make 0.0
let h = Store.make 0.0
let size = Store.make 42.0
let text = Store.make "Edit me, slide him ↑"

let style = Bulma.withBulmaHelpers [
    rule "input" [
        display "block"
        width "50%"
    ]
    rule "div.resizing" [
        display "inline-block"
        border "1pt solid #dddddd"
        resize "both"
    ]
]

let view() =
    div [
        div [
            class' "block"
            input [ type' "range"; bindAttr "value" size ]
        ]
        div [
            class' "block"
            input [ type' "text"; bindAttr "value" text ]
        ]

        div [
            bind2 w h <| fun (w',h') -> DOM.text $"Size: {w'}px x {h'}px"
        ]

        div [
            class' "resizing block"
            bindPropOut "clientWidth" w
            bindPropOut "clientHeight" h
            span [
                bindAttrIn "style" (size |> Store.map (fun n -> $"font-size: {n}px"))
                bind text DOM.text
            ]
        ]
    ] |> withStyle style