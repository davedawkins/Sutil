namespace Sveltish

open System

[<RequireQualifiedAccess>]
module SimpleStore =
    let log = Logging.log "store"

    type Store<'T> =
        {
            Value : (unit -> 'T)
            Set   : ('T -> unit)
            Subscribe : ('T -> unit) -> (unit -> unit)
        } with
        member this.SubscribeImpl(f) = ()
        interface IStore<'T> with
            member this.Update(f) = this.Set(this.Value() |> f)
            member this.Get = this.Value()
            //member this.Id = this.Id
            member this.Subscribe(f) =
                let handler v = f.OnNext(v)
                this.Subscribe(handler) |> Helpers.disposable

    type Subscriber<'T> = {
        Id : int
        Set : ('T -> unit)
    }

    let set (store:Store<'T>) (value:'T) = store.Set value
    let get (store:Store<'T>) = store.Value()
    let subscribe (store:Store<'T>) f = store.Subscribe(f)

    let newSubId = Helpers.makeIdGenerator()

    let makeFromGetSet<'T> (get : unit -> 'T) (set : 'T -> unit) =
        let mutable subscribers : Subscriber<'T> list = []
        {
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

