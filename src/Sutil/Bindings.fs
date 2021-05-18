[<AutoOpen>]
module Sutil.Bindings

(*

Grouping cases

1. Fragment

        div [
            div "Header"
            fragment [
                p "Body 1"
                p "Body 2"
            ]
            div "Footer"
        ]

        <div>
            <div>Header</div>
            <p>Body 1</p>
            <p>Body 2</p>
            <div>Footer</div>
        </div>

2. Binding to text

        div [
            div "Header"
            bind store (fun v -> text (string v))
            div "Footer"
        ]

3. Binding to element

        div [
            div "Header"
            bind store (fun v -> div (string v))
            div "Footer"
        ]

4. Binding to fragment

        div [
            div "Header"
            bind store (fun v -> fragment [
                div "Value is"
                div (string v)
            ])
            div "Footer"
        ]

5. Each of element
        div [
            div "Header"
            each items (fun item -> div (string item))
            div "Footer"
        ]

6. Each of fragment - not supported as yet - use a div wrapper

7. Each of binding
        div [
            div "Header"
            each items (fun item -> div (string item))
            div "Footer"
        ]
*)


open Transition
open DOM
open Browser.Types
open System
open Fable.Core

let private log s = Logging.log "bind" s

let private bindId = Helpers.makeIdGenerator()

