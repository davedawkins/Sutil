[<AutoOpen>]
module Sutil.Html

open Sutil.DOM
open Feliz
open System

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of SutilElement

type SutilEventEngine() =
    inherit EventEngine<SutilElement>( fun (event:string) handler -> Sutil.Attr.on (event.ToLower()) handler [] )

type SutilHtmlEngine() as this =
    inherit HtmlEngine<SutilElement>( el, text, (fun () -> fragment []) )
    member _.app (xs : seq<SutilElement>) : SutilElement = fragment xs
    member _.body (xs: seq<SutilElement>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (DomNode ctx.Document.body) |> buildChildren xs
        unitResult(ctx,"body")

    member _.parent (selector:string) (xs: seq<SutilElement>) = nodeFactory <| fun ctx ->
        ctx |> ContextHelpers.withParent (DomNode (ctx.Document.querySelector selector)) |> buildChildren xs
        unitResult(ctx,"parent")

    member _.text (v : IObservable<string>) = Bind.el (v |> Store.distinct, DOM.text)
    member _.text (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, DOM.text)
    member _.text (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, DOM.text)

    member _.td (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.td)
    member _.td (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.td)
    member _.td (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.td)

    member _.span (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.span)
    member _.span (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.span)
    member _.span (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.span)

    member _.h1 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h1)
    member _.h1 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h1)
    member _.h1 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h1)

    member _.h2 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h2)
    member _.h2 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h2)
    member _.h2 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h2)

    member _.h3 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h3)
    member _.h3 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h3)
    member _.h3 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h3)

    member _.h4 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h4)
    member _.h4 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h4)
    member _.h4 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h4)

    member _.h5 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h5)
    member _.h5 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h5)
    member _.h5 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h5)

    member _.h6 (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.h6)
    member _.h6 (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.h6)
    member _.h6 (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.h6)

    member _.div (v : IObservable<string>) = Bind.el (v |> Store.distinct, this.div)
    member _.div (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, this.div)
    member _.div (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, this.div)

    member _.fragment (v : IObservable<SutilElement>) = Bind.el(v,id)

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
    member _.styleAppend (cssAttrs : (string*obj) seq) = cssAttrs |> Sutil.Attr.styleAppend
    member _.style (cssAttrs : IObservable< #seq<string*obj> >) = Bind.style cssAttrs

    member _.none = nodeFactory <| fun ctx -> unitResult(ctx,"none")


let Html = SutilHtmlEngine()

let Attr = SutilAttrEngine()

let Ev = SutilEventEngine()

let Css =  CssEngine(fun k v -> k, box v)

let cssAttr = id
let addClass       (n:obj) = cssAttr("sutil-add-class",n)
let useGlobal              = cssAttr("sutil-use-global","" :> obj)

type Media() =
    static member Custom (condition : string) rules = makeMediaRule condition rules
    static member MinWidth (minWidth : Styles.ICssUnit, rules : StyleSheetDefinition list) = makeMediaRule (sprintf "(min-width: %s)" (string minWidth)) rules
    static member MaxWidth (maxWidth : Styles.ICssUnit, rules : StyleSheetDefinition list) = makeMediaRule (sprintf "(max-width: %s)" (string maxWidth)) rules

// Convenience
let text s = DOM.text s

