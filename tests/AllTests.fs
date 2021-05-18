module Program

//open Fable.Mocha
open TestFramework
open System

Browser.Dom.console.log("Running tests")

let main() = runTests [
        Test.Store.tests
        Test.Observable.tests
        Test.DOM.tests
        ]


main()
//open Fable.Core.JsInterop
//Browser.Dom.window?main <- main