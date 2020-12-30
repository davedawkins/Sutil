module App

open Fetch
open Sveltish
open Sveltish.Attr
open Sveltish.Styling
open Sveltish.DOM
open Sveltish.Bindings
open Sveltish.Transition
open Browser.Dom

let urlBase = "https://raw.githubusercontent.com/davedawkins/Fable.Sveltish/main/src/App"
//let log s = console.log(s)
let make v = v

type Model = {
    Demo : string
    Tab : string
    Source : string
}

type Message =
    | SetDemo of string
    | SetTab of string
    | FetchSource
    | SetSource of string

type Demo = {
    Title : string
    Category : string
    Create : (unit -> NodeFactory)
    Sources : string list
} with
    static member All = [
        { Category = "Introduction";Title = "Hello World";  Create = make HelloWorld.helloWorld ; Sources = ["HelloWorld.fs"]}
        { Category = "Introduction";Title = "Dynamic attributes";  Create = make DynamicAttributes.view ; Sources = ["DynamicAttributes.fs"]}
        { Category = "Introduction";Title = "Styling";  Create = make StylingExample.view ; Sources = ["Styling.fs"]}
        { Category = "Introduction";Title = "Nested components";  Create = make NestedComponents.view ; Sources = ["NestedComponents.fs"; "Nested.fs"]}
        { Category = "Reactivity";Title = "Reactive assignments";  Create = make Counter.Counter ; Sources = ["Counter.fs"]}
        { Category = "Reactivity";Title = "Reactive declarations";  Create = make ReactiveDeclarations.view ; Sources = ["ReactiveDeclarations.fs"]}
        { Category = "Reactivity";Title = "Reactive statements";  Create = make ReactiveStatements.view ; Sources = ["ReactiveStatements.fs"]}
        { Category = "Logic"; Title = "If blocks"; Create = make LogicIf.view; Sources = ["LogicIf.fs"] }
        { Category = "Logic"; Title = "Else blocks"; Create = make LogicElse.view; Sources = ["LogicElse.fs"] }
        { Category = "Logic"; Title = "Else-if blocks"; Create = make LogicElseIf.view; Sources = ["LogicElseIf.fs"] }
        { Category = "Transitions"; Title = "Transitions w/ animation"; Create = make Todos.view; Sources = ["Todos.fs"] }
        { Category = "Bindings";   Title = "Text inputs";  Create = make TextInputs.view ; Sources = ["TextInputs.fs"]}
        { Category = "Bindings";   Title = "Numeric inputs";  Create = make NumericInputs.view ; Sources = ["NumericInputs.fs"]}
        { Category = "Bindings";   Title = "Checkbox inputs";  Create = make CheckboxInputs.view ; Sources = ["CheckboxInputs.fs"]}
        { Category = "Bindings";   Title = "Group inputs";  Create = make GroupInputs.view ; Sources = ["GroupInputs.fs"]}
        { Category = "Bindings";   Title = "Select bindings";  Create = make SelectBindings.view ; Sources = ["SelectBindings.fs"]}
        { Category = "Bindings";   Title = "Select multiple";  Create = make SelectMultiple.view ; Sources = ["SelectMultiple.fs"]}
    ]

let fetchSource tab dispatch =
    let url = sprintf "%s/%s" urlBase tab
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (SetSource >> dispatch)
    |> ignore

module Cmd =
    let none = [ ]
    let ofMsg msg = [ fun d -> d msg ]

let init() =
    {
        Demo = Demo.All.Head.Title
        Source = ""
        Tab =  "demo"
    }, Cmd.none

let update msg model : Model * ObservableStore.Cmd<Message> =
    match msg with
    | SetTab t ->
        let cmd = if t = "demo" then Cmd.none else Cmd.ofMsg FetchSource
        { model with Tab = t }, cmd
    | SetDemo d ->
        { model with Demo = d; Source = ""; Tab = "demo" }, Cmd.none
    | SetSource src ->
        { model with Source = src }, Cmd.none
    | FetchSource ->
        model, [ fetchSource model.Tab ]

