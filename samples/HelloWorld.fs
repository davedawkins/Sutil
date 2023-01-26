module HelloWorld

open Sutil
open Sutil.Core
open Sutil.CoreElements

let view() = Html.div [
    text "Hello World!"
]

view() |> Program.mountElement "sutil-app"
