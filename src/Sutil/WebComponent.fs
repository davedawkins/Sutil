namespace Sutil

open Fable.Core
open Browser.Types
open DOM

module WebComponent =
    type Callbacks<'T> = {
        OnDisconnected : (unit -> unit)
        GetModel : unit -> 'T
        SetModel : 'T -> unit
        OnConnected : unit -> unit
    }

    [<Import("makeWebComponent", "./webcomponentinterop.js")>]
    let makeWebComponent<'T> name (ctor : Node -> Callbacks<'T>) (init : 'T) : unit = jsNative

open Fable.Core.JsInterop

type WebComponent =

    static member Register<'T>(name:string, view : IStore<'T> -> Node -> SutilElement, initValue : 'T, initModel: unit -> IStore<'T>, dispose : IStore<'T> -> unit) =

        // If model is instantiated here, then it doesn't get captured correctly within 'wrapper', with multiple calls
        // to Register() (such as the Counter example)
        //let model = initModel()

        let wrapper (host:Node) : WebComponent.Callbacks<'T> =
            let model = initModel()

            let sutilElement = view model host
            let disposeElement = DOM.mountOnShadowRoot sutilElement host

            let disposeWrapper() =
                dispose(model)
                disposeElement()

            {   OnDisconnected = disposeWrapper
                GetModel = (fun () -> model |> Store.current)
                SetModel = Store.set model
                OnConnected = fun _ -> DOM.dispatchSimple (host?shadowRoot?firstChild) Event.Connected //"sutil-connected"
                }

        WebComponent.makeWebComponent name wrapper initValue

    static member Register<'T>(name:string, view : IStore<'T> -> Node -> SutilElement, init : 'T ) =
        WebComponent.Register( name, view, init, (fun () -> Store.make init), (fun s -> s.Dispose()))

    static member Register<'T>(name:string, view : IStore<'T> -> SutilElement, init : 'T ) =
        WebComponent.Register( name, (fun store _ -> view store), init, (fun () -> Store.make init), (fun s -> s.Dispose()))
