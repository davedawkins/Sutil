module Sveltish.DOM

open Browser.Dom
open Browser.Types
open Browser.CssExtensions

let log = Logging.log "dom"


//
// Required to inflate a NodeFactory into a real DOM element
//
// StyleName. If <> "" then a class will be added to each element that keys
//            a set of CSS rules. See the `style` function
// MakeName.  A helper that can generate unique names given base name
//
// AppendChild/ReplaceChild/SetAttribute
//            Abstractions on the equivalent document methods. Defaults
//            are set to the document methods, but particular NodeFactory
//            functions may override these in the context they pass to their
//            children so that behaviour can be customized.
//
type BuildContext = {
    MakeName : (string -> string)
    StyleName : string
    AppendChild: (Node -> Node -> Node)
    ReplaceChild: (Node -> Node -> Node -> Node)
    SetAttribute: (Element->string->string->unit)
}

let makeContext =
    let gen = CodeGeneration.makeIdGenerator()
    {
        StyleName = ""
        AppendChild = (fun parent child -> parent.appendChild(child))
        ReplaceChild = (fun parent newChild oldChild -> parent.replaceChild(newChild, oldChild))
        SetAttribute = (fun parent name value -> parent.setAttribute(name,value))
        MakeName = fun baseName -> sprintf "%s-%d" baseName (gen())
    }

//
// Basic building block for documents
// The arguments to the factory are a context and the element's parent. If the
// factory makes an element, then this will be the return value. If the factory
// operates on the parent node, then the parent node is returned. For example, setting
// attribute.
//
type NodeFactory = (BuildContext * Node) -> Node

let appendAttribute (e:Element) attrName attrValue =
    if (attrValue <> "") then
        let currentValue = e.getAttribute(attrName)
        e.setAttribute(attrName,
            if ((isNull currentValue) || currentValue = "")
                then attrValue
                else (sprintf "%s %s" currentValue attrValue))

let el tag (xs : seq<NodeFactory>) : NodeFactory = fun (ctx,parent) ->
    let e  = document.createElement tag

    ctx.AppendChild parent (e:>Node) |> ignore

    for x in xs do x(ctx,e) |> ignore

    if ctx.StyleName <> "" then
        appendAttribute e "class" ctx.StyleName

    e :> Node

let inline attr (name,value:obj) : NodeFactory = fun (ctx,e) ->
    try
        ctx.SetAttribute (e :?> Element) name (string value) // Cannot type test on Element
    with _ -> invalidOp (sprintf "Cannot set attribute %s on a %A" name e)
    e

let text value : NodeFactory =
    fun (ctx,e) -> ctx.AppendChild e (document.createTextNode(value) :> Node)

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement selector = document.querySelector(selector)

//
// Mount a top-level application NodeFactory into an existing document
//
let rec mountElement selector (app : NodeFactory)  =
    let host = idSelector selector |> findElement :?> HTMLElement
    (app (makeContext,host)) |> ignore


let addTransform (node:HTMLElement) (a : ClientRect) =
    let b = node.getBoundingClientRect()
    if (a.left <> b.left || a.top <> b.top) then
        let s = window.getComputedStyle(node)
        let transform = if s.transform = "none" then "" else s.transform
        node.style.transform <- sprintf "%s translate(%fpx, %fpx)" "" (a.left - b.left) (a.top - b.top)
        log node.style.transform

let fixPosition (node:HTMLElement) =
    let s = window.getComputedStyle(node)
    if (s.position <> "absolute" && s.position <> "fixed") then
        let width  = s.width
        let height = s.height
        let a = node.getBoundingClientRect()
        node.style.position <- "absolute"
        node.style.width <- width
        node.style.height <- height
        addTransform node a

let asEl (node : Node) = (node :?> HTMLElement)

let clientRect el = (asEl el).getBoundingClientRect()

let removeNode (node:#Node) =
    log <| sprintf "removing node %A" node.textContent
    node.parentNode.removeChild( node ) |> ignore
