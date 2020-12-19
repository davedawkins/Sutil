module App

open System
open Sveltish

open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Transition

open Browser.Types
open Browser.Dom

let log s = console.log(s)


module BulmaStyling =
    open Styling

    let bulmaStyleSheet = [
        rule ".button" [
            alignItems "center"
            border "1px solid transparent"
            borderRadius "4px"
            boxShadow "none"
            display "inline-flex"
            fontSize "1rem"
            height "2.5em"
            justifyContent "flex-start"
            lineHeight "1.5"
            position "relative"
            verticalAlign "top"

            backgroundColor "#fff"
            borderColor "#dbdbdb"
            borderWidth "1px"
            color "#363636"
            cursor "pointer"
            justifyContent "center"
            paddingBottom "calc(.5em - 1px)"
            paddingLeft "1em"
            paddingRight "1em"
            paddingTop "calc(.5em - 1px)"
            textAlign "center"
            whiteSpace "nowrap"
        ]

        rule "body" [
            fontFamily "sans-serif"
            color "#4a4a4a"
            fontSize "1em"
            fontWeight "400"
            lineHeight "1.5"
        ]

        rule ".container" [
            margin "0 auto"
            position "relative"
            width "auto"
            maxWidth "960px"
        ]
    ]


module App =
    open Bindings
    open BulmaStyling
    open Stores

    let count = Stores.makeStore 0

    let testApp : NodeFactory =
        Styling.style bulmaStyleSheet <| Html.div [
            className "container"
            Html.p [ text "Sveltish is running" ]
            Html.p [ text "Counter" ]
            Counter.Counter { InitialCounter = 0; Label = "Click Me"; ShowHint = true }
            Html.p [ text "Todos" ]
            Todos.view
        ]

Sveltish.DOM.mountElement "sveltish-app"
    <| Counter.Counter { InitialCounter = 0; Label = "Click Me"; ShowHint = true }
