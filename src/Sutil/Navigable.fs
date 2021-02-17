module Sutil.Navigable

open Browser.Types
open Sutil.DOM
open Browser.Dom
open System

type Parser<'T> = Location -> 'T

let navigable<'T> (parser:Parser<'T>) (app : IObservable<'T> -> NodeFactory) =
    let locationStore = Store.make window.location

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
                locationStore <~ window.location
            |> box

        onChangeRef <- onChange

        window.addEventListener("popstate", unbox onChangeRef)
        window.addEventListener("hashchange", unbox onChangeRef)
//            window.addEventListener(Navigation.NavigatedEvent, unbox onChangeRef)

    let unsubscribe () =
        window.removeEventListener("popstate", unbox onChangeRef)
        window.removeEventListener("hashchange", unbox onChangeRef)
//            window.removeEventListener(Navigation.NavigatedEvent, unbox onChangeRef)

    subscribe()

    fragment [
        disposeOnUnmount [ locationStore; Helpers.disposable unsubscribe ]

        locationStore |> Store.map parser |> app
    ]

