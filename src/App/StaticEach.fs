module StaticEachBlocks

// Adapted from
// https://svelte.dev/examples

open Sutil.DOM

type Cat = { Id : string; Name : string }

let cats = [
    { Id = "J---aiyznGQ"; Name = "Keyboard Cat" }
    { Id = "z_AbfPXTKms"; Name = "Maru" }
    { Id = "OUtn3pvWmpg"; Name = "Henri The Existential Cat" }
]

let view() =
    Html.div [
        Html.h4 [ text "The Famous Cats of YouTube" ]
        Html.ul [

            // Simplest "each" case, but one-time generation only, and no index.
            // If the list is updated, then the view won't change
            for cat in cats do
                Html.li [
                    Html.a [
                        Attr.target "_blank"
                        Attr.href $"https://www.youtube.com/watch?v={cat.Id}"
                        text $"• {cat.Name}"
                    ]
                ]
        ]
    ]