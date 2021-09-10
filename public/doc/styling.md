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

```fsharp
open Feliz
open Sutil
open Sutil.Styling
open Sutil.DOM

let css =
    // use the rule function to specify a set of css properties
    // just like you would in normal css files
    [ rule "p" [
            // give it a list of css properties
            Css.color "orange"
            Css.fontFamily "'Comic Sans MS', cursive"
            Css.fontSize (length.em 2.0)
        ]
    ]

let neitherThese() =
    Html.p "I'm different"
    /// to add styles to a particular HTML Node
    /// use the `withStyle` function which takes a list of rules
    |> withStyle [ rule "p" [Css.color "rebeccapurple"] ]

Html.article [
    Html.div [
        Html.p [
            text "These styles..."
        ]
        neitherThese()
    ]
    Html.p "don't affect these"
    |> withStyle []
]
|> withStyle css
```

Even if the whole article HTML node has the following css rule
```css
p {
    color: orange;
    font-family: 'Comic Sans MS', cursive;
    font-size: 2em;
}
```
Only the `p` nodes that don't have another particular style associated with them are affected. Each call to `withStyles` creates a particular scope which can help you to encapsulate styles and avoid global conflicts.

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
