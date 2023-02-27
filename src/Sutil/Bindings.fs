///<exclude/>
module internal Sutil.Bindings

open Transition
open Core
open DomHelpers
open Browser.Types
open System
open Fable.Core
open CoreElements

let private logEnabled() = Logging.isEnabled "bind"
let private log s = Logging.log "bind" s
let private bindId = Helpers.makeIdGenerator()

// Binding helper
let bindSub<'T> (source : IObservable<'T>) (handler : BuildContext -> 'T -> unit) =
    SutilElement.Define( "bindSub",
    fun ctx ->
    let unsub = source.Subscribe( handler ctx )
    SutilEffect.RegisterDisposable(ctx.Parent,unsub)
    () )

let elementFromException (x : exn) =
    el "div" [
        attr ("style","color: #FF8888;")
        attr ("title", "See console for details")
        text ("sutil: exception in bind: " + x.Message)
    ]

let bindElementC<'T>  (store : IObservable<'T>) (element: 'T -> SutilElement) (compare : 'T -> 'T -> bool)=
    SutilElement.Define( "bindElementC",
    fun ctx ->
    let mutable node = SideEffect
    let group = SutilEffect.MakeGroup("bindc",ctx.Parent,ctx.Previous)
    let bindNode = Group group

    if logEnabled() then log($"bind: {group.Id} ctx={ctx.Action} prev={ctx.Previous}")
    ctx.AddChild bindNode

    let run() =
        let bindCtx = { ctx with Parent = bindNode }
        let disposable = store |> Observable.distinctUntilChangedCompare compare |> Store.subscribe (fun next ->
            try
                if logEnabled() then log($"bind: rebuild {group.Id} with {next}")
                node <- build (element(next)) (bindCtx |> ContextHelpers.withReplace (node,group.NextDomNode))
            with
            | x ->
                //Logging.error $"Exception in bindo: {x.StackTrace}: parent={ctx.Parent} node={node.ToString()}"
                JS.console.error(x)
                node <- build (elementFromException x) (bindCtx |> ContextHelpers.withReplace (node,group.NextDomNode))

        )
        group.RegisterUnsubscribe ( fun () ->
            if logEnabled() then log($"dispose: Bind.el: {group}")
            node.Dispose()
            disposable.Dispose())

    run()

    bindNode )

let bindElementCO<'T>  (store : IObservable<'T>) (element: IObservable<'T> -> SutilElement) (compare : 'T -> 'T -> bool)=
    SutilElement.Define( "bindElementCO",
    fun ctx ->
    let mutable node = SideEffect
    let group = SutilEffect.MakeGroup("bindco",ctx.Parent,ctx.Previous)
    let bindNode = Group group

    if logEnabled() then log($"bind: {group.Id} ctx={ctx.Action} prev={ctx.Previous}")
    ctx.AddChild bindNode

    let run() =
        let bindCtx = { ctx with Parent = bindNode }
        let disposable = store |> Observable.distinctUntilChangedCompare compare |> Store.subscribe (fun next ->
            try
                if logEnabled() then log($"bind: rebuild {group.Id} with {next}")
                node <- build (element(store)) (bindCtx |> ContextHelpers.withReplace (node,group.NextDomNode))
            with
            | x ->
                JS.console.error(x)
                node <- build (elementFromException x) (bindCtx |> ContextHelpers.withReplace (node,group.NextDomNode))
        )
        group.RegisterUnsubscribe ( fun () ->
            if logEnabled() then log($"dispose: Bind.el: {group}")
            node.Dispose()
            disposable.Dispose())


    run()

    bindNode )

let bindElement<'T>  (store : IObservable<'T>)  (element: 'T -> SutilElement) : SutilElement=
    bindElementCO store (Store.current >> element) (fun _ _-> false)

/// Backwards compatibility
let bindFragment = bindElement

let bindElement2<'A,'B> (a : IObservable<'A>) (b : IObservable<'B>)  (element: ('A*'B) -> SutilElement) =
    SutilElement.Define("bindElement2",
    fun ctx ->
    let mutable node : SutilEffect = SideEffect
    let group = SutilEffect.MakeGroup("bind2",ctx.Parent,ctx.Previous)
    let bindNode = Group group
    ctx.AddChild bindNode

    let bindCtx = { ctx with Parent = bindNode }

    let d = Store.subscribe2 a b (fun next ->
        try
            node <- build (element(next)) (bindCtx |> ContextHelpers.withReplace (node,group.NextDomNode))
        with
        | x -> Logging.error $"Exception in bind: {x.Message}"
    )

    group.RegisterUnsubscribe (Helpers.unsubify d)

    bindNode
    )

let bindElementKO<'T,'K when 'K : equality> (store : IObservable<'T>) (element: IObservable<'T> -> SutilElement) (key : 'T -> 'K) : SutilElement =
    let compare a b = key a = key b
    bindElementCO store element compare

let bindElementK<'T,'K when 'K : equality> (store : IObservable<'T>) (element: 'T -> SutilElement) (key : 'T -> 'K) : SutilElement =
    let compare a b = key a = key b
    bindElementC store element compare

let bindPromiseStore<'T>  (p : ObservablePromise<'T>)
        (waiting : SutilElement)
        (result: 'T -> SutilElement)
        (fail : Exception -> SutilElement)
        : SutilElement =
    bindElement p <| (function
        | PromiseState.Waiting -> waiting
        | PromiseState.Result r -> result r
        | PromiseState.Error x -> fail x)

let bindPromise<'T>  (p : JS.Promise<'T>)
        (waiting : SutilElement)
        (result: 'T -> SutilElement)
        (fail : Exception -> SutilElement)
        : SutilElement =
    let x = ObservablePromise<'T>(p)
    //x.Run p
    bindPromiseStore x waiting result fail

type BindFn<'T> = IObservable<'T> -> ('T -> SutilElement) -> SutilElement
let private getInputChecked el = Interop.get el "checked"
let private setInputChecked (el : Node) (v:obj) = Interop.set el "checked" v
let private getInputValue el : string = Interop.get el "value"
let private setInputValue el (v:string) = Interop.set el "value" v

let bindSelected<'T when 'T : equality> (selection:IObservable<List<'T>>) (dispatch : List<'T> -> unit) : SutilElement =
    SutilElement.Define("bindSelected",
    fun ctx ->

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
        SutilEffect.RegisterDisposable(ctx.Parent, unsub)

    SutilEffect.RegisterUnsubscribe(ctx.Parent,unsubInput)
    ()
    )

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

let bindGroup<'T> (store:Store<List<string>>) : SutilElement =
    SutilElement.Define( "bindGroup",
    fun ctx ->
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
    let unsubInput = DomHelpers.listen "input" parent <| fun _ ->
        getValueList() |> Store.set store

    // We need to finalize checked status after all attrs have been processed for input,
    // in case 'value' hasn't been set yet
    once Event.ElementReady parent <| fun _ ->
        store |> Store.get |> updateChecked

    // When store changes make sure check status is synced
    let unsub = store |> Store.subscribe (updateChecked)

    SutilEffect.RegisterDisposable(ctx.Parent,unsub)
    SutilEffect.RegisterUnsubscribe(ctx.Parent,unsubInput)
    () )

// T can realistically only be numeric or a string. We're relying (I think!) on JS's ability
// to turn a string into an int automatically in the Store.set call (maybe it's Fable doing that)
//
let bindRadioGroup<'T> (store:Store<'T>) : SutilElement =
    SutilElement.Define( "bindRadioGroup",
    fun ctx ->
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

    SutilEffect.RegisterDisposable(ctx.Parent,unsub)
    SutilEffect.RegisterUnsubscribe(ctx.Parent,inputUnsub)

    () )

let bindClassToggle (toggle:IObservable<bool>) (classesWhenTrue:string) (classesWhenFalse:string) =
    bindSub toggle <| fun ctx active ->
        if active then
            ctx.ParentElement |> ClassHelpers.removeFromClasslist classesWhenFalse
            ctx.ParentElement |> ClassHelpers.addToClasslist classesWhenTrue
        else
            ctx.ParentElement |> ClassHelpers.removeFromClasslist classesWhenTrue
            ctx.ParentElement |> ClassHelpers.addToClasslist classesWhenFalse

// Deprecated
let bindClass (toggle:IObservable<bool>) (classes:string) = bindClassToggle toggle classes ""

let bindClassNames (classNames:IObservable<#seq<string>>)  =
    bindSub classNames <| fun ctx current ->
        ctx.ParentElement.className <- ""
        ctx.ParentElement.classList.add( current |> Array.ofSeq )

let bindClassName (classNames:IObservable<string>)  =
    bindSub classNames <| fun ctx current ->
        ctx.ParentElement.className <- current

/// Bind a store value to an element attribute. Updates to the element are unhandled
let bindAttrIn<'T> (attrName:string) (store : IObservable<'T>) : SutilElement =
    SutilElement.Define("bindAttrIn",
    fun ctx ->
    let unsub =
        if attrName = "class" then
            store |> Store.subscribe (fun cls -> ctx.ParentElement.className <- (string cls))
        else
            store |> Store.subscribe (DomHelpers.setAttribute ctx.ParentElement attrName)
    SutilEffect.RegisterDisposable(ctx.Parent,unsub)
    () )

let bindAttrOut<'T> (attrName:string) (onchange : 'T -> unit) : SutilElement =
    SutilElement.Define( "bindAttrOut",
    fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = listen "input" parent <| fun _ ->
        Interop.get parent attrName |> onchange
    SutilEffect.RegisterUnsubscribe(ctx.Parent,unsubInput)
    () )

