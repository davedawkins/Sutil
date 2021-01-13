module Sveltish.Devtools

open Browser.Types
open Chrome.Devtools
open Sveltish
open Sveltish.DOM
open Sveltish.Attr
open Sveltish.Styling
open Browser.Dom

open Fable.Core
open Fable.Core.JsInterop

[<Import("injectedGetStores", from="./inject.js")>]
let injectedGetStores() : obj array = jsNative

type Message =
    | ViewStores

let mutable panel: Panel = Unchecked.defaultof<_>

let styleSheet = [
    rule ".sv-container" [ padding "12px";minHeight "100vh" ]
    rule ".sv-main" [ background "white"; minHeight "100vh" ]
    rule ".sv-sidebar" [ background "#eeeeee";borderRight "1pt solid #cccccc" ]
    rule "#sv-title" [ marginBottom "4px" ]
    rule ".sv-menu li" [ fontSize "90%"; cursor "pointer" ]
    rule ".sv-menu li:hover" [ textDecoration "underline" ]
    rule ".sv-menu li.active" [ fontWeight "bold" ]
]

let contentRun<'T,'A> (fn : 'A -> 'T) (arg:'A) (success : 'T -> unit) (failure: string -> unit) =
    console.log($" ({fn})({arg})")
    Chrome.Devtools.InspectedWindow.eval
        $"({fn})({arg})"
        {| |}
        (fun result ->
            if Interop.isUndefined result then failure("Unknown error") else success result)
            //if not (Interop.isUndefined result)
            //    then success(result)
            //else if not (Interop.isUndefined error)
            //    then failure(error)
            //else
            //    failwith "No result return from eval")

let buildStoresTable (idVals : obj array) =
    Html.div [
        Html.table [
            Html.thead [
                Html.tr [
                    Html.th [ text "Id" ]
                    Html.th [ text "Val" ]
                ]
            ]
            Html.tbody [
                for item in idVals do
                    Html.tr [
                        Html.td [ text (string item?Id) ]
                        Html.td [ text (Fable.Core.JS.JSON.stringify item?Val) ]
                    ]
            ]
        ]
    ] |> withStyle styleSheet

let viewStores (doc:Document) =
    console.log("Show stores")
    contentRun
        injectedGetStores
        ()
        (fun result ->
            mountElementOnDocument doc "sv-view" (buildStoresTable result?Data)
            console.dir(result)
            )
        (fun error -> console.dir(error))

// Working towards MVU
let dispatch doc (msg : Message) : unit =
    match msg with
    | ViewStores -> viewStores doc

let view dispatch  =
    Html.div [
        class' "sv-container"
        Html.div [
            class' "columns"
            Html.div [
                class' "sv-sidebar column is-one-fifth"
                Html.h4 [
                    id' "sv-title"
                    class' "title is-5"
                    text "Sveltish"
                ]
                Html.ul [
                    class' "sv-menu"
                    Html.li [
                        id' "stores"
                        onClick (fun _ -> dispatch ViewStores ) []
                        text "Stores" ]
                    Html.li [ text "Styles" ]
                    Html.li [ text "Maps" ]
                    Html.li [ text "Element Bindings" ]
                    Html.li [ text "Attribute Bindings" ]
                ]
            ]
            Html.div [
                class' "sv-main column is-four-fifths"
                Html.div [ id' "sv-view" ] ] ] ] |> withStyle styleSheet

let initialisePanel (win: Window) =
    view (dispatch win.document)
    |> mountElementOnDocument win.document "sveltish-app"

let unInitialisePanel (win: Window) = ()

let init (p: Panel) =
    panel <- p
    panel.onShown.addListener initialisePanel
    panel.onHidden.addListener unInitialisePanel

Panels.create
    "Sveltish Fable" // title
    "/icon.png" // icon
    "/html/panel.html"
    init
