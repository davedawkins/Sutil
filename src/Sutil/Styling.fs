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
    s.Split([| delim |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map f
        |> fun values -> String.Join(string delim, values)

let isPseudo s =
    s = "hover" || s = "active" || s = "visited" || s = "link" || s = "before" || s = "after" || s = "checked"

let isGlobal s = s = "body" || s = "html"

let specifySelector (styleName : string) (selectors : string) =
    let trans s = if isPseudo s || isGlobal s then s else sprintf "%s.%s" s styleName  // button -> button.styleA
    splitMapJoin ',' (splitMapJoin ' ' (splitMapJoin ':' trans)) selectors

let addStyleSheet (doc:Document) styleName (styleSheet : StyleSheet) =
    let isSutilRule (nm:string,v) = nm.StartsWith("sutil")
    let style = newStyleElement doc
    for rule in styleSheet do
        let styleText = String.Join ("", rule.Style |> Seq.filter (not << isSutilRule) |> Seq.map (fun (nm,v) -> $"{nm}: {v};"))
        [ specifySelector styleName rule.SelectorSpec; " {"; styleText; "}" ] |> String.concat "" |> doc.createTextNode |> style.appendChild |> ignore

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
    let result = {
        SelectorSpec = selector
        Selector = parseSelector selector
        Style = style
    }
    log($"%s{selector} -> %A{result.Selector}")
    result

let showEl (el : HTMLElement) isVisible =
    if isVisible then
        removeStyleAttr el "display"
    else
        addStyleAttr el "display" "none"
    let ev = Interop.customEvent (if isVisible then Event.Show else Event.Hide) {|  |}
    el.dispatchEvent(ev) |> ignore
    ()