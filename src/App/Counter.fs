module Counter

open Sutil
open Sutil.CoreElements

let view() =
    let count = Store.make 0

    Html.div [
        disposeOnUnmount [ count ]

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
        ]
    ]
