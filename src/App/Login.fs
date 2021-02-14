module Login

// Conversion of https://codepen.io/stevehalford/pen/YeYEOR
// Study of how real components look in Sutil. First it was converted to plain Html (see viewHtml() below),
// and then a Bulma module was created to reduce the noise. See viewBulma(). I will leave the viewHtml() function
// for comparison.
//
open Sutil.DOM
open Sutil.Attr

//TODO - Move into library
module FontAwesome =
    let fa name = Html.i [ class' ("fa fa-" + name) ]

//TODO - Move into library
module Bulma =

    type ColumnOptions() =
        member _.is5Tablet = class' "is-5-tablet"
        member _.is4Desktop = class' "is-4-desktop"
        member _.is3Widescreen = class' "is-3-widescreen"
        member _.isCentered = class' "is-centered"

    type ColumnsOptions() =
        member _.isCentered = class' "is-centered"

    type HeroOptions() =
        member _.isPrimary = class' "is-primary"
        member _.isFullheight = class' "is-fullheight"

    type ControlOptions() =
        member _.hasIconsLeft = class' "has-icons-left"

    type IconOptions() =
        member _.isSmall = class' "is-small"
        member _.isLeft = class' "is-left"

    type ButtonOptions() =
        member _.isSuccess = class' "is-success"

    type BulmaEngine() =
        member _.heroBody (props : NodeFactory list) = Html.div ([ class' "hero-body" ] @ props)
        member _.hero (props : NodeFactory list) = Html.section ([ class' "hero" ] @ props)
        member _.container (props : NodeFactory list) = Html.div ([ class' "container" ] @ props)
        member _.columns (props : NodeFactory list) = Html.div ([ class' "columns" ] @ props)
        member _.column (props : NodeFactory list) = Html.div ([ class' "column" ] @ props)
        member _.formBox (props : NodeFactory list) = Html.form ([ class' "box" ] @ props)
        member _.box (props : NodeFactory list) = Html.div ([ class' "box" ] @ props)
        member _.field (props : NodeFactory list) = Html.div ([ class' "field" ] @ props)
        member _.label (props : NodeFactory list) = Html.label ([ class' "label" ] @ props)
        member _.label (label:string) = Html.label ([ class' "label" ] @ [ text label ])
        member _.labelFor (target:string) (label:string) = Html.label [ class' "label"; for' target; text label ]
        member _.button (props : NodeFactory list) = Html.button ([ class' "button" ] @ props)
        member _.control (props : NodeFactory list) = Html.div ([ class' "control" ] @ props)
        member _.email (props : NodeFactory list) = Html.input ([ class' "input"; type' "email" ] @ props)
        member _.checkbox (props : NodeFactory list) = Html.input ([ type' "checkbox" ] @ props)
        member _.password (props : NodeFactory list) = Html.input ([ class' "input"; type' "password" ] @ props)
        member _.icon (props : NodeFactory list) = Html.span ([ class' "icon"; type' "email" ] @ props)
        member x.labelCheckbox (label:string) = Html.label [ class' "checkbox"; x.checkbox []; text label ]

open Bulma
open FontAwesome

let bulma = BulmaEngine()
let hero = HeroOptions()
let columns = ColumnsOptions()
let column = ColumnOptions()
let control = ControlOptions()
let icon = IconOptions()
let button = ButtonOptions()

//
// Using Bulma module
//
let viewBulma () =
    bulma.hero [
        hero.isPrimary; hero.isFullheight
        bulma.heroBody [
            bulma.container [
                bulma.columns [
                    columns.isCentered
                    bulma.column [
                        column.is5Tablet; column.is4Desktop; column.is3Widescreen
                        bulma.formBox [
                            action ""
                            bulma.field [
                                bulma.label "Email"
                                bulma.control [
                                    control.hasIconsLeft
                                    bulma.email [ placeholder "e.g. bobsmith@gmail.com";  required ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "envelope"
                                    ]
                                ]
                            ]
                            bulma.field [
                                bulma.label "Password"
                                bulma.control [
                                    control.hasIconsLeft
                                    bulma.password [ placeholder "Password"; required ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "lock"
                                    ]
                                ]
                            ]
                            bulma.field [ bulma.labelCheckbox " Remember me" ]
                            bulma.button [ button.isSuccess; text "Login" ]
                        ] ] ] ] ] ]

//
// Same as above, but closer to original HTML with classes
//
let viewHtml() =
    Html.section [ class' "hero is-primary is-fullheight"
                   Html.div [ class' "hero-body"
                              Html.div [ class' "container"
                                         Html.div [ class' "columns is-centered"
                                                    Html.div [ class' "column is-5-tablet is-4-desktop is-3-widescreen"
                                                               Html.form [ action ""
                                                                           class' "box"
                                                                           Html.div [ class' "field"
                                                                                      Html.label [ for' ""
                                                                                                   class' "label"
                                                                                                   text "Email" ]
                                                                                      Html.div [
                                                                                            class' "control has-icons-left"
                                                                                            Html.input [
                                                                                                type' "email"
                                                                                                placeholder "e.g. bobsmith@gmail.com"
                                                                                                class' "input"
                                                                                                required ]
                                                                                            Html.span
                                                                                                     [ class'
                                                                                                         "icon is-small is-left"
                                                                                                       Html.i [ class'
                                                                                                                    "fa fa-envelope" ] ] ] ]
                                                                           Html.div [ class' "field"
                                                                                      Html.label [ for' ""
                                                                                                   class' "label"
                                                                                                   text "Password" ]
                                                                                      Html.div [
                                                                                            class' "control has-icons-left"
                                                                                            Html.input [
                                                                                                type' "password"
                                                                                                placeholder "*******"
                                                                                                class' "input"
                                                                                                required ]
                                                                                            Html.span
                                                                                                     [ class'
                                                                                                         "icon is-small is-left"
                                                                                                       Html.i [ class'
                                                                                                                    "fa fa-lock" ] ] ] ]
                                                                           Html.div [ class' "field"
                                                                                      Html.label [ for' ""
                                                                                                   class' "checkbox"
                                                                                                   Html.input [ type'
                                                                                                                    "checkbox" ]
                                                                                                   text " Remember me" ] ]
                                                                           Html.div [ class' "field"
                                                                                      Html.button [ class'
                                                                                                        "button is-success"
                                                                                                    text "Login" ] ] ] ] ] ] ] ]


let view() = viewBulma()