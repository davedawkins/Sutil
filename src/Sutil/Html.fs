[<AutoOpen>]
module Sutil.Html

open Sutil.DOM
open Feliz
open System

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of NodeFactory

type SutilHtmlEngine(helper) =
    inherit HtmlEngine<NodeFactory>(helper)
    member _.app (xs : seq<NodeFactory>) : NodeFactory = fragment xs
    member _.body (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.body) |> buildChildren xs

    member _.parent (selector:string) (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.querySelector selector) |> buildChildren xs

    member _.text (v : IObservable<string>) = Bind.fragment (v |> Store.distinct) DOM.text
    member _.text (v : IObservable<int>) = Bind.fragment (v .> string |> Store.distinct) DOM.text
    member _.text (v : IObservable<float>) = Bind.fragment (v  .> string |> Store.distinct) DOM.text

    member _.fragment (v : IObservable<NodeFactory>) = Bind.fragment v id

type SutilAttrEngine(helper) =
    inherit AttrEngine<NodeFactory>(helper)

    member _.disabled<'T> (value: IObservable<'T>) = bindAttrIn "disabled" value

    member _.value<'T> (value: IObservable<'T>) = bindAttrIn "value" value
    member _.value<'T> (value: IObservable<'T>, dispatch: 'T -> unit) =
            bindAttrBoth "value" value dispatch

let Html = SutilHtmlEngine( {new HtmlHelper<NodeFactory> with
            member _.MakeNode(tag, nodes) = el tag nodes
            member _.StringToNode(v) = text v
            member _.EmptyNode = fragment []
            })

let Attr =
    SutilAttrEngine
        { new AttrHelper<NodeFactory> with
            member _.MakeAttr(key, value) = attr(key, value)
            member _.MakeBooleanAttr(key, value) = attr(key, value) }

// Convenience
let text s = DOM.text s