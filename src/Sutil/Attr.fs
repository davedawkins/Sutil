module Sutil.Attr

open DOM
open Browser.Types
open Fable.Core.JsInterop

// Attributes
#if !USE_FELIZ_ENGINE
let role         n = attr("role", n)
let ariaHidden   n = attr("aria-hidden", n)
let ariaExpanded n = attr("aria-expanded", n)
let dataTarget   n = attr("data-target", n)
let accept n       = attr("accept",n)
let name n         = attr("name",n)
let action n       = attr("action",n)
let ariaLabel n    = attr("aria-label",n)
let className n    = attr("class",n)
let placeholder n  = attr("placeholder",n)
let target n       = attr("target",n)
let href n         = attr("href",n)
let src n          = attr("src",n)
let alt n          = attr("alt",n)
let disabled n     = attr("disabled",n)
let min n          = attr("min",n)
let max n          = attr("max",n)
let size n         = attr("size",n)
let value n        = attr("value",n)
let multiple : NodeFactory = attr("multiple","")
let rows n         = attr("rows",n)
let cols n         = attr("cols",n)
let readonly : NodeFactory = attr("readonly","true" :> obj)
let required : NodeFactory = attr("required","true" :> obj)
#else



#endif

let autofocus : NodeFactory =
    nodeFactory <| fun ctx ->
        let e = ctx.Parent
        DOM.rafu (fun _ ->
            e?focus()
            e?setSelectionRange(99999,99999)
            )
        unitResult()


// Attributes that are either keywords or core functions
let id' n          = attr("id",n)
let type' n        = attr("type",n)
let for' n         = attr("for",n)
let class' n       = attr("class",n)
let unclass n      = attr("class-", n)
let unclass' n     = attr("class-", n)
let style (cssAttrs: (string * obj) seq) = attr("style", cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat "")

// Events

type EventModifier =
    | Once
    | PreventDefault
    | StopPropagation
    | StopImmediatePropagation

let on (event : string) (fn : Event -> unit) (options : EventModifier list) = nodeFactory <| fun ctx ->
    let el = ctx.Parent
    let rec h (e:Event) =
        for opt in options do
            match opt with
            | Once -> el.removeEventListener(event,h)
            | PreventDefault -> e.preventDefault()
            | StopPropagation -> e.stopPropagation()
            | StopImmediatePropagation -> e.stopImmediatePropagation()
        fn(e)
    el.addEventListener(event, h)
    unitResult()

//let on0 event fn : NodeFactory = fun (_,el) ->
//    el.addEventListener(event, fn)
//    unitResult()

let onKeyboard event (fn : KeyboardEvent -> unit) options =
    on event (unbox fn) options

let onMouse event (fn : MouseEvent -> unit) options =
    on event (unbox fn) options

let asElement<'T when 'T :> Node> (target:EventTarget) : 'T = (target :?> 'T)

let inline private _event x = (x :> obj :?> Event)

type InputEvent() =
    member x.event = _event x
    member x.inputElement =
        let _event x = (x :> obj :?> Event)
        asElement<HTMLInputElement> (_event x).target

let onInput (fn : InputEvent -> unit) options =
    on "input" (unbox fn) options

let onClick fn options = on "click" fn options

let onMount fn options = on Event.Mount fn options
let onUnmount fn options = on Event.Unmount fn options
let onShow fn options = on Event.Show fn options
let onHide fn options = on Event.Hide fn options

let onKeyDown (fn : (KeyboardEvent -> unit)) options  = onKeyboard "keydown" fn options
let onMouseMove fn options  = onMouse "mousemove" fn options

