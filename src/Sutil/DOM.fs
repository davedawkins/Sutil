module Sutil.DOM

open System
open Browser.Dom
open Browser.Types
open Browser.CssExtensions

let log = Logging.log "dom"

let domId = Helpers.makeIdGenerator()

[<Literal>]
let ElementNodeType = 1.0

[<Literal>]
let TextNodeType = 3.0

let isTextNode (n:Node) = n.nodeType = TextNodeType
let isElementNode (n:Node) = n.nodeType = ElementNodeType

let documentOf (n:Node) = n.ownerDocument

let SvIdKey = "_svid"

let setSvId (n:Node) id =
    Interop.set n SvIdKey id
    if (isElementNode n) then
        (n :?> HTMLElement).setAttribute(SvIdKey,(string id))

let svId (n:Node) = Interop.get n SvIdKey

let hasSvId (n:Node) = Interop.exists n SvIdKey

let rectStr (r:ClientRect) = $"{r.left},{r.top} -> {r.right},{r.bottom}"

let nodeStr (node : Node) =
    if isNull node then
        "null"
    else
        let mutable tc = node.textContent
        if  tc.Length > 80 then tc <- tc.Substring(0,80)
        match node.nodeType with
        | ElementNodeType ->
            let e = node :?> HTMLElement
            $"<{e.tagName.ToLower()}>#{svId node} \"{tc}\""
        | TextNodeType ->
            $"\"{tc}\"#{svId node}"
        | _ -> $"?'{tc}'#{svId node}"


module Event =
    let NewStore = "sutil-new-store"
    let UpdateStore = "sutil-update-store"
    let ElementReady = "sutil-element-ready"
    let Mount = "sutil-mount"
    let Unmount = "sutil-unmount"
    let Show = "sutil-show"
    let Hide = "sutil-hide"
    let Updated = "sutil-updated"
    //let NewStore = "sutil-new-store"
    //let DisposeStore = "sutil-dispose-store"

    let notifyEvent (doc : Document) name data =
        doc.dispatchEvent( Interop.customEvent name data ) |> ignore

    let notifyUpdated doc =
        log("notify document")
        notifyEvent doc Updated  {|  |}

let listen (event:string) (e:EventTarget) (fn: (Event -> unit)) : (unit -> unit)=
    e.addEventListener( event, fn )
    (fun () -> e.removeEventListener(event, fn) |> ignore)

let raf (f : float -> unit) = window.requestAnimationFrame( fun t -> try f t with|x -> Logging.error $"raf: {x.Message}" )
let rafu (f : unit -> unit) = window.requestAnimationFrame( fun _ -> try f() with|x -> Logging.error $"rafu: {x.Message}" ) |> ignore

let once (event:string) (target:EventTarget) (fn : Event->Unit) : unit =
    let rec inner e = target.removeEventListener( event, inner ); fn(e)
    listen event target inner |> ignore

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

type INode = interface
    abstract Remove: unit -> unit
    abstract Append: Node -> unit
    abstract Replace: Node -> Node -> unit
    abstract Node: Node
    end

type NodeRef =
    | RealNode of Node
    | VirtualNode of INode
    with
        member this.Node =
            match this with
            | RealNode n -> n
            | VirtualNode n -> n.Node
        member this.Remove () =
            match this with
            | RealNode n -> n.parentNode.removeChild(n) |> ignore
            | VirtualNode n -> n.Remove()
        member this.Append (parent : Node) =
            match this with
            | RealNode n -> parent.appendChild(n) |> ignore
            | VirtualNode n -> n.Append(parent)
        member this.Replace (parent : Node, newChild:Node) =
            match this with
            | RealNode n -> parent.replaceChild(n,newChild) |> ignore
            | VirtualNode n -> n.Replace parent newChild

type BuildResult =
    | Unit
    | Solitary of Node
    | Fragment of Node list
    | Binding of NodeRef

type DomAction =
    | Append
    | Replace of Node
    | After of Node
    | Before of Node

