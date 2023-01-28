Fable already provides an API for JavaScript's `Promise`, and Sutil provides helpers to convert a `Promise` into an `IObservable`.

## Bind.promise with defaults

```fs
//norepl
Bind.promise( p : Promise<T>, view : T -> SutilElement )
```

This example shows how we can bind to a record being fetched from a remote source. Note how the view shows "waiting ..." until the promise is resolved.

```fs
open Fable.Core

let fetchRecordFromDb() =
    promise {
        do! Promise.sleep(2000)
        return "David|Dawkins|UK"
    }


Html.div [
    Bind.promise(
        fetchRecordFromDb(),
        Html.div  // Same as (fun s -> Html.div s)
    )
]
```

The binding will also handle errors, and we can provide a style for the default waiting and errors messages:

```fs
open Fable.Core

let errorColor = "#880000"
let waitingColor = "#000088"

let bannerStyle color = [
    Css.color color
    Css.padding (rem 1)
    Css.border (px 1, borderStyle.solid, color)
    Css.borderRadius(rem 0.5)
    Css.textAlignCenter
    Css.fontSize (percent 125)
]

let style = [
    rule ".promise-error" (bannerStyle errorColor)
    rule ".promise-waiting" (bannerStyle waitingColor)
]

let fetchRecordFromDb() =
    promise {
        do! Promise.sleep(2000)
        failwith "Lost connection to database"
        return "David|Dawkins|UK"
    }

Html.div [
    Bind.promise(
        fetchRecordFromDb(),
        Html.div  // Same as (fun s -> Html.div s)
    )
] |> withStyle style
```

## Bind.promise with full control

```fs
//norepl
Bind.promise( p : Promise<T>, view : T -> SutilElement, waiting : SutilElement, error : Exception -> SutilElement )
```

There is an extended form of `Bind.promise` that allows us to supply the view for each of the Promise states: Waiting, Error and Success


```fs
open Fable.Core

let errorColor = "#880000"
let waitingColor = "#000088"

let bannerStyle color = [
    Css.color color
    Css.padding (rem 1)
    Css.border (px 1, borderStyle.solid, color)
    Css.borderRadius(rem 0.5)
    Css.textAlignCenter
    Css.fontSize (percent 125)
]

let style = [
    rule ".promise-error" (bannerStyle errorColor)
    rule ".promise-waiting" (bannerStyle waitingColor)
]

let fetchRecordFromDb() =
    promise {
        do! Promise.sleep(2000)

        failwith "Lost connection to database"

        return "David|Dawkins|UK"
    }


Html.div [
    Bind.promise(
        fetchRecordFromDb(),
        Html.div,
        Html.divc "promise-waiting" [ text "Retrieving data, please wait" ],
        (fun x -> Html.divc "promise-error" [ text (sprintf "Error: %s" x.Message) ] )
    )
] |> withStyle style
```

## Bind.promises

```fs
//norepl
Bind.promises (items : IObservable<JS.Promise<'T>>, view : 'T  -> SutilElement, waiting: SutilElement, error : Exception -> SutilElement)
```

See [AwaitBlocks](https://sutil.dev/#examples-await-blocks) for an example of how to bind to a store of promises.

For the curious, here is the definition for `Bind.promises`:

```fs
//norepl
static member promise (p : JS.Promise<'T>, view : 'T  -> SutilElement, waiting: SutilElement, error : Exception -> SutilElement)=
    Bind.el( p.ToObservable(), fun state ->
        match state with
        | PromiseState.Waiting -> waiting
        | PromiseState.Error x -> error x
        | PromiseState.Result r ->  view r
    )
```

## Promises with Elmish

The commands defined in [`Cmd.OfPromise`](https://sutil.dev/apidocs/reference/sutil-cmd-ofpromise.html) allow us to work with Promises in an Elmish application. The four functions available are:

| Function | Description |
|---|---|
| [`Cmd.OfPromise.either`](https://sutil.dev/apidocs/reference/sutil-cmd-ofpromise.html#either) | Handle both success and erroneous outcomes |
| [`Cmd.OfPromise.perform`](https://sutil.dev/apidocs/reference/sutil-cmd-ofpromise.html#perform) | Handle only successful outcomes |
| [`Cmd.OfPromise.attempt`](https://sutil.dev/apidocs/reference/sutil-cmd-ofpromise.html#attempt) | Handle only erroneous outcomes |
| [`Cmd.OfPromise.result`](https://sutil.dev/apidocs/reference/sutil-cmd-ofpromise.html#result)  | Dispatch a promise result |


This is an Elmish example simulates an unreliable server connection. Attempts to fetch records with the `Fetch` button will occasionally fail.

Note how the handler for `FetchRecord` in the `update` function uses `Cmd.OfPromise.either` to dispatch the call to the server. The outcome is
dispatched back into the Elmish processing loop as either a `SetRecord` message, or a `SetError` message.

```fs
open Fable.Core
open System
open Sutil.Transition

module Server =
    let rnd = System.Random()

    let fetchRecordFromDb() =
        promise {
            do! Promise.sleep(2000)
            if rnd.Next(0,2) = 0 then
                failwith "Server timed out - please try again"
            return "David|Dawkins|UK"
        }

type Model = {
    Record : string option
    Error : string option
    FetchInProgress : bool
}

[<AutoOpen>]
module ModelHelpers =
    let record m = m.Record
    let error m = m.Error
    let fetchInProgress m = m.FetchInProgress
    let errorExists m = m.Error.IsSome
    let errorMessage m = m.Error |> Option.defaultValue ""

let init current = { Record = current; Error = None; FetchInProgress = false }, Cmd.none

type Message =
    | SetError of Exception
    | SetRecord of string
    | ClearError
    | FetchRecord

let update msg model =
    match msg with
    | SetError x ->
        { model with
            Error = Some x.Message; FetchInProgress = false; Record = None }, Cmd.none
    | ClearError ->
        { model with
            Error = None }, Cmd.none
    | SetRecord r ->
        { model with
            Record = Some r; FetchInProgress = false }, Cmd.none
    | FetchRecord ->
        { model with
            FetchInProgress = true },
        Cmd.OfPromise.either (Server.fetchRecordFromDb) () SetRecord SetError

let model, dispatch = None |> Store.makeElmish init update ignore

Html.div [

    Bind.el (model, fun m ->
        Html.divc "block" [
            match m.FetchInProgress, m.Record with
            | true,  _ -> text "<fetching record>"
            | false, None -> text "<no record>"
            | false, Some data -> text data
        ]
    )

    Html.divc "buttons" [
        Html.buttonc "button" [
            Bind.attr(  "disabled", model .> fetchInProgress )
            text "Fetch"
            Ev.onClick (fun _ -> dispatch FetchRecord)
        ]
    ]

    Html.divc "notification is-danger" [
        Html.buttonc "delete" [
            Ev.onClick (fun _ -> dispatch ClearError)
        ]
        Bind.el( model .>> errorMessage, text )
    ] |> Transition.transition [InOut fade] (model .>> errorExists)

]
```
