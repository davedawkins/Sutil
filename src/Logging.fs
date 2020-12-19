namespace Sveltish
module Logging =

    open System.Collections.Generic
    open Browser.Dom
    open System
    open Browser.Types

    let enabled = Dictionary<string,bool>()

    let init =
        enabled.["store"] <- true
        enabled.["trans"] <- true
        enabled.["dom"  ] <- false
        enabled.["style"  ] <- false

    let log source (message : string) =
        if not (enabled.ContainsKey(source)) || enabled.[source] then
            console.log(sprintf "%s: %s" source message)
