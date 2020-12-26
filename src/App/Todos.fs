module Todos

open Sveltish
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Browser.Types

open Sveltish.Transition

type Todo = {
        Id : int
        mutable Done: bool
        Description: string
    }

// Todo helpers
let isDone t = t.Done
let isPending = isDone >> not
let key r = r.Id
let hasKey k t = t.Id = k
let toggleDone (t:Todo) = t.Done <- not t.Done

type Model = {
    Todos : Store<List<Todo>>
}

type Message =
    |AddTodo of desc:string
    |ToggleTodo of id:int
    |DeleteTodo of id:int
    |CompleteAll

let makeExampleTodos() = Store.make [
    { Id = 1; Done = false; Description = "1:write some docs" }
    { Id = 2; Done = false; Description = "2:start writing JSConf talk" }
    { Id = 3; Done =  true; Description = "3:buy some milk" }
    { Id = 4; Done = false; Description = "4:mow the lawn" }
    { Id = 5; Done = false; Description = "5:feed the turtle" }
    { Id = 6; Done = false; Description = "6:fix some bugs" }
]

let newUid = CodeGeneration.makeIdGeneratorFrom(7)

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
        marginTop "4px"
        marginBottom "4px"
        fontSize "80%"
        color "#aaa"
    ]

    rule "div.complete-all-container" [
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

let update (message : Message) (model : Model) : unit =

    match message with
    | AddTodo desc ->
        let todo = {
            Id = newUid() + 10
            Done = false
            Description = desc
        }
        model.Todos <~= (fun x -> x @ [ todo ]) // Mutation of model
    | ToggleTodo id ->
        model.Todos |> Store.modifyWhere (hasKey id)  toggleDone
    | DeleteTodo id ->
        model.Todos <~= List.filter (fun t -> t.Id <> id)
    | CompleteAll ->
        model.Todos <~= List.map (fun t -> { t with Done = true })


let fader  x = transition <| Both (fade,[ Duration 300.0 ]) <| x
let slider x = transition <| Both (slide,[ Duration 300.0 ])  <| x

let todosList title filter tin tout model dispatch =
    Html.div [
        class' title
        Html.h2 [ text title ]

        each model.Todos key filter (InOut (tin,tout) ) (fun todo ->
            Html.label [
                Html.input [
                    type' "checkbox"
                    attrNotify "checked" todo.Done (fun _ -> todo.Id |> ToggleTodo |> dispatch)
                ]
                text $" {todo.Description}"
                Html.button [
                    on "click" (fun _ -> todo.Id |> DeleteTodo |> dispatch)
                    text "x"
                ]
            ]
        )
    ]

let view (model : Model) dispatch : NodeFactory =
    let (send,recv) = Transition.crossfade [ ]
    let tsend = send, []
    let trecv = recv, []

    let completed = model.Todos |%> List.filter isDone
    let lotsDone  = completed |%> fun x -> (x |> List.length >= 3)

    style styleSheet <| Html.div [
        class' "board"

        Html.input [
            class' "new-todo"
            placeholder "what needs to be done?"
            onKeyDown (fun e ->
                // This isn't the right test for mobile users
                if e.key = "Enter" then (e.currentTarget :?> HTMLInputElement).value |> AddTodo |> dispatch
                printfn($"{e.key}")
            )
        ]

        Html.div [
            class' "complete-all-container"
            Html.a [
                href "#"
                text "complete all"
                on "click" (fun e -> e.preventDefault();dispatch CompleteAll)
            ]
        ]

        Html.div [
            class' "kudos"
            bind completed (fun x -> text $"{x.Length} tasks completed! Good job!")
        ] |> fader lotsDone

        Html.div [
            class' "row"
            todosList "todo" isPending trecv tsend model dispatch
            todosList "done" isDone trecv tsend model dispatch
        ]
    ]