module Sveltish.Logging

open System.Collections.Generic
open Browser.Dom

let enabled = Dictionary<string,bool>()

let le() = DevToolsControl.Options.LoggingEnabled

let init =
    enabled.["store"] <- false
    enabled.["trans"] <- true
    enabled.["dom"  ] <- true
    enabled.["style"] <- false
    enabled.["bind" ] <- true

let log source (message : string) =
    if le() && (not (enabled.ContainsKey(source)) || enabled.[source]) then
        console.log(sprintf "%0.3f: %s: %s" (((float)System.DateTime.Now.Ticks / 10000000.0) % 60.0) source message)

let warning (message : string) =
    console.log(sprintf "warning: %s" message)

let error (message : string) =
    console.log(sprintf "error: %s" message)

