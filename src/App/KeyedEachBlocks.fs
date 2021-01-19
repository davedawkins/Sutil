module KeyedEachBlocks

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

let things = Store.make [
    { Id = 1; Color = "darkblue" }
    { Id = 2; Color = "indigo" }
    { Id = 3; Color = "deeppink" }
    { Id = 4; Color = "salmon" }
    { Id = 5; Color = "gold" }
]

let ThingView (thing : IObservable<Thing>) : NodeFactory =
        let viewId = nextId() // So we can see the lifetime of this view instance
        let initialColor = thing |> Store.current |> Color

        let thingStyle = [
            rule "span" [
                display "inline-block"
                padding "0.2em 0.5em"
                margin "0 0.2em 0.2em 0"
                width "8em"
                textAlign "center"
                borderRadius "0.2em"
                color "#eeeeee"
            ]
        ]

        Html.div [
            bind thing <| fun t ->
                Html.p [
                    Html.span [ style $"background-color: {t.Color};"; text $"{t.Id} {t.Color} #{viewId}" ]
                    Html.span [ style $"background-color: {initialColor};"; text "initial" ]
                ] |> withStyle thingStyle
        ]

let handleClick _ =
    things |> Store.modify List.tail

let view() =
    Html.div [
        Html.button [
            onClick handleClick []
            text "Remove first thing"
        ]

        Html.div [
            style "display: grid; grid-template-columns: 1fr 1fr; grid-gap: 1em"

            Html.div [
                Html.h2 [ text "Keyed" ]
                eachiko things (snd>>ThingView) (snd>>Id)  None
            ]

            Html.div [
                Html.h2 [ text "Unkeyed" ]
                eachio things (snd>>ThingView) None
            ]
        ]
    ]