module Sutil.Devtools

// https://github.com/mdn/webextensions-examples/tree/master/devtools-panels
// https://stackoverflow.com/questions/4532236/how-to-access-the-webpage-dom-rather-than-the-extension-page-dom
// http://galadriel.cs.utsa.edu/plugin_study/injected_apps/brave_injected/sources/browser-android-tabs/chrome/common/extensions/docs/templates/intros/devtools_inspectedWindow.html
// https://gist.github.com/TaijaQ/5aff8ade70b386ba8527f6328914879f


open System
open Browser.Types
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Bindings
open Sutil.Transition
open Browser.Dom

open Fable.Core
open Fable.Core.JsInterop

type StoreIdVal = {  Id : int; Val : obj; NumSubscribers : int }
type GetStoresResult = { Data: StoreIdVal array }
type GetMountPointsResult = DevToolsControl.IMountPoint array
type LogState = string * bool
type LogOptions = LogState array

[<Import("GetMountPoints", from="./inject.js")>]
let jsGetMountPoints() : GetMountPointsResult = jsNative

[<Import("Remount", from="./inject.js")>]
let jsRemount(id: string) : bool = jsNative

[<Import("GetStores", from="./inject.js")>]
let jsGetStores() : GetStoresResult = jsNative

[<Import("Dollar0", from="./inject.js")>]
let jsDollar0() : obj = jsNative

[<Import("Version", from="./inject.js")>]
let jsVersion() : DevToolsControl.Version = jsNative

[<Import("GetOptions", from="./inject.js")>]
let jsGetOptions() : DevToolsControl.SutilOptions = jsNative

[<Import("SetOptions", from="./inject.js")>]
let jsSetOptions( options : DevToolsControl.SutilOptions ) : bool = jsNative

[<Import("GetLogCategories", from="./inject.js")>]
let jsGetLogCategories() : LogState array = jsNative

[<Import("SetLogCategories", from="./inject.js")>]
let jsSetLogCategories( nameState : LogState array ) : bool = jsNative

