namespace Sveltish

open System

[<RequireQualifiedAccess>]
module ObservableStore =

    // Dave's understanding of the types here.
    // ('Model -> 'Model) is a model updater
    type Update<'Model> = ('Model -> 'Model) -> unit // A store updater. Store updates by being passed a model updater
    type Dispatch<'Msg> = 'Msg -> unit // Message dispatcher
    type Cmd<'Msg> = (Dispatch<'Msg> -> unit) list // List of commands. A command needs a dispatcher to execute

    // Store constructor
    // init(), dispose() and returns a store and a model updater
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

    // Allow stores that can handle mutable 'Model types (eg, <input>.FileList). In this
    // case we can pass (fun _ _ -> true)
    type Store<'Model>(init: unit -> 'Model, dispose: 'Model -> unit, accept: 'Model -> 'Model -> bool) =
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

        member _.Get = model()

        member _.Update(f: 'Model -> 'Model) =
            if subscribers.Count > 0 then
                let newModel = f (model())

                // We need to do this only where we know it's
                // a) worth doing, and
                // b) we don't need notifications of calls to Update.
                //
                // For example, when we bind to <input>.files this will initially
                // change from null -> FileList. As the user changes their
                // selection of files, the DOM instance of FileList appears to mutate
                // We get notification of the change in files, but we are sending the
                // *same* instance into Update. The end consumer is likely to just be
                // iterating the contents, and doesn't care that the instance is the
                // same.
                //
                if accept _model newModel then
                    _model <- newModel
                    subscribers.Values
                    |> Seq.iter (fun s -> s.OnNext(_model))

        member _.Subscribe(observer: IObserver<'Model>): IDisposable =
            let id = uid
            uid <- uid + 1
            subscribers.Add(id, observer)

            // TODO: Is this the right way to report the model to the subscriber immediately?
            //Fable.Core.JS.setTimeout (fun _ -> observer.OnNext(model)) 0 |> ignore

            // Sveltish depends on an immediate callback
            observer.OnNext(model())

            Helpers.disposable <| fun () ->
                if subscribers.Remove(id) && subscribers.Count = 0 then
                    dispose (model())
                    _model <- Unchecked.defaultof<_>

        interface IStore<'Model> with
            member this.Subscribe(observer: IObserver<'Model>) = this.Subscribe(observer)
            member this.Update(f) = this.Update(f)
            member this.Get = this.Get

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

    // Assume that Elmish programs are working with immutable records as their Model
    let private acceptIfDifferent a b = Helpers.fastEquals a b |> not

    let makeElmish (init: 'Props -> 'Model * Cmd<'Msg>)
                   (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        makeElmishWithCons init update dispose (fun i d ->
            let s = Store(i, d, acceptIfDifferent)
            let u f = s.Update(f); DOM.Event.notifyDocument()
            upcast s, u)

    let makeElmishSimple (init: 'Props -> 'Model)
                   (update: 'Msg -> 'Model -> 'Model)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        let init p = init p, []
        let update msg model = update msg model, []
        makeElmishWithCons init update dispose (fun i d ->
            let s = Store(i, d, acceptIfDifferent)
            let u f = s.Update(f); DOM.Event.notifyDocument()
            upcast s, u)

