module TransitionCustomCss

// Adapted from
// https://svelte.dev/examples

open type Feliz.length
open type Feliz.transform
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
        Css.positionAbsolute
        Css.left (percent 50.0)
        Css.top (percent 50.0)
        Css.transform( translate( percent -50, percent -50 ) )
    ]

    rule "span" [
        Css.positionAbsolute
        Css.transform( translate( percent -50, percent -50 ) )
        Css.fontSize (em 4.0)
    ]

    rule ".container" [
        Css.positionRelative
        Css.height (vh 60.0)
        Css.width (percent 100.0)
    ]
]

let view() =
    let visible = Store.make false

    Html.div [
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

        disposeOnUnmount [visible]
        onMount (fun _ -> true |> Store.set visible) [] // Force a transition upon first showing
    ] |> withStyle styleSheet


view() |> Program.mountElement "sutil-app"
