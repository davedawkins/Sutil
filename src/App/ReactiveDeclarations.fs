module ReactiveDeclarations

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings

let count      = Store.make 1
let doubled    = count |> Store.map ((*) 2)
let quadrupled = doubled |> Store.map ((*)2);

let handleClick _ =
    count |> Store.modify (fun n -> n + 1)

let view() =
    Html.div [
        Html.button [
            class' "block"
            onClick handleClick
            bind count (fun n -> text $"Count: {n}")
        ]

        Html.p [
            class' "block"
            bind2 count doubled
                (fun (c,d) -> text $"{c} * 2 = {d}")
        ]

        Html.p [
            class' "block"
            bind2 doubled quadrupled
                (fun (d,q) -> text $"{d} * 2 = {q}")
        ]
    ]