// Bind a scalar value to an element attribute. Listen for onchange events and dispatch the
// attribute's current value to the given function. This form is useful for view templates
// where v is invariant (for example, an each that already filters on the value of v, like Todo.Done)
let attrNotify<'T> (attrName:string) (value :'T) (onchange : 'T -> unit) : SutilElement =
    SutilElement.Define( "attrNotify",
    fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = listen "input" parent  <| fun _ ->
        Interop.get parent attrName |> onchange
    Interop.set parent attrName value
    SutilEffect.RegisterUnsubscribe(ctx.Parent, unsubInput)
    () )

// Bind an observable value to an element attribute. Listen for onchange events and dispatch the
// attribute's current value to the given function
let bindAttrBoth<'T> (attrName:string) (value : IObservable<'T>) (onchange : 'T -> unit) : SutilElement =
    fragment [
        bindAttrIn attrName value
        bindAttrOut attrName onchange
    ]

let bindListen<'T> (attrName:string) (store : IObservable<'T>) (event:string) (handler : Event -> unit) : SutilElement =
    SutilElement.Define( "bindListen",
    fun ctx ->
    let parent = ctx.ParentNode
    let unsubA = DomHelpers.listen event parent handler
    let unsubB = store |> Store.subscribe ( Interop.set parent attrName )
    SutilEffect.RegisterUnsubscribe(ctx.Parent,unsubA)
    SutilEffect.RegisterDisposable(ctx.Parent,unsubB)
    () )

