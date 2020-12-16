module App

open Fvelize
open Sveltish

module BulmaStyling =
    let bulmaStyleSheet = [
        rule ".button" [
            alignItems "center"
            border "1px solid transparent"
            borderRadius "4px"
            boxShadow "none"
            display "inline-flex"
            fontSize "1rem"
            height "2.5em"
            justifyContent "flex-start"
            lineHeight "1.5"
            position "relative"
            verticalAlign "top"

            backgroundColor "#fff"
            borderColor "#dbdbdb"
            borderWidth "1px"
            color "#363636"
            cursor "pointer"
            justifyContent "center"
            paddingBottom "calc(.5em - 1px)"
            paddingLeft "1em"
            paddingRight "1em"
            paddingTop "calc(.5em - 1px)"
            textAlign "center"
            whiteSpace "nowrap"
        ]

        rule "body" [
            fontFamily "sans-serif"
            color "#4a4a4a"
            fontSize "1em"
            fontWeight "400"
            lineHeight "1.5"
        ]

        rule ".container" [
            margin "0 auto"
            position "relative"
            width "auto"
            maxWidth "960px"
        ]
    ]

open BulmaStyling
open Browser.Dom


module Counter =

    let Counter attrs =
        let count = Sveltish.makeStore 0
        div [
            button [
                className "button"
                onClick (fun _ ->
                    console.log("click")
                    count.Value() + 1 |> count.Set)

                (fun () ->
                    str <| if count.Value() = 0 then "Click Me" else count.Value() |> sprintf "You clicked: %i time(s)"
                ) |> bind count
            ]

            button [
                className "button"
                Attribute ("style", "margin-left: 12px;" )
                onClick (fun _ -> 0 |> count.Set)
                str "Reset"
            ]
        ]

let view =
        style bulmaStyleSheet <| div [
            className "container"

            p [ str "This component has its own style sheet"]

            p [ str "Fable is running" ]

            p [ str "You can click on this button:" ]

            Counter.Counter []

            p [ str "Here's a second counter "]

            Counter.Counter []

            p [ str "Here's the Svelte TODOs demo, which also has its own stylesheet"]

            Todos.view
        ]

mount "fvelte-main-app" view
