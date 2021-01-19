module TransitionCustomCss

open System
open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Transition
open Browser.Types
open Sveltish.Styling

let visible = Store.make true

let spin (options : TransitionProp list) node =
    fun () ->
        let user = applyProps options Transition.Default
        {
            user with
                Css = (fun t _ ->
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
        position "absolute"
        left "50%"
        top "50%"
        transform "translate(-50%,-50%)"
    ]

    rule "span" [
        position "absolute"
        transform "translate(-50%,-50%)"
        fontSize "4em"
    ]

    rule ".container" [
        position "relative"
        height "60vh"
        width "100%"
    ]
]

let view() =
    Html.div [
        class' "container"

        Html.label [
            Html.input [
                type' "checkbox"
                bindAttr "checked" visible
            ]
            text " visible"
        ]

        let flyIn = (spin, [ Duration 8000.0 ])
        let fadeOut = (fade, [])

        transition (InOut(flyIn,fadeOut)) visible <|
            Html.div [
                class' "centered"
                Html.span [ text "transitions!" ]
            ]

    ] |> withStyle styleSheet

