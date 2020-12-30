module Sveltish.Logging

    open System.Collections.Generic
    open Browser.Dom

    let enabled = Dictionary<string,bool>()

    let loggingEnabled = false

    let init =
        enabled.["store"] <- loggingEnabled && false
        enabled.["trans"] <- loggingEnabled && true
        enabled.["dom"  ] <- loggingEnabled && false
        enabled.["style"] <- loggingEnabled && false
        enabled.["bind" ] <- loggingEnabled && true

    let log source (message : string) =
        if not (enabled.ContainsKey(source)) || enabled.[source] then
            console.log(sprintf "%s: %s" source message)
