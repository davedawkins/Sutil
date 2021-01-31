module Sutil.Transition
// Adapted from svelte/transitions/index.js

open Browser.Dom
open Browser.CssExtensions
open Browser.Types
open Sutil.Styling
open Sutil.DOM
open System.Collections.Generic
open System
open Fable.Core

let log = Sutil.Logging.log "trans"

module LoopTasks =

    type private Task = { C : float -> bool; F : unit -> unit }

    type LoopTask = { Promise : JS.Promise<unit>; Abort: (unit -> unit) }

    let mutable private tasks = new HashSet<Task>()

    let rec runTasks(now) =
        tasks |> Array.ofSeq |> Array.iter (fun task ->
            if not (task.C(now)) then
                tasks.Remove(task) |> ignore
                task.F()
        )
        if tasks.Count <> 0 then
            raf runTasks |> ignore

    (**
     * For testing purposes only!
     *)
    let clearLoops =
        tasks.Clear()

    (**
     * Creates a new task that runs on each raf frame
     * until it returns a falsy value or is aborted
     *)
    let loop (callback: float -> bool) =
        let mutable task = Unchecked.defaultof<Task>

        if tasks.Count = 0 then
            raf runTasks |> ignore

        {
            Promise = Promise.create( fun fulfill _ ->
                task <- { C = callback;  F = fulfill }
                tasks.Add(task) |> ignore
            )

            Abort = fun _ -> tasks.Remove (task) |> ignore
        }

type TransitionProp =
    | Key of string
    | X of float
    | Y of float
    | Opacity of float
    | Delay of float
    | Duration of float
    | DurationFn of (float -> float)
    | Ease of (float -> float)
    | CssGen of (float -> float -> string )
    | Tick of (float -> float -> unit)
    | Speed of float
    | Fallback of TransitionBuilder

and Transition =
    {
        Key : string
        X : float
        Y : float
        Opacity : float
        Delay : float
        Duration : float
        DurationFn : (float->float) option
        Speed : float
        Ease : (float -> float)
        CssGen : (float -> float -> string ) option
        Tick: (float -> float -> unit) option
        Fallback: TransitionBuilder option
    } with
            static member Default = {
                Key =""
                X = 0.0
                Y = 0.0
                Delay = 0.0
                Opacity = 0.0
                Duration = 0.0
                DurationFn = None
                Speed = 0.0
                Ease = Easing.linear
                CssGen = None
                Fallback = None
                Tick = None }

and CreateTransition =
     unit -> Transition

and TransitionBuilder = TransitionProp list -> HTMLElement -> CreateTransition

type Animation = {
    From: ClientRect
    To: ClientRect
}

let mergeProps newerProps existingProps : TransitionProp list =
    existingProps @ newerProps

let withProps (userProps : TransitionProp list) (f : TransitionBuilder) : TransitionBuilder =
    fun (initProps : TransitionProp list) ->
        initProps |> mergeProps userProps |> f

type TransitionAttribute =
    | InOut of TransitionBuilder
    | In of TransitionBuilder
    | Out of TransitionBuilder

let overrideDuration d = if Sutil.DevToolsControl.Options.SlowAnimations then 10.0 * d else d
let overrideDurationFn fo = if Sutil.DevToolsControl.Options.SlowAnimations then (fo |> Option.map (fun f -> ((*)10.0 << f))) else fo

let private applyProp (r:Transition) (prop : TransitionProp) =
    match prop with
    | Delay d -> { r with Delay = d }
    | Duration d -> { r with Duration = d; DurationFn = None }
    | DurationFn fo -> { r with DurationFn = Some fo; Duration = 0.0 }
    | Ease f -> { r with Ease = f }
    | CssGen f -> { r with CssGen = Some f }
    | Tick f -> { r with Tick = Some f }
    | Speed s -> { r with Speed = s }
    | X n -> { r with X = n }
    | Y n -> { r with Y = n }
    | Opacity n -> { r with Opacity = n }
    | Key f -> { r with Key = f }
    | Fallback f -> { r with Fallback = Some f }

let applyProps (props : TransitionProp list) (tr:Transition) = props |> List.fold applyProp tr
let makeTransition (props : TransitionProp list) = applyProps props Transition.Default
let mapTrans (f: Transition -> TransitionProp list) t = applyProps (f t) t

let element (doc:Document) tag = doc.createElement(tag)

let mutable numActiveAnimations = 0
let mutable tasks : (unit -> unit) list = []
let mutable activeDocs : Map<int,Document> = Map.empty

let registerDoc (doc:Document) =
    activeDocs <- activeDocs.Add( doc.GetHashCode(), doc )
    log($"Active docs: {activeDocs.Count}")

