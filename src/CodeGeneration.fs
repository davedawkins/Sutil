module Sveltish
module CodeGeneration =
    let makeIdGenerator() =
        let mutable id = 0
        fun () ->
            let r = id
            id <- id+1
            r