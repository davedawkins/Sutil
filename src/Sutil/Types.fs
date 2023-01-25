namespace Sutil

open System

// /// <exclude/>
// module CssRules =
//     type CssSelector =
//         | Tag of string
//         | Cls of string
//         | Id of string
//         | All of CssSelector list
//         | Any of CssSelector list
//         | Attr of CssSelector * string * string
//         | NotImplemented
//         member this.Match(el: Browser.Types.HTMLElement) =
//             match this with
//             | NotImplemented -> false
//             | Tag tag -> el.tagName = tag
//             | Cls cls -> el.classList.contains (cls)
//             | Id id -> el.id = id
//             | Attr (sub, name, value) -> sub.Match(el) && el.getAttribute (name) = value
//             | All rules ->
//                 rules
//                 |> List.fold (fun a r -> a && r.Match el) true
//             | Any rules ->
//                 rules
//                 |> List.fold (fun a r -> a || r.Match el) false


/// <exclude/>
type StyleRule =
    { SelectorSpec: string
      //Selector: CssRules.CssSelector
      Style: (string * obj) list }

/// <exclude/>
type KeyFrame =
    { StartAt: int
      Style: (string * obj) list }

/// <exclude/>
type KeyFrames = { Name: string; Frames: KeyFrame list }

/// <exclude/>
type MediaRule =
    { Condition: string
      Rules: StyleSheetDefinition list }

/// <exclude/>
and StyleSheetDefinition =
    | Rule of StyleRule
    | KeyFrames of KeyFrames
    | MediaRule of MediaRule

/// <exclude/>
type StyleSheetDefinitions = StyleSheetDefinition list

/// <exclude/>
type NamedStyleSheet =
    { Name: string
      StyleSheet: StyleSheetDefinitions }

