module Sutil.Attr

open DOM
open Browser.Types
open Fable.Core.JsInterop

let autofocus : SutilElement =
    nodeFactory <| fun ctx ->
        let e = ctx.Parent
        DOM.rafu (fun _ ->
            e?focus()
            e?setSelectionRange(99999,99999)
            )
        unitResult(ctx, "autofocus")

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
    let el = ctx.ParentNode
    let rec h (e:Event) =
        for opt in options do
            match opt with
            | Once -> el.removeEventListener(event,h)
            | PreventDefault -> e.preventDefault()
            | StopPropagation -> e.stopPropagation()
            | StopImmediatePropagation -> e.stopImmediatePropagation()
        fn(e)
    el.addEventListener(event, h)
    unitResult(ctx, "on")

let onCustomEvent<'T> (event: string) (fn: CustomEvent<'T> -> unit) (options: EventModifier list) =
    on event (unbox fn) options
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

let subscribeOnMount (f : unit -> (unit -> unit)) = onMount (fun e -> SutilNode.RegisterUnsubscribe(asElement<Node>(e.target),f())) [Once]

