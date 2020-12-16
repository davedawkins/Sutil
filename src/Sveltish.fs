module Sveltish

type Store<'T> = {
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
    {
        Value = get
        Set  = fun (v : 'T) -> set(v); for s in subscribers do s.Set(v)
        Subscribe = (fun notify ->
            let id = newSubId()
            let unsub = (fun () ->
                subscribers <- subscribers |> List.filter (fun s -> s.Id = id)
            )
            subscribers <- { Id = id; Set = notify } :: subscribers
            notify(get())
            unsub
        )
    }

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

[<Emit("() => ($0).$1")>]
let getter obj name = jsNative

[<Emit("value => ($0).$1 = value")>]
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

// Dependency here on the DOM builder
let bind<'T> (store : Store<'T>) (e : unit -> Fvelize.ElementChild) =
    Fvelize.Binding (
            e,
            (fun callback ->
                store.Subscribe( fun newValue -> callback() )
                |> ignore)
    )

let bindAttr (store : Store<obj>) attrName =
    Fvelize.BindingAttribute (attrName,
        store.Subscribe,
        store.Set )