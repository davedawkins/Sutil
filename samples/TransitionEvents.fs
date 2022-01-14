module TransitionEvents

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition

let view() =
    let visible = Store.make true
    let status  = Store.make "Waiting..."

    Html.div [
        disposeOnUnmount [ visible; status ]

        Html.p [
            class' "block"
            text "status: "
            Bind.el(status,text)
        ]
        Html.label [
            Html.input [
                type' "checkbox"
                Bind.attr ("checked", visible )
            ]
            text " visible"
        ]
        transition [fly |> withProps [ Duration 2000.0; Y 200.0 ] |> InOut] visible <|
            Html.p [
                on "introstart" (fun _ -> status <~ "intro started") []
                on "introend" (fun _ -> status <~ "intro ended") []
                on "outrostart" (fun _ -> status <~ "outro started") []
                on "outroend" (fun _ -> status <~ "outro ended") []
                text "Flies in and out"
            ]
    ]

view() |> Program.mountElement "sutil-app"