let runTasks() =
    let copy = tasks
    tasks <- []
    if (copy.Length > 0) then
        log($"- - - Tasks: running {copy.Length} tasks - - - - - - - - - - - - - -")
    for f in copy do f()

let waitAnimationFrame tag f =
    let init = tasks.IsEmpty
    tasks <- f :: tasks
    log $"raf task {tag}"
    if init then
        window.requestAnimationFrame( fun _ ->
            log "Running raf tasks"
            runTasks()
        ) |> ignore

let getSutilStyleElement (doc : Document) =
    let mutable e = doc.querySelector("head style#__sutil_keyframes")
    if (isNull e) then
        e <- element doc "style"
        e.setAttribute("id", "__sutil_keyframes")
        doc.head.appendChild(e) |> ignore
    e

let dotSheet styleElem : CSSStyleSheet = Interop.get styleElem "sheet"

let getSutilStylesheet (doc : Document) =
    getSutilStyleElement doc |> dotSheet

let nextRuleId = Helpers.makeIdGenerator()

let toEmptyStr s = if System.String.IsNullOrEmpty(s) then "" else s

let createRule (node : HTMLElement) (a:float) (b:float) tr (uid:int) =
    registerDoc (documentOf node)

    let css = match tr.CssGen with
                | Some f -> f
                | None -> failwith "No CSS function supplied"

    if (tr.DurationFn.IsSome) then
        failwith "Duration function not permitted in createRule"

    let durn = tr.Duration |> overrideDuration

    let step = 16.666 / durn
    let mutable keyframes = [ "{\n" ];

    for p in [0.0 ..step.. 1.0] do
        let t = a + (b - a) * tr.Ease(p)
        keyframes <- keyframes @ [ sprintf "%f%%{%s}\n" (p * 100.0) (css t (1.0 - t)) ]

    let rule = keyframes @ [ sprintf "100%% {%s}\n" (css b (1.0 - b)) ] |> String.concat ""

    let name = sprintf "__sutil_%d" (if uid = 0 then nextRuleId() else uid)
    let keyframeText = sprintf "@keyframes %s %s" name rule
    log <| sprintf "keyframe: %s" (keyframes |> List.skip (keyframes.Length / 2) |> List.head)
    log($"createRule {name} {durn}ms for {nodeStr node}")

    let stylesheet = getSutilStylesheet (documentOf node)
    stylesheet.insertRule( keyframeText, stylesheet.cssRules.length) |> ignore

    let animations =
        if System.String.IsNullOrEmpty(node.style.animation) then [] else [ node.style.animation ]
        @ [ sprintf "%s %fms linear %fms 1 both" name durn tr.Delay ]

    node.style.animation <- animations |> String.concat ", "
    numActiveAnimations <- numActiveAnimations + 1
    name

let clearAnimations (node:HTMLElement) =
    node.style.animation <-""

let clearRules() =
    window.requestAnimationFrame( fun _ ->
        if (numActiveAnimations = 0) then
            for kv in activeDocs do
                let doc = kv.Value
                let stylesheet = getSutilStylesheet doc
                log <| sprintf "clearing %d rules" (int stylesheet.cssRules.length)
                for i in [(int stylesheet.cssRules.length-1) .. -1 .. 0] do
                    stylesheet.deleteRule( float i )
            //doc.__svelte_rules = {};
        activeDocs <- Map.empty
    ) |> ignore

let deleteRule (node:HTMLElement) (name:string) =
    let previous = (toEmptyStr node.style.animation).Split( ',' )
    let next =
        previous |> Array.filter
            (if System.String.IsNullOrEmpty(name)
                then (fun anim -> anim.IndexOf(name) < 0) // remove specific animation
                else (fun anim -> anim.IndexOf("__Sutil") < 0)) // remove all Svelte animations
    let deleted = previous.Length - next.Length
    if (deleted > 0) then
        //log <| sprintf "Deleted rule(s) %s (%d removed)" name deleted
        node.style.animation <- next |> Array.map (fun s -> s.Trim()) |> String.concat ", "
        numActiveAnimations <- numActiveAnimations - deleted
        if (numActiveAnimations = 0) then clearRules()


