module Sveltish.Attr

open DOM
open Browser.Types


// Attributes
let className n    = attr("class",n)
let placeholder n  = attr("placeholder",n)
let href n         = attr("href",n)
let src n          = attr("src",n)
let alt n          = attr("alt",n)
let disabled n     = attr("disabled",n)
let type' n        = attr("type",n)

let class' = className

let on event fn : NodeFactory = fun (_,el) ->
    el.addEventListener(event, fn)
    el

let onKeyboard event (fn : KeyboardEvent -> unit) : NodeFactory = fun (_,el) ->
    el.addEventListener(event, unbox fn )
    el

let onClick fn = on "click" fn

let onKeyDown (fn : (KeyboardEvent -> unit)) = onKeyboard "keydown" fn

let cssAttr = id

// Styles
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
let borderRadius   (n:obj) = cssAttr("border-radius",n)
let boxShadow      (n:obj) = cssAttr("box-shadow",n)
let zIndex         (n:obj) = cssAttr("z-index",n)
let display        (n:obj) = cssAttr("display",n)
let fontSize       (n:obj) = cssAttr("font-size",n)
let fontFamily     (n:obj) = cssAttr("font-family",n)
let maxHeight      (n:obj) = cssAttr("maxHeight",n)
let width          (n:obj) = cssAttr("width",n)
let maxWidth       (n:obj) = cssAttr("max-width",n)
let height         (n:obj) = cssAttr("height",n)
let lineHeight     (n:obj) = cssAttr("line-height",n)
let position       (n:obj) = cssAttr("position",n)
let verticalAlign  (n:obj) = cssAttr("vertical-align",n)
let fontWeight     (n:obj) = cssAttr("font-height",n)
let float'         (n:obj) = cssAttr("float",n)
let padding        (n:obj) = cssAttr("padding",n)
let boxSizing      (n:obj) = cssAttr("box-sizing",n)
let userSelect     (n:obj) = cssAttr("user-select",n)
let top            (n:obj) = cssAttr("top",n)
let left           (n:obj) = cssAttr("left",n)
let opacity        (n:obj) = cssAttr("opacity",n)
let transition     (n:obj) = cssAttr("transition",n)
let textDecoration (n:obj) = cssAttr("text-decoration",n)
let addClass       (n:obj) = cssAttr("sveltish-add-class",n)