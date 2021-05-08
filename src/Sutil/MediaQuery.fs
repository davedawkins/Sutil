
module Sutil.MediaQuery

open System
open Sutil.DOM
open Sutil.Bindings
open Sutil.Transition
open Browser
open Fable.Core.JsInterop
open Interop

let listenMedia (query:string) (handler : bool -> unit) : unit -> unit =
    let mql = Window.matchMedia( query )
    handler (mql.matches)
    listen "change" mql <| fun e -> e?matches |> handler

let bindMediaQuery (query:string) (view : bool -> SutilElement) =
    let s = Store.make false
    let u = listenMedia query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        Bind.fragment s view
    ]

let showIfMedia2 (query:string) (f:bool->bool) (trans) (view : SutilElement) =
    let s = Store.make false
    let u = listenMedia query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        transition trans (s .> f) view
    ]

let showIfMedia (query:string) (trans) (view : SutilElement) =
    showIfMedia2 query id trans view

let media<'T> (query:string) (map:bool -> 'T) (app : IObservable<'T> -> SutilElement) =
    let s = Store.make false
    let u = listenMedia query (fun m -> s <~ m)
    fragment [
        disposeOnUnmount [ Helpers.disposable u; s ]
        s .> map |> app
    ]
