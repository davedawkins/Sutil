module HelloWorld

open Sveltish
open Sveltish.DOM

let helloWorld() = Html.div [
    text "Hello World!"
]

mountElement "sveltish-app" <| helloWorld()