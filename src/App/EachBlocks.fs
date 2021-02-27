module EachBlocks

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM
open Sutil.Bindings
open Sutil.Attr

type Cat = { Id : string; Name : string }

let view() =
    let extraCat = { Id = "0Bmhjf0rKe8"; Name = "Surprise Kitten" }

    let cats = Store.make [
        { Id = "J---aiyznGQ"; Name = "Keyboard Cat" }
        { Id = "z_AbfPXTKms"; Name = "Maru" }
        { Id = "OUtn3pvWmpg"; Name = "Henri The Existential Cat" }
    ]

    let addCat cat =
       cats |> Store.modify (fun x -> x @ [cat])

    Html.div [
        disposeOnUnmount [ cats ]

        Html.h4 [ text "The Famous Cats of YouTube" ]
        Html.ul [
            // Each with dynamic binding, and index.
            // If the list changes, the view will update accordingly.
            // It isn't necessary to make a store to loop over a data structure,
            // see StaticEach.fs and StaticEachWithIndex.fs
            eachi cats (fun (i,cat) ->
                Html.li [
                    Html.a [
                        target "_blank"
                        href $"https://www.youtube.com/watch?v={cat.Id}"
                        text $"{i + 1}: {cat.Name}"
                    ]
                ]) []
        ]
        Html.button [
            style  [ Css.marginTop 12 ]
            text "More Cats"
            bindAttrIn "disabled" (cats |> Store.map (fun cats' -> cats'.Length = 4))
            onClick (fun _ -> addCat extraCat) []
        ]
    ]