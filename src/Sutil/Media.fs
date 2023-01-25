namespace Sutil

open System
open Sutil.Core
open Sutil.CoreElements
open Sutil.Transition
open Sutil.DomHelpers
open Fable.Core.JsInterop
open Sutil.Interop
open Feliz

/// <summary>
/// Helpers for listening and reacting to media changes
/// </summary>
type Media =
    /// <summary>
    /// </summary>
    static member listenMedia (query:string, handler : bool -> unit) : unit -> unit =
        let mql = Window.matchMedia( query )
        handler (mql.matches)
        listen "change" mql (fun e -> e?matches |> handler)

    /// <summary>
    /// </summary>
    static member bindMediaQuery (query:string, view : bool -> SutilElement) =
        let s = Store.make false
        let u = Media.listenMedia(query,fun m -> s <~ m)
        fragment [
            disposeOnUnmount [ Helpers.disposable u; s ]
            Bind.el(s,view)
        ]

    /// <summary>
    /// </summary>
    static member showIfMedia (query:string, f:bool->bool, trans, view : SutilElement) =
        let s = Store.make false
        let u = Media.listenMedia(query, fun m -> s <~ m)
        fragment [
            disposeOnUnmount [ Helpers.disposable u; s ]
            transition trans (s .> f) view
        ]

    /// <summary>
    /// </summary>
    static member showIfMedia (query:string, trans, view : SutilElement) =
        Media.showIfMedia(query,id,trans,view)

    /// <summary>
    /// </summary>
    static member media<'T> (query:string, map:bool -> 'T, app : IObservable<'T> -> SutilElement) =
        let s = Store.make false
        let u = Media.listenMedia(query,fun m -> s <~ m)
        fragment [
            disposeOnUnmount [ Helpers.disposable u; s ]
            s .> map |> app
        ]

/// <summary>
/// Helpers for basic CSS media queries
/// </summary>
type CssMedia =
    /// <summary>
    /// Create a <c>@media</c> CSS rule for a custom condition and stylesheet
    /// </summary>
    static member custom (condition : string, rules) =
        Styling.makeMediaRule condition rules
    /// <summary>
    /// Create a <c>@media (min-width: &lt;nnn>)</c> CSS rule
    /// </summary>
    static member minWidth (minWidth : Styles.ICssUnit, rules : StyleSheetDefinition list) =
        Styling.makeMediaRule (sprintf "(min-width: %s)" (string minWidth)) rules
    /// <summary>
    /// Create a <c>@media (max-width: &lt;nnn>)</c> CSS rule
    /// </summary>
    static member maxWidth (maxWidth : Styles.ICssUnit, rules : StyleSheetDefinition list) =
        Styling.makeMediaRule (sprintf "(max-width: %s)" (string maxWidth)) rules
