module Stores

open Sutil
open Sutil.Html

let view () = Html.div [ text "Stores" ]

view() |> Program.mountElement "sutil-app"