let flip (node:Element) (animation:Animation) props =
    let tr = applyProps props  {
            Transition.Default with
                Delay = 0.0
                DurationFn = Some (fun d -> System.Math.Sqrt(d) * 120.0)
                Ease = Easing.cubicOut }
    let style = window.getComputedStyle(node)
    let transform = if style.transform = "none" then "" else style.transform
    let scaleX = animation.From.width / node.clientWidth
    let scaleY = animation.From.height / node.clientHeight
    let dx = (animation.From.left - animation.To.left) / scaleX
    let dy = (animation.From.top - animation.To.top) / scaleY
    let d = System.Math.Sqrt(dx * dx + dy * dy)
    log $"flip: {dx},{dy} {transform} {animation.From} -> {animation.To}"
    {
        tr with
            Duration = match tr.DurationFn with
                        | None -> tr.Duration //
                        | Some f -> f(d) // Use user's function or our default
            DurationFn = None  // Original converts any function into a scalar value
            CssGen = Some (fun t u -> sprintf "transform: %s translate(%fpx, %fpx);`" transform (u * dx) (u * dy))
    }

let createAnimation (node:HTMLElement) (from:ClientRect) (animateFn : Element -> Animation -> TransitionProp list -> Transition) props =
    //if (!from)
    //    return noop;
    let tgt (* to *) = node.getBoundingClientRect()

    //log( sprintf "from=%f,%f to=%f,%f" from.left from.top tgt.left tgt.top)
    let shouldCreate = not (isNull from) && not (from.left = tgt.left && from.right = tgt.right && from.top = tgt.top && from.bottom = tgt.bottom)
    //    return noop;

    //let { delay = 0, duration = 300, easing = identity,
    //        start: start_time = exports.now() + delay,
    //        end = start_time + duration, tick = noop, css } = fn(node, { From = from; To = tgt }, props);

    // TODO : Tick loop

    let a = animateFn node { From = from; To = tgt } props
    let r = { a with Duration = if (a.Duration = 0.0 && a.DurationFn.IsNone) then 300.0 else a.Duration }

    if (shouldCreate)
        then
            //log(sprintf "Creating animation for %s" node.innerText)
            createRule node 0.0 1.0 r 0
        else
            //log(sprintf "No animation for %s" node.innerText)
            ""

let private waitAnimationEnd tag (el : HTMLElement) (f : unit -> unit) =
    let rec cb _ =
        //log($"animationend: {tag}")
        el.removeEventListener("animationend",cb)
        f()
    //log($"waitAnimationEnd: {tag}")
    el.addEventListener("animationend", cb)

let animateNode (node : HTMLElement) from =
    //let from = node.getBoundingClientRect()
    waitAnimationFrame $"animateNode {nodeStr node} {rectStr from} {rectStr <| node.getBoundingClientRect()}" <| fun () ->
        //window.requestAnimationFrame( fun _ ->
        log("animateNode: start")
        let name = createAnimation node from flip []
        log($"Animation is {name}")
        waitAnimationEnd "deleteRule" node <|
            fun _ ->
                log("animateNode: end")
                deleteRule node name

let private tickGen = Helpers.makeIdGenerator()

let private findTransition (intro:bool) (trans : TransitionAttribute list) : TransitionBuilder option =
    let mutable result : TransitionBuilder option = None
    for x in trans do
        result <- match result, x, intro with
                    | Some _, _, _ -> result
                    | None, In x, true -> Some x
                    | None, Out x, false -> Some x
                    | None, InOut x, _ -> Some x
                    | _ -> None
    result

