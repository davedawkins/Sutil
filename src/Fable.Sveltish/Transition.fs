module Sveltish.Transition
// Adapted from svelte/transitions/index.js

    open Browser.Dom
    open Browser.CssExtensions
    open Browser.Types
    open Sveltish.Styling
    open Sveltish.DOM
    open System.Collections.Generic
    open System

    let log = Sveltish.Logging.log "trans"

    module Easing =
        // Adapted from svelte/easing/index.js
        let linear = id
        let cubicIn t = t * t * t
        let cubicOut t =
            let f = t - 1.0
            f * f * f + 1.0
        let cubicInOut t =
            if t < 0.5 then 4.0 * t * t * t else 0.5 * System.Math.Pow(2.0 * t - 2.0, 3.0) + 1.0
        // ... loads more

    type TransitionProp =
        | Key of string // Come back to this
        | X of float
        | Y of float
        | Opacity of float
        | Delay of float
        | Duration of float
        | DurationFn of (float -> float) option
        | Ease of (float -> float)
        | Css of (float -> float -> string )
        | Tick of (float -> float -> unit)
        | Speed of float

    // Beginning to think this needs to be a dynamic object in order to be forward compatible
    // with new transitions
    type Transition =
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
            Css : (float -> float -> string )
            Tick: (float -> float -> unit)
        }
        with
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
            Css = fun a b -> ""
            Tick = fun a b -> () }

    type TransitionFn = (TransitionProp list -> HTMLElement -> unit -> Transition)

    type TransitionFactory = TransitionFn * (TransitionProp list)

    type TransitionAttribute =
        | Both of TransitionFactory
        | In of TransitionFactory
        | Out of TransitionFactory
        | InOut of (TransitionFactory * TransitionFactory)


    let private applyProp (r:Transition) (prop : TransitionProp) =
        match prop with
        | Delay d -> { r with Delay = d }
        | Duration d -> { r with Duration = d; DurationFn = None }
        | DurationFn fo -> { r with DurationFn = fo; Duration = 0.0 }
        | Ease f -> { r with Ease = f }
        | Css f -> { r with Css = f }
        | Tick f -> { r with Tick = f }
        | Speed s -> { r with Speed = s }
        | X n -> { r with X = n }
        | Y n -> { r with Y = n }
        | Opacity n -> { r with Opacity = n }
        | Key f -> { r with Key = f }

    let private applyProps (props : TransitionProp list) (tr:Transition) = props |> List.fold applyProp tr

    let private computedStyleOpacity e =
        try
            float (window.getComputedStyle(e).opacity)
        with
        | _ ->
            log(sprintf "parse error: '%A'" (window.getComputedStyle(e).opacity))
            1.0

    let element tag = document.createElement(tag)

    let mutable numActiveAnimations = 0
    let mutable tasks : (unit -> unit) list = []

    let runTasks() =
        let copy = tasks
        tasks <- []
        for f in copy do f()

    let waitAnimationFrame f =
        let init = tasks.IsEmpty
        tasks <- f :: tasks
        if init then
            window.requestAnimationFrame( fun _ ->
                runTasks()
            ) |> ignore

    let getSveltishStyleElement (doc : Document) =
        let mutable e = doc.querySelector("head style#__sveltish_keyframes")
        if (isNull e) then
            e <- element("style")
            e.setAttribute("id", "__sveltish_keyframes")
            doc.head.appendChild(e) |> ignore
        e

    let dotSheet styleElem : CSSStyleSheet = Interop.get styleElem "sheet"

    let getSveltishStylesheet (doc : Document) =
        getSveltishStyleElement doc |> dotSheet

    let nextRuleId = Helpers.makeIdGenerator()

    let toEmptyStr s = if System.String.IsNullOrEmpty(s) then "" else s

    let createRule (node : HTMLElement) (a:float) (b:float) (trfn : unit -> Transition) (uid:int) =
        let tr = trfn()

        let durn =
            match tr.DurationFn with
            | Some f -> f(a)
            | None -> tr.Duration

        let step = 16.666 / durn
        let mutable keyframes = [ "{\n" ];

        for p in [0.0 ..step.. 1.0] do
            let t = a + (b - a) * tr.Ease(p)
            keyframes <- keyframes @ [ sprintf "%f%%{%s}\n" (p * 100.0) (tr.Css t (1.0 - t)) ]

        let rule = keyframes @ [ sprintf "100%% {%s}\n" (tr.Css b (1.0 - b)) ] |> String.concat ""

        let name = sprintf "__sveltish_%d" (if uid = 0 then nextRuleId() else uid)
        let keyframeText = sprintf "@keyframes %s %s" name rule
        //log <| sprintf "keyframe: %s" (keyframes |> List.skip (keyframes.Length / 2) |> List.head)

        let stylesheet = getSveltishStylesheet document
        stylesheet.insertRule( keyframeText, stylesheet.cssRules.length) |> ignore

        let animations =
            if System.String.IsNullOrEmpty(node.style.animation) then [] else [ node.style.animation ]
            @ [ sprintf "%s %fms linear %fms 1 both" name tr.Duration tr.Delay ]

        node.style.animation <- animations |> String.concat ", "
        numActiveAnimations <- numActiveAnimations + 1
        name

    let clearRules() =
        window.requestAnimationFrame( fun _ ->
            if (numActiveAnimations = 0) then
                let doc = document  // Svelte supports multiple active documents
                let stylesheet = getSveltishStylesheet doc
                log <| sprintf "clearing %d rules" (int stylesheet.cssRules.length)
                for i in [(int stylesheet.cssRules.length-1) .. -1 .. 0] do
                    stylesheet.deleteRule( float i )
                //doc.__svelte_rules = {};
            //active_docs.clear();
        ) |> ignore

    let deleteRule (node:HTMLElement) (name:string) =
        let previous = (toEmptyStr node.style.animation).Split( ',' )
        let next =
            previous |> Array.filter
                (if System.String.IsNullOrEmpty(name)
                    then (fun anim -> anim.IndexOf(name) < 0) // remove specific animation
                    else (fun anim -> anim.IndexOf("__sveltish") < 0)) // remove all Svelte animations
        let deleted = previous.Length - next.Length
        if (deleted > 0) then
            //log <| sprintf "Deleted rule(s) %s (%d removed)" name deleted
            node.style.animation <- next |> Array.map (fun s -> s.Trim()) |> String.concat ", "
            numActiveAnimations <- numActiveAnimations - deleted
            if (numActiveAnimations = 0) then clearRules()

    let fade (props : TransitionProp list) (node : Element) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.linear }
        fun () -> { tr with Css = (fun t _ -> sprintf "opacity: %f" (t* computedStyleOpacity node)) }

    let parseFloat (s:string, name) =
        if isNull s
            then 0.0
            else
                let s' = s.Replace("px","")
                let (success, num) = System.Double.TryParse s'
                if (success) then num else 0.0

    let slide (props : TransitionProp list) (node : HTMLElement) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut }

        let style = window.getComputedStyle(node)

        let opacity = parseFloat (style.opacity, "opacity")
        let height = parseFloat (style.height, "height")
        let padding_top = parseFloat(style.paddingTop, "paddingTop")
        let padding_bottom = parseFloat(style.paddingBottom, "paddingBottom")
        let margin_top = parseFloat(style.marginTop, "marginTop")
        let margin_bottom = parseFloat(style.marginBottom, "marginBottom")
        let border_top_width = parseFloat(style.borderTopWidth, "borderTopWidth")
        let border_bottom_width = parseFloat(style.borderBottomWidth, "borderBottomWidth")

        let set (name,value,units) = sprintf "%s: %s%s;" name value units

        fun () -> { tr with Css = (fun t _ ->
                                let result = ([
                                        ("overflow", "hidden", "")
                                        ("opacity",  (min (t * 20.0) 1.0) * opacity  |> string, "")
                                        ("height",  t * height|> string, "px")
                                        ("padding-top",  t * padding_top|> string, "px")
                                        ("padding-bottom",  t * padding_bottom|> string, "px")
                                        ("margin-top",  t * margin_top|> string, "px")
                                        ("margin-bottom",  t * margin_bottom|> string, "px")
                                        ("border-top-width",  t * border_top_width|> string, "px")
                                        ("border-bottom-width",  t * border_bottom_width|> string, "px")
                                    ] |> List.map set |> String.concat "")
                                result )
        }

    //function draw(node, { delay = 0, speed, duration, easing: easing$1 = easing.cubicInOut }) {
    let draw (props : TransitionProp list) (node : SVGPathElement) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 800.0; Ease = Easing.cubicInOut }

        let len = node.getTotalLength()

        // TODO:
        // Use optional duration & speed (original compares to undefined not 0.0)
        // USe optional duration function (if present, it's a function of len)
        let duration =
            match tr.Duration with
            | 0.0 -> if tr.Speed = 0.0 then 800.0 else len / tr.Speed
            | d -> d

        fun () -> { tr with
                        Duration = duration
                        Css = fun t u -> sprintf "stroke-dasharray: %f %f" (t*len) (u*len) }

    let fly (props : TransitionProp list) (node : Element) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut }
        let style = window.getComputedStyle(node)
        let targetOpacity = computedStyleOpacity node
        let transform = if style.transform = "none" then "" else style.transform
        let od = targetOpacity * (1.0 - tr.Opacity)

        fun () -> {
            tr with
                Css = (fun t u ->
                    sprintf "transform: %s translate(%fpx, %fpx); opacity: %f;"
                            transform
                            ((1.0 - t) * tr.X)
                            ((1.0 - t) * tr.Y)
                            (targetOpacity - (od * u)))
        }

    let crossfade commonProps =
        let commonTr = applyProps commonProps Transition.Default

        let toReceive = Dictionary<string,ClientRect>()
        let toSend = Dictionary<string,ClientRect>()

        let crossfadeInner ( from : ClientRect, node : Element, props, intro) =
            let tr = applyProps props { Transition.Default with Ease = Easing.cubicOut; DurationFn = Some (fun d -> System.Math.Sqrt(d) * 30.0) }

            let tgt = node.getBoundingClientRect() // was "to"
            let dx = from.left - tgt.left
            let dy = from.top - tgt.top
            let dw = from.width / tgt.width
            let dh = from.height / tgt.height
            if (intro) then
                log(sprintf "crossfade from %f,%f -> %f,%f" from.left from.top tgt.left tgt.top)
            let d = System.Math.Sqrt(dx * dx + dy * dy)
            let style = window.getComputedStyle(node)
            let transform = if style.transform = "none" then "" else style.transform
            let opacity = computedStyleOpacity node
            let duration = match tr.DurationFn with
                            | Some f -> f(d)
                            | None -> tr.Duration
            {
                tr with
                    DurationFn = None
                    Duration = duration
                    Css = fun t u ->
                        sprintf """
                          opacity: %f;
                          transform-origin: top left;
                          transform: %s translate(%fpx,%fpx) scale(%f, %f);"""
                            (t * opacity)
                            transform
                            (u * dx)
                            (u * dy)
                            (t + (1.0 - t) * dw)
                            (t + (1.0 - t) * dh)
            }

        let transition( items : Dictionary<string,ClientRect>, counterparts : Dictionary<string,ClientRect>, intro : bool) =
            fun (props : TransitionProp list) (node : Element) ->
                let propsRec = applyProps props commonTr
                let key = propsRec.Key
                let r = node.getBoundingClientRect()
                items.[ key ] <- r

                let trfac()  =
                    if (counterparts.ContainsKey(key)) then
                        let rect = counterparts.[key]
                        counterparts.Remove(key) |> ignore
                        crossfadeInner(rect, node, props, intro)
                    else
                        // if the node is disappearing altogether
                        // (i.e. wasn't claimed by the other list)
                        // then we need to supply an outro
                        //items.Remove(key) |> ignore
                        (fade props node)()
                        //fallback && fallback(node, props, intro);
                trfac
        (
            transition(toSend, toReceive, false),
            transition(toReceive, toSend, true)
        )

    type Animation = {
        From: ClientRect
        To: ClientRect
    }

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
        {
            tr with
                Duration = match tr.DurationFn with
                            | None -> tr.Duration //
                            | Some f -> f(d) // Use user's function or our default
                DurationFn = None  // Original converts any function into a scalar value
                Css = fun _t u -> sprintf "transform: %s translate(%fpx, %fpx);`" transform (u * dx) (u * dy)
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
                createRule node 0.0 1.0 (fun () -> r) 0
            else
                //log(sprintf "No animation for %s" node.innerText)
                ""

    let waitAnimationEnd (el : HTMLElement) (f : unit -> unit) =
        let rec cb _ =
            el.removeEventListener("animationend",cb)
            f()
        el.addEventListener("animationend", cb)

    let animateNode (node : HTMLElement) from =
        //let from = node.getBoundingClientRect()
        waitAnimationFrame <| fun () ->
            window.requestAnimationFrame( fun _ ->
                let name = createAnimation node from flip []
                waitAnimationEnd node <| fun _ -> deleteRule node name
                ) |> ignore


    let transitionNode (el : HTMLElement) (trans : TransitionAttribute option) (transProps : TransitionProp list) (isVisible :bool) (complete: HTMLElement -> unit)=
        let mutable ruleName = ""

        let hide() =
            showEl el false
            complete el
            deleteRule el ruleName

        let rec show() =
            showEl el true
            complete el
            deleteRule el ruleName

        let tr = trans |> Option.bind (fun x ->
            match x with
            | Both t -> Some t
            | In t -> if isVisible then Some t else None
            | Out t -> if isVisible then None else Some t
            | InOut (tin,tout) -> if isVisible then Some tin else Some tout
            )

        match tr with
        | None ->
            showEl el isVisible
            complete el
        | Some (tr,trProps) ->
            deleteRule el ""
            if isVisible then
                let trans = (tr (transProps @ trProps) el)
                waitAnimationFrame <| fun () ->
                    waitAnimationEnd el show
                    showEl el true
                    ruleName <- createRule el 0.0 1.0 trans 0
            else
                let trans = (tr transProps el)
                waitAnimationFrame <| fun () ->
                    waitAnimationEnd el hide
                    ruleName <- createRule el 1.0 0.0 trans 0

    type Hideable = {
        predicate : IObservable<bool>
        element   : NodeFactory
        transOpt  : TransitionAttribute option
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

    let transitionList (list : Hideable list) = fun (ctx, parent) ->
        let runtimes = list |> List.map createHideableRuntime
        for rt in runtimes do
            rt.unsubscribe <- Store.subscribe rt.hideable.predicate ( fun show ->
                if (isNull rt.target) then
                    rt.target <- rt.hideable.element(ctx,parent)
                    rt.cache <- not show

                if (rt.cache <> show) then
                    rt.cache <- show
                    transitionNode (rt.target :?> HTMLElement) rt.hideable.transOpt [] show ignore
            )
        runtimes.Head.target

    type MatchOption<'T> = ('T -> bool) *  NodeFactory * TransitionAttribute option

    let makeHideable guard element transOpt = {
        element = element
        transOpt = transOpt
        predicate = guard
    }
    let transitionMatch<'T> (store : IObservable<'T>) (options : MatchOption<'T> list) =
        options |> List.map (fun (p,e,t) -> makeHideable (store |> Store.map p) e t) |> transitionList

    let transitionOpt (trans : TransitionAttribute option) (store : IObservable<bool>) (element: NodeFactory) (elseElement : NodeFactory option): NodeFactory = fun (ctx,parent) ->
        let mutable target : Node = null
        let mutable cache = false

        let mutable targetElse : Node = null

        let unsub = Store.subscribe store (fun isVisible ->
            if isNull target then
                target <- element(ctx,parent)
                cache <- not isVisible
                match elseElement with
                | Some e ->
                    targetElse <- e(ctx,parent)
                    //ctx.AppendChild parent targetElse |> ignore
                | None -> ()

            if cache <> isVisible then
                cache <- isVisible
                transitionNode (target :?> HTMLElement) trans [] isVisible ignore
                if not (isNull targetElse) then
                    transitionNode (targetElse :?> HTMLElement) trans [] (not isVisible) ignore
        )
        // Not sure about this. Something is wrong in the design, since we (might) have created two elements
        // We could create a container div to hold them and return that div.
        target

    // Show or hide according to a Store<bool> using a transition
    let transition<'T> (trans : TransitionAttribute) store element =
        transitionOpt (Some trans) store element None

    // Alternate between a pair of elements according to a Store<bool> with no transition
    let transitionElse<'T> (trans : TransitionAttribute) store element otherElement=
        transitionOpt (Some trans) store element (Some otherElement)

    // Show or hide according to a Store<bool> with no transition
    let show<'T> store element =
        transitionOpt None store element None

    // Alternate between a pair of elements according to a Store<bool> with no transition
    let showElse<'T> store element otherElement=
        transitionOpt None store element (Some otherElement)



