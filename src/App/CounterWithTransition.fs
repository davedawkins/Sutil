module CounterWithTransition

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Transition

//
// Private styling for the counter
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

open Sutil.Transition
open Browser.Dom

//
// Add this to a document with
// mountElement Counter.Counter { .. props .. }
//
//
let Counter() =
    let count = Store.make 0

    withStyle counterStyle <| Html.div [
        Html.button [
            onClick (fun _ -> count <~= (+) 1) []

            count |=> fun n ->
                text <| if n = 0 then "Click Me" else n |> sprintf "You clicked: %i time(s)"
        ]

        Html.button [
            class' "reset"
            on "click" (fun _ -> count <~ 0) []
            text "Reset"
        ]

        (Html.div [ class' "hint"; text "Click button to start counting" ])
        |> transition
                (Both (Transition.fly,[ X 100.0 ]))
                (count .> (=) 0)  // Visible if 'count = 0'
    ]
