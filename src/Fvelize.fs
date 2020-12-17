//
// Quick and dirty DOM builder, as a layer over document.createElement. This is just enough of
// an implementation to show the proof of concept. There may be one of these out there somewhere.
// Its main requirement is that builds real DOM, and helps with DOM editing.
//
module Fvelize

open Browser.Dom
open Browser.CssExtensions
open Browser.Types
open System
open Transition

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement selector = document.querySelector(selector)

type SubscriptionCallback = (unit -> unit) -> unit
type SubscriptionGet<'T> = ('T -> unit) -> unit -> unit

type Fn<'T> = (unit -> 'T)



type StyleRule = {
        Selector : string
        Style : (string*obj) list
    }

type StyleSheet = StyleRule list

let rule name style = {
    Selector = name
    Style = style
}

type TransitionFactory = HTMLElement -> (TransitionProp list) -> Transition

type TransitionAttribute =
    | Both of TransitionFactory
    | In of TransitionFactory
    | Out of TransitionFactory
    | InOut of (TransitionFactory * TransitionFactory)

type ElementChild =
    | Event of string * (Event -> unit)
    | Attribute of string * obj
    | Element of string*ElementChild list
    | Text of string
    | Styled of (StyleSheet * ElementChild)
    | BindingAttribute of string * SubscriptionGet<obj> * (obj -> unit)
    | BindingVisibility of ElementChild * SubscriptionGet<bool> * TransitionAttribute option
    | Binding of (unit -> ElementChild) * SubscriptionCallback

//type Attr = string * string
let makeIdGenerator() =
    let mutable id = 0
    fun () ->
        let r = id
        id <- id+1
        r

let childrenOf (e:HTMLElement) =
    let rec toList (n:Node) =
        match n with
        | null -> []
        | x  -> x :: (toList x.nextSibling)
    toList e.firstChild

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

let appendAttribute (e:Element) attrName attrValue =
    if (attrValue <> "") then
        let currentValue = e.getAttribute(attrName)
        e.setAttribute(attrName,
            if ((isNull currentValue) || currentValue = "")
                then attrValue
                else (sprintf "%s %s" currentValue attrValue))

type BuildContext = {
    StyleName : string
    AppendChild : (Element -> Node -> unit)
    MakeName : (string -> string)
}

let makeContext =
    let gen = makeIdGenerator()
    {
        StyleName = ""
        AppendChild = fun parent child -> parent.appendChild(child) |> ignore
        MakeName = fun baseName -> sprintf "%s-%d" baseName (gen())
    }

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
        |> Array.filter (fun (k,v) -> console.log(sprintf "%s=%s %A %s" k v (k <> name) name); k <> name)
        |> emitStyleAttr

let getStyleAttr (el : HTMLElement) =
    match el.getAttribute("style") with
    | null -> ""
    | s -> s

let addStyleAttr (el : HTMLElement) name value =
    let style = getStyleAttr el |> filterStyleAttr name
    el.setAttribute( "style", sprintf "%s%s:%s;" style name value )

let removeStyleAttr (el : HTMLElement) name =
    console.log( sprintf "filter by %s: %A -> %A" name (getStyleAttr el) (getStyleAttr el |> filterStyleAttr name) )
    el.setAttribute( "style", getStyleAttr el |> filterStyleAttr name )

