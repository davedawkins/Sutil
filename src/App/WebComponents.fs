module WebComponents

open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.WebComponents

type CounterProps = {
    value : int
    label : string
}

let Counter (model : IStore<CounterProps>) =
    Html.div [
        Bind.el(model |> Store.map (fun m -> m.label),Html.span)
        Bind.el(model |> Store.map (fun m -> m.value),Html.text)

        Html.div [
            Html.button [
                text "+"
                onClick (fun _ -> model |> Store.modify (fun m -> { m with value = m.value + 1 } )) []
            ]
            Html.button [
                text "-"
                onClick (fun _ -> model |> Store.modify (fun m -> { m with value = m.value - 1 } )) []
            ]
        ]
    ]

registerWebComponent "my-counter" Counter { label = ""; value = 0}

type GreetingProps = {
    greeting : string
    subject : string
}

let Greeting (model : IStore<GreetingProps>) =
    Html.div [
        Bind.el(model |> Store.map (fun m -> m.greeting),Html.span)
        text " "
        Bind.el(model |> Store.map (fun m -> m.subject),Html.text)
    ]

registerWebComponent "my-greeting" Greeting { greeting = "Bonjour"; subject = "Marie-France"}

let view() =
    DOM.html """
        <my-counter value='10' label='Counter: '></my-counter>
        <br>
        <my-greeting></my-greeting>
    """
