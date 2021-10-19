module Describe
open Fable.Core
open System
open Browser.Dom
open Browser.Types

let mutable currentEl : HTMLElement = Unchecked.defaultof<_>

#if HEADLESS

open Expect
open Expect.Dom
open WebTestRunner

let mountTestApp app =
    use container = Container.New()
    currentEl <- container.El
    Sutil.DOM.mountOn app currentEl |> ignore

#else

open BrowserFramework

[<AllowNullLiteral>]
type Context(name:string) =
    let mutable groups : Context list = []
    let mutable tests : TestCase list = []

    member _.Add( group : Context ) =
        groups <- groups @ [ group ]

    member _.Tests = tests
    member _.Name = name
    member _.Groups = groups

    member _.AddTest( test : TestCase ) =
        tests <- tests @ [ test ]

    member this.Build(level : int) : unit =
        let rec suites (name : string) (ctx:Context) =
            seq {
                yield testList name ctx.Tests
                for g in ctx.Groups do
                    yield! suites (ctx.Name + "/" + g.Name) g
            }
        (suites this.Name this) |> Seq.iter addSuite

[<AutoOpen>]
module Mocha =
    let mutable currentContext : Context = null

    let describe(name : string) (fn : unit -> unit ) =
        let child = Context(name)
        let save = currentContext
        currentContext <- child
        fn()
        currentContext <- save
        if isNull currentContext then
            child.Build(0)
        else
            currentContext.Add(child)

    let it(name : string) (fn : unit -> JS.Promise<unit> ) : unit =
        currentContext.AddTest( testCaseP name fn )

[<Literal>]
let TestAppId = "test-app"

let mountTestApp app =
    let host = document.querySelector("#" + TestAppId) :?> HTMLElement
    host.innerHTML <- ""
    let result = Sutil.DOM.mountOn app host
    currentEl <- host

#endif

type Expect =
    static member assertTrue (cond:bool) (message:string) =
        if not cond then
            failwith message

    static member areEqual<'T when 'T:equality>(actual : 'T, expected: 'T, message : string ) =
        Expect.assertTrue (expected=actual) $"{message}: areEqual: expected: '{expected}' actual: '{actual}'"

    static member areEqual<'T when 'T:equality>(actual : 'T, expected: 'T) =
        Expect.areEqual(actual, expected, "")

    static member notNull (actual:obj) = Expect.assertTrue (not(isNull actual)) "notNull: actual: '{obj}'"

    static member queryText (query:string) (expected:string) =
        let el = currentEl.querySelector(":scope " + query) :?> HTMLElement
        Expect.assertTrue (not(isNull el)) ("queryText: Query failed: " + query)
        Expect.areEqual(el.innerText,expected, "queryText")

    static member queryNumChildren (query:string) (expected:int) =
        let el = currentEl.querySelector(":scope " + query) :?> HTMLElement
        Expect.assertTrue (not(isNull el)) ("queryText: Query failed: " + query)
        Expect.areEqual(el.children.length,expected,"queryNumChildren")

let expectListEqual (expected:List<'T>) (value: List<'T>) =
    Expect.areEqual(expected.Length,value.Length,"List lengths")
    for p in List.zip expected value do
        Expect.areEqual((fst p),(snd p),"List elements equal")
