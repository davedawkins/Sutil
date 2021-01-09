module Program

open Fable.Mocha
open System

Mocha.runTests  [
        Test.DOM.tests
        ]
