/// <summary>
/// The core <c>SutilElement</c>s required for building DOM, setting attributes, parsing HTML, managing resources etc.
/// </summary>
module Sutil.CoreElements

open System
open Sutil
open Sutil.DomHelpers
open Sutil.Core
open Browser.Types
open Browser.Dom

let private makeElementWithSutilId (doc : Browser.Types.Document) (tag : string) (ns : string) =
    let e: Element = (if ns <> "" then doc.createElementNS (ns, tag) else  document.createElement (tag))
    let id = domId ()
    log ("create <" + tag + "> #" + string id)
    setSvId e id
    e

/// <summary>
/// Dispose all given items when the parent <c>SutilElement</c> is unmounted. Each item should implement <c>System.IDisposable</c>.
///
/// See also: <seealso cref="M:Sutil.Core.unsubscribeOnUnmount"/>
/// </summary>
let disposeOnUnmount (ds: IDisposable list) =
    defineSutilElement
    <| fun ctx ->
        ds
        |> List.iter (fun d -> SutilEffect.RegisterDisposable(ctx.Parent, d))

        sideEffect (ctx, "disposeOnUnmount")


/// <summary>
/// Call each function of type `(unit -> unit)` when the element is unmounted
/// </summary>
let unsubscribeOnUnmount (ds: (unit -> unit) list) =
    defineSutilElement
    <| fun ctx ->
        ds
        |> List.iter (fun d -> SutilEffect.RegisterUnsubscribe(ctx.Parent, d))

        sideEffect (ctx, "unsubscribeOnUnmount")


/// <summary>
/// Remove all existing DOM children before constructing the given `SutilElement`
/// </summary>
let exclusive (f: SutilElement) =
    defineSutilElement
    <| fun ctx ->
        log $"exclusive {ctx.Parent}"
        ctx.Parent.Clear()
        ctx |> build f

/// <summary>
/// Provides a hook for the build context. If you need to use this, please log an issue in the github repo for Sutil :-)
/// </summary>
let hookContext (hook: BuildContext -> unit) : SutilElement =
    defineSutilElement
    <| fun ctx ->
        hook ctx
        sideEffect (ctx, "hookContext")

/// <summary>
/// Provides a hook for the parent DOM Node
/// </summary>
let hookParent (hook: Node -> unit) : SutilElement =
    defineSutilElement
    <| fun ctx ->
        ctx.ParentNode |> hook
        sideEffect (ctx, "hookParent")

/// <summary>
/// Provides a hook for the parent <c>HTMLElement</c>. This can be used, for example, to mount a React component. See https://sutil.dev/#documentation-hosting-react
/// This will throw an <c>InvalidCastException</c> if the parent node is not an <c>HTMLElement</c>
/// </summary>
let hookElement (hook: HTMLElement -> unit) =
    defineSutilElement
    <| fun ctx ->
        ctx.ParentElement |> hook
        sideEffect (ctx, "host")

/// <summary>
/// Backwards compatibility. Obsolete
/// </summary>
let host = hookElement

let internal setClass (name: string) =
    hookElement (ClassHelpers.setClass name)

let toggleClass (name: string) =
    hookElement (ClassHelpers.toggleClass name)

let addClass (name: string) =
    hookElement (ClassHelpers.addToClasslist name)

let removeClass (name: string) =
    hookElement (ClassHelpers.removeFromClasslist name)

// ----------------------------------------------------------------------------
// Element builder with namespace

let internal elns ns tag (xs: seq<SutilElement>) : SutilElement =
    defineSutilElement
    <| fun ctx ->
        let e: Element = makeElementWithSutilId ctx.Document tag ns
//        Fable.Core.JS.console.log(buildLevelStr(), "++ making ", nodeStrShort e)
        let snodeEl = DomNode e

        ctx
        |> ContextHelpers.withParent snodeEl
        |> buildChildren xs

        ctx.AddChild(DomNode e)

        // Effect 5
        dispatchSimple e Event.ElementReady

//        Fable.Core.JS.console.log(buildLevelStr(), "-- returning ", nodeStrShort e)

        domResult e
// ----------------------------------------------------------------------------
// Element builder for DOM

let internal el tag (xs: seq<SutilElement>) : SutilElement = elns "" tag xs

