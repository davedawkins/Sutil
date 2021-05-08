module Sutil.Navigable

open Browser.Types
open Sutil.DOM
open Browser.Dom
open System
open Interop

type Parser<'T> = Location -> 'T

let listenLocation<'T> (parser:Parser<'T>) (dispatch: 'T -> unit) =
    let mutable onChangeRef : obj -> obj =
        fun _ ->
            failwith "`onChangeRef` has not been initialized.\nPlease make sure you used Elmish.Navigation.Program.Internal.subscribe"

    let subscribe () =
        let mutable lastLocation = None
        let onChange _ =
            match lastLocation with
            | Some href when href = Window.location.href -> ()
            | _ ->
                lastLocation <- Some Window.location.href
                Window.location |> parser |> dispatch
            |> box

        onChangeRef <- onChange

        Window.addEventListener("popstate", unbox onChangeRef)
        Window.addEventListener("hashchange", unbox onChangeRef)

        onChange() |> ignore // Initialize with starting href

    let unsubscribe () =
        Window.removeEventListener("popstate", unbox onChangeRef)
        Window.removeEventListener("hashchange", unbox onChangeRef)

    subscribe()

    unsubscribe

let navigable<'T> (parser:Parser<'T>) (app : IObservable<'T> -> SutilElement) =
    let store = Store.make (Window.location |> parser)
    let u = listenLocation parser (Store.set store)

    fragment [
        disposeOnUnmount [ store; Helpers.disposable u ]
        store |> app
    ]