// All bindings ought to either end up calling this or at least doing the same registration
let bindSub<'T> (source : IObservable<'T>) (handler : BuildContext -> 'T -> unit) = nodeFactory <| fun ctx ->
    let unsub = source.Subscribe( handler ctx )
    SutilNode.RegisterDisposable(ctx.Parent,unsub)
    unitResult(ctx,"bindSub")


let bindFragment<'T>  (store : IObservable<'T>)  (element: 'T -> SutilElement) = nodeFactory <| fun ctx ->
    let mutable node = EmptyNode
    let vnode = NodeGroup("bind",ctx.Parent,ctx.Previous)
    let bindNode = GroupNode vnode
    //vnode.Parent <- ctx.Parent

    log($"bindFragment: {vnode.Id} ctx={ctx.Action} prev={ctx.Previous}")
    ctx.AddChild bindNode

    let bindCtx = { ctx with Parent = bindNode }
    let disposable = store |> Store.subscribe (fun next ->
        try
            node <- build (element(next)) (bindCtx |> ContextHelpers.withReplace (node,vnode.NextDomNode))
        with
        | x -> Logging.error $"Exception in bind: {x.Message} parent {ctx.Parent} node {node.ToString()} node.Parent "
    )

    vnode.SetDispose (Helpers.unsubify disposable)

    sutilResult bindNode


let bindFragment2<'A,'B> (a : IObservable<'A>) (b : IObservable<'B>)  (element: ('A*'B) -> SutilElement) = nodeFactory <| fun ctx ->
    let mutable node : SutilNode = EmptyNode
    let vnode = NodeGroup("bind2",ctx.Parent,ctx.Previous)
    let bindNode = GroupNode vnode
    ctx.AddChild bindNode

    let bindCtx = { ctx with Parent = bindNode }

    let d = Store.subscribe2 a b (fun next ->
        try
            node <- build (element(next)) (bindCtx |> ContextHelpers.withReplace (node,vnode.NextDomNode))
        with
        | x -> Logging.error $"Exception in bind: {x.Message}"
    )

    vnode.SetDispose (Helpers.unsubify d)

    sutilResult bindNode

let bindPromiseStore<'T>  (p : ObservablePromise<'T>)
        (waiting : SutilElement)
        (result: 'T -> SutilElement)
        (fail : Exception -> SutilElement)
        : SutilElement =
    bindFragment p <| (function
        | Waiting -> waiting
        | Result r -> result r
        | Error x -> fail x)

let bindPromise<'T>  (p : JS.Promise<'T>)
        (waiting : SutilElement)
        (result: 'T -> SutilElement)
        (fail : Exception -> SutilElement)
        : SutilElement =
    let x = ObservablePromise<'T>()
    x.Run p
    bindPromiseStore x waiting result fail

type BindFn<'T> = IObservable<'T> -> ('T -> SutilElement) -> SutilElement
let private getInputChecked el = Interop.get el "checked"
let private setInputChecked (el : Node) (v:obj) = Interop.set el "checked" v
let private getInputValue el : string = Interop.get el "value"
let private setInputValue el (v:string) = Interop.set el "value" v

let bindSelected<'T when 'T : equality> (selection:IObservable<List<'T>>) (dispatch : List<'T> -> unit) : SutilElement = nodeFactory <| fun ctx ->

    let selectElement = ctx.ParentElement :?> HTMLSelectElement
    let selOps = selectElement.selectedOptions
    let op (coll:HTMLCollection) i = coll.[i] :?> HTMLOptionElement
    let opValue op : 'T = Interop.get op "__value"

    let getValueList() =
        [0..selOps.length-1] |> List.map (fun i -> opValue (op selOps i))

    let updateSelected (v : List<'T>) =
        let ops = selectElement.options
        for i in [0..ops.length-1] do
            let o = op ops i
            o.selected <- v |> List.contains (opValue o)

    let unsubInput = listen "input" selectElement <| fun _ ->
        getValueList() |> dispatch

    // We need to finalize checked status after all attrs have been processed for input,
    // in case 'value' hasn't been set yet
    once Event.ElementReady selectElement <| fun _ ->
        let unsub = selection |> Store.subscribe (updateSelected)
        SutilNode.RegisterDisposable(ctx.Parent, unsub)

    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubInput)

    unitResult(ctx,"bindSelected")

let bindSelectMultiple<'T when 'T : equality> (store:IStore<List<'T>>) : SutilElement =
    bindSelected store (fun sln -> store <~ sln)

let bindSelectSingle<'T when 'T : equality> (store:IStore<'T>) : SutilElement =
    bindSelected (store .> List.singleton) (fun sln -> sln |> List.exactlyOne |> Store.set store)

let bindSelectOptional<'T when 'T : equality> (store:IStore<'T option>) : SutilElement =
    let toList topt = match topt with |None -> []|Some t -> List.singleton t
    let fromList list = match list with |[] -> None |x::_ -> Some x
    bindSelected (store .> toList) (fun sln -> sln |> fromList |> Store.set store)

let private isNullString (obj:obj) =
    isNull obj || System.String.IsNullOrEmpty(downcast obj)

let private getId (s : IStore<'T>) = s.GetHashCode()

let bindGroup<'T> (store:Store<List<string>>) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let name = match Interop.get parent "name" with
                | s when isNullString s -> $"store-{getId store}"
                | s -> s

    // Group this input with all other inputs that reference the same store
    Interop.set parent "name" name

    let getValueList() =
        let inputs = (documentOf parent).querySelectorAll(@$"input[name=""{name}""]")
        [0..(inputs.length-1)] |> List.map (fun i -> inputs.[i]) |> List.filter getInputChecked |> List.map getInputValue

    let updateChecked (v : List<string>) =
        setInputChecked parent ( v |> List.contains (getInputValue parent) )

    // Update the store when the radio box is clicked on
    let unsubInput = DOM.listen "input" parent <| fun _ ->
        getValueList() |> Store.set store

    // We need to finalize checked status after all attrs have been processed for input,
    // in case 'value' hasn't been set yet
    once Event.ElementReady parent <| fun _ ->
        store |> Store.get |> updateChecked

    // When store changes make sure check status is synced
    let unsub = store |> Store.subscribe (updateChecked)

    SutilNode.RegisterDisposable(ctx.Parent,unsub)
    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubInput)

    unitResult(ctx,"bindGroup")

// T can realistically only be numeric or a string. We're relying (I think!) on JS's ability
// to turn a string into an int automatically in the Store.set call (maybe it's Fable doing that)
//
let bindRadioGroup<'T> (store:Store<'T>) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let name = match Interop.get parent "name" with
                | s when isNullString s -> $"store-{getId store}"
                | s -> s
    // Group this input with all other inputs that reference the same store
    Interop.set parent "name" name

    let updateChecked (v : obj) =
        setInputChecked parent ( (string v) = getInputValue parent )

    // Update the store when the radio box is clicked on
    let inputUnsub = listen "input" parent <| fun _ ->
        Interop.get parent "value" |> Store.set store

    // We need to finalize checked status after all attrs have been processed for input,
    // in case 'value' hasn't been set yet
    once Event.ElementReady parent <| fun _ ->
        store |> Store.get |> updateChecked

    // When store changes make sure check status is synced
    let unsub = store |> Store.subscribe updateChecked

    SutilNode.RegisterDisposable(ctx.Parent,unsub)
    SutilNode.RegisterUnsubscribe(ctx.Parent,inputUnsub)

    unitResult(ctx,"bindRadioGroup")

let bindClass (toggle:IObservable<bool>) (classes:string) =
    bindSub toggle <| fun ctx active ->
        if active then
            ctx.ParentElement |> addToClasslist classes
        else
            ctx.ParentElement |> removeFromClasslist classes

// Bind a store value to an element attribute. Updates to the element are unhandled
let bindAttrIn<'T> (attrName:string) (store : IObservable<'T>) : SutilElement = nodeFactory <| fun ctx ->
    let unsub =
        if attrName = "class" then
            store |> Store.subscribe (fun cls -> ctx.ParentElement.className <- (string cls))
        else
            store |> Store.subscribe (DOM.setAttribute ctx.ParentElement attrName)
    SutilNode.RegisterDisposable(ctx.Parent,unsub)
    unitResult(ctx,"bindAttrIn")

let bindAttrOut<'T> (attrName:string) (onchange : 'T -> unit) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = listen "input" parent <| fun _ ->
        Interop.get parent attrName |> onchange
    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubInput)
    unitResult(ctx,"bindAttrOut")

// Bind a scalar value to an element attribute. Listen for onchange events and dispatch the
// attribute's current value to the given function. This form is useful for view templates
// where v is invariant (for example, an each that already filters on the value of v, like Todo.Done)
let attrNotify<'T> (attrName:string) (value :'T) (onchange : 'T -> unit) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = listen "input" parent  <| fun _ ->
        Interop.get parent attrName |> onchange
    Interop.set parent attrName value
    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubInput)
    unitResult(ctx,"attrNotify")

