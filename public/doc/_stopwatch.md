## Abandoned as an example - I want to see if I can fix the framework to make this readable

## Stopwatch Example

```fs
let now() = System.DateTime.Now

let initValue() = (0.0, now())

let stopwatchTimer =
    Store.make (initValue())


let updateTimer() =
    stopwatchTimer
    |> Store.modify (fun (elapsedMs, startTime) ->
        let t = now()
        let sinceStartMs = (t - startTime).TotalMilliseconds
        (elapsedMs + sinceStartMs, t)
    )

let cancelInterval - Store.make (None : (unit -> unit) option)

let stop() =
    cancelInterval
    |> Store.modify (fun c ->
        match c with
        | Some st ->
            st()
            stopTicker <- None
        | _ -> () )

let start() =
    match stopTicker with
    | None ->
        stopwatchTimer |> Store.modify (fun (elapsedMs,_) -> (elapsedMs, now()))
        stopTicker <- Some (interval updateTimer 40)
    | _ -> ()

let reset() =
    stop()
    Store.set stopwatchTimer (initValue())

Html.div [
    Bind.el( stopwatchTimer, fun (elapsed,_) -> sprintf ("%0.3f" ) (elapsed/1000.0) |> Html.div)

    Html.button [
        text "Start"
        Ev.onClick (fun _ -> start())
    ]
    Html.button [
        text "Stop"
        Ev.onClick (fun _ -> stop())
    ]
    Html.button [
        text "Reset"
        Ev.onClick (fun _ -> reset())
    ]
]
```
