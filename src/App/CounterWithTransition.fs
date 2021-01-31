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
        Css.fontFamily "sans-serif"
        Css.color "#4a4a4a"
        Css.fontSize "1em"
        Css.fontWeight "400"
        Css.lineHeight "1.5"
    ]

    rule "button" [
        Css.border "1px solid transparent"
        Css.borderRadius "4px"
        Css.boxShadow "none"
        Css.fontSize "1rem"
        Css.height "2.5em"
        Css.position "relative"
        Css.verticalAlign "top"

        Css.backgroundColor "#fff"
        Css.borderColor "#dbdbdb"
        Css.borderWidth "1px"
        Css.color "#363636"
        Css.cursor "pointer"
        Css.paddingBottom "calc(.5em - 1px)"
        Css.paddingLeft "1em"
        Css.paddingRight "1em"
        Css.paddingTop "calc(.5em - 1px)"
        Css.textAlign "center"
        Css.whiteSpace "nowrap"
    ]

    rule "button.reset" [
        Css.marginLeft "12px"
    ]

    rule "div.hint" [
        Css.marginTop "8px"
        Css.marginLeft "8px"
        Css.fontSize "80%"
        Css.color "gray"
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
                [InOut (fly |> withProps [ X 100.0 ])]
                (count .> (=) 0)  // Visible if 'count = 0'
    ]
