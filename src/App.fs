module App

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Styling

let bulmaStyleSheet = [
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

let init() = Todos.init()
let update = Todos.update

let app model dispatch =
    Styling.style bulmaStyleSheet <| Html.div [
        class' "container"
        Html.h1 [ text "Sveltish Todos" ]
        Html.div [ Todos.view model dispatch ]
    ]

Sveltish.Program.makeProgram "sveltish-app" init update app