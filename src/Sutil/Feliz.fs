module Sutil.Feliz

open Sutil.DOM

#if USE_FELIZ_ENGINE
open Feliz

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of NodeFactory

let Css =
    CssEngine
        { new CssHelper<string * obj> with
            override _.MakeStyle (key,value) = (key,value)
        }

let Html =
    HtmlEngine
        { new IConverter<NodeFactory, NodeAttr> with
            override _.CreateEl(tag, nodes) = el tag (unbox nodes)
            override _.ChildrenToProp(children) = NodeAttr(fragment children)
            override _.StringToEl(v) = text v

            override _.EmptyEl = fun _ -> unitResult()
            override _.FloatToEl(v) = text $"{v}"
            override _.IntToEl(v) = text $"{v}"
            override _.BoolToEl(v) = text $"{v}" }
#endif