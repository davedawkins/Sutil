namespace Sutil

open System

/// <exclude/>
[<Obsolete("Use \"open Sutil\" instead", true)>]
module Html =
    do ()

open Sutil.Core
open Sutil.CoreElements
open Feliz
open System
open Bindings
open CoreElements

// Dummy type to avoid problems with overload resolution in HtmlEngine
///<exclude/>
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of SutilElement

/// <summary>
/// DOM attributes for listening to events
/// </summary>
/// <example>
/// <code>
/// Html.button [
///     Ev.onClick (fun _ -> Fable.Core.JS.console.log("Clicked"))
/// ]
/// </code>
/// </example>
type SutilEventEngine() =
    inherit EventEngine<SutilElement>( fun (event:string) handler -> on (event.ToLower()) handler [] )
    member __.onMount( handler ) = onMount handler []
    member __.onUnmount( handler ) = onUnmount handler []

/// <summary>
/// Functions for building DOM elements.
///
/// Note that not all members are documented here, only a few specialized augmentations over the class <c><a href="https://github.com/alfonsogarciacaro/Feliz.Engine/blob/main/src/Feliz.Engine/HtmlEngine.fs">Feliz.HtmlEngine</a></c>
///
/// In theory, every <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element">HTML element</a> should be an inherited member of <c>SutilHtmlEngine</c>, in the form
///
/// <code>
///     // Create div with float value as a child text node
///     member _.div (value: float)
///
///     // Create div with float value as a child text node
///     member _.div (value: int)
///
///     // Create div with SutilElement value as a child element
///     member _.div (value: SutilElement)
///
///     // Create div with string value as child text node
///     member _.div (value: string)
///
///     // Create div with multiple child SutilElements.
///     // Not all SutilElements are DOM elements; they may be attribute-setters, or event-listeners
///     member _.div (children: seq&lt;SutilElement>)
/// </code>
/// </summary>
/// <example>
/// <code>
/// open Sutil
///
/// let view() =
///     Html.div [
///         Html.h1 "Hello"
///         Html.p "world"
///     ]
/// </code>
/// </example>
type SutilHtmlEngine() as this =
    inherit HtmlEngine<SutilElement>( el, text, (fun () -> fragment []) )

    let _clsch cls ch = [ attr("class",cls); yield! ch ]

    do ()

    member _.app (xs : seq<SutilElement>) : SutilElement = fragment xs
    member _.body (xs: seq<SutilElement>) =
        SutilElement.Define( "Html.body",
        fun ctx ->
            ctx |> ContextHelpers.withParent (DomNode ctx.Document.body) |> buildChildren xs
            () )

    member _.parse (html : string) = CoreElements.html html

    member _.parent (selector:string) (xs: seq<SutilElement>) =
        SutilElement.Define( "Html.parent",
        fun ctx ->
        ctx |> ContextHelpers.withParent (DomNode (ctx.Document.querySelector selector)) |> buildChildren xs
        () )

    member __.divc (cls : string) (children : seq<SutilElement>) = __.div (_clsch cls children)
    member __.buttonc (cls : string) (children : seq<SutilElement>) = __.button (_clsch cls children)
    member __.spanc (cls : string) (children : seq<SutilElement>) = __.span (_clsch cls children)
    member __.imgc (cls : string) (children : seq<SutilElement>) = __.img (_clsch cls children)
    member __.ic (cls : string) (children : seq<SutilElement>) = __.i (_clsch cls children)
    member __.tablec (cls : string) (children : seq<SutilElement>) = __.table (_clsch cls children)
    member __.theadc (cls : string) (children : seq<SutilElement>) = __.thead (_clsch cls children)
    member __.trc (cls : string) (children : seq<SutilElement>) = __.tr (_clsch cls children)
    member __.tdc (cls : string) (children : seq<SutilElement>) = __.td (_clsch cls children)
    member __.inputc (cls : string) (children : seq<SutilElement>) = __.input (_clsch cls children)
    member __.labelc (cls : string) (children : seq<SutilElement>) = __.label (_clsch cls children)
    member __.sectionc (cls : string) (children : seq<SutilElement>) = __.section (_clsch cls children)
    member __.navc (cls : string) (children : seq<SutilElement>) = __.nav (_clsch cls children)
    member __.headerc (cls : string) (children : seq<SutilElement>) = __.header (_clsch cls children)
    member __.footerc (cls : string) (children : seq<SutilElement>) = __.footer (_clsch cls children)
    member __.articlec (cls : string) (children : seq<SutilElement>) = __.article (_clsch cls children)
    member __.asidec (cls : string) (children : seq<SutilElement>) = __.aside (_clsch cls children)
    member __.ulc (cls : string) (children : seq<SutilElement>) = __.ul (_clsch cls children)
    member __.lic (cls : string) (children : seq<SutilElement>) = __.li (_clsch cls children)
    member __.dlc (cls : string) (children : seq<SutilElement>) = __.dl (_clsch cls children)
    member __.dtc (cls : string) (children : seq<SutilElement>) = __.dt (_clsch cls children)
    member __.ddc (cls : string) (children : seq<SutilElement>) = __.dd (_clsch cls children)
    member __.pc (cls : string) (children : seq<SutilElement>) = __.p (_clsch cls children)
    member __.h1c (cls : string) (children : seq<SutilElement>) = __.h1 (_clsch cls children)
    member __.h2c (cls : string) (children : seq<SutilElement>) = __.h2 (_clsch cls children)
    member __.h3c (cls : string) (children : seq<SutilElement>) = __.h3 (_clsch cls children)
    member __.h4c (cls : string) (children : seq<SutilElement>) = __.h4 (_clsch cls children)
    member __.h5c (cls : string) (children : seq<SutilElement>) = __.h5 (_clsch cls children)
    member __.h6c (cls : string) (children : seq<SutilElement>) = __.h6 (_clsch cls children)

    member __.ac (cls : string) (children : seq<SutilElement>) = __.a (_clsch cls children)
    member __.hrc (cls : string) (children : seq<SutilElement>) = __.hr (_clsch cls children)
    member __.tbodyc (cls : string) (children : seq<SutilElement>) = __.tbody (_clsch cls children)
    member __.olc (cls : string) (children : seq<SutilElement>) = __.ol (_clsch cls children)


    member _.text (v : IObservable<string>) = Bind.el (v |> Store.distinct, CoreElements.text)
    member _.text (v : IObservable<int>) = Bind.el (v .> string |> Store.distinct, CoreElements.text)
    member _.text (v : IObservable<float>) = Bind.el (v  .> string |> Store.distinct, CoreElements.text)

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

