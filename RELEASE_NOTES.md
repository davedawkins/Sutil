### 2.0.17
- Merged PR #95 from Maxime Mangel, providing a full set of boolean attributes (thank you!)

### 2.0.16
- Fix for Ev.onUnmount (issue #93) with unit test
- Fixed FSharp.Core dependency to 5.0.2
- Removed build.fsx (Fake), replaced with EasyBuild CLI - thanks to Maxime for this
- Fixes to Cells.fs and Draw.fs in App examples since 5.0.2 doesn't seem to have Map.keys / Map.values

### 2.0.15
- Added missing file "adoptedStyleSheets.js"

### 2.0.14
- Issue #91: "Attr.readOnly false" fixed
- Removed diagnostic output to console
- Observable.init: Provide the initial value for a sequence so that new subscribers will receive an immediate update of the current value. Probably only useful for IObservables that are not derived from an initial IStore
- IReadOnlyStore.Dispose(f), IStore.Dispose(f) : Call f() when the store is disposed
- Removed dependency on ConstructStyleSheetsPolyfill and included directly
- Fixed some warnings in the App (not part of the nuget pkg)

### 2.0.13
- Merged PR #84: Added Observable.map2 and Store.map2 functions (thanks to sajagi)

### 2.0.12
- Slightly more helpful error message to console when Bind throws
- Added `includeRule` for CSS style sheets. For example, `includeRule "wh100"` will look for and include CSS definitions for `rule ".wh100"` from the same style sheet

### 2.0.11
- New interface IReadOnlyStore, like an IObservable where you can query the current value
- Unprotect a few core functions and modules to enable development of new SutilElements (ContextHelpers, build, SutilEffect.MakeGroup, SutilGroup.RegisterUnsubscribe, SutilCore.ContextHelpers, SutilCore.build)
-
### 2.0.10
- Expose DomHelpers.nodeStrShort, nodeStr, DomEdit
- Added more `c` helpers (ac, hrc, tbodyc, olc)
- Fix for Store.iter

### 2.0.9
- Remove cleanup diagnostics

### 2.0.8
- Fix for broken REPL links in docs
- Added Program.mountAppend
- Additional docs for other Program.mount variants
- Additional <tag>c variants like divc

### 2.0.7
- Latest version of fable-publish-utils needed for nuget push with snupkg

### 2.0.6
- Improved packaging for SourceLink (include .snupkg etc). Template from Fable.Promise.

### 2.0.5
- Better support for SVG
- Return value from Program.mount can be disposed to perform an unmount. See docs for Program.mount

### 2.0.4
- Added Program.unmount to allow elements to be removed and cleaned up
- Only clear existing children for Program.mount( el : SutilElement ), the default case.
- Deprecate Store.write in favour of more intuitive new function Store.iter

### 2.0.3
- Rename problematic overload of Bind.attr( name, init, dispatch ) to Bind.attrInit. This should be OK, since it was introduced in 2.0 to move sc/App/Todos.fs away from Bindings.attrNotify
- Don't allow log functions to build strings when logging is disabled (reported performance issue)

### 2.0.2
- Extend Node with `asTextNode` and `asHtmlElement` (`DomHelpers`)
- Improvement to obsolescence message for `Program.mountElement`

### 2.0.1
- Move <head> helpers into DomHelpers and CoreElements
- Added Html.buttonc, Html.spanc
- Added Cmd.ofEffect
- Bind operator >/
- Navigable.listenLocation refactored
- Program.mountElement deprecated. use Program.mount overloads
- CustomDispatch.dispatch overloads. Use `CustomDispatch<_>.dispatch(el,"your-event")` instead of `dispatchSimple`
- Enable SourceLink (thanks to PierreYvesR)

### 2.0.0
- Major version bump because of refactorings which will break existing code
- Added "Upgrading to Sutil 2.x" doc
- Removed beta classification
- Refactor Sutil.DOM into Sutil.Core, Sutil.CoreHelpers, Sutill.DomHelpers
- Other refactoring around Sutil.Media, Sutil.Bindings
- Generate API documentation with fsdocs (linked from Sutil documentation tab)
- Document (minimally) every top-level module/type
- Add documentation for each topic previously marked "Coming soon"
- Added Html.divc to allow class to be specified (common usage, for me at least)
- Added Store.mapDistinct
- Renamed SutilResult to SutilEffect
- Added Name and Children to SutilElement
- Added SutilElement.Define() static members
- Unbreak WebComponents
- Hide Sutil.Logging from users
- Added ifSetElse as call to JS "$0 || $1"
- DIstinguish the various methods named "addClass", using modules
- Add "addClass", "removeClass", "setClass", "toggleClass" to Attr
- Ignore .vscode
- Remove FSharp.Core from PackageReferences
- Fix comments

### 1.0.0-beta-023
- Add global.json to prevent .NET7 for now

### 1.0.0-beta-022
- rx,ry helpers for svg
- prop, text helpers for code produced from Html2Feliz (thanks to @dejanmilicic)

### 1.0.0-beta-021
- Additional overload for Bind.promise
- Fix existing Bind.promise to be uncurried arguments
- Use fastEquals to filter out identical updates to store

### 1.0.0-beta-020
- Added `Bind.promise( p : Promise<'T>, view : 'T -> SutilElement )`
- Added extension `ToObservable<'T>()` for `Promise<'T>`, used by `Bind.promise`
- `addStyleSheet` returns a function that will remove the sheet
- Stop diagnostic messages to console
- Fix typos in `doc/stores.md` (thank you Jlll1!)

### 1.0.0-beta-019
- Fix cleanup of fragments
- Unregister event handlers created within fragment when fragment disposed

### 1.0.0-beta-018
- parentFragment, allows you to apply elements to this element's parent
- Bind.style, passing an updater function: 'T -> CSSStyleDeclaration -> unit
- Bind.widthHeight, Bind.topLeft (wrappers on Bind.style)

### 1.0.0-beta-017
- Fix for Bind.each taking a view function of type : IObservable<T> -> SutilElement

### 1.0.0-beta-016
- Show exceptions from Bind both on screen and in console

### 1.0.0-beta-015
- Support for arrays in `each`

### 1.0.0-beta-014

- Support for media queries in `withStyle`
- Bug fix for Observable.distinctUntilChangedCompare

### 1.0.0-beta-013

- Typesafe events from Feliz.Engine.Event
- Fixes to adopted style sheets
- Finish conversion of old tests into headless/browser
- Few extra SVG attributes and elements (needs an engine)

### 1.0.0-beta-012

- Headless tests using Fable.Expect
- Remove "open Feliz" to avoid clash when integrating with React
- New static Bind members: visibility, style, toggleClass, className, classNames

### 1.0.0-beta-011

- Fix case for Chrome.DevTools.fs

### 1.0.0-beta-010

- Replace npm 'constructable-stylesheets-polyfill' with nuget package 'ConstructableStylesheetsPolyfill'

### 1.0.0-beta-009

- Chrome Extension example + minimal chrome.* api bindings
- Use importSideEffects for constructable-stylesheets-polyfill

### 1.0.0-beta-008

- Remove log message
- Fix "fetch source" in App
- Rename webcomponent.js to webcomponentinterop.js to avoid name clash

### 1.0.0-beta-007

- Support for adopted stylesheets
- Fixes for disposal of bind nodes

### 1.0.0-beta-006

- Eliminate use of eval in webcomponent.js

### 1.0.0-beta-005

- Additional overloads for Bind.el, allowing keyed bindings
- Support for web components
- Fixes to node disposal
- Front page for docs/website

### 1.0.0-beta-004

- Bind.fragment deprecated, use Bind.el instead
- Documentation code examples have max height, with faded text and Expand/Collapse buttons

### 1.0.0-beta-003

- Implemented "Open in REPL" in docs (App)
- Fix CSS for mobile
- Added DOM.mountAfter
- Added Bind.each overloads
- Added DataSimulation / SampleData for examples, docs and repl

### 1.0.0-beta-002

- Remove proxy.js and unused function makeProxy, unused

### 1.0.0-beta-001

- Rework to support fragments in various binding arrangements

### 1.0.0-alpha-006

- Support for Feliz.Engine 1.0.0-beta-004
- Support for Feliz.Engine.Bulma 1.0.0-beta-005

### 1.0.0-alpha-005

- Support for Feliz.Engine 1.0.0-beta-003
- Support for Feliz.Engine.Bulma 1.0.0-beta-004
- Fix cleanup bug in TimerLogic.fs
- Initialize page according to URL
- Reload url when book added, in case url referenced the new book
- Mobile drag/drop support in example app
- Improved layout for mobile in SortableTimerList example
- Change order of arguments for Store.map, DOM.addToClasslist, DOM.removeFromClasslist to favour pipeline style
- Added DOM.interval, DOM.timeout as unsubscribable wrappers to setTimeout and setInterval
- More easing functions (quad, quart, expo)
- Bug fix for clearing animations
- Html.value overrides to support storing of native value
- Timer example shows component hierarchy

### 1.0.0-alpha-004

- Change versioning scheme (same as Feliz.Engine, Feliz.Engine.Bulma)
- Bulma batteries included
- Convert all examples to new DSL and new Bulma
- Cleanup of old DSL code
- Extend DSL to remove binding boilerplate (if desired)

### 0.2.1-alpha

- Remove USE_FELIZ_ENGINE - ignored when when build nupkg
- Copied in latest Feliz.Engine direct from repo (package still not updated)
- Added nav elements to Bulma:
    navbar
    navbarBrand
    navbarStart
    navbarEnd
    navbarDropdown
    navbarItemA
    navbarItemDiv
- Now converted to Feliz.HtmlEngine, Feliz.AttrEngine & Feliz.CssEngine
- Previous change should have read "convert fully to Feliz.CssEngine"
- Convert fully to Feliz.Engine
- Updates to README.md and about_sutil.md
- Update to installation.md

### 0.1.3-alpha
- nav, aria*
- Store.zip, Store.distinct

### 0.1.2-alpha
- nav/aside elements (full implementation coming very soon)
- Cmd.ofAsync* support
- Added Bulma.fs to Sutil package

### 0.1.1-alpha
- Refactor App to support "books". "Examples" is now a book, and "Documentation" is another
- Self-hosting documentation in App
- Add FAKE tooling to build nuget package
- Added 7GUIs CRUD example
- Start refactor Bindings to BindAPI with overloads and function documentation
- More tweaks to login example - outline email field when invalid
- event-to-store support, allowing input events to end up binding to class "is-danger" for showing email field invalid
- Improved login example
- Fix z-order for header
- Login example with prototype Bulma module
- Fix sorting for "each" (used in Animation TODOs example)
- Better UX for examples App on mobile
- Consistent event dispatching for onMount
- Clean-up of stores now working correctly
- Navigable wrapper provides IObservable<'T> with a parser (Location -> 'T), allowing (for example) '#' and '?' to control the view
- Don't allow "sutil-add-class" into stylesheet (regression when changing name to Sutil)
- Hide withStyleSheet, confuses user who is actually looking for withStyle
- Partial implementation of CssEngine to allow incremental conversion ready for the full implementation
- Fix regression to Cells example
- Fix regression to Await example, involved adding onMount handler
- Big changes while integrating with Feliz.Engine, work-in-progress
- Bar chart SVG example
- Modal example
- cleanup when unmounting (switching between examples in app)
- dynamic update of store values
- firstOf, selectApp used in app to unmount apps and recreate when they are selected. Before, they
were all created all at once and then just hidden/shown. With unmounting, can begin to test resource
clean up (eg, disposing registered stores, subscriptions, tasks etc)
- support in devtools for refresh when new store is created
- move all Store.make calls into `view` function to scope resource usage
- disposeOnUnmount to allow components to register the stores and resources they use
- Example TransitionCustomCode, with the typewriter transition
- More easing functions. Implemented Fallback for crossfade, nicer outro now in Todos app.
- Fix for source display
- Clean up TransitionProp API
- Repository renamed
- Add Mountpoints tab in DevTools - allows a page remount without refresh
- Bug fixes, events for intros and outros
- Transitions section in app
- Start renaming to sutil
- Unit tests
- Fix animation regression in App/Todos.fs. DevTools made this a lot easier, being able to switch logging options on and off
- Implement ordering for each blocks
- Logging options in DevTools. Rework of promise bindings. Cleaner interface between app and devtools.
  Intermediate check-in
