module FileInputs

// Adapted from
// https://svelte.dev/examples

open Sutil
open Sutil.Core
open Sutil.CoreElements
open Sutil.Core



let view() =
    let files = Store.make Unchecked.defaultof<Browser.Types.FileList>
    let fileSeq = files |> Store.map (Helpers.fileListToSeq >> Seq.toList)

    fileSeq |> (Store.iter (fun fileSeq ->
        // Note that `files` is of type `FileList`, not an Array:
        // https://developer.mozilla.org/en-US/docs/Web/API/FileList
        //console.log(files);

        for file in fileSeq do
            Browser.Dom.console.log($"{file.name}: {file.size} bytes")))

    Html.div [
        disposeOnUnmount [ files ]

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "avatar"; text "Upload a picture:" ]
            Html.input [
                Attr.accept "image/png, image/jpeg"
                Bind.attr("files",files)
                Attr.id "avatar"
                Attr.name "avatar"
                Attr.typeFile
            ]
        ]

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "many";text "Upload multiple files of any type:" ]
            Html.input [
                Bind.attr("files",files)
                Attr.id "many"
                Attr.multiple true
                Attr.typeFile
            ]
        ]

        Bind.el(fileSeq, fun _files ->
                                Html.div [
                                    class' "control"
                                    Html.h3 [ text "Selected files" ]
                                    for file in _files do
                                        Html.p [ text $"{file.name} ({file.size} bytes)" ]
                                ])
    ]
