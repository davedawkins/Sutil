namespace Sveltish

[<RequireQualifiedAccess>]
module SimpleStore =
    let newStoreId = CodeGeneration.makeIdGenerator()

    let log = Logging.log "store"

    type Store<'T> =
        {
            Id : int
            Value : (unit -> 'T)
            Set   : ('T -> unit)

            // Subscribe takes a callback that will be called immediately upon
            // subscription, and when the value changes
            // Result is an unsubscription function
            Subscribe : ('T -> unit) -> (unit -> unit)
        }
        interface IStore<'T>

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

    let makeFromGetSet<'T> (get : unit -> 'T) (set : 'T -> unit) =
        let mutable subscribers : Subscriber<'T> list = []
        let myId = newStoreId()
        {
            Id = myId
            Value = get
            Set  = fun (v : 'T) ->
                set(v)
                for s in subscribers do s.Set(get())
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

