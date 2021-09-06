# WIP: Rendering Lists

[Syntax]: http://
[examples]: https://sutil.dev/#examples-each-blocks?EachBlocks.fs
[Stores]: https://
[each keyed]: https://sutil.dev/#examples-keyed-each-blocks?KeyedEachBlocks.fs

> A little lost? check the [Syntax] first!
>
> Also check the [examples]

Rendering lists (or any enumerable, iterable) is a common task in any web application. Let's see how it can be done.

The most basic form is to do a foreach or any iterating functions that F# provides like List.map or Seq.map

```fsharp
let numbers = [1;2;3;4;5;6;7;8;9]

Html.ul [
    for number in numbers do
        Html.li [ text $"{number}" ]
]
```

This will create a `li` tag for each element in the list although this approach is not recommended because it may cause performance issues, specially if you have a large list of elements or you need to re-arrange the order of these elements.


## each, eachi, eachio, eachk, eachiko

For the purpose of efficient rendering you should `each`, `eachi`, `eachio`, `eachk`, `eachiko` all of these create a tracked elements with make it more performant when dealing with larger and more complex lists specially those that you may need to manipulate often.


> Many things are bound to [Stores] in Sutil so if things get confusing around stores, you should take a look at that page.

All of the above operate on observable instances and you will need to make them in order to make it work propperly.

### each 

`each` takes three parameters

- An observable of a list of items
- A templating function (a function that returns a SutilNode)
- A list of transitions

```fsharp
Html.div [
  let items = Store.make [ for i in 0..10 do i ]
  Html.ul [
      disposeOnUnmount [ items ]
      each
        items 
        (fun item -> Html.li [text $"{item}"])
        []
  ]
]
```

Internally the key for each of these objects will be the result of a `GetHashCode()` method call.


### eachi

`eachi` takes three parameters

- An observable of a list of items
- A templating function

    The difference betwen `each` and `eachi` is that `eachi` gives you the index in the templating function

- A list of transitions

```fsharp
Html.div [
  let items = Store.make [ for i in 10..20 do i ]
  Html.ul [
    disposeOnUnmount [ items ]
    eachi
      items 
      (fun index item -> Html.li [ text $"{index} - {item}" ])
      []
  ]
]
```

Internally the key for each of these objects will be the index of the element.

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