let dispatchPromise (success: 'a -> unit) failure p =
    p   |> Promise.map success
        |> Promise.catch failure

let getMountPoints() = Chrome.Helpers.inject jsGetMountPoints ()
let getStores() = Chrome.Helpers.inject jsGetStores ()
let getOptions() = Chrome.Helpers.inject jsGetOptions ()
let getLogCategories() = Chrome.Helpers.inject jsGetLogCategories ()
let writeLogCategories lcs = Chrome.Helpers.inject jsSetLogCategories lcs |> ignore
let writeOptions opt = Chrome.Helpers.inject jsSetOptions opt |> ignore
let remount mountId = Chrome.Helpers.inject jsRemount mountId |> ignore

type Page =
    |Stores
    |Options
    |MountPoints

type Model = {
    Page : Page
    LogCategories : LogState array
    Stores : StoreIdVal array
    Options : DevToolsControl.SutilOptions
    MountPoints : DevToolsControl.IMountPoint array
    }

let connectedStores = ObservablePromise<GetStoresResult>()

type Message =
    | ViewPage of Page
    // Outgoing from DevTools to app
    | SetSlowAnimations of bool
    | SetLoggingEnabled of bool
    | SetLogCategory of LogState
    // Incoming from the app
    | MountPointsFromApp of DevToolsControl.IMountPoint array
    | StoresFromApp of StoreIdVal array
    | LogCategoriesFromApp of LogState array
    | OptionsFromApp of DevToolsControl.SutilOptions

let page m = m.Page
let logCategories m = m.LogCategories
let stores m = m.Stores
let mountPoints m = m.MountPoints |> Array.toList
let slowAnimations m = m.Options.SlowAnimations
let loggingEnabled m = m.Options.LoggingEnabled

let init() =
    {
        Page = Options
        Options = {
            SlowAnimations = false
            LoggingEnabled = false
        }
        LogCategories = [| |]
        Stores = [| |]
        MountPoints = [| |]
    }, Cmd.none

let update msg model : Model * Cmd<Message> =
    //console.log($"update: {msg}\n{model}")
    match msg with
    | MountPointsFromApp mps ->
        { model with MountPoints = mps }, Cmd.none
    | OptionsFromApp op ->
        { model with Options = op }, Cmd.none
    | StoresFromApp s ->
        { model with Stores = s }, Cmd.none
    | LogCategoriesFromApp lcs ->
        { model with LogCategories = lcs }, Cmd.none
    | ViewPage p ->
        { model with Page = p }, Cmd.none
    | SetSlowAnimations f ->
        let m = { model with Options = { model.Options with SlowAnimations = f } }
        // Write back to app
        writeOptions m.Options
        m, Cmd.none
    | SetLoggingEnabled f ->
        let m = { model with Options = { model.Options with LoggingEnabled = f } }
        // Write back to app
        writeOptions m.Options
        m, Cmd.none
    | SetLogCategory (name,state) ->
        let m = { model with
                        LogCategories = model.LogCategories
                                        |> Array.map (fun (n,s) -> if n = name then (name,state) else (n,s))
                        }
        // Write back to app
        writeLogCategories m.LogCategories
        m, Cmd.none

let mutable panel: Chrome.Devtools.Panels.ExtensionPanel = Unchecked.defaultof<_>
let mutable sidePanel : Chrome.Devtools.Panels.ExtensionSidebarPane = Unchecked.defaultof<_>
let mutable panelDoc : Document = Unchecked.defaultof<_>

let styleSheet = [
    rule ".sv-container" [ Css.padding "12px";Css.minHeight "100vh" ]
    rule ".sv-main" [ Css.background "white"; Css.minHeight "100vh" ]
    rule ".sv-sidebar" [ Css.background "#eeeeee";Css.borderRight "1pt solid #cccccc"; Css.paddingRight "0" ]
    rule "#sv-title" [ Css.marginBottom "4px" ]
    rule ".sv-menu li" [ Css.fontSize "90%"; Css.cursor "pointer"; Css.paddingLeft "4px" ]
    rule ".sv-menu li:hover" [ Css.textDecoration "underline" ]
    rule ".sv-menu li.active" [
        Css.borderTop "1pt solid #cccccc"
        Css.borderLeft "1pt solid #cccccc"
        Css.borderBottom "1pt solid #cccccc"
        Css.borderTopLeftRadius "4px"
        Css.borderBottomLeftRadius "4px"
        Css.background "white"
        Css.marginRight "-2px"
        Css.marginLeft "-4px"
        Css.paddingLeft "8px" ]
    rule ".o-val"   [ Css.color "#1F618D" ]
    rule ".o-str"   [ Css.color "#B03A2E" ]
    rule ".o-bool"  [ Css.color "#3498DB" ]
    rule ".o-int"   [ Css.color "#117864" ]
    rule ".o-float" [ Css.color "#117864" ]
    rule ".table" [
        Css.fontSize "8pt"
        Css.fontFamily "Consolas,Menlo,Monaco,Lucida Console,Liberation Mono,DejaVu Sans Mono,Bitstream Vera Sans Mono,Courier New,monospace,sans-serif"
    ]
    rule ".options" [
        Css.fontSize "80%"
    ]
    rule ".log-categories" [
        Css.marginLeft "16px"
    ]
    rule ".log-categories .field" [
        Css.marginBottom "0.5rem"
    ]
]

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
                    Html.th [ text "NumSub" ]
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
                        Html.td [ text (string item.NumSubscribers) ]
                    ]
            ]
        ]
    ]

let viewStores model dispatch =
    Html.div [
        bind (model .> stores) buildStoresTable
    ]

let viewMountPoints model dispatch =
    Html.div [
        Html.ul [
            each (model .> mountPoints) (fun mp ->
                Html.li [
                    text mp.Id
                    Html.button [
                        class' "button is-small"
                        style [ Css.marginLeft "12px" ]
                        text "Remount"
                        onClick (fun _ -> remount mp.Id) []
                    ]
                ]
            ) []
        ]
    ]

let divc name children = class' name :: children |> Html.div
let labelc name children = class' name :: children |> Html.label
let inputc name children = class' name :: children |> Html.input

let bindCheckboxField label (model:IObservable<bool>) dispatch =
    divc "field" [
        labelc "checkbox" [
            Html.input [
                type' "checkbox"
                bindAttrNotify "checked" model dispatch
            ]
            text $" {label}"
        ]
    ]

let viewOptions (model:IObservable<Model>) dispatch =
    divc "options" [
        bindCheckboxField "Slow Animations" (model .> slowAnimations) (dispatch << SetSlowAnimations)
        bindCheckboxField "Logging Enabled" (model .> loggingEnabled) (dispatch << SetLoggingEnabled)
        bind (model .> logCategories) <| fun lcs ->
            divc "log-categories" [
                for (name,state) in lcs do
                    bindCheckboxField name (Store.make state) (fun v -> (name,v) |> SetLogCategory |> dispatch)
            ]
    ]

let makeStore doc = ObservableStore.makeElmishWithDocument doc init update ignore

