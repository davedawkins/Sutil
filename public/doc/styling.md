# Styling Sutil Elements

[live on the repl]: https://sutil.dev/repl/#?names=NoIgggDhB0BmDOIC6Q&codes=NoIgtg9gJgrgNgUwAQAkFzhA6hATnKAHQDsB6UpLXASwBdkBjaZPJTAQyiXaQGd2wAB0RIAZrghg+1KAgBG7XCRIRBCYkgDKMWtTgq1G7brgA6ACIB5ALIH1WnXtMBBWrSXESiWt0GCAFACUSAC8SCRIkai0YGaKugwiwBFRqSgxZgAWAIzhIGgYEEgA7ngEAISEICmpkemxpnI6tBAawEiJ7Ly8AOR5vDAMDAjdVQDc0Q30AB4+VQCSPVI8TW6tlSAAujW19WarLW0dcF29ecWKxNTEAObjk2YzcyCLy0gH61XbGrV1GY3NVpIdqdbp9KoIXASJQgCZ7UxPPKvbjvQHEDbfVLfEgABQkN1wAlMkBgxFoAFFEGB1M8BiYALTsPxVJD+JkBQKBLZAA&html=DwQgIg8gwgKgmgBQKIAIAWAXAtgGwHwBQwmuKOAhgHYDmAvAEQCml9hxj5AJoSrysFkYZyKAMZpyAJwDOQhgFUYAMQC0ADlYE+-QcPQYMABxWMAjgFcAlgDcGADRXyAgiqgB7LIfIZLAIxyM9GJulBjMGAwAkki0jJzUgTx8AkIilOSCDNaWjADuhm6SGEGiIWGhDLmWnBhotJyM2aKMKlU1aAA0KJaUlj7kOCrSogOMtACMAHQADJrawDg9ANYokow4DNIYAJ4B0miMQkFoawBmDJhG0gBcAPS3opyUkwBW0g2L1pKTlEK3lIYsLdDJZRL1RHNkj4MAE8ABlcw+HDAW7Q2FEW4HLhsXxuTjbJK8YCcGzdTibRGWQbkQyGVgoknWNi3XH45kkfBAA&css=Q

[Zanaptak.TypedCssClasses]: https://github.com/zanaptak/TypedCssClasses

Styling Elements in Sutil is not complicated fortunately, let's take the following example:

> Check this sample [live on the repl]!


```fsharp
open Sutil
open Sutil.DOM
open Sutil.Attr

Html.article [
    Html.h1 "Hello world!"
    Html.button [ class' "success"; Html.text "I'm a button!"]
    Html.button [ class' "warning"; Html.text "I'm a button!"]
    Html.button [ class' "error"; Html.text "I'm a button!"]
]
```

this will allow you to use the classic css class based styles, you can define you stylesheets with `css`/`sass`/`less` and similar tools.

This styling _style_ can be complemented with tools like [Zanaptak.TypedCssClasses] which generate type information about your css files giving you typed access to CSS classes in a type safe manner.


## CSS in JS... I mean CSS in F#

If you would like to define your styles with code because of style scoping like scoped css in `Vue` or `Svelte` then Sutil has you covered.


> Check this [repl live](https://sutil.dev/repl/#?names=NoIgggDhB0BmDOIC6Q&codes=NoIgtg9gJgrgNgUwAQDkEGcAuCoGEJgAOEAdgiZugDok0SHlIBiCcAlgF50MlIDKMTGzjdGAoXAB0fTAE92JAOaje44ZIAiAeQCyNGokxIAxunRIAvDSQ2kwJACd4yKiEKu7129++4zk4wg4CAckVxCAQyUEVy8fHz90SQAzUkwmCLBhWTCQAHJ8LON+KPMdPjyAGhMYB3Q2ADcYkDj420SUtL5OZAAKRCVMAAtJBDAkACZJAAYASlbvAF1W5doSQyQyNmGEBwAVIYwEXtnLMN5bAAlMMClCXIBJPPGoNmTk3fJMWIubAB8AHxIADu2yGMnkyHsTkQuXcIDsHUCwVCrgcCAARghjMYIoRaoREK5FkhVgYEEYGmwEMCTpZWtdbpIIg4hMZYcAFlcblJXg1PL82jZGXcBUKhdgAB5GVwHI5ILCQpKSSQ-cW2VbqmxbHb7Q7oY7zQVLLnCnmSe6uKCkPJGCLvbFGHYGtVtQEgsEQjmajWtd2g4Ze5CmaiC-QkKk0unugAKDggigcmUkkBgFAAoogwF9cuhBMIALR4+E0ECLIA&html=DwQgIg8gwgKgmgBQKIAIAWAXAtgGwHwBQwmuKOAhgHYDmAvAEQCml9hxj5AJoSrysFkYZyKAMZpyAJwDOQhgFUYAMQC0ADlYE+-QcPQYMABxWMAjgFcAlgDcGADRXyAgiqgB7LIfIZLAIxyM9GJulBjMGAwAkki0jJzUgTx8AkIilOSCDNaWjADuhm6SGEGiIWGhDLmWnBhotJyM2aKMKlU1aAA0KJaUlj7kOCrSogOMtACMAHQADJrawDg9ANYokow4DIZrOG5cQWhrAGYMmEbSAFwA9JeinJSTAFbSDYvWkpOUQpeUhliXvuYcFhyAABaaTACck3GN2k0n+gOBkywPUmojhQXI0gY0gwAE8AnNkotKCttjj8QFpGhGEJ9kcTgZDBdrrd7k8XjZ3p8MN9fgigaDwVCYej4QDBcjUWKibxgD4MAE8ABlcw+HDAS4KpVaZK4gmMJLaXi+NycPEoADeuuNfGBkmoPXOKHGACZDAAPADcNuNAF9fXLLvqdZqaVw2KbzUb+JwbN1ODi1ZZBuRDIZWJq49Y2P8zXjcyR8EA&css=Q)

```fsharp
// For this style of styling both
// Feliz and Sutil.Styling modules are required
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
Only the `p` nodes that don't have another particular style associated with it are affected. each call to `withStyles` creates a particular scope which can help you to encapsulate styles and avoid global conflicts.

This also means that you can use `seq` expressions and `yield!` to append different styles for example:

```fsharp
let myArticleStyles = 
    seq {
        yield! globalRules
        match article.state with 
        | Published -> yield! publishedStyles
        | OnRevision -> yield! onRevisionStyles
        | Draft -> yield! [ rule "article" [ Css.color "rebeccapurple" ] ]
        
    } |> Seq.toList

Html.article [ (* ... content ... *) ] |> withStyle myArticleStyles
```

that's perhaps not the best example, but I'll leave the creative minds do the actual work and just let you know that anything might be possible :)
