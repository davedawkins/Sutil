module Counter

open Sveltish
open Sveltish.Bindings
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Stores
open Sveltish.Styling

//
// Properties for the counter
//
type CounterProps = {
    InitialCounter : int
    ShowHint : bool
    Label: string
}

//
// Privte styling for the counter
//
let private counterStyle = [

    rule "div" [
        fontFamily "sans-serif"
        color "#4a4a4a"
        fontSize "1em"
        fontWeight "400"
        lineHeight "1.5"
    ]

    rule "button" [
        border "1px solid transparent"
        borderRadius "4px"
        boxShadow "none"
        fontSize "1rem"
        height "2.5em"
        position "relative"
        verticalAlign "top"

        backgroundColor "#fff"
        borderColor "#dbdbdb"
        borderWidth "1px"
        color "#363636"
        cursor "pointer"
        paddingBottom "calc(.5em - 1px)"
        paddingLeft "1em"
        paddingRight "1em"
        paddingTop "calc(.5em - 1px)"
        textAlign "center"
        whiteSpace "nowrap"
    ]

    rule "button.reset" [
        marginLeft "12px"
    ]

    rule "div.hint" [
        marginTop "8px"
        marginLeft "8px"
        fontSize "80%"
        color "gray"
    ]
]

open Sveltish.Transition

//
// Add this to a document with
// mountElement Counter.Counter { .. props .. }
//
//
let Counter props =
    let count = makeStore props.InitialCounter

    style counterStyle <| Html.div [
        Html.button [
            on "click" (fun _ -> count.Value() + 1 |> count.Set)

            (fun () ->
                text <| if count.Value() = 0 then props.Label else count.Value() |> sprintf "You clicked: %i time(s)"
            ) |> Bindings.bind count
        ]

        Html.button [
            className "reset"
            on "click" (fun _ -> 0 |> count.Set)
            text "Reset"
        ]

        (Html.div [ className "hint"; text "Click button to start counting" ])
        |> Bindings.transition
                (Both (Transition.fly [ X 100.0; Y 100.0; ]))
                (count |~> exprStore (fun () -> count.Value() = 0 && props.ShowHint))  // Visible if 'count = 0'
    ]
