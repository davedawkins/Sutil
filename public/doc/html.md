Sutil comes batteries-included with `Feliz.Engine` as a DOM builder.

### HTML Elements

All HTML elements are member functions of an `HTML` object. The general format is

```fsharp
Html.<elementName> [ <child-element> | <attribute> ]//norepl
```

For example:

```fsharp
open Sutil

let app() =
    Html.div [
        Html.h1 [
            Html.text "Introduction"
        ]
        Html.p [
            Html.text "Welcome to "
            Html.a [
                Attr.href "https://sutil.dev"
                Html.text "Sutil"
            ]
        ]
    ]

app() |> Program.mountElement "sutil-app"
```

This would produce the following HTML:
```html
<div>
    <h1>Introduction</h1>
    <p>Welcome to <a href="https://sutil.dev">Sutil</p>
</div>
```

### Text Nodes

Use `Html.text` to create a text node:

```fsharp
Html.p [
    Html.text "This is paragraph text"
]
```

Most elements have an overload that allows them to take a single string value as an argument. So, the following is
an shortcut for the above example:

```fsharp
Html.p "This is paragraph text"
```

### Attributes

All HTML attributes are member functions of an `Attr` object. For example:

```fsharp
Html.a [
    Attr.href "https://sutil.dev"
    Html.text "Sutil"
]
```

```fsharp
Html.input [
    Attr.type' "email"
    Attr.value "bob@foo.com"
]
```

### Attributes `class` and `id`

The HTML attribute `class` is spelled `Attr.className` in Sutil, to avoid confusion with the keyword.

There are also convenience functions named `class'` and `id'` (`id` isn't a keyword in F#, but is a commonly-used core function).

For example:
```fsharp
Html.div [
    Attr.className "container is-active"
    Attr.id "main"
]
```

```fsharp
Html.div [
    class' "container is-active"
    id' "main"
]
```

will both produce

```html
<div class="container is-active" id="main"></div>
```

### Fragments

Fragments look like elements, but their children and attributes belong to the fragment's parent. For example:

```fsharp
Html.div [
    Html.p "First line"
    fragment [
        Html.p "Fragment"
    ]
    Html.p "Last line"
]
```

is equivalent to

```fsharp
Html.div [
    Html.p "First line"
    Html.p "Fragment"
    Html.p "Last line"
]
```

Fragments are useful for defining functions that return a collection of elements, attributes and events that you want to be inserted without having to create a wrapper element (such as a `div`).

For example, you may have a function `okButton()`, and use it like this:

```fsharp
let okButton() =
    Html.button [ text "OK" ]

Html.div [
    Html.text "Press OK to continue "
    okButton()
]
```

This works just fine, but perhaps you want to define a list of buttons, like this:

```fsharp
let buttons() =
    [
        Html.button [ text "Cancel" ]
        Html.button [ text "OK" ]
    ]//norepl
```

Your initial attempt to use this function will cause a compilation error:
```fsharp
Html.div [
    Html.text "Press OK to continue, or Cancel"
    buttons() // Won't compile
] //norepl
```

You can fix this by using a fragment:
```fsharp
let buttons() =
    fragment [
        Html.button [ text "Cancel" ]
        Html.button [ text "OK" ]
    ]

Html.div [
    Html.text "Press OK to continue, or Cancel"
    buttons() // Compiles!
]
```

The resulting HTML will be (with some additional whitespace for readability):
```html
    <div>
        Press OK to continue, or Cancel
        <button>Cancel</button>
        <button>OK</button>
    </div>
```
