namespace Sveltish
open System.Collections.Generic
module Transition =
    open Browser.Dom
    open Browser.CssExtensions
    open Browser.Types
    open Fable.Core.Util
    open Fable.Core

    module Easing =
        // Adapted from svelte/easing/index.mjs
        let linear = id
        let cubicIn t = t * t * t
        let cubicOut t =
            let f = t - 1.0
            f * f * f + 1.0

        let cubicInOut t =
            if t < 0.5 then 4.0 * t * t * t else 0.5 * System.Math.Pow(2.0 * t - 2.0, 3.0) + 1.0
        // ... loads more

    type TransitionProp =
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

    let private applyProps (props : TransitionProp list) (tr:Transition) = props |> List.fold applyProp tr

    let private computedStyleOpacity e = float (window.getComputedStyle(e).opacity)

    let element tag = document.createElement(tag)

    [<Emit("$0.sheet")>]
    let dotSheet styleElem : CSSStyleSheet = jsNative

    // You know, this is a port from svelte/index.js. I could just import it :-/
    // That's still an option for doing a complete implementation.

    //let mutable rules : string list = []

    let ruleIndexes = Dictionary<string,float>()

    let getSveltishStylesheet =
        let mutable e = document.querySelector("head style")
        if (isNull e) then
            e <- element("style")
            document.head.appendChild(e) |> ignore
        e |> dotSheet

    let deleteRule ruleName =
        let s = getSveltishStylesheet
        s.deleteRule( ruleIndexes.[ruleName] )

    let createRule (node : HTMLElement) (a:float) (b:float) (tr : Transition) (uid:int) =
        let step = 16.666 / tr.Duration
        let mutable keyframes = [ "{\n" ];

        for p in [0.0 ..step.. 1.0] do
            let t = a + (b - a) * tr.Ease(p)
            keyframes <- keyframes @ [ sprintf "%f%%{%s}\n" (p * 100.0) (tr.Css t (1.0 - t)) ]

        let rule = keyframes @ [ sprintf "100%% {%s}\n" (tr.Css b (1.0 - b)) ] |> String.concat ""

        let name = sprintf "__sveltish_%d" uid

        let stylesheet = getSveltishStylesheet
        let ruleIndex = stylesheet.insertRule( (sprintf "@keyframes %s %s" name rule), stylesheet.cssRules.length)

        node.style.animation <- sprintf "%s %fms linear %fms 1 both" name tr.Duration tr.Delay
        ruleIndexes.[name] <- ruleIndex
        name

    let fade (props : TransitionProp list) (node : Element) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.linear }
        { tr with Css = (fun t _ -> sprintf "opacity: %f" (t* computedStyleOpacity node)) }

    let parseFloat (s:string, name) =
        if isNull s
            then 0.0
            else
                let s' = s.Replace("px","")
                console.log(sprintf "%s=%s" name s')
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

        { tr with Css = (fun t _ ->
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

        { tr with
            Duration = duration
            Css = fun t u -> sprintf "stroke-dasharray: %f %f" (t*len) (u*len)

        }

    let fly (props : TransitionProp list) (node : Element) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut }
        let style = window.getComputedStyle(node)
        let targetOpacity = computedStyleOpacity node
        let transform = if style.transform = "none" then "" else style.transform
        let od = targetOpacity * (1.0 - tr.Opacity)

        {
            tr with
                Css = (fun t u ->
                    sprintf "transform: %s translate(%fpx, %fpx); opacity: %f;"
                            transform
                            ((1.0 - t) * tr.X)
                            ((1.0 - t) * tr.Y)
                            (targetOpacity - (od * u)))
        }
