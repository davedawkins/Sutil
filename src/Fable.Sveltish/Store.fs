namespace Sveltish

open System
open Browser.Dom

module internal StoreHelpers =
    let disposable f =
        { new IDisposable with
            member _.Dispose() = f () }

[<RequireQualifiedAccess>]
module Store =
#if USE_SIMPLE_STORE
    let private st (a:IStore<'T>) : SimpleStore.Store<'T> = downcast a

    let make (init:'T) : IStore<'T> = (SimpleStore.make init) :> IStore<'T>
    let get (s : IStore<'T>) = SimpleStore.get (st s)
    let set (s : IStore<'T>) v = SimpleStore.set (st s)  v
    let subscribe (a : IStore<'T>) (f : 'T -> unit) : IDisposable =
        SimpleStore.subscribe (st a) f |> StoreHelpers.disposable
#else
    let private st (a:IStore<'T>) : ObservableStore.Store<'T> = downcast a

    let make (modelInit:'T) : IStore<'T> =
        let init() = modelInit
        let dispose(m) = ()
        let s = ObservableStore.Store( init, dispose )
        upcast s

    let get (s : IStore<'T>) : 'T = (st s).Get
    let set (s : IStore<'T>) v : unit = (st s).Update( fun _ -> v )
    let subscribe (a : IStore<'T>) (f : 'T -> unit) = (st a).Subscribe(f)
#endif

    // Map the wrapped value. For a List<T> (instead of a Store<T>) this might be
    // called foldMap
    let getMap f s =
        s |> get |> f

    // Call f upon initialization and whenever the store is updated. This is the same as subscribe
    // and ignoring the unsubscription callback
    let write<'A,'B> (f: 'A -> unit) (s : Store<'A>) =
        let unsub = subscribe s f
        ()

    // Map f onto s, to produce a new store. The new store will be updated whenever
    // the source store changes
    //
    let map<'A,'B> (f : 'A -> 'B) (s : Store<'A>) =
        let result = s |> getMap f |> make // Initialize with mapped value
        let unsub = subscribe s (f >> (set result))
        result

    // Modify the store by mapping its current value with f
    //
    let modify (f:('T -> 'T)) (store:Store<'T>)  =
        store |> getMap f |> set store

    // Helpers for list stores
    let fetch pred (store:Store<List<'T>>) =
        store |> getMap (List.tryFind pred)

    let fetchByKey kf key (store:Store<List<'T>>) =
        let pred r = kf(r) = key
        fetch pred store

    let subscribe2<'A,'B>  (a : IStore<'A>) (b : IStore<'B>)  (callback: ('A*'B) -> unit) : System.IDisposable =
        let unsuba = subscribe a ( fun v ->
            callback(v,get b)
        )
        let unsubb = subscribe b ( fun v ->
            callback(get a,v)
        )
        StoreHelpers.disposable <| fun () ->
            unsuba.Dispose()
            unsubb.Dispose()

    let map2<'A,'B,'R> (f : ('A * 'B) -> 'R) (a : Store<'A>) (b : Store<'B>)=
        let result = make( (get a, get b) |> f )
        let unsub = subscribe2 a b <| fun (x,y) -> set result ((x, y) |> f)
        result


#if DEBUG
    let makeElmishSimple init update _ = fun () ->
        let s = init() |> make
        let d msg =
            s |> modify (update msg)
        s, d
#else
    let makeElmishSimple = ObservableStore.makeElmishSimple
#endif

[<AutoOpen>]
module StoreOperators =
    let (|%>) s f = Store.map f s
    let (|->) s f = Store.getMap f s

    let (<~) (s : IStore<'T>) v =
        Store.set s v

    let (<~-) (s : IStore<'T>) v =
        Store.set s v

    let (-~>) v (s : IStore<'T>) =
        Store.set s v

    let (<~=) store map = Store.modify map store
    let (=~>) map store = Store.modify map store
