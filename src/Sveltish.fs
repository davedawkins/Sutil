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
let mutable nextSubId = 0
let newSubId() =
    let r = nextSubId
    nextSubId <- nextSubId + 1
    r

let makeStore<'T> (v : 'T) =
    // Storage is separated from Store<T> so that it doesn't leak
    // through the abstraction.
    let mutable value = v
    let mutable subscribers : Subscriber<'T> list = []
    let get() = value
    {
        Value = get
        Set  = fun (v : 'T) -> value <- v; for s in subscribers do s.Set(v)
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

// Dependency here on the DOM builder
let bind<'T> (e : (unit -> Fvelize.ElementChild)) (store : Store<'T>) =
    Fvelize.BoundNode (
            e,
            (fun callback ->
                store.Subscribe( fun newValue -> callback() )
                |> ignore)
    )
