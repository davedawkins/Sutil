module Sutil.Logging

open System.Collections.Generic
open Browser.Dom

let enabled = Dictionary<string,bool>()

let le() = DevToolsControl.Options.LoggingEnabled

let mutable initialized = false

let init =
    if not initialized then
        console.log("logging:init defaults")
        initialized <- true
        enabled.["store"] <- false
        enabled.["trans"] <- false
        enabled.["dom"  ] <- false
        enabled.["style"] <- false
        enabled.["bind" ] <- false
        enabled.["each" ] <- false
        enabled.["tick" ] <- false

let initWith states =
    console.log("logging:init with states")
    initialized <- true
    for (name,state) in states do
        console.log($"logging:{name}: {state}")
        enabled.[name] <- state

let log source (message : string) =
    if le() && (not (enabled.ContainsKey(source)) || enabled.[source]) then
        console.log(sprintf "%0.3f: %s: %s" (((float)System.DateTime.Now.Ticks / 10000000.0) % 60.0) source message)

let warning (message : string) =
    console.log(sprintf "warning: %s" message)

let error (message : string) =
    console.log(sprintf "error: %s" message)

