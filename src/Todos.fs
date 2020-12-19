module Todos

open Sveltish
open Browser.Dom
open Sveltish.Stores
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM

type Todo = {
        Id : int
        mutable Done: bool
        Description: string
    }

let todos = makeStore [
    { Id = 1; Done = false; Description = "1:write some docs" }
    { Id = 2; Done = false; Description = "2:start writing JSConf talk" }
    { Id = 3; Done =  true;  Description = "3:buy some milk" }
    { Id = 4; Done = false; Description = "4:mow the lawn" }
    { Id = 5; Done = false; Description = "5:feed the turtle" }
    { Id = 6; Done = false; Description = "6:fix some bugs" }
]
let newUid = CodeGeneration.makeIdGenerator()

let add(desc) =
    let todo = {
        Id = newUid()
        Done = false
        Description = desc
    }

    todos.Set( todo :: todos.Value() )

    //input.value =

let remove(todo) =
    todos.Set( todos.Value() |> List.filter (fun t -> t <> todo) )

let styleSheet = [
    rule ".new-todo" [
        fontSize "1.4em"
        width "100%"
        margin "2em 0 1em 0"
    ]

    rule ".board" [
        maxWidth "36em"
        margin "0 auto"
    ]

    rule ".left, .right" [
        ``float`` "left"
        width "50%"
        padding "0 1em 0 0"
        boxSizing "border-box"
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

    rule ".right label" [
        backgroundColor "rgb(180,240,100)"
    ]

    rule "button" [
        ``float`` "right"
        height "1em"
        boxSizing "border-box"
        padding "0 0.5em"
        lineHeight "1"
        backgroundColor "transparent"
        border "none"
        color "rgb(170,30,30)"
        opacity "0"
        transition "opacity 0.2s"
    ]

    rule "label:hover button" [
        opacity "1"
    ]
]

let toBool obj =
    match string obj with
    | "1" -> true
    | "1.0" -> true
    | "on" -> true
    | "true" -> true
    | "yes" -> true
    | _ -> false

open Sveltish.Bindings

let todosList cls title filter tin tout =

    Html.div [
        className cls
        Html.h2 [ text title ]

        Bindings.each todos (fun (x:Todo) -> x.Id) filter (InOut (tin,tout) ) (fun todo ->
            Html.label [
                Html.input [
                    attr ("type","checkbox")
                    Bindings.bindAttr "checked"
                        ((makePropertyStore todo "Done") <~| todos)
                ]
                text " "
                text todo.Description
                Html.button [
                    on "click" (fun _ -> remove(todo))
                    text "x"
                ]
            ]
        )
    ]

let view : NodeFactory =
    let (send,recv) = Transition.crossfade [ ]
    let tsend = send, []
    let trecv = recv, []

    style styleSheet <| Html.div [
        className "board"
        Html.input [
            className "new-todo"
            attr ("placeholder","what needs to be done?")
            on "keydown" (fun e ->
                let key = (e :?> Browser.Types.KeyboardEvent).key
                if key = "Enter" then add( (e.currentTarget :?> Browser.Types.HTMLInputElement).value )
            )
        ]


        todosList "left" "todo" (fun t -> not t.Done) trecv tsend
        todosList "right" "done" (fun t -> t.Done) trecv tsend
    ]
