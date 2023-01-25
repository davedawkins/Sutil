module HelloWorldTest

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil

describe "Hello World" <| fun () ->
    it "says hello" <| fun () -> promise {
        mountTestApp <| Html.div "Hello World"
        Expect.queryText "div" "Hello World"
    }

let init() =
    System.Console.WriteLine("HelloWorld init")
