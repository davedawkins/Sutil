module App

open Fetch

open Sutil
open Sutil.Attr
open Sutil.Styling
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition
open System
open Browser.Types

let urlBase = "https://raw.githubusercontent.com/davedawkins/Sutil/main/src/App"
//let log s = console.log(s)

type Model = {
    Source : string
    ShowContents : bool
}

let showContents m = m.ShowContents

type Message =
    | FetchSource of string
    | SetSource of string
    | SetShowContents of bool
    | ToggleShowContents

let toHash (title:string) = title.ToLower().Replace(" ", "-")

type Demo = {
    Title : string
    Category : string
    Create : (unit -> NodeFactory)
    Sources : string list
} with
    static member All = [
        { Category = "Introduction";Title = "Hello World";  Create = HelloWorld.helloWorld ; Sources = ["HelloWorld.fs"]}
        { Category = "Introduction";Title = "Dynamic attributes";  Create = DynamicAttributes.view ; Sources = ["DynamicAttributes.fs"]}
        { Category = "Introduction";Title = "Styling";  Create = StylingExample.view ; Sources = ["Styling.fs"]}
        { Category = "Introduction";Title = "Nested components";  Create = NestedComponents.view ; Sources = ["NestedComponents.fs"; "Nested.fs"]}
        { Category = "Introduction";Title = "HTML tags";  Create = HtmlTags.view ; Sources = ["HtmlTags.fs"]}
        { Category = "Reactivity";Title = "Reactive assignments";  Create = Counter.Counter ; Sources = ["Counter.fs"]}
        { Category = "Reactivity";Title = "Reactive declarations";  Create = ReactiveDeclarations.view ; Sources = ["ReactiveDeclarations.fs"]}
        { Category = "Reactivity";Title = "Reactive statements";  Create = ReactiveStatements.view ; Sources = ["ReactiveStatements.fs"]}
        { Category = "Logic"; Title = "If blocks"; Create = LogicIf.view; Sources = ["LogicIf.fs"] }
        { Category = "Logic"; Title = "Else blocks"; Create = LogicElse.view; Sources = ["LogicElse.fs"] }
        { Category = "Logic"; Title = "Else-if blocks"; Create = LogicElseIf.view; Sources = ["LogicElseIf.fs"] }
        { Category = "Logic"; Title = "Static each blocks"; Create = StaticEachBlocks.view; Sources = ["StaticEach.fs"] }
        { Category = "Logic"; Title = "Static each with index"; Create = StaticEachWithIndex.view; Sources = ["StaticEachWithIndex.fs"] }
        { Category = "Logic"; Title = "Each blocks"; Create = EachBlocks.view; Sources = ["EachBlocks.fs"] }
        { Category = "Logic"; Title = "Keyed-each blocks"; Create = KeyedEachBlocks.view; Sources = ["KeyedEachBlocks.fs"] }
        { Category = "Logic"; Title = "Await blocks"; Create = AwaitBlocks.view; Sources = ["AwaitBlocks.fs"] }
        { Category = "Events"; Title = "DOM events"; Create = DomEvents.view; Sources = ["DomEvents.fs"] }
        { Category = "Events"; Title = "Event modifiers"; Create = EventModifiers.view; Sources = ["EventModifiers.fs"] }
        { Category = "Transitions"; Title = "Transition"; Create = Transition.view; Sources = ["Transition.fs"] }
        { Category = "Transitions"; Title = "Adding parameters"; Create = TransitionParameters.view; Sources = ["TransitionParameters.fs"] }
        { Category = "Transitions"; Title = "In and out"; Create = TransitionInOut.view; Sources = ["TransitionInOut.fs"] }
        { Category = "Transitions"; Title = "Custom CSS"; Create = TransitionCustomCss.view; Sources = ["TransitionCustomCss.fs"] }
        { Category = "Transitions"; Title = "Custom Code"; Create = TransitionCustom.view; Sources = ["TransitionCustom.fs"] }
        { Category = "Transitions"; Title = "Transition events"; Create = TransitionEvents.view; Sources = ["TransitionEvents.fs"] }
        { Category = "Transitions"; Title = "Animation"; Create = Todos.view; Sources = ["Todos.fs"] }
        { Category = "Bindings";   Title = "Text inputs";  Create = TextInputs.view ; Sources = ["TextInputs.fs"]}
        { Category = "Bindings";   Title = "Numeric inputs";  Create = NumericInputs.view ; Sources = ["NumericInputs.fs"]}
        { Category = "Bindings";   Title = "Checkbox inputs";  Create = CheckboxInputs.view ; Sources = ["CheckboxInputs.fs"]}
        { Category = "Bindings";   Title = "Group inputs";  Create = GroupInputs.view ; Sources = ["GroupInputs.fs"]}
        { Category = "Bindings";   Title = "Textarea inputs";  Create = TextArea.view ; Sources = ["TextArea.fs"]}
        { Category = "Bindings";   Title = "File inputs";  Create = FileInputs.view ; Sources = ["FileInputs.fs"]}
        { Category = "Bindings";   Title = "Select bindings";  Create = SelectBindings.view ; Sources = ["SelectBindings.fs"]}
        { Category = "Bindings";   Title = "Select multiple";  Create = SelectMultiple.view ; Sources = ["SelectMultiple.fs"]}
        { Category = "Bindings";   Title = "Dimensions";  Create = Dimensions.view ; Sources = ["Dimensions.fs"]}
        { Category = "Svg";   Title = "Bar chart";  Create = BarChart.view ; Sources = ["BarChart.fs"]}
        { Category = "Miscellaneous";   Title = "Spreadsheet";  Create = Spreadsheet.view ; Sources = ["Spreadsheet.fs"; "Evaluator.fs"; "Parser.fs"]}
        { Category = "Miscellaneous";   Title = "Modal";  Create = Modal.view ; Sources = ["Modal.fs"]}
        { Category = "Miscellaneous";   Title = "Login";  Create = LoginExample.create ; Sources = ["LoginExample.fs"; "Login.fs"]}
        { Category = "7Guis";   Title = "Cells";  Create = SevenGuisCells.view ; Sources = ["Cells.fs"]}
    ]

