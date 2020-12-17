module Sveltish
open Browser.Dom

let newStoreId = Fvelize.makeIdGenerator()

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

let newSubId = Fvelize.makeIdGenerator()

let makeGetSetStore<'T> (get : unit -> 'T) (set : 'T -> unit) =
    let mutable subscribers : Subscriber<'T> list = []
    let myId = newStoreId()
    console.log(sprintf "New store #%d = %A" myId (get()))
    {
        Id = myId
        Value = get
        Set  = fun (v : 'T) ->
            set(v)
            console.log(sprintf "%d: Notify %d subscribers of value %A" myId subscribers.Length v)
            for s in subscribers do
//                console.log( sprintf "%d: Notifying %A" myId v )
                s.Set(get())
        Subscribe = (fun notify ->
            let id = newSubId()
            let unsub = (fun () ->
                subscribers <- subscribers |> List.filter (fun s -> s.Id = id)
            )
            subscribers <- { Id = id; Set = notify } :: subscribers
            console.log(sprintf "%d: %d subscribers" myId subscribers.Length)
            notify(get())
            unsub
        )
    }

let forceNotify (store : Store<'T>) =
    console.log(sprintf "%d: forceNotify %A" store.Id  (store.Value()))
    store.Value() |> store.Set

let makeStore<'T> (v : 'T) =
    // Storage is separated from Store<T> so that it doesn't leak
    // through the abstraction.
    let mutable value = v
    let get() = value
    let set(v) = value <- v
    makeGetSetStore get set

open Fable.Core
open Fvelize

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
        (fun v ->
            let vreal = expr()
            console.log(sprintf "expression set : eval = %A" vreal)
            cache <- expr())

let exprStore = makeExpressionStore

let propagateNotifications (fromStore : Store<'T1>) (toStore : Store<'T2>) =
    let mutable init = false
    fromStore.Subscribe( fun _ ->
        console.log( sprintf "Received update from %d" fromStore.Id)
        if init then forceNotify toStore else init <- true
        ) |> ignore
    toStore

let (|~>) a b = propagateNotifications a b

// Dependency here on the DOM builder
let bind<'T> (store : Store<'T>) (e : unit -> Fvelize.ElementChild) =
    Fvelize.Binding (
            e,
            (fun callback ->
                store.Subscribe( fun newValue -> callback() )
                |> ignore)
    )

let bindVisibility<'T> (store : Store<bool>) (e : Fvelize.ElementChild) =
    console.log(sprintf "Binding visibility of %A to store #%d" e (store.Id))
    Fvelize.BindingVisibility (
            e,
            store.Subscribe,
            None )

let transition<'T> (trans : TransitionAttribute) (store : Store<bool>) (e : Fvelize.ElementChild) =
    console.log(sprintf "Binding transition of %A to store #%d" e (store.Id))
    Fvelize.BindingVisibility (
            e,
            store.Subscribe,
            Some trans )

let bindAttr (store : Store<obj>) attrName =
    Fvelize.BindingAttribute (attrName,
        store.Subscribe,
        store.Set )