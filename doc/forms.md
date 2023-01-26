# WIP: Forms


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

This would be near equivalent to the following form in html/js:

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