// Bind an observable value to an element attribute. Listen for onchange events and dispatch the
// attribute's current value to the given function
let bindAttrBoth<'T> (attrName:string) (value : IObservable<'T>) (onchange : 'T -> unit) : SutilElement =
    fragment [
        bindAttrIn attrName value
        bindAttrOut attrName onchange
    ]

let bindListen<'T> (attrName:string) (store : IObservable<'T>) (event:string) (handler : Event -> unit) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let unsubA = Sutil.DOM.listen event parent handler
    let unsubB = store |> Store.subscribe ( Interop.set parent attrName )
    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubA)
    SutilNode.RegisterDisposable(ctx.Parent,unsubB)
    unitResult(ctx,"bindListen")

// Bind a store value to an element attribute. Listen for onchange events write the converted
// value back to the store
let private bindAttrConvert<'T> (attrName:string) (store : Store<'T>) (convert : obj -> 'T) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    //let attrName' = if attrName = "value" then "__value" else attrName
    let unsubInput = DOM.listen "input" parent <| fun _ ->
        Interop.get parent attrName |> convert |> Store.set store
    let unsub = store |> Store.subscribe ( Interop.set parent attrName )
    SutilNode.RegisterUnsubscribe(parent,unsubInput)
    SutilNode.RegisterDisposable(parent,unsub)
    unitResult(ctx,"bindAttrConvert")

// Unsure how to safely convert Element.getAttribute():string to 'T
let private convertObj<'T> (v:obj) : 'T  =
    v :?> 'T

// Bind a store to an attribute in both directions
let bindAttrStoreBoth<'T> (attrName:string) (store : Store<'T>) =
    bindAttrConvert attrName store convertObj<'T>

let bindAttrStoreOut<'T> (attrName:string) (store : Store<'T>) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = DOM.listen "input" parent <| fun _ ->
        Interop.get parent attrName |> convertObj<'T> |> Store.set store
    //(asEl parent).addEventListener("input", (fun _ -> Interop.get parent attrName |> convertObj<'T> |> Store.set store ))
    SutilNode.RegisterUnsubscribe(ctx.Parent,unsubInput)
    unitResult(ctx,"bindAttrStoreOut")

