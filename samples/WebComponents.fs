module WebComponents

open Sutil
open Sutil.Attr
open Sutil.DOM
open Browser.Types
open Sutil.Styling

type CounterProps = {
    value : int
    label : string
}

let CounterStyles = [
    rule "div" [
        Css.backgroundColor "#DEEEFF"
        Css.width (Feliz.length.percent 50)
        Css.padding (Feliz.length.rem 0.5)
    ]
    rule "button" [
        Css.padding (Feliz.length.rem 0.25)
    ]
]

let Counter (model : IStore<CounterProps>) =
    Html.div [
        adoptStyleSheet CounterStyles

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

WebComponent.Register("my-counter",Counter,{ label = ""; value = 0})

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

WebComponent.Register("my-greeting",Greeting,{ greeting = "Bonjour"; subject = "Marie-France"})


let view() =
    // Example of a 3rd party app, that doesn't really know anything about Sutil.
    // The JS event handlers are looking up the 'greet1' component and sending the
    // current input value into the appropriate property. You can use setAttribute too.
    Html.div [
        // Consider this to be <script src='app.js'></script> for the 3rd party app
        // Can't pass <script> tags to DOM.html
        Html.script [
            text """
                function greetingUpdated(e) {
                    var greet = document.getElementById('greet1')
                    greet.greeting = e.target.value;
                }
                function subjectUpdated(e) {
                    var greet = document.getElementById('greet1')
                    greet.subject = e.target.value;
                }
                function resetCounters(e) {
                    var counters = document.querySelectorAll("my-counter");
                    for (var c of counters) { c.value = 0; }
                }
            """
        ]

        // Consider this to be <body> for the 3rd party app
        Html.div [
            DOM.html """
                <my-counter value='10' label='Counter: '></my-counter>
                <br>
                <my-counter value='100' label='Counter #2: '></my-counter>
                <br>
                <button onclick="resetCounters(event)" >External Reset</button>
                <hr>
                <my-greeting></my-greeting>
                <hr>

                <my-greeting id='greet1' greeting="こんにちは" subject="アルフォンソ"></my-greeting>

                <div style='margin-top: 8px;'>
                    <span>Greeting: <input type='text' value='こんにちは' oninput='greetingUpdated(event)'></span>
                    <span>Subject:  <input type='text' value='アルフォンソ' oninput='subjectUpdated(event)'></span>
                </div>
            """
        ]
    ]

view() |> Program.mountElement "sutil-app"
