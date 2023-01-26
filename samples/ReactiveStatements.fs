module ReactiveStatements

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Core
open Sutil.CoreElements

open Browser.Dom

let inc n = n + 1
let plural n = if n = 1 then "" else "s"


let view() =
    let count = Store.make 0

    count |> Store.write (fun n ->
        if n >= 10 then
            window.alert("count is dangerously high!")
            count <~ 9
        )

    let handleClick _ =
        count <~= inc   // or: Store.modify count inc

    Html.button [
        disposeOnUnmount [count]

        onClick handleClick []
        count |=> (fun n -> text $"Clicked {n} time{plural n}")
    ]
view() |> Program.mountElement "sutil-app"
