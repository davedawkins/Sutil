module Sutil.Bulma

open Feliz
open type Feliz.length
open Sutil.Styling
open Sutil.DOM
open Sutil.Attr

let styleHelpers = [
    rule "h1" [ addClass "title"; addClass "is-1" ]
    rule "h2" [ addClass "title"; addClass "is-2" ]
    rule "h3" [ addClass "title"; addClass "is-3" ]
    rule "h4" [ addClass "title"; addClass "is-4" ]
    rule "h5" [ addClass "title"; addClass "is-5" ]
    rule "button" [ addClass "button" ]

    rule "input[type='file']" [ addClass "file-cta" ]

    rule "input[type='text']" [
        addClass "input"
    ]

    rule "input[type='radio']" [
        addClass "radio"
    ]

    rule "input[type='checkbox']" [
        addClass "checkbox"
    ]

    rule "input[type='number']" [
        addClass "input"
        addClass "is-small"
        Css.maxWidth (length.percent 50)
    ]

    rule "input[type='range']" [
        addClass "input"
        addClass "is-small"
        Css.maxWidth (length.percent 50)
    ]
]

let withBulmaHelpers s =
    s @ styleHelpers

[<AutoOpen>]
module FontAwesome =
    let fa name = Html.i [ class' ("fa fa-" + name) ]

type ColorOptions() =
    member _.hasTextDanger = class' "has-text-danger"

type IsColorOptions() =
    member _.isDanger = class' "is-danger"
    member _.isLink = class' "is-link"
    member _.isInfo = class' "is-info"
    member _.isSuccess = class' "is-success"
    member _.isPrimary = class' "is-primary"
    member _.isWarning = class' "is-warning"

type ColumnOptions() =
    member _.is (n:int) = class' ("is-" + string n)
    member _.tabletIs (n:int) = class' (sprintf "is-%d-tablet" n)
    member _.desktopIs (n:int) = class' (sprintf "is-%d-desktop" n)
    member _.widescreenIs (n:int) = class' (sprintf "is-%d-widescreen" n)

type ColumnsOptions() =
    member _.isCentered = class' "is-centered"

type HeroOptions() =
    inherit IsColorOptions()
    member _.isFullheight = class' "is-fullheight"

type ControlOptions() =
    member _.hasIconsLeft = class' "has-icons-left"
    member _.hasIconsRight = class' "has-icons-right"

type IconOptions() =
    member _.isSmall = class' "is-small"
    member _.isLeft = class' "is-left"

type ButtonOptions() =
    inherit IsColorOptions()
    member _.isRounded = class' "is-rounded"
    member _.isOutlined = class' "is-outlined"
    member _.isInverted = class' "is-inverted"
    member _.isHovered = class' "is-hovered"
    member _.isActive = class' "is-active"
    member _.isLoading = class' "is-loading"

type FieldOptions() =
    member _.isGrouped = class' "is-grouped"
    member _.isHorizontal = class' "is-horizontal"

type FieldLabelOptions() =
    member _.isNormal = class' "is-normal"

type BulmaEngine() =
    member _.heroBody (props : NodeFactory list) = Html.div ([ class' "hero-body" ] @ props)
    member _.hero (props : NodeFactory list) = Html.section ([ class' "hero" ] @ props)
    member _.container (props : NodeFactory list) = Html.div ([ class' "container" ] @ props)
    member _.section (props : NodeFactory list) = Html.div ([ class' "section" ] @ props)
    member _.columns (props : NodeFactory list) = Html.div ([ class' "columns" ] @ props)
    member _.column (props : NodeFactory list) = Html.div ([ class' "column" ] @ props)
    member _.formBox (props : NodeFactory list) = Html.form ([ class' "box" ] @ props)
    member _.box (props : NodeFactory list) = Html.div ([ class' "box" ] @ props)
    member _.field (props : NodeFactory list) = Html.div ([ class' "field" ] @ props)
    member _.fieldLabel (props : NodeFactory list) = Html.div ([ class' "field-label" ] @ props)
    member _.fieldBody (props : NodeFactory list) = Html.div ([ class' "field-body" ] @ props)
    member _.label (props : NodeFactory list) = Html.label ([ class' "label" ] @ props)
    member _.label (label:string) = Html.label ([ class' "label" ] @ [ text label ])
    member _.labelFor (target:string) (label:string) = Html.label [ class' "label"; for' target; text label ]
    member _.button (props : NodeFactory list) = Html.button ([ class' "button" ] @ props)
    member _.control (props : NodeFactory list) = Html.div ([ class' "control" ] @ props)
    member _.controlInline (props : NodeFactory list) = Html.p ([ class' "control" ] @ props)
    member _.email (props : NodeFactory list) = Html.input ([ class' "input"; type' "email" ] @ props)
    member _.input (props : NodeFactory list) = Html.input ([ class' "input"; type' "text" ] @ props)
    member _.checkbox (props : NodeFactory list) = Html.input ([ type' "checkbox" ] @ props)

    member _.selectList (props : NodeFactory list) =
        Html.div [
            class' "select is-multiple"
            Html.select props
        ] |> withStyleAppend [
                    rule "option" [
                        Css.padding(em 0.5, em 1.0)
                    ]
                    rule "select" [
                        Css.height auto
                        Css.padding 0
                    ] ]

    member _.selectMultiple (props : NodeFactory list) = Html.div [ class' "select is-multiple"; Html.select ([ multiple ] @ props) ]

    member _.password (props : NodeFactory list) = Html.input ([ class' "input"; type' "password" ] @ props)
    member _.icon (props : NodeFactory list) = Html.span ([ class' "icon"; type' "email" ] @ props)
    member x.labelCheckbox (label:string) checkboxProps= Html.label [ class' "checkbox"; x.checkbox checkboxProps; text label ]

// Issue #2110
// .select select[multiple] option {
//    padding: .5em 1em;
//}
//.select select[multiple] {
//    height: auto;
//    padding: 0;
//}

let bulma = BulmaEngine()
let hero = HeroOptions()
let columns = ColumnsOptions()
let column = ColumnOptions()
let control = ControlOptions()
let icon = IconOptions()
let button = ButtonOptions()
let field = FieldOptions()
let fieldLabel = FieldLabelOptions()
let color = ColorOptions()
