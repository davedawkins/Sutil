//
// Quick and dirty DOM builder, as a layer over document.createElement. This is just enough of
// an implementation to show the proof of concept. There may be one of these out there somewhere.
// Its main requirement is that builds real DOM, and helps with DOM editing.
//
module Fvelize

open Browser.Dom
open Browser.Types

let idSelector = sprintf "#%s"
let classSelector = sprintf ".%s"
let findElement selector = document.querySelector(selector)

type Subscription = (unit -> unit) -> unit
type Fn<'T> = (unit -> 'T)

type ElementChild =
    | Event of string * (Event -> unit)
    | Attribute of string * obj
    | ChildElement of HTMLElement
    | ChildNode of Node
    | BoundNode of (Fn<ElementChild>) * Subscription
    | Styled of styles : ((string*obj) list) * children: ElementChild list

type Attr = string * string

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

let mutable nextStyleId = 0

let makeStyleSheet (styles : (string*obj) list) =
    let style = newStyleElement
    let styleId = sprintf "style-%d" nextStyleId
    nextStyleId <- nextStyleId + 1
    let styleText = System.String.Join ("", styles |> Seq.map (fun (nm,v) -> sprintf "\n\t%s: %A;" nm v))
    [ "."; styleId; ", ."; styleId; " * {\n"; styleText; "\n}\n" ] |> String.concat "" |> document.createTextNode |> style.appendChild |> ignore
    styleId

let appendAttribute (e:HTMLElement) attrName attrValue =
    let currentValue = e.getAttribute(attrName)
    e.setAttribute(attrName, if ((isNull currentValue) || currentValue = "") then attrValue else (sprintf "%s %s" currentValue attrValue))

let rec topElements (e : ElementChild) =
    seq {
        match e with
            | ChildElement ce -> yield ce
            | Styled (_, styled) ->
                for child in styled do
                    yield! topElements child
            | _ -> ()
    }

let rec applyStyle styles styledNodes =
    let styleId = makeStyleSheet styles
    for n in styledNodes do
        for root in topElements n do
            appendAttribute root "class" styleId

let el name (children : ElementChild list) =
    let rec build (parent : HTMLElement) child =
        match child with
        | Event (name,f) -> parent.addEventListener(name, f, true)
        | Attribute (name,value) ->
            parent.setAttribute(name,sprintf "%A" value)
            |> ignore

        | ChildNode n -> parent.appendChild n |> ignore
        | ChildElement e -> parent.appendChild e |> ignore

        | Styled (styles, styledNodes) ->
            applyStyle styles styledNodes
            for n in styledNodes do
                build parent n

        | BoundNode (n,sub) ->
            let mutable (current:Node)  = null
            sub( fun () ->
                match n() with
                | ChildNode node ->
                    let old = current
                    current <- node
                    if isNull old then
                        parent.appendChild current
                    else
                        parent.replaceChild(current,old)
                    |> ignore
                | _ -> invalidArg "Expected a node" |> ignore
                ()
            )
    let e = document.createElement name
    for child in children do build e child
    e |> ChildElement


let div children  = el "div" children
let p children = el "p" children
let button children = el "button" children
let str text = (text |> document.createTextNode) :> Node |> ChildNode
let className name = Attribute ("class",name)

let rec mount selector app  =
    match app with
    | Styled (styles, styledNodes) ->
            applyStyle styles styledNodes
            for node in styledNodes do
                mount selector node
    | ChildElement appNode ->  (idSelector selector |> findElement).appendChild(appNode) |> ignore
    | _ -> invalidArg "Expected a node" |> ignore

let style (styles:(string*obj) list) (children:ElementChild list) =
    Styled (styles, children)
let onClick f = Event ("click",f)

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
