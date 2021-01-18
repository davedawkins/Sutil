module Sveltish.Attr

open DOM
open Browser.Types
open Fable.Core.JsInterop

// Attributes
let accept n       = attr("accept",n)
let name n         = attr("name",n)
let className n    = attr("class",n)
let placeholder n  = attr("placeholder",n)
let target n       = attr("target",n)
let href n         = attr("href",n)
let src n          = attr("src",n)
let alt n          = attr("alt",n)
let disabled n     = attr("disabled",n)
let min n          = attr("min",n)
let max n          = attr("max",n)
let value n        = attr("value",n)
let style n        = attr("style",n)
let multiple : NodeFactory = attr("multiple","")
let rows n         = attr("rows",n)
let cols n         = attr("cols",n)
let readonly : NodeFactory = attr("readonly","true" :> obj)
let autofocus : NodeFactory =
    fun ctx ->
        let e = ctx.Parent
        DOM.raf (fun _ ->
            e?focus()
            e?setSelectionRange(99999,99999)
            ) |> ignore
        unitResult()


// Attributes that are either keywords or core functions
let id' n          = attr("id",n)
let type' n        = attr("type",n)
let for' n         = attr("for",n)
let class'         = className
let unclass n      = attr("class-", n)
let unclass' n     = attr("class-", n)

// Events

type EventModifier =
    | Once
    | PreventDefault
    | StopPropagation
    | StopImmediatePropagation

let on (event : string) (fn : Event -> unit) (options : EventModifier list) = fun ctx ->
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

let onClick fn options = on "click" fn options

let onShow fn options = on Event.Show fn options
let onHide fn options = on Event.Hide fn options

let onKeyDown (fn : (KeyboardEvent -> unit)) options  = onKeyboard "keydown" fn options
let onMouseMove fn options  = onMouse "mousemove" fn options

let cssAttr = id

// Styles
let all (n:obj)            = cssAttr("all",n)
let margin (n:obj)         = cssAttr("margin",n)
let marginTop (n:obj)      = cssAttr("margin-top",n)
let marginLeft (n:obj)     = cssAttr("margin-left",n)
let marginBottom (n:obj)   = cssAttr("margin-bottom",n)
let marginRight (n:obj)    = cssAttr("margin-right",n)
let backgroundColor(n:obj) = cssAttr("background-color",n)
let borderColor (n:obj)    = cssAttr("border-color",n)
let borderWidth (n:obj)    = cssAttr("border-width",n)
let color (n:obj)          = cssAttr("color",n)
let cursor (n:obj)         = cssAttr("cursor",n)
let justifyContent (n:obj) = cssAttr("justify-content",n)
let paddingBottom (n:obj)  = cssAttr("padding-bottom",n)
let paddingLeft (n:obj)    = cssAttr("padding-left",n)
let paddingRight (n:obj)   = cssAttr("padding-right",n)
let paddingTop (n:obj)     = cssAttr("padding-top",n)
let textAlign (n:obj)      = cssAttr("text-align",n)
let whiteSpace (n:obj    ) = cssAttr("white-space",n)
let alignItems     (n:obj) = cssAttr("align-items",n)
let border         (n:obj) = cssAttr("border",n)
let background     (n:obj) = cssAttr("background",n)
let borderRadius   (n:obj) = cssAttr("border-radius",n)
let borderTopLeftRadius   (n:obj) = cssAttr("border-top-left-radius",n)
let borderTopRightRadius   (n:obj) = cssAttr("border-top-right-radius",n)
let borderBottomLeftRadius   (n:obj) = cssAttr("border-bottom-left-radius",n)
let borderBottomRightRadius   (n:obj) = cssAttr("border-bottom-right-radius",n)
let boxShadow      (n:obj) = cssAttr("box-shadow",n)
let zIndex         (n:obj) = cssAttr("z-index",n)
let display        (n:obj) = cssAttr("display",n)
let fontSize       (n:obj) = cssAttr("font-size",n)
let fontFamily     (n:obj) = cssAttr("font-family",n)
let minHeight      (n:obj) = cssAttr("min-height",n)
let maxHeight      (n:obj) = cssAttr("max-height",n)
let width          (n:obj) = cssAttr("width",n)
let minWidth       (n:obj) = cssAttr("min-width",n)
let maxWidth       (n:obj) = cssAttr("max-width",n)
let height         (n:obj) = cssAttr("height",n)
let lineHeight     (n:obj) = cssAttr("line-height",n)
let position       (n:obj) = cssAttr("position",n)
let verticalAlign  (n:obj) = cssAttr("vertical-align",n)
let fontWeight     (n:obj) = cssAttr("font-weight",n)
let float'         (n:obj) = cssAttr("float",n)
let padding        (n:obj) = cssAttr("padding",n)
let boxSizing      (n:obj) = cssAttr("box-sizing",n)
let userSelect     (n:obj) = cssAttr("user-select",n)
let top            (n:obj) = cssAttr("top",n)
let left           (n:obj) = cssAttr("left",n)
let opacity        (n:obj) = cssAttr("opacity",n)
let transition     (n:obj) = cssAttr("transition",n)
let resize         (n:obj) = cssAttr("resize",n)
let overflow       (n:obj) = cssAttr("overflow",n)
let textDecoration (n:obj) = cssAttr("text-decoration",n)
let textDecorationStyle (n:obj) = cssAttr("text-decoration-style",n)
let textDecorationColor (n:obj) = cssAttr("text-decoration-color",n)
let borderSpacing  (n:obj) = cssAttr("border-spacing",n)
let borderBottom   (n:obj) = cssAttr("border-bottom",n)
let borderRight    (n:obj) = cssAttr("border-right",n)
let borderLeft     (n:obj) = cssAttr("border-left",n)
let borderTop      (n:obj) = cssAttr("border-top",n)
let flex           (n:obj) = cssAttr("flex",n)
let flexDirection  (n:obj) = cssAttr("flex-direction",n)

let addClass       (n:obj) = cssAttr("sveltish-add-class",n)
let useGlobal              = cssAttr("sveltish-use-global","" :> obj)