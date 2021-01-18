module Test.DOM

open Util
open Fable.Mocha

let tests = testList "Sveltish.DOM" [

    testCase "Dummy test" <| fun () ->
        Expect.areEqual 1 1

]
