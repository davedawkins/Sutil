namespace Feliz

open System

type SvgHelper<'Node> =
    abstract MakeNode: string * 'Node seq -> 'Node
    abstract StringToNode: string -> 'Node
    abstract EmptyNode: 'Node

module internal SvgHelperExtensions =
    type SvgHelper<'Node> with
        member this.MakeNode(tag: string, v: string) =
            this.MakeNode(tag, [this.StringToNode v])

open SvgHelperExtensions

type SvgEngine<'Node>(h: SvgHelper<'Node>) =
    /// Create a custom element
    ///
    /// You generally shouldn't need to use this, if you notice an element missing please submit an issue.
    member _.custom (key: string, children: seq<'Node>) = h.MakeNode(key, children)
    /// The empty element, renders nothing on screen
    member _.none : 'Node = h.EmptyNode

    /// SVG Image element, not to be confused with HTML img alias.
    member _.image (children: seq<'Node>) = h.MakeNode("image", children)
    /// The svg element is a container that defines a new coordinate system and viewport. It is used as the outermost element of SVG documents, but it can also be used to embed an SVG fragment inside an SVG or HTML document.
    member _.svg (children: seq<'Node>) = h.MakeNode("svg", children)
    member _.circle (children: seq<'Node>) = h.MakeNode("circle", children)
    member _.clipPath (children: seq<'Node>) = h.MakeNode("clipPath", children)
    /// The <defs> element is used to store graphical objects that will be used at a later time. Objects created inside a <defs> element are not rendered directly. To display them you have to reference them (with a <use> element for example). https://developer.mozilla.org/en-US/docs/Web/SVG/Element/defs
    member _.defs (children: seq<'Node>) = h.MakeNode("defs", children)
    /// The <desc> element provides an accessible, long-text description of any SVG container element or graphics element.
    member _.desc (value: string) = h.MakeNode("desc", value)
    member _.ellipse (children: seq<'Node>) = h.MakeNode("ellipse", children)
    member _.feBlend (children: seq<'Node>) = h.MakeNode("feBlend", children)
    member _.feColorMatrix (children: seq<'Node>) = h.MakeNode("feColorMatrix", children)
    member _.feComponentTransfer (children: seq<'Node>) = h.MakeNode("feComponentTransfer", children)
    member _.feComposite (children: seq<'Node>) = h.MakeNode("feComposite", children)
    member _.feConvolveMatrix (children: seq<'Node>) = h.MakeNode("feConvolveMatrix", children)
    member _.feDiffuseLighting (children: seq<'Node>) = h.MakeNode("feDiffuseLighting", children)
    member _.feDisplacementMap (children: seq<'Node>) = h.MakeNode("feDisplacementMap", children)
    member _.feDistantLight (children: seq<'Node>) = h.MakeNode("feDistantLight", children)
    member _.feDropShadow (children: seq<'Node>) = h.MakeNode("feDropShadow", children)
    member _.feFlood (children: seq<'Node>) = h.MakeNode("feFlood", children)
    member _.feFuncA (children: seq<'Node>) = h.MakeNode("feFuncA", children)
    member _.feFuncB (children: seq<'Node>) = h.MakeNode("feFuncB", children)
    member _.feFuncG (children: seq<'Node>) = h.MakeNode("feFuncG", children)
    member _.feFuncR (children: seq<'Node>) = h.MakeNode("feFuncR", children)
    member _.feGaussianBlur (children: seq<'Node>) = h.MakeNode("feGaussianBlur", children)
    member _.feImage (children: seq<'Node>) = h.MakeNode("feImage", children)
    member _.feMerge (children: seq<'Node>) = h.MakeNode("feMerge", children)
    member _.feMergeNode (children: seq<'Node>) = h.MakeNode("feMergeNode", children)
    member _.feMorphology (children: seq<'Node>) = h.MakeNode("feMorphology", children)
    member _.feOffset (children: seq<'Node>) = h.MakeNode("feOffset", children)
    member _.fePointLight (children: seq<'Node>) = h.MakeNode("fePointLight", children)
    member _.feSpecularLighting (children: seq<'Node>) = h.MakeNode("feSpecularLighting", children)
    member _.feSpotLight (children: seq<'Node>) = h.MakeNode("feSpotLight", children)
    member _.feTile (children: seq<'Node>) = h.MakeNode("feTile", children)
    member _.feTurbulence (children: seq<'Node>) = h.MakeNode("feTurbulence", children)
    member _.filter (children: seq<'Node>) = h.MakeNode("filter", children)
    member _.foreignObject (children: seq<'Node>) = h.MakeNode("foreignObject", children)
    /// The <g> SVG element is a container used to group other SVG elements.
    ///
    /// Transformations applied to the <g> element are performed on its child elements, and its attributes are inherited by its children. It can also group multiple elements to be referenced later with the <use> element.
    member _.g (children: seq<'Node>) = h.MakeNode("g", children)
    member _.line (children: seq<'Node>) = h.MakeNode("line", children)
    member _.linearGradient (children: seq<'Node>) = h.MakeNode("linearGradient", children)
    /// The <marker> element defines the graphic that is to be used for drawing arrowheads or polymarkers on a given <path>, <line>, <polyline> or <polygon> element.
    member _.marker (children: seq<'Node>) = h.MakeNode("marker", children)
    member _.mask (children: seq<'Node>) = h.MakeNode("marker", children)
    member _.mpath (children: seq<'Node>) = h.MakeNode("mpath", children)
    member _.path (children: seq<'Node>) = h.MakeNode("path", children)
    member _.pattern (children: seq<'Node>) = h.MakeNode("pattern", children)
    member _.polygon (children: seq<'Node>) = h.MakeNode("polygon", children)
    member _.polyline (children: seq<'Node>) = h.MakeNode("polyline", children)
    member _.set (children: seq<'Node>) = h.MakeNode("set", children)
    member _.stop (children: seq<'Node>) = h.MakeNode("stop", children)
    member _.style (value: string) = h.MakeNode("style", value)
    member _.switch (children: seq<'Node>) = h.MakeNode("switch", children)
    member _.symbol (children: seq<'Node>) = h.MakeNode("symbol", children)
    member _.text (content: string) = h.MakeNode("text", content)
    member _.title (content: string) = h.MakeNode("title", content)
    member _.textPath (children: seq<'Node>) = h.MakeNode("textPath", children)
    member _.tspan (children: seq<'Node>) = h.MakeNode("tspan", children)
    member _.use' (children: seq<'Node>) = h.MakeNode("use", children)
    member _.radialGradient (children: seq<'Node>) = h.MakeNode("radialGradient", children)
    member _.rect (children: seq<'Node>) = h.MakeNode("rect", children)
    member _.view (children: seq<'Node>) = h.MakeNode("view", children)
