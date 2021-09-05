# Sutil Syntaxis

[Feliz.Engine]: https://github.com/alfonsogarciacaro/Feliz.Engine
[Feliz DSL]: https://github.com/Zaid-Ajaj/Feliz
[@zaid_ajaj]: https://twitter.com/zaid_ajaj

The `type safe` <abbr title="Domain Specific Language">DSL</abbr> from Sutil comes from [Feliz.Engine] which in turn comes from the original [Feliz DSL] made by [@zaid_ajaj].

Sutil provides you with some helpers to make your code feel more natural rather than following the original DSL which was modeled after React JSX API.


## HTML Elements

each HTML element is enclosed in the `Html` value which is an instance of `SutilHtmlEngine`.

An HTML tag is usually declared like this

```html
<div data-custom-attribute="value" class="my-class">
    child text
    <button onclick="console.log('event handler')">child elements</button>
</div>
```
Although, inline javascript is heavily not recommended you'll see that it's one way to handle events in HTML.

In Sutil you need to call a method on the `Html` instance and pick your *tag*, attributes and children are declared inside the *tag*'s "definition"

```fsharp
Html.div [
    // attributes that are not part of the spec can be specified as well
    Attr.custom("data-custom-attribute", "value")
    // classes can be declared as usual
    class' "my-class"
    // text will create a text node rather than a span or another tag
    text "child-text"
    // you can nest other Html elements as well
    Html.button [
        // event handlers are bound via listeners not inline javascript
        on "click" (fun _ -> JS.console.log("event handler")) []
        // or onClick (fun _ -> JS.console.log("event handler")) []\

        text "child elements"
    ]
]
```


What is happening here is that the div and button take an `F# List` as a parameter, in sutil each of the nodes is a SutilElement, the engine takes care of determining **what** should go **where** regarding the attributes, events and children.