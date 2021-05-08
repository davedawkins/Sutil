module TimerLogic

/// A component that supplies state logic and no visual elements
/// It's an extreme example, but it demonstrates that it is possible
/// to make use of componentized services or behaviour

open Sutil
open type Feliz.length
open Sutil.DOM
open System

type Model = {
    TimerTask : (unit -> unit) option; // Unsubscribe function for current timer, if any
    StartedAt : DateTime
    Elapsed : float
}

let stopTimerTask m = m.TimerTask |> Option.iter (fun f -> f())
let isRunning m = m.TimerTask.IsSome

type Message =
    |StartTimer
    |StopTimer
    |Tick
    |SetTask of (unit -> unit) option

let init () = { StartedAt = DateTime.UtcNow; Elapsed = 0.0; TimerTask = None }, Cmd.none

let update msg model =
    match msg with
    | StartTimer ->
        { model with
            StartedAt = DateTime.UtcNow
            Elapsed = 0.0
            }, [ fun d -> interval (fun _ -> Tick |> d) 1000 |> Some |> SetTask |> d ]
    | StopTimer ->
        { model with Elapsed = 0.0 }, Cmd.ofMsg (SetTask None)
    | Tick ->
        { model with Elapsed = (DateTime.UtcNow - model.StartedAt).TotalSeconds }, Cmd.none
    | SetTask t ->
        model |> stopTimerTask // Dispose existing timer
        { model with TimerTask = t }, Cmd.none

let create (run : IObservable<bool>) (view : IObservable<bool * float> -> SutilElement) =

    let log s = Browser.Dom.console.log s
    let model, dispatch = () |> Store.makeElmish init update ignore

    let stop() = dispatch StopTimer
    let start() = dispatch StartTimer

    /// Convert the user-supplied command observable into dispatches in our private MVU
    let watch = run
                |> Store.distinct
                |> Store.subscribe (function
                    | true -> start()
                    | false -> stop())

    let cleanup() =
        watch.Dispose()
        model |> Store.get |> stopTimerTask
        model.Dispose()

    /// Here we are just returning the user's view template for the timer, passing the
    /// time as an observable. This component adds no view elements of its own, it
    /// just provides the timer functionality
    /// Since we have resources to clean up, we 'inject' them into the view template
    /// So if the view creates a top-level div, then our cleanup functions will
    /// be called when that div itself is cleaned up

    model
    |> Store.map (fun m ->  isRunning m, m.Elapsed)
    |> view // User's view component
    |> inject [ // Attach our cleanup to the view component
        unsubscribeOnUnmount [ cleanup ] // Clean up the timer subscription
    ]
