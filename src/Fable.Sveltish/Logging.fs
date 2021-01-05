module Sveltish.Logging

open System.Collections.Generic
open Browser.Dom

let enabled = Dictionary<string,bool>()

let loggingEnabled = false

let init =
    enabled.["store"] <- loggingEnabled && false
    enabled.["trans"] <- loggingEnabled && false
    enabled.["dom"  ] <- loggingEnabled && true
    enabled.["style"] <- loggingEnabled && false
    enabled.["bind" ] <- loggingEnabled && true

let log source (message : string) =
    if not (enabled.ContainsKey(source)) || enabled.[source] then
        console.log(sprintf "%s: %s" source message)

let warning (message : string) =
    console.log(sprintf "warning: %s" message)

let error (message : string) =
    console.log(sprintf "error: %s" message)

