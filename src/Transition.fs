module Sveltish.Transition
// Adapted from svelte/transitions/index.js

    open Browser.Dom
    open Browser.CssExtensions
    open Browser.Types
    open Fable.Core.Util
    open Fable.Core
    open System.Collections.Generic

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
            Speed = 0.0
            Ease = Easing.linear
            Css = fun a b -> ""
            Tick = fun a b -> () }

    let private applyProp (r:Transition) (prop : TransitionProp) =
        match prop with
        | Delay d -> { r with Delay = d }
        | Duration d -> { r with Duration = d }
        | Ease f -> { r with Ease = f }
        | Css f -> { r with Css = f }
        | Tick f -> { r with Tick = f }
        | Speed s -> { r with Speed = s }
        | X n -> { r with X = n }
        | Y n -> { r with Y = n }
        | Opacity n -> { r with Opacity = n }
        | Key f -> { r with Key = f }

    let private applyProps (props : TransitionProp list) (tr:Transition) = props |> List.fold applyProp tr

    let private computedStyleOpacity e = float (window.getComputedStyle(e).opacity)

    let element tag = document.createElement(tag)

    [<Emit("$0.sheet")>]
    let dotSheet styleElem : CSSStyleSheet = jsNative

    // You know, this is a port from svelte/index.js. I could just import it :-/
    // That's still an option for doing a complete implementation.

    //let mutable rules : string list = []

    let ruleIndexes = Dictionary<string,float>()

    let getSveltishStyleElement() =
        let mutable e = document.querySelector("head style")
        if (isNull e) then
            log "creating style sheet for transitions"
            e <- element("style")
            document.head.appendChild(e) |> ignore
        e

    let getSveltishStylesheet() =
        getSveltishStyleElement() |> dotSheet

    let deleteRule ruleName =
        //let s = getSveltishStylesheet
        //s.deleteRule( ruleIndexes.[ruleName] )
        ()

    let createRule (node : HTMLElement) (a:float) (b:float) (trfn : unit -> Transition) (uid:int) =
        log "Creating rule"
        let tr = trfn()
        let step = 16.666 / tr.Duration
        let mutable keyframes = [ "{\n" ];

        for p in [0.0 ..step.. 1.0] do
            let t = a + (b - a) * tr.Ease(p)
            keyframes <- keyframes @ [ sprintf "%f%%{%s}\n" (p * 100.0) (tr.Css t (1.0 - t)) ]

        let rule = keyframes @ [ sprintf "100%% {%s}\n" (tr.Css b (1.0 - b)) ] |> String.concat ""

        let name = sprintf "__sveltish_%d" uid
        let keyframeText = sprintf "@keyframes %s %s" name rule

        let stylesheet = getSveltishStylesheet()
        let ruleIndex = stylesheet.insertRule( keyframeText, stylesheet.cssRules.length)

        node.style.animation <- sprintf "%s %fms linear %fms 1 both" name tr.Duration tr.Delay
        ruleIndexes.[name] <- ruleIndex
        name

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

        let crossfadeInner ( from : ClientRect, node : Element, props) =
            let tr = applyProps props { Transition.Default with Ease = Easing.cubicOut; Duration = 2000.0 }
            //let { delay = 0, duration = d -> Math.sqrt(d) * 30, easing: easing$1 = easing.cubicOut } = internal.assign(internal.assign({}, defaults), params)
            let tgt = node.getBoundingClientRect() // was "to"
            let dx = from.left - tgt.left
            let dy = from.top - tgt.top
            let dw = from.width / tgt.width
            let dh = from.height / tgt.height
            let d = System.Math.Sqrt(dx * dx + dy * dy)
            let style = window.getComputedStyle(node)
            let transform = if style.transform = "none" then "" else style.transform
            let opacity = computedStyleOpacity node
            {
                tr with
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
                let side = if intro then "right" else "left"
                let r = node.getBoundingClientRect()
                items.[ key ] <- r
                log(sprintf "%s: received %A" side key)
                let dump() =
                    log (sprintf "%s: items: %s" side (System.String.Join(", ", items.Keys)) )
                    log (sprintf "%s: cpart: %s" side (System.String.Join(", ", counterparts.Keys)) )
                let trfac()  =
                    if (counterparts.ContainsKey(key)) then
                        let rect = counterparts.[key]
                        counterparts.Remove(key) |> ignore
                        log(sprintf "%s: crossfading %A %f,%f %s" side key rect.left rect.top node.innerHTML)
                        dump()
                        crossfadeInner(rect, node, props)
                    else
                        // if the node is disappearing altogether
                        // (i.e. wasn't claimed by the other list)
                        // then we need to supply an outro
                        log(sprintf "%s: falling back for %A" side key)
                        //items.Remove(key) |> ignore
                        dump()
                        (fade props node)()
                        //fallback && fallback(node, props, intro);
                trfac
        (
            transition(toSend, toReceive, false),
            transition(toReceive, toSend, true)
        )
