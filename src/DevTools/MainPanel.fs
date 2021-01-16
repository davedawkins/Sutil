module Sveltish.Devtools

// https://github.com/mdn/webextensions-examples/tree/master/devtools-panels
// https://stackoverflow.com/questions/4532236/how-to-access-the-webpage-dom-rather-than-the-extension-page-dom
// http://galadriel.cs.utsa.edu/plugin_study/injected_apps/brave_injected/sources/browser-android-tabs/chrome/common/extensions/docs/templates/intros/devtools_inspectedWindow.html
// https://gist.github.com/TaijaQ/5aff8ade70b386ba8527f6328914879f


open System
open Browser.Types
open Sveltish
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Styling
open Sveltish.Bindings
open Sveltish.Transition
open Browser.Dom

open Fable.Core
open Fable.Core.JsInterop

type StoreIdVal = {  Id : int; Val : obj }
type GetStoresResult = { Data: StoreIdVal array }

[<Import("GetStores", from="./inject.js")>]
let jsGetStores() : GetStoresResult = jsNative

[<Import("Dollar0", from="./inject.js")>]
let jsDollar0() : obj = jsNative

[<Import("Version", from="./inject.js")>]
let jsVersion() : DevToolsControl.Version = jsNative

[<Import("GetOptions", from="./inject.js")>]
let jsGetOptions() : DevToolsControl.SveltishOptions = jsNative

[<Import("SetOptions", from="./inject.js")>]
let jsSetOptions( options : DevToolsControl.SveltishOptions ) : bool = jsNative

[<Import("GetLogCategories", from="./inject.js")>]
let jsGetLogCategories() : (string * bool) array = jsNative

[<Import("SetLogCategory", from="./inject.js")>]
let jsSetLogCategory( nameState : obj array ) : bool = jsNative

type Page =
    |Stores
    |Options

type Model = {
    Page : Page
    Options : Sveltish.DevToolsControl.SveltishOptions
    }


type Message =
    | ViewPage of Page
    | SetSlowAnimations of bool
    | SetLoggingEnabled of bool
    | SetLoggingOption of string * bool

let page m = m.Page
let slowAnimations m = m.Options.SlowAnimations
let loggingEnabled m = m.Options.LoggingEnabled

let init() = {
    Page = Stores
    Options = {
        SlowAnimations = false
        LoggingEnabled = false
    }
}

let update msg model =
    //console.log($"update: {msg}\n{model}")
    match msg with
    | ViewPage p ->
        { model with Page = p }
    | SetSlowAnimations f ->
        let m = { model with Options = { model.Options with SlowAnimations = f } }
        Chrome.Helpers.inject jsSetOptions  (m.Options) |> ignore
        m
    | SetLoggingEnabled f ->
        let m = { model with Options = { model.Options with LoggingEnabled = f } }
        Chrome.Helpers.inject jsSetOptions  (m.Options) |> ignore
        m
    | SetLoggingOption (name,state) ->
        Chrome.Helpers.inject jsSetLogCategory [| name; state |] |> ignore
        model

let mutable panel: Chrome.Devtools.Panels.ExtensionPanel = Unchecked.defaultof<_>
let mutable sidePanel : Chrome.Devtools.Panels.ExtensionSidebarPane = Unchecked.defaultof<_>
let mutable panelDoc : Document = Unchecked.defaultof<_>
//let mutable stores : ObservablePromise<GetStoresResult> = Unchecked.defaultof<_>

let styleSheet = [
    rule ".sv-container" [ padding "12px";minHeight "100vh" ]
    rule ".sv-main" [ background "white"; minHeight "100vh" ]
    rule ".sv-sidebar" [ background "#eeeeee";borderRight "1pt solid #cccccc" ]
    rule "#sv-title" [ marginBottom "4px" ]
    rule ".sv-menu li" [ fontSize "90%"; cursor "pointer" ]
    rule ".sv-menu li:hover" [ textDecoration "underline" ]
    rule ".sv-menu li.active" [ fontWeight "bold" ]
    rule ".o-val" [ color "#1F618D" ]
    rule ".o-str" [ color "#B03A2E" ]
    rule ".o-bool" [ color "#3498DB" ]
    rule ".o-int" [ color "#117864" ]
    rule ".o-float" [ color "#117864" ]
    rule ".table" [
        fontSize "8pt"
        fontFamily "Consolas,Menlo,Monaco,Lucida Console,Liberation Mono,DejaVu Sans Mono,Bitstream Vera Sans Mono,Courier New,monospace,sans-serif"
    ]

    rule ".log-categories" [
        fontSize "80%"
        marginLeft "16px"
    ]
    rule ".log-categories .field" [
        marginBottom "0.5rem"
    ]
]

let getStores() = Chrome.Helpers.inject jsGetStores ()

