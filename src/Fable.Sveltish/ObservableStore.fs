namespace Sveltish

open System

type ObservableStore<'Model> =
    inherit IObservable<'Model>
    abstract Update: f:('Model -> 'Model) -> unit

[<RequireQualifiedAccess>]
module ObservableStore =

    type IStore<'Model> = ObservableStore<'Model>

    open Browser.Dom
    open Browser.Event

    type Update<'Model> = ('Model -> 'Model) -> unit
    type Dispatch<'Msg> = 'Msg -> unit
    type Cmd<'Msg> = (Dispatch<'Msg> -> unit) list

    type StoreCons<'Model, 'Store> = (unit -> 'Model) -> ('Model -> unit) -> 'Store * Update<'Model>

    module internal Helpers =
        type CmdHandler<'Msg>(handler, ?dispose) =
            member _.Handle(cmd: Cmd<'Msg>): unit = handler cmd
            member _.Dispose() = match dispose with Some d -> d () | None -> ()
            interface IDisposable with
                member this.Dispose() = this.Dispose()

        let disposable f =
            { new IDisposable with
                member _.Dispose() = f () }

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

    type Store<'Model>(init: unit -> 'Model, dispose: 'Model -> unit) =
        let mutable uid = 0
        let mutable model = Unchecked.defaultof<_>

        let subscribers =
            Collections.Generic.Dictionary<_, IObserver<'Model>>()

        member _.Update(f: 'Model -> 'Model) =
            if subscribers.Count > 0 then
                let newModel = f model

                if not (Helpers.fastEquals model newModel) then
                    model <- newModel

                    subscribers.Values
                    |> Seq.iter (fun s -> s.OnNext(model))

        member _.Subscribe(observer: IObserver<'Model>): IDisposable =
            if subscribers.Count = 0 then model <- init ()
            let id = uid
            uid <- uid + 1
            subscribers.Add(id, observer)

            // TODO: Is this the right way to report the model to the subscriber immediately?
            Fable.Core.JS.setTimeout (fun _ -> observer.OnNext(model)) 0 |> ignore

            { new IDisposable with
                member _.Dispose() =
                    if subscribers.Remove(id) && subscribers.Count = 0 then
                        dispose model
                        model <- Unchecked.defaultof<_> }

        interface IStore<'Model> with
            member this.Update(f) = this.Update(f)
            member this.Subscribe(observer: IObserver<'Model>) = this.Subscribe(observer)



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
            | Some storeDispatch -> storeDispatch
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

    let makeElmish (init: 'Props -> 'Model * Cmd<'Msg>)
                   (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        makeElmishWithCons init update dispose (fun i d ->
            let s = Store(i, d)
            upcast s, s.Update)

    let makeElmishSimple (init: 'Props -> 'Model)
                   (update: 'Msg -> 'Model -> 'Model)
                   (dispose: 'Model -> unit)
                   : 'Props -> IStore<'Model> * Dispatch<'Msg> =

        let init p = init p, []
        let update msg model = update msg model, []
        makeElmishWithCons init update dispose (fun i d ->
            let s = Store(i, d)
            upcast s, s.Update)

    let newStoreId = CodeGeneration.makeIdGenerator()

    let log = Logging.log "store"

    let newSubId = CodeGeneration.makeIdGenerator()

    //
    // For a given triggering event (eg user checks a box) store subscriptions may want
    // to defer side effects until all subscriptions have been notified
    //

    let mutable notifyLevel = 0
    let mutable waiting = []
    let startNotify() =
        notifyLevel <- notifyLevel + 1

    let notifyDocument() =
        document.dispatchEvent( Interop.customEvent DOM.Event.Updated  {|  |} ) |> ignore

    let endNotify() =
        notifyLevel <- notifyLevel - 1
        if (notifyLevel = 0) then
            let rec n w =
                match w with
                | [] -> ()
                | f :: xs -> f(); n xs
            let w = waiting
            waiting <- []
            n w
            notifyDocument()

    let waitEndNotify f =
        if (notifyLevel = 0)
            then f()
            else waiting <- f :: waiting
