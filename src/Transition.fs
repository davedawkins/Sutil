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
        // ... loads more

    type TransitionProp =
        | Delay of float
        | Duration of float
        | Ease of (float -> float)
        | Css of (float -> float -> string )
        | Tick of (float -> float -> unit)

    type Transition =
        {
            Delay : float
            Duration : float
            Ease : (float -> float)
            Css : (float -> float -> string )
            Tick: (float -> float -> unit)
        }
        with
        static member Default = {
            Delay = 0.0
            Duration = 400.0
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

    let fade (node : Element) (props : TransitionProp list) =
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

    let slide (node : HTMLElement) (props : TransitionProp list) =
        let tr = applyProps props { Transition.Default with Delay = 0.0; Duration = 400.0; Ease = Easing.cubicOut }

        let style = window.getComputedStyle(node)
        console.log(style)
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