// Bind a store value to an element attribute. Listen for onchange events write the converted
// value back to the store
let private bindAttrConvert<'T> (attrName:string) (store : Store<'T>) (convert : obj -> 'T) : SutilElement =
    SutilElement.Define( "bindAttrConvert",
    fun ctx ->
    let parent = ctx.ParentNode
    //let attrName' = if attrName = "value" then "__value" else attrName
    let unsubInput = DomHelpers.listen "input" parent <| fun _ ->
        Interop.get parent attrName |> convert |> Store.set store
    let unsub = store |> Store.subscribe ( Interop.set parent attrName )
    SutilEffect.RegisterUnsubscribe(parent,unsubInput)
    SutilEffect.RegisterDisposable(parent,unsub)
    () )

// Unsure how to safely convert Element.getAttribute():string to 'T
let private convertObj<'T> (v:obj) : 'T  =
    v :?> 'T

// Bind a store to an attribute in both directions
let bindAttrStoreBoth<'T> (attrName:string) (store : Store<'T>) =
    bindAttrConvert attrName store convertObj<'T>

let bindAttrStoreOut<'T> (attrName:string) (store : Store<'T>) : SutilElement =
    SutilElement.Define( "bindAttrStoreOut",
    fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = DomHelpers.listen "input" parent <| fun _ ->
        Interop.get parent attrName |> convertObj<'T> |> Store.set store
    //(asEl parent).addEventListener("input", (fun _ -> Interop.get parent attrName |> convertObj<'T> |> Store.set store ))
    SutilEffect.RegisterUnsubscribe(ctx.Parent,unsubInput)
    ()
    )

