module Util

open System
open Fable.Mocha


let expectListEqual (expected:List<'T>) (value: List<'T>) =
    Expect.areEqual expected.Length value.Length
    for p in List.zip expected value do
        Expect.areEqual (fst p) (snd p)
