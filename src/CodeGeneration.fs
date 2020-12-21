module Sveltish.CodeGeneration


let makeIdGeneratorFrom start =
    let mutable id = start
    fun () ->
        let r = id
        id <- id+1
        r

let makeIdGenerator() = makeIdGeneratorFrom 0
