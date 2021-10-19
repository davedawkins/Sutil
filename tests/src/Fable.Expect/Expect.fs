namespace Expect

open Fable.Core

module private ExpectUtil =
    [<Emit("throw $0", isStatement=true)>]
    let throw (error: obj): 'T = jsNative

    let quote (v: obj) =
        match v with
        | :? string as s -> box("\"" + s + "\"")
        | _ -> v

open ExpectUtil

// Because of performance issues, custom exceptions in Fable
// don't inherit JS Error. So we create our own binding here.
[<Global("Error")>]
type JsError(message: string) =
    member _.stack with get(): string = jsNative and set(v: string): unit = jsNative

[<AttachMembers>]
type AssertionError<'T>(assertion: string, ?description: string, ?actual: 'T, ?expected: 'T, ?brief: bool, ?extra: string) =
    inherit JsError(
        let brief = defaultArg brief false
        [
            "Expected " |> Some
            description |> Option.map (fun v -> $"'{v}' ")
            if not brief then actual |> Option.map (fun v -> $"{quote v} ")
            $"to {assertion}" |> Some
            if not brief then expected |> Option.map (fun v -> $" {quote v} ")
            extra |> Option.map (fun v -> $". {v}")
        ] |> List.choose id |> String.concat ""
    )
    // Hide stack for cleaner reports as the test name is already shown
    // If we use an empty string the test runner shows the message twice (?)
    do base.stack <- "<stack hidden>"
    // Test runner requires these properties to be settable, not sure why
    member val actual = actual with get, set
    member val expected = expected with get, set

type AssertionError =
    static member Throw(assertion: string, ?description, ?actual: 'T, ?expected: 'T, ?brief: bool, ?extra) =
        AssertionError(assertion, ?description=description, ?actual=actual, ?expected=expected, ?brief=brief, ?extra=extra) |> throw

// TODO: String and collection assertions
[<RequireQualifiedAccess>]
module Expect =
    let equal expected actual =
        if not(actual = expected) then
            AssertionError.Throw("equal", actual=actual, expected=expected)

    let notEqual expected actual =
        if not(actual <> expected) then
            AssertionError.Throw("not equal", actual=actual, expected=expected)

    let greaterThan expected actual =
        if not(actual > expected) then
            AssertionError.Throw("be greater than", actual=actual, expected=expected)

    let lessThan expected actual =
        if not(actual < expected) then
            AssertionError.Throw("be less than", actual=actual, expected=expected)

    let greaterOrEqual expected actual =
        if not(actual >= expected) then
            AssertionError.Throw("be greater than or equal to", actual=actual, expected=expected)

    let lessOrEqual expected actual =
        if not(actual <= expected) then
            AssertionError.Throw("be less than or equal to", actual=actual, expected=expected)

    let betweenInclusive lowerBound upperBound actual =
        if not(lowerBound <= actual && actual <= upperBound) then
            AssertionError.Throw($"be between inclusive {lowerBound} and {upperBound}", actual=actual)

    let betweenExclusive lowerBound upperBound actual =
        if not(lowerBound < actual && actual < upperBound) then
            AssertionError.Throw($"be between exclusive {lowerBound} and {upperBound}", actual=actual)

    let isTrue (msg: string) (condition: 'T -> bool) (argument: 'T) =
        if not(condition argument) then
            AssertionError.Throw("be true", description=msg)

    let isFalse (msg: string) (condition: 'T -> bool) (argument: 'T) =
        if condition argument then
            AssertionError.Throw("be false", description=msg)

    let map (f: 'T1 -> 'T2) (x: 'T1): 'T2 =
        f x

    let some (msg: string) (x: 'T option): 'T =
        match x with
        | Some x -> x
        | None -> AssertionError.Throw("be some", description=msg)

    let find (msg: string) (condition: 'T -> bool) (items: 'T seq) =
        items
        |> Seq.tryFind condition
        |> function
            | Some x -> x
            | None -> AssertionError.Throw("be found", description=msg)

    let errorAnd (msg: string) (f: 'T -> 'Result) (argument: 'T): exn =
        let e =
            try
                let _ = f argument
                None
            with e ->
                Some e
        match e with
        | Some e -> e
        | None -> AssertionError.Throw("fail", description=msg)

    let error (msg: string) (f: 'T -> 'Result) (argument: 'T): unit =
        errorAnd msg f argument |> ignore

    let successAnd (msg: string) (f: 'T -> 'Result) (argument: 'T): 'Result =
        try
            f argument
        with e ->
            AssertionError.Throw("succeed", description=msg, extra="Error: " + e.Message)

    let success (msg: string) (f: 'T -> 'Result) (argument: 'T): unit =
        successAnd msg f argument |> ignore

    let beforeTimeout (msg: string) (ms: int) (pr: JS.Promise<'T>): JS.Promise<'T> =
        JS.Constructors.Promise.race [|
            pr |> Promise.map box
            Promise.sleep ms |> Promise.map (fun _ -> box "timeout")
        |]
        |> Promise.map (function
            | :? string as s when s = "timeout" ->
                AssertionError.Throw($"happen before {ms}ms timeout", description=msg)
            | v -> v :?> 'T)

    /// Retry an action every X milliseconds until timeout
    let retryUntilWith (msg: string) (intervalMs: int) (timeoutMs: int) (action: unit -> 'T) =
        promise {
            let mutable totalMs = 0
            let mutable success = false
            let mutable res = Unchecked.defaultof<'T>
            while not success do
                try
                    res <- action()
                    success <- true
                    // printfn $"Success in {totalMs}ms"
                with _ ->
                    // printfn $"Error in {totalMs}ms"
                    ()
                if not success then
                    if totalMs >= timeoutMs then
                        AssertionError.Throw($"happen before {timeoutMs}ms timeout", description=msg)
                    do! Promise.sleep intervalMs
                    totalMs <- totalMs + intervalMs
            return res
        }

    /// Retry an action every 200ms with a timeout of 2000ms
    let retryUntil (msg: string) (action: unit -> 'T) =
        retryUntilWith msg 200 2000 action
