module Sutil.Attr

open DOM
open Browser.Types
open Fable.Core.JsInterop

// Attributes
let accept n       = attr("accept",n)
let name n         = attr("name",n)
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
let value n        = attr("value",n)
let style (cssAttrs: (string * obj) seq) = attr("style", cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat "")
let multiple : NodeFactory = attr("multiple","")
let rows n         = attr("rows",n)
let cols n         = attr("cols",n)
let readonly : NodeFactory = attr("readonly","true" :> obj)
let autofocus : NodeFactory =
    fun ctx ->
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

let onMount fn options = on Event.Mount fn options
let onShow fn options = on Event.Show fn options
let onHide fn options = on Event.Hide fn options

let onKeyDown (fn : (KeyboardEvent -> unit)) options  = onKeyboard "keydown" fn options
let onMouseMove fn options  = onMouse "mousemove" fn options

let cssAttr = id

#if !USE_FELIZ_ENGINE

type ICssUnit = interface end

type Units =
    | Auto
    | Zero
    | Em of float
    | Px of float
    | Pct of float
    | Pt of float
    with
        override this.ToString() =
            match this with
            |Auto -> "auto"
            |Zero -> "0"
            |Em  n -> $"{n}em"
            |Px  n -> $"{n}px"
            |Pct n -> $"{n}%%"
            |Pt  n -> $"{n}pt"
        interface ICssUnit

