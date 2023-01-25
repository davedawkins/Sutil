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

view() |> Program.mountElement "sutil-app"
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

A full-Elmish example includes support for commands. Our example with commands would look like this:

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

view() |> Program.mountElement "sutil-app"
```

Note the differences:

- `init` and `update` return a tuple of `(Model * Cmd<Message>)`
- initialization is performed with `Store.makeElmish` (not `Store.makeElmishSimple`)

We now have the ability to execute commands synchronously and asynchronously in response to the `init` and `update` functions.
