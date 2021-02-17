module Counter

open Sutil
open Sutil.Bindings
open Sutil.DOM
open Sutil.Attr

open Feliz

// Dummy type to avoid problems with overload resolution in HtmlEngine
type [<Fable.Core.Erase>] NodeAttr = NodeAttr of NodeFactory

let Html =
    HtmlEngine
        { new HtmlHelper<NodeFactory, NodeAttr> with
            override _.MakeEl(tag, nodes) = el tag (unbox nodes)
            override _.ChildrenToProp(children) = NodeAttr(fragment children)
            override _.StringToEl(v) = text v

            override _.EmptyEl = failwith "Not Implemented"
            override _.FloatToEl(v) = failwith "Not Implemented"
            override _.IntToEl(v) = failwith "Not Implemented"
            override _.BoolToEl(v) = failwith "Not Implemented" }

let Counter() =
    bindStore 0 <| fun count -> Html.div [
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
        ]
    ]
