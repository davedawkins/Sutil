module LogicIf

open Sveltish
open Sveltish.Bindings
open Sveltish.DOM
open Sveltish.Attr

let user = {| loggedIn = Store.make false |}

let toggle _ =
    user.loggedIn |> Store.modify not

let view() =

    bind user.loggedIn <| fun loggedIn ->
        Html.div [
            if loggedIn then
                Html.button [
                    onClick toggle
                    text "Log out"
                ]

            if not loggedIn then
                Html.button [
                    onClick toggle
                    text "Log in"
                ]
        ]
