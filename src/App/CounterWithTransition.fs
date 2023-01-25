module CounterWithTransition

open Sutil
open type Feliz.length
open Sutil.Core
open Sutil.CoreElements

open Sutil.Styling
open Sutil.Transition
open Sutil.Bindings

//
// Private styling for the counter
//
let private counterStyle = [
    rule "div" [
        Css.fontFamily "sans-serif"
        Css.color "#4a4a4a"
        Css.fontSize (em 1.0)
        Css.fontWeight 400
        Css.lineHeight (px 1.5)
    ]

    rule "button" [
        Css.border(px 1, Feliz.borderStyle.solid, Feliz.color.transparent)
        Css.borderRadius 4
        Css.boxShadowNone
        Css.fontSize (rem 1.0)
        Css.height (em 2.5)
        Css.positionRelative
        Css.verticalAlignTop

        Css.backgroundColor "#fff"
        Css.borderColor "#dbdbdb"
        Css.borderWidth 1
        Css.color "#363636"
        Css.cursorPointer

        Css.padding( calc "0.5em - 1px", em 1 )

        Css.textAlignCenter
        Css.whiteSpaceNowrap
    ]

    rule "button.reset" [
        Css.marginLeft 12
    ]

    rule "div.hint" [
        Css.marginTop 8
        Css.marginLeft 8
        Css.fontSize (percent 80.0)
        Css.color "gray"
    ]
]

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
