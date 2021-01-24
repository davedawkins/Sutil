module Todos

open Sutil
open Sutil.Styling
open Sutil.Attr
open Sutil.DOM
open Sutil.Bindings
open Browser.Types

open Sutil.Transition

type Todo = {
        Id : int
        Done: bool
        Description: string
    }

// Todo helpers
let isDone t = t.Done
let isPending = isDone >> not
let key r = r.Id
let hasKey k t = t.Id = k

type Model = {
    Todos : List<Todo>
} with
    override this.ToString() = $"The Model: {this.Todos.Length} items"

type Message =
    |AddTodo of desc:string
    |ToggleTodo of id:int
    |DeleteTodo of id:int
    |CompleteAll

let makeExampleTodos() = [
    { Id = 1; Done = false; Description = "write some docs" }
    { Id = 2; Done = false; Description = "start writing JSConf talk" }
    { Id = 3; Done =  true; Description = "buy some milk" }
    { Id = 4; Done = false; Description = "mow the lawn" }
    { Id = 5; Done = false; Description = "feed the turtle" }
    { Id = 6; Done = false; Description = "fix some bugs" }
]

let newUid = Helpers.makeIdGeneratorFrom(7)

let styleSheet = [

    rule ".new-todo" [
        fontSize "1.4em"
        width "100%"
        //margin "2em 0 1em 0"
    ]

    rule ".board" [
        maxWidth "36em"
        margin "0 auto"
    ]

    rule ".todo, .done" [
        //float' "left"
        width "50%"
        padding "0 1em 0 0"
        boxSizing "border-box"
    ]

    rule ".title" [
        marginTop "24px"
    ]

    rule "h2" [
        fontSize "2em"
        fontWeight  "200"
        userSelect  "none"
    ]

    rule "label"  [
        top "0"
        left "0"
        display "block"
        fontSize "1em"
        lineHeight "1"
        padding "0.5em"
        margin "0 auto 0.5em auto"
        borderRadius "2px"
        backgroundColor "#eee"
        userSelect "none"
    ]

    rule "input" [  margin "0" ]

    rule ".done label" [
        backgroundColor "rgb(180,240,100)"
    ]

    rule "label>button" [
        float' "right"
        height "1em"
        boxSizing "border-box"
        padding "0 0.5em"
        lineHeight "1"
        backgroundColor "transparent"
        border "none"
        color "rgb(170,30,30)"
        opacity "0"
        Attr.transition "opacity 0.2s"
    ]

    rule "label:hover button" [
        opacity "1"
    ]

    rule ".row" [
        display "flex"
    ]

    rule ".kudos" [
        fontSize "80%"
        color "#888"
    ]

    rule "div.complete-all-container" [
        display "flex"
        justifyContent "space-between"
        marginTop "4px"
    ]

    rule ".complete-all-container a" [
        cursor "pointer"
        textDecoration "none"

        fontSize "80%"
        color "#888"
    ]

    rule ".complete-all-container a:hover" [
        textDecoration "underline"
    ]
]

let init() = { Todos = makeExampleTodos() }

let toggle id todo = if todo.Id = id then { todo with Done = not todo.Done } else todo

let update (message : Message) (model : Model) : Model =
    match message with
    | AddTodo desc ->
        let todo = {
            Id = newUid() + 10
            Done = false
            Description = desc
        }
        { model with Todos = model.Todos @ [ todo ] }
    | ToggleTodo id ->
        { model with Todos = model.Todos |> List.map (toggle id) }
    | DeleteTodo id ->
        { model with Todos = model.Todos |> List.filter (fun t -> t.Id <> id) }
    | CompleteAll ->
        { model with Todos = model.Todos |> List.map (fun t -> { t with Done = true }) }


let fader  x = transition <| [ InOut (fade  |> withProps [ Duration 300.0 ])] <| x
let slider x = transition <| [ InOut (slide |> withProps [ Duration 300.0 ])]  <| x

let todosList title (filter : Todo -> bool) tin tout model dispatch =
    let filteredTodos = model |> Store.map (fun x -> x.Todos |> List.filter filter)
    Html.div [
        class' title
        Html.h2 [ text title ]

        eachk filteredTodos (fun todo ->
            Html.label [
                Html.input [
                    type' "checkbox"
                    attrNotify "checked" todo.Done (fun _ -> todo.Id |> ToggleTodo |> dispatch)
                    ]
                text $" {todo.Description}"
                Html.button [
                    onClick (fun _ -> todo.Id |> DeleteTodo |> dispatch) []
                    text "x"
                ]
            ]
        ) key [In tin; Out tout]
    ]

let makeStore = Store.makeElmishSimple init update ignore

let fallback (props : TransitionProp list) (node : HTMLElement) = fun _ ->
    let transform = computedStyleTransform node

    { (applyProps props Transition.Default) with
            Duration = 600.0
            Ease = Easing.quintOut
            Css = Some(fun t _ -> $"transform: {transform} scale({t}); opacity: {t}") }

let view () : NodeFactory =
    let (send,recv) = crossfade [ Fallback fallback ]

    let model, dispatch = makeStore()

    let completed = model |> Store.map (fun m -> m.Todos |> List.filter isDone)
    let lotsDone  = completed |> Store.map (fun x -> (x |> List.length >= 3))

    withStyle styleSheet <| Html.div [
        class' "board"

        Html.input [
            class' "new-todo"
            placeholder "what needs to be done?"
            onKeyDown (fun e ->
                // This isn't the right test for mobile users
                if e.key = "Enter" then (e.currentTarget :?> HTMLInputElement).value |> AddTodo |> dispatch
                printfn($"{e.key}")
            ) []
        ]

        Html.div [
            class' "complete-all-container"
            Html.a [
                href "#"
                text "complete all"
                onClick (fun _ -> dispatch CompleteAll) [ PreventDefault ]
            ]
            Html.span [
                class' "kudos"
                bind completed (fun x -> text $"{x.Length} tasks completed! Good job!")
            ] |> fader lotsDone
        ]

        Html.div [
            class' "row"
            todosList "todo" isPending recv send model dispatch
            todosList "done" isDone recv send model dispatch
        ]
    ]