type BuildContext =
    {
        // Parent / replace context
        Parent : Node
        Action  : DomAction  // Consider making this "SvId option" and then finding node to replace

        // Naming service
        MakeName : (string -> string)

        // Style context
        StyleSheet : NamedStyleSheet option

        // IDom
        AppendChild: (Node -> Node -> Node)
        ReplaceChild: (Node -> Node -> Node -> Node)
        SetAttribute: (Element->string->string->unit) }
    with
        member this.Document = documentOf this.Parent
        member this.ParentElement : HTMLElement = downcast this.Parent
        //member this.Create (f : NodeFactory) = f this

// Private so that we must use build to instantiate the DOM fragment.
type NodeFactory = private { Builder: BuildContext -> BuildResult }

let nodeFactory f = { Builder = f }

let private makeContext parent =
    let gen = Helpers.makeIdGenerator()
    {
        Parent = parent
        Action = Append
        StyleSheet = None
        AppendChild = (fun parent child -> parent.appendChild(child))
        ReplaceChild = (fun parent newChild oldChild -> parent.replaceChild(newChild, oldChild))
        SetAttribute = (fun parent name value -> parent.setAttribute(name,value))
        MakeName = fun baseName -> sprintf "%s-%d" baseName (gen())
    }

module ContextHelpers =
    let withStyleSheet sheet ctx : BuildContext =
        { ctx with StyleSheet = Some sheet }

    let withParent parent ctx =
        { ctx with Parent = parent; Action = Append }

    let withReplace toReplace ctx =
        { ctx with Action = if isNull toReplace then Append else Replace toReplace }

    let withAfter after ctx =
        { ctx with Action = After after }

    let withBefore before ctx =
        { ctx with Action = Before before }

type Fragment = Node list

let nodeResult (node:Node) = Solitary node
let fragmentResult (nodes:Node list) = Fragment nodes
let unitResult()  = Unit
let bindResult r = Binding r

let errorNode (parent:Node) message : Node=
    let doc = documentOf parent
    let d = doc.createElement("div")
    d.appendChild(doc.createTextNode($"sutil-error: {message}")) |> ignore
    parent.appendChild(d) |> ignore
    d.setAttribute("style", "color: red; padding: 4px; font-size: 10px;")
    upcast d

let collectFragment (result : BuildResult) =
    match result with
    | Solitary n -> [ n ]
    | Binding b -> [ b.Node ] // TODO: Hmm..
    | Unit -> []
    | Fragment xs -> xs

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

let rec rootStyle (sheet : NamedStyleSheet) =
    match sheet.Parent with
    | None -> sheet
    | Some parentSheet -> rootStyle parentSheet

let rec rootStyleName sheet =
    (rootStyle sheet).Name

let getSutilClasses (e:HTMLElement) =
    let classes =
        [0..e.classList.length-1]
            |> List.map (fun i -> e.classList.[i])
            |> List.filter (fun cls -> cls.StartsWith("Sutil"));
    classes

let rec applyCustomRules e (namedSheet:NamedStyleSheet) =
    // TODO: Remove all classes added by previous calls to this function
    // TODO: Store them in a custom attribute on 'e'
    let sheet = namedSheet.StyleSheet
    for rule in sheet |> List.filter (ruleMatchEl e) do
        for custom in rule.Style |> List.filter (fun (nm,v) -> nm.StartsWith("sutil")) do
            match custom with
            | (nm,v) when nm = "sutil-use-global" ->
                let root = rootStyle namedSheet
                if root.Name <> namedSheet.Name then
                    e.classList.add(root.Name)
                    applyCustomRules e root
                ()
            | (nm,v) when nm = "sutil-use-parent" ->
                ()
            | (nm,v) when nm = "sutil-add-class" ->
                //log($"Matches: {e.tagName} '%A{e.classList}' -> %A{rule.Selector}")
                //log($"Adding class {v}")
                e.classList.add(string v)
                // TODO: Also add this class to a custom attribute so we can clean them up
                // TODO: on subsequent calls
            | _ -> log($"Unimplemented: {fst custom}")

let dispatch (node:Node) name (data:obj)=
    node.dispatchEvent( Interop.customEvent name data) |> ignore

