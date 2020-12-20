module Sveltish.DOM
open Browser.Dom
open Browser.Types

let log = Logging.log "dom"

type BuildContext = {
    StyleName : string
    AppendChild: (Node -> Node -> Node)
    ReplaceChild: (Node -> Node -> Node -> Node)
    SetAttribute: (Element->string->string->unit)
    MakeName : (string -> string)
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

type NodeFactory = (BuildContext * HTMLElement) -> Node

let appendAttribute (e:Element) attrName attrValue =
    if (attrValue <> "") then
        let currentValue = e.getAttribute(attrName)
        e.setAttribute(attrName,
            if ((isNull currentValue) || currentValue = "")
                then attrValue
                else (sprintf "%s %s" currentValue attrValue))

let el tag (xs : seq<NodeFactory>) : NodeFactory = fun (ctx,parent) ->
    let e  = document.createElement tag

    ctx.AppendChild (parent:>Node) (e:>Node) |> ignore

    for x in xs do x(ctx,e) |> ignore

    if ctx.StyleName <> "" then
        appendAttribute e "class" ctx.StyleName

    e :> Node

let inline castNodeToElement (node : Node) : Element = node :?> Element

let attr (name,value) : NodeFactory = fun (ctx,e) ->
    ctx.SetAttribute (castNodeToElement e) name value
    e :> Node

let text value : NodeFactory =
    fun (ctx,e) -> ctx.AppendChild (e:>Node) (document.createTextNode(value) :> Node)

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement selector = document.querySelector(selector)

let rec mountElement selector (app : NodeFactory)  =
    let host = idSelector selector |> findElement :?> HTMLElement
    (app (makeContext,host)) |> ignore
