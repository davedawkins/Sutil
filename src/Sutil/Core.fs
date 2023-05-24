///<summary>
/// The Sutil core engine. Definition for type <c>SutilElement</c> and functions that instantiate Browser DOM from <c>SutilElement</c>.
/// </summary>
module Sutil.Core

open System
open Browser.Dom
open Browser.Types

open DomHelpers
open Fable.Core.JsInterop
open Fable.Core

let private logEnabled() = Logging.isEnabled "core"
let private log s = Logging.log "core" s

//let log s = Fable.Core.JS.console.log(s)

/// <summary>
/// A SutilEffect is the result of evaluating a SutilElement, and can be one of the following:
/// <dl>
///     <dt><code>SideEffect</code></dt>
///     <dd>This usually means the SutilElement was an attribute, and its evaluation resulted in a DOM call such as setAttribute, or classlist.add, etc </dd>
/// </dl>
/// <dl>
///     <dt><code>DomNode of Node</code></dt>
///     <dd>The SutilElement created a DOM Node. This is usually a Text or an HTMLElement</dd>
/// </dl>
/// <dl>
///     <dt><code>Group of SutilGroup</code></dt>
///     <dd>A SutilGroup has been created</dd>
/// </dl>
/// </summary>
type SutilEffect =
    | SideEffect // Not set
    | DomNode of Node // A real browser DOM node
    | Group of SutilGroup // A group of SutilEffects
    member private this.mapDefault f defaultValue =
        match this with
        | DomNode n -> f n
        | Group n -> n.MapParent(f)
        | _ -> defaultValue

    member private this.iter f = this.mapDefault f ()

    member internal this.IsConnected() =
        match this with
        | SideEffect -> false
        | DomNode n -> nodeIsConnected n
        | Group g -> g.IsConnected()

    static member private GetGroups(node: Node) =
        let groups: (SutilGroup list) option = NodeKey.get node NodeKey.Groups
        groups

    static member private GetCreateGroups(node: Node) =
        let groups: (SutilGroup list) =
            NodeKey.getCreate node NodeKey.Groups (fun () -> [])

        groups

    static member private CleanupGroups(n: Node) =
        let groups = SutilEffect.GetGroups(n)

        groups
        |> Option.iter (
            List.iter (fun g -> g.Dispose())
        )

        NodeKey.clear n NodeKey.Groups

    member this.Register(childGroup: SutilGroup) =
        match this with
        | SideEffect -> ()
        | DomNode n ->
            let groups = SutilEffect.GetCreateGroups(n)

            if List.isEmpty groups then
                SutilEffect.RegisterUnsubscribe(n, (fun _ -> SutilEffect.CleanupGroups n))

            Interop.set n NodeKey.Groups (groups @ [ childGroup ])
        | Group g -> g.Register(childGroup)

    member internal this.PrettyPrint(label: string) =
        let rec pr level deep node =
            let indent l = String(' ', l * 4)
            let log l s = log ((indent l) + s)

            let rec prDomNode l (dn: Node) =
                let groups = SutilGroup.GroupsOf dn
                let l' = l + groups.Length

                groups
                |> List.iteri (fun i g -> log (l + i) $"<'{g.Name}'> #{g.Id}")

                match dn with
                | null -> log l "(null)"
                | t when isTextNode (t) -> log l $"'{t.textContent}'"
                | _ ->
                    let e = dn :?> HTMLElement
                    log l' ("<" + e.tagName + "> #" + (string (svId e)))

                    if deep then
                        children e |> Seq.iter (prDomNode (l' + 1))

                        if Interop.exists e NodeKey.Groups then
                            let groups: SutilGroup list = (Interop.get e NodeKey.Groups)

                            for g in groups do
                                prVNode (l' + 1) g

            and prVNode level v =
                let ch =
                    String.Join(
                        ",",
                        v.Children
                        |> List.map (fun (c: SutilEffect) -> "#" + c.Id)
                    )

                log
                    level
                    ("group '"
                     + v.Name
                     + "' #"
                     + (v.Id)
                     + " children=["
                     + ch
                     + "]")

            match node with
            | SideEffect -> log level "-"
            | DomNode n -> prDomNode level n
            | Group v -> prVNode level v

        Browser.Dom.console.groupCollapsed (label)
        pr 0 true this
        Browser.Dom.console.groupEnd ()

    member internal this.Id
        with get (): string =
            match this with
            | SideEffect -> "-"
            | DomNode n -> svId n
            | Group v -> v.Id
        and set id =
            match this with
            | SideEffect -> ()
            | DomNode n -> setSvId n id
            | Group v -> v.Id <- id

    member internal this.IsSameNode(node: SutilEffect) =
        match this, node with
        | SideEffect, SideEffect -> true
        | DomNode a, DomNode b -> a.isSameNode (b)
        | Group a, Group b -> a.Id = b.Id
        | _ -> false

    member internal this.Document =
        match this with
        | SideEffect -> window.document
        | DomNode n -> n.ownerDocument
        | Group v -> v.Document

    member internal this.IsEmpty = this = SideEffect

    member internal this.PrevNode =
        match this with
        | SideEffect -> SideEffect
        | DomNode n -> DomNode(n.previousSibling)
        | Group v -> v.PrevNode

    member private this.NextDomNode =
        match this with
        | SideEffect -> null
        | DomNode node ->
            if isNull node then
                null
            else
                node.nextSibling
        | Group g -> g.NextDomNode

    // All descendant DOM nodes of this SutilEffect. Only groups recurse to their children,
    // we only want the first (parent) DOM node.
    member internal this.collectDomNodes() = this.DomNodes()

    member internal this.DomNodes() =
        match this with
        | SideEffect -> []
        | DomNode n -> [ n ]
        | Group v -> v.DomNodes()

    member public this.AsDomNode = this.mapDefault id null

    member node.Dispose() =
        match node with
        | Group v -> v.Dispose()
        | DomNode n ->  unmount n // cleanupDeep n
        | _ -> ()

    static member RegisterDisposable(node: Node, d: IDisposable) : unit =
        Interop.set node NodeKey.Disposables (d :: getDisposables (node))

    static member RegisterDisposable(node: SutilEffect, d: IDisposable) : unit =
        if logEnabled() then log $"register disposable on {node}"
        match node with
        | SideEffect -> ()
        | DomNode n -> SutilEffect.RegisterDisposable(n, d)
        | Group v -> v.RegisterUnsubscribe(fun _ -> d.Dispose())

    static member RegisterUnsubscribe(node: Node, d: unit -> unit) : unit =
        SutilEffect.RegisterDisposable(node, Helpers.disposable d)

    static member RegisterUnsubscribe(node: SutilEffect, d: unit -> unit) : unit =
        SutilEffect.RegisterDisposable(node, Helpers.disposable d)

    static member private ReplaceGroup(parent: Node, nodes: Node list, existing: Node list) =
        if logEnabled() then log ($"ReplaceGroup: nodes {nodes.Length} existing {existing.Length}")

        let insertBefore =
            match existing with
            | [] -> null
            | [ x ] -> x.nextSibling
            | _ -> (existing |> List.last).nextSibling

        let remove n =
            cleanupDeep n

            if isNull (n.parentNode) then
                if logEnabled() then log $"Warning: Node {nodeStr n} was unmounted unexpectedly"
            else
                if (not (parent.isSameNode (n.parentNode))) then
                    if logEnabled() then log $"Warning: Node {nodeStr n} has unexpected parent"

                DomEdit.removeChild n.parentNode n

        let insert n =
            DomEdit.insertBefore parent n insertBefore

        existing |> List.iter remove
        nodes |> List.iter insert //ids

    member internal this.InsertBefore(node: Node, refNode: Node) : unit =
        this.iter (fun parent -> DomEdit.insertBefore parent node refNode)

    member internal this.InsertAfter(node: SutilEffect, refNode: SutilEffect) =
        match this with

        | SideEffect ->
            JS.console.warn ("InsertAfter called for empty node - disposing child")
            node.Dispose()

        | DomNode parent ->
            if logEnabled() then log ($"InsertAfter (parent = {this}: refNode={refNode} refNode.NextDomNode={nodeStr refNode.NextDomNode}")
            let refDomNode = refNode.NextDomNode

            node.collectDomNodes ()
            |> List.iter (fun child -> DomEdit.insertBefore parent child refDomNode)

        | Group g -> g.InsertAfter(node, refNode)

    member internal this.InsertAfter(node: Node, refNode: Node) =
        this.iter (fun parent -> DomEdit.insertAfter parent node refNode)

    member internal this.ReplaceGroup(node: SutilEffect, existing: SutilEffect, insertBefore: Node) =
        if logEnabled() then log ($"ReplaceGroup({node}, {existing})")

        match this with
        | SideEffect -> ()
        | DomNode parent -> SutilEffect.ReplaceGroup(parent, node.collectDomNodes (), existing.collectDomNodes ())
        // Todo. Remove existing VirtualNodes contained in existing from parent
        // Todo. Add VirtualNodes in node to parent
        | Group parent -> parent.ReplaceChild(node, existing, insertBefore)

    member internal this.AppendChild(child: Node) =
        match this with
        | SideEffect -> ()
        | DomNode parent -> DomEdit.appendChild parent child
        | Group parent -> parent.AppendChild(DomNode child)

    member internal this.FirstDomNodeInOrAfter =
        match this with
        | SideEffect -> null
        | DomNode n -> n
        | Group g -> g.FirstDomNodeInOrAfter

    override this.ToString() =
        match this with
        | SideEffect -> "SideEffect"
        | DomNode n -> nodeStrShort n
        | Group v -> v.ToString()

    member internal this.Clear() = this.iter clear

    static member MakeGroup(name: string, parent: SutilEffect, prevInit: SutilEffect) =
        SutilGroup.Create(name, parent, prevInit)

    interface IDisposable with
        member __.Dispose() = __.Dispose()

and SutilGroup private (_name, _parent, _prevInit) as this =
    let mutable id = domId () |> string
    let mutable _dispose: (unit -> unit) list = []
    let mutable _children = []
    let mutable _prev = _prevInit
    let mutable _childGroups = []

    let updateChildrenPrev () =
        //log($"updating children {childStrs()}")
        let mutable p = SideEffect

        for c in _children do
            match c with
            | Group v -> v.PrevNode <- p
            | _ -> ()

            p <- c

    let parentDomNode () =
        let rec findParent p =
            match p with
            | SideEffect -> null
            | DomNode n -> n
            | Group v -> findParent v.Parent

        findParent _parent

    do
        _parent.Register(this)
        ()
    with
        member this.Document = parentDomNode().ownerDocument

        static member internal Create(name, parent, prevInit) = SutilGroup(name, parent, prevInit)

        member internal this.IsConnected() = parentDomNode () |> nodeIsConnected

        member internal this.AssertIsConnected() =
            if (not <| this.IsConnected()) then
                failwith $"Not connected: {this}"

        override this.ToString() =
            _name
            + "["
            + (String.Join(",", _children |> List.map (fun n -> n.ToString())))
            + "]#"
            + id

        member internal this.Parent = _parent

        member internal this.Register(childGroup: SutilGroup) =
            _childGroups <- childGroup :: _childGroups

        member internal this.PrevNode
            with get () = _prev
            and set v = _prev <- v

        member internal this.DomNodes() =
            this.Children
            |> List.collect (fun c -> c.DomNodes())

        member internal this.PrevDomNode =
            let result =
                match this.PrevNode with
                | DomNode n -> n
                | Group v ->
                    match v.LastDomNode with
                    | null -> v.PrevDomNode
                    | n -> n
                | SideEffect -> // We're the first child
                    match this.Parent with
                    | Group pv -> pv.PrevDomNode
                    | _ -> null

            if logEnabled() then log ($"PrevDomNode of {this} -> '{nodeStr result}' PrevNode={this.PrevNode}")
            result

        member internal this.NextDomNode =
            let result =
                match this.DomNodes() with
                // We don't have any nodes.
                | [] ->
                    match this.PrevDomNode with
                    | null -> // No DOM node before us, so our next node must be parent DOM node's first child
                        match parentDomNode () with
                        | null ->
                            null
                        | p ->
                            p.firstChild
                    | prev ->
                        prev.nextSibling

                // We do have nodes, so next node is last node's next sibling
                | ns ->
                    match ns |> List.last with
                    | null ->
                        null
                    | last ->
                        last.nextSibling
            result

        member internal this.FirstDomNode =
            match this.DomNodes() with
            | [] -> null
            | n :: ns -> n

        member internal this.LastDomNode =
            match this.DomNodes() with
            | [] -> null
            | ns -> ns |> List.last

        member internal this.FirstDomNodeInOrAfter =
            match this.FirstDomNode with
            | null -> this.NextDomNode
            | first -> first

        member internal this.MapParent<'T>(f: (Node -> 'T)) = f (parentDomNode ())

        member private this.OwnX(n: Node) = Interop.set n "__sutil_snode" this

        member private this.OwnX(child: SutilEffect) =
            match child with
            | DomNode n -> this.OwnX(n)
            | _ -> ()

        static member internal GroupOf(n: Node) : SutilGroup option = Interop.getOption n "__sutil_snode"

        static member internal GroupsOf(n: Node) : SutilGroup list =
            let rec parentsOf (r: SutilGroup list) =
                match r with
                | [] -> r
                | x :: xs ->
                    match x.Parent with
                    | Group g -> parentsOf (g :: r)
                    | _ -> r

            let init n =
                match Interop.getOption n "__sutil_snode" with
                | None -> []
                | Some g -> [ g ]

            parentsOf (init n)

        member internal this.Clear() =
            // TODO clean up each node
            _children <- []

        member internal this.AddChild(child: SutilEffect) =
            this.OwnX(child)
            _children <- _children @ [ child ]
            updateChildrenPrev ()

        member internal this.AppendChild(child: SutilEffect) =
            match this.Parent with
            | SideEffect -> ()
            | _ ->
                let cn = this.DomNodes() |> List.map nodeStrShort

                let pn =
                    this.PrevNode.DomNodes() |> List.map nodeStrShort

                let parent = parentDomNode ()
                let before = this.NextDomNode

                child.collectDomNodes ()
                |> List.iter (fun ch ->
                    DomEdit.insertBefore parent ch before)
            this.OwnX(child)
            _children <- _children @ [ child ]
            updateChildrenPrev ()

        member private this.FirstChild =
            match _children with
            | [] -> SideEffect
            | x :: xs -> x

        member private this.ChildAfter(prev: SutilEffect) =
            match prev with
            | SideEffect -> this.FirstChild
            | _ ->
                let rec find (list: SutilEffect list) =
                    match list with
                    | [] ->
                        SideEffect
                    | x :: [] when x.IsSameNode(prev) ->
                        SideEffect
                    | x :: y :: _ when x.IsSameNode(prev) ->
                        y
                    | x :: xs ->
                        find xs

                find _children

        member internal this.InsertAfter(child: SutilEffect, prev: SutilEffect) =
            this.InsertBefore(child, this.ChildAfter(prev))

        member private this.InsertBefore(child: SutilEffect, refNode: SutilEffect) =
            let refDomNode =
                match refNode with
                | SideEffect -> this.NextDomNode
                | _ -> refNode.FirstDomNodeInOrAfter

            if logEnabled() then log (
                $"InsertBefore: child='{child}' before '{refNode}' refDomNode='{nodeStrShort refDomNode}' child.PrevNode='{child.PrevNode}'"
            )

            let parent = parentDomNode ()
            let len = _children.Length

            for dnode in child.collectDomNodes () do
                DomEdit.insertBefore parent dnode refDomNode

            if refNode = SideEffect then
                this.AddChild(child)
            else
                _children <-
                    _children
                    |> List.fold
                        (fun list ch ->
                            match ch with
                            | n when n.IsSameNode(refNode) -> list @ [ child ] @ [ n ]
                            | _ -> list @ [ ch ])
                        []

                this.OwnX(child)

            updateChildrenPrev ()

            if _children.Length = len then
                if logEnabled() then log ($"Error: Child was not added")

        member internal _.RemoveChild(child: SutilEffect) =
            let rec rc (p: SutilGroup) (c: SutilEffect) =
                match c with
                | SideEffect -> ()
                | DomNode n -> unmount n
                | Group g ->
                    g.Children
                    |> List.iter (fun gc -> g.RemoveChild(gc))

                    g.Dispose()

            let newChildren =
                _children |> List.filter (fun n -> n <> child)
            rc this child
            _children <- newChildren
            updateChildrenPrev ()

        member internal this.ReplaceChild(child: SutilEffect, oldChild: SutilEffect, insertBefore: Node) =
            let deleteOldNodes () =
                let oldNodes = oldChild.collectDomNodes ()

                oldNodes
                |> List.iter (fun c ->
                    if (isNull c.parentNode) then // We were unexpectedly removed from the DOM by something else (perhaps)
                        if logEnabled() then log ($"Node has no parent: {nodeStrShort c}")
                    else
                        DomEdit.removeChild c.parentNode c)

            let nodes = child.collectDomNodes ()

            assertTrue (child <> SideEffect) "Empty child for replace child"

            if oldChild <> SideEffect then
                assertTrue
                    (_children
                     |> List.exists (fun c -> c.Id = oldChild.Id))
                    "Child not found"

                child.Id <- oldChild.Id

            let parent = parentDomNode ()

            nodes
            |> List.iter (fun n ->
                DomEdit.insertBefore parent n insertBefore)

            deleteOldNodes ()

            match oldChild with
            | Group g -> g.Dispose()
            | _ -> ()

            if isNull insertBefore || oldChild = SideEffect then
                this.AddChild child
            else
                this.OwnX child

                _children <-
                    _children
                    |> List.map (fun n -> if n.Id = oldChild.Id then child else n)

            updateChildrenPrev ()

        member internal _.Name = _name

        member internal _.Id
            with get () = id
            and set id' = id <- id'

        member internal _.Children = _children

        member _.RegisterUnsubscribe d =
            _dispose <- _dispose @ [ d ]

        member _.Dispose() =
            _childGroups |> List.iter (fun c -> c.Dispose())
            _dispose |> List.iter (fun d -> d ())
            _dispose <- []

let private notifySutilEvents (parent : SutilEffect) (node : SutilEffect) =
    if (parent.IsConnected()) then
        node.collectDomNodes ()
        |> List.iter (fun n ->
                CustomDispatch<_>.dispatch(n,Event.Connected)
                CustomDispatch<_>.dispatch(n,Event.Mount)

                n
                |> DomHelpers.descendants
                |> Seq.filter DomHelpers.isElementNode
                |> Seq.iter (fun n ->  CustomDispatch<_>.dispatch(n,Event.Mount))

            )

/// <exclude/>
type DomAction =
    | Append // appendChild
    | Replace of SutilEffect * Node // bindings use this to replace the previous DOM fragment
    | Nothing

type PipelineFn = (BuildContext*SutilEffect) ->  (BuildContext*SutilEffect)

/// <exclude/>
and  BuildContext =
    { Document: Browser.Types.Document
      Parent: SutilEffect
      Previous: SutilEffect
      Action: DomAction
      MakeName: (string -> string)
      Class: string option
      Debug: bool
      Pipeline : PipelineFn
      }

    member this.ParentElement: HTMLElement = this.Parent.AsDomNode :?> HTMLElement
    member this.ParentNode: Node = this.Parent.AsDomNode

    member ctx.AddChild(node: SutilEffect) : unit =
        match ctx.Action with
        | Nothing -> ()

        | Append ->
            if logEnabled() then log $"ctx.Append '{node}' to '{ctx.Parent}' after {ctx.Previous}"
            ctx.Parent.InsertAfter(node, ctx.Previous)

            notifySutilEvents ctx.Parent node

        | Replace (existing, insertBefore) ->
            if logEnabled() then log $"ctx.Replace '{existing}' with '{node}' before '{nodeStrShort insertBefore}'"
            ctx.Parent.ReplaceGroup(node, existing, insertBefore)

            notifySutilEvents ctx.Parent node
        ()


let internal domResult (node: Node) = DomNode node
let internal sutilResult (node: SutilEffect) = node

let internal sideEffect (ctx, name) =
    let text () =
        let tn = ctx.Document.createTextNode name
        let d = ctx.Document.createElement ("div")
        DomEdit.appendChild d tn
        ctx.AddChild(DomNode d)
        d

    if ctx.Debug then
        DomNode(text ())
    else
        SideEffect
/// <summary>
/// Sutil's element type. This is an abstraction of DOM elements, attributes, events, etc.
/// The type itself is a function that maps <c>BuildContext</c> to a <c>SutilEffect</c>,
/// wrapped in a private record to isolate users from implementation details as much as possible:
/// <code>
/// type SutilElement = private { Builder: BuildContext -> SutilEffect }
/// </code>
///
/// Examples of SutilElements:
///
/// <ul>
/// <li> <code>Html.div</code></li>
/// <li> <code>Attr.className</code></li>
/// <li> <code>Ev.onClick</code></li>
/// <li> <code>Core.disposeOnUnmount</code></li>
/// <li> <code>Core.host</code></li>
/// </ul>
///
/// </summary>
type SutilElement private (name : string, children : seq<SutilElement>, builder : BuildContext -> SutilEffect) =
    do ()
    static member Define( builder : BuildContext -> SutilEffect ) =
        SutilElement( "", [], builder )

    static member Define( name : string, builder : BuildContext -> SutilEffect ) =
        SutilElement( name, [], builder )

    static member Define( name : string, builder : BuildContext -> unit ) =
        SutilElement( name, [], (fun ctx -> ctx |> builder;  sideEffect(ctx, name)) )

    static member Define( name : string, children : seq<SutilElement>, builder : BuildContext -> Node ) =
        SutilElement( name, children, fun ctx -> ctx |> builder |> DomNode )

    member internal __.Builder = builder

let private defaultContext (parent : Node) =
    let gen = Helpers.makeIdGenerator ()

    { Document = parent.ownerDocument
      Parent = DomNode parent
      Previous = SideEffect
      Action = Append
//      StyleSheet = None
      Class = None
      Debug = false
      MakeName = fun baseName -> sprintf "%s-%d" baseName (gen ())
      Pipeline = id
      }

let private makeContext (parent: Node) =

    let getSutilClasses (e: HTMLElement) =
        let classes =
            [ 0 .. e.classList.length - 1 ]
            |> List.map (fun i -> e.classList.[i])
            |> List.filter (fun cls -> cls.StartsWith("sutil"))
        classes

    { defaultContext parent with
          //Pipeline = pipeline

          // Ensures that if we create and mount DOM nodes onto a styled element, then
          // we inherit the stylesheet class on the mounted nodes
          Class =
            parent :?> HTMLElement
            |> Option.ofObj
            |> Option.bind (fun e -> getSutilClasses e |> List.tryHead)
    }

let private makeShadowContext (customElement: Node) =
    { defaultContext customElement with
        Action = Nothing
    }


module ContextHelpers =
    let withStyleSheet sheet ctx : BuildContext = ctx //{ ctx with StyleSheet = Some sheet }

    let withDebug ctx : BuildContext = { ctx with Debug = true }

    let withPreProcess f (ctx:BuildContext) = { ctx with Pipeline = (f>>ctx.Pipeline) }
    let withPostProcess f (ctx:BuildContext) = { ctx with Pipeline = (ctx.Pipeline>>f) }

    let withParent parent ctx : BuildContext =
        { ctx with
            Parent = parent
            Action = Append }

    let withPrevious prev ctx : BuildContext = { ctx with Previous = prev }

    let withParentNode parent ctx : BuildContext = withParent (DomNode parent) ctx

    let withReplace (toReplace: SutilEffect, before: Node) ctx =
        { ctx with Action = Replace(toReplace, before) }

let internal errorNode (parent: SutilEffect) message : Node =
    let doc = parent.Document
    let d = doc.createElement ("div")
    DomEdit.appendChild d (doc.createTextNode ($"sutil-error: {message}"))
    parent.AppendChild(d)
    d.setAttribute ("style", "color: red; padding: 4px; font-size: 10px;")
    upcast d

/// <summary>
/// Instantiate a <c>SutilElement</c>.
/// </summary>
let build (f: SutilElement) (ctx: BuildContext) =
    (ctx, f.Builder ctx)
    |> ctx.Pipeline
    |> snd

let internal buildOnly (f: SutilElement) (ctx: BuildContext) =
    f.Builder ctx

let private pipelineDispatchMount (ctx : BuildContext, result : SutilEffect) =
    match result with
    | DomNode n -> CustomDispatch<_>.dispatch(n,Event.Mount)
    | _ -> ()
    (ctx,result)

let private pipelineAddClass (ctx: BuildContext, result : SutilEffect) =
    match ctx.Class, result with
    | Some cls, DomNode _ ->
        result.AsDomNode
        |> applyIfElement (ClassHelpers.addToClasslist cls)
    | _ -> ()
    (ctx, result)

let internal buildChildren (xs: seq<SutilElement>) (ctx: BuildContext) : unit =
    let e = ctx.Parent

    let mutable prev = SideEffect

    for x in xs do
        //log($"  buildChildren: prev={prev}")
        match ctx |> ContextHelpers.withPrevious prev |> build x with
        | SideEffect -> ()
        | r -> prev <- r

    ()

/// <exclude/>
[<Global>]
type ShadowRoot() =
    member internal this.appendChild(el: Browser.Types.Node) = jsNative

let internal mountOnShadowRoot app (host: Node) : (unit -> unit) =
    let el = build app (makeShadowContext host)

    match el with
    | DomNode node ->
        let shadowRoot: ShadowRoot = host?shadowRoot
        shadowRoot.appendChild (node)
    | Group group ->
        let shadowRoot: ShadowRoot = host?shadowRoot

        for node in group.DomNodes() do
            shadowRoot.appendChild (node)
    | SideEffect -> failwith "Custom components must return at least one node"

    let dispose () =
        el.Dispose()

    dispose

let internal mount app ((op,eref) : MountPoint) =
    let node = eref.AsElement

    match op with
    | AppendTo ->
        build app (makeContext node)

    | InsertAfter ->
        build app { (makeContext node.parentElement) with Previous = DomNode node }
