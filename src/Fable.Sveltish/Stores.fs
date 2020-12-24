module Sveltish.Stores

    open Browser.Dom
    open Browser.Event

    let newStoreId = CodeGeneration.makeIdGenerator()

    let log = Logging.log "store"

    type Store<'T> = {
        Id : int
        Value : (unit -> 'T)
        Set   : ('T -> unit)

        // Subscribe takes a callback that will be called immediately upon
        // subscription, and when the value changes
        // Result is an unsubscription function
        Subscribe : ('T -> unit) -> (unit -> unit)
    }

    type Subscriber<'T> = {
        Id : int
        Set : ('T -> unit)
    }

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

    let makeGetSetStore<'T> (get : unit -> 'T) (set : 'T -> unit) =
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

    let makeStore<'T> (v : 'T) =
        // Storage is separated from Store<T> so that it doesn't leak
        // through the abstraction.
        let mutable value = v
        let get() = value
        let set(v) = value <- v
        makeGetSetStore get set


    let makePropertyStore obj name =
        let get = Interop.getter obj name
        let set = Interop.setter obj name
        makeGetSetStore get set

    let makeExpressionStore<'T> (expr : (unit -> 'T)) =
        let mutable cache : 'T = expr()
        makeGetSetStore
            (fun () -> cache)

            // This setter will be called by forceNotify. We don't care about the incoming
            // value (which will have been from our getter() anyway), and so we use
            // this opportunity to recache the expression value.
            //
            // Code smell, since caller will be surprised that their supplied value was
            // silently ignored.
            // Ideally, the getter wants to know whether the expression has changed value
            // since it was last cached, which can be implemented by having a notification
            // when its dependencies have changed.
            (fun _ -> cache <- expr())

    let exprStore = makeExpressionStore

    let propagateNotifications (fromStore : Store<'T1>) (toStore : Store<'T2>) =
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

    let (|~>) a b = propagateNotifications a b |> ignore; b
    let (<~|) a b = propagateNotifications a b |> ignore; a

    // ----------------------------------------------------------------------------
    // Store bind/map
    // These can move to Sveltish.Stores

    //
    // Map the wrapped value. For a List<T> (instead of a Store<T>) this might be
    // called foldMap
    //
    let storeGetMap f s =
        s.Value() |> f

    //
    // Map f onto s, to produce a new store. The new store will be updated whenever
    // the source store changes
    //
    let storeMap f s =
        let get() = s.Value() |> f
        s |~> makeGetSetStore get ignore

    //
    // Monadic bind. Since f already produces a store, all we need to do
    // is propagate notifications from the source store s.
    //
    let storeBind<'A,'B> (f : ('A -> Store<'B>)) (s : Store<'A>)=
        s |~> (s.Value() |> f)


    let (|%>) s f = storeMap f s
    let (>%>) s f = storeBind f s
    let (|->) s f = storeGetMap f s

    let (<~) (s : Store<'T>) v =
        s.Set(v)

    let (<~-) (s : Store<'T>) v =
        s.Set(v)

    let (-~>) v (s : Store<'T>) =
        s.Set(v)

    let storeModify (store:Store<'T>) (map:('T -> 'T)) =
        store <~ (store |-> map)

    let (<~=) store map = storeModify store map
    let (=~>) store map = storeModify store map

    // Helpers for list stores
    let storeFetch pred (store:Store<List<'T>>) =
        store |-> (List.tryFind pred)

    let storeFetchByKey kf key (store:Store<List<'T>>) =
        let pred r = kf(r) = key
        storeFetch pred store

    // Study in how the expressions compose
    //let lotsDone'Form1 = storeMap (fun x -> x |> (listCount isDone) >= 3) todos
    //let lotsDone'Form2 = todos |>  storeMap (fun x -> x |> (listCount isDone) >= 3)
    //let lotsDone'Form3 = todos |%>  (fun x -> x |> (listCount isDone) >= 3)