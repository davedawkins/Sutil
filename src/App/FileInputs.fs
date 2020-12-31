module FileInputs

open Sveltish
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Bindings
open Browser.Types
open Browser.Dom

let files = Store.make Unchecked.defaultof<FileList>
let fileSeq = files |> Store.map (Helpers.fileListToSeq >> Seq.toList)


fileSeq |> (Store.write (fun fileSeq ->
    // Note that `files` is of type `FileList`, not an Array:
    // https://developer.mozilla.org/en-US/docs/Web/API/FileList
    //console.log(files);

    for file in fileSeq do
        console.log($"{file.name}: {file.size} bytes")))

let view() =
    Html.div [

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "avatar"; text "Upload a picture:" ]
            Html.input [
                accept "image/png, image/jpeg"
                bindAttrOut "files" files
                id' "avatar"
                name "avatar"
                type' "file"
            ]
        ]

        Html.div [
            class' "block"
            Html.label [ class' "file-label"; for' "many";text "Upload multiple files of any type:" ]
            Html.input [
                bindAttrOut "files" files
                id' "many"
                multiple
                type' "file"
            ]
        ]

        bind fileSeq <| fun _files ->
                                Html.div [
                                    class' "control"
                                    Html.h3 [ text "Selected files" ]
                                    for file in _files do
                                        Html.p [ text $"{file.name} ({file.size} bytes)" ]
                                ]
    ]