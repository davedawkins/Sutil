module Todos

open Sveltish
open Fvelize
open Browser.Dom

(*
	import { quintOut } from 'svelte/easing';
	import { crossfade } from 'svelte/transition';
	import { flip } from 'svelte/animate';

	const [send, receive] = crossfade({
		fallback(node, params) {
			const style = getComputedStyle(node);
			const transform = style.transform === 'none' ? '' : style.transform;

			return {
				duration: 600,
				easing: quintOut,
				css: t => `
					transform: ${transform} scale(${t});
					opacity: ${t}
				`
			};
		}
	});
*)

type Todo = {
        Id : int
        mutable Done: bool
        Description: string
    }

let todos = Sveltish.makeStore [
    { Id = 1; Done = false; Description = "write some docs" }
    { Id = 2; Done = false; Description = "start writing JSConf talk" }
    { Id = 3; Done =  true;  Description = "buy some milk" }
    { Id = 4; Done = false; Description = "mow the lawn" }
    { Id = 5; Done = false; Description = "feed the turtle" }
    { Id = 6; Done = false; Description = "fix some bugs" } ]

let newUid = Fvelize.makeIdGenerator()

let add(desc) =
    let todo = {
        Id = newUid()
        Done = false
        Description = desc
    }

    console.log(sprintf "Adding %s" desc)

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

let todosList cls title filter () =
    div [
        className cls
        h2 [ str title ]
        for todo in todos.Value() |> List.filter filter do
            label [
                //in:receive="{{key: todo.id}}"
                //out:send="{{key: todo.id}}"
                //animate:flip

                input [
                    Attribute ("type","checkbox")
                    bindAttr
                        (makeGetSetStore
                            (fun () -> todo.Done :> obj)
                            (fun z -> todo.Done <- toBool z; todos.Value() |> todos.Set ))
                        "checked"
                ]
                str " "
                str todo.Description
                button [
                    onClick (fun _ -> remove(todo))
                    str "x"
                ]
            ]
    ]

let view =
        style styleSheet <| div [
            className "board"
            input [
                className "new-todo"
                Attribute ("placeholder","what needs to be done?")
                onKeyDown (fun e ->
                    let key = (e :?> Browser.Types.KeyboardEvent).key
                    if key = "Enter" then add( (e.currentTarget :?> Browser.Types.HTMLInputElement).value )
                )
            ]

            todosList "left" "todo" (fun t -> not t.Done) |> bind todos
            todosList "right" "done" (fun t -> t.Done) |> bind todos
        ]
