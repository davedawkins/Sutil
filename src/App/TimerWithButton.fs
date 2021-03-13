module TimerWithButton

open Sutil
open Sutil.Bulma
open type Feliz.length
open Sutil.Attr
open System
open Sutil.DOM

//
// Example component making use of the TimerLogic component.
// User supplies a text label
//

type Model = {  Started : bool }

type Message =  Toggle

let init () = { Started = false }, Cmd.none


let update msg model =
    match msg with
    | Toggle -> { model with Started = not model.Started }, Cmd.none

let icon (name : IObservable<string>) =
    Html.i [
        //class' "fa fa-play"
        Bindings.bindAttrIn "class" (name |> Store.map (sprintf "fa fa-%s"))
    ]

let create (slot : IObservable<bool * float> -> NodeFactory) =
    let model, dispatch = () |> Store.makeElmish init update ignore

    model
    |> Store.map (fun m -> m.Started)
    |> TimerLogic.create <| fun elapsed ->
        bulma.columns [
            columns.isVcentered
            Bindings.bindClass (model |> Store.map (fun m -> m.Started)) "running"

            bulma.column [
                column.isHalf
                slot elapsed
                //elapsed
                //|> Store.map (fun t -> sprintf "00:%02ds"  (int t))
                //|> Html.text
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
