/// <summary>
/// General purpose helpers
/// </summary>
module Sutil.Helpers

open Browser.Types

#if FABLE_COMPILER
open Fable.Core

[<Emit("$0 === $1")>]
let fastEquals (x: 'T) (y: 'T): bool = jsNative
#else
let fastEquals (x: 'T) (y: 'T): bool = Unchecked.equals x y
#endif
let fastNotEquals x y = fastEquals x y |> not

let fileListToSeq (files:FileList) : File seq =
    if not (isNull files) then
        seq {
            for i in [0 .. files.length-1] do
                yield files.[i]
        }
    else
        Seq.empty

let disposable f =
    { new System.IDisposable with
        member _.Dispose() = f () }

let internal unsubify (d : System.IDisposable) = fun () -> d.Dispose()

let makeIdGeneratorFrom start =
    let mutable id = start
    fun () ->
        let r = id
        id <- id+1
        r

let makeIdGenerator() = makeIdGeneratorFrom 0
