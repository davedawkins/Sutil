module Test.DOM

open Util
open Fable.Mocha

let tests = testList "Sveltish.DOM" [

    testCase "Create a drawing" <| fun () ->
        Expect.areEqual 1 1

]
