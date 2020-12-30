module Sveltish.Styling

    open System
    open Browser.Types
    open Sveltish.DOM
    open Browser.Dom

    let log s = Logging.log "style" s
    let findElement selector = document.querySelector(selector)

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

    let newStyleElement =
        let head = "head" |> findElement
        let style = document.createElement("style")
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

    let addStyleSheet styleName (styleSheet : StyleSheet) =
        let isSveltishRule (nm:string,v) = nm.StartsWith("sveltish")
        let style = newStyleElement
        for rule in styleSheet do
            let styleText = String.Join ("", rule.Style |> Seq.filter (not << isSveltishRule) |> Seq.map (fun (nm,v) -> $"{nm}: {v};"))
            [ specifySelector styleName rule.SelectorSpec; " {"; styleText; "}" ] |> String.concat "" |> document.createTextNode |> style.appendChild |> ignore

    let headStylesheet (url : string) = fun (ctx,parent) ->
        let head = findElement "head"
        let styleEl = document.createElement("link")
        head.appendChild( styleEl ) |> ignore
        styleEl.setAttribute( "rel", "stylesheet" )
        styleEl.setAttribute( "href", url ) |> ignore
        parent

    let headScript (url : string) = fun (ctx,parent) ->
        let head = findElement "head"
        let el = document.createElement("script")
        head.appendChild( el ) |> ignore
        el.setAttribute( "src", url ) |> ignore
        parent

    let headEmbedScript (source : string) = fun (ctx,parent) ->
        let head = findElement "head"
        let el = document.createElement("script")
        head.appendChild( el ) |> ignore
        el.appendChild(document.createTextNode(source)) |> ignore
        parent

    let headTitle (title : string) = fun (ctx,parent) ->
        let head = findElement "head"
        let existingTitle = findElement "head>title"

        if not (isNull existingTitle) then
            head.removeChild(existingTitle) |> ignore

        let titleEl = document.createElement("title")
        titleEl.appendChild( document.createTextNode(title) ) |> ignore
        head.appendChild(titleEl) |> ignore

        parent

    let withStyle styleSheet (element : NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let name = ctx.MakeName "sveltish"
        addStyleSheet name styleSheet
        element({ ctx with StyleSheet = Some { Name = name; StyleSheet = styleSheet; Parent = ctx.StyleSheet } },parent)

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