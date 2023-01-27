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
