module FileInputs

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Bindings
open Browser.Types
open Browser.Dom

let view() =
    let files = Store.make Unchecked.defaultof<FileList>
    let fileSeq = files |> Store.map (Helpers.fileListToSeq >> Seq.toList)

    fileSeq |> (Store.write (fun fileSeq ->
        // Note that `files` is of type `FileList`, not an Array:
        // https://developer.mozilla.org/en-US/docs/Web/API/FileList
        //console.log(files);

        for file in fileSeq do
            console.log($"{file.name}: {file.size} bytes")))

    Html.div [
        disposeOnUnmount [ files ]

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "avatar"; text "Upload a picture:" ]
            Html.input [
                accept "image/png, image/jpeg"
                Bind.attr("files",files)
                id' "avatar"
                name "avatar"
                type' "file"
            ]
        ]

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "many";text "Upload multiple files of any type:" ]
            Html.input [
                Bind.attr("files",files)
                id' "many"
                multiple
                type' "file"
            ]
        ]

        Bind.fragment fileSeq <| fun _files ->
                                Html.div [
                                    class' "control"
                                    Html.h3 [ text "Selected files" ]
                                    for file in _files do
                                        Html.p [ text $"{file.name} ({file.size} bytes)" ]
                                ]
    ]