let private attrIsSizeRelated  (attrName:string) =
    let upr = attrName.ToUpper()
    upr.IndexOf("WIDTH") >= 0 || upr.IndexOf("HEIGHT") >= 0

let listenToProp<'T> (attrName:string) (dispatch: 'T -> unit) : SutilElement =
    SutilElement.Define( sprintf "listenToProp %s" attrName,
    fun ctx ->
    let parent = ctx.ParentNode
    let notify() = Interop.get parent attrName |> convertObj<'T> |> dispatch

    once Event.ElementReady parent <| fun _ ->
        if attrIsSizeRelated attrName then
            SutilEffect.RegisterDisposable(parent,(ResizeObserver.getResizer (downcast parent)).Subscribe( notify ))
        else
            SutilEffect.RegisterUnsubscribe(parent, DomHelpers.listen "input" parent (fun _ -> notify()))

        rafu notify
    () )

let bindPropOut<'T> (attrName:string) (store : Store<'T>) : SutilElement =
    listenToProp attrName (Store.set store)

type KeyedStoreItem<'T,'K> = {
    Key : 'K
    //CachedElement : HTMLElement
    Node : SutilEffect
    SvId : int
    Position : IStore<int>
    Value: IStore<'T>
    Rect: ClientRect
}

let private findCurrentNode doc (current:Node) (id:int) =
    if (isNull current || isNull current.parentNode) then
        if logEnabled() then log($"each: Find node with id {id}")
        match DomHelpers.findNodeWithSvId doc id with
        | None ->
            if logEnabled() then log("each: Disaster: cannot find node")
            null
        | Some n ->
            if logEnabled() then log($"each: Found it: {n}")
            n
    else
        //log($"Cannot find node with id {id}")
        current

let private findCurrentElement doc (current:Node) (id:int) =
    let node = findCurrentNode doc current id
    match node with
    | null -> null
    | n when isElementNode n -> n :?> HTMLElement
    | x ->  if logEnabled() then log $"each: Disaster: found node but it's not an HTMLElement"
            null

let private genEachId = Helpers.makeIdGenerator()


let private asDomNode (element: SutilEffect) (ctx: BuildContext) : Node =
    //let result = (ctx |> build element)
    match element.collectDomNodes () with
    | [ n ] -> n
    | [] -> errorNode ctx.Parent $"Error: Empty node from {element} #{element.Id}"
    | xs ->
        let doc = ctx.Document
        let tmpDiv = doc.createElement ("div")

        let en =
            errorNode (DomNode tmpDiv) "'fragment' not allowed as root for 'each' blocks"

        DomEdit.appendChild tmpDiv en
        ctx.Parent.AppendChild tmpDiv

        xs
        |> List.iter (fun x -> DomEdit.appendChild tmpDiv x)

        upcast tmpDiv

let private asDomElement (element: SutilEffect) (ctx: BuildContext) : HTMLElement =
    let node = asDomNode element ctx

    if isElementNode node then
        downcast node
    else
        let doc = ctx.Document
        let span = doc.createElement ("span")
        DomEdit.appendChild span node
        ctx.Parent.AppendChild span
        span

