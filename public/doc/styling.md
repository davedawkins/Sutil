Styling in Sutil is as you'd expect with a plain HTML document. You can:

- Use a `style` attribute
- Use a stylesheet (and specify `class` attributes)

In addition, Sutil borrows features from Svelte, where you can give an element its own private stylesheet.

## Style Atttribute

This uses the `Feliz.Engine` `Attr.style` attribute to specify the style in text form:

```fsharp
Html.div [
    Attr.style "color:red; text-transform: capitalize;"
    Html.text "hello world"
]
```

Sutil supplies another `style` function, that allows you to pass `Css` values:

```fsharp
Html.div [
    style [
        Css.color "red"
        Css.textTransformCapitalize
    ]
    Html.text "hello world"
]
```

Using the `Css` values means that you get code-completion and IntelliSense, and the compiler won't let you specify unknown (or misspelled) attributes.

## CSS Stylesheets

You can of course just include a CSS stylesheet or framework into your `index.html`. For example, if you use bulma:

```html
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css">
```

You can then use the `class` attribute to style your elements:

```fsharp
Html.div [
    class' "has-text-danger is-capitalized"
    Html.text "hello world"
]
```
## Component Style Sheets

### withStyle

Sutil also allows you to create stylesheets, which can then be applied to your component.

- Create a list of Css `rule`s
- Apply to your element using `withStyle`

```fsharp
let css = [
    rule "div" [
        Css.color "red"
        Css.textTransformCapitalize
    ]
]

Html.div [
    Html.text "hello world"
] |> withStyle css
```

An example where we use class selectors:

```fsharp
let css =
    [
        rule ".red-caps" [
            Css.color "red"
            Css.textTransformCapitalize
        ]
        rule ".blue-yelling" [
            Css.color "blue"
            Css.textTransformUppercase
        ]
    ]

Html.div [
    Html.p [
        class' "red-caps"
        text "this sentence should be capitalized in red"
    ]
    Html.p [
        class' "blue-yelling"
        text "this sentence should be shouting in blue"
    ]
] |> withStyle css
```

An important feature of inline stylesheets applied using `withStyle` is that they can be either be merged with parent stylesheets or completely insulated.

In this example, note how the subcomponent's `<p>` element isn't affected by the main component's styling:

```fsharp
module SubComponent =
    let subCss = [
        rule "div" [
            Css.borderColor "gray"
            Css.borderRadius (length.px 5)
            Css.borderWidth (length.px 1)
            Css.borderStyleSolid
            Css.padding (length.px 4)
        ]
    ]

    let view() =
        Html.div [
            Html.p "I'm a subcomponent"
        ] |> withStyle subCss

let css =
    [
        rule "p" [
            Css.textAlignRight
            Css.color "orange"
        ]
    ]

Html.div [
    Html.p [
        text "I'm the main component"
    ]
    SubComponent.view()
] |> withStyle css
```

### withStyleAppend

Compare this with the use of `withStyleAppend` in the same example. In this case, the effect is to append the subcomponent's stylesheet into any parent stylesheet.

```fsharp
module SubComponent =
    let subCss = [
        rule "div" [
            Css.borderColor "gray"
            Css.borderRadius (length.px 5)
            Css.borderWidth (length.px 1)
            Css.borderStyleSolid
            Css.padding (length.px 4)
        ]
    ]

    let view() =
        Html.div [
            Html.p "I'm a subcomponent"
        ] |> withStyleAppend subCss

let css =
    [
        rule "p" [
            Css.textAlignRight
            Css.color "orange"
        ]
    ]

Html.div [
    Html.p [
        text "I'm the main component"
    ]
    SubComponent.view()
] |> withStyle css
```

## Advanced Styling

Styles don't have to be static collections; they can be calculated according to application state.

```fsharp
let myArticleStyles =
    seq {
        yield! globalRules
        match article.state with
        | Published -> yield! publishedStyles
        | OnRevision -> yield! onRevisionStyles
        | Draft -> yield! [ rule "article" [ Css.color "rebeccapurple" ] ]

    } |> Seq.toList

Html.article [ (* ... content ... *) ] |> withStyle myArticleStyles//norepl
```
