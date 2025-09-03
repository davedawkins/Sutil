namespace Sutil

open System

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
type ICollectionWrapper<'T> =
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
type IReadOnlyStore<'T> =
    inherit IObservable<'T>
    inherit IDisposable
    abstract Value : 'T
    abstract OnDispose: (unit -> unit) -> unit

type IStore<'T> =
    inherit IReadOnlyStore<'T>
    abstract Update : f: ('T -> 'T) -> unit
    abstract Debugger : IStoreDebugger
    abstract Name : string with get, set
    abstract Id : int with get
    
///  <exclude />
type Store<'T> = IStore<'T>

///  <exclude />
type Update<'Model> = ('Model -> 'Model) -> unit // A store updater. Store updates by being passed a model updater

type ElementRef =
    | Id of string
    | Selector of string
    | Element of Browser.Types.HTMLElement
with
    member __.AsElement =
        match __ with
        | Id s -> Browser.Dom.document.querySelector("#" + s) :?> Browser.Types.HTMLElement
        | Selector s -> Browser.Dom.document.querySelector(s) :?> Browser.Types.HTMLElement
        | Element e -> e

type internal MountOp =
    | AppendTo
    | InsertAfter

type internal MountPoint = MountOp * ElementRef


type TransitionProp =
    | Key of string
    | X of float
    | Y of float
    | Opacity of float
    | Delay of float
    | Duration of float
    | DurationFn of (float -> float)
    | Ease of (float -> float)
    | CssGen of (float -> float -> string )
    | Tick of (float -> float -> unit)
    | Speed of float
    | Fallback of TransitionBuilder

and Transition =
    {
        Key : string
        X : float
        Y : float
        Opacity : float
        Delay : float
        Duration : float
        DurationFn : (float->float) option
        Speed : float
        Ease : (float -> float)
        CssGen : (float -> float -> string ) option
        Tick: (float -> float -> unit) option
        Fallback: TransitionBuilder option
    } with
            static member Default = {
                Key =""
                X = 0.0
                Y = 0.0
                Delay = 0.0
                Opacity = 0.0
                Duration = 0.0
                DurationFn = None
                Speed = 0.0
                Ease = Easing.linear
                CssGen = None
                Fallback = None
                Tick = None }

and CreateTransition =
     unit -> Transition

and TransitionBuilder = TransitionProp list -> Browser.Types.HTMLElement -> CreateTransition

type Animation = {
    From:  Browser.Types.ClientRect
    To:  Browser.Types.ClientRect
}

type AnimationBuilder = TransitionProp list ->  Browser.Types.HTMLElement -> Animation -> Transition

type TransitionAttribute =
    | InOut of TransitionBuilder
    | In of TransitionBuilder
    | Out of TransitionBuilder
    | Animate of AnimationBuilder

type EachOptions = 
    {
        Transition : TransitionAttribute list
        PreRender : unit -> unit
        PostRender : unit -> unit
    }
    static member Default = { Transition = []; PreRender = ignore; PostRender = ignore }
    static member From( trans : TransitionAttribute list ) = { EachOptions.Default with Transition = trans }
