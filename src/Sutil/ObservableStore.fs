namespace Sutil

open System
open Browser.Dom
open System.Collections.Generic
open Browser.Types

[<RequireQualifiedAccess>]
module ObservableStore =

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
        open Fable.Core.JsInterop

        let mutable nextId = 0
        let idToStore = new Dictionary<int,obj>()
        let storeToId = new Dictionary<obj,int>()

        let notifyMakeStore s =
            let id = nextId
            nextId <- nextId + 1
            idToStore.[id] <- s
            storeToId.[s] <- id
            DOM.dispatchSimple window.document DOM.Event.NewStore

        let notifyDisposeStore  s =
            let id = storeToId.[s]
            idToStore.Remove(id) |> ignore
            storeToId.Remove(s) |> ignore

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
                member _.GetMountPoints() =
                    DOM.allMountPoints()
                        |> List.map (fun mp -> { new DevToolsControl.IMountPoint with
                                            member _.Id = mp.MountId
                                            member _.Remount() = mp.Mount() |> ignore })
                        |> List.toArray
        }

        let initialise (doc:Document) =
            DevToolsControl.initialise doc (controlBlock())

    // Allow stores that can handle mutable 'Model types (eg, <input>.FileList). In this
    // case we can pass (fun _ _ -> true)
    type Store<'Model>(init: unit -> 'Model, dispose: 'Model -> unit) =
        let mutable uid = 0
        let mutable _modelInitialized = false
        let mutable _model = Unchecked.defaultof<_>
        let model() =
            if not _modelInitialized then
                _model <- init()
                _modelInitialized <- true
            _model
        let subscribers =
            Collections.Generic.Dictionary<_, IObserver<'Model>>()

        member _.Value = model()

        member _.Update(f: 'Model -> 'Model) =
            let newModel = f (model())

            // Send every update. Use 'distinctUntilChanged' with fastEquals to get previous behaviour
            _model <- newModel

            if subscribers.Count > 0 then
                subscribers.Values
                    |> Seq.iter (fun s -> s.OnNext(_model))

        member this.Subscribe(observer: IObserver<'Model>): IDisposable =
            let id = uid
            uid <- uid + 1
            subscribers.Add(id, observer)

            // TODO: Is this the right way to report the model to the subscriber immediately?
            //Fable.Core.JS.setTimeout (fun _ -> observer.OnNext(model)) 0 |> ignore

            // Sutil depends on an immediate callback
            observer.OnNext(model())

            Helpers.disposable <| fun () -> subscribers.Remove(id) |> ignore

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

        let mutable _cmdHandler =
            Unchecked.defaultof<Helpers.CmdHandler<'Msg>>

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
        s

    let makeElmishWithDocument (doc:Document) (init: 'Props -> 'Model * Cmd<'Msg>)
                   (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        Registry.initialise doc

        makeElmishWithCons init update dispose (fun i d ->
            let s = makeStore i  d
            let u f = s.Update(f); DOM.Event.notifyUpdated doc
            upcast s, u)

    let makeElmishSimpleWithDocument (doc:Document) (init: 'Props -> 'Model)
                   (update: 'Msg -> 'Model -> 'Model)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        Registry.initialise doc

        let init p = init p, []
        let update msg model = update msg model, []
        makeElmishWithCons init update dispose (fun i d ->
            let s = makeStore i  d
            let u f = s.Update(f); DOM.Event.notifyUpdated doc
            upcast s, u)

    let makeElmishSimple i u d = makeElmishSimpleWithDocument document i u d
    let makeElmish       i u d = makeElmishWithDocument       document i u d
