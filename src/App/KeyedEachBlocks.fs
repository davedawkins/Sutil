module KeyedEachBlocks

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Sutil.Styling
open System
open Browser.Dom
open Browser.Types

let nextId = Helpers.makeIdGenerator()

type Thing = { Id : int; Color : string }
let Color t = t.Color
let Id t = t.Id

let ThingView (thing : IObservable<Thing>) : NodeFactory =
        let viewId = nextId() // So we can see the lifetime of this view instance
        let initialColor = thing |> Store.current |> Color

        let thingStyle = [
            rule "span" [
                Css.display "inline-block"
                Css.padding "0.2em 0.5em"
                Css.margin(Zero, Em 0.2, Em 0.2, Zero)
                Css.width "8em"
                Css.textAlign "center"
                Css.borderRadius "0.2em"
                Css.color "#eeeeee"
            ]
        ]

        Html.div [
            bind thing <| fun t ->
                Html.p [
                    Html.span [ style [ Css.backgroundColor t.Color ]; text $"{t.Id} {t.Color} #{viewId}" ]
                    Html.span [ style [ Css.backgroundColor initialColor ]; text "initial" ]
                ] |> withStyle thingStyle
        ]

let view() =
    let things = Store.make [
        { Id = 1; Color = "darkblue" }
        { Id = 2; Color = "indigo" }
        { Id = 3; Color = "deeppink" }
        { Id = 4; Color = "salmon" }
        { Id = 5; Color = "gold" }
    ]

    let handleClick _ =
        things |> Store.modify List.tail

    Html.div [
        disposeOnUnmount [things]

        Html.button [
            onClick handleClick []
            text "Remove first thing"
        ]

        Html.div [
            style [ Css.display "grid"; Css.gridTemplateColumns "1fr 1fr"; Css.gridGap "1em" ]

            Html.div [
                Html.h2 [ text "Keyed" ]
                eachiko things (snd>>ThingView) (snd>>Id) []
            ]

            Html.div [
                Html.h2 [ text "Unkeyed" ]
                eachio things (snd>>ThingView) []
            ]
        ]
    ]