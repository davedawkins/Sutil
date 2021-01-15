namespace Chrome

open System
open Fable.Core
open Browser.Types

[<RequireQualifiedAccess>]
module Devtools =

    [<RequireQualifiedAccess>]
    module Panels =
        type Event<'T> = interface
            abstract addListener: ('T -> unit ) -> unit
            end

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