let dispatchSimple (node:Node) name =
    dispatch node name {| |}

let build (f : NodeFactory) (ctx : BuildContext) =
    let result = f.Builder ctx
    match result with
    | Solitary n -> dispatchSimple n Event.Mount
    | Fragment ns -> ns |> List.iter (fun n -> dispatchSimple n Event.Mount)
    | _ -> ()
    result

let expectSolitary (f : NodeFactory) ctx =
    match ctx |> build f with
    | Solitary n -> n
    | Binding b -> b.Node
        //errorNode parent "Expected single node, binding found"
    | Unit ->
        errorNode ctx.Parent "Expected single node, none found"
    | Fragment xs ->
        let doc = ctx.Document
        let tmpDiv = doc.createElement("div")
        let en = errorNode tmpDiv "'fragment' not allowed as root for 'each' blocks"
        tmpDiv.appendChild en |> ignore
        ctx.Parent.appendChild tmpDiv |> ignore
        xs |> List.iter (tmpDiv.appendChild >> ignore)
        upcast tmpDiv


let buildSolitary (f : NodeFactory) ctx =
    expectSolitary f ctx

let buildChildren(xs : seq<NodeFactory>)  (ctx:BuildContext) =
    let e = ctx.ParentElement

    // Effect 2
    for x in xs do
        ctx |> build x |> ignore

    // Effect 3
    match ctx.StyleSheet with
    | Some namedSheet ->
        e.classList.add(namedSheet.Name)
        applyCustomRules e namedSheet
    | None -> ()
    unitResult()


let findSvIdElement (doc : Document) id : HTMLElement =
    downcast doc.querySelector($"[_svid='{id}']")

let splitBySpace (s:string) = s.Split([|' '|],StringSplitOptions.RemoveEmptyEntries)

let addToClasslist (e:HTMLElement) classes =
    e.classList.add( classes |> splitBySpace )

let removeFromClasslist (e:HTMLElement) classes =
    e.classList.remove( classes |> splitBySpace )

let attr (name,value:obj) : NodeFactory = nodeFactory <| fun ctx ->
    let parent = ctx.Parent
    try
        let e = ctx.Parent :?> HTMLElement

        if name = "class" then
            addToClasslist e (string value)
        else  if name = "class-" then
            removeFromClasslist e (string value)
        else
            ctx.SetAttribute (upcast e) name (string value)

        if (name = "value") then
            Interop.set e "__value" value // Un-stringified version of value

        match ctx.StyleSheet with
        | Some namedSheet ->
            applyCustomRules e namedSheet
        | None -> ()

    with _ -> invalidOp (sprintf "Cannot set attribute %s on a %A %f %s" name parent parent.nodeType (parent :?> HTMLElement).tagName)
    unitResult()

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement (doc: Document) selector = doc.querySelector(selector)

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

let prevSibling (node : Node) : Node =
    match node with |null->null|_->node.previousSibling

let rec lastSibling (node : Node) : Node =
    if (isNull node || isNull node.nextSibling) then
        node
    else
        lastSibling node.nextSibling

let lastChild (node : Node) : Node = lastSibling (node.firstChild)

let rec firstSiblingWhere (node:Node) (condition:Node -> bool) =
    if isNull node || condition node then node else firstSiblingWhere (node.nextSibling) condition

let firstChildWhere (node:Node) (condition:Node -> bool) =
    firstSiblingWhere node.firstChild condition

let rec lastSiblingWhere (node:Node) (condition:Node -> bool) =
    if  isNull node || (condition node && (isNull node.nextSibling || not (condition node.nextSibling))) then
        node
    else
        lastSiblingWhere node.nextSibling condition

let lastChildWhere (node:Node) (condition:Node->bool) =
    lastSiblingWhere node.firstChild condition

let rec visitElementChildren (parent:Node) (f : HTMLElement -> unit) =
    visitChildren parent
                    (fun child ->
                        if (child.nodeType = 1.0) then f(downcast child)
                        true)

let findNodeWithSvId (doc : Document) id =
    let getId n =
        let r = svId n
        if (r = id) then Some n else None
    findNode doc.body getId

