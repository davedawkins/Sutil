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

/// <summary>
/// Functions for working with stores
/// </summary>
[<RequireQualifiedAccess>]
module Store =

    /// <summary>
    /// Create a new store
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    ///     let intStore: IStore&lt;int&gt; = Store.make 1
    ///
    ///     let anonymousStore:
    ///         IStore&lt;{| prop1: int;
    ///                   prop2: string option |}&gt;
    ///         = Store.make {| prop1 = 10; prop2 = None |}
    ///     (* After using the store *)
    ///     intStore.Dispose()
    ///     anonymousStore.Dispose()
    /// </code>
    /// </example>
    let make (modelInit: 'T) : IStore<'T> =
        let init () = modelInit
        let s = ObservableStore.makeStore init ignore
        upcast s

    /// <summary>
    /// Obtains the current value of the store
    /// </summary>
    /// <example><code>
    ///     let value = Store.get initStore
    ///     value = 1 // true
    ///     let value2 = Store.get anonymousStore
    ///     Option.isNone value2.prop2 // true
    /// </code></example>
    let get (store: IStore<'T>) : 'T = store.Value

    /// <summary>
    /// Replaces the current value of the store
    /// </summary>
    /// <example><code>
    ///     Store.set 2 intStore
    ///     let value = Store.get intStore
    ///     value = 1 // false
    /// </code></example>
    let set (store: IStore<'T>) newValue : unit = store.Update(fun _ -> newValue)

    /// <summary>
    /// Provides a subscription that invokes a callback
    /// every time the store value is updated
    /// </summary>
    /// <example>
    /// <code>
    ///     let subscription =
    ///         Store.subscribe (fun value -> printfn $"{value}") intStore
    ///
    ///     (* after you are done with the subscription *)
    ///
    ///     subscription.Dispose()
    /// </code>
    /// </example>
    let subscribe (callback: 'T -> unit) (store: IObservable<'T>) = store.Subscribe(callback)

    /// <summary>
    /// Returns an observable that will resolve to the result of said callback
    /// </summary>
    /// <example>
    /// <code>
    ///     let subscription: IObservable&lt;string&gt; =
    ///         Store.map (fun value -> $"{value}") intStore
    ///
    ///     (* after you are done with the subscription *)
    ///
    ///     subscription.Dispose()
    /// </code>
    /// </example>
    let map<'A, 'B> (callback: 'A -> 'B) (store: IObservable<'A>) = store |> Observable.map callback

    /// <summary>
    /// Applies a predicate function to obtain an observable of the elements that evaluated to true
    /// </summary>
    /// <example>
    /// <code>
    ///     let usersOver18: IObservable&lt;string&gt; =
    ///         Store.filter (fun user -> user.age > 18) usersStore
    ///
    ///     (* after you are done with the subscription *)
    ///
    ///     over18.Dispose()
    /// </code>
    /// </example>
    let filter<'A> (predicate: 'A -> bool) (store: IObservable<'A>) = store |> Observable.filter predicate

    /// <summary>
    /// Provides an observable that will emit a value only when the updated store value is different from the previous one
    /// </summary>
    /// <example>
    /// <code>
    ///     let store = Store.make 0
    ///     let whenDistinct = Store.distinct store
    ///     let sub1 = store.subscribe(printfn "number: %i")
    ///     let sub2 = whenDistinct.subscribe(printfn "number: %i")
    ///     Store.set 0 store // store emits
    ///     Store.set 0 store // when distinct doesn't emit
    ///     Store.set 1 store // both store and distinct emit
    ///     Store.set 1 store // when distinct doesn't emit
    /// </code>
    /// </example>
    let distinct<'T when 'T: equality> (source: IObservable<'T>) = Observable.distinctUntilChanged source

    /// <summary>
    /// Helper function for commonly used form
    /// <code>
    ///     store |> Store.map (fun s -> s.Thing) |> Observable.distinctUntilChanged
    /// </code>
    /// </summary>
    let mapDistinct (callback: 'A -> 'B) (store: IObservable<'A>) =
        store |> map callback |> distinct

    /// <summary>
    /// Merges two stores into a single tupled observable
    /// </summary>
    /// <example>
    /// <code>
    ///     let tableInfo =
    ///     Observable.zip
    ///         (Strore.map(fun model -> model.rows) model)
    ///         (Strore.map(fun model -> model.columns) model)
    ///
    ///     (* once done with tableInfo *)
    ///
    ///     tableInfo.Dispose()
    /// </code>
    /// </example>
    let zip source1 source2 = Observable.zip source1 source2

    let current (store: IObservable<'T>) =
        let mutable value = Unchecked.defaultof<'T>
        store.Subscribe(fun v -> value <- v).Dispose() // Works only when root observable is a store, or root responds immediately upon subscription
        value

    /// <summary>
    /// Takes a store and applies a mapping function then returns the value from the evaluated function
    /// </summary>
    /// <remarks>
    /// This might be called foldMap
    /// </remarks>
    /// <example>
    /// <code>
    ///     let store: IStore&lt;{| name: string; budget: decimal |}> =
    ///     Store.make {| name = "Frank"; budget = 547863.26M |}
    ///
    ///     let formattedBudget: string =
    ///         Store.getMap
    ///             (fun model -> sprintf $"$ %0.00M{model.budget}")
    ///             store
    ///     printf %"Budget available: {formattedBudget}
    /// </code>
    /// </example>
    let getMap callback store = store |> get |> callback

    /// <summary>
    /// Invoke callback for each observed value from source
    /// </summary>
    /// <example>
    /// <code>
    ///    source |> Store.iter (fun v -> printfn $"New value: {v}")
    /// </code>
    /// </example>
    let iter<'A> (callback: 'A -> unit) (source: IObservable<'A>) =
        subscribe callback source

    [<Obsolete("Use Store.iter instead")>]
    let write<'A> (callback: 'A -> unit) (store: IObservable<'A>) = iter callback store |> ignore

    /// <summary>Modify the store by mapping its current value with a callback</summary>
    /// <example>
    /// <code>
    ///     let store: IStore&lt;int> = Store.make 2
    ///
    ///     let squareMe() =
    ///         Store.modify (fun model -> model * model) store
    ///
    ///     Html.div [
    ///         bindFragment store &lt;| fun model -> text $"The value is {model}"
    ///         Html.button [
    ///             onClick (fun _ -> squareMe()) []
    ///             text "Square me"
    ///         ]
    ///     ]
    /// </code>
    /// </example>
    let modify (callback: ('T -> 'T)) (store: Store<'T>) = store |> getMap callback |> set store

    /// <summary>
    /// Takes two observables and subscribes to both with a single callback,
    /// both values will be cached individually and
    /// on every notify they will be updated and emitted,
    /// every notification can come from any of the observables
    /// </summary>
    /// <example>
    /// <code>
    ///     let player1Score = Store.make 0
    ///     let player2Score = Store.make 0
    ///
    ///     let printPlayerScores (score1: int * score2: int) =
    ///         printfn $"Player 1: {score1}\nPlayer2: {score2}"
    ///
    ///     let scores =
    ///         Store.subscribe2
    ///             player1Score
    ///             player2Score
    ///             printPlayerScore
    ///     (* Game Finished, dispose the observables *)
    ///     scores.Dispose()
    /// </code>
    /// </example>
    let subscribe2<'A, 'B>
        (source1: IObservable<'A>)
        (source2: IObservable<'B>)
        (callback: ('A * 'B) -> unit)
        : System.IDisposable =
        // Requires that subscribe makes an initializing first callback. Otherwise, there will
        // be a missing value for A (say) when B (say) sends an update.
        let mutable initState = 0

        let mutable cachea : 'A = Unchecked.defaultof<'A>
        let mutable cacheb : 'B = Unchecked.defaultof<'B>

        let notify () =
            if initState = 2 then
                callback (cachea, cacheb)

        let unsuba =
            source1
            |> subscribe
                (fun v ->
                    if initState = 0 then initState <- 1
                    cachea <- v
                    notify ())

        let unsubb =
            source2
            |> subscribe
                (fun v ->
                    if initState = 1 then initState <- 2
                    cacheb <- v
                    notify ())

        if (initState <> 2) then
            console.log ("Error: subscribe didn't initialize us")
            failwith "Subscribe didn't initialize us"

        StoreHelpers.disposable
        <| fun () ->
            unsuba.Dispose()
            unsubb.Dispose()

    ///<summary>
    /// Creates a store and a dispatch method commonly used
    /// in elmish programs, this can be used to model more complex views that require better
    /// control flow and a predictable state.
    /// </summary>
    /// <example>
    /// <code>
    ///     type State = { count: int }
    ///     type Msg =
    ///         | Increment
    ///         | Decrement
    ///         | Reset
    ///     let init _ = { count = 0 }
    ///
    ///     let upddate msg state =
    ///         match msg with
    ///         | Increment -> { state = state.count + 1 }
    ///         | Decrement -> { state = state.count - 1 }
    ///         | Reset -> { state = 0 }
    ///
    ///     let view() =
    ///         let state, dispatch = Store.makeElmishSimple init update ignore ()
    ///
    ///         Html.article [
    ///             disposeOnUnmount [ state ]
    ///             bindFragment state &lt;| fun state -> text $"Count: {state.count}"
    ///
    ///             Html.button [ text "Increment"; onClick (fun _ -> dispatch) [] ]
    ///             Html.button [ text "Decrement"; onClick (fun _ -> dispatch) [] ]
    ///             Html.button [ text "Reset"; onClick (fun _ -> dispatch Reset) [] ]
    ///         ]
    /// </code>
    /// </example>
    let makeElmishSimple<'Props, 'Model, 'Msg>
        (init: 'Props -> 'Model)
        (update: 'Msg -> 'Model -> 'Model)
        (dispose: 'Model -> unit)
        =
        ObservableStore.makeElmishSimple init update dispose

    ///<summary>
    /// Creates a store and a dispatch function as <c>Store.makeElmishSimple</c>
    /// the difference being that this version handles [Elmish commands](https://elmish.github.io/elmish/index.html#Commands)
    /// as well, generally used in more complex UIs given that with commands you can also handle
    /// asynchronous code like fetching resources from a server or calling any
    /// function that returns a promise or async
    /// </summary>
    /// <example>
    /// <code>
    ///     type State = { count: int }
    ///     type Msg =
    ///         | Increment
    ///         | Decrement
    ///         | Reset
    ///         | AsyncIncrement
    ///         | AsyncDecrement
    ///     let init _ = { count = 0 }, Cmd.ofMsg AsyncIncrement
    ///
    ///     let wait1S () =
    ///         async {
    ///             do! Async.Sleep 1000
    ///         }
    ///
    ///     let upddate msg state =
    ///         match msg with
    ///         | Increment -> { state = state.count + 1 }, Cmd.none
    ///         | Decrement -> { state = state.count - 1 }, Cmd.none
    ///         | AsyncIncrement ->
    ///             state, Cmd.ofAsync.perform () wait1S Increment
    ///         | AsyncDecrement->
    ///             state, Cmd.ofAsync.perform () wait1S Decrement
    ///         | Reset -> { state = 0 } Cmd.none
    ///
    ///     let view() =
    ///         let state, dispatch = Store.makeElmish init update ignore ()
    ///
    ///         Html.article [
    ///             disposeOnUnmount [ state ]
    ///             bindFragment state &lt;| fun state -> text $"Count: {state.count}"
    ///
    ///             Html.button [ text "Increment"; onClick (fun _ -> dispatch Increment) [] ]
    ///             Html.button [ text "Async Increment"; onClick (fun _ -> dispatch AsyncIncrement) [] ]
    ///             Html.button [ text "Decrement"; onClick (fun _ -> dispatch Decrement) [] ]
    ///             Html.button [ text "Async Decrement"; onClick (fun _ -> dispatch AsyncDecrement) [] ]
    ///             Html.button [ text "Reset"; onClick (fun _ -> dispatch Reset) [] ]
    ///         ]
    /// </code>
    /// </example>
    let makeElmish<'Props, 'Model, 'Msg>
        (init: 'Props -> 'Model * Cmd<'Msg>)
        (update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>)
        (dispose: 'Model -> unit)
        =
        ObservableStore.makeElmish init update dispose

/// <summary>
/// Operators for store functions
/// </summary>
[<AutoOpen>]
module StoreOperators =

    /// <summary>
    /// Alias for <c>Store.getMap</c>, takes a store and applies a mapping function then returns the value from the evaluated function
    /// </summary>
    /// <remarks>
    /// This might be called foldMap
    /// </remarks>
    /// <example>
    /// <code>
    ///     let store: IStore&lt;{| name: string; budget: decimal |}> =
    ///     Store.make {| name = "Frank"; budget = 547863.26M |}
    ///
    ///     let formattedBudget: string =
    ///         store |-> (fun model -> sprintf $"$ %0.00M{model.budget}")
    ///     printf %"Budget available: {formattedBudget}
    ///  </code>
    ///  </example>
    //let (|%>) s f = Store.map f s
    let (|->) s f = Store.getMap f s

    /// <summary>
    /// Alias for <c>Store.map</c>, returns an observable that will resolve to the result of said callback
    /// </summary>
    /// <example>
    /// <code>
    ///     let subscription: IObservable&lt;string&gt; =
    ///         intStore .> (fun value -> $"{value}")
    ///
    ///     (* after you are done with the subscription *)
    ///
    ///     subscription.Dispose()
    /// </code>
    /// </example>
    let (.>) s f = Store.map f s

    /// <summary>
    /// Alias for <c>Store.mapDistinct</c>
    /// </summary>
    let (.>>) s f = Store.mapDistinct f s

    /// <summary>
    /// Alias for <c>Store.set</c>,  replaces the current value of the store
    /// </summary>
    /// <example>
    /// <code>
    ///     intStore &lt;~ 2
    ///     let value = Store.get intStore
    ///     value = 1 // false
    /// </code>
    /// </example>
    let (<~) s v = Store.set s v

    /// <summary>
    /// Alias for <c>Store.set</c>, replaces the current value of the store
    /// </summary>
    /// <example>
    /// <code>
    ///     intStore &lt;~- 2
    ///     let value = Store.get intStore
    ///     value = 1 // false
    /// </code>
    /// </example>
    let (<~-) s v = Store.set s v

    /// <summary>
    /// Alias for <c>Store.set</c>,  replaces the current value of the store
    /// </summary>
    /// <example>
    /// <code>
    ///     2 -~> intStore
    ///     let value = Store.get intStore
    ///     value = 1 // false
    /// </code>
    /// </example>
    let (-~>) v s = Store.set s v

    /// <summary>
    /// Alias for <c>Store.modify</c>. Modify the store by mapping its current value with a callback
    /// </summary>
    /// <example>
    /// <code>
    ///     let store: IStore&lt;int> = Store.make 2
    ///
    ///     let squareMe() =
    ///         store &lt;~= (fun model -> model * model)
    ///
    ///     Html.div [
    ///         bindFragment store &lt;| fun model -> text $"The value is {model}"
    ///         Html.button [
    ///             onClick (fun _ -> squareMe()) []
    ///             text "Square me"
    ///         ]
    ///     ]
    /// </code>
    /// </example>
    let (<~=) store map = Store.modify map store

    /// <summary>
    /// Alias for <c>Store.modify</c> Modify the store by mapping its current value with a callback
    /// </summary>
    /// <example>
    /// <code>
    ///     let store: IStore&lt;int> = Store.make 2
    ///
    ///     let squareMe() =
    ///         (fun model -> model * model) =~> store
    ///
    ///     Html.div [
    ///         bindFragment store &lt;| fun model -> text $"The value is {model}"
    ///         Html.button [
    ///             onClick (fun _ -> squareMe()) []
    ///             text "Square me"
    ///         ]
    ///     ]
    /// </code>
    /// </example>
    let (=~>) map store = Store.modify map store



/// <exclude/>
[<AutoOpen>]
module StoreExtensions =

    // No uses of this in the Sutil code base.
    //
    // One issue is that 'u' should be cleaned up somewhere, so perhaps turning this into
    // a SutilElement would improve it?
    // Appears to return a store of indexes, whose value will always be the first observable
    // that is currently <c>true</c>
    //
    let firstOf (selectors: IObservable<bool> list) =
        let matches = new HashSet<int>()
        let mutable current = -1
        let s = Store.make current

        let setMatch i state =
            if state then
                matches.Add(i) |> ignore
            else
                matches.Remove(i) |> ignore

        let scan () =
            let next =
                matches
                |> Seq.fold (fun a i -> if a < 0 || i < a then i else a) -1

            if (next <> current) then
                s <~ next
                current <- next

        selectors
        |> List.iteri
            (fun i pred ->
                let u =
                    pred.Subscribe
                        (fun state ->
                            setMatch i state
                            scan ())

                ())

        s
