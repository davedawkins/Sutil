namespace Sveltish

type Store<'T> = {
    Id : int
    Value : (unit -> 'T)
    Set   : ('T -> unit)

    // Subscribe takes a callback that will be called immediately upon
    // subscription, and when the value changes
    // Result is an unsubscription function
    Subscribe : ('T -> unit) -> (unit -> unit)
}

[<RequireQualifiedAccess>]
module Store =

    open Browser.Dom
    open Browser.Event

    let newStoreId = CodeGeneration.makeIdGenerator()

    let log = Logging.log "store"

    type Subscriber<'T> = {
        Id : int
        Set : ('T -> unit)
    }

    let set (store:Store<'T>) (value:'T) = store.Set value
    let get (store:Store<'T>) = store.Value()
    let subscribe (store:Store<'T>) f = store.Subscribe(f)

    let subscribe2<'A,'B>  (a : Store<'A>) (b : Store<'B>)  (callback: ('A*'B) -> unit) : (unit -> unit) =
        let unsuba = a.Subscribe( fun v ->
            callback(v,b.Value())
        )
        let unsubb = b.Subscribe( fun v ->
            callback(a.Value(),v)
        )
        let unsubBoth() =
            unsuba()
            unsubb()
        unsubBoth

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

    let makeFromGetSet<'T> (get : unit -> 'T) (set : 'T -> unit) =
        let mutable subscribers : Subscriber<'T> list = []
        let myId = newStoreId()
        {
            Id = myId
            Value = get
            Set  = fun (v : 'T) ->
                startNotify()
                set(v)
                for s in subscribers do s.Set(get())
                endNotify()
            Subscribe = (fun notify ->
                let id = newSubId()
                let unsub = (fun () ->
                    subscribers <- subscribers |> List.filter (fun s -> s.Id <> id)
                )
                subscribers <- { Id = id; Set = notify } :: subscribers
                notify(get())
                unsub
            )
        }

    // Make a store from an initial value.
    let make<'T> (v : 'T) =
        // Storage is separated from Store<T> so that it doesn't leak
        // through the abstraction.
        let mutable value = v
        let get'() = value
        let set'(v) = value <- v
        makeFromGetSet get' set'

    // Make a store from a JS object property
    let property obj name =
        let get = Interop.getter obj name
        let set = Interop.setter obj name
        makeFromGetSet get set

    let makeFromProperty = property

    // Subscribe wants a setter (T->unit), this converts that into a notifier (unit -> unit)
    // This allows us to know what something changed, ignoring the type and the value. It's a
    // sign though that we intend to do a general re-evaluation to bring things back in
    // sync, so perhaps a code smell for notifications not being as fine grained as they could be
    let makeNotifier store = (fun callback -> store.Subscribe( fun _ -> callback() )  |> ignore)

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

    let (<~) (s : Store<'T>) v =
        Store.set s v

    let (<~-) (s : Store<'T>) v =
        Store.set s v

    let (-~>) v (s : Store<'T>) =
        Store.set s v

    let (<~=) store map = Store.modify map store
    let (=~>) map store = Store.modify map store
