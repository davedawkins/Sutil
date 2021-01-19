module App

open Fetch
open Sutil
open Sutil.Attr
open Sutil.Styling
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition
open Browser.Dom

let urlBase = "https://raw.githubusercontent.com/davedawkins/Sutil/main/src/App"
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
        { Category = "Introduction";Title = "HTML tags";  Create = make HtmlTags.view ; Sources = ["HtmlTags.fs"]}
        { Category = "Reactivity";Title = "Reactive assignments";  Create = make Counter.Counter ; Sources = ["Counter.fs"]}
        { Category = "Reactivity";Title = "Reactive declarations";  Create = make ReactiveDeclarations.view ; Sources = ["ReactiveDeclarations.fs"]}
        { Category = "Reactivity";Title = "Reactive statements";  Create = make ReactiveStatements.view ; Sources = ["ReactiveStatements.fs"]}
        { Category = "Logic"; Title = "If blocks"; Create = make LogicIf.view; Sources = ["LogicIf.fs"] }
        { Category = "Logic"; Title = "Else blocks"; Create = make LogicElse.view; Sources = ["LogicElse.fs"] }
        { Category = "Logic"; Title = "Else-if blocks"; Create = make LogicElseIf.view; Sources = ["LogicElseIf.fs"] }
        { Category = "Logic"; Title = "Static each blocks"; Create = make StaticEachBlocks.view; Sources = ["StaticEachBlocks.fs"] }
        { Category = "Logic"; Title = "Static each with index"; Create = make StaticEachWithIndex.view; Sources = ["StaticEachWithIndex.fs"] }
        { Category = "Logic"; Title = "Each blocks"; Create = make EachBlocks.view; Sources = ["EachBlocks.fs"] }
        { Category = "Logic"; Title = "Keyed-each blocks"; Create = make KeyedEachBlocks.view; Sources = ["KeyedEachBlocks.fs"] }
        { Category = "Logic"; Title = "Await blocks"; Create = make AwaitBlocks.view; Sources = ["AwaitBlocks.fs"] }
        { Category = "Events"; Title = "DOM events"; Create = make DomEvents.view; Sources = ["DomEvents.fs"] }
        { Category = "Events"; Title = "Event modifiers"; Create = make EventModifiers.view; Sources = ["EventModifiers.fs"] }
        { Category = "Transitions"; Title = "Transition"; Create = make Transition.view; Sources = ["Transition.fs"] }
        { Category = "Transitions"; Title = "Adding parameters"; Create = make TransitionParameters.view; Sources = ["TransitionParameters.fs"] }
        { Category = "Transitions"; Title = "In and out"; Create = make TransitionInOut.view; Sources = ["TransitionInOut.fs"] }
        { Category = "Transitions"; Title = "Custom CSS"; Create = make TransitionCustomCss.view; Sources = ["TransitionCustomCss.fs"] }
        { Category = "Transitions"; Title = "Transition events"; Create = make TransitionEvents.view; Sources = ["TransitionEvents.fs"] }
        { Category = "Transitions"; Title = "Animation"; Create = make Todos.view; Sources = ["Todos.fs"] }
        { Category = "Bindings";   Title = "Text inputs";  Create = make TextInputs.view ; Sources = ["TextInputs.fs"]}
        { Category = "Bindings";   Title = "Numeric inputs";  Create = make NumericInputs.view ; Sources = ["NumericInputs.fs"]}
        { Category = "Bindings";   Title = "Checkbox inputs";  Create = make CheckboxInputs.view ; Sources = ["CheckboxInputs.fs"]}
        { Category = "Bindings";   Title = "Group inputs";  Create = make GroupInputs.view ; Sources = ["GroupInputs.fs"]}
        { Category = "Bindings";   Title = "Textarea inputs";  Create = make TextArea.view ; Sources = ["TextArea.fs"]}
        { Category = "Bindings";   Title = "File inputs";  Create = make FileInputs.view ; Sources = ["FileInputs.fs"]}
        { Category = "Bindings";   Title = "Select bindings";  Create = make SelectBindings.view ; Sources = ["SelectBindings.fs"]}
        { Category = "Bindings";   Title = "Select multiple";  Create = make SelectMultiple.view ; Sources = ["SelectMultiple.fs"]}
        { Category = "Bindings";   Title = "Dimensions";  Create = make Dimensions.view ; Sources = ["Dimensions.fs"]}
        { Category = "Miscellaneous";   Title = "Spreadsheet";  Create = make Spreadsheet.view ; Sources = ["Spreadsheet.fs"; "Evaluator.fs"; "Parser.fs"]}
        { Category = "7Guis";   Title = "Cells";  Create = make SevenGuisCells.view ; Sources = ["Cells.fs"]}
    ]

let fetchSource tab dispatch =
    let url = sprintf "%s/%s" urlBase tab
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (SetSource >> dispatch)
    |> ignore

