namespace Sutil

open Fable.Core
open Browser.Types
open DOM
module WebComponent =
    type Callbacks<'T> = {
        Dispose : (unit -> unit)
        GetModel : unit -> 'T
        SetModel : 'T -> unit
        OnConnected : unit -> unit
    }

    [<Import("makeWebComponent", "./webcomponentinterop.js")>]
    let makeWebComponent<'T> name (ctor : Node -> Callbacks<'T>) (init : 'T) : unit = jsNative

open Fable.Core.JsInterop

type WebComponent =

    static member Register<'T>(name:string, ctor : IStore<'T> -> Node -> SutilElement, init : 'T ) =
        let wrapper (host:Node) : WebComponent.Callbacks<'T> =
            let model = Store.make init

            let result = ctor model host
            let disposeElement = DOM.mountOnShadowRoot result host

            let disposeWrapper() =
                model.Dispose()
                disposeElement()

            {   Dispose = disposeWrapper
                GetModel = (fun () -> model |> Store.current)
                SetModel = Store.set model
                OnConnected = fun _ -> DOM.dispatchSimple (host?shadowRoot?firstChild) "sutil-connected"
                }

        WebComponent.makeWebComponent name wrapper init

    static member Register<'T>(name:string, ctor : IStore<'T> -> SutilElement, init : 'T ) =
        WebComponent.Register( name, (fun store _ -> ctor store), init)
