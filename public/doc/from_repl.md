Sutil has its own [online REPL](https://sutil.dev/repl), where you can create small applications and try out the various features.

Throughout this documentation, you will see various code examples. If the example is self-contained, it will have a link below that reads "Open in REPL". Click on this link to send the example straight to the REPL, where it will be compiled and executed.

Try it now!

```fsharp
Html.div "Edit me in the REPL"
```

You'll notice that `open` and `mountElement` statements are added if necessary.

## Samples

The REPL contains a pre-defined set of samples that you can load, run and modify. These are the same examples that appear in the `Examples` section of [sutil.dev](https://sutil.dev)

## Elmish Counter

Here's a counter example, using ELmish

```fsharp
module App

open Sutil
open Sutil.DOM
open Sutil.Attr

type Model = { Counter : int }

// Model helpers
let getCounter m = m.Counter

type Message =
    | Increment
    | Decrement

let init () : Model = { Counter = 0 }

let update (msg : Message) (model : Model) : Model =
    match msg with
    |Increment -> { model with Counter = model.Counter + 1 }
    |Decrement -> { model with Counter = model.Counter - 1 }

// In Sutil, the view() function is called *once*
let view() =

    // model is an IStore<ModeL>
    // This means we can write to it if we want, but when we're adopting
    // Elmish, we treat it like an IObservable<Model>
    let model, dispatch = () |> Store.makeElmishSimple init update ignore

    Html.div [
        disposeOnUnmount [ model ]

        // See Sutil.Styling for more advanced styling options
        style [
            Css.fontFamily "Arial, Helvetica, sans-serif"
            Css.margin 20
        ]

        // Think of this line as
        // text $"Counter = {model.counter}"
        Bind.el (model |> Store.map getCounter, fun n ->
            text $"Counter = {n}" )

        Html.div [
            Html.button [
                class' "button" // Bulma styling, included in index.html

                onClick (fun _ -> dispatch Decrement) []
                text "-"
            ]

            Html.button [
                class' "button"
                onClick (fun _ -> dispatch Increment) []
                text "+"
            ]
        ]]

// Start the app
view() |> Program.mountElement "sutil-app"
```
