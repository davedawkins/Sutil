module Sutil.WebComponents

open Fable.Core
open Browser.Types
open DOM

// Web components

type WebComponentCallbacks<'T> = {
    Dispose : (unit -> unit)
    GetModel : unit -> 'T
    SetModel : 'T -> unit
}

[<Import("makeWebComponent", "./webcomponent.js")>]
let makeWebComponent<'T> name (ctor : Node -> WebComponentCallbacks<'T>) (init : 'T) : unit = jsNative

let registerWebComponent<'T> name (ctor : IStore<'T> -> SutilElement) (init : 'T) : unit =

    let wrapper (host:Node) : WebComponentCallbacks<'T> =
        let model = Store.make init
        let result = ctor(model)
        let disposeElement = DOM.mountOnShadowRoot result host

        let disposeWrapper() =
            model.Dispose()
            disposeElement()

        {   Dispose = disposeWrapper
            GetModel = (fun () -> model |> Store.current)
            SetModel = Store.set model }

    makeWebComponent name wrapper init