let private attrIsSizeRelated  (attrName:string) =
    let upr = attrName.ToUpper()
    upr.IndexOf("WIDTH") >= 0 || upr.IndexOf("HEIGHT") >= 0

let listenToProp<'T> (attrName:string) (dispatch: 'T -> unit) : SutilElement = nodeFactory <| fun ctx ->
    let parent = ctx.ParentNode
    let notify() = Interop.get parent attrName |> convertObj<'T> |> dispatch

    once Event.ElementReady parent <| fun _ ->
        if attrIsSizeRelated attrName then
            SutilNode.RegisterDisposable(parent,(ResizeObserver.getResizer (downcast parent)).Subscribe( notify ))
        else
            SutilNode.RegisterUnsubscribe(parent,DOM.listen "input" parent (fun _ -> notify()))

        rafu notify

    unitResult(ctx,"listenToProp")

let bindPropOut<'T> (attrName:string) (store : Store<'T>) : SutilElement =
    listenToProp attrName (Store.set store)

type KeyedStoreItem<'T,'K> = {
    Key : 'K
    //CachedElement : HTMLElement
    Node : SutilNode
    SvId : int
    Position : IStore<int>
    Value: IStore<'T>
    Rect: ClientRect
}

let private findCurrentNode doc (current:Node) (id:int) =
    if (isNull current || isNull current.parentNode) then
        log($"each: Find node with id {id}")
        match DOM.findNodeWithSvId doc id with
        | None ->
            log("each: Disaster: cannot find node")
            null
        | Some n ->
            log($"each: Found it: {n}")
            n
    else
        //log($"Cannot find node with id {id}")
        current

let private findCurrentElement doc (current:Node) (id:int) =
    let node = findCurrentNode doc current id
    match node with
    | null -> null
    | n when isElementNode n -> n :?> HTMLElement
    | x ->  log $"each: Disaster: found node but it's not an HTMLElement"
            null

let private genEachId = Helpers.makeIdGenerator()

