module ReactiveStatements

open Sveltish
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Attr
open Browser.Dom

let count = Store.make 0

let inc n = n + 1
let plural n = if n = 1 then "" else "s"

Store.subscribe count (fun n ->
    if n >= 10 then
        window.alert("count is dangerously high!")
        count <~ 9
    ) |> ignore // throw away the unsubscription function

let handleClick _ =
    count <~= inc   // or: Store.modify count inc

let view() =
    Html.button [
        onClick handleClick
        count |=> (fun n -> text $"Clicked {n} time{plural n}")
    ]