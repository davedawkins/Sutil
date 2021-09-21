module KeyedEachBlocks

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Styling
open System
open Feliz
open type Feliz.length

type Thing = { Id : int; Color : string }
let Color t = t.Color
let Id t = t.Id

let ThingView (viewId : int) (thing : IObservable<Thing>) : SutilElement =
        let initialColor = thing |> Store.current |> Color

        let thingStyle = [
            rule "span" [
                Css.displayInlineBlock
                Css.padding( em 0.2, em 0.5 )
                Css.margin( zero, em 0.2, em 0.2, zero )
                Css.width (em 8)
                Css.textAlignCenter
                Css.borderRadius (em 0.2)
                Css.color "#eeeeee"
            ]
        ]

        Html.div [
            Bind.el(thing,fun t ->
                Html.p [
                    Html.span [ style [ Css.backgroundColor t.Color ]; text $"{t.Id} {t.Color} #{viewId}" ]
                    Html.span [ style [ Css.backgroundColor initialColor ]; text "initial" ]
                ] |> withStyle thingStyle)
        ]

let view() =
    let nextId = Helpers.makeIdGenerator()

    let makeThing thing = ThingView (nextId()) thing

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
            style [ Css.displayGrid; Css.gridTemplateColumns [fr 1; fr 1]; Css.gap (em 1) ]

            Html.div [
                Html.h2 [ text "Keyed" ]
                Bind.eachi( things, snd>>makeThing, (snd>>Id) )
            ]

            Html.div [
                Html.h2 [ text "Unkeyed" ]
                Bind.eachi( things, (snd>>makeThing) )
            ]
        ]
    ]
