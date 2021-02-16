module LogicIf

// Adapted from
// https://svelte.dev/examples

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

        Bind.fragment user <| fun u ->
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