let init() =
    {
        Demo = Demo.All.Head.Title
        Source = ""
        Tab =  "demo"
    }, Cmd.none

let update msg model : Model * Cmd<Message> =
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

let mainStyleSheet = Bulma.withBulmaHelpers [

    rule ".app-main" [
        height "100%"
    ]

    rule ".app-heading" [
        position "fixed"
        width "100vw"
        backgroundColor "white"
        padding "12px"
        //paddingBottom "4px"
        boxShadow "-0.4rem 0.01rem 0.3rem rgba(0,0,0,.5)"
        marginBottom "4px"
        zIndex "100"
    ]

    rule ".app-contents" [
        backgroundColor "#676778"
        color "white"
        overflow "scroll"
    ]

    rule ".app-contents ul" [
        paddingLeft "20px"
    ]

    rule ".app-contents .title" [
        color "white"
        //padding "12px"
        marginLeft "12px"
        marginBottom "8px"
        marginTop "16px"
    ]

    rule ".app-contents a" [
        cursor "pointer"
        color "white"
        textDecoration "none"
    ]

    rule ".app-contents a:hover" [
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

    rule ".logo" [
        fontFamily "'HelveticaNeue-Light', 'Helvetica Neue Light', 'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif"
        //fontFamily "'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif"
        fontWeight  "400"
        fontSize    "42px"
        letterSpacing "2px"
    ]

    rule ".logo:hover" [
        textDecoration "underline"
        textDecorationColor "#bbbbbb"
        textDecorationThickness "0.01em"
    ]

    rule ".logo .lt, .logo .gt" [
        fontSize "40%"
        color "#bbbbbb"
        color "white"
    ]

    rule ".logo .stl" [
        color "#804041"
    ]

    rule ".logo .ui" [
        color "#804041"
        fontStyle "italic"
        color "#757575"
        fontWeight "400"
    ]

    rule ".slogo" [
        display "inline-flex"
        fontFamily "'Coda Caption'"
        alignItems "center"
        justifyContent "center"
        width "32px"
        height "24px"
        background "#444444"
        color "white"
    ]
]

let demos (model : IStore<Model>) =
    Html.div [
        class' "column app-demo"
        for d in Demo.All do
            d.Create() |> showIf (model .> (fun m -> m.Demo = d.Title))
    ]

let Section (name:string) model dispatch = fragment [
    Html.h5 [ class' "title is-6"; text (name.ToUpper()) ]
    Html.ul [
        for d in Demo.All |> List.filter (fun x -> x.Category = name) do
            Html.li [
                Html.a [
                    href "#"
                    text d.Title
                    onClick (fun _ -> d.Title |> SetDemo |> dispatch ) [PreventDefault]
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
            onClick (fun _ -> SetTab name |> dispatch) [PreventDefault]
        ]
    ]

let viewSource (model : IStore<Model>) dispatch =
    let source = model |> Store.map (fun m -> m.Source)
    Html.div [
        class' "column"
        Html.pre [
            Html.code [
                class' "fsharp"
                on "Sutil-show" (fun e -> log($"2show source {e.target}")) [StopPropagation]
                bind source text
            ]
        ]
    ]

let makeStore = Store.makeElmish init update ignore

let appMain () =

    let model,dispatch = makeStore()

    let currentDemo = model |> Store.map findDemo
    let tab = model |> Store.map (fun m -> m.Tab)

    let logo = Html.span [
        class' "logo"
        //Html.span [ class' "lt";  text "<" ]
        Html.span [ class' "stl"; text "s" ]
        Html.span [ class' "ui";  text "u" ]
        Html.span [ class' "stl"; text "t" ]
        Html.span [ class' "ui";  text "i" ]
        Html.span [ class' "stl"; text "l" ]
        //Html.span [ class' "gt";  text ">" ]
    ]

    withStyle mainStyleSheet <|
        Html.div [
            class' "app-main"

            Html.div [
                class' "app-heading"
                //Html.a [ href "https://github.com/davedawkins/Sutil"; logo ]
                Html.h1 [
                    class' "title is-4"
                    Html.a [
                        href "https://github.com/davedawkins/Sutil"
                        Html.div [ class' "slogo"; Html.span [ text "<>" ] ]
                        text " SUTIL"
                    ]
                ]
            ]

            Html.div [
                class' "columns app-main-section"

                Html.div [
                    class' "column is-one-quarter app-contents"
                    Section "Introduction" model dispatch
                    Section "Reactivity" model dispatch
                    Section "Logic" model dispatch
                    Section "Events" model dispatch
                    Section "Transitions" model dispatch
                    Section "Bindings" model dispatch
                    Section "Miscellaneous" model dispatch
                    Section "7Guis" model dispatch
                ]

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
        headTitle "Sutil"

        // Bulma style framework
        headStylesheet "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css"

        // Build the app
        appMain()
    ]

app() |> mountElement "Sutil-app"
