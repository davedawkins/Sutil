module SelectBindings

// Adapted from
// https://svelte.dev/examples

open Browser
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling

type Question = {
    Id : int
    Text : string
}

let questions = [
    { Id = 1; Text = "How much water have you drunk today?" }
    { Id = 2; Text = "When did you last take a break?" }
    { Id = 3; Text = "Do you plan to go for a walk later?" }
];

let appStyle = [
    rule "input" [
        addClass "input"
        Css.display "block"
        Css.width "620px"
        Css.maxWidth "100%"
    ]
    rule "button" [ addClass "button" ]
    rule "form" [ addClass "block" ]
    rule "h2" [ addClass "title"; addClass "is-2" ]
]

// HTML helpers
let block children =
    Html.div <| (class' "block") :: children

let view() =
    let answer   = Store.make("")
    let selected = Store.make( questions |> List.head )

    let handleSubmit (e : Types.Event) =
        e.preventDefault()
        let a = Store.get answer
        let q = Store.get selected
        window.alert($"Answered question {q.Id} ({q.Text}) with '{a}'");

    Html.div [
        disposeOnUnmount [ answer; selected ]

        Html.h2 [ text "Health Check" ]

        Html.form [
            on "submit" handleSubmit []

            Html.div [
                class' "select block"
                Html.select [
                    Bindings.bindSelect selected
                    on "change" (fun _ -> Store.set answer "") []
                    for question in questions do
                        Html.option [
                            value question
                            text question.Text
                        ]
                ]
            ]

            block [
                Html.input [
                    type' "text"
                    Bindings.bindAttr "value" answer
                ]
            ]

            block [
                Html.button [
                    Bindings.bindAttrIn "disabled" (answer |> Store.map (fun a -> a = ""))
                    type' "submit"
                    text "Submit"
                ]
            ]
        ]

        block [
            Bindings.bind selected <| fun q ->
                Html.p [
                    text $"Selected question {q.Id}"
                ]
        ]
    ] |> withStyle appStyle