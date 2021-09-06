# WIP: Forms

[Stores]: https://
[MDN web docs]: https://developer.mozilla.org/en-US/docs/Web/HTML/Element/form

> This chapter has a heavy use of [Stores] feel free to have a read if things feel too confusing.

Forms are very likely be the bread and butter of websites they allow you to capture data from the user in a group of inputs.

A simple form can be defined in the following way

```fsharp
Html.form [
    Html.section [
        Html.label [ text "Email" ]
        Html.input [ type' "email" ]
    ]
    Html.section [
        Html.label [ text "Password"]
        Html.input [ type' "password" ]
    ]
]
```

> Please note that since we're using standard HTML you can also find information about forms in places like the [MDN web docs].

Our form is a standard HTML form  so you need to take care of the following events

- submit
    - remember to pass prevent default in the modifiers
- input events
    - you can use helpers like
        - onInput handler []
        - onKeyboardDown handler []
        - onMouseMove handler []
        - `on` "event name" handler []
    - use `Bind.attr("value", store)`

```fsharp
let formSection (email: string option) =
    // use an email if we have it otherwise start with an empty string
    let email = Store.make (defaultArg email "")
    let password = Store.make ""

    let onSubmitEvent _ =
        printfn $"""Email: {email.Value}
        Password: {password.Value}
        """

    Html.form [
        // to prevent memory leaks always dispose your stores
        disposeOnUnmount [ email; password ]
        // in events, the last list (usually empty) is for event modifiders
        // in this case PreventDefault
        on "submit" onSubmitEvent [PreventDefault]

        Html.section [
            Html.label [ text "Email" ]
            Html.input [
                type' "email"
                Attr.required true
                Bind.attr ("value", email)
            ]
        ]
        Html.section [
            Html.label [ text "Password"]
            Html.input [
                type' "password"
                Attr.required true
                Bind.attr ("value", password)
            ]
        ]
    ]
```

this would be near equivalent to the following form in html/js
```html
<form>
    <section>
        <label>Email</label>
        <input type="email" required />
    </section>
    <section>
        <label>Password</label>
        <input type="password" required />
    </section>
</form>
<script type="text/javascript">
    let email = "";
    let password = "";

    const form = document.querySelector("form");
    const emailInput = form.querySelector("[type=email]")
    const passwordInput = form.querySelector("[type=password]")

    function onSubmit(e) {
        e.preventDefault();
        console.log(`Email: ${email}\nPassword: ${password}`)
    }

    function onKeyUpEmail(e) {
        email = e.target.value;
    }

    function onKeyUpPassword(e) {
        password = e.target.value;
    }

    function onTextChange(kind) {
        return onKeyUp
    }

    form.addEventListener("submit", onSubmit)
    emailInput.addEventListener("keyup", onKeyUpEmail)
    passwordInput.addEventListener("keyup", onKeyUpPassword)
</script>
```

Sutil provides helpers to bind a particular attribute to a store, so it's simpler to react to inpit changes, which in contrast to plain javascript becomes a litle more annoying because there's no built-in binding behavior so you need to react to events in any case, also we only registered the events here, we still need to be aware of when this form and their inputs are going to dissapear from the DOM to also remove their listeners, otherwise we could have memory leaks for not removing them.

Also it's worth noting that in HTML there's no type safety so you could easily put a typo on a tag or attribute, as well as missing a `"` to close an attribute and even if the browser will compensate for it, it is very likely to produce a behavior bug.