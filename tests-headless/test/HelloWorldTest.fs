module HelloWorldTest

open Fable.Core
open Fable.Core.JsInterop
open Expect
open Expect.Dom
open WebTestRunner
open Browser
open Browser.Types

open Sutil

describe "Hello World" <| fun () ->
    it "can say hello" <| fun () -> promise {
        use container = Container.New()

        let mutable sideEffect = 0

        let hello = Html.div "Hello World"

        Sutil.DOM.mountOn hello container.El |> ignore

        let div = container.El.getSelector("div")

        Expect.Dom.Expect.innerText "Hello World" div
    }

