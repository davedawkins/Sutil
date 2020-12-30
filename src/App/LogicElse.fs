module LogicElse

open Sveltish
open Sveltish.Bindings
open Sveltish.DOM
open Sveltish.Attr

let user = Store.make {| loggedIn = false |}

let toggle _ =
    user |> Store.modify (fun u -> {| u with loggedIn = not u.loggedIn |})

let view() =

    bind user <| fun u ->
        Html.div [
            if u.loggedIn then
                Html.button [
                    onClick toggle
                    text "Log out"
                ]
            else
                Html.button [
                    onClick toggle
                    text "Log in"
                ]
        ]
