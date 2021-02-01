# Sutil

## Changelog (most recent first)

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