let children (node:Node) =
    let rec visit (child:Node) =
        seq {
            if not (isNull child) then
                yield child
                yield! visit child.nextSibling
        }
    visit node.firstChild

let rec descendants (node:Node) =
    seq {
        for child in children node do
            yield child
            yield! descendants child
    }

let rec descendantsDepthFirst (node:Node) =
    seq {
        for child in children node do
            yield! descendants child
            yield child
    }

[<RequireQualifiedAccessAttribute>]
module NodeKey =
    let Disposables = "__sutil_disposables"
    let ResizeObserver = "__sutil_resizeObserver"
    let TickTask = "__sutil_tickTask"
    let Promise = "__sutil_promise"

    let clear (node:Node) (key:string) =
        Interop.delete node key

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

let registerDisposable (node:Node) (d:IDisposable) : unit =
    log $"register disposable on {nodeStr node}"
    let disposables : List<IDisposable> = NodeKey.getCreate node NodeKey.Disposables (fun () -> [])
    Interop.set node NodeKey.Disposables (d :: disposables)

let registerUnsubscribe (node:Node) (d:unit->unit) : unit =
    registerDisposable node (Helpers.disposable d)
    //let disposables : List<unit->unit> = NodeKey.getCreate node NodeKey.Disposables (fun () -> [])
    //Interop.set node NodeKey.Disposables (d :: disposables)

let disposeOnUnmount (ds : IDisposable list) = nodeFactory <| fun ctx ->
    ds |> List.iter (registerDisposable ctx.Parent)
    unitResult()

let private hasDisposables (node:Node) : bool =
    Interop.exists node NodeKey.Disposables

let private getDisposables (node:Node) : IDisposable list =
    if hasDisposables node then Interop.get node NodeKey.Disposables else []

let private clearDisposables (node:Node) : unit =
    Interop.delete node NodeKey.Disposables

let private updateCustom (el:HTMLElement) (name:string) (property:string) (value:obj) =
    let r = NodeKey.getCreate el name (fun () -> {| |})
    Interop.set r property value
    Interop.set el name r

// Call all registered disposables on this node
let private cleanup (node:Node) : unit =
    let safeDispose (d: IDisposable) =
        try d.Dispose()
        with
            |x ->
                Logging.error $"Disposing {d}: {x.Message}"
                console.dir(d)

    let d = getDisposables node
    log $"cleanup {nodeStr node} - {d.Length} disposable(s)"

    d |> List.iter safeDispose

    clearDisposables node
    dispatchSimple node Event.Unmount

let private cleanupDeep (node:Node) : unit=
    descendantsDepthFirst node |> Array.ofSeq |> Array.iter cleanup
    cleanup node

// Cleanup all descendants and this node
// Remove node from parent
let unmount (node:Node) : unit=
    cleanupDeep node
    if not(isNull(node.parentNode)) then
        node.parentNode.removeChild node |> ignore

let clear (node:Node) =
    children node |> Array.ofSeq |> Array.iter unmount

let exclusive (f : NodeFactory) = nodeFactory <| fun ctx ->
    log $"exclusive {nodeStr ctx.Parent}"
    clear ctx.Parent
    ctx |> build f

let hookContext (hook: BuildContext -> unit) : NodeFactory = nodeFactory <| fun ctx ->
    hook ctx
    unitResult()

let hookParent (hook: Node -> unit) : NodeFactory = nodeFactory <| fun ctx ->
    hook ctx.Parent
    unitResult()

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
        log $"fixPosition {nodeStr node}"
        let width  = s.width
        let height = s.height
        let a = node.getBoundingClientRect()
        node.style.position <- "absolute"
        node.style.width <- width
        node.style.height <- height
        addTransform node a

