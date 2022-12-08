namespace Sutil

open Fable.Core
open Browser.Types
open DOM

type WrapUnitUnit = System.Func<unit,unit>

module WebComponent =
    type Callbacks<'T> = {
        OnDisconnected : WrapUnitUnit
        GetModel : System.Func<unit,'T> // unit -> 'T
        SetModel : System.Func<'T, unit>
        OnConnected : WrapUnitUnit
    }

    [<Import("makeWebComponent", "./webcomponentinterop.js")>]
    let makeWebComponent<'T> name (ctor : Node -> Callbacks<'T>) (init : 'T) : unit = jsNative

open Fable.Core.JsInterop

type WebComponent =

    static member Register<'T>(name:string, ctor : IStore<'T> -> Node -> SutilElement, model : IStore<'T>, dispose ) =
        let wrapper (host:Node) : WebComponent.Callbacks<'T> =
            let result = ctor model host
            let disposeElement = DOM.mountOnShadowRoot result host

            let disposeWrapper() =
                dispose()
                disposeElement()

            {   OnDisconnected = WrapUnitUnit(disposeWrapper)
                GetModel = System.Func<unit,'T>(fun () -> model |> Store.current)
                SetModel = System.Func<'T,unit>(Store.set model)
                OnConnected = WrapUnitUnit( fun _ -> DOM.dispatchSimple (host?shadowRoot?firstChild) Event.Connected )//"sutil-connected"
                }

        WebComponent.makeWebComponent name wrapper (model |> Store.get)

    static member Register<'T>(name:string, ctor : IStore<'T> -> Node -> SutilElement, model : IStore<'T> ) =
        WebComponent.Register(name, ctor, model, ignore)

    //
    // Web component manages store internally, you have no access to it
    // View function takes both the store and the Node of the component
    //
    static member Register<'T>(name:string, ctor : IStore<'T> -> Node -> SutilElement, init : 'T ) =
        let model = Store.make init
        let dispose = fun() ->
            Fable.Core.JS.console.log("Register: dispose called")
            model.Dispose()
        WebComponent.Register(name, ctor, model, dispose)

    //
    // Web component manages store internally, you have no access to it
    //
    static member Register<'T>(name:string, ctor : IStore<'T> -> SutilElement, init : 'T ) =
        WebComponent.Register( name, (fun store _ -> ctor store), init)