- Options window in DevTools, communicating with Sutil core. Still debugging, but *almost* working.
- Converted DevTools panel to sutil-Fable. This will make it easier and faster to develop. I want to use it to help
diagnose recent regression with the animation of the todos.
- Reworking of Cells example to try and make the reactive bindings neater. It's a challenge.
- NodeFactory is now "BuildContext -> BuildResult"
- Append/Replace is now fundamental within `DOM.El`. It means that bind etc don't need to augment BuildContext with special handlers
- Spreadsheet sample moved from 7GUIs to Miscellaneous. New Cells example that observes more of the specification outlined by 7GUIs (which is mainly about only redrawing/recalculating changes).
- Spreadsheet example
- Added tests, but cannot get them to compile (a TODO)
- Reorganized docs
- DomEvents example
- EventModifiers example
- HtmlTags example
- Add ObservablePromise to replace PromiseStore
- Switch await example to randomuser.me
- Await blocks example. This was fun and looks quite neat
- KeyedEach example. More rework of NodeFactory API
- Improved return value from NodeFactory. Experimented with IFactory, moved to branch.
- Rework on binding and each block. Better tracking of created elements and reference fixing when bound elements changed etc
- More "each" block examples. Covered a few more cases than are given in Svelte, due to differences in F# and JS. Still a w.i.p.
- Browser DevTools plugin. Only *just* got the stores view working. See section `DevTools` further down.
<img src="images/devtools.png" width="400" alt="Screenshot of Sutil DevTools plugin">

