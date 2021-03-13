namespace Sutil

open System
open Browser.Dom
open Microsoft.FSharp.Core
open Browser.Types
open System.Collections.Generic

module internal StoreHelpers =
    let disposable f =
        { new IDisposable with
            member _.Dispose() = f () }

[<RequireQualifiedAccess>]
module Store =

    let make (modelInit:'T) : IStore<'T> =
        let init() = modelInit
        let s = ObservableStore.makeStore init ignore
        upcast s

    let get (s : IStore<'T>) : 'T = s.Value
    let set (s : IStore<'T>) v : unit = s.Update( fun _ -> v )
    let subscribe (f : 'T -> unit) (a : IObservable<'T>) = a.Subscribe(f)
    let map<'A,'B> (f : 'A -> 'B) (s : IObservable<'A>) = s |> Observable.map f
    let filter<'A> (f : 'A -> bool) (s : IObservable<'A>) = s |> Observable.filter f
    let distinct<'T when 'T : equality> (source : IObservable<'T>) = Observable.distinctUntilChanged source
    let zip a b = Observable.zip a b

    let current (o : IObservable<'T>) =
        let mutable value = Unchecked.defaultof<'T>
        o.Subscribe(fun v -> value <- v).Dispose() // Works only when root observable is a store, or root responds immediately upon subscription
        value

    // Map the wrapped value. For a List<T> (instead of a Store<T>) this might be
    // called foldMap
    let getMap f s =
        s |> get |> f

    // Call f upon initialization and whenever the store is updated. This is the same as subscribe
    // and ignoring the unsubscription callback
    let write<'A> (f: 'A -> unit) (s : IObservable<'A>) =
        let unsub = subscribe f s
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

        let unsuba = a |> subscribe (fun v ->
            if initState = 0 then initState <- 1
            cachea <- v
            notify()
        )

        let unsubb = b |> subscribe ( fun v ->
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

    // Strange runtime error when type specifications are missing
    let makeElmishSimple<'Props,'Model,'Msg> (init: 'Props -> 'Model) (update: 'Msg -> 'Model -> 'Model) (dispose: 'Model -> unit) =
        ObservableStore.makeElmishSimple init update dispose

    let makeElmish<'Props,'Model,'Msg> (init: 'Props -> 'Model * Cmd<'Msg>) (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>) (dispose: 'Model -> unit) =
        ObservableStore.makeElmish init update dispose

[<AutoOpen>]
module StoreOperators =
    //let (|%>) s f = Store.map f s
    let (|->) s f = Store.getMap f s

    let (.>) s f = Store.map f s

    let (<~) s v =
        Store.set s v

    let (<~-) s v =
        Store.set s v

    let (-~>) v s =
        Store.set s v

    let (<~=) store map = Store.modify map store
    let (=~>) map store = Store.modify map store

[<AutoOpen>]
module StoreExtensions =

    let firstOf (selectors : IObservable<bool> list) =
        let matches = new HashSet<int>()
        let mutable current = -1
        let s = Store.make current

        let setMatch i state =
            if state then
                matches.Add(i) |> ignore
            else
                matches.Remove(i) |> ignore

        let scan() =
            let next = matches |> Seq.fold (fun a i -> if a < 0 || i < a then i else a) -1
            if (next <> current) then
                s <~ next
                current <- next

        selectors |> List.iteri (fun i pred ->
            let u = pred.Subscribe( fun state ->
                setMatch i state
                scan()
            )
            () )
        s