//
// I think we could just have
// type DemoView = { Demo : Demo; File = string opt }
//
type DemoView =
    | DemoApp of Demo
    | DemoSrc of Demo * string

let fetchSource tab dispatch =
    let url = sprintf "%s/%s" urlBase tab
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (SetSource >> dispatch)
    |> ignore

let init() =
    {
        Source = ""
        ShowContents = false
    }, Cmd.none

let update msg model : Model * Cmd<Message> =
    match msg with
    | ToggleShowContents ->
        { model with ShowContents = not model.ShowContents }, Cmd.none
    | SetShowContents f ->
        { model with ShowContents = f }, Cmd.none
    | SetSource content ->
        { model with Source = content }, Cmd.none
    | FetchSource file ->
        { model with Source = "// Loading..." }, [ fetchSource file ]

let mainStyleSheet = Bulma.withBulmaHelpers [

    rule ".app-main" [
        Css.height "100%" //(Pct 100.0)
    ]

    rule ".app-heading" [
        Css.display "flex"
        Css.flexDirection "row"
        Css.justifyContent "space-between"
        Css.position "fixed"
        Css.width "100vw"
        Css.backgroundColor "white"
        Css.padding "12px"
        //paddingBottom "4px"
        Css.boxShadow "-0.4rem 0.01rem 0.3rem rgba(0,0,0,.5)"
        Css.marginBottom "4px"
        Css.zIndex 1   // Messes with .modal button
    ]

    rule ".app-heading h1" [
       Css.marginBottom "0px"
    ]

    rule ".app-contents" [
        Css.backgroundColor "#676778"
        Css.color "white"
        Css.overflow "scroll"
    ]

    rule ".app-contents ul" [
        Css.paddingLeft "20px"
    ]

    rule ".app-contents .title" [
        Css.color "white"
        Css.marginLeft "12px"
        Css.marginBottom "8px"
        Css.marginTop "16px"
    ]

    rule ".app-contents a" [
        Css.cursor "pointer"
        Css.color "white"
        Css.textDecoration "none"
    ]

    rule ".app-contents a:hover" [
        Css.color "white"
        Css.textDecoration "underline"
    ]

    rule ".app-main-section" [
        Css.marginTop "0px"
        Css.paddingTop "50px"
        Css.height "100%"
    ]

    rule ".app-demo" [
        Css.backgroundColor "white"
    ]

    rule ".app-heading a" [
        Css.color "#676778"
    ]

    rule ".app-toolbar a" [
        Css.color "#676778"
        Css.fontSize "80%"
        Css.padding "12px"
    ]

    rule ".app-toolbar ul" [
        Css.display "inline"
    ]

    rule ".app-toolbar li" [
        Css.display "inline"
    ]

    rule "pre" [
        Css.padding 0
        Css.background "white"
    ]

    //rule ".logo" [
    //    Css.fontFamily "'HelveticaNeue-Light', 'Helvetica Neue Light', 'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif"
    //    Css.fontWeight  "400"
    //    Css.fontSize    "42px"
    //    Css.letterSpacing "2px"
    //]

    rule ".slogo" [
        Css.display "inline-flex"
        Css.fontFamily "'Coda Caption'"
        Css.alignItems "center"
        Css.justifyContent "center"
        Css.width "32px"
        Css.height "24px"
        Css.background "#444444"
        Css.color "white"
    ]

    rule ".show-contents-button" [
        Css.fontSize "18px"
    ]

]

