// Work in progress
module GroupInputs

open Sveltish
open Sveltish.DOM
open Sveltish.Attr

let flavours = Store.make( [ "Mint choc chip" ] )
let numFlavours = flavours |> Store.map (fun x -> x.Length)
let scoops = Store.make(1)

let menu = [
    "Cookies and cream"
    "Mint choc chip"
    "Raspberry ripple"
]

let scoopMenu = [
    "One scoop"
    "Two scoops"
    "Three scoops"
]

//let join flavours =
//    if (flavours.length = 1) then flavours[0]
//    $"{flavours.slice(0, -1).join(", ")} and ${flavours[flavours.length - 1]}"

let control children =
    Html.div <| (class' "control") :: children

// Control with only 1 label child
let controlLabel children =
    Html.div [ class' "control"; Html.label children ]

let view() =
    Html.div [

        control [
            Html.label [
                class' "label";
                text "Scoops"
            ]

            controlLabel [
                class' "radio"
                Html.input [
                    type' "radio"
                    Bindings.bindGroup scoops
                    value "1"
                ]
                text " One scoop "
            ]

            controlLabel [
                class' "radio"
                Html.input [
                    type' "radio"
                    Bindings.bindGroup scoops
                    value "2"
                ]
                text " Two scoops "
            ]

            controlLabel [
                class' "radio"
                Html.input [
                    type' "radio"
                    Bindings.bindGroup scoops
                    value "3"
                ]
                text " Three scoops "
            ]
        ]

        control [
            Html.label [
                class' "label";
                text "Flavours"
            ]

            //{#each menu as flavour}
            fragment [
                for flavour in menu do
                    controlLabel [
                        class' "checkbox"
                        Html.input [
                            type' "checkbox"
                            // bind:group={flavours}
                            value flavour
                            ]
                        text $" {flavour}"
                    ]
            ]
        ]
        //{/each}

        Html.p [
            Bindings.bind2 scoops numFlavours (fun (ns,nf) ->text $"Number of scoops: {ns} Number of flavours: {nf}")
        ]

        //Html.p [
        //    Bindings.bind scoops  (fun ns -> text $"{ns}" )
        //]
        //Html.p [
        //    Bindings.bind numFlavours (fun nf -> text $"{nf}" )
        //]
        //Bindings.transitionList [
        //    {   predicate = numFlavours |> Store.map ((=) 0)
        //        transOpt = None
        //        element = Html.p [ text "Please select at least one flavour" ]
        //        }
        //    {   predicate = Store.map2 (fun (nf,ns) -> nf > ns) numFlavours scoops
        //        transOpt = None
        //        element = Html.p [ text "Can't order more flavours than scoops!" ]
        //        }
        //    {   predicate = Store.map2 (fun (nf,ns) -> nf > 0 && nf <= ns) numFlavours scoops
        //        transOpt = None
        //        element = Html.p [ text $"You ordered _ns_ of _flavours_" ]
        //        }
        //]
    ]