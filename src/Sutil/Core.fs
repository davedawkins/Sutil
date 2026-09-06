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

/// <summary>
/// Registration points for per-node cleanup. Disposables registered here run when the node
/// unmounts. This is the only survivor of the old <c>SutilEffect</c> DU: bindings now anchor on a
/// comment node and build results are plain <c>Node[]</c>, so effects no longer need a type (#896).
/// </summary>
[<AbstractClass; Sealed>]
type SutilEffect =
    static member RegisterDisposable(node: Node, d: IDisposable) : unit =
        Interop.set node NodeKey.Disposables (d :: getDisposables (node))

    static member RegisterUnsubscribe(node: Node, d: unit -> unit) : unit =
        SutilEffect.RegisterDisposable(node, Helpers.disposable d)

module internal MountListenersInternal =
    let mutable private nextOnMountId = 0
    let mutable private onMountListeners : Map<int, Node -> bool -> unit> = Map.empty

    let internal notifyOnMountListeners (node : Node, isRoot : bool) =
        onMountListeners |> Seq.toArray |> Array.iter (fun kv -> kv.Value (node) isRoot)

    let internal onMount<'T when 'T :> Node> (f : 'T -> bool -> unit) : (unit -> unit) =
        let _id = nextOnMountId
        nextOnMountId <- nextOnMountId + 1
        onMountListeners <- onMountListeners.Add(_id, unbox f)
        (fun _ ->
            if onMountListeners.ContainsKey _id then
                onMountListeners <- onMountListeners.Remove(_id)
        )

[<Erase>]
type MountListeners() =
    static member OnMountWithIsRoot<'T when 'T :> Node>( f : 'T -> bool -> unit ) : (unit -> unit) =
        MountListenersInternal.onMount f

    static member OnMount<'T when 'T :> Node>( f : 'T -> unit ) : (unit -> unit) =
        MountListeners.OnMountWithIsRoot( fun node isRoot -> f node )

    /// findNode takes the just-mounted node and returns either null or the matching node
    /// None will be passed
    static member WaitUntil<'T when 'T :> Node>( matcher : 'T -> 'T option, f : 'T -> unit ) : (unit -> unit) =
        let mutable stop = ignore
        let mutable disposed = false

        let dispose() =
            if not disposed then
                stop()
                disposed <- true

        let matched (node : 'T) =
            if not disposed then
                dispose()
                f node

        let tryMatch (mounted : 'T) (succ : 'T -> unit) =
            matcher mounted |> Option.iter succ

        stop <- MountListenersInternal.onMount( fun mounted _ -> tryMatch mounted matched )
        dispose

    static member WaitUntil( selector : string, f : Node -> unit ) : (unit -> unit) =

        let findNode( _ ) : Node option =
            let node = document.querySelector(selector)
            if isNull node then None else Some node

        match findNode(null) with
        | Some matched ->
            f matched
            ignore
        | None ->
            MountListeners.WaitUntil( findNode, f )

let private notifySutilEvents (parent : Node) (onMountElements : ResizeArray<HTMLElement>) =
    if not (isNull (box onMountElements)) && nodeIsConnected parent then
        let _nodes = onMountElements.ToArray()
        onMountElements.Clear()
        _nodes |> Array.iter (fun n ->
                CustomDispatch<_>.dispatch(n,Event.Connected)
                CustomDispatch<_>.dispatch(n,Event.Mount)
                MountListenersInternal.notifyOnMountListeners(n, true)
            )

type PipelineFn = (BuildContext * Node[]) ->  (BuildContext * Node[])

/// <exclude/>
and  BuildContext =
    { Document: Browser.Types.Document
      /// The DOM node new content inserts into.
      Parent: Node
      /// New content inserts immediately before this node; null appends. A binding passes its
      /// anchor here, which is the whole replacement for the old sibling-walking arithmetic (#896).
      Before: Node
      /// The node owning registrations made at this build position: the parent element normally,
      /// a fragment's marker or a binding's anchor inside those, so registrations keep the
      /// lifetime the old group tree gave them (#896).
      Host: Node
      MakeName: (string -> string)
      Class: string option
      OnMount: ResizeArray<HTMLElement>
      Debug: bool
      Pipeline : PipelineFn
      }

    member this.ParentElement: HTMLElement = this.Parent :?> HTMLElement
    member this.ParentNode: Node = this.Parent

    member ctx.AddChild(node: Node) : unit =
        if logEnabled() then log $"ctx.AddChild '{nodeStrShort node}' to '{nodeStrShort ctx.Parent}' before {nodeStrShort ctx.Before}"
        DomEdit.insertBefore ctx.Parent node ctx.Before
        notifySutilEvents ctx.Parent (ctx.OnMount)

let internal domResult (node: Node) : Node[] = [| node |]

let internal sideEffect (ctx, name) : Node[] =
    if ctx.Debug then
        let tn = ctx.Document.createTextNode name
        let d = ctx.Document.createElement ("div")
        DomEdit.appendChild d tn
        ctx.AddChild(d :> Node)
        [| d :> Node |]
    else
        [||]

/// <summary>
/// Sutil's element type. This is an abstraction of DOM elements, attributes, events, etc.
/// The type itself is a function that maps <c>BuildContext</c> to the top-level DOM nodes produced,
/// wrapped in a private record to isolate users from implementation details as much as possible:
/// <code>
/// type SutilElement = private { Builder: BuildContext -> Node[] }
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
type SutilElement private (name : string, children : seq<SutilElement>, builder : BuildContext -> Node[]) =
    do ()
    static member Define( builder : BuildContext -> Node[] ) =
        SutilElement( "", [], builder )

    static member Define( name : string, builder : BuildContext -> Node[] ) =
        SutilElement( name, [], builder )

    static member Define( name : string, builder : BuildContext -> unit ) =
        SutilElement( name, [], (fun ctx -> ctx |> builder;  sideEffect(ctx, name)) )

    static member Define( name : string, children : seq<SutilElement>, builder : BuildContext -> Node ) =
        SutilElement( name, children, fun ctx -> [| ctx |> builder |] )

    member internal __.Builder = builder

let private defaultContext (parent : Node) =
    let gen = Helpers.makeIdGenerator ()

    { Document = parent.ownerDocument
      Parent = parent
      Before = null
      Host = parent
      Class = None
      OnMount = Unchecked.defaultof<_>
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

          OnMount = new ResizeArray<_>()

          // Ensures that if we create and mount DOM nodes onto a styled element, then
          // we inherit the stylesheet class on the mounted nodes
          Class =
            parent :?> HTMLElement
            |> Option.ofObj
            |> Option.bind (fun e -> getSutilClasses e |> List.tryHead)
    }

module ContextHelpers =
    let withStyleSheet sheet ctx : BuildContext = ctx //{ ctx with StyleSheet = Some sheet }

    let withDebug ctx : BuildContext = { ctx with Debug = true }

    let withPreProcess f (ctx:BuildContext) = { ctx with Pipeline = (f>>ctx.Pipeline) }
    let withPostProcess f (ctx:BuildContext) = { ctx with Pipeline = (ctx.Pipeline>>f) }

    let withParent (parent: Node) ctx : BuildContext =
        { ctx with
            Parent = parent
            Before = null
            Host = parent }

    let withParentNode (parent: Node) ctx : BuildContext = withParent parent ctx

    /// Position subsequent builds immediately before the binding's anchor. Reads the anchor's
    /// parent at call time, so a moved anchor (shadow root, external re-parenting) stays correct (#896).
    let withAnchor (anchor: Node) ctx : BuildContext =
        { ctx with
            Parent = anchor.parentNode
            Before = anchor
            Host = anchor }

let internal errorNode (parent: Node) message : Node =
    let doc = documentOf parent
    let d = doc.createElement ("div")
    DomEdit.appendChild d (doc.createTextNode ($"sutil-error: {message}"))
    DomEdit.appendChild parent d
    d.setAttribute ("style", "color: red; padding: 4px; font-size: 10px;")
    upcast d

/// <summary>
/// Instantiate a <c>SutilElement</c>, returning the top-level DOM nodes it produced. Anchors in
/// the result stand for bindings and hold their rendered content (see <c>DomHelpers.getBindNodes</c>).
/// Every node in the result keeps its identity for the lifetime of its binding or mount (#896).
/// </summary>
let build (f: SutilElement) (ctx: BuildContext) : Node[] =
    (ctx, f.Builder ctx)
    |> ctx.Pipeline
    |> snd

let internal buildOnly (f: SutilElement) (ctx: BuildContext) : Node[] =
    f.Builder ctx

let internal buildChildren (xs: seq<SutilElement>) (ctx: BuildContext) : unit =
    for x in xs do
        build x ctx |> ignore

/// <exclude/>
[<Global>]
type ShadowRoot() =
    member internal this.appendChild(el: Browser.Types.Node) = jsNative

let internal mountOnShadowRoot app (host: Node) : (unit -> unit) =
    // Build into a fragment so nothing attaches to the host itself; anchors move with
    // their content, so later rebinds still position correctly inside the shadow root (#896).
    let frag = host.ownerDocument.createDocumentFragment ()
    let nodes = build app { defaultContext host with Parent = (frag :> Node); OnMount = ResizeArray<_>() }

    if Array.isEmpty nodes then
        failwith "Custom components must return at least one node"

    let shadowRoot: ShadowRoot = host?shadowRoot

    while not (isNull frag.firstChild) do
        shadowRoot.appendChild (frag.firstChild)

    let dispose () =
        nodes |> Array.iter removeNode

    dispose

let internal mount app ((op,eref) : MountPoint) : IDisposable =
    let node = eref.AsElement

    let nodes =
        match op with
        | AppendTo ->
            build app (makeContext node)

        | InsertAfter ->
            build app { (makeContext node.parentElement) with Before = node.nextSibling }

    Helpers.disposable (fun () -> nodes |> Array.iter removeNode)
