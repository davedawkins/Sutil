module Sveltish.DOM

open System
open Browser.Dom
open Browser.Types
open Browser.CssExtensions

let log = Logging.log "dom"

let domId = Helpers.makeIdGenerator()

let isTextNode (n:Node) = n.nodeType = 3.0
let isElementNode (n:Node) = n.nodeType = 1.0

let SvIdKey = "_svid"

let setSvId (n:Node) id =
    Interop.set n SvIdKey id
    if (isElementNode n) then
        (n :?> HTMLElement).setAttribute(SvIdKey,(string id))

let svId (n:Node) = Interop.get n SvIdKey

let hasSvId (n:Node) = Interop.exists n SvIdKey

module Event =
    let ElementReady = "sveltish-element-ready"
    let Show = "sveltish-show"
    let Hide = "sveltish-hide"
    let Updated = "sveltish-updated"
    let NewStore = "sveltish-new-store"
    let DisposeStore = "sveltish-dispose-store"

    let notifyEvent name data =
        document.dispatchEvent( Interop.customEvent name data ) |> ignore

    let notifyUpdated() =
        log("notify document")
        notifyEvent Updated  {|  |}

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
    Parent : NamedStyleSheet option
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
    let gen = Helpers.makeIdGenerator()
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

type Fragment = Node list
type FactoryArgs = BuildContext * Node

type IFactory = interface
    abstract BuildFragment: args:FactoryArgs -> Fragment
    end

[<AbstractClass>]
type Factory<'T>(f : FactoryArgs -> 'T) =
    abstract BuildFragment: args: FactoryArgs -> Fragment
    interface IFactory with
        member this.BuildFragment(args: FactoryArgs) = this.BuildFragment(args)

type NodeFactory(f : FactoryArgs -> Node) =
    inherit Factory<Node>(f)
    member _.BuildElement (args: FactoryArgs) =
        args |> f
    override _.BuildFragment (args: FactoryArgs) =
        [ args |> f ]

type FragmentFactory(f : FactoryArgs -> Fragment) =
    inherit Factory<Fragment>(f)
    override _.BuildFragment (args: FactoryArgs) =
        args |> f

type UnitFactory(f : FactoryArgs -> unit) =
    inherit Factory<unit>(f)
    override _.BuildFragment (args: FactoryArgs) =
        args |> f
        []

#if false
let nodeFactory f : IFactory = upcast (f |> NodeFactory)
let fragmentFactory f : IFactory = upcast (f |> FragmentFactory)
let unitFactory f : IFactory = upcast (UnitFactory(f))
#else
let nodeFactory f  =  (f |> NodeFactory)
let fragmentFactory f  =  (f |> FragmentFactory)
let unitFactory f  =  (UnitFactory(f))
#endif