let transitionNode  (el : HTMLElement)
                    (trans : TransitionAttribute list)
                    (initProps : TransitionProp list) // Likely to just be Key, if anything
                    (isVisible : bool)
                    (start: HTMLElement -> unit)
                    (complete: HTMLElement -> unit) =
    let mutable ruleName = ""

    let cancelTick () =
        match NodeKey.get<Unsubscribe> el NodeKey.TickTask with
        | Some f ->
            NodeKey.clear el NodeKey.TickTask
            f()
        | None -> ()

    let runTick tr b durn =
        let log = Logging.log "tick"

        let a = if b = 0.0 then 1.0 else 0.0
        let d = b - a
        let tickId = tickGen()
        let tick = match tr.Tick with|Some f -> f|None -> failwith "No tick function supplied"
        let ease = tr.Ease
        let delay = tr.Delay

        let mutable t = a
        let mutable start = 0.0
        let mutable finish = 0.0
        let mutable started = false
        let mutable finished = false

        Interop.set el NodeKey.TickTask (fun () ->
            log $"#{tickId}: cancel"
            finished <- true
        )

        log $"#{tickId}: run b={b} durn={durn}"
        if (b > 0.0) then
            tick 0.0 1.0

        LoopTasks.loop <| fun now ->
            if not started then
                start <- now + delay
                finish <- start + durn
                log $"#{tickId}: start: start={start} finish={finish}"
                started <- true

            if finished || now >= finish then
                log $"#{tickId}: finish: t={t}"
                t <- b
                tick t (1.0 - t)
                finished <- true

            else if now >= start then
                let e = now - start
                let t0 = e / durn
                t <- a + d * (ease t0)
                log $"#{tickId}: tick: t={t} t0={t0} e={e}"
                tick t (1.0 - t)

            not finished

    let hide() =
        log $"hide {nodeStr el}"
        showEl el false
        complete el
        if ruleName <> "" then deleteRule el ruleName
        dispatchSimple el "outroend"

    let rec show() =
        log $"show {nodeStr el}"
        showEl el true
        complete el
        if ruleName <> "" then deleteRule el ruleName
        dispatchSimple el "introend"

    let tr = findTransition isVisible trans

    let startTransition createTrans =
        let tag   = if isVisible then "show"       else "hide"
        let event = if isVisible then "introstart" else "outrostart"
        let (a,b) = if isVisible then (0.0, 1.0)   else (1.0, 0.0)
        let onEnd = if isVisible then show         else hide

        cancelTick()

        waitAnimationFrame tag <| fun () ->
            dispatchSimple el event
            start el
            waitAnimationEnd tag el onEnd
            if (isVisible) then
                showEl el true // Check: we're doing this again at animationEnd
            let tr = createTrans()
            if tr.DurationFn.IsSome then failwith "Duration function not permitted"
            let d = tr.Duration
            if tr.CssGen.IsSome then
                ruleName <- createRule el a b tr 0
            if tr.Tick.IsSome then
                // Wait for the cancelled runTick to finish
                DOM.wait el (fun () ->
                    let t = runTick tr b d
                    t.Promise)

    match tr with
    | None ->
        showEl el isVisible
        complete el
    | Some init ->
        deleteRule el ""
        let createTrans = (init initProps) el
        startTransition createTrans

type Hideable = {
    predicate : IObservable<bool>
    element   : NodeFactory
    transOpt  : TransitionAttribute list
}

type HideableRuntime = {
    hideable : Hideable
    mutable target : Node
    mutable cache : bool
    mutable unsubscribe : System.IDisposable
}

let createHideableRuntime h =
    {
        hideable = h
        target = null
        cache = false
        unsubscribe = null
    }

let transitionList (list : Hideable list) : NodeFactory = fun ctx ->
    let runtimes = list |> List.map createHideableRuntime
    for rt in runtimes do
        rt.unsubscribe <- Store.subscribe rt.hideable.predicate ( fun show ->
            if (isNull rt.target) then
                rt.target <- buildSolitary rt.hideable.element ctx
                rt.cache <- not show

            if (rt.cache <> show) then
                rt.cache <- show
                transitionNode (rt.target :?> HTMLElement) rt.hideable.transOpt [] show ignore ignore
        )
    unitResult()

type MatchOption<'T> = ('T -> bool) *  NodeFactory * TransitionAttribute list

let makeHideable guard element transOpt = {
    element = element
    transOpt = transOpt
    predicate = guard
}

let transitionMatch<'T> (store : IObservable<'T>) (options : MatchOption<'T> list) =
    options |> List.map (fun (p,e,t) -> makeHideable (store |> Store.map p) e t) |> transitionList

let transitionOpt   (trans : TransitionAttribute list)
                    (store : IObservable<bool>)
                    (element: NodeFactory)
                    (elseElement : NodeFactory option) : NodeFactory = fun ctx ->
    let mutable target : Node = null
    let mutable cache = false
    let mutable targetElse : Node = null

    let unsub = Store.subscribe store (fun isVisible ->
        if isNull target then
            target <- buildSolitary element ctx
            cache <- not isVisible
            match elseElement with
            | Some e -> targetElse <- buildSolitary e ctx
            | None -> ()

        if cache <> isVisible then
            cache <- isVisible
            transitionNode (target :?> HTMLElement) trans [] isVisible ignore ignore
            if not (isNull targetElse) then
                transitionNode (targetElse :?> HTMLElement) trans [] (not isVisible) ignore ignore
    )
    unitResult()

// Show or hide according to a Store<bool> using a transition
let transition<'T> (trans : TransitionAttribute list) store element =
    transitionOpt trans store element None

// Alternate between a pair of elements according to a Store<bool> with no transition
let transitionElse<'T> (trans : TransitionAttribute list) store element otherElement=
    transitionOpt trans store element (Some otherElement)

// Show or hide according to a Store<bool> with no transition
let showIf<'T> store element =
    transitionOpt [] store element None

// Alternate between a pair of elements according to a Store<bool> with no transition
let showIfElse<'T> store element otherElement=
    transitionOpt [] store element (Some otherElement)



