/// <summary>
/// Transitions that can be used in the <c>transition</c> function
/// </summary>
[<AutoOpenAttribute>]
module Sutil.TransitionFunctions
// Adapted from svelte/transitions/index.js

open Browser.Types
open Sutil.Transition
open Sutil.DomHelpers
open System.Collections.Generic
open Interop

let private logEnabled() = Logging.isEnabled "trfn"
let private log = Logging.log "trfn"

let parseFloat (s:string, name) =
    if isNull s
        then 0.0
        else
            let s' = s.Replace("px","")
            let (success, num) = System.Double.TryParse s'
            if (success) then num else 0.0

let fade (initProps : TransitionProp list) (node : HTMLElement) =
        fun () ->
        let tr = applyProps initProps { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.linear }
        {
            tr with CssGen = Some (fun t _ -> sprintf "opacity: %f" (t* computedStyleOpacity node))
        }

let slide (props : TransitionProp list) = fun (node : HTMLElement) ->
    let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut }

    let style = Window.getComputedStyle(node)

    let opacity = parseFloat (style.opacity, "opacity")
    let height = parseFloat (style.height, "height")
    let padding_top = parseFloat(style.paddingTop, "paddingTop")
    let padding_bottom = parseFloat(style.paddingBottom, "paddingBottom")
    let margin_top = parseFloat(style.marginTop, "marginTop")
    let margin_bottom = parseFloat(style.marginBottom, "marginBottom")
    let border_top_width = parseFloat(style.borderTopWidth, "borderTopWidth")
    let border_bottom_width = parseFloat(style.borderBottomWidth, "borderBottomWidth")

    let set (name,value,units) = sprintf "%s: %s%s;" name value units

    fun () -> { tr with CssGen = Some(fun t _ ->
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

let draw (props : TransitionProp list) = fun (node : SVGPathElement) ->
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
                    CssGen = Some <| fun t u -> sprintf "stroke-dasharray: %f %f" (t*len) (u*len) }

let fly (props : TransitionProp list) (node : Element) =
    let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut; X = 0.0; Y = 0.0 }

    let style = Window.getComputedStyle(node)
    let targetOpacity = computedStyleOpacity node
    let transform = if style.transform = "none" then "" else style.transform
    let od = targetOpacity * (1.0 - tr.Opacity)

    fun () -> {
        // Called when animation ready to run
        tr with
            CssGen = Some(fun t u ->
                sprintf "transform: %s translate(%fpx, %fpx); opacity: %f;"
                        transform
                        ((1.0 - t) * tr.X)
                        ((1.0 - t) * tr.Y)
                        (targetOpacity - (od * u)))
    }

let crossfade (userProps : TransitionProp list) =
    let fallback = (applyProps userProps Transition.Default).Fallback

    let toReceive = Dictionary<string,ClientRect>()
    let toSend = Dictionary<string,ClientRect>()

    let dump() =
        let ks (d:Dictionary<string,ClientRect>) = System.String.Join(", ", d.Keys)
        if logEnabled() then log($"toReceive = {ks toReceive}")
        if logEnabled() then log($"toSend    = {ks toSend}")

    let crossfadeInner ( from : ClientRect, node : Element, props, intro) =
        let tr =
                { Transition.Default with Ease = Easing.cubicOut; DurationFn = Some (fun d -> System.Math.Sqrt(d) * 30.0) }
                |> applyProps props
                |> applyProps userProps
        //log($"crossfade props: {tr} from {props}")
        let tgt = node.getBoundingClientRect() // was "to"
        let dx = from.left - tgt.left
        let dy = from.top - tgt.top
        let dw = from.width / tgt.width
        let dh = from.height / tgt.height
        //if (intro) then
        if logEnabled() then log(sprintf "crossfade from %f,%f -> %f,%f" from.left from.top tgt.left tgt.top)
        let d = System.Math.Sqrt(dx * dx + dy * dy)
        let style = Window.getComputedStyle(node)
        let transform = if style.transform = "none" then "" else style.transform
        let opacity = computedStyleOpacity node
        let duration = match tr.DurationFn with
                        | Some f -> f(d)
                        | None -> tr.Duration
        {
            tr with
                DurationFn = None
                Duration = duration
                CssGen = Some <| fun t u ->
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

    // Called in DOM construction
    let transition( items : Dictionary<string,ClientRect>, counterparts : Dictionary<string,ClientRect>, intro : bool) =
        // Called during change propagation (not all changes applied yet)
        fun (props : TransitionProp list) (node : HTMLElement) ->
            // At this stage, I think props will only contain a Key

            //log $"crossfade.transition props {props}"
            let initProps = applyProps props Transition.Default // Just to retrieve key
            let key = initProps.Key

            let r = node.getBoundingClientRect()
            let action = if intro then "receiving" else "sending"
            if logEnabled() then log($"{action} {key} (adding)")
            items.[ key ] <- r

            // Called when animation ready to run (on RAF)
            let trfac ()  =
                let finalProps = props

                if (counterparts.ContainsKey(key)) then
                    let rect = counterparts.[key]
                    if logEnabled() then log($"{action} {key} (removing from counterparts)")

                    counterparts.Remove(key) |> ignore
                    crossfadeInner(rect, node, finalProps, intro)
                else
                    // if the node is disappearing altogether
                    // (i.e. wasn't claimed by the other list)
                    // then we need to supply an outro
                    items.Remove(key) |> ignore
                    if logEnabled() then log($"{action} falling back for {key}")
                    match fallback with
                    | Some f -> (f finalProps node)()
                    | None -> (fade finalProps node)()
                    //(fade finalProps node)()
                    //fallback && fallback(node, props, intro);
            trfac
    (
        transition(toSend, toReceive, false),
        transition(toReceive, toSend, true)
    )