let Section (name:string) model dispatch = fragment [
    Html.h5 [ class' "title is-6"; text (name.ToUpper()) ]
    Html.ul [
        for d in Demo.All |> List.filter (fun x -> x.Category = name) do
            Html.li [
                Html.a [
                    href ("#" + (toHash d.Title))
                    text d.Title
                ]
            ]
        ]
    ]

let tabItem (demo:Demo) name  =
    Html.li [
        Html.a [
            href <| "#" + (toHash demo.Title) + (if name = "demo" then "" else "?" + name)
            text name
        ]
    ]

let viewSource (model : IStore<Model>) =
    let source = model |> Store.map (fun m -> m.Source)
    Html.div [
        Html.pre [
            Html.code [
                class' "fsharp"
                //on "sutil-show" (fun e -> log($"2show source {e.target}")) [StopPropagation]
                Bind.fragment source (exclusive << text)
            ]
        ]
    ]

let demoTab (demoView : IObservable<DemoView>) model dispatch =
    Html.div [
        class' "column app-demo"
        Bind.fragment demoView (fun dv ->
            match dv with
            | DemoApp d ->
                SetShowContents false |> dispatch
                d.Create()
            | DemoSrc (d,file) ->
                if file = "" then
                    try
                        d.Create()
                    with
                        |x -> Html.div[ text $"Creating example {d.Title}: {x.Message}" ]
                else
                    file |> FetchSource |> dispatch
                    viewSource model
            )
    ]

let appMain (currentDemo : IObservable<DemoView>) (isMobile : IObservable<bool>) =
    let model, dispatch = () |> Store.makeElmish init update ignore

    // Show the contents if not on mobile, or model.ShowContents is true
    let showContents = ObservableX.zip isMobile model .> (fun (mob,mdl) -> not mob || mdl.ShowContents)

    withStyle mainStyleSheet <|
        Html.div [
            class' "app-main"

            Html.div [
                class' "app-heading"
                Html.h1 [
                    class' "title is-4"
                    Html.a [
                        href "https://github.com/davedawkins/Sutil"
                        Html.div [ class' "slogo"; Html.span [ text "<>" ] ]
                        text " SUTIL"
                    ]
                ]
                transition [InOut fade] isMobile <| Html.a [
                    class' "show-contents-button"
                    href "#"
                    Html.i [ class' "fa fa-bars" ]
                    onClick (fun _ -> ToggleShowContents |> dispatch) [ PreventDefault ]
                ]
            ]

            Html.div [
                class' "columns app-main-section"

                transition [fly |> withProps [ Duration 500.0; X -500.0 ] |> InOut] showContents <| Html.div [
                    class' "column is-one-quarter app-contents"
                    Section "Introduction" model dispatch
                    Section "Reactivity" model dispatch
                    Section "Logic" model dispatch
                    Section "Events" model dispatch
                    Section "Transitions" model dispatch
                    Section "Bindings" model dispatch
                    Section "Svg" model dispatch
                    Section "Miscellaneous" model dispatch
                    Section "7Guis" model dispatch
                ]

                Html.div [
                    class' "column app-demo-section"

                    Html.div [
                        class' "app-toolbar"
                        Bind.fragment currentDemo (fun demoView ->
                            let demo = match demoView with |DemoApp d -> d|DemoSrc (d,_) -> d
                            Html.ul [
                                class' "app-tab"
                                tabItem demo "demo"
                                demo.Sources |> List.map (tabItem demo) |> fragment
                            ]
                        )
                    ]

                    demoTab currentDemo model dispatch
                ]
            ]
        ]

let parseUrl (location: Location) =
    let hash =
        if location.hash.Length > 1 then location.hash.Substring 1
        else ""
    if hash.Contains("?") then
        let h = hash.Substring(0, hash.IndexOf("?"))
        h, hash.Substring(h.Length+1)
    else
        hash, ""

let parseDemoView (loc:Location) : DemoView =
    let hash, query = (parseUrl loc)

    match Demo.All |> List.tryFind (fun d -> toHash d.Title = hash) with
    | None -> DemoApp (Demo.All.Head)
    | Some demo ->
        if query = "" then
            DemoApp demo
        else
            // Don't allow '#?../../../etc/passwd'
            if demo.Sources |> List.contains query then
                DemoSrc (demo,query)
            else
                DemoApp demo

let app () =
    Html.app [
        // Page title
        headTitle "Sutil"

        // Bulma style framework
        headStylesheet "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css"

        // Build the app
        Navigable.navigable parseDemoView <| fun demo ->
            MediaQuery.media "(max-width: 768px)" id (appMain demo)
    ]

app() |> mountElement "sutil-app"
