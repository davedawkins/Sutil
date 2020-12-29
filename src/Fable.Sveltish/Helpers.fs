module Sveltish.Helpers

let internal disposable f =
    { new System.IDisposable with
        member _.Dispose() = f () }


let makeIdGeneratorFrom start =
    let mutable id = start
    fun () ->
        let r = id
        id <- id+1
        r

let makeIdGenerator() = makeIdGeneratorFrom 0
