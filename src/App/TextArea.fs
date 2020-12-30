module TextArea

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Styling
open Fable.Core

[<ImportAll("./marked.min.js")>]
let marked text : string = jsNative

let inputText =
    Store.make
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
    Html.div [
        Html.textarea [
            rows "5"
            Bindings.bindAttr "value" inputText
        ]

        Html.span [
            Bindings.bind inputText <| fun t -> html $"{marked t}"
        ] |> withStyle Markdown.style
    ] |> withStyle style
