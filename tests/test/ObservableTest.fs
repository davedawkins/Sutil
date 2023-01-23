module ObservableTest

open Describe

#if HEADLESS
open WebTestRunner
#endif

open Sutil
open Sutil.Core
open Sutil.CoreElements

describe "Sutil.Observable" <| fun () ->

    it "distinctUntilChanged" <| fun () -> promise {
//        let s1 = Store.make 0
        let s2 = Store.make 0
        let mutable n1 = 0
        let mutable n2 = 0

//        s1 |> Store.subscribe (fun _ -> n1 <- n1 + 1) |> ignore
        s2 |> Observable.distinctUntilChanged |> Store.subscribe (fun _ -> n2 <- n2 + 1) |> ignore

//        [ 1; 1; 1; 2; 2; 2 ] |> List.iter (Store.set s1)
        [ 1; 1; 1; 2; 2; 2 ] |> List.iter (Store.set s2)

//        Expect.areEqual(7,n1) // 0 1 1 1 2 2 2
        Expect.areEqual(3,n2) // 0 1 2
    }

    it "distinctUntilChangedCompare" <| fun () -> promise {
  //      let s1 = Store.make '-'
        let s2 = Store.make '-'
        let mutable n1 = 0
        let mutable n2 = 0

//        s1 |> Store.subscribe (fun _ -> n1 <- n1 + 1) |> ignore
        s2 |> Observable.distinctUntilChangedCompare (fun a b -> System.Char.ToUpper a = System.Char.ToUpper b)
           |> Store.subscribe (fun _ -> n2 <- n2 + 1) |> ignore

        let data = [ 'a'; 'A'; 'a'; 'b'; 'B'; 'b' ]

//        data |> List.iter (Store.set s1)
        data |> List.iter (Store.set s2)

//        Expect.areEqual(7,n1) // - a A a b B b
        Expect.areEqual(3,n2) // - a b
    }

    //it "exists" <| fun () ->
    //    let s1 = Store.make '-'
    //    let s2 =
(*
    it "choose" <| fun () -> promise {
        let s1 = Store.make '-'
        let s2 = Store.make '-'
        let mutable n1 = 0
        let mutable n2 = 0

        s1 |> Store.subscribe (fun _ -> n1 <- n1 + 1) |> ignore
        s2 |> Observable.choose (fun x -> Some x)
            |> Store.subscribe (fun _ -> n2 <- n2 + 1) |> ignore

        let data = [ 'a'; 'A'; 'a'; 'b'; 'B'; 'b' ]

        data |> List.iter (Store.set s1)
        data |> List.iter (Store.set s2)

        Expect.areEqual 7 n1 // - a A a b B b
        Expect.areEqual 3 n2 // - a b
     }*)

let init() = ()
