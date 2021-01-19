namespace Sutil

open System

[<RequireQualifiedAccess>]
module ObservableX =

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

    let zip<'A,'B> (a:IObservable<'A>) (b:IObservable<'B>) : IObservable<'A*'B> =
        { new System.IObservable<'A*'B> with
            member _.Subscribe( h : IObserver<'A*'B> ) =
                let mutable initState = 0
                let mutable valueA = Unchecked.defaultof<'A>
                let mutable valueB = Unchecked.defaultof<'B>

                let notify() =
                    if initState = 2 then h.OnNext( valueA, valueB )

                let disposeA = a.Subscribe( fun v ->
                    if (initState = 0) then initState <- 1
                    valueA <- v
                    notify()
                )

                let disposeB = b.Subscribe( fun v ->
                    if (initState = 1) then initState <- 2
                    valueB <- v
                    notify()
                )

                Helpers.disposable (fun _ ->
                    disposeA.Dispose()
                    disposeB.Dispose()
                )
        }

    let distinctUntilChangedCompare<'T> (source:IObservable<'T>) (eq:'T -> 'T -> bool): IObservable<'T> =
        { new System.IObservable<'T> with
            member _.Subscribe( h : IObserver<'T> ) =
                let mutable value = Unchecked.defaultof<'T>

                let disposeA = source.Subscribe( fun next ->
                    if not (eq value next) then
                        h.OnNext next
                        value <- next
                )

                Helpers.disposable (fun _ ->
                    disposeA.Dispose()
                )
        }

    let distinctUntilChanged<'T when 'T : equality> (source:IObservable<'T>) : IObservable<'T> =
        distinctUntilChangedCompare source (=)
