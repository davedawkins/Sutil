module StoreTest

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.DOM

describe "Sutil.Store" <| fun () ->

    it "Immediate initialisation upon subscription" <| fun () -> promise {
        let s = Store.make 42
        let mutable v = 0
        let u = s |> Store.subscribe (fun v' -> v <- v')
        u.Dispose()
        Expect.areEqual(v,42)
    }

    it "Store updates when no subscribers" <| fun () -> promise {
        let s = Store.make 42
        Store.set s 43

        let mutable v = 0
        let u = s |> Store.subscribe (fun v' -> v <- v')
        u.Dispose()
        Expect.areEqual(v,43)
    }

    it "Update store notifies subscriber" <| fun () -> promise {
        let s = Store.make 42
        let mutable v = 0
        let u = s |> Store.subscribe (fun v' -> v <- v')
        Expect.areEqual(v,42)

        Store.set s 43

        u.Dispose()
        Expect.areEqual(v,43)
    }

    it "Multiple subscribers initialize" <| fun () -> promise {
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = s |> Store.subscribe (fun v -> v1 <- v)
        let u2 = s |> Store.subscribe (fun v -> v2 <- v)

        Expect.areEqual(v1,42)
        Expect.areEqual(v2,42)
    }

    it "Multiple subscribers update" <| fun () -> promise {
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = s |> Store.subscribe (fun v -> v1 <- v)
        let u2 = s |> Store.subscribe (fun v -> v2 <- v)
        Expect.areEqual(v1,42)
        Expect.areEqual(v2,42)

        Store.set s 43

        Expect.areEqual(v1,43)
        Expect.areEqual(v2,43)
    }

    it "Dispose terminates subscription" <| fun () -> promise {
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u1 = s |> Store.subscribe (fun v -> v1 <- v)
        let u2 = s |> Store.subscribe (fun v -> v2 <- v)

        Expect.areEqual(v1,42)
        Expect.areEqual(v2,42)

        u1.Dispose();

        Store.set s 43

        Expect.areEqual(v1,42)
        Expect.areEqual(v2,43)
    }

    it "No dispose on number of subscribers becoming 0" <| fun () -> promise {
        let s = Store.make 42

        let mutable v1 = 0
        let mutable v2 = 0

        let u = s |> Store.subscribe (fun v -> v1 <- v)

        // Number of subscribers now 0 again
        u.Dispose()

        let u = s |> Store.subscribe (fun v -> v2 <- v)
        u |> ignore
        Store.set s 43

        Expect.areEqual(v1,42)
        Expect.areEqual(v2,43)
    }

    it "Notify all updates even when equal" <| fun () -> promise {
        let inputValue = 42

        let s = Store.make inputValue
        let mutable n = 0

        // Will set n = 1 (immediate update)
        let u = s |> Store.subscribe (fun _ -> n <- n + 1)

        Store.set s inputValue  // n := 2
        Store.set s inputValue  // n := 3

        u.Dispose()

        Expect.areEqual(n,3)
    }

let init() = ()
