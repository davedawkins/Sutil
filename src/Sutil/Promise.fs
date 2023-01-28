namespace Sutil

open System
open Fable.Core

/// <summary>
/// Support for <c>IObservable&lt;Promise&lt;T>></c>
/// </summary>
[<AutoOpen>]
module ObservablePromise =

    [<RequireQualifiedAccess>]
    type PromiseState<'T> =
        | Waiting
        | Result of 'T
        | Error of Exception

    type ObservablePromise<'T>(p : JS.Promise<'T>) =
        let store = Store.make PromiseState.Waiting
        // TODO: Clean up store

        let run () =
                store <~ PromiseState.Waiting
                p |> Promise.map (fun v -> store <~ PromiseState.Result v)
                  |> Promise.catch (fun x -> store <~ PromiseState.Error x)
                  |> ignore

        do
            run()

        interface IObservable<PromiseState<'T>> with
            member this.Subscribe(observer: IObserver<PromiseState<'T>>) = store.Subscribe(observer)

    type JS.Promise<'T> with
        member self.ToObservable() : ObservablePromise<'T> =
            ObservablePromise<'T>(self)

