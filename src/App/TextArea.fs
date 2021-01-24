module TextArea

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Styling
open Fable.Core

[<ImportAll("./marked.min.js")>]
let marked text : string = jsNative

let sampleText =
     """## Markdown

- Some words are *italic*
- some are **bold**"""

let style = [
    rule "textarea" [
        width  "100%"
        height "100%"
        fontFamily "monospace"
        padding "4px"
    ]

    rule "span" [
        display     "block"
        marginTop   "40px"
    ]
]

let view() =
    let inputText = Store.make sampleText

    Html.div [
        disposeOnUnmount [inputText]

        Html.textarea [
            rows "5"
            Bindings.bindAttr "value" inputText
        ]

        Html.span [
            Bindings.bind inputText <| fun t -> html $"{marked t}"
        ] |> withStyle Markdown.style
    ] |> withStyle style
