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

    let forceNotify (store : Store<'T>) =
        store.Value() |> store.Set

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

    //let expr = makeFromExpression

    let link (fromStore : Store<'T1>) (toStore : Store<'T2>) =
        let mutable init = false
        fromStore.Subscribe( fun _ ->
            if init then forceNotify toStore else init <- true
            ) |> ignore
        toStore

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

    // Modify the store by mapping its current value with f
    //
    let modify (store:Store<'T>) (f:('T -> 'T)) =
        store |> getMap f |> set store

    // Helpers for list stores
    let fetch pred (store:Store<List<'T>>) =
        store |> getMap (List.tryFind pred)

    let fetchByKey kf key (store:Store<List<'T>>) =
        let pred r = kf(r) = key
        fetch pred store

    // Modifies store list elements in place, SQL-like
    // Finally, notifies store subscribers
    // Granularity of notification is "the list changed", but no modification may have actually
    // taken place (eg, filter selected none, modify is identity), and no information about what
    // changed is given
    // However this simplies some simple UI view scenarios (see Todos.fs)
    let modifyWhere (select: 'T -> bool) (modify: 'T -> unit) (store:Store<List<'T>>) =
        for x in store |> get |> List.filter select do
            x |> modify
        forceNotify store

[<AutoOpen>]
module StoreOperators =
    let (|~>) a b = Store.link a b
    let (<~|) a b = Store.link a b |> ignore; a

    let (|%>) s f = Store.map f s
    let (|->) s f = Store.getMap f s

    let (<~) (s : Store<'T>) v =
        Store.set s v

    let (<~-) (s : Store<'T>) v =
        Store.set s v

    let (-~>) v (s : Store<'T>) =
        Store.set s v

    let (<~=) store map = Store.modify store map
    let (=~>) map store = Store.modify store map

    // Study in how the expressions compose
    //let lotsDone'Form1 = storeMap (fun x -> x |> (listCount isDone) >= 3) todos
    //let lotsDone'Form2 = todos |>  storeMap (fun x -> x |> (listCount isDone) >= 3)
    //let lotsDone'Form3 = todos |%>  (fun x -> x |> (listCount isDone) >= 3)