let internal keyedEl (tag: string) (key: string) (init: seq<SutilElement>) (update: seq<SutilElement>) =
    defineSutilElement
    <| fun ctx ->
        let e: Element =
            let existing = ctx.Document.getElementById key

            if existing <> null then
                existing
            else
                let svid = domId ()
                log ("create <" + tag + "> #" + string id)
                let e' = ctx.Document.createElement (tag)

                ctx
                |> ContextHelpers.withParent (DomNode e')
                |> buildChildren init

                setSvId e' svid
                e'.setAttribute ("id", key)
                e'

        // Considering packing these effects into pipeline that lives on ctx.
        // User can then extend the pipeline, or even re-arrange. No immediate
        // need for it right now.

        // Effect 1
        ctx
        |> ContextHelpers.withParent (DomNode e)
        |> buildChildren update

        if e.parentElement = null then
            // Effect 40
            ctx.AddChild(DomNode e)
            // Effect 5
            dispatchSimple e Event.ElementReady

        domResult e

let internal elAppend selector (xs: seq<SutilElement>) : SutilElement =
    defineSutilElement
    <| fun ctx ->
        let e: Element = ctx.Document.querySelector (selector)

        if isNull e then
            failwith ("Not found " + selector)

        let snodeEl = DomNode e

        let id = domId ()
        log ("append <" + selector + "> #" + string id)
        setSvId e id

        ctx
        |> ContextHelpers.withParent snodeEl
        |> buildChildren xs

        sideEffect (ctx, "elAppend")

/// Merge these `SutilElement`s with another `SutilElement`.
let inject (elements: SutilElement seq) (element: SutilElement) =
    defineSutilElement
    <| fun ctx ->
        let e = build element ctx

        e.collectDomNodes ()
        |> List.iter (fun n ->
            ctx
            |> ContextHelpers.withParent (DomNode n)
            |> buildChildren elements
            |> ignore)

        e

/// Create a TextNode
let internal text value : SutilElement =
    defineSutilElement
    <| fun ctx ->
        let tn = DomHelpers.textNode ctx.Document value
        ctx.AddChild(DomNode tn)
        domResult tn


/// Set a property on the parent DOM Node
let setProperty<'T> (key: string) (value: 'T) =
    hookParent (fun n -> Interop.set n key value)

/// Backwards compatibility. Obsolete
let setValue = setProperty

/// <summary>
/// An empty element. This could be considered the <c>unit</c> value for a <c>SutilElement</c>. It is very similar in effect <c>fragment []</c>, since
/// neither will add any HTMLElements. The main difference is that <c>nothing</c> will make no changes at all to the DOM, while <c>fragment</c> will
/// create an internal <c>SutilGroup</c> that is registered on the parent element as a property.
/// </summary>
let nothing =
    defineSutilElement
    <| fun ctx -> sideEffect (ctx, "nothing")

let internal attr (name, value: obj) : SutilElement =
    defineSutilElement
    <| fun ctx ->
        let parent = ctx.Parent.AsDomNode

        try
            let e = parent :?> HTMLElement

            setAttribute e name value

            // match ctx.StyleSheet with
            // | Some namedSheet -> applyCustomRules namedSheet e
            // | None -> ()

        with
        | _ ->
            invalidOp (
                sprintf
                    "Cannot set attribute '%s' = '%A' on a %A %f %s"
                    name
                    value
                    parent
                    parent.nodeType
                    (parent :?> HTMLElement).tagName
            )

        sideEffect (ctx, "attr")


/// <summary>
/// Raw html that will be parsed and added as a child of the parent element
/// </summary>
let html (text : string) : SutilElement = defineSutilElement <| fun ctx ->
        ctx.Parent.AsDomNode
        |> applyIfElement (fun el ->
            el.innerHTML <- text.Trim()

            ctx.Class
            |> Option.iter (fun cls -> visitElementChildren el (fun ch -> ClassHelpers.addToClasslist cls ch ))

            match Interop.get (ctx.ParentElement) (NodeKey.StyleClass) with
            | None -> ()
            | Some styleClass ->
                visitElementChildren el (fun ch ->
                    ClassHelpers.addToClasslist styleClass ch
                    //applyCustomRules ns ch
                )

            Event.notifyUpdated ctx.Document)

        let nodes = ctx.ParentNode.childNodes.toSeq() |> Seq.toArray

        if nodes.Length = 1 then
            nodes.[0] |> DomNode |> sutilResult
        else
            let group = SutilEffect.MakeGroup( "html", ctx.Parent, ctx.Previous )
            nodes |> Seq.iter (fun n -> group.AddChild(DomNode n))
            group |> Group |> sutilResult

