module CustomEvents

open Sutil
open type Feliz.length
open Sutil.DOM
open Sutil.Attr
open Browser.Types
open System

let customDispatchButton() =
    let r = Random()

    let clickHandler (e: Event) =
        let props: CustomDispatch<string> list = [Bubbles true; Detail(Some $"Hello there! %i{r.Next(1000)}")]
        CustomDispatch.dispatch<string>(e,"on-custom-click", props)

    Html.button [
        onClick clickHandler []
        text "I will dispatch an 'on-custom-click' event"
    ]

let view() =
    Html.div [
        let m = Store.make ""

        disposeOnUnmount [m]

        onCustomEvent<string>
            "on-custom-click"
            (fun (e: CustomEvent<string>) -> e.detail |> Option.defaultValue "" |> Store.set m)
            []

        Html.div [
            customDispatchButton()

            Bind.el(m,fun s ->
                Html.p [
                    text $"Got: [{s}]"

                    style [ Css.marginTop (px 12) ]
                ] )
        ]
    ]

view() |> Program.mountElement "sutil-app"