let eachiko (items:IObservable<list<'T>>) (view : IObservable<int> * IObservable<'T> -> SutilElement) (key:int*'T->'K) (trans : TransitionAttribute list) : SutilElement =
    let log s = Logging.log "each" s
    nodeFactory <| fun ctx ->
        log($"eachiko: Previous = {ctx.Previous}")
        let eachGroup = NodeGroup("each",ctx.Parent,ctx.Previous)
        let eachNode = GroupNode eachGroup
        ctx.AddChild eachNode

        let mutable state : KeyedStoreItem<'T,'K> list = []
        let eachId = genEachId() + 1
        let idKey = "svEachId"
        let hasEid (n : Node) = Interop.exists n idKey
        let eachIdOf n : int = if hasEid n then  Interop.get n idKey else -1
        let setEid n = Interop.set n idKey eachId
        let eachCtx = ctx |> ContextHelpers.withParent eachNode

        let logState state' =
            Browser.Dom.console.groupCollapsed("each state #" + eachGroup.Id)
            state' |> List.map (fun s -> sprintf "%s %f,%f" (string s.Key) s.Rect.left s.Rect.top) |> List.iter (fun s -> log(s))
            Browser.Dom.console.groupEnd()

        let logItems (items : list<'T>) =
            Browser.Dom.console.groupCollapsed("each items #" + eachGroup.Id)
            items |> List.mapi (fun i s -> sprintf "%s" (string (key(i,s)))) |> List.iter (fun s -> log(s))
            Browser.Dom.console.groupEnd()

        let unsub = items |> Store.subscribe (fun newItems ->
            let wantAnimate = true

            log("-- Each Block Render -------------------------------------")
            log($"caching rects for render. Previous: {state |> List.length} items. Current {newItems |> List.length} items")

            state <- state |> List.map (fun ki ->
                let el = findCurrentElement ctx.Document (*ki.Element*)null ki.SvId
                { ki with (*Element = el; *)Rect = el.getBoundingClientRect() })

            //logItems newItems
            //logState state

            // Last child that doesn't have our eachId
            log($"Previous = {ctx.Previous}")
            //let prevNodeInit : Node = vnode.PrevDomNode
            let mutable prevNode = EmptyNode

            let newState = newItems |> List.mapi (fun itemIndex item ->
                let itemKey = key(itemIndex,item)
                let optKi = state |> Seq.tryFind (fun x -> x.Key = itemKey)
                match optKi with
                | None ->
                    let storePos = Store.make itemIndex
                    let storeVal = Store.make item
                    let ctx2 = eachCtx |> ContextHelpers.withPrevious prevNode
                    DomEdit.log $"++ creating new item '{item}' (key={itemKey}) with prev='{prevNode}' action={ctx2.Action}"
                    let sutilNode = ctx2 |> build (view (storePos,storeVal))
                    let itemNode = ctx2 |> asDomElement sutilNode
                    DomEdit.log $"-- created #{svId itemNode} with prev='{nodeStrShort (itemNode.previousSibling)}'"
                    setEid itemNode
                    SutilNode.RegisterDisposable(itemNode,storePos)
                    SutilNode.RegisterDisposable(itemNode,storeVal)
                    transitionNode itemNode trans [Key (string itemKey)] true ignore ignore
                    let newKi = {
                        SvId = svId itemNode
                        Key = itemKey
                        Node = sutilNode
                        //CachedElement = itemNode
                        Position = storePos
                        Rect = itemNode.getBoundingClientRect()
                        Value = storeVal
                    }

                    let prevEl = itemNode.previousSibling :?> HTMLElement
                    log $"new item #{newKi.SvId} eid={eachIdOf itemNode} {itemKey} {rectStr newKi.Rect} prevNode={prevNode} prevSibling={nodeStr prevEl}"
                    prevNode <- sutilNode
                    newKi
                | Some ki ->
                    ki.Position |> Store.modify (fun _ -> itemIndex)
                    ki.Value |> Store.modify (fun _ -> item)
                    let el = findCurrentElement ctx.Document null ki.SvId (*ki.Element*)
                    log $"existing item {ki.SvId} {ki.Key} {rectStr ki.Rect}"
                    if wantAnimate then
                        clearAnimations el
                        animateNode el (ki.Rect)
                    prevNode <- ki.Node
                    ki
            )

            //logState newState

            log("Remove old items")
            // Remove old items
            for oldItem in state do
                if not (newState |> Seq.exists (fun x -> x.Key = oldItem.Key)) then
                    log($"removing key {oldItem.Key}")
                    let el = findCurrentElement ctx.Document null oldItem.SvId (*oldItem.Element*)
                    fixPosition el
                    //ctx.Parent.RemoveChild(el) |> ignore
                    ctx.Parent.InsertBefore(el,null) |> ignore
                    //oldItem.Node.Dispose()
                    transitionNode el trans [Key (string oldItem.Key)] false
                        ignore (fun e -> eachGroup.RemoveChild(oldItem.Node))

            //ctx.Parent.PrettyPrint("each #" + vnode.Id + ": before reorder")

            // Reorder
            let mutable prevDomNode = eachGroup.PrevDomNode
            for ki in newState do
                log($"Re-order: #{ki.SvId}")
                let el = findCurrentElement ctx.Document null ki.SvId (*ki.Element*)
                if not (isNull el) then
                    if not(isSameNode prevDomNode el.previousSibling) then
                        log($"reordering: ki={nodeStr el} prevNode={nodeStr prevDomNode}")
                        log($"reordering key {ki.Key} {nodeStrShort el} parent={el.parentNode}")
                        ctx.Parent.RemoveChild(el) |> ignore
                        ctx.Parent.InsertAfter(el, prevDomNode)
                    prevDomNode <- el

            //ctx.Parent.PrettyPrint("each #" + vnode.Id + ": after reorder")

            state <- newState
        )

        eachGroup.SetDispose (Helpers.unsubify unsub)
        sutilResult eachNode

let private duc = Observable.distinctUntilChanged

let each (items:IObservable<list<'T>>) (view : 'T -> SutilElement) (trans : TransitionAttribute list) =
    eachiko items (fun (_,item) -> bindFragment (duc item) view) (fun (i,v) -> i,v.GetHashCode()) trans

let eachi (items:IObservable<list<'T>>) (view : (int*'T) -> SutilElement)  (trans : TransitionAttribute list) : SutilElement =
    eachiko items (fun (index,item) -> bindFragment2 (duc index) (duc item) view) fst trans

