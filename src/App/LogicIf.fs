module LogicIf

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

let user = Store.make {| loggedIn = false |}

let toggle _ =
    user |> Store.modify (fun u -> {| u with loggedIn = not u.loggedIn |})

let view() =
    Html.div [
        bind user <| fun u ->
            Html.div [
                if u.loggedIn then
                    Html.button [
                        onClick toggle []
                        text "Log out"
                    ]

                if not u.loggedIn then
                    Html.button [
                        onClick toggle []
                        text "Log in"
                    ]
            ]
    ]