let removeNode (node:#Node) =
    log <| sprintf "removing node %A" node.textContent
    node.parentNode.removeChild( node ) |> ignore

let fragment (elements : NodeFactory seq) = nodeFactory <| fun ctx ->
    fragmentResult (elements |> Seq.collect (fun e -> ctx |> e.Builder |> collectFragment) |> Seq.toList)

open Fable.Core.JS

// ----------------------------------------------------------------------------
// Serialize tasks through an element. If the task already has a running task
// wait for it to complete before starting the new task. Otherwise, run the
// new task immediately
//
let wait (el:HTMLElement) (andThen : unit -> Promise<unit>) =
    let key = NodeKey.Promise
    let run() = andThen() |> Interop.set el key
    if Interop.exists el key then
        let p = Interop.get<Promise<unit>> el key
        Interop.delete el key
        p.``then`` run |> ignore
    else
        run()

type MountPoint = {
        Doc : Document
        MountId : string
        App : NodeFactory
    }
    with
        member this.Mount() =
            let host = this.Doc.querySelector($"#{this.MountId}")
            build (exclusive this.App) (makeContext host)

// Tried to make these into static members, but get error
// "These element declarations are not permitted in an augmentation F# compiler"
// MountPoint is passed to DevTools
let mutable private _allMountPoints = []

let allMountPoints() = _allMountPoints

let private createMountPoint doc id app =
    let self = { Doc = doc; MountId = id; App = app }
    _allMountPoints <- self :: _allMountPoints
    console.log($"Mount points: {allMountPoints()}")
    self

//
// Mount a top-level application NodeFactory into an existing document
//
let rec mountElementOnDocument (doc : Document) id (app : NodeFactory)  =
    let mp = createMountPoint doc id app
    mp.Mount() |> ignore

let rec mountElement id (app : NodeFactory)  =
    mountElementOnDocument document id app

let computedStyleOpacity e =
    try
        float (window.getComputedStyle(e).opacity)
    with
    | _ ->
        log(sprintf "parse error: '%A'" (window.getComputedStyle(e).opacity))
        1.0

let computedStyleTransform node =
    let style = window.getComputedStyle(node)
    if style.transform = "none" then "" else style.transform

let declareResource<'T when 'T :> IDisposable> (init : unit -> 'T) (f : 'T -> unit) = nodeFactory <| fun ctx ->
    let r = init()
    registerDisposable ctx.Parent r
    f(r)
    unitResult()

let isSameNode (a:Node) (b:Node) =
    if isNull a then isNull b else a.isSameNode(b)

let insertAfter (parent:Node) (prev:Node) (node:Node) =
    parent.insertBefore(node, if isNull prev then parent.firstChild else prev.nextSibling) |> ignore

let appendReplaceChild (node : Node) (ctx : BuildContext) =
    log $"appendReplaceChild {ctx.Action}"
    match ctx.Action with
    | Append ->
        log $"append {nodeStr node} to {nodeStr ctx.Parent}"
        ctx.AppendChild ctx.Parent node |> ignore
    | Before next ->
        log $"insert {nodeStr node} before {nodeStr next} on {nodeStr ctx.Parent}"
        ctx.Parent.insertBefore(node,next) |> ignore
    | After prev ->
        log $"insert {nodeStr node} after {nodeStr prev} on {nodeStr ctx.Parent}"
        insertAfter ctx.Parent prev node
        //ctx.Parent.insertBefore(node, if isNull prev then ctx.Parent.firstChild else prev.nextSibling) |> ignore
    | Replace existing ->
        cleanupDeep existing

        if not (ctx.Parent.isSameNode(existing.parentNode)) then

            if isNull(existing.parentNode) then
                log $"reinstate {nodeStr node} to {nodeStr ctx.Parent} - existing {nodeStr existing} was unmounted unexpectedly"
            else
                log $"reinstate {nodeStr node} to {nodeStr ctx.Parent} - existing {nodeStr existing} has different parent - unmounting"
                existing.parentNode.removeChild(existing) |> ignore

            ctx.AppendChild ctx.Parent node |> ignore
        else
            log $"replace {nodeStr existing} with {nodeStr node} on {nodeStr ctx.Parent}"
            ctx.ReplaceChild ctx.Parent node existing |> ignore

        setSvId node (svId existing)
    node

// ----------------------------------------------------------------------------
// Element builder with namespace

let elns ns tag (xs : seq<NodeFactory>) : NodeFactory = nodeFactory <| fun ctx ->

    let e : Element = if ns = "" then upcast ctx.Document.createElement(tag) else ctx.Document.createElementNS(ns, tag)

    // Considering packing these effects into pipeline that lives on ctx.
    // User can then extend the pipeline, or even re-arrange. No immediate
    // need for it right now.

    // Effect 1
    let id = domId()
    log $"create <{tag}> #{id}"
    setSvId e id

    ctx |> ContextHelpers.withParent e |> buildChildren xs |> ignore

    // Effect 4
    appendReplaceChild e ctx |> ignore

    // Effect 5
    dispatchSimple e Event.ElementReady

    nodeResult e

// ----------------------------------------------------------------------------
// Element builder for DOM

let el tag (xs : seq<NodeFactory>) : NodeFactory = nodeFactory <| fun ctx ->

    let e : Element = upcast ctx.Document.createElement(tag)

    // Considering packing these effects into pipeline that lives on ctx.
    // User can then extend the pipeline, or even re-arrange. No immediate
    // need for it right now.

    // Effect 1
    let id = domId()
    log $"create <{tag}> #{id}"
    setSvId e id

    ctx |> ContextHelpers.withParent e |> buildChildren xs |> ignore

    // Effect 4
    appendReplaceChild e ctx |> ignore

    // Effect 5
    dispatchSimple e Event.ElementReady

    nodeResult e

let buildSolitaryElement (f : NodeFactory) ctx : HTMLElement =
    log $"buildSolitaryElement: {ctx.Action}"
    let node = expectSolitary f ctx
    if isElementNode node then
        node :?> HTMLElement
    else
        let spanWrapper = el "span" [ nodeFactory <| (fun _ -> nodeResult node) ]
        (expectSolitary spanWrapper ctx) :?> HTMLElement

// ----------------------------------------------------------------------------
// Text node

let textNode (doc : Document) value : Node =
    let id = domId()
    log $"create \"{value}\" #{id}"
    let n = doc.createTextNode(value)
    setSvId n id
    upcast n

let text value : NodeFactory =
    nodeFactory <| fun ctx ->
        let doc = ctx.Document
        nodeResult (appendReplaceChild (textNode doc value) ctx)

// ----------------------------------------------------------------------------
// Raw html node

let html text : NodeFactory = nodeFactory <| fun ctx ->
    let el = ctx.Parent :?> HTMLElement
    el.innerHTML <- text
    match ctx.StyleSheet with
    | None -> ()
    | Some ns -> visitElementChildren el (fun ch ->
                                        ch.classList.add ns.Name
                                        applyCustomRules ch ns)
    nodeResult el


module Html =
    let div xs : NodeFactory = el "div" xs
    let textarea xs = el "textarea" xs
    let section xs = el "section" xs
    let i  xs = el "i" xs
    let h1  xs = el "h1" xs
    let h2  xs = el "h2" xs
    let h3  xs = el "h3" xs
    let h4  xs = el "h4" xs
    let h5  xs = el "h5" xs
    let hr  xs = el "hr" xs
    let pre  xs = el "pre" xs
    let code  xs = el "code" xs
    let p  xs = el "p" xs
    let span xs = el "span" xs
    let button  xs = el "button" xs
    let input  xs = el "input" xs
    let label  xs = el "label" xs
    let a  xs = el "a" xs
    let ul  xs = el "ul" xs
    let li xs = el "li" xs
    let img xs = el "img" xs
    let option xs = el "option" xs
    let select xs = el "select" xs
    let form xs = el "form" xs
    let table xs = el "table" xs
    let tbody xs = el "tbody" xs
    let thead xs = el "thead" xs
    let tr xs = el "tr" xs
    let th xs = el "th" xs
    let td xs = el "td" xs

    let app (xs : seq<NodeFactory>) : NodeFactory = fragment xs

    let body (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.body) |> buildChildren xs

    let parent (selector:string) (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.querySelector selector) |> buildChildren xs

    //let sval<'T> (init:'T) (xs:seq<NodeFactory>) = fun ctx ->
    //    let s = Store.make init
    //    DOM.registerDisposable ctx.Parent s
    //    unitResult()