//
// Builds the element and passes to post-processing function
//
let postProcess (f : SutilEffect -> SutilEffect) (view : SutilElement) : SutilElement = defineSutilElement <| fun ctx ->
    // let result = Core.build view ctx
    // ctx.Parent.AsDomNode |> applyIfElement f
    // result
    ctx |> build view |> f

let postProcessElements (f : HTMLElement -> unit) (view : SutilElement) : SutilElement =
    let helper (se : SutilEffect) =
        Fable.Core.JS.console.log("post", se.ToString())
        se.AsDomNode |> applyIfElement f
        se
    view |> postProcess helper

let listenToResize (dispatch: HTMLElement -> unit) : SutilElement = defineSutilElement <| fun ctx ->
    let parent : HTMLElement = ctx.ParentElement
    let notify() = dispatch parent

    once Event.ElementReady parent <| fun _ ->
        SutilEffect.RegisterDisposable(parent,(ResizeObserver.getResizer parent).Subscribe( notify ))
        rafu notify

    sideEffect(ctx,"listenToResize")

let subscribe (source : System.IObservable<'T>) (handler : BuildContext -> 'T -> unit) = defineSutilElement <| fun ctx ->
    let unsub = source.Subscribe( handler ctx )
    SutilEffect.RegisterDisposable(ctx.Parent,unsub)
    sideEffect(ctx,"subscribe")


open Fable.Core.JsInterop

let autofocus : SutilElement =
    defineSutilElement <| fun ctx ->
        let e = ctx.ParentElement
        DomHelpers.rafu (fun _ ->
            e.focus()
            e?setSelectionRange(99999,99999)
            )
        sideEffect(ctx, "autofocus")

// Attributes that are either keywords or core functions
let id' n          = attr("id",n)
let type' n        = attr("type",n)
let for' n         = attr("for",n)
let class' n       = attr("class",n)
let unclass n      = attr("class-", n)
let unclass' n     = attr("class-", n)

let style (cssAttrs: (string * obj) seq) =
    attr("style", cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat "")

let styleAppend (cssAttrs: (string * obj) seq) =
    attr("style+", cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat "")

// Events

type EventModifier =
    | Once
    | PreventDefault
    | StopPropagation
    | StopImmediatePropagation

let on (event : string) (fn : Event -> unit) (options : EventModifier list) = defineSutilElement <| fun ctx ->
    let el = ctx.ParentNode
    let rec h (e:Event) =
        for opt in options do
            match opt with
            | Once -> el.removeEventListener(event,h)
            | PreventDefault -> e.preventDefault()
            | StopPropagation -> e.stopPropagation()
            | StopImmediatePropagation -> e.stopImmediatePropagation()
        fn(e)
    el.addEventListener(event, h)
    SutilEffect.RegisterUnsubscribe( ctx.Parent,  fun _ -> el.removeEventListener(event,h) )
    sideEffect(ctx, "on")

let onCustomEvent<'T> (event: string) (fn: CustomEvent<'T> -> unit) (options: EventModifier list) =
    on event (unbox fn) options
let onKeyboard event (fn : KeyboardEvent -> unit) options =
    on event (unbox fn) options

let onMouse event (fn : MouseEvent -> unit) options =
    on event (unbox fn) options

let asElement<'T when 'T :> Node> (target:EventTarget) : 'T = (target :?> 'T)

let inline private _event x = (x :> obj :?> Event)

type InputEvent() =
    member x.event = _event x
    member x.inputElement =
        let _event x = (x :> obj :?> Event)
        asElement<HTMLInputElement> (_event x).target

let onInput (fn : InputEvent -> unit) options =
    on "input" (unbox fn) options

let onClick fn options = on "click" fn options

let onElementReady fn options = on Event.ElementReady fn options
let onMount fn options = on Event.Mount fn options
let onUnmount fn options = on Event.Unmount fn options
let onShow fn options = on Event.Show fn options
let onHide fn options = on Event.Hide fn options

let onKeyDown (fn : (KeyboardEvent -> unit)) options  = onKeyboard "keydown" fn options
let onMouseMove fn options  = onMouse "mousemove" fn options

let subscribeOnMount (f : unit -> (unit -> unit)) = onMount (fun e -> SutilEffect.RegisterUnsubscribe(asElement<Node>(e.target),f())) [Once]

