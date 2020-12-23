module App

open Sveltish
open Sveltish.Attr
open Sveltish.Styling
open Sveltish.DOM
open Sveltish.Stores
open Sveltish.Bindings
open Browser.Dom

let helloWorld _ _ = Html.div [ text "Hello World" ]

let counter _ _  = Counter.Counter { ShowHint = true; Label = "Click Me"; InitialCounter = 0 }

type Model = {
    Demo : Store<string>
    TodosModel : Todos.Model
    ShowingSource : Store<bool>
}

type Message =
    | SetDemo of string
    | TodosMsg of Todos.Message
    | ToggleSource


type Demo = {
    Title : string
    Category : string
    Create : (Model -> (Message->unit) -> NodeFactory)
    Source : string
} with
    static member All = [
        { Title = "Hello World"; Category = "Introduction"; Create = helloWorld ; Source = "Hello world source"}
        { Title = "Reactive Assignments"; Category = "Reactivity"; Create = counter ; Source = "Counter source"}
        { Title = "The animate directive"; Category = "Animations"; Create = (fun m d -> Todos.view m.TodosModel (d<<TodosMsg)); Source = "Todos Source" }
    ]

let init() =
    let todosModel = Todos.init()
    {
        Demo = makeStore("Hello World")
        TodosModel = todosModel
        ShowingSource = makeStore(false)
    }

let update msg model =
    match msg with
    | SetDemo d ->
        model.Demo <~ d
    | TodosMsg m ->
        Todos.update m model.TodosModel
    | ToggleSource -> model.ShowingSource <~ (model.ShowingSource |-> not)

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
        //height "100vh"
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
]

let currentDemo model dispatch =
    Html.div [
        class' "column app-demo"
        for d in Demo.All do
            d.Create model dispatch |> Bindings.show (model.Demo |%> (fun demo -> demo = d.Title))
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

let appMain (model:Model) (dispatch : Message -> unit) =

    style mainStyleSheet <|
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
                    Section "Animations" model dispatch

                Html.div [
                    class' "column app-demo-section"

                    //Html.div [
                    //    class' "app-toolbar"
                    //    Html.a [
                    //        href "#"
                    //        model.ShowingSource |=> (fun show -> text <| if show then "demo" else "source")
                    //        onClick (fun e -> e.preventDefault(); dispatch ToggleSource )
                    //    ]
                    //]

                    showElse model.ShowingSource
                        (Html.div [
                            class' "column"
                            on "sveltish-show" (fun _ -> console.log("Showing source"))
                            text "todo - fetch source code using Fable's equivalent of ajax"
                            ])
                        (currentDemo model dispatch)
                ]
            ]
        ]

let app model dispatch =
    Styling.app [
        headTitle "Sveltish"
        headStylesheet "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css"
        appMain model dispatch
    ]

Sveltish.Program.makeProgram "sveltish-app" init update app