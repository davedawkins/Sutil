module AwaitBlocks

open Sveltish
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings

module RandomUser =
    type Name = { title: string; first : string; last : string }
    type RandomUser = { name : Name }
    type RandomUserResult = { results : RandomUser array }

    let get() = promise {
        let! response = Fetch.fetch "https://randomuser.me/api/?inc=name" []
        let! responseText = response.text()
        let result = (Fable.Core.JS.JSON.parse responseText) :?> RandomUserResult
        return result.results.[0]
    }

let randomName = ObservablePromise<string>()

let getRandomName _ =
    randomName.Run <| promise {
        do! Promise.sleep(500) // So we can see more of the "Waiting..." phase

        let! u = RandomUser.get()

        if (u.GetHashCode() % 3 = 1) then // Simulate a failure so we an see "Error" state
            failwith "Randomly failed to get name"

        return $"{u.name.first} {u.name.last}"
    }

let view() =
    Html.div [

        onShow getRandomName []

        Html.button [
            class' "block"
            onClick getRandomName []
            text "generate random name"
        ]

        Html.div [
            class' "block"
            bind randomName <| function
                | Waiting ->
                    text "...waiting"
                | Result n ->
                    text $"Please welcome {n}"
                | Error x -> Html.p [
                    style "color: red"
                    text (string x.Message)
                ]
        ]
    ]
