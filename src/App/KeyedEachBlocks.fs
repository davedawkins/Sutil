module KeyedEachBlocks

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Styling
open System
open Browser.Dom
open Browser.Types

let ThingViewOb (current : IObservable<string>) =
    let thingStyle = [
        rule "span" [
            display "inline-block"
            padding "0.2em 0.5em"
            margin "0 0.2em 0.2em 0"
            width "6em"
            textAlign "center"
            borderRadius "0.2em"
            color "#eeeeee"
        ]
    ]
    let mutable initial = ""
    Html.div [
        bind current (fun color ->
            if initial = "" then initial <- color
            Html.p [
                Html.span [ style $"background-color: {initial};"; text "initial" ]
                Html.span [ style $"background-color: {color};"; text "current" ]
            ] |> withStyle thingStyle)
    ]

let ThingView (current : string) : NodeFactory =
        let thingStyle = [
            rule "span" [
                display "inline-block"
                padding "0.2em 0.5em"
                margin "0 0.2em 0.2em 0"
                width "6em"
                textAlign "center"
                borderRadius "0.2em"
                color "#eeeeee"
            ]
        ]
        let mutable initial = ""
        Html.div [
                if initial = "" then initial <- current
                Html.p [
                    Html.span [ style $"background-color: {initial};"; text "initial" ]
                    Html.span [ style $"background-color: {current};"; text "current" ]
                ] |> withStyle thingStyle
        ]

type Thing = { Id : int; Color : string }

let things = Store.make [
    { Id = 1; Color = "darkblue" }
    { Id = 2; Color = "indigo" }
    { Id = 3; Color = "deeppink" }
    { Id = 4; Color = "salmon" }
    { Id = 5; Color = "gold" }
]

let handleClick _ =
    things |> Store.modify List.tail

let view() =
    Html.div [
        Html.button [
            onClick handleClick
            text "Remove first thing"
        ]

        Html.div [
            style "display: grid; grid-template-columns: 1fr 1fr; grid-gap: 1em"

            Html.div [
                Html.h2 [ text "keyed" ]
                //keyedEach things (fun t -> t.Id) (Store.map (fun t -> t.Color) >> ThingView)
                    //{#each things as thing (thing.id)}
                    //	<Thing current={thing.color}/>
                    //{/each}
            ]
            Html.div [
                Html.h2 [ text "Unkeyed" ]
                //each things (Store.map (fun t -> t.Color) >> ThingView)
                eachi things (fun _ thing ->
                    ThingView thing.Color
                ) None
                //{#each things as thing}
                //	<Thing current={thing.color}/>
                //{/each}
            ]
        ]
    ]