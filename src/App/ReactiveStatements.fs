module ReactiveStatements

open Sutil
open Sutil.DOM
open Sutil.Bindings
open Sutil.Attr
open Browser.Dom

let count = Store.make 0

let inc n = n + 1
let plural n = if n = 1 then "" else "s"

count |> Store.write (fun n ->
    if n >= 10 then
        window.alert("count is dangerously high!")
        count <~ 9
    )

let handleClick _ =
    count <~= inc   // or: Store.modify count inc

let view() =
    Html.button [
        onClick handleClick []
        count |=> (fun n -> text $"Clicked {n} time{plural n}")
    ]