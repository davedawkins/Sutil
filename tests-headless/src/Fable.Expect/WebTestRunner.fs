module WebTestRunner

open System.Text.RegularExpressions
open Fable.Core
open Browser.Types

type SnapshotConfig =
    abstract updateSnapshots: bool

type AccessibilitySnapshot =
    interface end

type AccessibilityNode =
    abstract name: string
    abstract role: string
    abstract children: AccessibilityNode array

type JsObject<'T> =
    abstract Item: string -> 'T

type WebTestRunnerBindings =
    abstract getSnapshotConfig: unit -> JS.Promise<SnapshotConfig>
    abstract getSnapshots: unit -> JS.Promise<JsObject<string>>
    [<Emit("$0.getSnapshot({ name: $1 })")>]
    abstract getSnapshot: name: string -> JS.Promise<string>
    [<Emit("$0.saveSnapshot({ name: $1, content: $2 })")>]
    abstract saveSnapshot: name: string * content: string -> JS.Promise<unit>
    [<Emit("$0.removeSnapshot({ name: $1 })")>]
    abstract removeSnapshot: name: string -> JS.Promise<unit>
    [<Emit("$0.compareSnapshot({ name: $1, content: $2 })")>]
    abstract compareSnapshot: name: string * content: string -> JS.Promise<unit>

    /// Request a snapshot of the accessibility tree built in the browser representing the current page.
    abstract a11ySnapshot: unit -> JS.Promise<SnapshotConfig>
    /// Request a snapshot of the accessibility tree built in the browser representing the tree rooter by the passed selector property.
    [<Emit("$0.a11ySnapshot({ selector: $1 })")>]
    abstract a11ySnapshot: selector: string -> JS.Promise<SnapshotConfig>
    abstract findAccessibilityNode: snapshot: AccessibilitySnapshot * selector: (AccessibilityNode -> bool) -> AccessibilityNode option

    /// The specified path is resolved relative to the (JS-compiled) test file being executed.
    abstract writeFile: path: string * content: string -> JS.Promise<unit>
    /// The specified path is resolved relative to the (JS-compiled) test file being executed.
    abstract readFile: path: string -> JS.Promise<string>
    /// The specified path is resolved relative to the (JS-compiled) test file being executed.
    abstract removeFile: path: string -> JS.Promise<unit>

    [<Emit("$0.setViewport({ width: $1, height: $2 })")>]
    abstract setViewport: width: int * height: int -> JS.Promise<unit>

    abstract setUserAgent: userAgent: string -> JS.Promise<unit>

    [<Emit("$0.emulateMedia({ media: $1 })")>]
    abstract emulateMedia: media: string -> JS.Promise<unit>
    [<Emit("$0.emulateMedia({ colorSchema: $1 })")>]
    abstract emulateColorScheme: colorScheme: string -> JS.Promise<unit>
    [<Emit("$0.emulateMedia({ reducedMotion: $1 })")>]
    abstract emulateReducedMotion: reducedMotion: string -> JS.Promise<unit>

    /// Types a sequence of characters.
    /// Not affected by modifier keys, holding Shift will not type the text in upper-case.
    [<Emit("$0.sendKeys({ type: $1 })")>]
    abstract typeChars: characters: string -> JS.Promise<unit>
    /// Presses a single key, resulting in a key down, and a key up.
    /// Affected by modifier keys, holding Shift will type the text in upper-case.
    [<Emit("$0.sendKeys({ press: $1 })")>]
    abstract pressKey: key: string -> JS.Promise<unit>
    /// Holds down a single key.
    [<Emit("$0.sendKeys({ down: $1 })")>]
    abstract holdKey: key: string -> JS.Promise<unit>
    /// Releases a single key
    [<Emit("$0.sendKeys({ up: $1 })")>]
    abstract releaseKey: key: string -> JS.Promise<unit>

[<ImportAll("@web/test-runner-commands")>]
let Wtr: WebTestRunnerBindings = jsNative

[<AutoOpen>]
module Mocha =
    /// Test suite
    let [<Global>] describe (msg: string) (suite: unit -> unit): unit = jsNative

    /// Alias of `describe`
    let [<Global>] context (msg: string) (suite: unit -> unit): unit = jsNative

    /// Test case
    let [<Global>] it (msg: string) (test: unit -> JS.Promise<unit>): unit = jsNative

    /// Test case
    let [<Global("it")>] itSync (msg: string) (test: unit -> unit): unit = jsNative

    /// Run once before any test in the current suite
    let [<Global>] before (test: unit -> JS.Promise<unit>): unit = jsNative

    /// Run once before any test in the current suite
    let [<Global("before")>] beforeSync (test: unit -> unit): unit = jsNative

    /// Run before each test in the current suite
    let [<Global>] beforeEach (test: unit -> JS.Promise<unit>): unit = jsNative

    /// Run before each test in the current suite
    let [<Global("beforeEach")>] beforeEachSync (test: unit -> unit): unit = jsNative

    /// Run once after all tests in the current suite
    let [<Global>] after (test: unit -> JS.Promise<unit>): unit = jsNative

    /// Run once after all tests in the current suite
    let [<Global("after")>] afterSync (test: unit -> unit): unit = jsNative

    /// Run after each test in the current suite
    let [<Global>] afterEach (test: unit -> JS.Promise<unit>): unit = jsNative

    /// Run after each test in the current suite
    let [<Global("afterEach")>] afterEachSync (test: unit -> unit): unit = jsNative

[<RequireQualifiedAccess>]
module Expect =

    let private cleanHtml (html: string) =
        // Lit inserts comments with different values every time, so remove them
        let html = Regex(@"<\!--[\s\S]*?-->").Replace(html, "")
        // Trailing whitespace seems to cause issues too
        let html = Regex(@"\s+\n").Replace(html, "\n")
        html.Trim()

    /// Compares the content string with the snapshot of the given name within the current file.
    /// If the snapshot doesn't exist or tests are run with `--update-snapshots` option the snapshot will just be saved/updated.
    let matchSnapshot (description: string) (name: string) (content: string) = promise {
        let! config = Wtr.getSnapshotConfig()
        if config.updateSnapshots then
            return! Wtr.saveSnapshot(name, content)
        else
            try
                do! Wtr.compareSnapshot(name, content)
            with _ ->
                let! snapshot = Wtr.getSnapshot(name)
                // Snapshots can be large, so use `brief` argument to hide them in the error message (diffing should be displayed correctly)
                return Expect.AssertionError.Throw("match snapshot", description=description, actual=content, expected=snapshot, brief=true)
    }

    /// Compares `outerHML` (or `shadowRoot.innerHTML` for web components) of the element with the snapshot of the given name within the current file.
    /// If the snapshot doesn't exist or tests are run with `--update-snapshots` option the snapshot will just be saved/updated.
    let matchHtmlSnapshot (name: string) (el: HTMLElement) =
        let html, description =
            match el.shadowRoot with
            | null -> el.outerHTML, "outerHTML"
            | shadowRoot -> shadowRoot.innerHTML, "shadowRoot.innerHTML"

        html |> cleanHtml |> matchSnapshot description name
