module TransitionCustomCss

// Adapted from
// https://svelte.dev/examples

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
    rule ".centered" [
        Css.position "absolute"
        Css.left "50%"
        Css.top "50%"
        Css.transform "translate(-50%,-50%)"
    ]

    rule "span" [
        Css.position "absolute"
        Css.transform "translate(-50%,-50%)"
        Css.fontSize "4em"
    ]

    rule ".container" [
        Css.position "relative"
        Css.height "60vh"
        Css.width "100%"
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

