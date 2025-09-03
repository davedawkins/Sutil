/// <summary>
/// Support CSS styling
/// </summary>
module Sutil.Styling

open System
open Browser.Types
open Sutil.Core
open Sutil.DomHelpers
open Browser.Dom

let private logEnabled() = Logging.isEnabled "style"
let private log s = Logging.log "style" s
let private findElement (doc : Document) selector = doc.querySelector(selector)

let parseStyleAttr (style : string) =
    style.Split([|';'|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.collect (fun entry ->
                        entry.Split([|':'|],2)
                        |> Array.chunkBySize 2
                        |> Array.map (fun pair -> pair.[0].Trim(), pair.[1].Trim()))

let emitStyleAttr (keyValues : (string * string) array) =
    keyValues
        |> Array.map (fun (k,v) -> sprintf "%s:%s;" k v )
        |> String.concat ""

let filterStyleAttr name style =
    parseStyleAttr style
        |> Array.filter (fun (k,v) -> k <> name)
        |> emitStyleAttr

let getStyleAttr (el : HTMLElement) =
    match el.getAttribute("style") with
    | null -> ""
    | s -> s

let addStyleAttr (el : HTMLElement) name value =
    let style = getStyleAttr el |> filterStyleAttr name
    el.setAttribute( "style", sprintf "%s%s:%s;" style name value )

let removeStyleAttr (el : HTMLElement) name =
    if logEnabled() then log( sprintf "filter by %s: %A -> %A" name (getStyleAttr el) (getStyleAttr el |> filterStyleAttr name) )
    el.setAttribute( "style", getStyleAttr el |> filterStyleAttr name )

let newStyleElement (doc : Document)=
    let head = "head" |> findElement doc
    let style = doc.createElement("style")
    head.appendChild(style :> Node) |> ignore
    style

let splitMapJoin (delim:char) (f : string -> string) (s:string) =
    s.Split([| delim |], StringSplitOptions.RemoveEmptyEntries )
        |> Array.map f
        |> fun values -> String.Join(string delim, values)

let mapPseudo (f : string -> string) (s : string) =
    let i = s.IndexOf(':')
    if i < 0 then
        f s
    else
        f (s.Substring(0,i)) + (s.Substring(i))

let isPseudo s =
    s = "hover" || s = "active" || s = "visited" || s = "link" || s = "before" || s = "after" || s = "checked" || s = "marker"

let isGlobal s = s = "body" || s = "html"

let specifySelector (styleName : string) (selectors : string) =
    if (styleName = "") then
        selectors
    else
        let trans s = if isPseudo s || isGlobal s then s else sprintf "%s.%s" s styleName  // button -> button.styleA
        splitMapJoin ',' (splitMapJoin ' ' (mapPseudo trans)) selectors

let private styleListToText (css : list<string * obj>) =
    " {\n" +  String.Join ("\n", css |> Seq.map (fun (nm,v) -> $"    {nm}: {v};")) + " }\n"

let private frameToText (f : KeyFrame) =
    sprintf "%d%% %s" f.StartAt (styleListToText f.Style)

let private framesToText (frames : KeyFrames) =
    sprintf "@keyframes %s {\n%s\n}\n"
        frames.Name
        (String.Join("\n", frames.Frames |> List.map frameToText))

let private isSutilRule (nm:string,v) = nm.StartsWith("sutil")

let private ruleToText (classMap : Map<string,StyleRule>) (styleName : string) (rule:StyleRule) =
    //rule.SelectorSpec + (styleListToText rule.Style)

    let rec styleText (r : StyleRule) = 
        r.Style 
        |> Seq.filter (not << isSutilRule) 
        |> Seq.map (fun (nm,v) -> 
            if (nm.EndsWith("()")) then
                match classMap.TryFind nm[0..-3] with
                | Some subrule ->  
                    styleText subrule
                | _ -> 
                    Fable.Core.JS.console.warn("No class found for substitution: ", nm[0..-3])
                    ""
            else
                $"    {nm}: {v};")
        |> String.concat "\n"

    [
        specifySelector styleName rule.SelectorSpec
        " {\n"
        styleText rule
        "}\n"
    ] |> String.concat ""

let rec mediaRuleToText classMap styleName rule =
    sprintf "@media %s {\n%s\n}\n" (rule.Condition) (rule.Rules |> List.map (entryToText classMap styleName) |> String.concat "\n")

and entryToText classMap (styleName : string) = function
    | Rule rule ->
        ruleToText classMap styleName rule
    | KeyFrames frames ->
        framesToText frames
    | MediaRule rule ->
        mediaRuleToText classMap styleName rule

let private isClassChar c = Char.IsLetterOrDigit(c) || c = '-' || c = '_'

let private isClassName (s : string) =
    s.ToCharArray() |> Array.forall isClassChar

let private isClassOnly (s : string) = 
    s.Length >= 2 && s[0] = '.' && isClassName (s.Substring(1))


let getClassMap (styleSheet) =
    styleSheet 
    |> List.choose (fun d -> match d with Rule r when isClassOnly r.SelectorSpec -> Some (r.SelectorSpec.Substring(1),r) | _ -> None)
    //|> (fun items -> Fable.Core.JS.console.log("Class map: ", items |> List.map fst |> String.concat ","); items)
    |> Map


let includeRule (name : string) = (name + "()"), ("" :> obj)

let private styleSheetAsText (styleSheet : StyleSheetDefinitions) =
    let classMap : Map<string,StyleRule > = getClassMap styleSheet

    System.String.Join("\n", styleSheet |> List.map (entryToText classMap ""))

let private addStyleSheetText (doc:Document) styleName css =
    let style = newStyleElement doc
    css |> doc.createTextNode |> style.appendChild |> ignore
    (fun () -> style.parentElement.removeChild(style) |> ignore)


let private addStyleSheet (doc:Document) styleName (styleSheet : StyleSheetDefinitions) =
    let style = newStyleElement doc
    for entry in styleSheet do
        entryToText (getClassMap styleSheet) styleName entry |> doc.createTextNode |> style.appendChild |> ignore
    (fun () -> style.parentElement.removeChild(style) |> ignore)

let addGlobalStyleSheet (doc:Document) (styleSheet : StyleSheetDefinitions) =
    addStyleSheet doc "" styleSheet

let addGlobalCss (doc:Document) (name : string) (css : string) =
    addStyleSheetText doc name css

/// <summary>
/// Define a CSS styling rule
/// </summary>
let rule selector style =
    let result = Rule {
        SelectorSpec = selector
        Style = style
    }

    result

/// <summary>
/// Define a CSS keyframe as part of a keyframes sequence
/// See also: <seealso cref="M:Sutil.Styling.keyframes"/>
/// </summary>
let keyframe startAt style =
    {
        StartAt = startAt
        Style = style
    }

/// <summary>
/// Define a CSS keyframes sequence
/// </summary>
/// <example>
/// <code>
///    keyframes "dashdraw" [
///         keyframe 0 [
///             Css.custom("stroke-dashoffset", "10")
///         ]
///     ]
/// </code>
/// </example>
let keyframes name frames =
    KeyFrames {
        Name = name
        Frames = frames
    }

let internal showEl (el : HTMLElement) isVisible =
    if isVisible then
        if Interop.exists el "_display" then
            addStyleAttr el "display" (Interop.get el "_display")
        else
            removeStyleAttr el "display"
    else
        addStyleAttr el "display" "none"
    let ev = Interop.customEvent (if isVisible then Event.Show else Event.Hide) {|  |}
    el.dispatchEvent(ev) |> ignore
    ()

let internal makeMediaRule condition rules =
    MediaRule { Condition = condition; Rules = rules }

open Browser.Css

#if !FABLE_REPL_LIB
ConstructStyleSheetsPolyfill.register()
#endif

open Fable.Core


type internal Node with
    /// returns this DocumentOrShadow adopted stylesheets or sets them.
    /// https://wicg.github.io/construct-stylesheets/#using-constructed-stylesheets
    [<Emit("$0.adoptedStyleSheets{{=$1}}")>]
    member __.adoptedStyleSheets with get(): CSSStyleSheet array = jsNative and set(v: CSSStyleSheet array) = jsNative
    [<Emit("$0.getRootNode()")>]
    member __.getRootNode() : Node = jsNative

let adoptStyleSheet (styleSheet : StyleSheetDefinitions) =
    SutilElement.Define( "adoptStyleSheet",
    fun ctx ->
    let run() =
        let sheet = CSSStyleSheet.Create()
        sheet.replaceSync (styleSheetAsText styleSheet)

        let rootNode : Node = ctx.ParentNode.getRootNode()

        rootNode.adoptedStyleSheets <- Array.concat [ rootNode.adoptedStyleSheets; [| sheet |] ]

    if ctx.Parent.IsConnected() then
        run()
    else
        rafu run
    () )

let private ruleMatchEl (el: HTMLElement) (rule: StyleRule) =
    el.matches(rule.SelectorSpec)

let private rulesOf (styleSheet: StyleSheetDefinitions) =
    styleSheet
    |> List.map (function
        | Rule r -> Some r
        | _ -> None)
    |> List.choose id

open Browser.Types
open Browser.Css
open Browser.CssExtensions

let private applyCustomRulesToElement (rules : StyleRule list) (e: HTMLElement) =
    for rule in rules |> List.filter (ruleMatchEl e) do
        for custom in rule.Style do
            match custom with
            | (nm, v) when nm = "sutil-use-global" ->
                failwith "sutil-use-global not supported"
            | (nm, v) when nm = "sutil-use-parent" -> ()
            | (nm, v) when nm = "sutil-add-class" ->
                ClassHelpers.addToClasslist (string v) e
            | (nm,v) ->
                e.style.setProperty( nm, string v )


let private applyCustomRules (rules : StyleSheetDefinitions) (ctx: BuildContext, result : SutilEffect) =
    match result with
    | DomNode n ->
        n |> applyIfElement (rulesOf rules |> applyCustomRulesToElement)
    | _ -> ()
    (ctx, result)

/// <summary>
/// Support for the custom rules "sutil-add-class". They're clever but also difficult to understand. See their usage in Sutil.Bulma
/// </summary>
let withCustomRules (rules : StyleSheetDefinitions) (element : SutilElement) =
    SutilElement.Define("withCustomRules",
    fun ctx ->
    ctx
    |> ContextHelpers.withPostProcess (applyCustomRules rules)
    |> build element )

let private applyStyleSheet (namedSheet : NamedStyleSheet) (ctx: BuildContext, result : SutilEffect)=
    match result with
    | DomNode _ ->
        result.AsDomNode
        |> applyIfElement
            (fun el ->
                if not (Interop.exists el NodeKey.StyleClass) then
                    Interop.set el (NodeKey.StyleClass) namedSheet.Name
                    ClassHelpers.addToClasslist namedSheet.Name el)
    | _ -> ()
    (ctx, result)

let withStyle styleSheet (element : SutilElement) : SutilElement =
    SutilElement.Define("withStyle",
    fun ctx ->
    let name = ctx.MakeName "sutil"
    let namedSheet = { Name = name; StyleSheet = styleSheet }
    addStyleSheet ctx.Document name styleSheet |> ignore
    ctx
    |> ContextHelpers.withPreProcess (applyStyleSheet namedSheet)
    |> build element )
