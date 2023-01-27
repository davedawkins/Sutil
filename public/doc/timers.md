Sutil provides helpers for several Browser timer functions

| Function | JS Web API |
|----------|-------------|
| `DomHelpers.raf (f : float : unit)` | [window.requestAnimationFrame()](https://developer.mozilla.org/docs/Web/API/window/requestAnimationFrame) |
| `DomHelpers.rafu (f : unit : unit)` | [window.requestAnimationFrame()](https://developer.mozilla.org/docs/Web/API/window/requestAnimationFrame), but discards the timestamp |
| `DomHelpers.interval (f : unit -> unit) (delayMs : int)` | [setInterval()](https://developer.mozilla.org/docs/Web/API/setInterval) |
| `DomHelpers.timeout (f : unit -> unit) (delayMs : int)` | [setTimeout()](https://developer.mozilla.org/docs/Web/API/setTimeout) |

Each of the Sutil wrappers returns an unsubscribe function of type `unit -> unit`.

For example:

```fs
open Sutil.DomHelpers

let cancelTimeout = timeout (fun _ -> printfn "Hello!") 1000

// Some time later - maybe something happened that means we don't want the timer to fire now

cancelTimeout() //norepl
```

You are of course free to use Fable's own wrappers for these functions; Sutil only wraps them with unsubscription functions.


## Simple Clock Example


```fs
let time = Store.make (System.DateTime.Now)
let updateTime() = Store.set time System.DateTime.Now

Html.div [
    unsubscribeOnUnmount [
        interval updateTime 500 //ms
    ]

    Bind.el( time, fun t -> t.ToLongTimeString() |> text)
]
```

## Stopwatch Example

This example aims to show a more complicated usage of `DomHelpers.interval`, where we wish to start and stop a ticker.

I've avoided introducing Elmish for this example, but it is certainly styled in an Elmish way. We'll revisit this example in the section where we cover Sutil's Elmish.


```fs

type Stopwatch = {
    Elapsed : float
    StartedAt : System.DateTime
    IsRunning : bool
}

let now() = System.DateTime.Now

let init() = {
    Elapsed = 0.0; StartedAt = now(); IsRunning = false
}

let createStopwatch() =
    init() |> Store.make

let updateElapsed (stopwatch) =
    stopwatch
    |> Store.modify (fun sw ->
        let t = now()
        let sinceStartMs = (t - sw.StartedAt).TotalMilliseconds
        { sw with Elapsed = sw.Elapsed + sinceStartMs; StartedAt = t }
    )

let rec requestTick stopwatch =
    // State update
    updateElapsed stopwatch

    // Tick management
    if (stopwatch |> Store.getMap (fun sw -> sw.IsRunning)) then
       rafu (fun _ -> requestTick stopwatch) |> ignore

let stop (stopwatch) =
    stopwatch
    |> Store.modify (fun sw ->
        { sw with IsRunning = false }
    )

let start (stopwatch) =
    stopwatch
    |> Store.modify (fun sw ->
        { sw with
            StartedAt = now()
            IsRunning = true }
    )
    requestTick stopwatch

let reset (stopwatch) =
    stop stopwatch
    init() |> Store.set stopwatch

// stopwatch is an IStore<Stopwatch> and holds all of our application state
let stopwatch = init() |> Store.make

Html.div [
    // Update the elapsed time
    Bind.el( stopwatch, fun sw -> sprintf ("Elapsed: %0.3f" ) (sw.Elapsed/1000.0) |> Html.div)

    // Change the buttons depending on whether we're running or not
    // Use mapDistinct to prevent rebuilding the buttons upon each tick (which updates Elapsed)
    Bind.el( stopwatch |> Store.mapDistinct (fun sw -> sw.IsRunning), fun isRunning ->
        if isRunning then
            Html.button [
                Attr.className "button" // Bulma styling
                Attr.style [ Css.marginTop (rem 1) ]
                text "Stop"
                Ev.onClick (fun _ -> stop stopwatch)
            ]
        else
            Html.button [
                Attr.className "button" // Bulma styling
                Attr.style [ Css.marginTop (rem 1) ]
                text "Start"
                Ev.onClick (fun _ -> start stopwatch)
            ]
    )

    // Note that this button is only ever rendered once, as is the enclosing div
    Html.button [
        text "Reset"
        Ev.onClick (fun _ -> reset stopwatch)
        Attr.className "button"
        Attr.style [ Css.marginLeft (rem 1); Css.marginTop (rem 1) ]
    ]
]
```
