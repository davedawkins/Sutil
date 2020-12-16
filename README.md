# Sveltish

An experiment in applying the design principles to native Fable. It's very much a work-in-progress. The code is changing very rapidly. I've no idea if it will work.

Features:

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
Working like Svelt. Example coming



