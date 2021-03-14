module TimerWithButton

///
/// Example component making use of the TimerLogic component.
/// Applies a simple UI to the TimerLogic component, and allows the user to specify
/// how the elapsed time is presented.
///
/// let create (slot : IObservable<bool * float> -> NodeFactory)
///
///      slot - View for timer status. Argument is ( running : bool * elapsedTime : float )
///
/// Classes
/// .running   Added when the timer is running
///

open Sutil
open Sutil.Bulma
open type Feliz.length
open Sutil.Attr
open System
open Sutil.DOM

type Model = {  Started : bool }
type Message =  Toggle

let init () = { Started = false }, Cmd.none

let update msg model =
    match msg with
    | Toggle -> { model with Started = not model.Started }, Cmd.none

let icon (name : IObservable<string>) =
    Html.i [
        Bindings.bindAttrIn "class" (name |> Store.map (sprintf "fa fa-%s"))
    ]

let create (slot : IObservable<bool * float> -> NodeFactory) =
    let model, dispatch = () |> Store.makeElmish init update ignore

    model
    |> Store.map (fun m -> m.Started)
    |> TimerLogic.create <| fun elapsed ->
        bulma.columns [
            columns.isVcentered
            columns.isMobile
            Bindings.bindClass (model |> Store.map (fun m -> m.Started)) "running"

            bulma.column [
                column.isHalf
                slot elapsed
            ]

            bulma.column [
                column.isHalf
                bulma.m.text.hasTextRight

                bulma.button.a [
                    model
                    |> Store.map (fun m -> if m.Started then "stop" else "play")
                    |> icon
                    onClick (fun _ -> dispatch Toggle) [PreventDefault]
                ]
            ]
        ]
