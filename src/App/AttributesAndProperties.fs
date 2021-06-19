module AttributesAndProperties

open type Feliz.length
open Sutil
open Sutil.Attr
open Sutil.DOM
open Sutil.Styling


let view() =
    let bindToAttribute = Store.make "Some Value"
    let bindToProperty = Store.make false

    Html.div [
        Html.p [
            text """You can bind properties and attributes by using the `Bind` API. The Bind API provides with helpers to bind either attributes or properties.
            Sometimes developers may wrongly believe that Attributes and Properties are the same but HTML Elements have both
            and they don't necesarily match each other. Attributes are commonly refered in an HTML as 
            """
            Html.pre "<my-element this-is-an-attribute=\"and this is the value\"></my-element>"
            text "where as properties are attached to an HTML element instance like"
            Html.pre "documment.querySelector(\"#my-element\").myProperty"
            text "and these can be read or assigned to see how this example works you should open the browser developer tools and inspect this text"
            text "Example: "
            Html.br []
            Html.br []
            Bind.attr("data-my-attr", bindToAttribute)
            bindFragment bindToAttribute <| fun value ->
                text value
        ]

        Html.input [
            type' "checkbox"
            Bind.prop(
                "checked",
                bindToProperty,
                fun chkd -> 
                    if chkd then 
                        bindToAttribute <~ "I'm checked, inspect my attributes"
                    else 
                        bindToAttribute <~ "I'm not checked, inspect my attributes"
            )
        ]

        Html.p [
            text "In this case, we're binding the \"data-my-attr\" to the \"p\" tag above to a string store which in turn is "
            text "modified by the checkbox, we're also, binding the checkbox's checked property and adding a handler "
            text "That will update the string store each time it changes."
            Html.br []
            text "While there are many custom elements and HTML elements that might match the attribute name "
            text "to an element property not it is not always the case, so be sure to read your library's documentation."
        ]
    ]