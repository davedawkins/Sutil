module Sutil.Navigable

open Browser.Types
open Sutil.DOM
open Browser.Dom
open System

type Parser<'T> = Location -> 'T

let listenLocation<'T> (parser:Parser<'T>) (dispatch: 'T -> unit) =
    let mutable onChangeRef : obj -> obj =
        fun _ ->
            failwith "`onChangeRef` has not been initialized.\nPlease make sure you used Elmish.Navigation.Program.Internal.subscribe"

    let subscribe () =
        let mutable lastLocation = None
        let onChange _ =
            match lastLocation with
            | Some href when href = window.location.href -> ()
            | _ ->
                lastLocation <- Some window.location.href
                window.location |> parser |> dispatch
            |> box

        onChangeRef <- onChange

        window.addEventListener("popstate", unbox onChangeRef)
        window.addEventListener("hashchange", unbox onChangeRef)

        onChange() // Initialize with starting href

    let unsubscribe () =
        window.removeEventListener("popstate", unbox onChangeRef)
        window.removeEventListener("hashchange", unbox onChangeRef)

    subscribe()

    unsubscribe

let navigable<'T> (parser:Parser<'T>) (app : IObservable<'T> -> NodeFactory) =
    let store = Store.make (window.location |> parser)
    let u = listenLocation parser (Store.set store)

    fragment [
        disposeOnUnmount [ store; Helpers.disposable u ]
        store |> app
    ]

