module Sutil.Program

    open Sutil.DOM
    open Browser.Types
    open Browser.Dom

    type MountPoint = {
            Doc : Document
            MountId : string
            App : SutilElement
        }
        with
            member this.Mount() =
                let host = this.Doc.querySelector($"#{this.MountId}")
                mountOn (exclusive this.App) host

    // Tried to make these into static members, but get error
    // "These element declarations are not permitted in an augmentation F# compiler"
    // MountPoint is passed to DevTools
    let mutable private _allMountPoints = []

    let allMountPoints() = _allMountPoints

    let private createMountPoint doc id app =
        let self = { Doc = doc; MountId = id; App = app }
        _allMountPoints <- self :: _allMountPoints
        self

    //
    // Mount a top-level application SutilElement into an existing document
    //
    let rec mountElementOnDocument (doc : Document) id (app : SutilElement)  =
        let mp = createMountPoint doc id app
        ObservableStore.Registry.initialise doc
        mp.Mount() |> ignore

    let rec mountElement id (app : SutilElement)  =
        mountElementOnDocument document id app

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
                DOM.Event.notifyUpdated doc)

        mountElementOnDocument doc host <| view model (makeDispatcher update)