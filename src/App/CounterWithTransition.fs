module CounterWithTransition

open Sutil
open Feliz
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Transition

//
// Private styling for the counter
//
let private counterStyle = [
    let Em (n : double) = length.em n
    let Rem (n : double) = length.rem n
    let Pct (n : double) = length.percent n

    rule "div" [
        Css.fontFamily "sans-serif"
        Css.color "#4a4a4a"
        Css.fontSize (Em 1.0)
        CssXs.fontWeight 400
        Css.lineHeight 1.5
    ]

    rule "button" [
        CssXs.border "1px solid transparent"
        Css.borderRadius 4
        Css.boxShadow.none
        Css.fontSize (Rem 1.0)
        Css.height.custom(Em 2.5)
        Css.position.relative
        Css.verticalAlign.top

        Css.backgroundColor "#fff"
        Css.borderColor "#dbdbdb"
        Css.borderWidth 1
        Css.color "#363636"
        Css.cursor.pointer
        CssXs.padding "calc(.5em - 1px) 1.0em"
        Css.textAlign.center
        Css.whitespace.nowrap
    ]

    rule "button.reset" [
        Css.marginLeft 12
    ]

    rule "div.hint" [
        Css.marginTop 8
        Css.marginLeft 8
        Css.fontSize (Pct 80.0)
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