/// <summary>
/// DOM element attributes
/// </summary>
/// <example>
/// <code>
/// Html.button [
///     Attr.className "is-primary"
/// ]
/// </code>
/// </example>
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

    member _.style (cssAttrs : (string*obj) seq) = cssAttrs |> style
    member _.styleAppend (cssAttrs : (string*obj) seq) = cssAttrs |> styleAppend
    member _.style (cssAttrs : IObservable< #seq<string*obj> >) = Bind.style cssAttrs

    member _.setClass(className : string) = CoreElements.setClass className
    member _.toggleClass(className : string) = CoreElements.toggleClass className
    member _.addClass(className : string) = CoreElements.addClass className
    member _.removeClass(className : string) = CoreElements.removeClass className

    member _.none = CoreElements.nothing

    // Compatibility with code produced from https://thisfunctionaltom.github.io/Html2Feliz/
    // Eg prop.text "Hello World"
    member _.text s = CoreElements.text s


/// <summary>
/// DOM builders such as <c>Html</c>, <c>Attr</c>, <c>Ev</c> and <c>Css</c>
/// </summary>
[<AutoOpen>]
module EngineHelpers =
    /// <summary>
    /// DOM builder. For example, <code>Html.div [ (* children *) ]</code>
    /// </summary>
    let Html = SutilHtmlEngine()

    /// <summary>
    /// DOM attributes, which are key/value pairs for elements. For example, <code>Html.div [ Attr.tabIndex 0 ]</code>
    /// </summary>
    let Attr = SutilAttrEngine()

    /// <summary>
    /// DOM event handlers. Strictly speaking, these are attributes, but it's useful for them to have their
    /// own namespace. For example
    /// <code>
    /// Html.button [
    ///     Ev.onClick (fun _ -> console("click"))
    /// ]
    /// </code>
    /// </summary>
    let Ev = SutilEventEngine()

    /// <summary>
    /// CSS styles. For example, <code>Html.div [ Attr.style [ Css.backgroundColor "red" ] ]</code>
    /// </summary>
    let Css =  CssEngine(fun k v -> k, box v)

    // Convenience
    /// <exclude/>
    let text s = CoreElements.text s

    /// An alias for <c>Attr</c>
    /// Compatibility with code produced by https://thisfunctionaltom.github.io/Html2Feliz/
    // Thanks to @dejanmilicic for the suggestion
    let prop = Attr

///<summary>
/// Experimental pseudo-CSS styles
///</summary>
module PseudoCss =
    /// <exclude/>
    let private  cssAttr = id

    /// <summary>
    /// A pseudo-style that will add the given class if the rule matches. This example would
    /// add the class <c>framework-button</c> to any <c>button</c> element.
    /// <code>
    /// let css = [
    ///     rule "button" [
    ///         PseudoCss.addClass "framework-button"
    ///     ]
    /// ]
    ///
    /// let view() =
    ///     Html.button [
    ///         text "Styled as a framework button"
    ///     ] |> withStyle css
    /// </code>
    /// This was an experiment that would let me build DOM and then use a Sutil scoped style sheet to lift
    /// various elements into an 3rd-party framework (such as Bulma).
    /// In hindsight, it's too magical, not comprehensively implemented with respect to rule matching, and better
    /// achieved in other ways (for example, create wrapper buttons for the framework).
    /// </summary>
    let addClass (n:obj) = cssAttr("sutil-add-class",n)

    /// <exclude/>
    /// <summary>
    /// A pseudo-style that when the containing rule matches, uses the top-level Sutil scoped stylesheet.
    /// For example, this example shows use applying a style that's defined further up the DOM hierarchy
    /// <code>
    /// let rootStyle = [ rule "div" [ Css.color "red"] ]
    /// let middleStyle = [ rule "div" [ Css.color "green"]  ]
    /// let bottomStyle = [ rule "div" [ useGlobal ] ]
    ///
    /// let view () =
    ///     Html.div [
    ///         text "I'm red"
    ///         Html.div [
    ///             text "I'm green"
    ///
    ///             Html.div [
    ///                 text "I'm red"
    ///             ] |> withStyle bottomStyle
    ///
    ///             Html.div [
    ///                 text "I'm green"
    ///             ]
    ///         ] |> withStyle middleStyle
    ///     ] |> withStyle rootStyle
    /// </code>
    ///
    /// In hindsight, it's too magical, confusing and better achieved in other ways (for example have named
    /// stylesheets: <c>withStyle rootStyle</c>)
    /// </summary>
    [<Obsolete("Use named stylesheets instead", true)>]
    let useGlobal = cssAttr("sutil-use-global","" :> obj)
