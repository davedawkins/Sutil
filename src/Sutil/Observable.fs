namespace Sutil

open System

/// <summary>
/// Helper functions for <c>IObservables</c>
/// </summary>
[<RequireQualifiedAccess>]
module Observable =

    [<AbstractClass>]
    type BasicObserver<'T>() =

        let mutable stopped = false
        abstract Next : value : 'T -> unit
        abstract Error : error : exn -> unit
        abstract Completed : unit -> unit

        interface IObserver<'T> with
            member x.OnNext value =
                if not stopped then x.Next value
            member x.OnError e =
                if not stopped then stopped <- true; x.Error e
            member x.OnCompleted () =
                if not stopped then stopped <- true; x.Completed ()

    let map2<'A, 'B, 'Res> (f: 'A -> 'B -> 'Res) (a: IObservable<'A>) (b: IObservable<'B>) : IObservable<'Res> =
        { new IObservable<'Res> with
            member _.Subscribe(h: IObserver<'Res>) =
                let mutable valueA, valueB = None, None

                let notify() =
                     if valueA.IsSome && valueB.IsSome then h.OnNext (f valueA.Value valueB.Value)

                let disposeA = a.Subscribe(fun v -> valueA <- Some v; notify())
                let disposeB = b.Subscribe(fun v -> valueB <- Some v; notify())

                Helpers.disposable(fun _ -> disposeA.Dispose(); disposeB.Dispose())
        }

    let zip<'A,'B> (a:IObservable<'A>) (b:IObservable<'B>) : IObservable<'A*'B> =
        map2<'A, 'B, 'A * 'B> (fun a b -> a, b) a b

    let distinctUntilChangedCompare<'T> (eq:'T -> 'T -> bool) (source:IObservable<'T>) : IObservable<'T> =
        { new System.IObservable<'T> with
            member _.Subscribe( h : IObserver<'T> ) =
                let mutable value = Unchecked.defaultof<'T>
                let mutable init = false

                // For Fable: isNull(unbox(None)) = true
                // Can't use Unchecked.defaultof<'T> as meaning "init = false"
                let safeEq next = init && eq value next

                let disposeA = source.Subscribe( fun next ->
                    if not (safeEq next) then
                        h.OnNext next
                        value <- next
                        init <- true
                )

                Helpers.disposable (fun _ ->
                    disposeA.Dispose()
                )
        }

    let distinctUntilChanged<'T when 'T : equality> (source:IObservable<'T>) : IObservable<'T> =
        source |> distinctUntilChangedCompare (=)

    /// Determines whether an observable sequence contains a specified value
    /// which satisfies the given predicate
    let exists predicate (source: IObservable<'T>) =
        { new System.IObservable<'T> with
            member _.Subscribe( h : IObserver<'T> ) =
                let disposeA = source.Subscribe( fun x ->
                    try h.OnNext (predicate x)
                    with ex -> h.OnError ex
                )
                Helpers.disposable (fun _ -> disposeA.Dispose() )
        }

    /// Filters the observable elements of a sequence based on a predicate
    let filter predicate (source: IObservable<'T>) =
        { new System.IObservable<'T> with
            member _.Subscribe( h : IObserver<'T> ) =
                let disposeA = source.Subscribe( fun x ->
                    try if predicate x then h.OnNext x
                    with ex -> h.OnError ex
                )
                Helpers.disposable (fun _ -> disposeA.Dispose() )
        }

    //let choose (f : 'T option -> 'R option) (source:IObservable<'T option>) : IObservable<'R> =
    //    { new System.IObservable<_> with
    //        member _.Subscribe( h : IObserver<_> ) =
    //            let disposeA = source.Subscribe( fun x ->
    //                (try f x with ex -> h.OnError ex;None) |> Option.iter h.OnNext
    //            )
    //            Helpers.disposable (fun _ -> disposeA.Dispose() )
    //    }
