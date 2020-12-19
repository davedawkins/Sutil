module Sveltish.Attr

open DOM
open Browser.Types

let makeAttr = id
let className n = attr ("class",n)
let on event fn : NodeFactory = fun (_,el) ->
    el.addEventListener(event, fn)
    el :> Node
// Styles
let margin (n:obj)         = makeAttr("margin",n)
let marginTop (n:obj)      = makeAttr("margin-top",n)
let marginLeft (n:obj)     = makeAttr("margin-left",n)
let marginBottom (n:obj)   = makeAttr("margin-bottom",n)
let marginRight (n:obj)    = makeAttr("margin-right",n)
let backgroundColor(n:obj) = makeAttr("background-color",n)
let borderColor (n:obj)    = makeAttr("border-color",n)
let borderWidth (n:obj)    = makeAttr("border-width",n)
let color (n:obj)          = makeAttr("color",n)
let cursor (n:obj)         = makeAttr("cursor",n)
let justifyContent (n:obj) = makeAttr("justify-content",n)
let paddingBottom (n:obj)  = makeAttr("padding-bottom",n)
let paddingLeft (n:obj)    = makeAttr("padding-left",n)
let paddingRight (n:obj)   = makeAttr("padding-right",n)
let paddingTop (n:obj)     = makeAttr("padding-top",n)
let textAlign (n:obj)      = makeAttr("text-align",n)
let whiteSpace (n:obj    ) = makeAttr("white-space",n)
let alignItems     (n:obj) = makeAttr("align-items",n)
let border         (n:obj) = makeAttr("border",n)
let borderRadius   (n:obj) = makeAttr("border-radius",n)
let boxShadow      (n:obj) = makeAttr("box-shadow",n)
let display        (n:obj) = makeAttr("display",n)
let fontSize       (n:obj) = makeAttr("font-size",n)
let fontFamily     (n:obj) = makeAttr("font-family",n)
let width          (n:obj) = makeAttr("width",n)
let maxWidth       (n:obj) = makeAttr("max-width",n)
let height         (n:obj) = makeAttr("height",n)
let lineHeight     (n:obj) = makeAttr("line-height",n)
let position       (n:obj) = makeAttr("position",n)
let verticalAlign  (n:obj) = makeAttr("vertical-align",n)
let fontWeight     (n:obj) = makeAttr("font-height",n)
let ``float``      (n:obj) = makeAttr("float",n)
let padding        (n:obj) = makeAttr("padding",n)
let boxSizing      (n:obj) = makeAttr("box-sizing",n)
let userSelect     (n:obj) = makeAttr("user-select",n)
let top            (n:obj) = makeAttr("top",n)
let left           (n:obj) = makeAttr("left",n)
let opacity        (n:obj) = makeAttr("opacity",n)
let transition     (n:obj) = makeAttr("transition",n)

