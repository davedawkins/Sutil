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

[<Import("injectedGetStores", from="./inject.js")>]
let injectedGetStores() : obj = jsNative
[<Import("injectedDollar0", from="./inject.js")>]
let injectedDollar0() : obj = jsNative

[<Import("injectedGetOptions", from="./inject.js")>]
let injectedGetOptions() : obj = jsNative

[<Import("injectedSetOptions", from="./inject.js")>]
let injectedSetOptions( options : obj ) : obj = jsNative

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

let page m = m.Page
let slowAnimations m = m.Options.SlowAnimations
let loggingEnabled m = m.Options.LoggingEnabled


let run<'T,'A> (fn : 'A -> 'T) (arg:'A) : JS.Promise<'T> =
    console.log( $"run: ({fn})({JS.JSON.stringify arg})" )

    Promise.create( fun fulfil fail ->
        Chrome.Devtools.InspectedWindow.eval
            $"({fn})({JS.JSON.stringify arg})"
            {| |}
            (fun result -> if Interop.isUndefined result then (fail <| Exception("Unknown error")) else fulfil result)
    )

let init() = {
    Page = Stores
    Options = {
        SlowAnimations = false
        LoggingEnabled = false
    }
}

let update msg model =
    console.log($"update: {msg}\n{model}")
    let m =
        match msg with
        | ViewPage p -> { model with Page = p }
        | SetSlowAnimations f -> { model with Options = { model.Options with SlowAnimations = f } }
        | SetLoggingEnabled f -> { model with Options = { model.Options with LoggingEnabled = f } }
    run injectedSetOptions  (model.Options) |> ignore
    m

let mutable panel: Chrome.Devtools.Panels.ExtensionPanel = Unchecked.defaultof<_>
let mutable sidePanel : Chrome.Devtools.Panels.ExtensionSidebarPane = Unchecked.defaultof<_>
let mutable panelDoc : Document = Unchecked.defaultof<_>

let mutable stores : ObservablePromise<obj> = Unchecked.defaultof<_>

let styleSheet = [
    rule ".sv-container" [ padding "12px";minHeight "100vh" ]
    rule ".sv-main" [ background "white"; minHeight "100vh" ]
    rule ".sv-sidebar" [ background "#eeeeee";borderRight "1pt solid #cccccc" ]
    rule "#sv-title" [ marginBottom "4px" ]
    rule ".sv-menu li" [ fontSize "90%"; cursor "pointer" ]
    rule ".sv-menu li:hover" [ textDecoration "underline" ]
    rule ".sv-menu li.active" [ fontWeight "bold" ]
]

let contentRun<'T,'A> (fn : 'A -> 'T) (arg:'A) (success : 'T -> unit) (failure: string -> unit) =
    Chrome.Devtools.InspectedWindow.eval
        $"({fn})({JS.JSON.stringify arg})"
        {| |}
        (fun result ->
            if Interop.isUndefined result then failure("Unknown error") else success result)
            //if not (Interop.isUndefined result)
            //    then success(result)
            //else if not (Interop.isUndefined error)
            //    then failure(error)
            //else
            //    failwith "No result return from eval")

let getStores() = run injectedGetStores ()

let buildStoresTable (idVals : obj array) =
    Html.div [
        Html.table [
            Html.thead [
                Html.tr [
                    Html.th [ text "Id" ]
                    Html.th [ text "Val" ]
                ]
            ]
            Html.tbody [
                for item in idVals do
                    Html.tr [
                        Html.td [ text (string item?Id) ]
                        Html.td [ text (Fable.Core.JS.JSON.stringify item?Val) ]
                    ]
            ]
        ]
    ] |> withStyle styleSheet

let viewStores model dispatch =
    Html.div [
        bind stores <| function
            | Waiting -> text "Waiting"
            | Result r -> buildStoresTable r?Data
            | Error x -> text "Error"

        on Event.ElementReady (fun _ -> stores.Run (getStores())) []
    ]

let viewOptions (model:IObservable<Model>) dispatch =
    Html.div [
        Html.div [
            class' "field"
            Html.label [
                class' "checkbox"
                Html.input [
                    type' "checkbox"
                    bindAttrNotify "checked" (model .> slowAnimations) (dispatch << SetSlowAnimations)
                ]
                text " Slow Animations"
            ]
        ]
        Html.div [
            class' "field"
            Html.label [
                class' "checkbox"
                Html.input [
                    type' "checkbox"
                    bindAttrNotify "checked" (model .> loggingEnabled) (dispatch << SetLoggingEnabled)
                ]
                text " Logging Enabled"
            ]
        ]
    ]

let makeStore doc = ObservableStore.makeElmishSimpleWithDocument doc init update ignore

let view doc =
    stores <- ObservablePromise<obj>(doc)
    let model, dispatch = makeStore doc ()
    //let dispatch = ignore
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

Chrome.Devtools.Panels.elements.onSelectionChanged.addListener (
        fun _ ->
            contentRun
                injectedDollar0
                ()
                (fun result ->
                    sidePanel.setObject( result, "Selected", ignore)
                    console.dir(result)
                    )
                (fun _ -> console.log("failed"))
            console.log("elements.onSelectionChanged")
            )

Chrome.Devtools.Panels.elements.createSidebarPane(
    "Sveltish",
    fun sidebarPanel -> sidePanel <- sidebarPanel
    )

