module HelloWorld

//open Sutil.Html
open Sutil.DOM

//let helloWorld() = div [
//    text "Hello World!"
//]

//mountElement "sutil-app" <| helloWorld()
mountElement "sutil-app" (text "Hello World")
