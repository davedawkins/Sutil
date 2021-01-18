module Test.Store

open Sveltish
open Util
open Fable.Mocha

let tests = testList "Sveltish.Store" [

    testCase "Immediate initialisation upon subscription" <| fun () ->
        let s = Store.make 42
        let mutable v = 0
        let u = Store.subscribe s <| fun v' -> v <- v'
        u.Dispose()
        Expect.areEqual v 42

    testCase "Store updates when no subscribers" <| fun () ->
        let s = Store.make 42
        Store.set s 43

        let mutable v = 0
        let u = Store.subscribe s <| fun v' -> v <- v'
        u.Dispose()
        Expect.areEqual v 43

    testCase "Update store notifies subscriber" <| fun () ->
        let s = Store.make 42
        let mutable v = 0
        let u = Store.subscribe s <| fun v' -> v <- v'
        Expect.areEqual v 42

        Store.set s 43

        u.Dispose()
        Expect.areEqual v 43

    testCase "Multiple subscribers initialize" <| fun () ->
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = Store.subscribe s <| fun v -> v1 <- v
        let u2 = Store.subscribe s <| fun v -> v2 <- v

        Expect.areEqual v1 42
        Expect.areEqual v2 42

    testCase "Multiple subscribers update" <| fun () ->
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = Store.subscribe s <| fun v -> v1 <- v
        let u2 = Store.subscribe s <| fun v -> v2 <- v

        Expect.areEqual v1 42
        Expect.areEqual v2 42

        Store.set s 43

        Expect.areEqual v1 43
        Expect.areEqual v2 43

    testCase "Dispose terminates subscription" <| fun () ->
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = Store.subscribe s <| fun v -> v1 <- v
        let u2 = Store.subscribe s <| fun v -> v2 <- v

        Expect.areEqual v1 42
        Expect.areEqual v2 42

        u1.Dispose();

        Store.set s 43

        Expect.areEqual v1 42
        Expect.areEqual v2 43


    testCase "No dispose on number of subscribers becoming 0" <| fun () ->
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u = Store.subscribe s <| fun v -> v1 <- v

        // Number of subscribers now 0 again
        u.Dispose()

        let u = Store.subscribe s <| fun v -> v2 <- v
        u |> ignore
        Store.set s 43

        Expect.areEqual v1 42
        Expect.areEqual v2 43

    testCase "Notify all updates even when equal" <| fun () ->
        let inputValue = 42

        let s = Store.make inputValue
        let mutable n = 0

        // Will set n = 1 (immediate update)
        let u = Store.subscribe s <| fun _ -> n <- n + 1

        Store.set s inputValue  // n := 2
        Store.set s inputValue  // n := 3

        u.Dispose()

        Expect.areEqual n 3

]
