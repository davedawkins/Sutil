# Sveltish

An experiment in applying the design principles from [Svelte](svelte.dev) to native Fable. Svelte is impressive in its own right, but I can't help thinking that Fable is a compiler that's already in our toolchain, and is able to do what Svelte does with respect to generating boilerplate. 

It's all very much a work-in-progress. The code is changing very rapidly; I've no idea if it will work, but it's looking promising.

Some aspects that are working or in progress

*DOM builder*
Crude and minimal. It's Feliz-styled, but builds direct into DOM. If this project proceeds it would be good to layer on top of Feliz.

```
    div [
        className "container"
        p [ str "Fable is running" ]
    ]
```

*Stores*

Similar to Svelte stores, using the same API

```
    let count = Sveltish.makeStore 0
    button [
      className "button"
      onClick (fun _ -> count.Value() + 1 |> count.Set)
      count.Value() |> sprintf "You clicked: %i time(s)" |> str
    ]
```

*Bindings*

The intention is to have Fable or a Fable plugin analyze the AST and produce bindings automatically. F# even in my inexperienced hands does an amazing job of reducing boilerplate to a minimum, but it's still boiler plate.

The button example above won't yet update on button clicks. Here's how we make that happen:

```
    let count = Sveltish.makeStore 0
    button [
      className "button"
      onClick (fun _ -> count.Value() + 1 |> count.Set)
      count.Value() |> sprintf "You clicked: %i time(s)" |> str
    ]

    (fun () -> count.Value() |> sprintf "You clicked: %i time(s)" |> str)
       |> bind count
```

It's ugly, but with Fable's help that can be made to look this:

```
    let count = Sveltish.makeStore 0
    button [
      className "button"
      onClick (fun _ -> count + 1 |> count.Set)
      count |> sprintf "You clicked: %i time(s)" |> str
    ]
```

*Styling*

Working like Svelte. Here's how the Svelte `animation` example is coming along with respect to the styling.

```
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
        ``float`` "left"
        width "50%"
        padding "0 1em 0 0"
        boxSizing "border-box"
    ]
    
    // ...
]

let view =
    style styleSheet <| div [
        className "board"
        input [
            className "new-todo"
            placeholder "what needs to be done?"
        ]

        todosList "left" "todo" (fun t -> not t.Done) |> bind todos
        todosList "right" "done" (fun t -> t.Done) |> bind todos
    ]
```


