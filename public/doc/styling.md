## Attr.style

Use the `Attr.style` attribute to specify an inline element style.

With `Css` values:

```fsharp
Html.div [
    Attr.style [
        Css.color "red"
        Css.textTransformUppercase
    ]
    Html.text "hello world"
]
```

With a standard CSS style string:

```fsharp
Html.div [
    Attr.style "color:red; text-transform: uppercase;"
    Html.text "hello world"
]
```

If you specify more than one `Attr.style` attribute, then the last will overwrite all previous values.

One nice advantage of the `Css` values is that upon typing `Css.`, an autocompleting menu will appear to help
you find the right CSS property and value. This way, you can't specify unknown (or misspelled) properties and values.

Occasionally, the menu won't contain the exact item you need. In these cases, your fallback is to use `Css.custom`

```fs
//norepl
Css.custom( "container", "inline-size" )
```

Many CSS properties (such as `margin`, `padding`, `border`) have overloads:

```fs
//norepl
Css.margin (px 12)
Css.margin (px 0, auto)
Css.margin (px 4, px 8, px 12, rem 1)
```

Note that length units are specified as function calls, so that they read backwards: `px 10` instead of `10px` as it would be in a `.css` file.

The length `px` functions can be made globally available with:

```fs
//norepl
open type Feliz.length
```

Borders can be set as follows:

```fs
//norepl
Css.border (px 1, borderStyle.solid, "red")
```

Again, use `open type Feliz.borderStyle` if you'd rather write:

```fs
//norepl
Css.border (px 1, solid, "red")
```

## CSS Stylesheets

You can of course just include a CSS stylesheet or framework into your `index.html`. For example, if you use [bulma](https://bulma.io/):

```html
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css">
```

You can then use the `class` attribute to style your elements:

```fsharp
Html.div [
    Attr.className "has-text-danger is-capitalized"
    Html.text "hello world"
]
```

We can also use the `Html.divc` helper to specify the `class` attribute:

```fsharp
Html.divc "has-text-danger is-capitalized" [
    Html.text "hello world"
]
```

## Programmatic Style Sheet Links

You can also use `setHeadStylesheet doc url` to programmatically create a `<link rel="stylesheet" href={url}>` element in the given document's `<head>` section.

For example:

```fs
//norepl
    setHeadStyleSheet (Browser.Dom.document) "css/main.css"
```

You can also use `headStyleSheet` as a child element of your top level view, so that the current document is used. This might be important for web components, for example:

```fs
//norepl
    Html.div [
        headStyleSheet "css/main.css"
    ]
```

Note that the stylesheet isn't really a child of the main `div` element; we're taking advantage of the context supplied to the div element when it is constructed.

TODO: Design a less strange way of defining document level operations


## Programmatic Style Sheets

We can also create a stylesheet programmatically. For example:

```fs
//norepl
let myStyle = [
    rule ".red" [
        Css.color "red"
    ]
    rule ".green" [
        Css.color "green"
    ]
    rule ".blue" [
        Css.color "blue"
    ]
]
```

## Programmatic Style Sheets: Applied Globally

To apply globally, use `Sutil.Styling.applyStyleSheet`:

```fs

let myStyle = [
    rule ".red" [
        Css.color "red"
    ]
    rule ".green" [
        Css.color "green"
    ]
    rule ".blue" [
        Css.color "blue"
    ]
]

let removeSheet = myStyle |> addStyleSheet (Browser.Dom.document) ""

Html.div [
    Html.divc "blue" [ text "As in sky" ]
]
```

As you might expect, *any* element in our application with class "red", "green" or "blue" will now be affected this style sheet.

The return value `removeSheet` is a function that we can call to remove the style sheet. This means that we can build a different sheet
if we wish, and replace the previous.

For example, here's a primitive theming application:

```fs
let myStyle (preferDark : bool) = [
    let makeColor color =
        sprintf "%s%s" (if preferDark then "dark" else "light") color

    rule "div" [
        Css.padding (rem 1)
        Css.color (if preferDark then "white" else "black")
    ]

    rule "button" [
        Css.marginRight (rem 1)
        Css.marginTop (rem 1)
    ]

    rule ".green" [
        Css.backgroundColor (makeColor "green")
    ]

    rule ".blue" [
        Css.backgroundColor (makeColor "blue")
    ]
]

let mutable removeSheet = ignore

let installSheet( preferDark : bool ) =
    removeSheet()
    removeSheet <- preferDark |> myStyle |> addGlobalStyleSheet (Browser.Dom.document)

installSheet false

Html.section [
    Html.divc "green" [ text "As in grass" ]
    Html.divc "blue" [ text "As in sky" ]
    Html.button [
        Attr.className "button"
        text "Dark"
        Ev.onClick (fun _ -> installSheet true)
    ]
    Html.button [
        Attr.className "button"
        text "Light"
        Ev.onClick (fun _ -> installSheet false)
    ]
]
```

## Programmatic Style Sheets: Applied Per Component

Given a programmatic stylesheet like `myStyle` in the previous examples, we can apply this to a specific element (and its children).

This has the effect of scoping the style sheet to that element only, providing support for locally-scoped components.

For example:

```fsharp
let aComponent (label : string) =
    let componentStyle = [
        rule "div" [
            Css.color "orange"
            Css.border (px 1, borderStyle.solid, "orange")
            Css.borderRadius (px 6)
            Css.padding (rem 1)
            Css.marginTop (rem 1)
        ]
    ]
    Html.div [
        text label
    ] |> withStyle componentStyle

Html.section [
    Html.div [
        Attr.className "title is-4"
        text "An Application"
    ]
    aComponent "Hello World"
]
```

Note that the top-level `div` is styled differently to the component `div`.

We can even apply styling to the top-level application, and the style sheets remain scoped to the elements they were assigned to:

```fsharp
let aComponent (label : string) =
    let componentStyle = [
        rule "div" [
            Css.color "orange"
            Css.border (px 1, borderStyle.solid, "orange")
            Css.borderRadius (px 6)
            Css.padding (rem 1)
            Css.marginTop (rem 1)
        ]
    ]
    Html.div [
        text label
    ] |> withStyle componentStyle


let mainStyle = [
    rule "div" [
        Css.color "gray"
    ]
]

Html.section [
    Html.div [
        Attr.className "title is-4"
        text "An Application"
    ]
    aComponent "Hello World"
] |> withStyle mainStyle
```


This example shows that even though `unstyledComponent` is defined in its own function, this doesn't create a component boundary for the styling applied at the top-level, so it inherits the parent's styling as if it was defined inline:

```fs
let unstyledComponent (label : string) =
    Html.div [
        text label
    ]

let mainStyle = [
    rule "div" [
        Css.color "gray"
    ]
]

Html.section [
    Html.div [
        Attr.className "title is-4"
        text "An Application"
    ]
    unstyledComponent "Hello World"
] |> withStyle mainStyle
```
