[<AutoOpen>]
module Sutil.Html

open Sutil.DOM
open Feliz
open System

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of SutilElement

type SutilHtmlEngine() =
    inherit HtmlEngine<SutilElement>( el, text, (fun () -> fragment []) )
    member _.app (xs : seq<SutilElement>) : SutilElement = fragment xs
    member _.body (xs: seq<SutilElement>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (DomNode ctx.Document.body) |> buildChildren xs
        unitResult(ctx,"body")

    member _.parent (selector:string) (xs: seq<SutilElement>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (DomNode (ctx.Document.querySelector selector)) |> buildChildren xs
        unitResult(ctx,"parent")

    member _.text (v : IObservable<string>) = Bind.fragment (v |> Store.distinct) DOM.text
    member _.text (v : IObservable<int>) = Bind.fragment (v .> string |> Store.distinct) DOM.text
    member _.text (v : IObservable<float>) = Bind.fragment (v  .> string |> Store.distinct) DOM.text

    member _.fragment (v : IObservable<SutilElement>) = Bind.fragment v id

type SutilAttrEngine() =
    inherit AttrEngine<SutilElement>((fun key value -> attr(key, value)),
                                    (fun key value -> attr(key, value)))

    member _.disabled<'T> (value: IObservable<'T>) = bindAttrIn "disabled" value

    member _.value(value:obj)   = attr("value",value)
    member _.value(value:int)   = attr("value",value)
    member _.value(value:float) = attr("value",value)
    member _.value(value:bool)  = attr("value",value)

    member _.value<'T> (value: IObservable<'T>) = bindAttrIn "value" value
    member _.value<'T> (value: IObservable<'T>, dispatch: 'T -> unit) =
            bindAttrBoth "value" value dispatch

    member _.style (cssAttrs : (string*obj) seq) = cssAttrs |> Sutil.Attr.style

let Html = SutilHtmlEngine()

let Attr = SutilAttrEngine()

let Css =  CssEngine(fun k v -> k, box v)

let cssAttr = id
let addClass       (n:obj) = cssAttr("sutil-add-class",n)
let useGlobal              = cssAttr("sutil-use-global","" :> obj)

// Convenience
let text s = DOM.text s

let exampleVirtualNodes =
    Html.div [
        text "Hello"
        fragment [
            text "World"
        ]
        fragment [
            text "A"
            fragment [
                text "B"
            ]
            text "C"
        ]
    ]

(*

   DIVElement
      TextNode "Hello"
      TextNode "World"
      TextNode "A"
      TextNode "B"
      TextNode "C"
*)