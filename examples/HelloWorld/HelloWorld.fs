module HelloWorld

//open Sveltish.Html
open Sveltish.DOM

//let helloWorld() = div [
//    text "Hello World!"
//]

//mountElement "sveltish-app" <| helloWorld()
mountElement "sveltish-app" (text "Hello World")