let view model dispatch =

    let activeWhen p = bindClass (model .> (page >> (=) p)) "active"

    Html.div [
        class' "sv-container"
        Html.div [
            class' "columns"
            Html.div [
                class' "sv-sidebar column is-one-fifth"
                Html.h4 [
                    id' "sv-title"
                    class' "title is-5"
                    text "Sutil"
                ]
                Html.ul [
                    class' "sv-menu"
                    Html.li [
                        activeWhen Options
                        onClick (fun _ -> Options |> ViewPage |> dispatch) []
                        text "Options" ]
                    Html.li [
                        activeWhen Stores
                        onClick (fun _ -> Stores |> ViewPage |> dispatch ) []
                        text "Stores" ]
                    Html.li [
                        activeWhen MountPoints
                        onClick (fun _ -> MountPoints |> ViewPage |> dispatch ) []
                        text "Mount Points" ]
                    Html.li [ text "Styles" ]
                    Html.li [ text "Maps" ]
                    Html.li [ text "Element Bindings" ]
                    Html.li [ text "Attribute Bindings" ]
                ]
            ]
            Html.div [
                class' "sv-main column is-four-fifths"

                transitionMatch (model .> page) <| [
                    ((=) Stores,        viewStores  model dispatch, [])
                    ((=) Options,       viewOptions model dispatch, [])
                    ((=) MountPoints,   viewMountPoints model dispatch, [])
                ]

            ] ] ] |> withStyle styleSheet

let updateStoresFromApp dispatch =
    getStores()
        |> Promise.map (fun r -> r.Data)
        |> dispatchPromise (dispatch << StoresFromApp) (fun _ -> [| |] |> StoresFromApp |> dispatch)
        |> ignore


let initialiseConnectedApp (model:IObservable<Model>) dispatch =
    updateStoresFromApp dispatch

    getMountPoints()
        |> dispatchPromise (dispatch << MountPointsFromApp) (fun _ -> [| |] |> MountPointsFromApp |> dispatch)
        |> ignore

    let m = Store.current model

    // Learn the log categories upon first connection
    if m.LogCategories.Length = 0 then
        getOptions()
            |> dispatchPromise (dispatch << OptionsFromApp) ignore
            |> ignore
        getLogCategories()
            |> dispatchPromise (dispatch << LogCategoriesFromApp) ignore
            |> ignore
    else
        // assume user refreshed the app so push the options into the new app
        // small risk that it's a different version of the Sutil core.
        writeOptions m.Options
        writeLogCategories m.LogCategories

    //connectedStores.Run <| getStores()

let startMessageHandlers (model : IObservable<Model>) dispatch =
    Chrome.Devtools.Panels.elements.onSelectionChanged.addListener ( fun _ ->
        Chrome.Helpers.inject jsDollar0 ()
            |> Promise.iter (fun dollar0 -> sidePanel.setObject( dollar0, "Selected", ignore) ))

    Chrome.Devtools.Panels.elements.createSidebarPane(
        "Sutil",
        fun sidebarPanel -> sidePanel <- sidebarPanel
        )

    let backgroundPageConnection = Chrome.Runtime.connect( {| name = "devtools-page" |} )

    backgroundPageConnection.onMessage.addListener( fun msg ->
        match msg?name with
        |"content-page-connected" ->
            console.log("content page connected")
            initialiseConnectedApp model dispatch
        |"sutil-new-store" ->
            console.log("sutil-new-store")
            updateStoresFromApp dispatch
        |"sutil-update-store" ->
            console.log("sutil-update-store")
            updateStoresFromApp dispatch
        | _ ->
            console.log($"unhandled message: {msg?name}")
            ()
    )

    backgroundPageConnection.postMessage(
            {|
                name = "hello"
            |})

    backgroundPageConnection.postMessage(
            {|
                name = "init"
                tabId= Chrome.Devtools.InspectedWindow.tabId
            |})

let createMainPanel() =
    console.log("createMainPanel")
    let initialisePanel (win: Window) =
        panelDoc <- win.document
        let model, dispatch = makeStore panelDoc ()

        view model dispatch
            |> mountElementOnDocument panelDoc "sutil-app"

        startMessageHandlers model dispatch
        initialiseConnectedApp model dispatch

    let unInitialisePanel (win: Window) = ()

    Chrome.Devtools.Panels.create
        "Sutil" // title
        "/icon.png" // icon
        "/html/panel.html"
        (fun p ->
            panel <- p
            panel.onShown.addListener initialisePanel
            panel.onHidden.addListener unInitialisePanel)


createMainPanel()