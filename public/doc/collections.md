
[Syntax]: http://
[examples]: https://sutil.dev/#examples-each-blocks?EachBlocks.fs
[Stores]: https://
[each keyed]: https://sutil.dev/#examples-keyed-each-blocks?KeyedEachBlocks.fs
### Static Collections

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

### Dynamic Collections

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
### Keyed Collections

For keyed collections, we get persistent element views. Sutil knows if a view has already been created for a given element (record) and won't rebuild it, even if the element moves in the collection.

This example captures the initial value of the stock being displayed. Note how the view template now takes an `Observable`.

```fsharp
open System
open Sutil
open Sutil.DOM
open Sutil.Attr
open SampleData

let stocks = SampleData.stockFeed 10 500

let symbol (s : Stock) = s.Symbol
let floatStr (n : float) = sprintf "%-5.2f" n
let priceStr (s : Stock) = s.Price |> floatStr
let changeStr (init : Stock) (current : Stock)  =
    (current.Price - init.Price) |> floatStr

Html.table [
    class' "table"
    Html.thead [
        Html.tr [
            Html.th "Stock"
            Html.th "Initial"
            Html.th "Current"
            Html.th "Change"
        ]
    ]
    Html.tbody [
        Bind.each(
            stocks,
            fun (s:IObservable<Stock>) ->
                let init = s |> Store.current
                Html.tr [
                    Html.td (init.Symbol)
                    Html.td [
                        class' "has-text-right"
                        text (init |> priceStr)
                    ]
                    Html.td [
                        class' "has-text-right"
                        Html.text (s |> Store.map priceStr)
                    ]
                    Html.td [
                        class' "has-text-right"
                        Html.text (s |> Store.map (changeStr init))
                    ]
            ],
            symbol
        )
    ]
    disposeOnUnmount [stocks]
]
```

