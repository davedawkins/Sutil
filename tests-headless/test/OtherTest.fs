module OtherTest

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil

describe "Other" <| fun () ->
    it "plays nicely" <| fun () -> promise {
        mountTestApp <| Html.div "I play nicely"

        Expect.queryText "div" "I play nicely"
    }

