namespace Sutil

open Sutil
open Browser.Types
open Core
open CoreElements
open System
open Interop

/// <summary>
/// Window location events
/// </summary>
module Navigable =
    /// <summary>
    /// Parser that can convert <c>Location</c> to a <c>'T</c>. For example, convert the url <c>https//sutil.dev/#documentation</c> into
    /// the DU value <c>Pages.Documenation</c>
    /// </summary>
    type Parser<'T> = Location -> 'T

open Navigable

/// <summary>
/// Window location events
/// </summary>
type Navigable =

    /// <summary>
    /// Call <c>dispatch</c> each time the window's location changes. The location is parsed into a <c>'T</c> with the given <c>parser</c>/
    /// </summary>
    static member listenLocation (onChangeLocation: Location -> unit) =
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
                    Window.location |> onChangeLocation
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

    /// <summary>
    /// Call <c>dispatch</c> each time the window's location changes. The location is parsed into a <c>'T</c> with the given <c>parser</c>/
    /// </summary>
    static member listenLocation<'T> (parser:Parser<'T>, dispatch: 'T -> unit) =
        Navigable.listenLocation (dispatch<<parser)

    /// <summary>
    /// Bind the window location to a view
    /// </summary>
    static member bindLocation<'T> ( view : Location -> SutilElement ) =
        let store = Store.make (Window.location)
        fragment [
            disposeOnUnmount [
                store
                Navigable.listenLocation(id,Store.set store) |> Helpers.disposable
            ]
            Bind.el( store, view )
        ]

