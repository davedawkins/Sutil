# Sveltish

## Changelog (most recent first)

- Converted DevTools panel to Sveltish-Fable. This will make it easier and faster to develop. I want to use it to help
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
<img src="images/devtools.png" width="400" alt="Screenshot of Sveltish DevTools plugin">

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
