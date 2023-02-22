namespace Sutil

open System
open Browser.Dom
open System.Collections.Generic
open Browser.Types
open Interop
open Sutil.DomHelpers

/// <summary>
/// Stores are values that can
/// - be updated
/// - subscribed to
///
/// This module defines Sutil's <c>Store</c> type
/// </summary>
[<RequireQualifiedAccess>]
module ObservableStore =

    let private logEnabled() = Logging.isEnabled "store"
    let private log s = Logging.log "store" s

    type StoreCons<'Model, 'Store> = (unit -> 'Model) -> ('Model -> unit) -> 'Store * Update<'Model>

    module internal Helpers =
        type CmdHandler<'Msg>(handler, ?dispose) =
            member _.Handle(cmd: Cmd<'Msg>): unit = handler cmd
            member _.Dispose() = match dispose with Some d -> d () | None -> ()
            interface IDisposable with
                member this.Dispose() = this.Dispose()

    #if FABLE_COMPILER
        open Fable.Core

        [<Emit("$0 === $1")>]
        let fastEquals (x: 'T) (y: 'T): bool = jsNative

        let cmdHandler (dispatch: 'Msg -> unit): CmdHandler<'Msg> =
            new CmdHandler<_>(List.iter (fun cmd -> JS.setTimeout (fun _ -> cmd dispatch) 0 |> ignore))
    #else
        let fastEquals (x: 'T) (y: 'T): bool = Unchecked.equals x y

        let cmdHandler (dispatch: 'Msg -> unit): CmdHandler<'Msg> =
            let cts = new Threading.CancellationTokenSource()

            let mb = MailboxProcessor.Start(fun inbox -> async {
                while true do
                    let! msg = inbox.Receive()
                    dispatch msg
            }, cts.Token)

            new CmdHandler<_>(List.iter (fun cmd -> cmd mb.Post), fun _ -> cts.Cancel())
    #endif

    module Registry =
        open Fable.Core
        open Fable.Core.JsInterop

        let mutable nextId = 0
        let idToStore = new Dictionary<int,obj>()
        let storeToId = new Dictionary<obj,int>()
        //let disposed = new Dictionary<obj,int>()

        let notifyUpdateStore s v =
            CustomDispatch<_>.dispatch(Window.document,DomHelpers.Event.UpdateStore,{| Value = v |})

        let notifyMakeStore s =
            if storeToId.ContainsKey(s) then failwith "Store is already registered!"
            let id = nextId
            if logEnabled() then log $"make store #{id}"
            nextId <- nextId + 1
            idToStore.[id] <- s
            storeToId.[s] <- id
            CustomDispatch<_>.dispatch(Window.document,DomHelpers.Event.NewStore)

        let notifyDisposeStore (s:obj) =
            //if not (storeToId.ContainsKey(s)) then
            //    if disposed.ContainsKey(s) then
            //        failwith $"Store {disposed.[s]} has already been disposed"
            //    else
            //        failwith "Store is unknown to registry"

            let id = storeToId.[s]
            if logEnabled() then log($"dispose store #{id}")
            try
                idToStore.Remove(id) |> ignore
                storeToId.Remove(s) |> ignore
                //disposed.[s] <- id
            with
            | x -> Logging.error $"disposing store {id}: {x.Message}"

        let getStoreById id : IStoreDebugger =
            (idToStore.[id] :?> IStore<obj>).Debugger

        let controlBlock () : DevToolsControl.IControlBlock = {
            new DevToolsControl.IControlBlock with
                member _.ControlBlockVersion = 1
                member _.Version = { Major = 0; Minor = 1; Patch = 0 }
                member _.GetOptions() = DevToolsControl.Options
                member _.SetOptions(op) = DevToolsControl.Options <- op
                member _.GetStores() = storeToId.Values |> Seq.toArray
                member _.GetStoreById(id) = getStoreById id
                member _.GetLogCategories() = Logging.enabled |> Seq.map (fun k -> k.Key , k.Value) |> Seq.toArray
                member _.SetLogCategories(states) =
                    Logging.initWith states
                member _.PrettyPrint(id) =
                    (DomHelpers.findNodeWithSvId Window.document id) |> Option.iter (fun n -> (Core.DomNode n).PrettyPrint("Node #" + string id))
                member _.GetMountPoints() = [| |]
                    //Core.allMountPoints()
                    //    |> List.map (fun mp -> { new DevToolsControl.IMountPoint with
                    //                        member _.Id = mp.MountId
                    //                        member _.Remount() = mp.Mount() |> ignore })
                    //    |> List.toArray
        }

        let initialise (doc:Document) =
            DevToolsControl.initialise doc (controlBlock())

    let mutable private _nextStoreId = 0
    let private nextStoreId() =
        let n = _nextStoreId
        _nextStoreId <- n + 1
        n

    // Allow stores that can handle mutable 'Model types (eg, <input>.FileList). In this
    // case we can pass (fun _ _ -> true)
    type Store<'Model>(init: unit -> 'Model, dispose: 'Model -> unit) =
        let mutable uid = 0
        let storeId = nextStoreId()
        let mutable name = "store-" + (string storeId)
        let mutable _modelInitialized = false
        let mutable _model = Unchecked.defaultof<_>
        let model() =
            if not _modelInitialized then
                _model <- init()
                _modelInitialized <- true
            _model
        let subscribers =
            Collections.Generic.Dictionary<_, IObserver<'Model>>()

        override _.ToString() = $"#{storeId}={_model}"

        member _.Value = model()

        member this.Update(f: 'Model -> 'Model) =
            let newModel = f (model())

            // Send every update. Use 'distinctUntilChanged' with fastEquals to get previous behaviour
            //Fable.Core.JS.console.log($"Update {_model} -> {newModel}")
            if not (Helpers.fastEquals _model newModel) then
                _model <- newModel

                if subscribers.Count > 0 then
                    subscribers.Values
                        |> Seq.iter (fun s -> s.OnNext(_model))

        member this.Subscribe(observer: IObserver<'Model>): IDisposable =
            let id = uid
            uid <- uid + 1

            if logEnabled() then log $"subscribe {id}"

            subscribers.Add(id, observer)

            // TODO: Is this the right way to report the model to the subscriber immediately?
            //Fable.Core.JS.setTimeout (fun _ -> observer.OnNext(model)) 0 |> ignore

            // Sutil depends on an immediate callback
            observer.OnNext(model())

            Helpers.disposable <| fun () ->
                if logEnabled() then log $"unsubscribe {id}"
                subscribers.Remove(id) |> ignore

        member this.Name with get() = name and set (v) = name <- v

        member this.Dispose() =
            subscribers.Values |> Seq.iter (fun x -> x.OnCompleted())
            subscribers.Clear()
            dispose (model())
            _model <- Unchecked.defaultof<_>
            Registry.notifyDisposeStore this

        interface IStore<'Model> with
            member this.Subscribe(observer: IObserver<'Model>) = this.Subscribe(observer)
            member this.Update(f) = this.Update(f)
            member this.Value = this.Value
            member this.Name with get() = this.Name and set (v:string) = this.Name <- v
            member this.Debugger = {
                    new IStoreDebugger with
                        member _.Value = upcast this.Value
                        member _.NumSubscribers = subscribers.Count }

        interface IDisposable with
            member this.Dispose() = this.Dispose()

    let makeElmishWithCons (init: 'Props -> 'Model * Cmd<'Msg>)
                           (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
                           (dispose: 'Model -> unit)
                           (cons: StoreCons<'Model, 'Store>)
                           : 'Props -> 'Store * Dispatch<'Msg> =

        let mutable _storeDispatch: ('Store * Dispatch<'Msg>) option = None

        let mutable _cmdHandler = Unchecked.defaultof<Helpers.CmdHandler<'Msg>>
            //new Helpers.CmdHandler<'Msg>(ignore)

        fun props ->
            match _storeDispatch with
            | Some storeDispatch ->
                storeDispatch
            | None ->
                let store, storeUpdate =
                    cons
                        (fun () ->
                            let m, cmd = init props
                            _cmdHandler.Handle cmd
                            m)
                        (fun m ->
                            _cmdHandler.Dispose()
                            dispose m)

                let dispatch msg =
                    let mutable _cmds = []
                    storeUpdate(fun model ->
                        let model, cmds = update msg model
                        _cmds <- cmds
                        model)
                    _cmdHandler.Handle _cmds

                _cmdHandler <- Helpers.cmdHandler dispatch
                _storeDispatch <- Some(store, dispatch)
                store, dispatch

    let makeStore<'Model> (init:unit->'Model) (dispose:'Model->unit) =
        let s = new Store<'Model>(init,dispose)
        Registry.notifyMakeStore s
        // We have to delay this, because it will provoke a call through the subscribers, and _cmdHandler isn't set yet
        DomHelpers.rafu <| fun () -> s.Subscribe(Registry.notifyUpdateStore s) |> ignore
        s

    let makeElmishWithDocument (doc:Document) (init: 'Props -> 'Model * Cmd<'Msg>)
                   (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        //Registry.initialise doc

        makeElmishWithCons init update dispose (fun i d ->
            let s = makeStore i  d
            let u = (fun f -> s.Update(f); DomHelpers.Event.notifyUpdated doc)
            upcast s, u)

    let makeElmishSimpleWithDocument (doc:Document) (init: 'Props -> 'Model)
                   (update: 'Msg -> 'Model -> 'Model)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        //Registry.initialise doc

        let init p = init p, []
        let update msg model = update msg model, []
        makeElmishWithCons init update dispose (fun i d ->
            let s = makeStore i  d
            let u = (fun f -> s.Update(f); DomHelpers.Event.notifyUpdated doc)
            upcast s, u)

    let makeElmishSimple i u d = makeElmishSimpleWithDocument document i u d
    let makeElmish       i u d = makeElmishWithDocument       document i u d


#if USE_ELMISH_PROGRAM
type Program<'Arg,'Model,'Msg,'View> = private {
    Init : 'Arg -> 'Model * Cmd<'Msg>
    Update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>
    Dispose: 'Model -> unit
    View: IStore<'Model> -> Dispatch<'Msg> -> 'View
}

module Program =
    let map mapInit mapUpdate mapView mapDispose program = {
        Init = mapInit program.Init
        Update = mapUpdate program.Update
        View = mapView program.View
        Dispose = mapDispose program.Dispose
    }

    let mkSimple init update view =
        {
            Init = fun () -> init(), Cmd.none
            Update = fun msg model -> update msg model, Cmd.none
            Dispose = ignore
            View = view
        }

    let mkProgram init update view =
        {
            Init = init
            Update = update
            Dispose = ignore
            View = view
        }

    let run element arg p =
        let model, dispatch = arg |> ObservableStore.makeElmish p.Init p.Update p.Dispose
        p.View model dispatch |> Core.mountElement element
#endif
