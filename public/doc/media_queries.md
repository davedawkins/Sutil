Sutil provides helpers for

- The CSS `@media` directive

- The `Window.matchMedia` browser function

## CSS @media

There's nothing to stop you using a regular `.css` file in your application.

However, if you're using Sutil's `withStyle` function then you can use the helpers from `Sutil.CssMedia`.

For example:

```
open Sutil
open Sutil.Styling
open type Feliz.length

let style = [

    CssMedia.maxWidth(px 400, [
        rule ".container" [
            Css.width (px 300)
        ]
    ])

    rule ".container" [
        Css.width (px 600)
        Css.backgroundColor "gray"
    ]
]

let view =
    Html.div [
        Attr.className "container"
    ]

```

Currently `Sutil.CssMedia` contains the following helpers:

```
    static member custom (condition : string, rules)
    static member minWidth (minWidth : Styles.ICssUnit, rules : StyleSheetDefinition list)
    static member maxWidth (maxWidth : Styles.ICssUnit, rules : StyleSheetDefinition list)
```

## Window.matchMedia()

Use `Sutil.Media.listenMedia` in order to be notified when a given media configuration is in effect.

For example

```
    let view() =
        Html.div [
            disposeOnUnmount [
                Media.listenMedia( "(max-width: 768px)", dispatch << SetIsMobile)
            ]
            // ...
        ]
```

In this example, we will dispatch the Elmish message `SetIsMobile` when the browser window size is 768px or less.



