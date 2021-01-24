module SelectMultiple

open Sutil
open Sutil.DOM
open Sutil.Attr

let menu = [
    "Cookies and cream"
    "Mint choc chip"
    "Raspberry ripple"
]

let scoopMenu = [
    "One scoop"
    "Two scoops"
    "Three scoops"
]

// Text helpers
let plural word n =
    let s = if n = 1 then "" else "s"
    $"{word}{s}"

let scoop_s n = plural "scoop" n

let rec join (flavours : string list) =
    match flavours with
    | [] -> ""
    | x :: [y] -> $"{x} and {y}"
    | [x] -> x
    | x :: xs -> $"{x}, {join xs}"

// HTML helpers
let block children =
    Html.div <| (class' "block") :: children

// Control with only 1 label child
let controlLabel children =
    Html.div [ class' "control"; Html.label children ]

let label s = Html.label [ class' "label"; text s ]

// Main component view
let view() =
    let flavours = Store.make( [ menu |> List.head ] )
    let numFlavours = flavours |> Store.map (fun x -> x.Length)
    let scoops = Store.make(1)

    Html.div [
        disposeOnUnmount [ flavours; scoops ]

        block [
            label "Scoops"
            scoopMenu |> List.mapi (fun i scoopChoice ->
                controlLabel [
                    class' "radio"
                    Html.input [
                        type' "radio"
                        Bindings.bindRadioGroup scoops
                        i+1 |> string |> value
                    ]
                    text $" {scoopChoice}"
                ]) |> fragment
        ]

        block [
            label "Flavours"
            Html.div [
                class' "select is-multiple"
                Html.select [
                    multiple
                    Bindings.bindSelectMultiple flavours
                    fragment [
                        for flavour in menu do
                            Html.option [
                                value flavour
                                text $" {flavour}"
                            ]
                    ]
                ]
            ]
        ]

        block [
            Bindings.bind2 scoops flavours (fun (s,f) ->
                match (s,f) with
                | (_,[]) -> text "Please select at least one flavour"
                | (s,f) when f.Length > s -> text "Can't order more flavours than scoops!"
                | _ -> text $"You ordered {s} {scoop_s s} of {join f}")
        ]
    ]