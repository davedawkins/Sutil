/// <exclude/>
module internal Sutil.Interop

open Fable.Core
open Fable.Core.JsInterop

[<Emit("Math.random()")>]
let random() : float = jsNative

[<Emit("new CustomEvent($0, $1)")>]
let customEvent name data = jsNative

[<Emit("() => ($0)[$1]")>]
let getter obj name = jsNative

[<Emit("value => { ($0)[$1] = value; }")>]
let setter obj name = jsNative

[<Emit("$0[$1] = $2")>]
let set<'T> (ob:obj) (name:string) (value:'T) : unit = jsNative

[<Emit("$0[$1]")>]
let get<'T> (ob:obj) (name:string) : 'T = jsNative

[<Emit("delete $0[$1]")>]
let delete ob name : unit = jsNative

[<Emit("$0.hasOwnProperty($1)")>]
let exists (ob:obj) (name:string) : bool= jsNative

let getOption<'a> (ob:obj) (name:string) : 'a option =
    match exists ob name with
    | false -> None
    | true -> Some (get ob name)

let getDefault<'a> (ob:obj) (name:string) (defaultValue : 'a): 'a =
    match exists obj name with
    | false -> defaultValue
    | true -> get ob name

[<Emit("typeof $0 === 'undefined'")>]
let isUndefined (x: 'a) : bool = jsNative

[<Emit("$0 || $1")>]
let ifSetElse(a : 't, b : 't) : 't = jsNative

open Browser.Dom
open Browser.CssExtensions
open Browser.MediaQueryListExtensions

[<Emit("typeof window !== 'undefined'")>]
let windowIsDefined : bool = jsNative

type Window() =
    do ()
    with
        static member alert msg =
            if windowIsDefined then window.alert msg
        static member document =
            if windowIsDefined then window.document else null
        static member location =
            if windowIsDefined then window.location else null
        static member addEventListener(typ,listener) =
            if windowIsDefined then window.addEventListener(typ,listener)
        static member getComputedStyle(elt) =
            if windowIsDefined then window.getComputedStyle(elt) else null
        static member getComputedStyle(elt,pseudoElt) =
            if windowIsDefined then window.getComputedStyle(elt,pseudoElt) else null
        static member matchMedia query =
            if windowIsDefined then window.matchMedia query else null
        static member removeEventListener(typ,listener) =
            if windowIsDefined then window.removeEventListener(typ,listener)
        static member requestAnimationFrame callback =
            if windowIsDefined then window.requestAnimationFrame callback else 0.0

