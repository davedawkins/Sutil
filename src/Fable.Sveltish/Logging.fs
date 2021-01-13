module Sveltish.Logging

open System.Collections.Generic
open Browser.Dom

let enabled = Dictionary<string,bool>()

let loggingEnabled = true

let init =
    enabled.["store"] <- loggingEnabled && false
    enabled.["trans"] <- loggingEnabled && true
    enabled.["dom"  ] <- loggingEnabled && true
    enabled.["style"] <- loggingEnabled && false
    enabled.["bind" ] <- loggingEnabled && true

let log source (message : string) =
    if not (enabled.ContainsKey(source)) || enabled.[source] then
        console.log(sprintf "%0.3f: %s: %s" (((float)System.DateTime.Now.Ticks / 10000000.0) % 60.0) source message)

let warning (message : string) =
    console.log(sprintf "warning: %s" message)

let error (message : string) =
    console.log(sprintf "error: %s" message)

