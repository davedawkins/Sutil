namespace Sveltish

// Store API in use

[<RequireQualifiedAccess>]
module Store =
#if USE_OBSERVABLE_STORE
    let private st (a:IStore<'T>) : ObservableStore.Store<'T> = a :?> ObservableStore.Store<'T>

    let make (init:'T) : IStore<'T> = (ObservableStore.make init) :> IStore<'T>
    let get (s : IStore<'T>) = ObservableStore.get (st s)
    let set (s : IStore<'T>) v = ObservableStore.set (st s)  v
    let subscribe (a : IStore<'T>) f = ObservableStore.subscribe (st a) f
    let subscribe2 (a : IStore<'A>) (b : IStore<'B>) f = ObservableStore.subscribe2 (st a) (st b) f
    let waitEndNotify f = ObservableStore.waitEndNotify f
    let sid s = (st s).Id
#else
    let private st (a:IStore<'T>) : SimpleStore.Store<'T> = a :?> SimpleStore.Store<'T>

    let make (init:'T) : IStore<'T> = (SimpleStore.make init) :> IStore<'T>
    let get (s : IStore<'T>) = SimpleStore.get (st s)
    let set (s : IStore<'T>) v = SimpleStore.set (st s)  v
    let subscribe (a : IStore<'T>) f = SimpleStore.subscribe (st a) f
    let subscribe2 (a : IStore<'A>) (b : IStore<'B>) f = SimpleStore.subscribe2 (st a) (st b) f
    let waitEndNotify f = SimpleStore.waitEndNotify f
    let sid s = (st s).Id
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

    let map2<'A,'B,'R> (f : ('A * 'B) -> 'R) (a : Store<'A>) (b : Store<'B>)=
        let result = make( (get a, get b) |> f )
        let unsub = subscribe2 a b <| fun (x,y) -> set result ((x, y) |> f)
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