/// <exclude/>
type internal ICollectionWrapper<'T> =
    abstract member ToList : unit -> List<'T>
    abstract member ToArray : unit -> 'T array
    abstract member Length : int
    abstract member Mapi : (int -> 'T -> 'R) -> ICollectionWrapper<'R>
    abstract member Map : ('T -> 'R) -> ICollectionWrapper<'R>
    abstract member Exists : ('T -> bool) -> bool
    abstract member TryFind : ('T -> bool) -> 'T option
    inherit System.Collections.Generic.IEnumerable<'T>

///  <exclude />
[<AutoOpen>]
module CollectionWrapperExt =

    type private ListW<'T>(list: 'T list) =
        interface ICollectionWrapper<'T> with
            member _.ToList() = list
            member _.ToArray() = list |> Array.ofList
            member _.Length = list.Length
            member _.Mapi(f: (int -> 'T -> 'R)) = upcast ListW((list |> List.mapi f))
            member _.Map(f: ('T -> 'R)) = upcast ListW(list |> List.map f)
            member _.Exists(p: 'T -> bool) = list |> List.exists p
            member _.TryFind(p: 'T -> bool) = list |> List.tryFind p

        interface System.Collections.IEnumerable with
            member _.GetEnumerator() =
                upcast (list |> Seq.ofList).GetEnumerator()

        interface System.Collections.Generic.IEnumerable<'T> with
            member _.GetEnumerator() =
                upcast (list |> Seq.ofList).GetEnumerator()

    type private ArrayW<'T>(a: 'T array) =
        interface ICollectionWrapper<'T> with
            member _.ToList() = a |> List.ofArray
            member _.ToArray() = a
            member _.Length = a.Length
            member _.Mapi(f: (int -> 'T -> 'R)) = upcast ArrayW((a |> Array.mapi f))
            member _.Map(f: ('T -> 'R)) = upcast ArrayW(a |> Array.map f)
            member _.Exists(p: 'T -> bool) = a |> Array.exists p
            member _.TryFind(p: 'T -> bool) = a |> Array.tryFind p

        interface System.Collections.IEnumerable with
            member _.GetEnumerator() =
                upcast (a |> Seq.ofArray).GetEnumerator()

        interface System.Collections.Generic.IEnumerable<'T> with
            member _.GetEnumerator() =
                upcast (a |> Seq.ofArray).GetEnumerator()

    type List<'T> with
        member internal __.ToCollectionWrapper() : ICollectionWrapper<'T> = upcast ListW(__)

    type 'T ``[]`` with
        member internal __.ToCollectionWrapper() : ICollectionWrapper<'T> = upcast ArrayW(__)

///  <exclude />
module internal CollectionWrapper =
    let length (c: ICollectionWrapper<'T>) = c.Length
    let mapi (f: (int -> 'T -> 'R)) (c: ICollectionWrapper<'T>) = c.Mapi f
    let map (f: ('T -> 'R)) (c: ICollectionWrapper<'T>) = c.Map f
    let exists f (c: ICollectionWrapper<'T>) = c.Exists(f)
    let tryFind f (c: ICollectionWrapper<'T>) = c.TryFind(f)

///  <exclude />
type IStoreDebugger =
    interface
        abstract Value : obj
        abstract NumSubscribers : int
    end


///  <exclude />
module DevToolsControl =

    type SutilOptions =
        { SlowAnimations: bool
          LoggingEnabled: bool }

    let mutable Options =
        { SlowAnimations = false
          LoggingEnabled = false }

    type Version =
        { Major: int
          Minor: int
          Patch: int }
        override v.ToString() = $"{v.Major}.{v.Minor}.{v.Patch}"

    type IMountPoint =
        interface
            abstract Id : string
            abstract Remount : unit -> unit
        end

    type IControlBlock =
        interface
            abstract ControlBlockVersion : int
            abstract Version : Version
            abstract GetOptions : unit -> SutilOptions
            abstract SetOptions : SutilOptions -> unit
            abstract GetStores : unit -> int array
            abstract GetStoreById : int -> IStoreDebugger
            abstract GetLogCategories : unit -> (string * bool) array
            abstract SetLogCategories : (string * bool) array -> unit
            abstract GetMountPoints : unit -> IMountPoint array
            abstract PrettyPrint : int -> unit
        end

    let getControlBlock doc : IControlBlock = Interop.get doc "__sutil_cb"
    let setControlBlock doc (cb: IControlBlock) = Interop.set doc "__sutil_cb" cb

    let initialise doc controlBlock = setControlBlock doc controlBlock

///  <exclude />
type Unsubscribe = unit -> unit

///  <exclude />
type IStore<'T> =
    interface
        inherit IObservable<'T>
        inherit IDisposable
        abstract Update : f: ('T -> 'T) -> unit
        abstract Value : 'T
        abstract Debugger : IStoreDebugger
        abstract Name : string with get, set
    end

///  <exclude />
type Store<'T> = IStore<'T>

///  <exclude />
type Update<'Model> = ('Model -> 'Model) -> unit // A store updater. Store updates by being passed a model updater

///  <exclude />
type Dispatch<'Msg> = 'Msg -> unit // Message dispatcher

///  <exclude />
type Cmd<'Msg> = (Dispatch<'Msg> -> unit) list // List of commands. A command needs a dispatcher to execute

//
// All Cmd code take from Fable.Elmish/src/cmd.fs, by Maxel Mangime
// TODO: Refactor this into Sutil.Elmish module
//
#if FABLE_COMPILER
/// <exclude/>
module internal Timer =
    open System.Timers

    let delay interval callback =
        let t =
            new Timer(float interval, AutoReset = false)

        t.Elapsed.Add callback
        t.Enabled <- true
        t.Start()
#endif

/// <summary>
/// Sutil's Elmish Cmd
/// </summary>
module Cmd =

    let none: Cmd<'Msg> = []

    let map (f: 'MsgA -> 'MsgB) (cmd: Cmd<'MsgA>) : Cmd<'MsgB> =
        cmd
        |> List.map (fun g -> (fun dispatch -> f >> dispatch) >> g)

    let ofMsg msg : Cmd<'Msg> = [ fun d -> d msg ]

    let batch (cmds: Cmd<'Msg> list) : Cmd<'Msg> = cmds |> List.concat

    module OfFunc =
        let either (task: 'args -> _) (a: 'args) (success: _ -> 'msg') (error: _ -> 'msg') =
            [ fun d ->
                  try
                      task a |> (success >> d)
                  with
                  | x -> x |> (error >> d) ]

        let perform (task: 'args -> _) (a: 'args) (success: _ -> 'msg') =
            [ fun d ->
                  try
                      task a |> (success >> d)
                  with
                  | _ -> () ]

        let attempt (task: 'args -> unit) (a: 'args) (error: _ -> 'msg') =
            [ fun d ->
                  try
                      task a
                  with
                  | x -> x |> (error >> d) ]

    module OfAsyncWith =
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let either
            (start: Async<unit> -> unit)
            (task: 'a -> Async<_>)
            (arg: 'a)
            (ofSuccess: _ -> 'msg)
            (ofError: _ -> 'msg)
            : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch

                    dispatch (
                        match r with
                        | Choice1Of2 x -> ofSuccess x
                        | Choice2Of2 x -> ofError x
                    )
                }

            [ bind >> start ]

        /// Command that will evaluate an async block and map the success
        let perform (start: Async<unit> -> unit) (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch

                    match r with
                    | Choice1Of2 x -> dispatch (ofSuccess x)
                    | _ -> ()
                }

            [ bind >> start ]

        /// Command that will evaluate an async block and map the error (of exception)
        let attempt (start: Async<unit> -> unit) (task: 'a -> Async<_>) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch

                    match r with
                    | Choice2Of2 x -> dispatch (ofError x)
                    | _ -> ()
                }

            [ bind >> start ]

        /// Command that will evaluate an async block to the message
        let result (start: Async<unit> -> unit) (task: Async<'msg>) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task
                    dispatch r
                }

            [ bind >> start ]

    module OfAsync =
#if FABLE_COMPILER
        let start x =
            Timer.delay 0 (fun _ -> Async.StartImmediate x)
#else
        let inline start x = Async.Start x
#endif
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let inline either (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.either start task arg ofSuccess ofError

        /// Command that will evaluate an async block and map the success
        let inline perform (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.perform start task arg ofSuccess

        /// Command that will evaluate an async block and map the error (of exception)
        let inline attempt (task: 'a -> Async<_>) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.attempt start task arg ofError

        /// Command that will evaluate an async block to the message
        let inline result (task: Async<'msg>) : Cmd<'msg> = OfAsyncWith.result start task

    module OfAsyncImmediate =
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let inline either (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.either Async.StartImmediate task arg ofSuccess ofError

        /// Command that will evaluate an async block and map the success
        let inline perform (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.perform Async.StartImmediate task arg ofSuccess

        /// Command that will evaluate an async block and map the error (of exception)
        let inline attempt (task: 'a -> Async<_>) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.attempt Async.StartImmediate task arg ofError

        /// Command that will evaluate an async block to the message
        let inline result (task: Async<'msg>) : Cmd<'msg> =
            OfAsyncWith.result Async.StartImmediate task

#if FABLE_COMPILER
    module OfPromise =
        /// Command to call `promise` block and map the results
        let either
            (task: 'a -> Fable.Core.JS.Promise<_>)
            (arg: 'a)
            (ofSuccess: _ -> 'msg)
            (ofError: #exn -> 'msg)
            : Cmd<'msg> =
            let bind dispatch =
                (task arg)
                    .``then``(ofSuccess >> dispatch)
                    .catch (unbox >> ofError >> dispatch)
                |> ignore

            [ bind ]

        /// Command to call `promise` block and map the success
        let perform (task: 'a -> Fable.Core.JS.Promise<_>) (arg: 'a) (ofSuccess: _ -> 'msg) =
            let bind dispatch =
                (task arg).``then`` (ofSuccess >> dispatch)
                |> ignore

            [ bind ]

        /// Command to call `promise` block and map the error
        let attempt (task: 'a -> Fable.Core.JS.Promise<_>) (arg: 'a) (ofError: #exn -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                (task arg).catch (unbox >> ofError >> dispatch)
                |> ignore

            [ bind ]

        /// Command to dispatch the `promise` result
        let result (task: Fable.Core.JS.Promise<'msg>) =
            let bind dispatch = task.``then`` dispatch |> ignore
            [ bind ]

    [<Obsolete("Use `OfPromise.either` instead")>]
    let inline ofPromise
        (task: 'a -> Fable.Core.JS.Promise<_>)
        (arg: 'a)
        (ofSuccess: _ -> 'msg)
        (ofError: _ -> 'msg)
        : Cmd<'msg> =
        OfPromise.either task arg ofSuccess ofError
#else
    open System.Threading.Tasks

    module OfTask =
        /// Command to call a task and map the results
        let inline either (task: 'a -> Task<_>) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsync.either (task >> Async.AwaitTask) arg ofSuccess ofError

        /// Command to call a task and map the success
        let inline perform (task: 'a -> Task<_>) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsync.perform (task >> Async.AwaitTask) arg ofSuccess

        /// Command to call a task and map the error
        let inline attempt (task: 'a -> Task<_>) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsync.attempt (task >> Async.AwaitTask) arg ofError

        /// Command and map the task success
        let inline result (task: Task<'msg>) : Cmd<'msg> =
            OfAsync.result (task |> Async.AwaitTask)

    [<Obsolete("Use OfTask.either instead")>]
    let inline ofTask (task: 'a -> Task<_>) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
        OfTask.either task arg ofSuccess ofError
#endif
