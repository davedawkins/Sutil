///  <exclude />
module internal Sutil.Logging

open System.Collections.Generic
open Browser.Dom

let internal enabled = Dictionary<string,bool>()

let le() = DevToolsControl.Options.LoggingEnabled

let mutable initialized = false

let init =
    if not initialized then
        console.log("logging:init defaults")
        initialized <- true
        enabled.["store"] <- false
        enabled.["trans"] <- false
        enabled.["dom"  ] <- true
        enabled.["core"  ] <- true
        enabled.["core-elements"  ] <- true
        enabled.["style"] <- false
        enabled.["bind" ] <- true
        enabled.["each" ] <- true
        enabled.["tick" ] <- false

let initWith states =
    console.log("logging:init with states")
    initialized <- true
    for (name,state) in states do
        console.log($"logging:{name}: {state}")
        enabled.[name] <- state

let timestamp() =
    sprintf "%0.3f" (((float)System.DateTime.Now.Ticks / 10000000.0) % 60.0)

let isEnabled source =
    le() && (not (enabled.ContainsKey(source)) || enabled.[source])

let log source (message : string) =
    if isEnabled source then
        console.log(sprintf "%s: %s: %s" (timestamp()) source message)

let warning (message : string) =
    console.log(sprintf "warning: %s" message)

let error (message : string) =
    console.log(sprintf "error: %s" message)

