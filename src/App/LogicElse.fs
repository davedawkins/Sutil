module LogicElse

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

let view() =
    let user = Store.make {| loggedIn = false |}

    let toggle _ =
        user |> Store.modify (fun u -> {| u with loggedIn = not u.loggedIn |})

    Html.div [
        disposeOnUnmount [ user ]

        bind user <| fun u ->
            Html.div [
                if u.loggedIn then
                    Html.button [
                        onClick toggle []
                        text "Log out"
                    ]
                else
                    Html.button [
                        onClick toggle []
                        text "Log in"
                    ]
            ]
    ]
