/// <summary>
/// Main entry points for a Sutil program
/// </summary>
module Sutil.Program

open Core
open CoreElements
open Browser.Types
open Browser.Dom

let pipeline =
    Sutil.Core.pipeline()

type MountPoint = {
        Host : Element
        App : SutilElement
    }
    with
        member this.Mount() =
            mountOn (this.App) (this.Host) pipeline

        member this.MountAfter() =
            mountAfter (this.App) (this.Host :?> HTMLElement) pipeline

        //member this.Mount() =
        //    this.MountOn( this.Doc.querySelector($"#{this.MountId}") )
//                mountOn (exclusive this.App) host pipeline

// Tried to make these into static members, but get error
// "These element declarations are not permitted in an augmentation F# compiler"
// MountPoint is passed to DevTools
let mutable private _allMountPoints = []

let allMountPoints() = _allMountPoints

let private createMountPoint host app =
    let self = { Host = host; App = app }
    _allMountPoints <- self :: _allMountPoints
    self

let rec mountElementOnDocumentElement (host : Element) (app : SutilElement)  =
    let mp = createMountPoint host app
    ObservableStore.Registry.initialise (host.ownerDocument)
    mp.Mount() |> ignore

//
// Mount a top-level application SutilElement into an existing document
//
let rec mountElementOnDocument (doc : Document) id (app : SutilElement)  =
    let host = doc.querySelector($"#{id}")
    let mp = createMountPoint host app
    ObservableStore.Registry.initialise doc
    mp.Mount() |> ignore

let rec mountDomElement (host : Element) (app : SutilElement)  =
    mountElementOnDocumentElement host app

let rec mountElement id (app : SutilElement)  =
    mountElementOnDocument document id (exclusive app)

let rec mountElementAfter (after : HTMLElement) (app : SutilElement) =
    let mp = createMountPoint after app
    ObservableStore.Registry.initialise (after.ownerDocument)
    mp.MountAfter() |> ignore

//let makeComponent name (element : SutilElement) : SutilElement = fun (ctx,parent) ->
//    element( { ctx with StyleName = "" }, parent )

//
// Sutil Elmish
// The model mutates in Sutil, so the function signatures are slightly different to Elmish.
// This approach isn't necessary, but it could be helpful in that it encourages the view to
// dispatch messages that are then processed only in the update function.
//
let makeProgram host init update view =
    let doc = Browser.Dom.document
    let model = init()

    let makeDispatcher update =
        (fun msg ->
            update msg model
            DomHelpers.Event.notifyUpdated doc)

    mountElementOnDocument doc host <| view model (makeDispatcher update)
