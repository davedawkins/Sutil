### Modules

Sutil 2.0 refactors the Sutil.DOM module into

- Sutil.Core
- Sutil.CoreElements
- Sutil.DomHelpers

Most (if not all) existing Sutil code will work with the following edits:

- Fix `open`s

```fs
//norepl
//open Sutil.DOM    // Removed in Sutil 2.x
//open Sutil.Attr   // Removed in Sutil 2.x
open Sutil.Core
open Sutil.CoreElements
open Sutil.DomHelpers
```

- Fix qualified function calls

For example:

- `DOM.unmount`
- `DOM.disposeOnUnmount`
- etc

With the given `open` block shown above, you can remove the `DOM` prefixes


### Navigable

`Navigable` is now a class, so `Navigable.listenLocation a b` should now be `Navigable.listenLocation(a,b)`

### Media

`MediaQuery` is now `Media`, so `MediaQuery.listenMedia a b` should now be `Media.listenMedia(a,b)`

`Media.MinWidth` is now `CssMedia.minWidth `
`Media.MiaxWidth` is now `CssMedia.miaxWidth`

### Custom Events

`Interop.dispatch` and `Interop.dispatchSimple` are accessible via `CustomDispatch<'T>`.

Instead of `dispatchSimple target "my-event"`, use:

```fs
//norepl
CustomDispatch<_>.dispatch( target, "my-event" )
```

Instead of `dispatch target "my-event" data`, use:

```fs
//norepl
// Sutil 2.0.0
CustomDispatch<_>.dispatch( target, "my-event", [ Detail (Some data) ] )

// Sutil 2.0.1 and higher
CustomDispatch<_>.dispatch( target, "my-event", [ Detail data ] )

// or just:
CustomDispatch<_>.dispatch( target, "my-event", data )
```

### Program.mount

Instead of:

```fs
//norepl
let app() = Html.div [
    // ...
]

app() |> Program.mountElement "sutil-app"
```

use:

```fs
//norepl
app() |> Program.mount
```

There are overloads in [Program](https://sutil.dev/apidocs/reference/sutil-program.html) that can accommodate more unusual mount scenarios.
