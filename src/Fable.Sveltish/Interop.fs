module Sveltish.Interop

open Fable.Core
open Fable.Core.JsInterop

[<Emit("new CustomEvent($0, $1)")>]
let customEvent name data = jsNative

[<Emit("() => ($0)[$1]")>]
let getter obj name = jsNative

[<Emit("value => { ($0)[$1] = value; }")>]
let setter obj name = jsNative

[<Emit("$0[$1] = $2")>]
let set ob name value : unit = jsNative

[<Emit("$0[$1]")>]
let get ob name = jsNative

[<Emit("$0.hasOwnProperty($1)")>]
let exists ob name = jsNative

//[<ImportAll("../Fable.Sveltish/proxy.js")>]
let makeProxy<'T>  ((a:'T),(b: obj -> unit )) : 'T = importMember "../Fable.Sveltish/proxy.js"