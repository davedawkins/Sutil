module BarChart

// Adapted from
// https://svelte.dev/examples#bar-chart

open Sutil
open Sutil.Core
open Sutil.CoreElements

open Sutil.Styling
open type Feliz.length
open Fable.Core

module D3Scale =
    type Scale<'T> = interface
        abstract domain: 'T array -> Scale<'T>
        abstract range: 'T array -> Scale<'T>
        [<Emit("$0($1)")>]
        abstract get: 'T -> 'T
        end
    [<Import("scaleLinear","d3-scale")>]
    let scaleLinear<'T>() : Scale<'T> = jsNative

type YearRate = { Year: int; Birthrate: float }

let points = [
    { Year = 1990; Birthrate = 16.7 }
    { Year = 1995; Birthrate = 14.6 }
    { Year = 2000; Birthrate = 14.4 }
    { Year = 2005; Birthrate = 14.0 }
    { Year = 2010; Birthrate = 13.0 }
    { Year = 2015; Birthrate = 12.4 }
]

let xTicks = [1990; 1995; 2000; 2005; 2010; 2015]
let yTicks = [0; 5; 10; 15; 20]
let padding = {| top=20.; right=15.; bottom=20.; left=25. |}

let formatMobile (tick:int) =
    $"'{(string tick).[-2..]}"

open D3Scale

type Message =
    | UpdateSize of (float * float)

type Model =
    {
        Width: float
        Height : float
        XScale : int -> int
        YScale : float -> float
    } with
        member this.BarWidth =
            let innerWidth w = w - (padding.left + padding.right)
            (innerWidth this.Width) / float xTicks.Length

let makeXScale w = fun n ->
                        scaleLinear()
                            .domain([|0; xTicks.Length |])
                            .range([| int padding.left; int(w - padding.right) |])
                            .get(n)

let makeYScale h = fun n ->
                        scaleLinear()
                            .domain( [|0.0; yTicks |> List.max |> float|] )
                            .range( [| h - padding.bottom; padding.top |] )
                            .get(n)

let init() =
    {
        Width = 500.
        Height = 206.
        XScale = makeXScale 500.0
        YScale = makeYScale 206.0
    }

let update (msg:Message) (model:Model) =
    match msg with
    | UpdateSize (w,h) ->
        { model with Width = w; XScale = makeXScale w; Height = h; YScale = makeYScale h }

let styleSheet = [
    rule "h4" [
        Css.textAlignCenter
    ]

    rule ".chart" [
        Css.width (percent 100)
        Css.maxWidth 500
        Css.margin(zero, auto)
    ]

    rule "svg" [
        Css.positionRelative
        Css.width (percent 100)
        Css.height 250
    ]

    rule ".tick" [
        Css.fontFamily "Helvetica, Arial"
        Css.fontSize (em 0.725)
        Css.fontWeight 200
    ]

    rule ".tick line" [
        Css.custom( "stroke", "#e2e2e2" )
        Css.custom( "stroke-dasharray", "2" )
    ]

    rule ".tick text" [
        Css.fill "#ccc"
        Css.custom( "text-anchor", "start" )
    ]

    rule ".tick.tick-0 line" [
        Css.custom( "stroke-dasharray", "0" )
    ]

    rule ".x-axis .tick text" [
        Css.custom( "text-anchor", "middle" )
    ]

    rule ".bars rect" [
        Css.fill "#a11"
        Css.custom( "stroke", "none" )
        Css.opacity 0.65
    ]
]

let makeStore = Store.makeElmishSimple init update ignore

let view() =
    let model, dispatch = makeStore()

    Html.div [
        disposeOnUnmount [ model ]

        Html.h4 [ class' "title is-4"; text "US birthrate by year" ]

        Html.div [
            class' "chart"

            Bind.el(model, fun m ->
                Svg.svg [
                    Svg.g [
                        class' "axis y-axis"
                        for tick in yTicks do
                            Svg.g [
                                class' $"tick tick-{tick}"
                                Svg.transform $"translate(0, {m.YScale(float tick)})"
                                Svg.line [ Svg.x2 "100%" ]
                                Svg.text [
                                    Svg.y "-4"

                                    let label = if tick = 20 then " per 1,000 population" else ""
                                    text $"{tick} {label}"
                                ]
                            ]
                    ]

                    Svg.g [
                        class' "axis x-axis"
                        let mutable i = 0
                        for point in points do
                            Svg.g [
                                class' "tick"
                                Svg.transform $"translate({m.XScale(i)},{m.Height})"
                                Svg.text [
                                    Svg.x $"{m.BarWidth/2.0}"
                                    Svg.y "-6"
                                    text <| if m.Width > 380. then string point.Year else formatMobile(point.Year)
                                ]
                            ]
                            i <- i + 1
                    ]

                    Svg.g [
                        class' "bars"
                        let mutable i = 0
                        for point in points do
                            Svg.rect [
                                Svg.x $"{m.XScale(i) + 2}"
                                Svg.y $"{m.YScale(point.Birthrate)}"
                                Svg.width $"{m.BarWidth - 4.0}"
                                Svg.height $"{m.YScale(0.0) - m.YScale(point.Birthrate)}"
                            ]
                            i <- i + 1
                    ]
                ]
            )
            CoreElements.listenToResize( fun e -> (e.clientWidth, e.clientHeight) |> UpdateSize |> dispatch)
            ]
    ] |> withStyle styleSheet
