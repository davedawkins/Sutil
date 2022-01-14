module StaticEachWithIndex

// Adapted from
// https://svelte.dev/examples

open Sutil

type Cat = { Id : string; Name : string }

let cats = [
    { Id = "J---aiyznGQ"; Name = "Keyboard Cat" }
    { Id = "z_AbfPXTKms"; Name = "Maru" }
    { Id = "OUtn3pvWmpg"; Name = "Henri The Existential Cat" }
]

let makePair a b = (a,b)
let withIndex xs = xs |> List.mapi makePair

let view() =
    Html.div [
        Html.h4 [ text "The Famous Cats of YouTube" ]
        Html.ul [

            // Simple "each" case with index, but one-time generation only.
            // If the list is updated, then the view won't change

            for (i,cat) in (withIndex cats) do
                Html.li [
                    Html.a [
                        Attr.target "_blank"
                        Attr.href $"https://www.youtube.com/watch?v={cat.Id}"
                        Html.text $"{i + 1}: {cat.Name}"
                    ]
                ]
        ]
    ]
view() |> Program.mountElement "sutil-app"
