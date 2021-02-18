module Sutil.Feliz

open Feliz
open Sutil.DOM

let Css =
    CssEngine(fun k v -> k, box v)

let Html =
    HtmlEngine
        { new HtmlHelper<NodeFactory> with
            member _.MakeNode(tag, nodes) = el tag nodes
            member _.StringToNode(v) = text v
            member _.EmptyNode = fragment [] }

let Attr =
    AttrEngine
        { new AttrHelper<NodeFactory> with
            member _.MakeAttr(key, value) = attr(key, value)
            member _.MakeBooleanAttr(key, value) = attr(key, value) }
