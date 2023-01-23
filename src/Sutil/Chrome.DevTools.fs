namespace Chrome

open System
open Fable.Core
open Browser.Types
open Browser.Dom

///  <exclude />
type Event<'T> = interface
    abstract addListener: ('T -> unit ) -> unit
    end

///  <exclude />
type MessageSender = interface end

///  <exclude />
type Port = interface
    abstract disconnect : unit -> unit
    abstract name : string
    abstract onDisconnect : Event<unit>
    abstract onMessage : Event<obj>
    abstract postMessage: obj -> unit
    end

///  <exclude />
[<RequireQualifiedAccess>]
module Storage =
    type Sync() =
        [<Emit("chrome.storage.sync.get($0...)")>]
        static member get( keys : obj, callback : obj -> unit ) : unit = jsNative

        [<Emit("chrome.storage.sync.set($0)")>]
        static member set( keyValues : obj ) : unit = jsNative

///  <exclude />
type Runtime() =
    [<Emit("chrome.runtime.connect($0)")>]
    static member connect (connectInfo: obj) : Port = jsNative

    [<Emit("chrome.runtime.connect($0,$1)")>]
    static member connectToId (extensionId : string, connectInfo: obj) : obj = jsNative

    [<Emit("chrome.runtime.onConnect")>]
    static member onConnect : Event<obj> = jsNative

    [<Emit("chrome.runtime.onInstalled")>]
    static member onInstalled : Event<obj> = jsNative

///  <exclude />
[<RequireQualifiedAccess>]
module Devtools =

    [<RequireQualifiedAccess>]
    module Panels =
        type ExtensionPanel = interface
            abstract onShown : Event<Window>
            abstract onHidden : Event<Window>
            end

        type ExtensionSidebarPane = interface
            abstract onShown : Event<Window>
            abstract onHidden : Event<Window>
            abstract setExpression: string * string * (unit -> unit) -> unit
            abstract setHeight: height:string -> unit
            abstract setObject: jsonObject:obj * rootTitle:string * (unit -> unit) -> unit
            abstract setPage: page:string -> unit
            end

        type ElementsPanel = interface
            abstract onSelectionChanged : Event<unit>
            abstract createSidebarPane: title : string * (ExtensionSidebarPane -> unit) -> unit
            end

        [<Emit("chrome.devtools.panels.create($0...)")>]
        let create (title:string) (icon:string) (indexHtml:string) (success:ExtensionPanel->unit) : unit = jsNative

        [<Emit("chrome.devtools.panels.elements")>]
        let elements : ElementsPanel = jsNative

    // https://developer.chrome.com/docs/extensions/reference/devtools_inspectedWindow
    [<RequireQualifiedAccess>]
    module InspectedWindow =
        // Chrome: Documented return value when eval fails, but I only see a "Some undefined"
        type EvalExceptionInfo = {
            code : string
            description : string
            details : obj array
            isError: bool
            isException: bool
            value : string
        }

        // Documentation says it's this
        // type EvalCallback<'T> = ('T  * EvalExceptionInfo) -> unit
        // but it's this
        type EvalCallback<'T> = 'T -> unit

        [<Emit("chrome.devtools.inspectedWindow.eval($0,$1,$2)")>]
        let eval<'T> (expression:string) (options:obj) (callback : EvalCallback<'T>) : unit = jsNative

        [<Emit("chrome.devtools.inspectedWindow.tabId")>]
        let tabId : int = jsNative

///  <exclude />
module Helpers =
    let inject<'T,'A> (fn : 'A -> 'T) (arg:'A) : JS.Promise<'T> =
        //console.log($"({fn})({JS.JSON.stringify arg})")
        Promise.create( fun fulfil fail ->
            Devtools.InspectedWindow.eval
                $"({fn})({JS.JSON.stringify arg})"
                {| |}
                (fun result ->
                        //console.dir(result)
                        if Sutil.Interop.isUndefined result
                            then (fail <| Exception("Unknown error"))
                            else fulfil result)
        )
