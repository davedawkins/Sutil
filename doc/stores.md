Sutil's reactivity model is based on observables. You declare bindings between sections of your view and observable values, and Sutil rebuilds that view section *only*  when those values change.

This contrasts with React, where the *whole* view is rebuilt when *any* values change. React uses a virtual DOM and some clever diffing algorithms to make this as efficient as possible.

Observables are read-only. Sutil's `Store` makes an observable to which you can write values.

A Store is defined as follows
```fs
type IStore<'T> =
    interface
        inherit IObservable<'T>
        inherit IDisposable
        abstract Update : f: ('T -> 'T) -> unit
        abstract Value : 'T
    end
```

Stores are `IObservable` and `IDisposable`.

As an `IObservable`, they are compatible with other libraries such as [FSharp.Core](https://fsharp.github.io/fsharp-core-docs/).

As an `IDisposable`, they must be disposed of when no longer used. Sutil provides constructs to help with this.

## Functions

### Store.make : 'T -> IStore<'T>
Create a new store, with the given initial value.

#### Examples
```fsharp
let intStore: IStore<int> = Store.make 1

let anonymousStore: IStore<{| prop1: number; prop2: option string |}> =
    Store.make {| prop1 = 10; prop2 = None |}

(* After using the store *)

intStore.Dispose()
anonymousStore.Dispose()
```

### Store.get : () -> 'T
Retrieve the current value of the store

#### Examples
```fs
let value = Store.get initStore
value = 1 // true

let value2 = Store.get anonymousStore
Option.isNone value2.prop2 // true
```

### Store.set: 'T -> unit
Set the current value of the store. Subscriptions will be notified, even if the new value is the same as the current value.

#### Examples
```fs
Store.set intStore 2
let value = Store.get intStore
value = 1 // false
```
### Store.subscribe: ('T -> unit) -> IObservable<'T> -> IDisposable
Create a subscription that invokes a callback every time the store value is updated.

Note that subscriptions are notified even if the new value for the store is the same as its current value.

Cancel the subscription by disposing the returned object. Upon subscription, the callback will be called immediately with the store's current value.

#### Examples
```fs
let subscription =
    Store.subscribe (fun value -> printfn $"{value}") intStore

(* after you are done with the subscription *)

subscription.Dispose()
```
### Store.map: ('T -> 'R) -> IObservable<'T> -> IObservable<'R>
Project an observable using a mapping function.

#### Examples

```fs
let store = Store.make [1;2;3;4]
let squares = Store.map (fun n -> n * n) store

Html.div [
    disposeOnUnmount [ store; squares ]

    Html.ul [
        Html.li [ Html.h3 [text "numbers"] ]
        eachi store <| fun (i, num) -> Html.li [ text $"{num}" ]
    ]
    Html.ul [
        Html.li [ Html.h3 [text "Squares"] ]
        eachi squares <| fun (i, num) -> Html.li [ text $"{num}" ]
    ]
]
```

### Store.filter
`Store.filter` applies a predicate function to obtain an observable of the elements that evaluated to true, this can be useful to hide/show information to your users.


#### Examples
```fs
let store: IStore<User array> =
    Store.make []
let filteredUsers =
    Store.filter(fun user -> user.age > 18) store

Html.div [
    disposeOnUnmount [ store; filteredUsers ]

    Html.ul [
        Html.li [ Html.h3 [text "Users"] ]
        eachi store <| fun (i, user) -> Html.li [ text $"{user.name}" ]
    ]
    Html.ul [
        Html.li [ Html.h3 [text "Users Over 18"] ]
        eachi filteredUsers <| fun (i, user) -> Html.li [ text $"{user.name}" ]
    ]
]
```

### Store.distinct
`Store.distinct` provides an observable that will emit a value only when the updated store value is different from the previous one, use this to prevent repeated updates/expensive computations when the store updates with the same value


#### Examples
```fs
let store = Store.make 0
let whenDistinct = Store.distinct store


Html.div [
    disposeOnUnmount [ store; whenDistinct ]

    Html.button [ onClick(fun _ -> Store.set store 1) []; text "Set store to '1'" ]
    Html.button [ onClick(fun _ -> Store.set store 0) []; text "Set store to '0'" ]

    bindFragment store
        <| fun value ->
            Html.p [ text "The value is: {value}" ]
    bindFragment whenDistinct
        <| fun value ->
            Html.p [ text "The value is: {value}" ]
]
```

### Store.zip
`Store.zip` merges two stores into a single tupled observable
```fs
let tableInfo =
    Observable.zip
        (Strore.map(fun store -> store.rows) store)
        (Strore.map(fun store -> store.columns) store)
HTML.article [
    disposeOnUnmount [ tableInfo ]
    bindFragment tableInfo <| fun (rows, columns) -> Html.table [
        Html.thead [
            eachi columns <| fun (i, col) -> Html.th [text col.label ]
        ]
        Html.tbody [
            eachi rows <| fun (i, row) ->
                Html.tr [
                    eachi row <| Html.td [ row.name ]
                    eachi row <| Html.td [ row.age ]
                ]
        ]
    ]
]
```

### Store.getMap
`Store.getMap` takes an observable and applies a mapping function then returns the value from the evaluated function, this can be useful to perform a single transformation over store values

#### Examples
```fs
let store: IStore<{| name: string; budget: decimal |}> =
    Store.make {| name = "Frank"; budget = 547863.26M |}

let formattedBudget: string =
    Store.getMap (fun store -> sprintf $"$ %0.00M{store.budget}" )
printf %"Budget available: {formattedBudget}
```
### Store.write
`Store.write` calls the callback upon initialization and whenever the store is updated. This is the same as subscribe and ignoring the unsubscription callback

#### Examples
```fs
Store.write (fun value -> printfn $"{value}") intStore
```

### Store.modify
Modify the store by mapping its current value with a callback

#### Example
```fs
let store: IStore<int> = Store.make 2

let squareMe() =
    Store.modify (fun model -> model * model) store

Html.div [
    disposeOnUnmount [ store ]
    bindFragment store <| fun model -> text $"The value is {model}"
    Html.button [
        onClick (fun _ -> squareMe()) []
        text "Square me"
    ]
]
```

### Store.subscribe2
Takes two observables and subscribes to both with a single callback, both values will be cached individually and on every notify they will be updated and emitted, every notification can come from any of the observables

#### Example
```fs
let player1Score = Store.make 0
let player2Score = Store.make 0

let printPlayerScores (score1: int * score2: int) =
    printfn $"Player 1: {score1}\nPlayer2: {score2}"

let scores =
    Store.subscribe2
        player1Score
        player2Score
        printPlayerScore
(* Game Finished, dispose the observables *)
scores.Dispose()
```

### Store.makeElmishSimple
`Store.makeElmishSimple` will create a store and a dispatch method commonly used in elmish programs, this can be used to model more complex views that require better control flow and a predictable state

#### Example

```fs
type State = { count: int }
type Msg =
    | Increment
    | Decrement
    | Reset
let init _ = { count = 0 }

let upddate msg state =
    match msg with
    | Increment -> { state = state.count + 1 }
    | Decrement -> { state = state.count - 1 }
    | Reset -> { state = 0 }

let view() =
    let state, dispatch = Store.makeElmishSimple init update ignore ()

    Html.article [
        disposeOnUnmount [ state ]
        bindFragment state <| fun state -> text $"Count: {state.count}"

        Html.button [ text "Increment"; onClick (fun _ -> dispatch Increment) [] ]
        Html.button [ text "Decrement"; onClick (fun _ -> dispatch Decrement) [] ]
        Html.button [ text "Reset"; onClick (fun _ -> dispatch Reset) [] ]
    ]
```

### Store.makeElmish
`Store.makeElmish` will create a store and a dispatch function as `Store.makeElmishSimple` the difference being that this version handles [Elmish commands](https://elmish.github.io/elmish/index.html#Commands) as well, generally used in more complex UIs given that with commands you can also handle asynchronous code like fetching resources from a server or calling any function that returns a promise or async

#### Example

```fs
type State = { count: int }
type Msg =
    | Increment
    | Decrement
    | Reset
    | AsyncIncrement
    | AsyncDecrement
let init _ = { count = 0 }, Cmd.ofMsg AsyncIncrement

let wait1S () =
    async {
        do! Async.Sleep 1000
    }

let upddate msg state =
    match msg with
    | Increment -> { state = state.count + 1 }, Cmd.none
    | Decrement -> { state = state.count - 1 }, Cmd.none
    | AsyncIncrement ->
        state, Cmd.ofAsync.perform () wait1S Increment
    | AsyncDecrement->
        state, Cmd.ofAsync.perform () wait1S Decrement
    | Reset -> { state = 0 } Cmd.none

let view() =
    let state, dispatch = Store.makeElmish init update ignore ()

    Html.article [
        disposeOnUnmount [ state ]
        bindFragment state <| fun state -> text $"Count: {state.count}"

        Html.button [ text "Increment"; onClick (fun _ -> dispatch Increment) [] ]
        Html.button [ text "Async Increment"; onClick (fun _ -> dispatch AsyncIncrement) [] ]
        Html.button [ text "Decrement"; onClick (fun _ -> dispatch Decrement) [] ]
        Html.button [ text "Async Decrement"; onClick (fun _ -> dispatch AsyncDecrement) [] ]
        Html.button [ text "Reset"; onClick (fun _ -> dispatch Reset) [] ]
    ]
```



## Operators
Sometimes writing `Store.xxxxxxx` can be annoying and can make the code more verbose and tedious to read this is where operators come handy, they shorten the code and make it easier to read once you're familiar with them.

> **Note**: They can also confuse people that is not familiar with the codebase so remember that you can always switch to the original functions or leave coments in your code

### |->
Alias for `Store.getMap`, takes a store and applies a mapping function then returns the value from the evaluated function

> This might be called foldMap

#### Example
```fs
    let store: IStore<{| name: string; budget: decimal |}> =
        Store.make {| name = "Frank"; budget = 547863.26M

    let formattedBudget: string =
        store |-> (fun model -> sprintf $"$ %0.00M{model.budget}")
    printf %"Budget available: {formattedBudget}"
```
### .>
Alias for `Store.map`, returns an observable that will resolve to the result of said callback
#### Example
```fs
let subscription: IObservable<string> =
    intStore .> (fun value -> $"{value}")
(* after you are done with the subscription *)
subscription.Dispose()
```

### <~
Alias for `Store.set`,  replaces the current value of the store
#### Example
```fs
intStore <~ 2
let value = Store.get intStore
value = 1 // false
```

### -~>
Alias for `Store.set`,  replaces the current value of the store
#### Example
```fs
2 -~> intStore
let value = Store.get intStore
value = 1 // false
```

### <~=
Alias for `Store.modify`. Modify the store by mapping its current value with a callback
#### Example
```fs
let store: IStore<int> = Store.make 2
let squareMe() =
    store <~= (fun model -> model * model)
Html.div [
    bindFragment store <| fun model -> text $"The value is {model}"
    Html.button [
        onClick (fun _ -> squareMe()) []
        text "Square me"
    ]
]
```

### =~>
Alias for `Store.modify`. Modify the store by mapping its current value with a callback
#### Example
```fs
let store: IStore<int> = Store.make 2
let squareMe() =
    (fun model -> model * model) =~> store
Html.div [
    bindFragment store <| fun model -> text $"The value is {model}"
    Html.button [
        onClick (fun _ -> squareMe()) []
        text "Square me"
    ]
]
```
