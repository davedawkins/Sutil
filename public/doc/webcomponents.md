Sutil has support for creating [web components](https://developer.mozilla.org/en-US/docs/Web/Web_Components).

For example, you can define a component like this, in Sutil:

```fsharp
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
    ]//norepl
```

Register the component:

```fsharp
WebComponent.Register( "my-counter", Counter, { label = ""; value = 0} )//norepl
```

The arguments to `WebComponent.Register` are:

| Name          | Type           | Description        |
| ------------- | -------------  | ------------------ |
| name          | `string`         | Name of component. Must contain '-' |
| constructor   | `IStore<'Props> -> SutilElement` | Constructor function. Returns an Html.XXX element |
| init          | `'Props`         | Initial value for properties. |

Now you can use this component anywhere in your HTML, like this

```html
<body>
    <my-counter value='10' label='Counter: '></my-counter>
</body>
```

In this example note how we can pass initial values to the component's model. In addition, the component will react to `value` being set either as an attribute or a property. For example, if the element is selected with the browser's Inspector tool, then each of the following Javascript statements will update the counter value.

```js
$0.value = 20
$0.value = "30"
$0.setAttribute("value", "40")
```

![Example of setting attributes and properties](images/CounterGreetingComponent.gif)

Here is the full example which you can run in the REPL.
```fsharp
open Fable.Core
open Sutil
open Feliz
open Sutil.Attr
open Sutil.DOM
open Sutil.WebComponents

type CounterProps = {
    value : int
    label : string
}

let CounterStyles = [
    rule "div" [
        Css.backgroundColor "#DEEEFF"
        Css.width (length.percent 50)
        Css.padding (length.rem 0.5)
    ]
    rule "button" [
        Css.padding (length.rem 0.25)
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
    DOM.html """
        <my-counter value='10' label='Counter: '></my-counter>
        <br>
        <my-greeting></my-greeting>
    """

view() |> Program.mountElement "sutil-app"
```

## Shadow Styles

Use `adoptStyleSheet` to assign a stylesheet to the root element of a web component:

```fsharp
let styles = [
    rule "div" [
        Css.backgroundColor "#DEEEFF"
        Css.width (length.percent 50)
        Css.padding (length.rem 0.5)
    ]
    rule "button" [
        Css.padding (Feliz.length.rem 0.25)
    ]
]

let Counter (model : IStore<CounterProps>) =
    Html.div [
        adoptStyleSheet styles
        // ...
    ]//norepl
```

## Constructable Stylesheets

At the time of writing [Constructable Style Sheets](https://github.com/WICG/construct-stylesheets/blob/gh-pages/explainer.md) is not fully implemented across browsers other than Google Chrome, so Sutil uses a [polyfill](https://www.npmjs.com/package/construct-style-sheets-polyfill). Sutil injects the polyfill automatically at run-time

```html
    <script src='https://unpkg.com/construct-style-sheets-polyfill'></script>
```

## Event Handling

(coming soon)
