Sutil can host components from other frameworks such as React, using the `host` function:

|---|---|
| Module |  `Sutil.DOM` |
| Function | `host render` |
| Arguments | `render : HTMLElement -> unit` |

For example:

```fsharp

let app() = 
    Html.div [
        Html.text "Hello from Sutil"
        Html.div [
            host (fun el -> ReactDOM.render (ReactExample.Counter()))
        ]
    ]
//norepl
```

See [sutil-template-react](https://github.com/davedawkins/sutil-template-react) for a real example that hosts the awesome Fable.ReactFlow demo:

![reactflow](https://user-images.githubusercontent.com/285421/136655641-febf9fe3-f9bf-4c76-84ef-76e872a0d9f1.gif)
