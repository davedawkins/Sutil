module App

open Sveltish
open Sveltish.Attr
open Sveltish.Styling

let appStyleSheet = [
    rule "body" [
        fontFamily "sans-serif"
        color "#4a4a4a"
        fontSize "1em"
        fontWeight "400"
        lineHeight "1.5"
    ]
]

let init() = Todos.init()
let update = Todos.update

let app model dispatch =
    Styling.style appStyleSheet <| Html.div [
        Todos.view model dispatch
    ]

Sveltish.Program.makeProgram "sveltish-app" init update app