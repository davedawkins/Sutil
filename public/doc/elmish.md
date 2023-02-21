Sutil can be made reactive to application state entirely with stores and bindings, without using Elmish at all.

With that said, Elmish will nearly always bring benefits to your Sutil project.

- All updates to the `model` state are made in a single `update` function
- All updates are enumerated in a `message` type
- Model state is rendered from a single root `view` function

Here is a simple example:

```
open Sutil

type Model = { Counter : int }

type Message = Increment | Decrement

let init (v : int) = { Counter = v }

let update (msg : Message) (model : Model) =
    match msg with
    | Increment -> { model with Counter = model.Counter + 1 }
    | Decrement -> { model with Counter = model.Counter - 1 }

let view() =
    let model, dispatch = 0 |> Store.makeElmishSimple init update ignore

    Html.div [
        Bind.el( model, fun m -> Html.div $"Counter = {m.Counter}")

        Html.button [
            text "-"
            onClick (fun e -> dispatch Decrement) []
        ]

        Html.button [
            text "+"
            onClick (fun e -> dispatch Increment) []
        ]

    ]

view() |> Program.mount
```

Note that the type of model as returned by `Store.makeElmish` is `IStore<Model>`. Sutil updates this store upon each call to dispatch. Here is pseudo-code for the dispatch function:

```
    let dispatch msg =
        model |> Store.get |> update msg |> Store.set model
```

Since `Bind.el` listens to changes in the model, we can see how the cycle completes:

```
    dispatch => update model => trigger bindings => update view
```

In a more complex example, you will more likely make the binding using `Store.map`:

```
let counter model = model.Counter

Bind.el( model |> Store.map counter, fun n -> Html.div $"Counter = {n}")
```

Since the `update` function creates a new `Model` instance upon each call to dispatch, we may wish to avoid redraws for model fields that haven't changed. For example, our example model could include an extra field:

```
type Model = {
    Counter : int
    Tick : int }
```

where `Tick` is updated by a timer event every second. In this case, we may wish to avoid redrawing the counter's current value upon each tick. We can use `distinctUntilChanged` for this:

```
Bind.el(
    model |> Store.map counter |> Observable.distinctUntilChanged,
    fun n -> Html.div $"Counter = {n}" )
```

Since this is a common idiom, we can abbreviate this to:

```
Bind.el(
    model |> Store.mapDistinct counter,
    fun n -> Html.div $"Counter = {n}" )
```

A full-Elmish example includes support for commands. Commands are an additional return value from the Elmish `init()` and `update()` functions, and are executed within the Elmish processing loop.

Our example with commands would look like the example below.

```
open Sutil

type Model = { Counter : int }

type Message = Increment | Decrement

let init (v : int) = { Counter = v }, Cmd.none

let update (msg : Message) (model : Model) =
    match msg with
    | Increment -> { model with Counter = model.Counter + 1 }, Cmd.none
    | Decrement -> { model with Counter = model.Counter - 1 }, Cmd.none

let view() =
    let model, dispatch = 0 |> Store.makeElmish init update ignore

    Html.div [
        Bind.el( model, fun m -> Html.div $"Counter = {m.Counter}")

        Html.button [
            text "-"
            onClick (fun e -> dispatch Decrement) []
        ]

        Html.button [
            text "+"
            onClick (fun e -> dispatch Increment) []
        ]

    ]

view() |> Program.mount
```

Note the differences:

- `init` and `update` return a tuple of `(Model * Cmd<Message>)`

- initialization is performed with `Store.makeElmish` (not `Store.makeElmishSimple`)

The `init()` and `update()` functions are not just returning the new model state now, they also return a command.
However, in this example the command is always `Cmd.none`, which means "do nothing".

In the section on `Timers` we introduced a stopwatch example. Here is that same example as an Elmish program

```fs

type Stopwatch = {
    Elapsed : float
    StartedAt : System.DateTime
    IsRunning : bool
}

type Message =
    | Start
    | Stop
    | Reset
    | Tick

let now() = System.DateTime.Now

let init() =
    {
        Elapsed = 0.0; StartedAt = now(); IsRunning = false
    }, Cmd.none

let nextTick dispatch =
    rafu (fun _ -> dispatch Tick)

let update (msg : Message) (model : Stopwatch) =
    match msg with

    | Start ->
        { model with StartedAt = now(); IsRunning = true }, Cmd.ofEffect (nextTick)

    | Stop ->
        { model with IsRunning = false }, Cmd.none

    | Reset ->
        init()

    | Tick ->
        if model.IsRunning then
            let t = now()
            let sinceStartMs = (t - model.StartedAt).TotalMilliseconds
            { model with Elapsed = model.Elapsed + sinceStartMs; StartedAt = t },
                Cmd.ofEffect (nextTick)
        else
            model, Cmd.none

// stopwatch is an IStore<Stopwatch> and holds all of our application state
let stopwatch, dispatch = () |> Store.makeElmish init update ignore

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
                Ev.onClick (fun _ -> dispatch Stop)
            ]
        else
            Html.button [
                Attr.className "button" // Bulma styling
                Attr.style [ Css.marginTop (rem 1) ]
                text "Start"
                Ev.onClick (fun _ -> dispatch Start)
            ]
    )

    // Note that this button is only ever rendered once, as is the enclosing div
    Html.button [
        text "Reset"
        Ev.onClick (fun _ -> dispatch Reset)
        Attr.className "button"
        Attr.style [ Css.marginLeft (rem 1); Css.marginTop (rem 1) ]
    ]
]
```
