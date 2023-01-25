/// <summary>
/// Adapter for <a href="">Feliz.Engine.Bulma</a>
/// </summary>
module Sutil.Bulma

open Sutil
open Sutil.Styling
open Sutil.Core
open Sutil.CoreElements

open type Feliz.length


/// <summary>
/// Patches to help with issues with <c>Feliz.Engine.Bulma</c>
/// </summary>
module Helpers =
    // Issue #2110
    // .select select[multiple] option {
    //    padding: .5em 1em;
    //}
    //.select select[multiple] {
    //    height: auto;
    //    padding: 0;
    //}
    let selectList (props : SutilElement list) =
        Html.div [
            CoreElements.class' "select is-multiple"
            Html.select [
                Attr.style [ Css.height auto; Css.padding 0 ]
                yield! props
            ]
        ]
        // |> withStyleAppend [
        //             rule "option" [
        //                 Css.padding(em 0.5, em 1.0)
        //             ]
        //             rule "select" [
        //                 Css.height auto
        //                 Css.padding 0
        //             ] ]

    let selectMultiple (props : SutilElement list) = Html.div [ class' "select is-multiple"; Html.select ([ Attr.multiple true ] @ props) ]

let styleHelpers = [
    rule "h1" [ PseudoCss.addClass "title"; PseudoCss.addClass "is-1" ]
    rule "h2" [ PseudoCss.addClass "title"; PseudoCss.addClass "is-2" ]
    rule "h3" [ PseudoCss.addClass "title"; PseudoCss.addClass "is-3" ]
    rule "h4" [ PseudoCss.addClass "title"; PseudoCss.addClass "is-4" ]
    rule "h5" [ PseudoCss.addClass "title"; PseudoCss.addClass "is-5" ]
    rule "button" [ PseudoCss.addClass "button" ]

    rule "input[type='file']" [ PseudoCss.addClass "file-cta" ]

    rule "input[type='text']" [
        PseudoCss.addClass "input"
    ]

    rule "input[type='radio']" [
        PseudoCss.addClass "radio"
    ]

    rule "input[type='checkbox']" [
        PseudoCss.addClass "checkbox"
    ]

    rule "input[type='number']" [
        PseudoCss.addClass "input"
        PseudoCss.addClass "is-small"
        Css.maxWidth (percent 50)
    ]

    rule "input[type='range']" [
        PseudoCss.addClass "input"
        PseudoCss.addClass "is-small"
        Css.maxWidth (percent 50)
    ]

    rule ".is-multiple option" [
        Css.padding(em 0.5, em 1.0)
    ]
]

let withBulmaHelpers element = withCustomRules styleHelpers element

/// Helper for creating FontAwesome icons: <c>&lt;i class='fa fa-name'/></c>
[<AutoOpen>]
module FontAwesome =
    let fa name = Html.i [ class' ("fa fa-" + name) ]

// --------------------------------------------------------
// !!! DO NOT EDIT !!!
// Generated from templates/Binding.template.fs
// --------------------------------------------------------

// This isn't compiled into the Feliz.Engine.Bulma package. Copy it to your framework library
// and replace Framework and FrameworkElement appropriately. It isn't necessary, but it may
// help reduce some "bulma.m." boilerplate noise in your app

let bulma = Feliz.Engine.Bulma.BulmaEngine<SutilElement>( Html, Attr )

// Can these be generated with a source generator?
let helpers = bulma.m.helpers
let size = bulma.m.size
let spacing = bulma.m.spacing
let text = bulma.m.text
let color = bulma.m.color
let image = bulma.m.image
let progress = bulma.m.progress
let table = bulma.m.table
let tr = bulma.m.tr
let tag = bulma.m.tag
let tags = bulma.m.tags
let title = bulma.m.title
let tabs = bulma.m.tabs
let tab = bulma.m.tab
let breadcrumb = bulma.m.breadcrumb
let cardHeaderTitle = bulma.m.cardHeaderTitle
let dropdown = bulma.m.dropdown
let modal = bulma.m.modal
let modalClose = bulma.m.modalClose
let navbar = bulma.m.navbar
let navbarMenu = bulma.m.navbarMenu
let navbarBurger = bulma.m.navbarBurger
let navbarDropdown = bulma.m.navbarDropdown
let navbarLink = bulma.m.navbarLink
let navbarItem = bulma.m.navbarItem
let paginationLink = bulma.m.paginationLink
let file = bulma.m.file
let input = bulma.m.input
let button = bulma.m.button
let buttons = bulma.m.buttons
let fieldLabel = bulma.m.fieldLabel
let textarea = bulma.m.textarea
let field = bulma.m.field
let icon = bulma.m.icon
let select = bulma.m.select
let control = bulma.m.control
let ol = bulma.m.ol
let content = bulma.m.content
let delete = bulma.m.delete
let container = bulma.m.container
let level = bulma.m.level
let section = bulma.m.section
let hero = bulma.m.hero
let tile = bulma.m.tile
let columns = bulma.m.columns
let column = bulma.m.column
