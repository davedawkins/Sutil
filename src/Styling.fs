module Sveltish.Styling

    open System
    open Browser.Types
    open Sveltish.DOM
    open Browser.Dom

    type StyleRule = {
            Selector : string
            Style : (string*obj) list
        }

    type StyleSheet = StyleRule list

    let log s = Logging.log "style" s

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
        let style = newStyleElement
        for rule in styleSheet do
            let styleText = String.Join ("", rule.Style |> Seq.map (fun (nm,v) -> sprintf "%s: %A;" nm v))
            [ specifySelector styleName rule.Selector; " {"; styleText; "}" ] |> String.concat "" |> document.createTextNode |> style.appendChild |> ignore

    let style styleSheet (element : NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let name = ctx.MakeName "sveltish"
        addStyleSheet name styleSheet
        element({ ctx with StyleName = name },parent)

    let rule name style = {
        Selector = name
        Style = style
    }
