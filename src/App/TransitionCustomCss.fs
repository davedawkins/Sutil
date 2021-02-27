module TransitionCustomCss

// Adapted from
// https://svelte.dev/examples

open Feliz
open System
open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Transition
open Sutil.Styling

let spin (options : TransitionProp list) node =
    fun () ->
        let user = applyProps options Transition.Default
        {
            user with
                CssGen = Some (fun t _ ->
                let eased = Easing.elasticOut t
                [
                    $"transform: scale({eased}) rotate({eased * 1080.0}deg);"
                    $"color: hsl("
                    $"  {(t * 360.0)},"
                    $"  {Math.Min(100.0, 1000.0 - 1000.0 * t)}%%,"
                    $"  {Math.Min(50.0, 500.0 - 500.0 * t)}%%"
                    $");\n"
                ] |> String.concat "\n")
        }

let styleSheet = [
    let Pct (n : double) = length.percent n
    let Em (n : double) = length.em n
    let Vh (n : double) = length.vh n

    rule ".centered" [
        Css.position.absolute
        Css.left (Pct 50.0)
        Css.top (Pct 50.0)
        CssXs.transform  "translate(-50%,-50%)"
    ]

    rule "span" [
        Css.position.absolute
        CssXs.transform  "translate(-50%,-50%)"
        Css.fontSize (Em 4.0)
    ]

    rule ".container" [
        Css.position.relative
        Css.height.custom (Vh 60.0)
        Css.width (Pct 100.0)
    ]
]

let view() =
    let visible = Store.make false

    Html.div [
        disposeOnUnmount [visible]
        onMount (fun _ -> visible <~ true) []

        class' "container"

        Html.label [
            Html.input [
                type' "checkbox"
                Bind.attr ("checked", visible)
            ]
            text " visible"
        ]

        let flyIn = spin |> withProps [ Duration 8000.0 ]
        let fadeOut = fade

        transition [In flyIn; Out fadeOut] visible <|
            Html.div [
                class' "centered"
                Html.span [ text "transitions!" ]
            ]

    ] |> withStyle styleSheet

