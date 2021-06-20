module Sutil.DOM

open System
open Browser.Dom
open Browser.Types
open Browser.CssExtensions
open Interop

let log = Logging.log "dom"

let dispatch (target:EventTarget) name (data:obj) =
    if not (isNull target) then
        target.dispatchEvent( Interop.customEvent name data) |> ignore
let dispatchSimple (target:EventTarget) name =
    dispatch target name {| |}

let dispatchCustom<'T> (target: EventTarget) (name: string) (init: CustomEventInit<'T>) =
    if not (isNull target) then
        target.dispatchEvent(customEvent name init) |> ignore

[<RequireQualifiedAccessAttribute>]
module NodeKey =
    let Disposables = "__sutil_disposables"
    let ResizeObserver = "__sutil_resizeObserver"
    let TickTask = "__sutil_tickTask"
    let Promise = "__sutil_promise"
    let NodeMap = "__sutil_nodes"
    let Groups = "__sutil_groups"

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

type CustomDispatch<'T> =
    | Detail of 'T option
    | Bubbles of bool
    | Composed of bool
    with
        static member toCustomEvent<'T> (props : CustomDispatch<'T> list) =
            let mutable data : obj = upcast {| |}
            for p in props do
                match p with
                | Detail  d -> Interop.set data "detail" d
                | Bubbles b -> Interop.set data "bubbles" b
                | Composed c -> Interop.set data "composed" c
            data :?> CustomEventInit<'T>
        static member dispatch( target : EventTarget, name : string ) =
            dispatchCustom<unit> target name (CustomDispatch.toCustomEvent<unit>([]))

        static member dispatch( e : Event, name : string ) =
            dispatchCustom<unit> (e.target) name (CustomDispatch.toCustomEvent<unit>([]))

        static member dispatch<'T>( target : EventTarget, name : string, props : CustomDispatch<'T> list ) =
            dispatchCustom<'T> target name (CustomDispatch.toCustomEvent<'T> props)

        static member dispatch<'T>( e : Event, name : string, props : CustomDispatch<'T> list ) =
            dispatchCustom<'T> (e.target) name (CustomDispatch.toCustomEvent<'T> props)

let domId = Helpers.makeIdGenerator()

[<Literal>]
let ElementNodeType = 1.0

[<Literal>]
let TextNodeType = 3.0

let isTextNode (n:Node) = n.nodeType = TextNodeType
let isElementNode (n:Node) = n.nodeType = ElementNodeType
let asTryElement (n:Node) = if isElementNode n then Some (n :?> HTMLElement) else None
let documentOf (n:Node) = n.ownerDocument

let applyIfElement (f: HTMLElement -> unit) (n:Node) = if isElementNode n then f (n:?>HTMLElement)

let SvIdKey = "_svid"

let getNodeMap (doc:Document) : obj= NodeKey.getCreate doc.body NodeKey.NodeMap (fun () -> upcast {| |})

let setSvId (n:Node) id =
    let map = getNodeMap n.ownerDocument
    Interop.set map (string id) n
    Interop.set n SvIdKey id
    if (isElementNode n) then
        (n :?> HTMLElement).setAttribute(SvIdKey,(string id))

let svId (n:Node) = Interop.get n SvIdKey

let hasSvId (n:Node) = Interop.exists n SvIdKey

let findNodeWithSvId (doc : Document) id : Node option =
    let map = getNodeMap doc
    let key = string id
    match Interop.exists map key with
    | true -> Some (Interop.get map key)
    | _ -> None

    //let getId n =
    //    let r = svId n
    //    if (r = id) then Some n else None
    //findNode doc.body getId

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

let nodeStrShort (node : Node) =
    if isNull node then
        "null"
    else
        let mutable tc = node.textContent
        if  tc.Length > 16 then tc <- tc.Substring(0,16) + "..."
        match node.nodeType with
        | ElementNodeType ->
            let e = node :?> HTMLElement
            $"<{e.tagName.ToLower()}> #{svId node}"
        | TextNodeType ->
            $"text:\"{tc}\" #{svId node}"
        | _ -> $"?'{tc}'#{svId node}"

open Fable.Core.JsInterop

module DomEdit =
    let log s =
        if Interop.exists window "domeditlog" then
            window?domeditlog(s)
        else
            Logging.log "dom" s

    let appendChild (parent:Node) (child:Node) =
        log $"appendChild parent='{nodeStrShort parent}' child='{nodeStrShort child}'"
        parent.appendChild(child) |> ignore
        log $"after: appendChild parent='{nodeStrShort parent}' child='{nodeStrShort child}'"

    let removeChild (parent:Node) (child:Node) =
        log $"removeChild parent='{nodeStrShort parent}' child='{nodeStrShort child}'"
        parent.removeChild(child) |> ignore
        log $"after: removeChild parent='{nodeStrShort parent}' child='{nodeStrShort child}'"

    let insertBefore (parent:Node) (child:Node) (refNode:Node) =
        log $"insertBefore parent='{nodeStrShort parent}' child='{nodeStrShort child}' refNode='{nodeStrShort refNode}'"
        parent.insertBefore(child,refNode) |> ignore
        log $"after: insertBefore parent='{nodeStrShort parent}' child='{nodeStrShort child}' refNode='{nodeStrShort refNode}'"

    let insertAfter (parent : Node) (newChild : Node) (refChild : Node) =
        let beforeChild = if isNull refChild then parent.firstChild else refChild.nextSibling
        insertBefore parent newChild beforeChild

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

let isSameNode (a:Node) (b:Node) =
    if isNull a then isNull b else a.isSameNode(b)



let private hasDisposables (node:Node) : bool =
    Interop.exists node NodeKey.Disposables

let private getDisposables (node:Node) : IDisposable list =
    if hasDisposables node then Interop.get node NodeKey.Disposables else []

