namespace Chrome

open System
open Fable.Core
open Browser.Types

module Devtools =

    type Event = interface
        abstract addListener: (Window -> unit ) -> unit
        end

    and Panel = interface
        abstract onShown : Event
        abstract onHidden : Event
        end

    module Panels =
        [<Emit("chrome.devtools.panels.create($0,$1,$2,$3)")>]
        let create (title:string) (icon:string) (indexHtml:string) (success:Panel->unit) : unit = jsNative

    type EvalExceptionInfo = {
        code : string
        description : string
        details : obj array
        isError: bool
        isException: bool
        value : string
    }

    // Documentation says it's this
    //type EvalCallback<'T> = ('T  * EvalExceptionInfo) -> unit
    // but it's this
    type EvalCallback<'T> = 'T -> unit

    module InspectedWindow =
        [<Emit("chrome.devtools.inspectedWindow.eval($0,$1,$2)")>]
        let eval<'T> (expression:string) (options:obj) (callback : EvalCallback<'T>) : unit = jsNative