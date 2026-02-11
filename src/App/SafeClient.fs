module SAFE

open Sutil.CoreElements
module Shared =

    open System

    type Todo =
        { Id : Guid
          Description : string }

    module Todo =
        let isValid (description: string) =
            String.IsNullOrWhiteSpace description |> not

        let create (description: string) =
            { Id = Guid.NewGuid()
              Description = description }

    module Route =
        let builder typeName methodName =
            sprintf "/api/%s/%s" typeName methodName

    type ITodosApi =
        { getTodos : unit -> Async<Todo list>
          addTodo : Todo -> Async<Todo> }

open Shared
open Sutil

type Model =
    { Todos: Todo list
      Input: string }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo

#if USE_REMOTE_SERVER

open Fable.Remoting.Client

let todosApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

#else

[<RequireQualifiedAccess>]
module MockServer =
    let mutable todos : List<Todo> = [
        Todo.create "Check out SAFE"
        Todo.create "Write killer app"
        Todo.create "Maximize stonks"
    ]
    let getTodos() = async { return todos }
    let addTodo todo = async {
        todos <- todos @ [ todo ]
        return todo
    }
    let createApi() = { getTodos = getTodos; addTodo = addTodo }

let todosApi =
    MockServer.createApi()

#endif

let init(): Model * Cmd<Msg> =
    let model =
        { Todos = []
          Input = "" }
    let cmd = Cmd.OfAsync.perform todosApi.getTodos () GotTodos
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotTodos todos ->
        { model with Todos = todos }, Cmd.none
    | SetInput value ->
        { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input
        let cmd = Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo
        { model with Input = "" }, cmd
    | AddedTodo todo ->
        { model with Todos = model.Todos @ [ todo ] }, Cmd.none

open Sutil.Bulma

open System

[<AutoOpen>]
module private Helpers =
    let (|>>) store f = store |> Store.map f
    let mInput m = m.Input
    let mTodos m = m.Todos
    let tId t = t.Id
    let inline xlog m =
        Browser.Dom.console.log(string m)
        m

let navBrand =
    bulma.navbarBrand.div [
        bulma.navbarItem.a [
            Attr.href "https://safe-stack.github.io/"
            navbarItem.isActive
            Html.img [
                Attr.src "https://safe-stack.github.io/images/logos/safe_logo.png"
                Attr.alt "Logo"
            ]
        ]
    ]

let containerBox (model : IObservable<Model>) (dispatch : Msg -> unit) =
    bulma.box [
        bulma.content [
            Html.ol [
                Bind.each(
                    model |>> mTodos,
                    (fun todo -> Html.li [ Html.text todo.Description ]),
                    tId)
            ]
        ]
        bulma.field.div [
            field.isGrouped
            bulma.control.p [
                control.isExpanded
                bulma.input.text [
                    Attr.value( model |>> mInput, SetInput >> dispatch)
                    Attr.placeholder "What needs to be done?"
                ]
            ]
            bulma.control.p [
                bulma.button.a [
                    color.isPrimary
                    Attr.disabled (model |>> (mInput >> Todo.isValid >> not >> xlog))
                    onClick (fun _ -> dispatch AddTodo) []
                    Html.text "Add"
                ]
            ]
        ]
    ]

let view () =

    let model, dispatch = () |> Store.makeElmish init update ignore

    bulma.hero [
        hero.isFullheight
        color.isPrimary

        Attr.style [
            Css.backgroundSize "cover"
            Css.backgroundImageUrl "https://unsplash.it/1200/900?random"
            Css.backgroundPosition "no-repeat center center fixed"
        ]

        bulma.heroHead [
            //bulma.navbar [
                bulma.container [ navBrand ]
            //]
        ]
        bulma.heroBody [
            bulma.container [
                bulma.column [
                    column.is6
                    column.isOffset3
                    bulma.title.h1 [
                        text.hasTextCentered
                        Html.text "SAFE Todos"
                    ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