let private clearDisposables (node:Node) : unit =
    Interop.delete node NodeKey.Disposables

// Call all registered disposables on this node
let private cleanup (node:Node) : unit =
    let safeDispose (d: IDisposable) =
        try d.Dispose()
        with x -> Logging.error $"Disposing {d}: {x} from {nodeStr node}"

    let d = getDisposables node
    log $"cleanup {nodeStr node} - {d.Length} disposable(s)"

    d |> List.iter safeDispose

    clearDisposables node
    dispatchSimple node Event.Unmount

let assertTrue condition message =
    if not condition then failwith message

let private cleanupDeep (node:Node) : unit=
    descendantsDepthFirst node |> Array.ofSeq |> Array.iter cleanup
    cleanup node

// Cleanup all descendants and this node
// Remove node from parent
let unmount (node:Node) : unit=
    cleanupDeep node
    if not(isNull(node.parentNode)) then
        DomEdit.removeChild node.parentNode node

let clear (node:Node) =
    children node |> Array.ofSeq |> Array.iter unmount

let listen (event:string) (e:EventTarget) (fn: (Event -> unit)) : (unit -> unit)=
    e.addEventListener( event, fn )
    (fun () -> e.removeEventListener(event, fn) |> ignore)

let raf (f : float -> unit) = Window.requestAnimationFrame( fun t -> try f t with|x -> Logging.error $"raf: {x.Message}" )
let rafu (f : unit -> unit) = Window.requestAnimationFrame( fun _ -> try f() with|x -> Logging.error $"rafu: {x.Message}" ) |> ignore

let once (event:string) (target:EventTarget) (fn : Event->Unit) : unit =
    let rec inner e = target.removeEventListener( event, inner ); fn(e)
    listen event target inner |> ignore

let interval callback (delayMs : int) =
    let id = Fable.Core.JS.setInterval callback delayMs
    fun () -> Fable.Core.JS.clearInterval id

let timeout callback (delayMs : int) =
    let id = Fable.Core.JS.setTimeout callback delayMs
    fun () -> Fable.Core.JS.clearTimeout id

module CssRules =
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
    Selector : CssRules.CssSelector
    Style : (string*obj) list
}

type StyleSheet = StyleRule list

type NamedStyleSheet = {
    Name : string
    StyleSheet : StyleSheet
    Parent : NamedStyleSheet option
}

let rec private forEachChild (parent:Node) (f : Node -> unit) =
    let mutable child = parent.firstChild
    while not (isNull child) do
        f child
        child <- child.nextSibling

