module Counter

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

let Counter() =
    Html.div [
        bindStore 0 <| fun count -> fragment [
            Html.div [
                class' "block"
                Bind.fragment count <| fun n -> text $"Counter = {n}"
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
