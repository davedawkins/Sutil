namespace Sveltish

open System
open Browser.Dom
open Microsoft.FSharp.Core

module internal StoreHelpers =
    let disposable f =
        { new IDisposable with
            member _.Dispose() = f () }

[<RequireQualifiedAccess>]
module Store =

    let make (modelInit:'T) : IStore<'T> =
        let init() = modelInit
        let dispose(m) = ()
        let s = ObservableStore.makeStore init dispose (fun _ _ -> true)
        upcast s

    let get (s : IStore<'T>) : 'T = s.Get
    let set (s : IStore<'T>) v : unit = s.Update( fun _ -> v )
    let subscribe (a : IObservable<'T>) (f : 'T -> unit) = a.Subscribe(f)
    let map<'A,'B> (f : 'A -> 'B) (s : IObservable<'A>) = s |> Observable.map f
    let filter<'A> (f : 'A -> bool) (s : IObservable<'A>) = s |> Observable.filter f

    // Map the wrapped value. For a List<T> (instead of a Store<T>) this might be
    // called foldMap
    let getMap f s =
        s |> get |> f

    // Call f upon initialization and whenever the store is updated. This is the same as subscribe
    // and ignoring the unsubscription callback
    let write<'A> (f: 'A -> unit) (s : IObservable<'A>) =
        let unsub = subscribe s f
        ()

    // Modify the store by mapping its current value with f
    let modify (f:('T -> 'T)) (store:Store<'T>)  =
        store |> getMap f |> set store

    let subscribe2<'A,'B>  (a : IObservable<'A>) (b : IObservable<'B>)  (callback: ('A*'B) -> unit) : System.IDisposable =
        // Requires that subscribe makes an initializing first callback. Otherwise, there will
        // be a missing value for A (say) when B (say) sends an update.
        let mutable initState = 0

        let mutable cachea : 'A = Unchecked.defaultof<'A>
        let mutable cacheb : 'B = Unchecked.defaultof<'B>

        let notify() = if initState = 2 then callback(cachea, cacheb)

        let unsuba = subscribe a ( fun v ->
            if initState = 0 then initState <- 1
            cachea <- v
            notify()
        )

        let unsubb = subscribe b ( fun v ->
            if initState = 1 then initState <- 2
            cacheb <- v
            notify()
        )

        if (initState <> 2) then
            console.log("Error: subscribe didn't initialize us")
            failwith "Subscribe didn't initialize us"

        StoreHelpers.disposable <| fun () ->
            unsuba.Dispose()
            unsubb.Dispose()

    //let makeElmishSimple<'Props,'Model,'Msg> (init: 'Props -> 'Model) (update: ('Msg -> 'Model -> 'Model)) (dispose: 'Model -> unit) = fun () ->
    //    let s = init() |> make
    //    let d msg =
    //        s |> modify (update msg)
    //    s, d

    // Strange runtime error when type specifications are missing
    let makeElmishSimple<'Props,'Model,'Msg> (init: 'Props -> 'Model) (update: 'Msg -> 'Model -> 'Model) (dispose: 'Model -> unit) =
        ObservableStore.makeElmishSimple init update dispose

    let makeElmish<'Props,'Model,'Msg> (init: 'Props -> 'Model * ObservableStore.Cmd<'Msg>) (update: 'Msg -> 'Model -> 'Model * ObservableStore.Cmd<'Msg>) (dispose: 'Model -> unit) =
        ObservableStore.makeElmish init update dispose

[<AutoOpen>]
module StoreOperators =
    let (|%>) s f = Store.map f s
    let (|->) s f = Store.getMap f s

    let (<~) s v =
        Store.set s v

    let (<~-) s v =
        Store.set s v

    let (-~>) v s =
        Store.set s v

    let (<~=) store map = Store.modify map store
    let (=~>) map store = Store.modify map store