let eachiko_wrapper (items:IObservable<ICollectionWrapper<'T>>) (view : IObservable<int> * IObservable<'T> -> SutilElement) (key:int*'T->'K) (trans : TransitionAttribute list) : SutilElement =
    let log s = Logging.log "each" s

    SutilElement.Define("eachiko_wrapper",
    fun ctx ->
        log($"eachiko: Previous = {ctx.Previous}")
        let eachGroup = SutilEffect.MakeGroup("each",ctx.Parent,ctx.Previous)
        let eachNode = Group eachGroup
        ctx.AddChild eachNode

        let mutable state = ([| |] : KeyedStoreItem<'T,'K> array) .ToCollectionWrapper()
        let eachId = genEachId() + 1
        let idKey = "svEachId"
        let hasEid (n : Node) = Interop.exists n idKey
        let eachIdOf n : int = if hasEid n then  Interop.get n idKey else -1
        let setEid n = Interop.set n idKey eachId
        let eachCtx = ctx |> ContextHelpers.withParent eachNode

#if LOGGING_ENABLED
        let logState state' =
            Browser.Dom.console.groupCollapsed("each state #" + eachGroup.Id)
            state' |> List.map (fun s -> sprintf "%s %f,%f" (string s.Key) s.Rect.left s.Rect.top) |> List.iter (fun s -> log(s))
            Browser.Dom.console.groupEnd()

        let logItems (items : list<'T>) =
            Browser.Dom.console.groupCollapsed("each items #" + eachGroup.Id)
            items |> List.mapi (fun i s -> sprintf "%s" (string (key(i,s)))) |> List.iter (fun s -> log(s))
            Browser.Dom.console.groupEnd()
#endif

        let unsub = items |> Store.subscribe (fun newItems ->
            let wantAnimate = true

            if Logging.isEnabled "each" then
                log("-- Each Block Render -------------------------------------")
                log($"caching rects for render. Previous: {state |> CollectionWrapper.length} items. Current {newItems |> CollectionWrapper.length} items")

            state <- state |> CollectionWrapper.map (fun ki ->
                let el = findCurrentElement ctx.Document (*ki.Element*)null ki.SvId
                { ki with (*Element = el; *)Rect = el.getBoundingClientRect() })

            //logItems newItems
            //logState state

            // Last child that doesn't have our eachId
            if Logging.isEnabled "each" then log($"Previous = {ctx.Previous}")
            //let prevNodeInit : Node = vnode.PrevDomNode
            let mutable prevNode = SideEffect

            let newState = newItems |> CollectionWrapper.mapi (fun itemIndex item ->
                let itemKey = key(itemIndex,item)
                let optKi = state |> CollectionWrapper.tryFind (fun x -> x.Key = itemKey)
                match optKi with
                | None ->
                    let storePos = Store.make itemIndex
                    let storeVal = Store.make item
                    let ctx2 = eachCtx |> ContextHelpers.withPrevious prevNode
                    if Logging.isEnabled "each" then log $"++ creating new item '{item}' (key={itemKey}) with prev='{prevNode}' action={ctx2.Action}"
                    let sutilNode = ctx2 |> build (view (storePos,storeVal))
                    let itemNode = ctx2 |> asDomElement sutilNode
                    if Logging.isEnabled "each" then log $"-- created #{svId itemNode} with prev='{nodeStrShort (itemNode.previousSibling)}'"
                    setEid itemNode
                    SutilEffect.RegisterDisposable(sutilNode,storePos)
                    SutilEffect.RegisterDisposable(sutilNode,storeVal)
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
                    if Logging.isEnabled "each" then log $"new item #{newKi.SvId} eid={eachIdOf itemNode} {itemKey} {rectStr newKi.Rect} prevNode={prevNode} prevSibling={nodeStr prevEl}"
                    prevNode <- sutilNode
                    newKi
                | Some ki ->
                    ki.Position |> Store.modify (fun _ -> itemIndex)
                    ki.Value |> Store.modify (fun _ -> item)
                    let el = findCurrentElement ctx.Document null ki.SvId (*ki.Element*)
                    if Logging.isEnabled "each" then log $"existing item {ki.SvId} {ki.Key} {rectStr ki.Rect}"
                    if wantAnimate then
                        clearAnimations el
                        animateNode el (ki.Rect)
                    prevNode <- ki.Node
                    ki
            )

            //logState newState

            if Logging.isEnabled "each" then log("Remove old items")
            // Remove old items
            for oldItem in state do
                if not (newState |> CollectionWrapper.exists (fun x -> x.Key = oldItem.Key)) then
                    if Logging.isEnabled "each" then log($"removing key {oldItem.Key}")
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
                if Logging.isEnabled "each" then log($"Checking order: #{ki.SvId}")
                let el = findCurrentElement ctx.Document null ki.SvId (*ki.Element*)
                if not (isNull el) then
                    if not(isSameNode prevDomNode el.previousSibling) then
                        if Logging.isEnabled "each" then log($"reordering: ki={nodeStr el} prevNode={nodeStr prevDomNode}")
                        if Logging.isEnabled "each" then log($"reordering key {ki.Key} {nodeStrShort el} parent={el.parentNode}")
                        //ctx.Parent.RemoveChild(el) |> ignore
                        ctx.Parent.InsertAfter(el, prevDomNode)
                    prevDomNode <- el

            //ctx.Parent.PrettyPrint("each #" + vnode.Id + ": after reorder")

            state <- newState
        )

        eachGroup.RegisterUnsubscribe (Helpers.unsubify unsub)
        eachNode
    )

let private duc = Observable.distinctUntilChanged

let eachiko = eachiko_wrapper

let each (items:IObservable<ICollectionWrapper<'T>>) (view : 'T -> SutilElement) (trans : TransitionAttribute list) =
    eachiko_wrapper items (fun (_,item) -> bindElement (duc item) view) (fun (i,v) -> i,v.GetHashCode()) trans

let eachi (items:IObservable<ICollectionWrapper<'T>>) (view : (int*'T) -> SutilElement)  (trans : TransitionAttribute list) : SutilElement =
    eachiko items (fun (index,item) -> bindElement2 (duc index) (duc item) view) fst trans

