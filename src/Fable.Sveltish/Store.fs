namespace Sveltish

open System

type Store<'Model> =
    inherit IObservable<'Model>
    abstract Update: f:('Model -> 'Model) -> unit

// type Store<'T> = {
//     Id : int
//     Value : (unit -> 'T)
//     Set   : ('T -> unit)

//     // Subscribe takes a callback that will be called immediately upon
//     // subscription, and when the value changes
//     // Result is an unsubscription function
//     Subscribe : ('T -> unit) -> (unit -> unit)
// }

[<RequireQualifiedAccess>]
module Store =
    
    type IStore<'Model> = Store<'Model>
    
    

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

        // member _.SubscribeImmediate(observer: IObserver<'Model>): 'Model * IDisposable =
        //     if subscribers.Count = 0 then
        //         model <- init()
        //     let id = uid
        //     uid <- uid + 1
        //     subscribers.Add(id, observer)
        //     let disposable =
        //         { new IDisposable with
        //             member _.Dispose() =
        //                 if subscribers.Remove(id) && subscribers.Count = 0 then
        //                     dispose model
        //                     model <- Unchecked.defaultof<_> }
        //     model, disposable

        // member this.SubscribeImmediate(observer: 'Model -> unit): 'Model * IDisposable =
        //     let observer =
        //         { new IObserver<_> with
        //             member _.OnNext(value) = observer value
        //             member _.OnCompleted() = ()
        //             member _.OnError(_error: exn) = () }
        //     this.SubscribeImmediate(observer)

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

    // type Subscriber<'T> = {
    //     Id : int
    //     Set : ('T -> unit)
    // }

    // let set (store:Store<'T>) (value:'T) = store.Set value
    // let get (store:Store<'T>) = store.Value()
    // let subscribe (store:Store<'T>) f = store.Subscribe(f)

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
        document.dispatchEvent( Interop.customEvent "sveltish-updated"  {|  |} ) |> ignore

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

    // let makeFromGetSet<'T> (get : unit -> 'T) (set : 'T -> unit) =
    //     let mutable subscribers : Subscriber<'T> list = []
    //     let myId = newStoreId()
    //     {
    //         Id = myId
    //         Value = get
    //         Set  = fun (v : 'T) ->
    //             startNotify()
    //             set(v)
    //             for s in subscribers do s.Set(get())
    //             endNotify()
    //         Subscribe = (fun notify ->
    //             let id = newSubId()
    //             let unsub = (fun () ->
    //                 subscribers <- subscribers |> List.filter (fun s -> s.Id <> id)
    //             )
    //             subscribers <- { Id = id; Set = notify } :: subscribers
    //             notify(get())
    //             unsub
    //         )
    //     }

    // let forceNotify (store : Store<'T>) =
    //     store.Value() |> store.Set

//     let make<'T> (v : 'T) =
//         // Storage is separated from Store<T> so that it doesn't leak
//         // through the abstraction.
//         let mutable value = v
//         let get'() = value
//         let set'(v) = value <- v
//         makeFromGetSet get' set'

//     let makeFromProperty obj name =
//         let get = Interop.getter obj name
//         let set = Interop.setter obj name
//         makeFromGetSet get set

//     let makeFromExpression<'T> (expr : (unit -> 'T)) =
//         let mutable cache : 'T = expr()
//         makeFromGetSet
//             (fun () -> cache)

//             // This setter will be called by forceNotify. We don't care about the incoming
//             // value (which will have been from our getter() anyway), and so we use
//             // this opportunity to recache the expression value.
//             //
//             // Code smell, since caller will be surprised that their supplied value was
//             // silently ignored.
//             // Ideally, the getter wants to know whether the expression has changed value
//             // since it was last cached, which can be implemented by having a notification
//             // when its dependencies have changed.
//             (fun _ -> cache <- expr())

//     let expr = makeFromExpression

//     let link (fromStore : Store<'T1>) (toStore : Store<'T2>) =
//         let mutable init = false
//         fromStore.Subscribe( fun _ ->
//             if init then forceNotify toStore else init <- true
//             ) |> ignore
//         toStore

//     // Subscribe wants a setter (T->unit), this converts that into a notifier (unit -> unit)
//     // This allows us to know what something changed, ignoring the type and the value. It's a
//     // sign though that we intend to do a general re-evaluation to bring things back in
//     // sync, so perhaps a code smell for notifications not being as fine grained as they could be
//     let makeNotifier store = (fun callback -> store.Subscribe( fun _ -> callback() )  |> ignore)

//     //
//     // Map the wrapped value. For a List<T> (instead of a Store<T>) this might be
//     // called foldMap
//     //
//     let getMap f s =
//         s |> get |> f

//     //
//     // Map f onto s, to produce a new store. The new store will be updated whenever
//     // the source store changes
//     //
//     let map<'A,'B> (f : 'A -> 'B) (s : Store<'A>) =
//         let result = s |> getMap f |> make
//         let unsub = subscribe s (f >> (set result))
//         result

//     let modify (store:Store<'T>) (f:('T -> 'T)) =
//         store |> getMap f |> set store

//     // Helpers for list stores
//     let fetch pred (store:Store<List<'T>>) =
//         store |> getMap (List.tryFind pred)

//     let fetchByKey kf key (store:Store<List<'T>>) =
//         let pred r = kf(r) = key
//         fetch pred store

// [<AutoOpen>]
// module StoreOperators =
//     let (|~>) a b = Store.link a b |> ignore; b
//     let (<~|) a b = Store.link a b |> ignore; a

//     let (|%>) s f = Store.map f s
//     let (|->) s f = Store.getMap f s

//     let (<~) (s : Store<'T>) v =
//         Store.set s v

//     let (<~-) (s : Store<'T>) v =
//         Store.set s v

//     let (-~>) v (s : Store<'T>) =
//         Store.set s v

//     let (<~=) store map = Store.modify store map
//     let (=~>) store map = Store.modify store map

//     // Study in how the expressions compose
//     //let lotsDone'Form1 = storeMap (fun x -> x |> (listCount isDone) >= 3) todos
//     //let lotsDone'Form2 = todos |>  storeMap (fun x -> x |> (listCount isDone) >= 3)
//     //let lotsDone'Form3 = todos |%>  (fun x -> x |> (listCount isDone) >= 3)