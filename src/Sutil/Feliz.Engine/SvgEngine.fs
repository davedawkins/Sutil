namespace Feliz

open System

type SvgHelper<'Node> =
    abstract MakeSvgNode: string * 'Node seq -> 'Node
    abstract StringToSvgNode: string -> 'Node
    abstract EmptySvgNode: 'Node

module internal SvgHelperExtensions =
    type SvgHelper<'Node> with
        member this.MakeSvgNode(tag: string, v: string) =
            this.MakeSvgNode(tag, [this.StringToSvgNode v])

open SvgHelperExtensions

type SvgEngine<'Node>(h: SvgHelper<'Node>) =
    /// Create a custom element
    ///
    /// You generally shouldn't need to use this, if you notice an element missing please submit an issue.
    member _.custom (key: string, children: seq<'Node>) = h.MakeSvgNode(key, children)
    /// The empty element, renders nothing on screen
    member _.none : 'Node = h.EmptySvgNode

    /// SVG Image element, not to be confused with HTML img alias.
    member _.image (children: seq<'Node>) = h.MakeSvgNode("image", children)
    /// The svg element is a container that defines a new coordinate system and viewport. It is used as the outermost element of SVG documents, but it can also be used to embed an SVG fragment inside an SVG or HTML document.
    member _.svg (children: seq<'Node>) = h.MakeSvgNode("svg", children)
    member _.circle (children: seq<'Node>) = h.MakeSvgNode("circle", children)
    member _.clipPath (children: seq<'Node>) = h.MakeSvgNode("clipPath", children)
    /// The <defs> element is used to store graphical objects that will be used at a later time. Objects created inside a <defs> element are not rendered directly. To display them you have to reference them (with a <use> element for example). https://developer.mozilla.org/en-US/docs/Web/SVG/Element/defs
    member _.defs (children: seq<'Node>) = h.MakeSvgNode("defs", children)
    /// The <desc> element provides an accessible, long-text description of any SVG container element or graphics element.
    member _.desc (value: string) = h.MakeSvgNode("desc", value)
    member _.ellipse (children: seq<'Node>) = h.MakeSvgNode("ellipse", children)
    member _.feBlend (children: seq<'Node>) = h.MakeSvgNode("feBlend", children)
    member _.feColorMatrix (children: seq<'Node>) = h.MakeSvgNode("feColorMatrix", children)
    member _.feComponentTransfer (children: seq<'Node>) = h.MakeSvgNode("feComponentTransfer", children)
    member _.feComposite (children: seq<'Node>) = h.MakeSvgNode("feComposite", children)
    member _.feConvolveMatrix (children: seq<'Node>) = h.MakeSvgNode("feConvolveMatrix", children)
    member _.feDiffuseLighting (children: seq<'Node>) = h.MakeSvgNode("feDiffuseLighting", children)
    member _.feDisplacementMap (children: seq<'Node>) = h.MakeSvgNode("feDisplacementMap", children)
    member _.feDistantLight (children: seq<'Node>) = h.MakeSvgNode("feDistantLight", children)
    member _.feDropShadow (children: seq<'Node>) = h.MakeSvgNode("feDropShadow", children)
    member _.feFlood (children: seq<'Node>) = h.MakeSvgNode("feFlood", children)
    member _.feFuncA (children: seq<'Node>) = h.MakeSvgNode("feFuncA", children)
    member _.feFuncB (children: seq<'Node>) = h.MakeSvgNode("feFuncB", children)
    member _.feFuncG (children: seq<'Node>) = h.MakeSvgNode("feFuncG", children)
    member _.feFuncR (children: seq<'Node>) = h.MakeSvgNode("feFuncR", children)
    member _.feGaussianBlur (children: seq<'Node>) = h.MakeSvgNode("feGaussianBlur", children)
    member _.feImage (children: seq<'Node>) = h.MakeSvgNode("feImage", children)
    member _.feMerge (children: seq<'Node>) = h.MakeSvgNode("feMerge", children)
    member _.feMergeNode (children: seq<'Node>) = h.MakeSvgNode("feMergeNode", children)
    member _.feMorphology (children: seq<'Node>) = h.MakeSvgNode("feMorphology", children)
    member _.feOffset (children: seq<'Node>) = h.MakeSvgNode("feOffset", children)
    member _.fePointLight (children: seq<'Node>) = h.MakeSvgNode("fePointLight", children)
    member _.feSpecularLighting (children: seq<'Node>) = h.MakeSvgNode("feSpecularLighting", children)
    member _.feSpotLight (children: seq<'Node>) = h.MakeSvgNode("feSpotLight", children)
    member _.feTile (children: seq<'Node>) = h.MakeSvgNode("feTile", children)
    member _.feTurbulence (children: seq<'Node>) = h.MakeSvgNode("feTurbulence", children)
    member _.filter (children: seq<'Node>) = h.MakeSvgNode("filter", children)
    member _.foreignObject (children: seq<'Node>) = h.MakeSvgNode("foreignObject", children)
    /// The <g> SVG element is a container used to group other SVG elements.
    ///
    /// Transformations applied to the <g> element are performed on its child elements, and its attributes are inherited by its children. It can also group multiple elements to be referenced later with the <use> element.
    member _.g (children: seq<'Node>) = h.MakeSvgNode("g", children)
    member _.line (children: seq<'Node>) = h.MakeSvgNode("line", children)
    member _.linearGradient (children: seq<'Node>) = h.MakeSvgNode("linearGradient", children)
    /// The <marker> element defines the graphic that is to be used for drawing arrowheads or polymarkers on a given <path>, <line>, <polyline> or <polygon> element.
    member _.marker (children: seq<'Node>) = h.MakeSvgNode("marker", children)
    member _.mask (children: seq<'Node>) = h.MakeSvgNode("marker", children)
    member _.mpath (children: seq<'Node>) = h.MakeSvgNode("mpath", children)
    member _.path (children: seq<'Node>) = h.MakeSvgNode("path", children)
    member _.pattern (children: seq<'Node>) = h.MakeSvgNode("pattern", children)
    member _.polygon (children: seq<'Node>) = h.MakeSvgNode("polygon", children)
    member _.polyline (children: seq<'Node>) = h.MakeSvgNode("polyline", children)
    member _.set (children: seq<'Node>) = h.MakeSvgNode("set", children)
    member _.stop (children: seq<'Node>) = h.MakeSvgNode("stop", children)
    member _.style (value: string) = h.MakeSvgNode("style", value)
    member _.switch (children: seq<'Node>) = h.MakeSvgNode("switch", children)
    member _.symbol (children: seq<'Node>) = h.MakeSvgNode("symbol", children)
    member _.text (content: string) = h.MakeSvgNode("text", content)
    member _.title (content: string) = h.MakeSvgNode("title", content)
    member _.textPath (children: seq<'Node>) = h.MakeSvgNode("textPath", children)
    member _.tspan (children: seq<'Node>) = h.MakeSvgNode("tspan", children)
    member _.use' (children: seq<'Node>) = h.MakeSvgNode("use", children)
    member _.radialGradient (children: seq<'Node>) = h.MakeSvgNode("radialGradient", children)
    member _.rect (children: seq<'Node>) = h.MakeSvgNode("rect", children)
    member _.view (children: seq<'Node>) = h.MakeSvgNode("view", children)