let eachio (items:IObservable<ICollectionWrapper<'T>>) (view : (IObservable<int>*IObservable<'T>) -> SutilElement)  (trans : TransitionAttribute list) =
    eachiko items view fst trans

let eachk (items:IObservable<ICollectionWrapper<'T>>) (view : 'T -> SutilElement)  (key:'T -> 'K) (trans : TransitionAttribute list) =
    eachiko
        items
        (fun (_,item) -> bindElement (duc item) view)
        (snd>>key)
        trans

#if false
let each_seq (items:IObservable<seq<'T>>) (view : 'T -> SutilElement) (trans : TransitionAttribute list) =
    eachiko_seq items (fun (_,item) -> bindElement (duc item) view) (fun (i,v) -> i,v.GetHashCode()) trans

let eachi_seq (items:IObservable<seq<'T>>) (view : (int*'T) -> SutilElement)  (trans : TransitionAttribute list) : SutilElement =
    eachiko items (fun (index,item) -> bindElement2 (duc index) (duc item) view) fst trans

let eachio_seq (items:IObservable<seq<'T>>) (view : (IObservable<int>*IObservable<'T>) -> SutilElement)  (trans : TransitionAttribute list) =
    eachiko_seq items view fst trans

let eachk_seq (items:IObservable<seq<'T>>) (view : 'T -> SutilElement)  (key:'T -> 'K) (trans : TransitionAttribute list) =
    eachiko_seq
        items
        (fun (_,item) -> bindElement (duc item) view)
        (snd>>key)
        trans
#endif

let bindStore<'T> (init:'T) (app:Store<'T> -> Core.SutilElement) : Core.SutilElement =
    SutilElement.Define( "bindStore",
    fun ctx ->
    let s = Store.make init
    SutilEffect.RegisterDisposable(ctx.Parent,s)
    ctx |> (s |> app |> build)
    )

let declareStore<'T> (init : 'T) (f : Store<'T> -> unit) =
    declareResource (fun () -> Store.make init) f

open Browser.CssExtensions

let bindStyle<'T> (value : IObservable<'T>) (f : CSSStyleDeclaration -> 'T -> unit) =
    SutilElement.Define( "bindStyle",
    fun ctx ->
    let style = ctx.ParentElement.style
    let unsub = value.Subscribe(f style)
    SutilEffect.RegisterDisposable( ctx.Parent, unsub )
    () )

let bindWidthHeight (wh: IObservable<float*float>) =
    bindStyle wh (fun style (w,h) ->
        if w <> 0.0 && h <> 0.0 then
            style.width <- w.ToString() + "px"
            style.height <- h.ToString() + "px"
    )

let bindLeftTop (xy : IObservable<float*float>) =
    bindStyle xy (fun style (x,y) ->
        if x <> 0.0 && y <> 0.0 then
            style.left <- x.ToString() + "px"
            style.top <- y.ToString() + "px"
    )

let (|=>) store element = bindElement store element

let cssAttrsToString (cssAttrs) =
    cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat ""

let listWrap( list : 'T list ) = list.ToCollectionWrapper()
let listWrapO (list : IObservable<'T list>) = list |> Store.map listWrap

let arrayWrap( arr : 'T array ) = arr.ToCollectionWrapper()
let arrayWrapO (arr : IObservable<'T array>) = arr |> Store.map arrayWrap
