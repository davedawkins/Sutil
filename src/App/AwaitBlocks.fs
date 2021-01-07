module AwaitBlocks

open Fetch
open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open System

let promisedNo = PromiseStore<string>()

let getRandomNumber _ =
    promisedNo.Run <| promise {
        let! res = Helpers.fetch "http://svelte.dev/tutorial/random-number" []
        let! text = res.text()
        if not res.Ok then failwith text
        return text
    }

let view() =
    Html.div [

        onShow getRandomNumber

        Html.button [
            class' "block"
            onClick getRandomNumber
            text "generate random number"
        ]

        Html.div [
            class' "block"
            bind promisedNo.Store <| fun r ->
                match r with
                | Waiting ->
                    text "...waiting"
                | Result n ->
                    text $"The number is {n}"
                | Error x -> Html.p [
                    style "color: red"
                    text (string x.Message)
                ]
        ]
    ]