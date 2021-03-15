[<AutoOpen>]
module Sutil.Html

open Sutil.DOM
open Feliz
open System

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of NodeFactory

type SutilHtmlEngine() =
    inherit HtmlEngine<NodeFactory>(makeNode     = Func<string,NodeFactory seq,NodeFactory>(el),
                                    stringToNode = Func<string,NodeFactory>(text),
                                    emptyNode    = Func<NodeFactory>(fun () -> fragment []))
    member _.app (xs : seq<NodeFactory>) : NodeFactory = fragment xs
    member _.body (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.body) |> buildChildren xs

    member _.parent (selector:string) (xs: seq<NodeFactory>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (ctx.Document.querySelector selector) |> buildChildren xs

    member _.text (v : IObservable<string>) = Bind.fragment (v |> Store.distinct) DOM.text
    member _.text (v : IObservable<int>) = Bind.fragment (v .> string |> Store.distinct) DOM.text
    member _.text (v : IObservable<float>) = Bind.fragment (v  .> string |> Store.distinct) DOM.text

    member _.fragment (v : IObservable<NodeFactory>) = Bind.fragment v id

type SutilAttrEngine() =
    inherit AttrEngine<NodeFactory>(
                makeAttr        = Func<string,string,NodeFactory>(fun key value -> attr(key, value)),
                makeBooleanAttr = Func<string,bool,NodeFactory>(fun key value -> attr(key, value)))

    member _.disabled<'T> (value: IObservable<'T>) = bindAttrIn "disabled" value

    member _.value(value:obj)   = attr("value",value)
    member _.value(value:int)   = attr("value",value)
    member _.value(value:float) = attr("value",value)
    member _.value(value:bool)  = attr("value",value)

    member _.value<'T> (value: IObservable<'T>) = bindAttrIn "value" value
    member _.value<'T> (value: IObservable<'T>, dispatch: 'T -> unit) =
            bindAttrBoth "value" value dispatch

let Html = SutilHtmlEngine()

let Attr = SutilAttrEngine()

// Convenience
let text s = DOM.text s