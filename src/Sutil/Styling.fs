module Sutil.Styling

open System
open Browser.Types
open Sutil.DOM
open Browser.Dom

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
    log( sprintf "filter by %s: %A -> %A" name (getStyleAttr el) (getStyleAttr el |> filterStyleAttr name) )
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

let styleListToText (css : list<string * obj>) =
    " {\n" +  String.Join ("\n", css |> Seq.map (fun (nm,v) -> $"    {nm}: {v};")) + " }\n"

let frameToText (f : KeyFrame) =
    sprintf "%d%% %s" f.StartAt (styleListToText f.Style)

let framesToText (frames : KeyFrames) =
    sprintf "@keyframes %s {\n%s\n}\n"
        frames.Name
        (String.Join("\n", frames.Frames |> List.map frameToText))

let private isSutilRule (nm:string,v) = nm.StartsWith("sutil")

let private ruleToText (styleName : string) (rule:StyleRule) =
    //rule.SelectorSpec + (styleListToText rule.Style)
    let styleText = String.Join ("\n", rule.Style |> Seq.filter (not << isSutilRule) |> Seq.map (fun (nm,v) -> $"    {nm}: {v};"))
    [
        specifySelector styleName rule.SelectorSpec
        " {\n"
        styleText
        "}\n"
    ] |> String.concat ""

let rec mediaRuleToText styleName rule =
    sprintf "@media %s {\n%s\n}\n" (rule.Condition) (rule.Rules |> List.map (entryToText styleName) |> String.concat "\n")

and entryToText (styleName : string) = function
    | Rule rule ->
        ruleToText styleName rule
    | KeyFrames frames ->
        framesToText frames
    | MediaRule rule ->
        mediaRuleToText styleName rule

let styleSheetAsText (styleSheet : StyleSheet) =
    String.Join("\n", styleSheet |> List.map (entryToText ""))

let addStyleSheet (doc:Document) styleName (styleSheet : StyleSheet) =
    let style = newStyleElement doc
    for entry in styleSheet do
        entryToText styleName entry |> doc.createTextNode |> style.appendChild |> ignore

let headStylesheet (url : string) : SutilElement = nodeFactory <| fun ctx ->
    let doc = ctx.Document
    let head = findElement doc "head"
    let styleEl = doc.createElement("link")
    head.appendChild( styleEl ) |> ignore
    styleEl.setAttribute( "rel", "stylesheet" )
    styleEl.setAttribute( "href", url ) |> ignore
    unitResult(ctx, "headStylesheet")

let headScript (url : string) : SutilElement = nodeFactory <| fun ctx ->
    let doc = ctx.Document
    let head = findElement doc "head"
    let el = doc.createElement("script")
    head.appendChild( el ) |> ignore
    el.setAttribute( "src", url ) |> ignore
    unitResult(ctx, "headScript")

let headEmbedScript (source : string) : SutilElement = nodeFactory <| fun ctx ->
    let doc = ctx.Document
    let head = findElement doc "head"
    let el = doc.createElement("script")
    head.appendChild( el ) |> ignore
    el.appendChild(doc.createTextNode(source)) |> ignore
    unitResult(ctx, "headEmbedScript")

let headTitle (title : string) : SutilElement = nodeFactory <| fun ctx ->
    let doc = ctx.Document
    let head = findElement doc "head"
    let existingTitle = findElement doc "head>title"

    if not (isNull existingTitle) then
        head.removeChild(existingTitle) |> ignore

    let titleEl = doc.createElement("title")
    titleEl.appendChild( doc.createTextNode(title) ) |> ignore
    head.appendChild(titleEl) |> ignore

    unitResult(ctx, "headTitle")

let withStyle styleSheet (element : SutilElement) : SutilElement = nodeFactory <| fun ctx ->
    let name = ctx.MakeName "sutil"
    addStyleSheet ctx.Document name styleSheet
    ctx |> ContextHelpers.withStyleSheet { Name = name; StyleSheet = styleSheet; Parent = ctx.StyleSheet} |> build element

let withStyleAppend styleSheet (element : SutilElement) : SutilElement = nodeFactory <| fun ctx ->
    let name = match ctx.StyleSheet with | None -> "" | Some s -> s.Name
    addStyleSheet ctx.Document name styleSheet
    ctx |> build element

let rule selector style =
    let result = Rule {
        SelectorSpec = selector
        Selector = parseSelector selector
        Style = style
    }
    //log($"%s{selector} -> %A{result.Selector}")
    result

let keyframe startAt style =
    {
        StartAt = startAt
        Style = style
    }

let keyframes name frames =
    KeyFrames {
        Name = name
        Frames = frames
    }

let showEl (el : HTMLElement) isVisible =
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


open Browser.Css
open Fable.Core.JsInterop
//open Browser.CssExtensions

ConstructStyleSheetsPolyfill.register()

open Fable.Core

type Node with
    /// returns this DocumentOrShadow adopted stylesheets or sets them.
    /// https://wicg.github.io/construct-stylesheets/#using-constructed-stylesheets
    [<Emit("$0.adoptedStyleSheets{{=$1}}")>]
    member __.adoptedStyleSheets with get(): CSSStyleSheet array = jsNative and set(v: CSSStyleSheet array) = jsNative
    [<Emit("$0.getRootNode()")>]
    member __.getRootNode() : Node = jsNative

let adoptStyleSheet (styleSheet : StyleSheet) = nodeFactory <| fun ctx ->
    let run() =
        let sheet = CSSStyleSheet.Create()
        sheet.replaceSync (styleSheetAsText styleSheet)

        let rootNode : Node = ctx.ParentNode.getRootNode()

        rootNode.adoptedStyleSheets <- Array.concat [ rootNode.adoptedStyleSheets; [| sheet |] ]

    if ctx.Parent.IsConnected() then
        run()
    else
        rafu run

    unitResult(ctx,"adoptStyleSheet")
