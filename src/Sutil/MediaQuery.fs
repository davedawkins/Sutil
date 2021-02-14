
module Sutil.MediaQuery

open System
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition
open Browser
open Fable.Core.JsInterop

let mediaQuery (query:string) (handler : bool -> unit) : unit -> unit =
    let mql = window.matchMedia( query )
    handler (mql.matches)
    listen "change" mql <| fun e -> e?matches |> handler

let bindMediaQuery (query:string) (view : bool -> NodeFactory) =
    let s = Store.make false
    let u = mediaQuery query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        bind s view
    ]

let showIfMedia2 (query:string) (f:bool->bool) (trans) (view : NodeFactory) =
    let s = Store.make false
    let u = mediaQuery query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        transition trans (s .> f) view
    ]

let showIfMedia (query:string) (trans) (view : NodeFactory) =
    showIfMedia2 query id trans view

let media<'T> (query:string) (map:bool -> 'T) (app : IObservable<'T> -> NodeFactory) =
    let s = Store.make false
    let u = mediaQuery query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        s .> map |> app
    ]