#if !NO_HACKS
//let seqIFactory (xs:#seq<NodeFactory>) : seq<IFactory> = xs :> obj :?> seq<IFactory>
let seqIFactory (xs:#seq<NodeFactory>) : seq<IFactory> = xs |> Seq.map (fun x -> x :> IFactory)
#endif

// Abstractions that help with type checking and type signatures as shown by the editor
// I would hope the identity functions are optimized away
let nodeResult (node:Node) : Node = node
let nodeFragment (nodes : Node list) : Fragment = nodes

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

let rec rootStyle sheet =
    match sheet.Parent with
    | None -> sheet
    | Some parentSheet -> rootStyle parentSheet

let rec rootStyleName sheet =
    (rootStyle sheet).Name

let getSveltishClasses (e:HTMLElement) =
    let classes =
        [0..e.classList.length-1]
            |> List.map (fun i -> e.classList.[i])
            |> List.filter (fun cls -> cls.StartsWith("sveltish"));
    classes

let rec applyCustomRules e (namedSheet:NamedStyleSheet) =
    // TODO: Remove all classes added by previous calls to this function
    // TODO: Store them in a custom attribute on 'e'
    let sheet = namedSheet.StyleSheet
    for rule in sheet |> List.filter (ruleMatchEl e) do
        for custom in rule.Style |> List.filter (fun (nm,v) -> nm.StartsWith("sveltish")) do
            match custom with
            | (nm,v) when nm = "sveltish-use-global" ->
                let root = rootStyle namedSheet
                if root.Name <> namedSheet.Name then
                    e.classList.add(root.Name)
                    applyCustomRules e root
                ()
            | (nm,v) when nm = "sveltish-use-parent" ->
                ()
            | (nm,v) when nm = "sveltish-add-class" ->
                //log($"Matches: {e.tagName} '%A{e.classList}' -> %A{rule.Selector}")
                //log($"Adding class {v}")
                e.classList.add(string v)
                // TODO: Also add this class to a custom attribute so we can clean them up
                // TODO: on subsequent calls
            | _ -> log($"Unimplemented: {fst custom}")

let el tag (xs : #seq<IFactory>) = nodeFactory <| fun (ctx,parent) ->
    let e  = document.createElement tag

    setSvId e (domId())

    ctx.AppendChild parent (e:>Node) |> ignore

    for x in xs do
        x.BuildFragment(ctx,upcast e) |> ignore

    match ctx.StyleSheet with
    | Some namedSheet ->
        e.classList.add(namedSheet.Name)
        applyCustomRules e namedSheet
    | None -> ()

    e.dispatchEvent( Interop.customEvent Event.ElementReady {| |}) |> ignore

    e :> Node |> nodeResult

let findSvIdElement id : HTMLElement =
    downcast document.querySelector($"[_svid='{id}']")

let inline attr (name,value:obj) = unitFactory <| fun (ctx,e) ->
    try
        ctx.SetAttribute (e :?> Element) name (string value) // Cannot type test on Element
        if (name = "value") then
            Interop.set e "__value" value
        match ctx.StyleSheet with
        | Some namedSheet ->
            applyCustomRules (e :?> HTMLElement) namedSheet
        | None -> ()

    with _ -> invalidOp (sprintf "Cannot set attribute %s on a %A" name e)
    ()

let textNode value : Node =
    let n = document.createTextNode(value)
    setSvId n (domId())
    upcast n

let text value =
    nodeFactory <| fun (ctx,e) -> ctx.AppendChild e (textNode value) |> nodeResult

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement selector = document.querySelector(selector)

let rec visitChildren (parent:Node) (f : Node -> bool) =
    let mutable child = parent.firstChild
    while not (isNull child) do
        if f(child) then
            visitChildren (downcast child) f
            child <- child.nextSibling
        else
            child <- null

let rec findNode<'T> (parent:Node) (f : Node -> 'T option)  : 'T option=
    let mutable child = parent.firstChild
    let mutable result : 'T option = None
    while not (isNull child) do
        result <- f(child)
        if (result.IsNone) then result <- findNode child f
        child <- match result with
                    | None -> child.nextSibling
                    | Some x -> null
    result

let rec visitElementChildren (parent:Node) (f : HTMLElement -> unit) =
    visitChildren parent
                    (fun child ->
                        if (child.nodeType = 1.0) then f(downcast child)
                        true)

let findNodeWithSvId id =
    let getId n =
        let r = svId n
        if (r = id) then Some n else None
    findNode document.body getId

let htmlCollectionToSeq<'T when 'T :> Node> (coll:HTMLCollection) =
    seq {
        for i in [0..coll.length-1] do yield (coll.[i] :> obj :?> 'T)
    }

let html text = fragmentFactory <| fun (ctx,parent) ->
    let el = parent :?> HTMLElement
    el.innerHTML <- text

    match ctx.StyleSheet with
    | None -> ()
    | Some ns -> visitElementChildren el (fun ch ->
                                        ch.classList.add ns.Name
                                        applyCustomRules ch ns)

    el.children |> htmlCollectionToSeq<Node> |> Seq.toList

//
// Mount a top-level application NodeFactory into an existing document
//
let rec mountElement selector (app : IFactory)  =
    let host = idSelector selector |> findElement :?> HTMLElement
    (app.BuildFragment(makeContext,upcast host)) |> ignore

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

let removeNode (node:Node) =
    log <| sprintf "removing node %A" node.textContent
    node.parentNode.removeChild( node ) |> ignore

let fragment (elements : #seq<IFactory>) = fragmentFactory <| fun ctxParent ->
    elements |> Seq.collect (fun e -> e.BuildFragment ctxParent) |> Seq.toList |> nodeFragment

let isCrossOrigin = false // TODO

let listen (event:string) (e:EventTarget) (fn: (Event -> unit)) : (unit -> unit)=
    e.addEventListener( event, fn )
    (fun () -> e.removeEventListener(event, fn ) |> ignore)

let listenOneShot (event:string) (target:EventTarget) (fn : Unit->Unit) : (unit -> unit) =
    let rec inner _ = target.removeEventListener( event, inner ); fn()
    listen event target inner

type private ResizeSubscriber = {
    Callback: unit -> unit
    Id : int
}

// Ported from Svelte
type ResizeObserver( el : HTMLElement ) =
    let mutable iframe : HTMLIFrameElement = Unchecked.defaultof<_>
    let mutable subId = 0
    let mutable unsubscribe : (unit -> unit) = Unchecked.defaultof<_>

    let mutable subscribers = []

    let notify _ =
        subscribers |> List.iter (fun sub -> sub.Callback())
    do
        let computedStyle = window.getComputedStyle(el)
        let zIndex =  (try int(computedStyle.zIndex) with |_ -> 0) - 1;
        if computedStyle.position = "static" then
            el.style.position <- "relative"

        iframe <- downcast el.ownerDocument.createElement("iframe")
        let style = sprintf "%sz-index: %i;" "display: block; position: absolute; top: 0; left: 0; width: 100%; height: 100%; overflow: hidden; border: 0; opacity: 0; pointer-events: none;" zIndex
        iframe.setAttribute("style", style)
        iframe.setAttribute("aria-hidden", "true")
        iframe.setAttribute("tabindex", "-1")

        if isCrossOrigin then
            iframe.setAttribute("src", "data:text/html,<script>onresize=function(){parent.postMessage(0,'*')}</script>")

            unsubscribe <- listen "message" window
                (fun e -> if Helpers.fastEquals (Interop.get e "source") iframe.contentWindow then notify(e))
        else
            iframe.setAttribute("src", "about:blank")
            iframe.onload <- (fun e ->
                unsubscribe <- listen "resize" iframe.contentWindow notify)

        el.appendChild(iframe) |> ignore

    member _.Subscribe(callback : (unit -> unit)) =
        let sub = { Callback = callback; Id = subId }
        subId <- subId + 1
        subscribers <- sub :: subscribers
        Helpers.disposable <| fun () -> subscribers <- subscribers |> List.filter (fun s -> s.Id <> sub.Id)

    member _.Dispose() =
        try unsubscribe() with |_ -> ()
        if not (isNull iframe) then
            removeNode iframe

    interface IDisposable with
        member this.Dispose() = this.Dispose()

[<RequireQualifiedAccessAttribute>]
module NodeKey =
    let Disposables = "__sveltish_disposables"
    let ResizeObserver = "__sveltish_resizeObserver"

    let get<'T> (node:Node) key : 'T option  =
        let v : obj = Interop.get node key
        if isNull v then None else v :?> 'T |> Some

    let getCreate<'T> (node:Node) key (cons:unit -> 'T): 'T =
        match get node key with
        | Some v -> v
        | None ->
            let newVal = cons()
            Interop.set node key newVal
            newVal

let registerUnsubscribe (node:Node) (d:unit->unit) : unit =
    let disposables : List<unit->unit> = NodeKey.getCreate node NodeKey.Disposables (fun () -> [])
    Interop.set node NodeKey.Disposables (d :: disposables)

let registerDisposable (node:Node) (d:IDisposable) : unit =
    registerUnsubscribe node (fun () -> d.Dispose())

let hasDisposables (node:Node) : bool =
    Interop.exists node NodeKey.Disposables

let getResizer (el:HTMLElement) : ResizeObserver =
    NodeKey.getCreate el NodeKey.ResizeObserver (fun () -> new ResizeObserver(el))

let updateCustom (el:HTMLElement) (name:string) (property:string) (value:obj) =
    let r = NodeKey.getCreate el name (fun () -> {| |})
    Interop.set r property value
    Interop.set el name r