let mainStyleSheet = [

    rule ".app-main" [
        height "100%"
    ]

    rule ".app-heading" [
        position "fixed"
        width "100vw"
        backgroundColor "white"
        padding "12px"
        boxShadow "-0.4rem 0.01rem 0.3rem rgba(0,0,0,.5)"
        marginBottom "4px"
        zIndex "100"
    ]

    rule ".app-contents" [
        backgroundColor "#676778"
        color "white"
    ]

    rule ".app-contents ul" [
        paddingLeft "20px"
    ]

    rule ".app-contents .title" [
        color "white"
        marginLeft "12px"
        marginBottom "8px"
        marginTop "16px"
    ]

    rule "a" [
        cursor "pointer"
        color "white"
        textDecoration "none"
    ]

    rule "a:hover" [
        color "white"
        textDecoration "underline"
    ]

    rule ".app-main-section" [
        marginTop "0px"
        paddingTop "50px"
        height "100%"
    ]

    rule ".app-demo" [
        backgroundColor "white"
    ]

    rule ".app-heading a" [
        color "#676778"
    ]

    rule ".app-toolbar a" [
        color "#676778"
        fontSize "80%"
        padding "12px"
    ]

    rule ".app-toolbar ul" [
        display "inline"
    ]

    rule ".app-toolbar li" [
        display "inline"
    ]

    rule "pre" [
        padding 0
    ]


    rule "input[type='text']" [
        addClass "input"
    ]

    rule "input[type='radio']" [
        addClass "radio"
    ]

    rule "input[type='checkbox']" [
        addClass "checkbox"
    ]

    rule "input[type='number']" [
        addClass "input"
        addClass "is-small"
        maxWidth "50%"
    ]

    rule "input[type='range']" [
        addClass "input"
        addClass "is-small"
        maxWidth "50%"
    ]

    rule "h2" [ addClass "title"; addClass "is-2" ]
    rule "h3" [ addClass "title"; addClass "is-3" ]
    rule "h4" [ addClass "title"; addClass "is-4" ]
    rule "h5" [ addClass "title"; addClass "is-5" ]
    rule "button" [ addClass "button" ]
]

let demos (model : IStore<Model>) =
    Html.div [
        class' "column app-demo"
        for d in Demo.All do
            d.Create() |> show (model |> Store.map (fun m -> m.Demo = d.Title))
    ]

let Section (name:string) model dispatch = [
    Html.h5 [ class' "title is-6"; text (name.ToUpper()) ]
    Html.ul [
        for d in Demo.All |> List.filter (fun x -> x.Category = name) do
            Html.li [
                Html.a [
                    href "#"
                    text d.Title
                    onClick (fun e -> e.preventDefault(); d.Title |> SetDemo |> dispatch )
                ]
            ]
        ]
    ]

let findDemo (model : Model) =
    Demo.All |> List.find (fun d -> d.Title = model.Demo)

let tabItem dispatch name  =
    Html.li [
        Html.a [
            href "#"
            text name
            onClick (fun e -> e.preventDefault(); (SetTab name |> dispatch))
        ]
    ]

let viewSource (model : IStore<Model>) dispatch =
    let source = model |> Store.map (fun m -> m.Source)
    Html.div [
        class' "column"
        //on "sveltish-show" <| fun _ -> fetchSource (model |> Store.getMap (fun m -> m.Tab)) dispatch
        //on "sveltish-show" <| fun e -> log($"show source {e.target}"); dispatch FetchSource
        Html.pre [
            Html.code [
                class' "fsharp"
                on "sveltish-show" <| fun e -> log($"2show source {e.target}"); e.stopPropagation()
                bind source text
            ]
        ]
    ]

let makeStore = Store.makeElmish init update ignore

let appMain () =

    let model,dispatch = makeStore()

    let currentDemo = model |> Store.map findDemo
    let tab = model |> Store.map (fun m -> m.Tab)

    withStyle mainStyleSheet <|
        Html.div [
            class' "app-main"

            Html.div [
                class' "app-heading"
                Html.h1 [ class' "title is-4"; Html.a [ href "https://github.com/davedawkins/Fable.Sveltish"; text "sveltish" ] ]
            ]

            Html.div [
                class' "columns app-main-section"

                Html.div <|
                    (class' "column is-one-quarter app-contents") ::
                    Section "Introduction" model dispatch @
                    Section "Reactivity" model dispatch @
                    Section "Logic" model dispatch @
                    Section "Transitions" model dispatch @
                    Section "Bindings" model dispatch

                Html.div [
                    class' "column app-demo-section"

                    Html.div [
                        class' "app-toolbar"
                        bind currentDemo (fun demo ->
                            Html.ul [
                                class' "app-tab"
                                tabItem dispatch "demo"
                                demo.Sources |> List.map (tabItem dispatch) |> fragment
                            ]
                        )
                    ]

                    transitionMatch tab <| [
                        ((fun t -> t = "demo"),  demos model,      None)
                        ((fun t -> t <> "demo"), viewSource model dispatch, None)
                    ]
                ]
            ]
        ]

let app() =
    Html.app [
        // Page title
        headTitle "Sveltish"

        // Bulma style framework
        headStylesheet "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css"

        // Build the app
        appMain()
    ]

app() |> mountElement "sveltish-app"