let viewStr s =
    Html.span [
        text "\""
        Html.span [ class' "o-str"; text s ]
        text "\""
    ]

let viewBool (b:bool) =
    Html.span [
        Html.span [ class' "o-bool"; text<| string b ]
    ]

let viewInt i  =
    Html.span [
        Html.span [ class' "o-int"; text i ]
    ]

let viewFlt f  =
    Html.span [
        Html.span [ class' "o-float"; text f ]
    ]

let rec viewObject (x:obj) : NodeFactory =
    match  x with
    | :? int -> viewInt (downcast x)
    | :? float -> viewFlt (downcast x)
    | :? string -> viewStr (downcast x)
    | :? bool -> viewBool (downcast x)
    | x -> text (JS.JSON.stringify x)

let buildStoresTable (idVals : StoreIdVal array) =
    Html.div [
        Html.table [
            class' "table"
            Html.thead [
                Html.tr [
                    Html.th [ text "Id" ]
                    Html.th [ text "Val" ]
                ]
            ]
            Html.tbody [
                for item in idVals do
                    Html.tr [
                        Html.td [ text (string item.Id) ]
                        Html.td [
                            class' "o-val"
                            viewObject (item.Val)
                        ]
                    ]
            ]
        ]
    ]

let viewStores model dispatch =
    Html.div [
        bindPromise (getStores())
            (text "Waiting")
            (fun r -> buildStoresTable r.Data)
            (fun _ -> text "Error")
    ]

let divc name children = class' name :: children |> Html.div
let labelc name children = class' name :: children |> Html.label
let inputc name children = class' name :: children |> Html.input

let bindCheckboxField label (model:IObservable<bool>) dispatch =
    divc "field is-small" [
        labelc "checkbox is-small" [
            inputc "is-small" [
                type' "checkbox"
                bindAttrNotify "checked" model dispatch
            ]
            text $" {label}"
        ]
    ]


let viewOptions (model:IObservable<Model>) dispatch =
    //let mutable p : Node = null

    Html.div [
        bindCheckboxField "Slow Animations" (model .> slowAnimations) (dispatch << SetSlowAnimations)
        bindCheckboxField "Logging Enabled" (model .> loggingEnabled) (dispatch << SetLoggingEnabled)
        bindPromise (Chrome.Helpers.inject jsGetLogCategories ())
            (text "Waiting")
            (fun lcs ->
                Html.div [
                    class' "log-categories"
                    for (name,state) in lcs do
                        bindCheckboxField name (Store.make state) (fun v -> (name,v) |> SetLoggingOption |> dispatch)
                ])
            (fun x -> text "Error")
    ]

let makeStore doc = ObservableStore.makeElmishSimpleWithDocument doc init update ignore

let view doc =
    let model, dispatch = makeStore doc ()

    Html.div [
        class' "sv-container"
        Html.div [
            class' "columns"
            Html.div [
                class' "sv-sidebar column is-one-fifth"
                Html.h4 [
                    id' "sv-title"
                    class' "title is-5"
                    text "Sveltish"
                ]
                Html.ul [
                    class' "sv-menu"
                    Html.li [
                        onClick (fun _ -> Stores |> ViewPage |> dispatch ) []
                        text "Stores" ]
                    Html.li [ text "Styles" ]
                    Html.li [ text "Maps" ]
                    Html.li [ text "Element Bindings" ]
                    Html.li [ text "Attribute Bindings" ]
                    Html.li [
                        onClick (fun _ -> Options |> ViewPage |> dispatch) []
                        text "Options"
                        ]
                ]
            ]
            Html.div [
                class' "sv-main column is-four-fifths"

                transitionMatch (model .> page) <| [
                    ((=) Stores,  viewStores  model dispatch, None)
                    ((=) Options, viewOptions model dispatch, None)
                ]

            ] ] ] |> withStyle styleSheet

let initialisePanel (win: Window) =
    panelDoc <- win.document
    view panelDoc
    |> mountElementOnDocument panelDoc "sveltish-app"

let unInitialisePanel (win: Window) = ()

let initPanel (p: Chrome.Devtools.Panels.ExtensionPanel) =
    panel <- p
    panel.onShown.addListener initialisePanel
    panel.onHidden.addListener unInitialisePanel

Chrome.Devtools.Panels.create
    "Sveltish" // title
    "/icon.png" // icon
    "/html/panel.html"
    initPanel

Chrome.Devtools.Panels.elements.onSelectionChanged.addListener ( fun _ ->
    Chrome.Helpers.inject jsDollar0 ()
        |> Promise.iter (fun dollar0 -> sidePanel.setObject( dollar0, "Selected", ignore) ))

Chrome.Devtools.Panels.elements.createSidebarPane(
    "Sveltish",
    fun sidebarPanel -> sidePanel <- sidebarPanel
    )

