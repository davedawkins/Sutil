module TextArea

// Adapted from
// https://svelte.dev/examples

open Sutil
open type Feliz.length

open Sutil.Core
open Sutil.CoreElements
open Sutil.Styling
open Fable.Core

let marked text : string =
    let doc = Fable.Formatting.Markdown.Markdown.Parse(text)
    Fable.Formatting.Markdown.Markdown.ToHtml(doc)

let sampleText =
     """## Markdown

- Some words are *italic*
- some are **bold**"""

let style = [
    rule "textarea" [
        Css.width  (percent 100)
        Css.height (percent 100)
        Css.fontFamily "monospace"
        Css.padding 4
    ]

    rule "span" [
        Css.displayBlock
        Css.marginTop 40
    ]
]

let view() =
    let inputText = Store.make sampleText

    Html.div [
        disposeOnUnmount [inputText]

        Html.textarea [
            Attr.rows 5
            Bind.attr("value",inputText)
        ]

        Html.span [
            Bind.el(inputText , fun t -> Html.parse $"{marked t}")
        ] |> withStyle Markdown.style
    ] |> withStyle style
