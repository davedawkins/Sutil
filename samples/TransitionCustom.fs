module TransitionCustom

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition
open Browser.Types

let typewriter (userProps : TransitionProp list) (node: HTMLElement) = fun _ ->
    let valid = node.childNodes.length = 1 && DOM.isTextNode(node.childNodes.[0])

    if not valid then
        failwith "This transition only works on elements with a single text node child"

    let nodeText = node.textContent

    [ Speed 50.0 ] //  Default speed
        |> mergeProps userProps
        |> makeTransition
        |> mapTrans  (fun t ->
            [
                Duration (float(nodeText.Length) * t.Speed)
                Tick (fun t _ ->
                    let i = int( float(nodeText.Length) * t )
                    node.textContent <- nodeText.Substring(0,i)
                )
            ])

let view() =
    let visible = Store.make false

    Html.div [
        class' "container"

        Html.label [
            Html.input [
                type' "checkbox"
                Bind.attr( "checked", visible )
            ]
            text " visible"
        ]

        transition [In typewriter] visible <|
            Html.p [
                text "The quick brown fox jumps over the lazy dog"
            ]

        disposeOnUnmount [visible]
        onMount (fun _ -> true |> Store.set visible) [] // Force a transition upon first showing
    ]

view() |> Program.mountElement "sutil-app"
