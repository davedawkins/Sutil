module Fragment

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Transition

type Model = {
    Greeting : string
    Color : string
    TickCount : int
    UnsubscribeCount : int
}

type Message =
    | SetGreeting of string
    | SetColor of string
    | Tick
    | Unsubscribe

let init() = {
    Greeting = "Hello World"
    Color = "black"
    TickCount = 0
    UnsubscribeCount = 0
}

let update msg model =
    match msg with
    | SetGreeting s -> { model with Greeting = s }
    | SetColor c -> { model with Color = c }
    | Tick -> { model with TickCount = model.TickCount + 1 }
    | Unsubscribe -> { model with UnsubscribeCount = model.UnsubscribeCount + 1 }

let viewFragments() =

    let model, dispatch = () |> Store.makeElmishSimple init update ignore

    let start() = DOM.interval (fun _ -> dispatch Tick) 1000

    Html.div [
        subscribeOnMount start

        Html.h4 "Fragments, nested fragments"

        Html.div "1 Header div - always first"

        fragment [
            Html.div "2 fragment [ div ]"
        ]

        fragment [
            Html.div "3.1 fragment [ div [ fragment [ div; div; div ] ] ]"
            fragment [
                Html.div "3.2 header"
                Html.div "3.3 body"
                Html.div "3.4 footer"
            ]
        ]

        Html.h4 [ Attr.style [ Css.marginTop 12 ]; text "Bindings" ]

        bindFragment model <| fun m -> Html.div $"4 bind div: tick = {m.TickCount}"

        bindFragment model <| fun m ->
            fragment [
                unsubscribeOnUnmount [ (fun _ -> dispatch Unsubscribe) ]
                Html.div $"5 bind fragment [ div: tick = {m.TickCount} ]"
            ]

        fragment [
            Html.div "6.1 fragment [ div; bind div; div ]"
            bindFragment model <| fun m -> Html.div $"6.2 tick = {m.TickCount}"
            Html.div "6.3 end"
        ]

        bindFragment model <| fun m -> Html.div $"Unsubscribed: {m.UnsubscribeCount}"
        Html.div "7 Footer div - always last"
    ]


let viewFragments_2() =

    let model, dispatch = () |> Store.makeElmishSimple init update ignore

    //let start() = DOM.interval (fun _ -> dispatch Tick) 1000

    Html.div [
        //subscribeOnMount start
        //Html.h4 [ Attr.style [ Css.marginTop 12 ]; text "Bindings" ]
        //bindFragment model <| fun m -> Html.div $"4 bind div: tick = {m.TickCount}"

        bindFragment model <| fun m ->
            fragment [
                Html.div $"TickCount = {m.TickCount}"
            ]

        Html.button [
            text "Tick"
            onClick (fun e ->
                dispatch Tick
            ) [ PreventDefault ]
        ]

        //fragment [
        //    Html.div "6.1 fragment [ div; bind div; div ]"
        //    bindFragment model <| fun m -> Html.div $"6.2 tick = {m.TickCount}"
        //    Html.div "6.3 end"
        //]

        //bindFragment model <| fun m -> Html.div $"Unsubscribed: {m.UnsubscribeCount}"
        //Html.div "7 Footer div - always last"
    ]

let view_attempt_1() =
    let store = [ "one (i)" (*; "two (i)" *)] |> Store.make

    Html.div [
        each store (fun item ->
            Html.div item
        ) []
        Html.button [
            text "Add item (i)"
            onClick (fun e ->
                store.Update (fun items -> "new (i)" + string items.Length :: items ) //id
            ) [ PreventDefault ]
        ]
    ]


let view_attempt_2() =
    let store = ([ "one (ii)"] : string list) |> Store.make

    fragment [
        each store (fun item ->
            Html.div item
        ) []
        Html.button [
            text "Add item (ii)"
            onClick (fun e ->
                store.Update (fun items -> "new (ii)" + string items.Length :: items ) //id
            ) [ PreventDefault ]
        ]
    ]

let view_attempt_3() =
    let store = [ "one (iii)" ] |> Store.make

    fragment [
        Html.div "Header item (iii)"
        each store (fun item ->
            Html.div item
        ) []
        Html.button [
            text "Add item (iii)"
            onClick (fun e ->
                store.Update (fun items -> "new (iii)" + string items.Length :: items ) //id
            ) [ PreventDefault ]
        ]
    ]

let view_workaround_4() =
    let store = [ "one (iv)" ] |> Store.make

    fragment [
        Html.div [
            eachi store (fun (i, item) ->
                Html.div item
            ) []
        ]
        Html.button [
            text "Add item (iv)"
            onClick (fun e ->
                store.Update (fun items -> "new (iv)" + string items.Length :: items )
            ) [ PreventDefault ]
        ]
    ]

Interop.set Browser.Dom.window "pp" (fun x -> (DomNode x).PrettyPrint("pp"))

let leftRight() =
    let show = Store.make true
    Html.div [
        class' "columns"
        Html.div [
            class' "column"; text "Left"
        ] |> Sutil.Transition.transition []  show
        fragment [
            Html.div [
                class' "column"; text "Right"
            ]
        ]
    ]

let fragmentList() =
    Html.div [
        fragment [
            Html.h5 "Section 1"
            Html.div "Item 1.a"
        ]
        fragment [
            Html.h5 "Section 2"
            Html.div "Item 2.a"
        ]
    ]

let view() =
    Html.div [
        fragment [
            Html.h5 "Section 1"
            Html.div "Item 1.a"
        ]
        fragment [
            Html.h5 "Section 2"
            Html.div "Item 2.a"
        ]
        leftRight()
        view_attempt_1()
        Html.hr []
        view_attempt_2()
        Html.hr []
        view_attempt_3()
        Html.hr []
        view_workaround_4()
        Html.hr []
        viewFragments()
    ]