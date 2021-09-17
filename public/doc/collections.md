
[Syntax]: http://
[examples]: https://sutil.dev/#examples-each-blocks?EachBlocks.fs
[Stores]: https://
[each keyed]: https://sutil.dev/#examples-keyed-each-blocks?KeyedEachBlocks.fs

## Static Collections

Rendering collections such as lists, arrays, sequences is a common task in any web application.

For a static collection, we can use a `for` loop:

```fsharp
Html.ul [
    for number in [1..9] do
        Html.li [ text $"{number}" ]
]
```

Another example for static collections using `List.mapi`:

```fsharp
let contributors = [ "Alfonso Garcia"; "Angel Munoz"; "Daniel Sokil" ]
Html.ul
    (contributors
        |> List.mapi (fun i v ->
            sprintf "%i : %s" i v |> Html.div))
 |> Program.mountElement "sutil-app"
 ```

## Dynamic Collections

For collections of values that are going to change, use `Bind.each` which will use the given template function for each element in the collection

```fsharp
let stocks = SampleData.stockFeed 10 500

Html.table [
    class' "table"
    Html.tbody [
        Bind.each(stocks,fun r ->
            Html.tr [
                Html.td r.Symbol
                Html.td (sprintf "%-5.2f" r.Price)
            ]
        )
    ]
    disposeOnUnmount [stocks]
]
```

Use `Bind.eachi` if you need the element index:

```fsharp
let todos = SampleData.todosFeed 5 3000

let viewTodo (i,todo:SampleData.Todo) =
    Html.tr [
        Html.td (i+1)
        Html.td todo.Description
        Html.td [
            Html.input [
                Attr.typeCheckbox
                if todo.Completed then
                    Attr.isChecked true ]
        ]
    ]

Html.table [
    class' "table"
    Bind.eachi(todos, viewTodo)
]
```

### eachio

`eachio` takes three parameters

- An observable of a list of items
- A templating function

    The difference betwen `eachio` and the previous is that the templating function has a tuple of two observables, the first element is the index and the second is the observable of the item

- A list of transitions

```fsharp

let renderIndex (index: IObservable<int>) =
    Html.span [
        // bind an observable to a span
        bindFragment index <| fun index -> text $"{index}"
    ]

let renderItem<'T> (item: IObservable<'T>) =
    Html.b [
        // bind an observable to a <b></b>
        bindFragment item <| fun item -> text $"{item}"
    ]

Html.div [
  let items = Store.make ["one string"; "two strings" ]
  Html.ul [
    disposeOnUnmount [ items ]
    eachio
      items
      (fun index item -> Html.li [ remderindex index; renderItem item ])
      []
  ]
]
```
Internally the key for each of these objects will be the index of the element.

This might not look that useful but once you get to know stores better you'll see that passing observables around sometimes is more convenient for complex objects.


### eachk

`eachk` takes four parameters

- An observable of a list of items
- A templating function

    That takes an item and returns a `SutilElement`

- A function to get a key from your object
- A list of transitions

```fsharp

Html.div [
  let items =
    Store.make
        [ {| name = "Peter"; lastName = "Parker"|}
          {| name = "Bruce"; lastName = "Wayne"|} ]
  Html.ul [
    disposeOnUnmount [ items ]
    eachk
      items
      (fun index item -> Html.li [ text $"{index} - {item}" ])
      (fun item -> item.lastName)
      []
  ]
]
```

### eachiko

`eachiko` takes four parameters

- An observable of a list of items
- A templating function

    That takes an observable index, and an observable item just as in `eachio` and returns a `SutilElement`

- A function to get a key from your object
- A list of transitions

```fsharp

let renderIndex (index: IObservable<int>) =
    Html.span [
        // bind an observable to a span
        bindFragment index <| fun index -> text $"{index}"
    ]

let renderItem (item: IObservable<{| name: string; lastName : string |}>) =
    Html.b [
        // bind an observable to a <b></b>
        bindFragment item <| fun item -> text $"{item.lastName} {item.name}"
    ]

Html.div [
  let items =
    Store.make
        [ {| name = "Peter"; lastName = "Parker"|}
          {| name = "Bruce"; lastName = "Wayne"|} ]
  Html.ul [
    disposeOnUnmount [ items ]
    eachiko
      items
      (fun index item -> Html.li [ remderindex index; renderItem item ])
      (fun inex item -> $"{item.lastName}-{index}")
      []
  ]
]
```

`eachiko` is used internally by the other functions, in this case `eachiko` is the function that will give you the most control over how you want to iterate over collections.
