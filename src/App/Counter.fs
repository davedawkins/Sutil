module Counter

open Sutil
open Sutil.Attr
open Sutil.Feliz

let Counter() =
    bindStore 0 <| fun count -> Html.div [
        Html.div [
            Attr.className "block"
            Bind.fragment count <| fun n -> Html.text $"Counter = {n}"
        ]

        Html.div [
            Attr.className "block"
            Html.button [
                onClick (fun _ -> count <~= (fun n -> n-1)) []
                Html.text "-"
            ]
            Html.button [
                onClick (fun _ -> count <~= (fun n -> n+1)) []
                Html.text "+"
            ]
        ]
    ]