let eachio (items:IObservable<list<'T>>) (view : (IObservable<int>*IObservable<'T>) -> SutilElement)  (trans : TransitionAttribute list) =
    eachiko items view fst trans

let eachk (items:IObservable<list<'T>>) (view : 'T -> SutilElement)  (key:'T -> 'K) (trans : TransitionAttribute list) =
    eachiko
        items
        (fun (_,item) -> bindFragment (duc item) view)
        (snd>>key)
        trans

//
// Turn events into an IObservable using a map function
// Pass IObservable to SutilElement function so that containing element can be side-effected (eg, bindClass)
//
let bindEvent<'T> (event:string) (map:Event -> 'T) (app:IObservable<'T> -> DOM.SutilElement) : DOM.SutilElement = nodeFactory <| fun ctx ->
    let s = Store.make Unchecked.defaultof<'T>
    let u = listen event ctx.ParentNode (map >> Store.set s)
    SutilNode.RegisterDisposable(ctx.Parent,s)
    SutilNode.RegisterUnsubscribe(ctx.Parent,u)
    ctx |> (s |> app |> build)

let bindEventU<'T> (event:string) (map:Event -> 'T) (app:IObservable<'T> -> unit) : DOM.SutilElement =
     bindEvent event map (fun s -> app(s); fragment[])

let bindStore<'T> (init:'T) (app:Store<'T> -> DOM.SutilElement) : DOM.SutilElement = nodeFactory <| fun ctx ->
    let s = Store.make init
    SutilNode.RegisterDisposable(ctx.Parent,s)
    ctx |> (s |> app |> build)

let declareStore<'T> (init : 'T) (f : Store<'T> -> unit) =
    declareResource (fun () -> Store.make init) f

let (|=>) a b = bindFragment a b

let selectApp (selectors : (IObservable<bool> * (unit ->SutilElement)) list) = nodeFactory <| fun ctx ->
    let s = selectors |> List.map fst |> firstOf
    let apps = selectors |> List.map snd |> Array.ofList

    let u = s.Subscribe(fun i ->
        if i >= 0 then
            build (exclusive (apps.[i]())) ctx |> ignore
    )

    unitResult(ctx,"selectApp")


// BindApi is a way for me to refactor this module into a public-facing documentation API with
// overloads where appropriate.
// Some examples will still be referencing Bindings.*

[<AutoOpen>]
module BindApi =
    type Bind =
        /// Dual-binding for a given attribute. Changes to value are written to the attribute, while
        /// changes to the attribute are written back to the store. Note that an IStore is also
        /// an IObservable, for which a separate overload exists.
        static member attr<'T> (name:string, value: IStore<'T>) = bindAttrStoreBoth name value

        /// One-way binding from value to attribute. Note that passing store to this function will
        /// select the more specific `attr<'T>( string, IStore<'T>)` overload.
        /// If that looks to be a problem, we'll rename both of them to force a considered choice.
        static member attr<'T> (name:string, value: IObservable<'T>) = bindAttrIn name value

        /// One-way binding from attribute to dispatch function
        static member attr<'T> (name:string, dispatch: 'T -> unit) = bindAttrOut name dispatch

        /// Two-way binding from value to attribute and from attribute to dispatch function
        static member attr<'T> (name:string, value: IObservable<'T>, dispatch: 'T -> unit) =
            bindAttrBoth name value dispatch


        /// Binding from value to a DOM fragment. Each change in value replaces the current DOM fragment
        /// with a new one.
        static member fragment<'T>  (value : IObservable<'T>)  (element: 'T -> SutilElement) = bindFragment value element

        /// Binding from two values to a DOM fragment. See fragment<'T>
        static member fragment2<'A,'B>  (valueA : IObservable<'A>) (valueB : IObservable<'B>) (element: 'A * 'B -> SutilElement) = bindFragment2 valueA valueB element

        static member selected<'T when 'T : equality>  (value : IObservable<'T list>, dispatch : 'T list -> unit) = bindSelected value dispatch
        static member selected<'T when 'T : equality>  (store : IStore<'T list>) = bindSelectMultiple store
        static member selected<'T when 'T : equality>  (store : IStore<'T option>) = bindSelectOptional store
        static member selected<'T when 'T : equality>  (store : IStore<'T>) = bindSelectSingle store


