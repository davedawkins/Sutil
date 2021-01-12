namespace Sveltish
open System

type IStore<'T> = interface
    inherit IObservable<'T>
    abstract Update: f:('T -> 'T) -> unit
    abstract Value: 'T
    end

type Store<'T> = IStore<'T>

type Update<'Model> = ('Model -> 'Model) -> unit // A store updater. Store updates by being passed a model updater
type Dispatch<'Msg> = 'Msg -> unit // Message dispatcher
type Cmd<'Msg> = (Dispatch<'Msg> -> unit) list // List of commands. A command needs a dispatcher to execute

module Cmd =

    let none : Cmd<'Msg> = [ ]

    let ofMsg msg : Cmd<'Msg> = [ fun d -> d msg ]

    let batch (cmds : Cmd<'Msg> list) : Cmd<'Msg> = cmds |> List.concat

    module OfFunc =
        let either (task: 'args -> _)  (a:'args) (success:_ -> 'msg')(error: _ -> 'msg') =
            [ fun d -> try task a |> (success >> d) with |x -> x |> (error >> d) ]

        let perform (task: 'args -> _) (a:'args) (success:_ -> 'msg') =
            [ fun d -> try task a |> (success >> d) with |_ -> () ]

        let attempt (task: 'args -> unit) (a:'args) (error: _ -> 'msg') =
            [ fun d -> try task a with |x -> x |> (error >> d) ]

