module SelectBindings

// Adapted from
// https://svelte.dev/examples

open Browser
open Sutil
open Sutil.Core
open Sutil.CoreElements

open Sutil.Styling

open type Feliz.length

type Question = {
    Id : int
    Text : string
}

let questions = [
    { Id = 1; Text = "How much water have you drunk today?" }
    { Id = 2; Text = "When did you last take a break?" }
    { Id = 3; Text = "Do you plan to go for a walk later?" }
]

let appStyle = [
    rule "input" [
        Css.displayBlock
        Css.width 620
        Css.maxWidth (percent 100)
    ]
]

let input children = Html.input [ Attr.className "input"; yield! children ]
let form children = Html.form [ Attr.className "block"; yield! children ]
let button children = Html.button [ Attr.className "button"; yield! children ]
let h2 children = Html.h2 [ Attr.className "title is-2"; yield! children ]

// HTML helpers
let block children = Html.div [ Attr.className "block"; yield! children ]

let view() =
    let answer   = Store.make("")
    let selected : IStore<Question> = Store.make( questions |> List.head )

    let handleSubmit (e : Types.Event) =
        e.preventDefault()
        let a = Store.get answer
        let q = Store.get selected
        window.alert($"Answered question {q.Id} ({q.Text}) with '{a}'")

    Html.div [
        disposeOnUnmount [ answer; selected ]

        h2 [ text "Health Check" ]

        form [
            on "submit" handleSubmit []

            Html.div [
                Attr.className "select block"
                Html.select [
                    Bind.selected selected
                    on "change" (fun _ -> Store.set answer "") []
                    for question in questions do
                        Html.option [
                            Attr.value question // FIXME: Add obj overload for value
                            text question.Text
                        ]
                ]
            ]

            block [
                input [
                    Attr.typeText
                    Bind.attr ("value",answer)
                ]
            ]

            block [
                button [
                    Bind.attr ("disabled",answer |> Store.map (fun a -> a = ""))
                    Attr.typeSubmit
                    text "Submit"
                ]
            ]
        ]

        block [
            Bind.el( selected, fun q ->
                Html.p [
                    text $"Selected question {q.Id}"
                ] )
        ]
    ] |> withStyle appStyle
