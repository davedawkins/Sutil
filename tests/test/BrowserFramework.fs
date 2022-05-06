module BrowserFramework

open System
open Browser.Dom
open Browser.Types
open Fable.Core


let testAppId = "test-app"
let mutable currentEl : HTMLElement = Unchecked.defaultof<_>

type TestCase = {
    Name : string
    Test : unit -> JS.Promise<unit>
}

type TestSuite = {
    Name : string
    Suites : TestSuite list
    Tests : TestCase list
}

let logC (s: string) = Browser.Dom.console.log(s)

let log (category:string) (s : string) =
    logC(s)
    let logE = document.querySelector("#test-log")
    if not(isNull logE) then
        let span = document.createElement("span")
        span.classList.add([|category|])
        let t = document.createTextNode(s + "\n")
        span.appendChild(t) |> ignore
        logE.appendChild(span) |> ignore

let timeNow() = (float)System.DateTime.Now.Ticks / 10000000.0

let timestamp (t : float) =
    sprintf "%0.3f" t

let logS s = log "success" s
let logE s = log "failure" s
let logH s = log "heading" s
let logI s = log "info" s

let private rafu f =
    window.requestAnimationFrame (fun _ -> f()) |> ignore

let waitAnimationFrame () : JS.Promise<unit> =
    Fable.Core.JS.Constructors.Promise.Create <|
        fun accept _ -> rafu accept

let mountTestApp app =
    let el = document.querySelector("#" + testAppId)
    el.innerHTML <- ""
    Sutil.Program.mountElement testAppId app

let testCase (name:string) (f:unit -> unit) =
    {
        Name = name;
        Test = fun () ->
            promise {
                f()
            }
    }

let testCaseP (name:string) (f:unit -> JS.Promise<unit>) =
    {
        Name = name;
        Test = f
    }

let testList (name:string) (tests : TestCase list) = { Name = name; Tests = tests; Suites = [] }

type TestContext = {
    StartTime : float
    NumFail : int
    NumPass : int
    TestSuites : TestSuite list
    TestCases : TestCase list
}

let rec nextTestCase (testCtx : TestContext) =
    rafu (fun _ -> nextTestCaseNow testCtx) // Allow browser to update between tests
    //nextTestCaseNow testCtx // Allow browser to update between tests

and nextTestCaseNow (testCtx : TestContext) =
    match testCtx.TestCases with
    | [] ->
        nextTestSuite testCtx
    | test :: tests ->
        test.Test()
            |> Promise.map (fun _ ->
                logS ($"{timestamp(timeNow() - testCtx.StartTime)}: " + test.Name + ": PASS")
                nextTestCase { testCtx with TestCases = tests; NumPass = testCtx.NumPass + 1 }
            )
            |> Promise.catch (fun x ->
                logE ($"{timestamp(timeNow() - testCtx.StartTime)}: " + test.Name + ": FAIL: "+ x.Message)
                nextTestCase { testCtx with TestCases = tests; NumFail = testCtx.NumFail + 1 }
            )
            |> ignore

and nextTestSuite (testCtx : TestContext) =
    match testCtx.TestSuites with
    | [] ->
        logH "Result"
        if testCtx.NumFail = 0 then
            logS ($"{timestamp(timeNow() - testCtx.StartTime)}: SUCCESS: All tests passed (" + string testCtx.NumPass + ")")
        else
            logE ($"{timestamp(timeNow() - testCtx.StartTime)}: FAIL: " + string testCtx.NumFail + " failed, " + string testCtx.NumPass + " passed")
    | suite :: suites ->
        logH (suite.Name)
        nextTestCase { testCtx with TestCases = suite.Tests; TestSuites = suites }

let mutable testSuites : TestSuite list = []

let addSuite suite =
    testSuites <- testSuites @ [ suite ]

let runTests (tests : TestSuite list) =
    logH $"Test Suite"
    logI $"{timestamp(0.)}: Starting test"
    nextTestSuite { StartTime = timeNow(); NumPass = 0; NumFail = 0; TestSuites = tests; TestCases = [] }

let runAll() =
    runTests testSuites
