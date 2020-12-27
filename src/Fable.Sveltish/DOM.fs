module Sveltish.DOM

open Browser.Dom
open Browser.Types
open Browser.CssExtensions

let log = Logging.log "dom"

module Event =
    let ElementReady = "sveltish-element-ready"
    let Show = "sveltish-show"
    let Hide = "sveltish-hide"
    let Updated = "sveltish-updated"

type CssSelector =
    | Tag of string
    | Cls of string
    | Id of string
    | All of CssSelector list
    | Any of CssSelector list
    | Attr of CssSelector * string * string
    | NotImplemented
    with
    member this.Match (el:HTMLElement)=
        match this with
        | NotImplemented -> false
        | Tag tag -> el.tagName = tag
        | Cls cls -> el.classList.contains(cls)
        | Id id -> el.id = id
        | Attr (sub,name,value) -> sub.Match(el) && el.getAttribute(name) = value
        | All rules -> rules |> List.fold (fun a r -> a && r.Match el) true
        | Any rules -> rules |> List.fold (fun a r -> a || r.Match el) false


type StyleRule = {
    SelectorSpec : string
    Selector : CssSelector
    Style : (string*obj) list
}

type StyleSheet = StyleRule list

type NamedStyleSheet = {
    Name : string
    StyleSheet : StyleSheet
}

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
    StyleSheet : NamedStyleSheet option
    AppendChild: (Node -> Node -> Node)
    ReplaceChild: (Node -> Node -> Node -> Node)
    SetAttribute: (Element->string->string->unit)
}

let makeContext =
    let gen = CodeGeneration.makeIdGenerator()
    {
        StyleSheet = None
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

// TODO: We can make a better parser using combinators. This lets me prove this idea tbough
// Don't judge me
let rec parseSelector (source:string) : CssSelector =
    let trimQuotes (s:string) = s.Trim().Trim( [| '\''; '"' |])

    let rec parseSingle (token : string) =
        if token.StartsWith(".")
            then Cls (token.Substring(1))
        else if token.StartsWith("#")
            then Id (token.Substring(1))
        else if token.Contains(":") || token.Contains(">") || token.Contains("[")
            then NotImplemented
        else
            Tag (token.ToUpper())

    let rec parseAttr (token : string) =
        if token.Contains("[") && token.EndsWith("]")
            then
                let i = token.IndexOf('[')
                let single = parseSingle(token.Substring(0,i).Trim())
                let attrExpr = token.Substring(i+1, token.Length - i - 2)
                let attrTokens = attrExpr.Split([|'='|], 2)
                if attrTokens.Length = 2 then
                    Attr (single, attrTokens.[0].Trim(), attrTokens.[1] |> trimQuotes )
                else
                    NotImplemented
            else parseSingle token

    let rec parseAll (token : string) =
        let spacedItems = token.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
        if (spacedItems.Length = 1)
            then parseAttr spacedItems.[0]
            else spacedItems |> Array.map parseAttr |> Array.toList |> Any

    let items = source.Split(',')
    if items.Length = 1
        then parseAll items.[0]
        else items |> Array.map parseAll |> Array.toList |> All

let ruleMatchEl (el:HTMLElement) (rule:StyleRule) =
    rule.Selector.Match el

let applyCustomRules e sheet =
    // TODO: Remove all classes added by previous calls to this function
    // TODO: Store them in a custom attribute on 'e'
    for rule in sheet |> List.filter (ruleMatchEl e) do
        for custom in rule.Style |> List.filter (fun (nm,v) -> nm.StartsWith("sveltish")) do
            match custom with
            | (nm,v) when nm = "sveltish-add-class" ->
                log($"Matches: {e.tagName} '%A{e.classList}' -> %A{rule.Selector}")
                log($"Adding class {v}")
                e.classList.add(string v)
                // TODO: Also add this class to a custom attribute so we can clean them up
                // TODO: on subsequent calls
            | _ -> log($"Unimplemented: {fst custom}")

let el tag (xs : seq<NodeFactory>) : NodeFactory = fun (ctx,parent) ->
    let e  = document.createElement tag

    ctx.AppendChild parent (e:>Node) |> ignore

    for x in xs do x(ctx,e) |> ignore

    match ctx.StyleSheet with
    | Some { Name = name; StyleSheet = sheet } ->
        e.classList.add(name)
        applyCustomRules e sheet
    | None -> ()

    e.dispatchEvent( Interop.customEvent Event.ElementReady {| |}) |> ignore

    e :> Node

let inline attr (name,value:obj) : NodeFactory = fun (ctx,e) ->
    try
        ctx.SetAttribute (e :?> Element) name (string value) // Cannot type test on Element
        if (name = "value") then
            Interop.set e "__value" value
        match ctx.StyleSheet with
        | Some { Name = _; StyleSheet = sheet } ->
            applyCustomRules (e :?> HTMLElement) sheet
        | None -> ()

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

let findChildWhere (node:Node) (f : Node -> bool) =
    let rec search (n : Node) (f : Node -> bool) =
        if isNull n then
            null
        else
            if (f n) then n else search n.nextSibling f
    search node.firstChild f

let children (node:Node) =
    let rec visit (child:Node) : Node list=
        match child with
        | null -> []
        | x -> x :: (visit child.nextSibling)
    visit node.firstChild

let addTransform (node:HTMLElement) (a : ClientRect) =
    let b = node.getBoundingClientRect()
    if (a.left <> b.left || a.top <> b.top) then
        let s = window.getComputedStyle(node)
        let transform = if s.transform = "none" then "" else s.transform
        node.style.transform <- sprintf "%s translate(%fpx, %fpx)" transform (a.left - b.left) (a.top - b.top)
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

let fragment (elements : NodeFactory list) = fun (ctx,parent) ->
    let mutable last : Node = null
    for e in elements do
        last <- e(ctx,parent)
    last