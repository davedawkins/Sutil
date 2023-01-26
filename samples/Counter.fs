module Counter

open Sutil
open Sutil.Core
open Sutil.CoreElements


let view() =
    Html.div [
        bindStore 0 <| fun count -> fragment [
            Html.div [
                class' "block"
                Bind.el(count, fun n -> text $"Counter = {n}")
            ]

            Html.div [
                class' "block"
                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n-1)) []
                    text "-"
                ]

                Html.button [
                    onClick (fun _ -> count <~= (fun n -> n+1)) []
                    text "+"
                ]
            ]]
    ]

view() |> Program.mountElement "sutil-app"
