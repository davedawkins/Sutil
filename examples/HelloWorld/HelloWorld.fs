module HelloWorld

//open Sutil.Html
open Sutil.DOM

//let helloWorld() = div [
//    text "Hello World!"
//]

//mountElement "Sutil-app" <| helloWorld()
mountElement "Sutil-app" (text "Hello World")
