module Sveltish.Interop

open Fable.Core

[<Emit("new CustomEvent($0, $1)")>]
let customEvent name data = jsNative

[<Emit("() => ($0)[$1]")>]
let getter obj name = jsNative

[<Emit("value => { ($0)[$1] = value; }")>]
let setter obj name = jsNative

[<Emit("$0[$1] = $2")>]
let set ob name value = jsNative

[<Emit("$0[$1]")>]
let get ob name = jsNative

[<Emit("$0.hasOwnProperty($1)")>]
let exists ob name = jsNative

