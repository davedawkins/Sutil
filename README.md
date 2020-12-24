# Sveltish

An experiment in applying the design principles from [Svelte](https://svelte.dev/) to native Fable. At first I thought that we might make use of a Fable compiler plugin to generate boilerplate, but it turns out that F# does a pretty good job of that itself.

See the [Sveltish website](https://davedawkins.github.io/Fable.Sveltish/) for demos.

Here's how the Sveltish Todos app looks. This is an augmented port of the [Svelte animate example](https://svelte.dev/examples#animate)

<img src="images/todosGoodJob.gif" width="400">

Implementing this app is pushing the library. As each new demo example is added, it might require tweaking to existing capabilities, a bug fix, a new feature to be added (or ported over from Svelte), or even a rework of some of the internals. It's a good driver for the library in its own right.

## Changelog (most recent first)

- Fixes to demo app styling and production bundling thanks to a PR from [s0kil](https://github.com/s0kil)
- Demo app support for syntax highlighting example code. See section **Interacting with 3rd-party libraries**
- More examples ported into demo app
- .. more to come .. 


Some aspects that are working or in progress:

## DOM builder
Crude and minimal. It's Feliz-styled, but builds direct into DOM.

```fsharp
    div [
        class' "container"
        p [ text "Fable is running" ]
    ]
```

## Stores

Similar to Svelte stores, using the same API

```fsharp
    let count = Sveltish.makeStore 0
    button [
      class' "button"
      onClick (fun _ -> count.Value() + 1 |> count.Set)
      text "Click Me"
    ]
```

## Bindings

We can react to count being updated:

```fsharp
    let count = Sveltish.makeStore 0
    button [
      class' "button"
      onClick (fun _ -> count.Value() + 1 |> count.Set)
      (fun n -> sprintf "You clicked: %i time(s)" n |> text)
          |> bind count
    ]

```

## Operators

Instead of using the `bind` function, we can use the `|=>` operator.

```fsharp
    let count = Sveltish.makeStore 0
    button [
      class' "button"
      onClick (fun _ -> count.Value() + 1 |> count.Set)
      count |=> (fun n -> sprintf "You clicked: %i time(s)" n |> text)
    ]
```

Instead of calling `count.Set` we can use the `<~` operator:

```fsharp
    let count = Sveltish.makeStore 0
    button [
      class' "button"
      onClick (fun _ -> count <~ count.Value() + 1)
      count |=> (fun n -> sprintf "You clicked: %i time(s)" n |> text)
    ]
```

Finally for this example, instead of accessing count.Value(), we can use the `|->` operator

```fsharp
    let count = Sveltish.makeStore 0
    button [
      class' "button"
      onClick (fun _ -> count <~ count |-> (fun n -> n + 1))
      count |=> (fun n -> sprintf "You clicked: %i time(s)" n |> text)
    ]
```

The operators may change in their spelling as the project develops. Here are the current set of operators

```fsharp
    let (|%>) s f = storeMap f s     // Store<a> -> (a -> b) -> Store<b>
    let (>%>) s f = storeBind f s    // Store<a> -> (a -> Store<b>) -> Store<b>
    let (|->) s f = storeGetMap f s  // Store<a> -> (a -> b) -> b

    // Update a store with a new value
    let (<~-) (s : Store<'T>) (v : 'T) = s.Set(v)
    let (<~)  (s : Store<'T>) (v : 'T) = s.Set(v)
    let (-~>) (v : 'T) (s : Store<'T>) = s.Set(v)

    // Update a store as a function of the current value
    let (<~=) (s : Store<'T>) (f : 'T -> 'T) = s <~ s |-> f
    let (=~>) (f : 'T -> 'T) (s : Store<'T>) = s <~ s |-> f

    let (|=>) a b = bind a b //
```

I don't see a use for storeBind (m-----ic bind) yet, but was interested to see how it would be implemented.

## Styling

Working like Svelte. Here's how the Svelte `animation` example is coming along with respect to the styling.

<img alt="Todos Progress" width="400" src="images/todos.png">

```fsharp
let styleSheet = [
    rule ".new-todo" [
        fontSize "1.4em"
        width "100%"
        margin "2em 0 1em 0"
    ]

    rule ".board" [
        maxWidth "36em"
        margin "0 auto"
    ]

    rule ".left, .right" [
        float' "left"
        width "50%"
        padding "0 1em 0 0"
        boxSizing "border-box"
    ]

    // ...
]

let view =
    style styleSheet <| div [
        class' "board"
        input [
            class' "new-todo"
            placeholder "what needs to be done?"
        ]

        todosList "left" "todo" (fun t -> not t.Done) |> bind todos
        todosList "right" "done" (fun t -> t.Done) |> bind todos
    ]
```

## Transitions

Transitions can be specified with the `transition` binding function. This is an extended form of the `show` binding which
just shows or hides an element according to a given `Store<bool>`.

TODO All examples from here on need updating

<img alt="Transitions Progress" width="400" src="images/transition.gif">

Here's the code for this component:

```fsharp

let Counter attrs =
    let count = Sveltish.makeStore 0
    div [
        button [
            class' "button"
            onClick (fun _ ->
                console.log("click")
                count.Value() + 1 |> count.Set)

            // Boiler plate to be generated by Fable plugin
            (fun () ->
                text <| if count.Value() = 0 then "Click Me" else count.Value() |> sprintf "You clicked: %i time(s)"
            ) |> bind count
        ]

        button [
            class' "button"
            Attribute ("style", "margin-left: 12px;" )
            onClick (fun _ -> 0 |> count.Set)
            text "Reset"
        ]

        // More boilerplate that can be generated automatically
        (div [ text "Click button to start counting" ])
        |> transition
                (InOut (Transition.slide, Transition.fade))
                (count |~> exprStore (fun () -> count.Value() = 0))  // Visible if 'count = 0'

    ]
```
The `transition` wrapper manages visibility of the contained element, according to the expression. It then uses
the specified transitions to handle entry and exit of the element from the DOM.

We now have `fade`, `fly` and `slide` transitions

<img src="images/fly.gif" width="400">

```fsharp
(Html.div [ className "hint"; text "Click button to start counting" ])
|> Bindings.transition
        (Both (Transition.fly,[ X 100.0; Y 100.0; ]))
        (count |~> exprStore (fun () -> count.Value() = 0 && props.ShowHint))  // Visible if 'count = 0'
```

We also have an `each` control that manages lists. Items that appear in, disappear from and move around in the list
can be transitioned:

<img src="images/transfade.gif" width="400">

```fsharp
let todosList cls title filter =
    Html.div [
        className cls
        Html.h2 [ text title ]

        Bindings.each todos (fun (x:Todo) -> x.Id) filter (Both (Transition.fade [])) (fun todo ->
            Html.label [
                Html.input [
                    attr ("type","checkbox")
                    Bindings.bindAttr "checked"
                        ((makePropertyStore todo "Done") <~| todos)
                ]
                text " "
                text todo.Description
                Html.button [
                    on "click" (fun _ -> remove(todo))
                    text "x"
                ]
            ]
        )

    ]
```

I'm looking forward to seeing how much boilerplate we can remove with a compiler plugin.

Crossfade is now working. This animation is deliberately set to run slowly so that I could
check the behaviour. The final part of this example is the `animate:flip` directive.

<img src="images/crossfade.gif" width="400">

Crossfade with animate:flip is now working

## Model-View-Update support

Experimenting with a slightly modified form, where the model mutates. There's still value in organizing
the program into view and update functions. The view function exists naturally in Sveltish, and
the dispatch->update separation means that all updates to the model can be made in the update function,
with the view only issuing dispatched messages.

The view has bindings to derivations of the model's store and so updates in response to changes made
by the update function.

Of course, you can mutate the model directly in the event handlers, but if you like the organization
that Elmish/MVU brings, this is still an option.

Main app that sets up the application main view including the Todos comnponent

```fsharp
let init() = Todos.init()
let update = Todos.update

let app model dispatch =
    Styling.style bulmaStyleSheet <| Html.div [
        class' "container"
        Html.h1 [ text "Sveltish Todos" ]
        Html.div [ Todos.view model dispatch ]
    ]

Sveltish.Program.makeProgram "sveltish-app" init update app
```

The Todos component

```fsharp
type Todo = {
        Id : int
        mutable Done: bool
        Description: string
    }

type Model = {
    Todos : Store<List<Todo>>
}

type Message =
    |AddTodo of desc:string
    |ToggleTodo of id:int
    |DeleteTodo of id:int
    |CompleteAll

let styleSheet = [
    rule ".new-todo" [
        fontSize "1.4em"
        width "100%"
        margin "2em 0 1em 0"
    ]
    //...
]

let init() = { Todos = makeExampleTodos() }

//
// All model mutation happens here
//
let update (message : Message) (model : Model) : unit =

    match message with
    | AddTodo desc ->
        let todo = {
            Id = newUid() + 10
            Done = false
            Description = desc
        }
        model.Todos <~ (model.Todos |-> (fun x -> x @ [ todo ])) // Mutation of model
    | ToggleTodo id ->
        match (storeFetchByKey todoKey id model.Todos) with
        |None -> ()
        |Some todo ->
            todo.Done <- not todo.Done
            forceNotify model.Todos // People will forget to do this
    | DeleteTodo id ->
        model.Todos <~ (model.Todos |-> List.filter (fun t -> t.Id <> id) )
    | CompleteAll ->
        model.Todos <~ (model.Todos |-> List.map (fun t -> { t with Done = true }) )

let fader  x = transition <| Both (Transition.fade,[ Duration 200.0 ]) <| x
let slider x = transition <| Both (Transition.slide,[ Duration 200.0 ])  <| x


let todosList title filter tin tout model dispatch =
    Html.div [
        class' title
        Html.h2 [ text title ]

        each model.Todos todoKey filter (InOut (tin,tout) ) (fun todo ->
            Html.label [
                Html.input [
                    attr ("type","checkbox")
                    on "change" (fun e -> todo.Id |> ToggleTodo |> dispatch)
                    bindAttrIn "checked" (model.Todos |~> (makePropertyStore todo "Done"))
                ]
                text " "
                text todo.Description
                Html.button [
                    on "click" (fun _ -> todo.Id |> DeleteTodo |> dispatch)
                    text "x"
                ]
            ]
        )
    ]

let view (model : Model) dispatch : NodeFactory =
    let (send,recv) = Transition.crossfade [ ]
    let tsend = send, []
    let trecv = recv, []

    let completed = model.Todos |%> List.filter isDone
    let lotsDone  = completed |%> fun x -> (x |> List.length >= 3)

    style styleSheet <| Html.div [
        class' "board"

        Html.h1 [ text "Sveltish Todos" ]

        Html.input [
            class' "new-todo"
            placeholder "what needs to be done?"
            onKeyDown (fun e ->
                if e.key = "Enter" then (e.currentTarget :?> HTMLInputElement).value |> AddTodo |> dispatch
            )
        ]

        Html.div [
            class' "complete-all-container"
            Html.a [
                href "#"
                text "complete all"
                on "click" (fun e -> e.preventDefault();dispatch CompleteAll)
            ]
        ]

        Html.div [
            class' "welldone"
            bind completed (fun x -> text <| sprintf "%d tasks completed! Good job!" x.Length)
        ] |> fader lotsDone

        Html.div [
            class' "row"
            todosList "todo" isPending trecv tsend model dispatch
            todosList "done" isDone trecv tsend model dispatch
        ]
    ]

```

## Interaction with 3rd-party libraries

The app now has syntax highlighting, in the form of highlight.js (a dependency of the app, and not the library). This required a few enhancements:
- custom events from Sveltish that can be used to tell highlight.js to reformat 
- resilience to DOM nodes disappearing that were being watched by Sveltish (this will be an ongoing challenge). 
- relaxation of the idea that all edits to index.html happen programmatically - you can add stylesheet and script tags to the head programmatically 
but things like highlight.js needed a short JS fragment. (However, writing this just now, I think I see how I can actually just write that in Fable
with a minor interop edit. Fable is fantastic with JS interop.)

The problem I had was because I was Sveltish-binding to a `text` node, which would be replaced by highlight.js. Sveltish handles this, but can't be
100% sure on the recovery strategy. It's a good test case so I've left it this way in the way, but a quicker fix by far would have been to bind to the
outer `<code>` or `<pre>` elements.