[<AutoOpen>]
module CssEngine =
    type CssHelper<'Style> =
        // TODO: Should the value be string too?
        abstract MakeStyle: key: string * value: obj -> 'Style

    type CssEngine<'Style>(h: CssHelper<'Style>) =
        member _.all (value: string) = h.MakeStyle("all", value)
        member _.zIndex(value: int) = h.MakeStyle("z-index", value)

        member _.margin (all:int)
                    = h.MakeStyle("margin",$"{all}px")

        member _.margin (all:float)
                    = h.MakeStyle("margin",$"{all}px")

        member _.margin (all:ICssUnit)
                    = h.MakeStyle("margin",$"{all}")

        member _.margin (left:int,top:int,right:int,bottom:int)
                    = h.MakeStyle("margin",$"{left}px {top}px {right}px {bottom}px")

        member _.margin (left:float,top:float,right:float,bottom:float)
                    = h.MakeStyle("margin",$"{left}px {top}px {right}px {bottom}px")

        member _.margin (left:ICssUnit,top:ICssUnit,right:ICssUnit,bottom:ICssUnit)
                    = h.MakeStyle("margin",$"{left} {top} {right} {bottom}")

        member _.margin (lr:ICssUnit,tb:ICssUnit)
                    = h.MakeStyle("margin",$"{lr} {tb}")

        member _.marginTop (n:obj)      = h.MakeStyle("margin-top",n)
        member _.marginLeft (n:obj)     = h.MakeStyle("margin-left",n)
        member _.marginBottom (n:obj)   = h.MakeStyle("margin-bottom",n)
        member _.marginRight (n:obj)    = h.MakeStyle("margin-right",n)
        member _.backgroundColor(n:obj) = h.MakeStyle("background-color",n)
        member _.borderColor (n:obj)    = h.MakeStyle("border-color",n)
        member _.borderWidth (n:obj)    = h.MakeStyle("border-width",n)
        member _.color (n:obj)          = h.MakeStyle("color",n)
        member _.cursor (n:obj)         = h.MakeStyle("cursor",n)
        member _.justifyContent (n:obj) = h.MakeStyle("justify-content",n)
        member _.paddingBottom (n:obj)  = h.MakeStyle("padding-bottom",n)
        member _.paddingLeft (n:obj)    = h.MakeStyle("padding-left",n)
        member _.paddingRight (n:obj)   = h.MakeStyle("padding-right",n)
        member _.paddingTop (n:obj)     = h.MakeStyle("padding-top",n)
        member _.textAlign (n:obj)      = h.MakeStyle("text-align",n)
        member _.whiteSpace (n:obj    ) = h.MakeStyle("white-space",n)
        member _.alignItems     (n:obj) = h.MakeStyle("align-items",n)
        member _.border         (n:obj) = h.MakeStyle("border",n)
        member _.background     (n:obj) = h.MakeStyle("background",n)
        member _.borderRadius   (n:obj) = h.MakeStyle("border-radius",n)
        member _.borderTopLeftRadius   (n:obj) = h.MakeStyle("border-top-left-radius",n)
        member _.borderTopRightRadius   (n:obj) = h.MakeStyle("border-top-right-radius",n)
        member _.borderBottomLeftRadius   (n:obj) = h.MakeStyle("border-bottom-left-radius",n)
        member _.borderBottomRightRadius   (n:obj) = h.MakeStyle("border-bottom-right-radius",n)
        member _.boxShadow      (n:obj) = h.MakeStyle("box-shadow",n)
        member _.display        (n:obj) = h.MakeStyle("display",n)
        member _.fontSize       (n:obj) = h.MakeStyle("font-size",n)
        member _.fontStyle      (n:obj) = h.MakeStyle("font-style",n)
        member _.fontFamily     (n:obj) = h.MakeStyle("font-family",n)
        member _.minHeight      (n:obj) = h.MakeStyle("min-height",n)
        member _.maxHeight      (n:obj) = h.MakeStyle("max-height",n)
        member _.width          (n:obj) = h.MakeStyle("width",n)
        member _.minWidth       (n:obj) = h.MakeStyle("min-width",n)
        member _.maxWidth       (n:obj) = h.MakeStyle("max-width",n)
        member _.height         (n:obj) = h.MakeStyle("height",n)
        member _.lineHeight     (n:obj) = h.MakeStyle("line-height",n)
        member _.position       (n:obj) = h.MakeStyle("position",n)
        member _.verticalAlign  (n:obj) = h.MakeStyle("vertical-align",n)
        member _.fontWeight     (n:obj) = h.MakeStyle("font-weight",n)
        member _.float'         (n:obj) = h.MakeStyle("float",n)
        member _.padding        (n:obj) = h.MakeStyle("padding",n)
        member _.boxSizing      (n:obj) = h.MakeStyle("box-sizing",n)
        member _.userSelect     (n:obj) = h.MakeStyle("user-select",n)
        member _.top            (n:obj) = h.MakeStyle("top",n)
        member _.left           (n:obj) = h.MakeStyle("left",n)
        member _.bottom           (n:obj) = h.MakeStyle("bottom",n)
        member _.right           (n:obj) = h.MakeStyle("right",n)
        member _.opacity        (n:obj) = h.MakeStyle("opacity",n)
        member _.transition     (n:obj) = h.MakeStyle("transition",n)
        member _.resize         (n:obj) = h.MakeStyle("resize",n)
        member _.overflow       (n:obj) = h.MakeStyle("overflow",n)
        member _.textDecoration (n:obj) = h.MakeStyle("text-decoration",n)
        member _.textDecorationStyle (n:obj) = h.MakeStyle("text-decoration-style",n)
        member _.textDecorationColor (n:obj) = h.MakeStyle("text-decoration-color",n)
        member _.textDecorationThickness (n:obj) = h.MakeStyle("text-decoration-thickness",n)
        member _.borderSpacing  (n:obj) = h.MakeStyle("border-spacing",n)
        member _.letterSpacing  (n:obj) = h.MakeStyle("letter-spacing",n)
        member _.borderBottom   (n:obj) = h.MakeStyle("border-bottom",n)
        member _.borderRight    (n:obj) = h.MakeStyle("border-right",n)
        member _.borderLeft     (n:obj) = h.MakeStyle("border-left",n)
        member _.borderTop      (n:obj) = h.MakeStyle("border-top",n)
        member _.flex           (n:obj) = h.MakeStyle("flex",n)
        member _.flexDirection  (n:obj) = h.MakeStyle("flex-direction",n)
        member _.transform      (n:obj) = h.MakeStyle("transform",n)
        member _.gridTemplateColumns (n:obj) = h.MakeStyle("grid-template-columns",n)
        member _.gridGap        (n:obj) = h.MakeStyle("grid-gap",n)

        member _.stroke          (n:obj) = h.MakeStyle("stroke",n)
        member _.fill            (n:obj) = h.MakeStyle("fill",n)
        member _.strokeDasharray (n:obj) = h.MakeStyle("stroke-dasharray",n)
        member _.textAnchor      (n:obj) = h.MakeStyle("text-anchor",n)

#else

open Feliz

#endif

let Css =
    CssEngine
        { new CssHelper<string * obj> with
            override _.MakeStyle (key,value) = (key,value)
        }


let addClass       (n:obj) = cssAttr("sutil-add-class",n)
let useGlobal              = cssAttr("sutil-use-global","" :> obj)