let rec build context (parent : Element) (child : ElementChild) =
    match child with
    | Event (name,f) -> parent.addEventListener( name, f, true)

    | Attribute (name,value) ->
        parent.setAttribute(name,sprintf "%A" value)
        |> ignore


    | Text s ->
        context.AppendChild parent (document.createTextNode(s) :> Node)
        |> ignore

    | Element (tag,children) ->
        let e = document.createElement(tag)

        context.AppendChild parent (e :> Node)

        for c in children do
            build context e c

        if context.StyleName <> "" then
            appendAttribute e "class" context.StyleName

        |> ignore

    | Styled (styleSheet,element) ->
        let name = context.MakeName "sveltish"
        addStyleSheet name styleSheet
        build { context with StyleName = name } parent element

    | Binding (expr,sub) ->
        let mutable (current:Node)  = null

        let appendChild (p:Element) (node:Node) =
            let old = current

            if (not (isNull old)) && old.parentElement.isSameNode(p) then
                p.replaceChild(node,old)
            else
                p.appendChild node
            |> ignore

            if p.isSameNode(parent) then
                current <- node
            |> ignore

        sub( fun () ->
            build { context with AppendChild = appendChild } parent <| expr()
        )

    | BindingVisibility (elem,sub,ta) ->
        let mutable target : Node = null

        let getRef p c =
            if (parent.isSameNode(p)) then
                console.log("Captured reference")
                target <- c
            context.AppendChild p c

        console.log("Subscribing to visibility")

        sub( fun isVisible ->
            if isNull target then
                console.log("Building and waiting for reference")
                build { context with AppendChild = getRef } parent <| elem
            else
                console.log(sprintf "Visibility = %A" isVisible)
                let el = target :?> HTMLElement

                let rec hide = fun _ ->
                    addStyleAttr el "display" "none"
                    //el.hidden <- true
                    removeStyleAttr el "animation"
                    el.removeEventListener( "animationend", hide )

                let rec show = fun _ ->
                    removeStyleAttr el "display"
                    //el.hidden <- false
                    removeStyleAttr el "animation"
                    el.removeEventListener( "animationend", show )

                let isIn = isVisible
                let tr = ta |> Option.bind (fun x ->
                    match x with
                    | Both t -> Some t
                    | In t -> if isIn then Some t else None
                    | Out t -> if isIn then None else Some t
                    | InOut (tin,tout) -> if isIn then Some tin else Some tout
                    )

                match tr with
                | None ->
                    if isIn then
                        removeStyleAttr el "display"
                    else
                        addStyleAttr el "display" "none"
                | Some tr ->
                    if isVisible then
                        el.addEventListener( "animationend", show )
                        removeStyleAttr el "display"
                        Transition.createRule el 0.0 1.0 (tr el []) 314 |> ignore
                        //el.hidden <- false
                    else
                        el.addEventListener( "animationend", hide )
                        Transition.createRule el 1.0 0.0 (tr el []) 314 |> ignore

        ) |> ignore // the unsubscription
        ()
    | BindingAttribute (name,sub,set) ->
        let input = parent :?> HTMLInputElement
        parent.addEventListener("change", (fun e ->
            //console.log(sprintf "Changed %A" input.``checked``)
            set(input.``checked``)) )
        sub( fun value ->
            input.``checked`` <- (string value = "true")
        )
        |> ignore

let el name (children : ElementChild list) =
    Element (name,children)

// Elements
let div children  = el "div" children
let p children = el "p" children
let label children = el "label" children
let h2 children = el "h2" children
let input children = el "input" children
let button children = el "button" children
let span children = el "span" children

// Nodes
let str text = text |> Text

// Attributes
let className name = Attribute ("class",name)

// Event listeners
let on name f = Event(name,f)
let onClick = on "click"
let onKeyDown = on "keydown"

// Styles
let margin (n:obj) = "margin",n
let marginTop (n:obj) = "margin-top",n
let marginLeft (n:obj) = "margin-left",n
let marginBottom (n:obj) = "margin-bottom",n
let marginRight (n:obj) = "margin-right",n
let backgroundColor (n:obj) = "background-color",n
let borderColor (n:obj) = "border-color",n
let borderWidth (n:obj) = "border-width",n
let color (n:obj) = "color",n
let cursor (n:obj) = "cursor",n
let justifyContent (n:obj) = "justify-content",n
let paddingBottom (n:obj) = "padding-bottom",n
let paddingLeft (n:obj) = "padding-left",n
let paddingRight (n:obj) = "padding-right",n
let paddingTop (n:obj) = "padding-top",n
let textAlign (n:obj) = "text-align",n
let whiteSpace (n:obj) = "white-space",n
let alignItems     (n:obj) = "align-items",n
let border         (n:obj) = "border",n
let borderRadius   (n:obj) = "border-radius",n
let boxShadow      (n:obj) = "box-shadow",n
let display        (n:obj) = "display",n
let fontSize       (n:obj) = "font-size",n
let fontFamily     (n:obj) = "font-family",n
let width          (n:obj) = "width",n
let maxWidth       (n:obj) = "max-width",n
let height         (n:obj) = "height",n
let lineHeight     (n:obj) = "line-height",n
let position       (n:obj) = "position",n
let verticalAlign  (n:obj) = "vertical-align",n
let fontWeight     (n:obj) = "font-height",n
let ``float``      (n:obj) = "float",n
let padding        (n:obj) = "padding",n
let boxSizing      (n:obj) = "box-sizing",n
let userSelect     (n:obj) = "user-select",n
let top            (n:obj) = "top",n
let left           (n:obj) = "left",n
let opacity        (n:obj) = "opacity",n
let transition     (n:obj) = "transition",n

let rec mount selector app  =
    let context = makeContext
    let host = idSelector selector |> findElement
    build context host app

let style (styleSheet : StyleSheet) element=
    Styled (styleSheet, element)