- More examples: FileInputs, Dimensions, If/Else/If-Else. Ported resize observer from Svelte (very clever stuff)
- Textarea example. Introduces `html` element to inject raw HTML, using an imported JS markdown library.
- Fix for issue #5
- Convert main app to Elmish. See src/App/App.fs
- Alfonso's PR also gives us a better Elmish MVU architecture than I had previously. See section further down on MVU
- Switch to Observable-based stores, thanks to PR from Alfonso
- Select bindings example
- Select multiple example. Apply styling (Bulma) to other examples
- Finished GroupInputs example. Bindings for radio groups and checkbox groups
- Started on GroupInputs example. Introduces bindGroup. Added Store.map2
- Added NumericInputs examples
- Styling rules can extend existing classes like this
```
    // Turn <label> into <label class='label'>. Useful if you have Bulma loaded, for example
    rule label {
        addClass "label"
        // other styles
    }

let view() = input [ ... ]  // No need to add class 'label' to make it into a Bulma input
```
- Cleaner support for Store<List<'T>>. See updated Todos example
- Added CheckboxInputs example. Tweaks to attribute binding
- Added ReactiveStatements example
- Refactored Stores (we now have Store.map and readable function equivalents for all operators)
- Fixes to demo app styling and production bundling thanks to a PR from [s0kil](https://github.com/s0kil)
- Demo app support for syntax highlighting example code. See section **Interacting with 3rd-party libraries**
- More examples ported into demo app
