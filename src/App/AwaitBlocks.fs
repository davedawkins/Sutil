module AwaitBlocks

// Adapted from
// https://svelte.dev/examples#await-blocks

open Sutil

open Sutil.Core
open Sutil.CoreElements

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

let getRandomName() =
    promise {
        do! Promise.sleep(500) // So we can see more of the "Waiting..." phase

        let! u = RandomUser.get()

        if (u.GetHashCode() % 3 = 1) then // Simulate a failure so we an see "Error" state
            failwith "Simulated failure to get name"

        return $"{u.name.first} {u.name.last}"
    }

let randomNames = getRandomName() |> Store.make

let view() =
    Html.div [
        Html.button [
            class' "block"
            Ev.onClick (fun _ -> getRandomName() |> Store.set randomNames)
            text "generate random name"
        ]

        Html.div [
            class' "block"
            Bind.promises( randomNames,
                (fun n -> text $"Please welcome {n}"),
                (text "...waiting"),
                (fun x ->
                    Html.p [
                        style [ Css.color "red" ]
                        text (string x.Message)
                    ]
                )
            )
        ]
    ]
