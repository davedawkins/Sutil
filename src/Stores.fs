namespace Sveltish

module Stores =

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

    let mutable notifyLevel = 0
    let mutable waiting = []
    let startNotify() =
        notifyLevel <- notifyLevel + 1
        log(sprintf "notify: %d" notifyLevel)

    let endNotify() =
        notifyLevel <- notifyLevel - 1
        log(sprintf "notify: %d" notifyLevel)
        if (notifyLevel = 0) then
            let rec n w =
                match w with
                | [] -> ()
                | f :: xs -> f(); n xs
            let w = waiting
            waiting <- []
            n w

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

    open Fable.Core

    type Lens<'T> = {
            Get : unit -> 'T
            Set : 'T -> unit
        }

    [<Emit("() => ($0)[$1]")>]
    let getter obj name = jsNative

    [<Emit("value => { ($0)[$1] = value; }")>]
    let setter obj name = jsNative

    let makeLens obj name : Lens<obj> =
        {
            Get = getter obj name
            Set = setter obj name
        }

    let makePropertyStore obj name =
        let get = getter obj name
        let set = setter obj name
        makeGetSetStore get set

    let makeExpressionStore<'T> (expr : (unit -> 'T)) =
        let mutable cache : 'T = expr()
        makeGetSetStore
            (fun () -> cache)

            // This setter will be called by forceNotify. We don't about the incoming
            // value (which will have been from our getter() anyway), and so we use
            // this opportunity to recache the expression value.
            (fun v -> cache <- expr())

    let exprStore = makeExpressionStore

    let propagateNotifications (fromStore : Store<'T1>) (toStore : Store<'T2>) =
        let mutable init = false
        fromStore.Subscribe( fun _ ->
            if init then forceNotify toStore else init <- true
            ) |> ignore
        toStore

    let (|~>) a b = propagateNotifications a b |> ignore; b
    let (<~|) a b = propagateNotifications a b |> ignore; a

    // Subscribe wants a setter (T->unit), this converts that into a notifier (unit -> unit)
    let makeNotifier store = (fun callback -> store.Subscribe( fun _ -> callback() )  |> ignore)