/// SutilNode is a DOM node, the result of evaluating a SutilElement. Fragment and binding elements will return a GroupNode,
/// which is a grouping of DOM nodes, so a SutilNode can also be a GroupNode. Finally, SutilNode can be an EmptyNode to represent
/// the concept of None (or null)
type SutilNode =
    | EmptyNode   // Not set
    | DomNode of Node // A real browser DOM node
    | GroupNode of NodeGroup // A group of SutilNodes
    with
        member this.mapDefault f defaultValue =
            match this with
            |DomNode n -> f n
            |GroupNode n -> n.MapParent(f)
            |_-> defaultValue
        member this.iter f = this.mapDefault f ()
        member this.iterElement (f : HTMLElement -> unit) = this.mapDefault (applyIfElement f) ()

        member this.PrettyPrint(label:string) =
            let rec pr level deep node =
                let indent l = new String(' ', l*4)
                let log l s = log((indent l) + s)
                let rec prDomNode l (dn:Node) =
                    let groups = NodeGroup.GroupsOf dn
                    let l' = l + groups.Length
                    groups
                    |> List.iteri (fun i g -> log (l + i) $"<'{g.Name}'> #{g.Id}")

                    match dn with
                    | null -> log l "(null)"
                    | t when isTextNode(t) -> log l $"'{t.textContent}'"
                    | _ ->
                        let e = dn :?> HTMLElement
                        //let g =
                        //    NodeGroup.GroupOf e
                        //    |> Option.map (fun g -> " : " + g.Name + "#" + g.Id)
                        //    |> Option.defaultValue ""
                        log l' ("<" + e.tagName + "> #" + (string (svId e)))
                        if deep then
                            forEachChild e (prDomNode (l'+1))
                            if Interop.exists e NodeKey.Groups then
                                let groups : NodeGroup list = (Interop.get e NodeKey.Groups)
                                for g in groups do
                                    prVNode (l'+1) g
                and prVNode level v =
                    let ch = String.Join(",", v.Children |> List.map (fun (c:SutilNode) -> "#" + c.Id))
                    log level ("group '" + v.Name + "' #" + (v.Id) + " children=[" + ch + "]")
                    //for c in v.Children do
                    //    pr (level + 1) false c

                match node with
                | EmptyNode -> log level "-"
                | DomNode n -> prDomNode level n
                | GroupNode v -> prVNode level v
            Browser.Dom.console.groupCollapsed(label)
            pr 0 true this
            Browser.Dom.console.groupEnd()

        member this.Id
            with get() : string =
                match this with
                | EmptyNode -> "-"
                | DomNode n -> svId n
                | GroupNode v -> v.Id
            and set id =
                match this with
                | EmptyNode -> ()
                | DomNode n -> setSvId n id
                | GroupNode v -> v.Id <- id

        member this.IsSameNode (node:SutilNode) =
            match this,node with
            | EmptyNode,EmptyNode -> true
            | DomNode a, DomNode b -> a.isSameNode(b)
            | GroupNode a, GroupNode b -> a.Id = b.Id
            | _ -> false

        member this.Document =
            match this with
            | EmptyNode -> window.document
            | DomNode n -> n.ownerDocument
            | GroupNode v -> v.Document

        member this.IsEmpty = this = EmptyNode

        member this.LastDomNode : Node =
            match this with
            | EmptyNode -> null
            | DomNode n -> n
            | GroupNode _ -> match this.collectDomNodes() with |[] -> null | xs -> xs |> List.last

        member this.PrevNode =
            match this with
            | EmptyNode -> EmptyNode
            | DomNode n -> DomNode (n.previousSibling)
            | GroupNode v -> v.PrevNode

        member this.PrevDomNode =
            match this with
            | EmptyNode -> null
            | DomNode n -> n.previousSibling
            | GroupNode v -> match v.PrevNode.collectDomNodes() with |[] -> null | xs -> xs |> List.last

        member this.NextDomNode =
            match this with
            | EmptyNode -> null
            | DomNode node -> if isNull node then null else node.nextSibling
            | GroupNode g -> g.NextDomNode

        // All descendant DOM nodes of this SutilNode. Only groups recurse to their children,
        // we only want the first (parent) DOM node.
        member this.collectDomNodes () = this.DomNodes()

        member this.DomNodes() =
            match this with
            | EmptyNode -> []
            | DomNode n -> [ n ]
            | GroupNode v -> v.DomNodes()

        member this.AsDomNode = this.mapDefault id null

        member node.Disposables =
            match node with
            | EmptyNode -> []
            | DomNode n -> NodeKey.getCreate n NodeKey.Disposables (fun () -> [])
            | GroupNode v -> []

        member node.Dispose() =
            match node with
            | GroupNode v -> v.Dispose()
            | _ -> ()

        static member GetDisposables(node:Node) =
            NodeKey.getCreate node NodeKey.Disposables (fun () -> [])

        static member RegisterDisposable(node:Node, d:IDisposable) : unit =
            Interop.set node NodeKey.Disposables (d :: getDisposables(node))

        static member RegisterDisposable (node:SutilNode, d:IDisposable) : unit =
            log $"register disposable on {node}"
            //if registeredDisposables.ContainsKey(d) then failwith $"Disposable {d} has already been registered on {nodeStr registeredDisposables.[d]}, attempt to register on {nodeStr node}"
            //registeredDisposables.[d] <- node
            //let disposables : List<IDisposable> = node.Disposables
            //Interop.set node NodeKey.Disposables (d :: disposables)
            match node with
            | EmptyNode -> ()
            | DomNode n -> SutilNode.RegisterDisposable(n,d)
            | GroupNode v -> ()

        static member RegisterUnsubscribe (node : Node, d:unit->unit) : unit =
            SutilNode.RegisterDisposable (node,Helpers.disposable d)

        static member RegisterUnsubscribe (node : SutilNode, d:unit->unit) : unit =
            SutilNode.RegisterDisposable (node,Helpers.disposable d)

        static member private ReplaceGroup (parent : Node, nodes : Node list, existing : Node list ) =
            log($"ReplaceGroup: nodes {nodes.Length} existing {existing.Length}")
            let insertBefore =
                match existing with
                | [] -> null
                | [x] -> x.nextSibling
                | _ -> (existing |> List.last).nextSibling

            let remove n =
                cleanupDeep n
                if isNull(n.parentNode) then
                    log $"Warning: Node {nodeStr n} was unmounted unexpectedly"
                else
                    if (not (parent.isSameNode(n.parentNode))) then
                        log $"Warning: Node {nodeStr n} has unexpected parent"
                    DomEdit.removeChild n.parentNode n

            let insert  n =
                DomEdit.insertBefore parent n insertBefore
                //setSvId n id

            //let ids = existing |> List.map svId

            existing |> List.iter remove
            nodes |> List.iter insert //ids

        member this.InsertAfter(node:SutilNode,refNode:SutilNode) =
            match this with
            |EmptyNode -> ()
            |DomNode parent ->
                log($"InsertAfter (parent = {this}: refNode={refNode} refNode.NextDomNode={nodeStr refNode.NextDomNode}")
                let refDomNode = refNode.NextDomNode
                node.collectDomNodes() |> List.iter (fun child -> DomEdit.insertBefore parent child refDomNode)
            |GroupNode g -> g.InsertAfter(node,refNode)

        member this.InsertAfter(node:Node,refNode:Node) =
            this.iter (fun parent -> DomEdit.insertAfter parent node refNode)

        member this.RemoveChild(node:Node) =
            this.iter (fun parent -> DomEdit.removeChild parent node)

        member this.ReplaceGroup(node:SutilNode,existing:SutilNode, insertBefore : Node) =
            log($"ReplaceGroup({node}, {existing})")
            match this with
            | EmptyNode -> ()
            | DomNode parent ->
                SutilNode.ReplaceGroup(parent, node.collectDomNodes(), existing.collectDomNodes())
                // Todo. Remove existing VirtualNodes contained in existing from parent
                // Todo. Add VirtualNodes in node to parent
            | GroupNode parent ->
                parent.ReplaceChild(node,existing,insertBefore)

        member this.AppendChild (child : Node) =
            match this with
            | EmptyNode -> ()
            | DomNode parent -> DomEdit.appendChild parent child
            | GroupNode parent -> parent.AppendChild(DomNode child)

        member this.AppendChild (child : SutilNode) =
            match this with
            | EmptyNode -> ()
            | DomNode parent ->
                child.collectDomNodes() |> List.iter (fun child -> DomEdit.appendChild parent child)
            | GroupNode parent ->
                parent.AppendChild(child)

        member this.FirstDomNodeInOrAfter =
            match this with
            | EmptyNode -> null
            | DomNode n -> n
            | GroupNode g -> g.FirstDomNodeInOrAfter

        member this.InsertBefore(node:Node,refNode:Node) : unit =
            this.iter (fun parent ->
                DomEdit.insertBefore parent node refNode)

        //member private this.InsertBefore (child : SutilNode, refNode : SutilNode ) =
        //    match this with
        //    | EmptyNode -> ()
        //    | DomNode parent ->
        //        let refDomNode = refNode.FirstDomNodeInOrAfter
        //        child.collectDomNodes() |> List.iter (fun child -> DomEdit.insertBefore parent child refDomNode)
        //    | GroupNode g ->
        //        g.InsertBefore(child, refNode)

        member this.AddClass( cls : string ) = this.iterElement (fun parent -> parent.classList.add(cls))
        member this.RemoveClass( cls : string ) = this.iterElement (fun parent -> parent.classList.remove(cls))
        override this.ToString() =
            match this with
            | EmptyNode -> "EmptyNode"
            | DomNode n -> nodeStrShort n
            | GroupNode v -> v.ToString()
        member this.Clear() = this.iter clear

        member this.Children : list<SutilNode> =
            match this with
            | EmptyNode -> []
            | DomNode n -> [] // Careful!   div [ div[] fragment[] div[] ] -> Children of n are: DomNode, GroupNode, DomNode
            | GroupNode v -> v.Children

and NodeGroup(_name,_parent,_prevInit) as this =
    let mutable id = domId() |> string
    let mutable _dispose = ignore
    let mutable _children = []
    let mutable _prev = _prevInit
    let childDomNodes() = _children |> List.map (function |DomNode n -> [n]|_ -> [])

    let childStrs() = _children |> List.map string

    let assertIsChild (child:SutilNode) =
        let isChild = _children |> List.exists (fun c -> c.IsSameNode(child))
        if not isChild then
            log($"Not a child: {child}")
            failwith $"Not a child: {child}"

    let updateChildrenPrev() =
        log($"updating children {childStrs()}")
        let mutable p = EmptyNode
        for c in _children do
            match c with
            | GroupNode v -> v.PrevNode <- p
            | _ -> ()
            p <- c

    let parentDomNode() =
        let rec findParent p =
            match p with
            |EmptyNode -> null
            |DomNode n -> n
            |GroupNode v -> findParent v.Parent
        findParent _parent
    do
        let p = parentDomNode()
        let groups = NodeKey.getCreate p NodeKey.Groups (fun () -> [])
        Interop.set p NodeKey.Groups (groups @ [ this ])
        ()
    with
        member this.Document = parentDomNode().ownerDocument

        override this.ToString() =
            _name + "[" + (String.Join(",", _children |> List.map (fun n -> n.ToString()))) + "]#" + id

        member this.Parent = _parent

        member this.PrevNode with get () = _prev and set v = _prev <- v

        member this.DomNodes() =
            this.Children |> List.collect (fun c -> c.DomNodes())

        (*
            div [
                fragment [ ] // PrevDomNode = null
            ]

            div [
                div2
                fragment [ ] // PrevDomNode = div2
            ]

            div [
                fragment [ div2 ]
                fragment [ ] // PrevDomNode = div2
            ]

            div [
                fragment [
                    fragment [ ] // PrevDomNode = div2
                ]
            ]

            div [
                div2
                fragment [
                    fragment [ ] // PrevDomNode = div2
                ]
            ]

            div [

                fragment [
                    fragment [
                        // Prev = null
                    ]
                ]
            ]
        *)
        member this.PrevDomNode =
            let result =
                match this.PrevNode with
                | DomNode n -> n
                | GroupNode v ->
                    match v.LastDomNode with
                    | null -> v.PrevDomNode
                    | n -> n
                | EmptyNode -> // We're the first child
                    match this.Parent with
                    | GroupNode pv -> pv.PrevDomNode
                    | _ -> null
            log($"PrevDomNode of {this} -> '{nodeStr result}' PrevNode={this.PrevNode}")
            result

        (*
            div [
                fragment [ ] // NextDomNode = null, PrevDomNode = null
            ]
            div [
                fragment [ div1 ] // NextDomNode = null, PrevDomNode = null
            ]
            div [
                fragment [ ] // NextDomNode = div2, PrevDomNode = null
                div2
            ]
            div [
                fragment [ div1 ] // NextDomNode = div2, PrevDomNode = null
                div2
            ]
            div [
                div0
                fragment [ ] // NextDomNode = div2, PrevDomNode = div0
                div2
            ]
            div [
                div0
                fragment [ div1 ] // NextDomNode = div2, PrevDomNode = div0
                div2
            ]
            div [
                div0
                fragment [ ] // NextDomNode = null, PrevDomNode = div0
            ]
            div [
                div0
                fragment [ div1 ] // NextDomNode = null, PrevDomNode = div0
            ]
        *)

        member this.NextDomNode =
            //log($"NextDomNode this={this}")
            let result =
                match this.DomNodes() with
                // We don't have any nodes.
                | [] ->
                    //log("-- We have no nodes")
                    match this.PrevDomNode with
                    | null -> // No DOM node before us, so our next node must be parent DOM node's first child
                        //log("-- PrevDomNode is null")
                        match parentDomNode() with
                        | null ->
                            //log("-- parent DOM node is null")
                            null
                        | p ->
                            //log("-- parent's first child, since no nodes before us, and we don't have any nodes ourself")
                            p.firstChild
                    | prev ->
                        //log($"-- our next node is our PrevDomNode's next sibling (prev is {nodeStr prev})")
                        prev.nextSibling

                // We do have nodes, so next node is last node's next sibling
                | ns ->
                    match ns |> List.last with
                    | null ->
                        //log("-- Last node was null")
                        null
                    | last ->
                        //log("-- NextDomNode is nextSibling of our last node")
                        last.nextSibling
            //log($"NextDomNode of {this} -> '{nodeStr result}'")
            result

        member this.FirstDomNode =
            match this.DomNodes() with
            | [] -> null
            | n :: ns -> n

        member this.LastDomNode =
            match this.DomNodes() with
            | [] -> null
            | ns -> ns |> List.last

        member this.FirstDomNodeInOrAfter =
            match this.FirstDomNode with
            | null -> this.NextDomNode
            | first -> first

        //member this.ParentDomNode = parentDomNode()
        member this.MapParent<'T>( f : (Node -> 'T)) =
            f(parentDomNode())

        member private this.OwnX( n : Node ) =
            Interop.set n "__sutil_snode" this

        member private this.OwnX( child : SutilNode ) =
            match child with
            | DomNode n -> this.OwnX(n)
            | _ -> ()

        static member GroupOf (n : Node) : NodeGroup option =
            Interop.getOption n "__sutil_snode"

        static member GroupsOf (n : Node) : NodeGroup list =
            let rec parentsOf (r:NodeGroup list) =
                match r with
                | [] -> r
                | x::xs ->
                    match x.Parent with
                    | GroupNode g -> parentsOf (g :: r)
                    | _ -> r

            let init n =
                match Interop.getOption n "__sutil_snode" with
                | None -> []
                | Some g -> [g]

            parentsOf (init n)

        member this.Clear() =
            // TODO clean up each node
            _children <- []

        member private this.AddChild (child:SutilNode) =
            this.OwnX(child)
            _children <- _children @ [ child ]
            updateChildrenPrev()

        (*
            fragment [
                A
            ]
            fragment [
                fragment [
                    X
                ]
            ]

            div [
                fragment [
                    A
                    B
                ]
                fragment [
                    X
                ]
            ]
        *)
        member this.AppendChild(child : SutilNode) =
            //log($"NodeGroup.AppendChild: this='{this.Name} #{this.Id}' child='{child}' parent='{this.Parent}' prevDom='{nodeStrShort this.PrevDomNode}'")
            match this.Parent with
            |EmptyNode -> ()
            |_  ->
                let cn = this.DomNodes() |> List.map nodeStrShort
                let pn = this.PrevNode.DomNodes() |> List.map nodeStrShort
                let parent = parentDomNode()
                let before = this.NextDomNode
                child.collectDomNodes() |> List.iter (fun ch ->
                    //log($"AppendChild: insertBefore: {nodeStrShort ch} before {nodeStrShort before} prev={this.PrevNode}")
                    DomEdit.insertBefore parent ch before)
            //this.AddChild(child)
            this.OwnX(child)
            _children <- _children @ [ child ]
            updateChildrenPrev()

        member this.FirstChild =
            match _children with
            | [] -> EmptyNode
            | x :: xs -> x

        member this.LastChild =
            match _children with
            | [] -> EmptyNode
            | xs -> xs |> List.last

        member private this.ChildAfter (prev : SutilNode) =
            log($"ChildAfter: prev='{prev}' children={childStrs()} this='{this}'")
            match prev with
            | EmptyNode -> this.FirstChild
            | _ ->
                let rec find (list : SutilNode list) =
                    match list with
                    | [] ->
                        log($"Did not find {prev}")
                        EmptyNode
                    | x :: [] when x.IsSameNode(prev) ->
                        log($"Found {x} at end of list -> EmptyNode")
                        EmptyNode
                    | x :: y :: _ when x.IsSameNode(prev) ->
                        log($"Found {y} after {x}")
                        y
                    | x :: xs ->
                        log($"Found {x} but not equal to {prev}")
                        find xs
                find _children

        member this.InsertAfter (child : SutilNode, prev : SutilNode ) =
            this.InsertBefore( child, this.ChildAfter(prev) )

        (*
            insert 'div' into fragment#2 after  <empty>
            <DIV> #0
               <'bind'> #1
                   <'fragment'> #2
               <'bind'> #5
                   <'fragment'> #6
                       <DIV> #7
                           'Binding 2'
        *)

        member private this.InsertBefore (child : SutilNode, refNode : SutilNode ) =
            let refDomNode =
                match refNode with
                | EmptyNode -> this.NextDomNode
                | _ -> refNode.FirstDomNodeInOrAfter

            log($"InsertBefore: child='{child}' before '{refNode}' refDomNode='{nodeStrShort refDomNode}' child.PrevNode='{child.PrevNode}'")
            let parent = parentDomNode()
            let len = _children.Length


            for dnode in child.collectDomNodes() do
                DomEdit.insertBefore parent dnode refDomNode

            if refNode = EmptyNode then
                this.AddChild(child)
            else
                _children <- _children |> List.fold (fun list ch ->
                    match ch with
                    | n when n.IsSameNode(refNode) -> list @ [child] @ [ n ]
                    | _ -> list @ [ch]
                    ) []

                this.OwnX(child)

            updateChildrenPrev()
            log($"InsertBefore: child='{child}' refNode='{nodeStrShort refDomNode}' child.PrevNode='{child.PrevNode}'")

            if _children.Length = len then
                log($"Error: Child was not added")

        member _.RemoveChild (child:SutilNode) =
            let rec rc (p:NodeGroup) (c:SutilNode) =
                match c with
                | EmptyNode -> ()
                | DomNode n ->
                    unmount n
                | GroupNode g ->
                    g.Children |> List.iter (fun gc -> g.RemoveChild(gc))
                    g.Dispose()

            let newChildren = _children |> List.filter (fun n -> n <> child)
            //child.collectDomNodes() |> List.iter (fun c -> DomEdit.removeChild c.parentNode c)
            rc this child
            _children <- newChildren
            updateChildrenPrev()

        member this.ReplaceChild (child:SutilNode, oldChild:SutilNode, insertBefore : Node) =
            let deleteOldNodes() =
                let oldNodes = oldChild.collectDomNodes()

                oldNodes |> List.iter (fun c ->
                    if (isNull c.parentNode) then // We were unexpectedly removed from the DOM by something else (perhaps)
                        log($"Node has no parent: {nodeStrShort c}")
                    else
                        DomEdit.removeChild c.parentNode c)

            //log($"ReplaceChild: {oldChild} with {child} before {nodeStrShort insertBefore}")
            let nodes = child.collectDomNodes()

            assertTrue (child <> EmptyNode) "Empty child for replace child"

            if oldChild <> EmptyNode then
                assertTrue (_children |> List.exists (fun c -> c.Id = oldChild.Id) ) "Child not found"
                child.Id <- oldChild.Id

            //let insertBefore = match oldNodes with |[] -> null |_ -> (oldNodes |> List.last).nextSibling
            let parent = parentDomNode()

            nodes |> List.iter (fun n ->
                //log($"insertBefore {nodeStrShort n} before {nodeStrShort insertBefore} on parent {nodeStrShort parent}")
                DomEdit.insertBefore parent n insertBefore
                )

            deleteOldNodes()

            if isNull insertBefore || oldChild = EmptyNode then
                this.AddChild child
            else
                this.OwnX child
                _children <- _children |> List.map (fun n -> if n.Id = oldChild.Id then child else n)

            updateChildrenPrev()

        member _.Name = _name
        member _.Id
            with get() = id and set id' = id <- id'
        member _.Children = _children
        member _.SetDispose d = _dispose <- d
        member _.Dispose() = _dispose()

type BuildResult = SutilNode

/// Specific operation for BuildContext.AddChild.
type DomAction =
    | Append  // appendChild
    | Replace of SutilNode*Node // bindings use this to replace the previous DOM fragment

type BuildContext =
    {
        Document : Browser.Types.Document
        Parent   : SutilNode
        Previous : SutilNode
        Action   : DomAction  // Consider making this "SvId option" and then finding node to replace
        // Naming service
        MakeName : (string -> string)
        Debug : bool
        // Style context
        StyleSheet : NamedStyleSheet option }
    with
        //member this.Document = this.Parent.Document
        member this.ParentElement : HTMLElement = this.Parent.AsDomNode :?> HTMLElement
        member this.ParentNode : Node = this.Parent.AsDomNode
        member ctx.AddChild (node: SutilNode) : unit =
            match ctx.Action with
            | Append ->
                log $"ctx.Append '{node}' to '{ctx.Parent}' after {ctx.Previous}"
                ctx.Parent.InsertAfter(node,ctx.Previous)

            | Replace (existing,insertBefore)->
                log $"ctx.Replace '{existing}' with '{node}' before '{nodeStrShort insertBefore}'"
                ctx.Parent.ReplaceGroup(node,existing,insertBefore)

            ()

// Private so that we must use build to instantiate the DOM fragment.
type SutilElement = private { Builder: BuildContext -> BuildResult }

let nodeFactory f = { Builder = f }

let private makeContext (parent:Node) =
    let gen = Helpers.makeIdGenerator()
    {
        Document = parent.ownerDocument
        Parent = DomNode parent
        Previous = EmptyNode
        Action = Append
        StyleSheet = None
        Debug = false
        MakeName = fun baseName -> sprintf "%s-%d" baseName (gen())
    }

module ContextHelpers =
    let withStyleSheet sheet ctx : BuildContext =
        { ctx with StyleSheet = Some sheet }

    let withDebug ctx : BuildContext =
        { ctx with Debug = true }

    let withParent parent ctx : BuildContext =
        { ctx with Parent = parent; Action=Append}

    let withPrevious prev ctx : BuildContext =
        { ctx with Previous = prev }

    let withParentNode parent ctx : BuildContext =
        withParent (DomNode parent) ctx

    let withReplace (toReplace:SutilNode,before:Node) ctx =
        { ctx with Action = Replace (toReplace,before) }

type Fragment = Node list

let domResult (node:Node) = DomNode node
let sutilResult (node:SutilNode) = node

let unitResult(ctx, name)  =
    let text () =
            let tn = ctx.Document.createTextNode name
            let d = ctx.Document.createElement("div")
            DomEdit.appendChild d tn
            ctx.AddChild (DomNode d)
            d
    if ctx.Debug then DomNode (text()) else EmptyNode

let errorNode (parent:SutilNode) message : Node=
    let doc = parent.Document
    let d = doc.createElement("div")
    DomEdit.appendChild d (doc.createTextNode($"sutil-error: {message}"))
    parent.AppendChild(d)
    d.setAttribute("style", "color: red; padding: 4px; font-size: 10px;")
    upcast d

let collectFragment (result : BuildResult) = result

let appendAttribute (e:Element) attrName attrValue =
    if (attrValue <> "") then
        let currentValue = e.getAttribute(attrName)
        e.setAttribute(attrName,
            if ((isNull currentValue) || currentValue = "")
                then attrValue
                else (sprintf "%s %s" currentValue attrValue))

// TODO: We can make a better parser using combinators. This lets me prove this idea tbough
// Don't judge me
let rec internal parseSelector (source:string) : CssRules.CssSelector =
    let trimQuotes (s:string) = s.Trim().Trim( [| '\''; '"' |])

    let rec parseSingle (token : string) =
        if token.StartsWith(".")
            then CssRules.Cls (token.Substring(1))
        else if token.StartsWith("#")
            then CssRules.Id (token.Substring(1))
        else if token.Contains(":") || token.Contains(">") || token.Contains("[")
            then CssRules.NotImplemented
        else
            CssRules.Tag (token.ToUpper())

    let rec parseAttr (token : string) =
        if token.Contains("[") && token.EndsWith("]")
            then
                let i = token.IndexOf('[')
                let single = parseSingle(token.Substring(0,i).Trim())
                let attrExpr = token.Substring(i+1, token.Length - i - 2)
                let attrTokens = attrExpr.Split([|'='|], 2)
                if attrTokens.Length = 2 then
                    CssRules.Attr (single, attrTokens.[0].Trim(), attrTokens.[1] |> trimQuotes )
                else
                    CssRules.NotImplemented
            else parseSingle token

    let rec parseAll (token : string) =
        let spacedItems = token.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
        if (spacedItems.Length = 1)
            then parseAttr spacedItems.[0]
            else spacedItems |> Array.map parseAttr |> Array.toList |> CssRules.Any

    let items = source.Split(',')
    if items.Length = 1
        then parseAll items.[0]
        else items |> Array.map parseAll |> Array.toList |> CssRules.All

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
            |> List.filter (fun cls -> cls.StartsWith("sutil"));
    classes

let rec applyCustomRules (namedSheet:NamedStyleSheet) (e:HTMLElement) =
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
                    applyCustomRules root e
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

let build (f : SutilElement) (ctx : BuildContext) =
    let result = f.Builder ctx
    result.collectDomNodes() |> List.iter (fun n -> dispatchSimple n Event.Mount)
    result

let asDomNode (element:SutilNode) (ctx : BuildContext) : Node =
    //let result = (ctx |> build element)
    match element.collectDomNodes()  with
    | [n] -> n
    | [] -> errorNode ctx.Parent $"Error: Empty node from {element} #{element.Id}"
    | xs ->
        let doc = ctx.Document
        let tmpDiv = doc.createElement("div")
        let en = errorNode (DomNode tmpDiv) "'fragment' not allowed as root for 'each' blocks"
        DomEdit.appendChild tmpDiv en
        ctx.Parent.AppendChild tmpDiv
        xs |> List.iter (fun x -> DomEdit.appendChild tmpDiv x)
        upcast tmpDiv

let asDomElement (element : SutilNode) (ctx : BuildContext): HTMLElement =
    let node = asDomNode element ctx
    if isElementNode node then
        downcast node
    else
        let doc = ctx.Document
        let span = doc.createElement("span")
        DomEdit.appendChild span node
        ctx.Parent.AppendChild span
        span


let findSvIdElement (doc : Document) id : HTMLElement =
    downcast doc.querySelector($"[_svid='{id}']")

let splitBySpace (s:string) = s.Split([|' '|],StringSplitOptions.RemoveEmptyEntries)

let addToClasslist classes (e:HTMLElement) =
    e.classList.add( classes |> splitBySpace )

let removeFromClasslist classes (e:HTMLElement) =
    e.classList.remove( classes |> splitBySpace )

let setAttribute (el:HTMLElement) (name:string) (value:obj) =
    let svalue = string value
    if name = "class" then
        el |> addToClasslist svalue
    else  if name = "class-" then
        el |> removeFromClasslist svalue
    else if svalue = "false" && (name = "disabled" || name = "readonly" || name = "required") then
        el.removeAttribute( name )
    else if name = "value" then
        Interop.set el "__value" value // raw value
        Interop.set el "value" svalue //
    else
        el.setAttribute( name, svalue )

    //if (name = "value") then
    //    Interop.set el "__value" value // Un-stringified version of value

let attr (name,value:obj) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.Parent.AsDomNode
    try
        let e = parent :?> HTMLElement

        setAttribute e name value

        match ctx.StyleSheet with
        | Some namedSheet ->
            applyCustomRules namedSheet e
        | None -> ()

    with _ -> invalidOp (sprintf "Cannot set attribute %s on a %A %f %s" name parent parent.nodeType (parent :?> HTMLElement).tagName)
    unitResult(ctx, "attr")

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
    if isNull node then null else if condition node then node else firstSiblingWhere (node.nextSibling) condition

let firstChildWhere (node:Node) (condition:Node -> bool) =
    firstSiblingWhere node.firstChild condition

let rec lastSiblingWhere (node:Node) (condition:Node -> bool) =
    if isNull node then
        null
    else if (condition node && (isNull node.nextSibling || not (condition node.nextSibling))) then
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


//let registeredDisposables = new System.Collections.Generic.Dictionary<IDisposable,Node>()

let disposeOnUnmount (ds : IDisposable list) = nodeFactory <| fun ctx ->
    ds |> List.iter (fun d-> SutilNode.RegisterDisposable(ctx.Parent,d))
    unitResult(ctx, "disposeOnUnmount")

let unsubscribeOnUnmount (ds : (unit->unit) list) = nodeFactory <| fun ctx ->
    ds |> List.iter (fun d -> SutilNode.RegisterUnsubscribe(ctx.Parent,d))
    unitResult(ctx, "unsubscribeOnUnmount")

let private updateCustom (el:HTMLElement) (name:string) (property:string) (value:obj) =
    let r = NodeKey.getCreate el name (fun () -> {| |})
    Interop.set r property value
    Interop.set el name r

let exclusive (f : SutilElement) = nodeFactory <| fun ctx ->
    log $"exclusive {ctx.Parent}"
    ctx.Parent.Clear()
    ctx |> build f

let hookContext (hook: BuildContext -> unit) : SutilElement = nodeFactory <| fun ctx ->
    hook ctx
    unitResult(ctx, "hookContext")

let hookParent (hook: Node -> unit) : SutilElement = nodeFactory <| fun ctx ->
    hook ctx.Parent.AsDomNode
    unitResult(ctx, "hookParent")

let addTransform (node:HTMLElement) (a : ClientRect) =
    let b = node.getBoundingClientRect()
    if (a.left <> b.left || a.top <> b.top) then
        let s = Window.getComputedStyle(node)
        let transform = if s.transform = "none" then "" else s.transform
        node.style.transform <- sprintf "%s translate(%fpx, %fpx)" transform (a.left - b.left) (a.top - b.top)
        log node.style.transform

let fixPosition (node:HTMLElement) =
    let s = Window.getComputedStyle(node)
    if (s.position <> "absolute" && s.position <> "fixed") then
        log $"fixPosition {nodeStr node}"
        let width  = s.width
        let height = s.height
        let a = node.getBoundingClientRect()
        node.style.position <- "absolute"
        node.style.width <- width
        node.style.height <- height
        addTransform node a

//let removeNode (node:#Node) =
//    log <| sprintf "removing node %A" node.textContent
//    DomEdit.removeChild node.parentNode node

let buildChildren(xs : seq<SutilElement>) (ctx:BuildContext) : unit =
    let e = ctx.Parent

    let mutable prev = EmptyNode

    //log($"buildChildren for {ctx.Parent}")

    // Effect 2
    for x in xs do
        //log($"  buildChildren: prev={prev}")
        match ctx |> ContextHelpers.withPrevious prev |> build x with
        | EmptyNode -> ()
        | r -> prev <- r

    // Effect 3
    match ctx.StyleSheet with
    | Some namedSheet ->
        e.AddClass(namedSheet.Name)
        e.AsDomNode |> applyIfElement (applyCustomRules namedSheet)
    | None -> ()

    ()

let fragment (elements : SutilElement seq) = nodeFactory <| fun ctx ->
    let v = NodeGroup("fragment",ctx.Parent,ctx.Previous)
    let fragmentNode = GroupNode v
    let oldId = v.Id
    log($"fragment action='{ctx.Action}' #" + v.Id)
    ctx.AddChild fragmentNode
    log($"fragment now #" + v.Id + " (was #" + oldId + $"). Parent={v.Parent} Prev={v.PrevNode}" )

    let childCtx = { ctx with Parent = fragmentNode; Action = Append }
    childCtx |> buildChildren elements

    sutilResult fragmentNode

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

let mountOn app host =
    build app (makeContext host)

let computedStyleOpacity e =
    try
        float (Window.getComputedStyle(e).opacity)
    with
    | _ ->
        log(sprintf "parse error: '%A'" (Window.getComputedStyle(e).opacity))
        1.0

let computedStyleTransform node =
    let style = Window.getComputedStyle(node)
    if style.transform = "none" then "" else style.transform

let declareResource<'T when 'T :> IDisposable> (init : unit -> 'T) (f : 'T -> unit) = nodeFactory <| fun ctx ->
    let r = init()
    SutilNode.RegisterDisposable(ctx.Parent,r)
    f(r)
    unitResult(ctx, "declareResource")

// ----------------------------------------------------------------------------
// Element builder with namespace

let elns ns tag (xs : seq<SutilElement>) : SutilElement = nodeFactory <| fun ctx ->

    let e : Element = if ns = "" then upcast ctx.Document.createElement(tag) else ctx.Document.createElementNS(ns, tag)
    let snodeEl = DomNode e

    // Considering packing these effects into pipeline that lives on ctx.
    // User can then extend the pipeline, or even re-arrange. No immediate
    // need for it right now.

    // Effect 1
    let id = domId()
    log $"create <{tag}> #{id}"
    setSvId e id

    ctx |> ContextHelpers.withParent snodeEl |> buildChildren xs

    ctx.AddChild (DomNode e)
    // Effect 4
    //appendReplaceChild e ctx |> ignore

    // Effect 5
    dispatchSimple e Event.ElementReady

    domResult e

// ----------------------------------------------------------------------------
// Element builder for DOM

let el tag (xs : seq<SutilElement>) : SutilElement = nodeFactory <| fun ctx ->
    let e : Element = upcast ctx.Document.createElement(tag)
    let snodeEl = DomNode e

    // Considering packing these effects into pipeline that lives on ctx.
    // User can then extend the pipeline, or even re-arrange. No immediate
    // need for it right now.

    // Effect 1
    let id = domId()
    log("create <" + tag + "> #" + string id)
    setSvId e id

    ctx |> ContextHelpers.withParent snodeEl |> buildChildren xs

    // Effect 4
    ctx.AddChild (DomNode e)

    // Effect 5
    dispatchSimple e Event.ElementReady

    domResult e

(*
let buildSolitaryElement (f : SutilElement) ctx : HTMLElement =
    log $"buildSolitaryElement: {ctx.Action}"
    let node = expectSolitary f ctx
    if isElementNode node then
        node :?> HTMLElement
    else
        let spanWrapper = el "span" [ nodeFactory <| (fun _ -> nodeResult node) ]
        (expectSolitary spanWrapper ctx) :?> HTMLElement
*)
let inject (elements : SutilElement seq) (element : SutilElement) = nodeFactory <| fun ctx ->
    let e = build element ctx
    e.collectDomNodes() |> List.iter (fun n -> ctx |> ContextHelpers.withParent (DomNode n) |> buildChildren elements |> ignore)
    e

let setValue<'T> (key : string) (value : 'T) = nodeFactory <| fun ctx ->
    Interop.set ctx.ParentNode key value
    unitResult(ctx, "setValue")

// ----------------------------------------------------------------------------
// Text node

let textNode (doc : Document) value : Node =
    let id = domId()
    log $"create \"{value}\" #{id}"
    let n = doc.createTextNode(value)
    setSvId n id
    upcast n

let text value : SutilElement =
    nodeFactory <| fun ctx ->
        let tn = textNode ctx.Document value
        ctx.AddChild (DomNode tn)
        domResult tn

// ----------------------------------------------------------------------------
// Raw html node

let html text : SutilElement = nodeFactory <| fun ctx ->
    ctx.Parent.AsDomNode |> applyIfElement (fun el ->
        el.innerHTML <- text
        match ctx.StyleSheet with
        | None -> ()
        | Some ns -> visitElementChildren el (fun ch ->
                                            ch.classList.add ns.Name
                                            applyCustomRules ns ch))
    sutilResult <| ctx.Parent
