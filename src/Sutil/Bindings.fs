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
let logEachEnabled key = Logging.isEnabled key

// Binding helper
let bindSub<'T> (source : IObservable<'T>) (handler : BuildContext -> 'T -> unit) =
    SutilElement.Define( "bindSub",
    fun ctx ->
    let unsub = source.Subscribe( handler ctx )
    SutilEffect.RegisterDisposable(ctx.Host,unsub)
    () )

let elementFromException (x : exn) =
    el "div" [
        attr ("style","color: #FF8888;")
        attr ("title", "See console for details")
        text ("sutil: exception in bind: " + x.Message)
    ]


// Create the binding's comment anchor at the current build position (#896).
let private bindAnchor (name : string) (ctx : BuildContext) : Node =
    let anchor = ctx.Document.createComment name
    ctx.AddChild (anchor :> Node)
    upcast anchor

// Replace the binding's rendered nodes: build the new content immediately before the anchor,
// record it, then remove the previous set — insert-before-delete, as ReplaceChild did (#896).
let private rebuildAt (anchor : Node) (ctx : BuildContext) (se : SutilElement) : unit =
    if isNull anchor.parentNode then
        // Non-Sutil DOM code destroyed the anchor: the binding is dead. Dispose it on the next
        // frame (not inside this store notification) so sibling subscribers keep working (#896).
        Fable.Core.JS.console.error("sutil: binding anchor was removed by non-Sutil code; disposing binding", ctx.ParentNode)
        rafu (fun () -> cleanupDeep anchor)
    else
        let previous = getBindNodes anchor

        let nodes = build se (ctx |> ContextHelpers.withAnchor anchor)
        setBindNodes anchor nodes

        match previous with
        | Some old -> old |> Array.iter removeNode
        | None -> ()

let bindDelay<'T>  (view : HTMLElement -> SutilElement)=
    SutilElement.Define(
        "bindDelay",
        fun ctx ->
            let anchor = bindAnchor "bindDelay" ctx

            let updateView (se : SutilElement) =
                try
                    rebuildAt anchor ctx se
                with
                | x ->
                    JS.console.error(x)
                    rebuildAt anchor ctx (elementFromException x)

            updateView
                (el "div" [
                    onMount (fun _ -> rafu2 (fun _ -> updateView (view ctx.ParentElement))) []
                ])

            [| anchor |]
    )

let bindElementC<'T>  (store : IObservable<'T>) (element: 'T -> SutilElement) (compare : 'T -> 'T -> bool)=
    SutilElement.Define( "bindElementC",
    fun ctx ->
    let anchor = bindAnchor "bindc" ctx

    let disposable = store |> Observable.distinctUntilChangedCompare compare |> Store.subscribe (fun next ->
        try
            if logEnabled() then log($"bind: rebuild bindc with {next}")
            rebuildAt anchor ctx (element next)
        with
        | x ->
            JS.console.error(x)
            rebuildAt anchor ctx (elementFromException x)
    )

    SutilEffect.RegisterUnsubscribe( anchor, fun () ->
        if logEnabled() then log($"dispose: Bind.el (bindc)")
        disposable.Dispose())

    [| anchor |] )

let bindElementCO<'T>  (store : IObservable<'T>) (element: IObservable<'T> -> SutilElement) (compare : 'T -> 'T -> bool)=
    SutilElement.Define( "bindElementCO",
    fun ctx ->
    let anchor = bindAnchor "bindco" ctx

    let disposable = store |> Observable.distinctUntilChangedCompare compare |> Store.subscribe (fun next ->
        try
            if logEnabled() then log($"bind: rebuild bindco with {next}")
            rebuildAt anchor ctx (element store)
        with
        | x ->
            JS.console.error("sutil.bindElementCO:parentNode: ", ctx.ParentNode, "exception:", x)
            rebuildAt anchor ctx (elementFromException x)
    )

    SutilEffect.RegisterUnsubscribe( anchor, fun () ->
        if logEnabled() then log($"dispose: Bind.el (bindco)")
        disposable.Dispose())

    [| anchor |] )

let bindElement<'T>  (store : IObservable<'T>)  (element: 'T -> SutilElement) : SutilElement=
    SutilElement.Define( "bindElement",
    fun ctx ->
    let anchor = bindAnchor "bind" ctx
    let mutable _init = false

    let disposable = store |> Store.subscribe (fun next ->

        // Diagnostic trail for rebuilds against a detached parent; the anchor is the
        // binding's one stable node, so its connectedness is the truth (#895, #896).
        if _init && not (nodeIsConnected anchor) then
            Fable.Core.JS.console.error($"NOT CONNECTED: bind ", ctx.ParentElement)

        try
            if logEnabled() then log($"bind: rebuild with {next}")
            rebuildAt anchor ctx (element next)
            _init <- true
        with
        | x ->
            JS.console.error("sutil.bindElement:parentNode: ", ctx.ParentNode, "exception:", x)
            rebuildAt anchor ctx (elementFromException x)
    )

    SutilEffect.RegisterUnsubscribe( anchor, fun () ->
        if logEnabled() then log($"dispose: Bind.el")
        disposable.Dispose()
    )

    [| anchor |] )
/// Backwards compatibility
let bindFragment = bindElement

let bindElement2<'A,'B> (a : IObservable<'A>) (b : IObservable<'B>)  (element: ('A*'B) -> SutilElement) =
    SutilElement.Define("bindElement2",
    fun ctx ->
    let anchor = bindAnchor "bind2" ctx

    let d = Store.subscribe2 a b (fun next ->
        try
            rebuildAt anchor ctx (element next)
        with
        | x -> Logging.error $"Exception in bind: {x.Message}"
    )

    SutilEffect.RegisterUnsubscribe( anchor, Helpers.unsubify d )

    [| anchor |]
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
        SutilEffect.RegisterDisposable(ctx.Host, unsub)

    SutilEffect.RegisterUnsubscribe(ctx.Host,unsubInput)
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

    SutilEffect.RegisterDisposable(ctx.Host,unsub)
    SutilEffect.RegisterUnsubscribe(ctx.Host,unsubInput)
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

    SutilEffect.RegisterDisposable(ctx.Host,unsub)
    SutilEffect.RegisterUnsubscribe(ctx.Host,inputUnsub)

    () )

let bindClassToggle (toggle:IObservable<bool>) (classesWhenTrue:string) (classesWhenFalse:string) =
    bindSub toggle <| fun ctx active ->
        if active then
            ctx.ParentElement |> ClassHelpers.removeFromClasslist classesWhenFalse
            ctx.ParentElement |> ClassHelpers.addToClasslist classesWhenTrue
        else
            ctx.ParentElement |> ClassHelpers.removeFromClasslist classesWhenTrue
            ctx.ParentElement |> ClassHelpers.addToClasslist classesWhenFalse

let bindBoolAttr (toggle:IObservable<bool>) (boolAttr : string) =
    bindSub toggle <| fun ctx active ->
        match active with
        | true -> ctx.ParentElement.setAttribute(boolAttr,boolAttr)
        | false -> ctx.ParentElement.removeAttribute(boolAttr)

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
    SutilEffect.RegisterDisposable(ctx.Host,unsub)
    () )

/// Bind a store value to an element property.
let bindPropIn<'T> (propName:string) (store : IObservable<'T>) : SutilElement =
    SutilElement.Define("bindPropIn",
    fun ctx ->
    let unsub =
        if propName = "class" then
            store |> Store.subscribe (fun cls -> ctx.ParentElement.className <- (string cls))
        else
            store |> Store.subscribe (fun v -> Interop.set ctx.ParentElement propName v )
    SutilEffect.RegisterDisposable(ctx.Host,unsub)
    () )

// This is mis-named, but I'm going to leave it alone. It doesn't get attribute values. I think
// I was a bit of a DOM noob when I wrote this... :-/
let bindAttrOut<'T> (attrName:string) (onchange : 'T -> unit) : SutilElement =
    SutilElement.Define( "bindAttrOut",
    fun ctx ->
    let parent = ctx.ParentNode
    let unsubInput = listen "input" parent <| fun _ ->
        Interop.get parent attrName |> onchange
    SutilEffect.RegisterUnsubscribe(ctx.Host,unsubInput)
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
    SutilEffect.RegisterUnsubscribe(ctx.Host, unsubInput)
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
    SutilEffect.RegisterUnsubscribe(ctx.Host,unsubA)
    SutilEffect.RegisterDisposable(ctx.Host,unsubB)
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
    SutilEffect.RegisterUnsubscribe(ctx.Host,unsubInput)
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

let bindPropBoth<'T> (propName:string) (value : IObservable<'T>) (onchange : 'T -> unit) : SutilElement =
    fragment [
        bindPropIn propName value
        listenToProp propName onchange
    ]

type KeyedStoreItem<'T,'K> = {
    Key : 'K
    // The item's stable top-level nodes: anchors for binding-rooted views, elements otherwise (#896).
    Nodes : Node[]
    Position : IStore<int>
    Value: IStore<'T>
}

let private genEachId = Helpers.makeIdGenerator()


let private asDomNode (nodes: Node[]) (ctx: BuildContext) : Node =
    // Markers and empty anchors are bookkeeping, not content (#896).
    match resolveNodes nodes |> Array.filter (fun n -> n.nodeType <> 8.0) with
    | [| n |] -> n
    | [||] -> errorNode ctx.Parent $"Error: Empty node"
    | xs ->
        let doc = ctx.Document
        let tmpDiv = doc.createElement ("div")

        let en =
            errorNode (tmpDiv :> Node) "'fragment' not allowed as root for 'each' blocks"

        DomEdit.appendChild tmpDiv en
        ctx.AddChild (tmpDiv :> Node)

        xs
        |> Array.iter (fun x -> DomEdit.appendChild tmpDiv x)

        upcast tmpDiv

let private asDomElement (nodes: Node[]) (ctx: BuildContext) : HTMLElement =
    let node = asDomNode nodes ctx

    if isElementNode node then
        downcast node
    else
        let doc = ctx.Document
        let span = doc.createElement ("span")
        ctx.AddChild (span :> Node)
        DomEdit.appendChild span node
        span

type EachItemRenderer<'T> =
    | Static of ('T -> SutilElement)
    | LiveStore of (IReadOnlyStore<'T> -> SutilElement)
    | Live of (IObservable<'T> -> SutilElement)
    | StaticIndexed of (int * 'T -> SutilElement)
    | LiveIndexed of (IObservable<int> * IObservable<'T> -> SutilElement)

let private eachItemRender (renderer : EachItemRenderer<'T>) (index : IStore<int>) (item : IStore<'T>) : SutilElement =
    match renderer with 
    | Static v -> v (item.Value)
    | StaticIndexed v -> v (index.Value, item.Value)
    | Live v -> v item
    | LiveStore v -> v item
    | LiveIndexed v -> v (index,item)

let getAnimator (trans : TransitionAttribute list)  =
    trans 
    |> List.tryFind (fun p -> match p with Animate a -> true|_ -> false)
    |> Option.bind (fun x -> match x with Animate a -> Some a | _ -> None)

let eachiko_wrapper (items:IObservable<ICollectionWrapper<'T>>) (view : EachItemRenderer<'T>) (key:int*'T->'K) (options : EachOptions) : SutilElement =
    //let log (s:string) = Fable.Core.JS.console.log("each", s) // Logging.log "each" s
    let trans  = options.Transition
    let animator =getAnimator trans

    SutilElement.Define("eachiko_wrapper",
    fun ctx ->
        let anchor : Node = upcast ctx.Document.createComment "each"
        ctx.AddChild anchor

        let mutable state = ([| |] : KeyedStoreItem<'T,'K> array) .ToCollectionWrapper()
        let eachId = genEachId() + 1
        let idKey = "svEachId"
        let setEid n = Interop.set n idKey eachId

        // The item's current single element, resolved through any binding anchors at use time,
        // which is what keeps this correct after inner rebinds replace the element (#896).
        let itemElement (ki : KeyedStoreItem<'T,'K>) : HTMLElement =
            resolveNodes ki.Nodes
            |> Array.tryPick (fun n -> if isElementNode n then Some (n :?> HTMLElement) else None)
            |> Option.defaultValue null

        let unsub = items |> Store.subscribe (fun newItems ->
            options.PreRender()

            if logEachEnabled "each" then
                log("-- Each Block Render -------------------------------------")
                log($"Previous: {state |> CollectionWrapper.length} items. Current {newItems |> CollectionWrapper.length} items")

            let eachCtx = ctx |> ContextHelpers.withAnchor anchor

            let newState = newItems |> CollectionWrapper.mapi (fun itemIndex item ->
                let itemKey = key(itemIndex,item)
                let optKi = state |> CollectionWrapper.tryFind (fun x -> x.Key = itemKey)
                match optKi with
                | None ->
                    let storePos = Store.make itemIndex
                    let storeVal = Store.make item
                    if logEachEnabled "each" then log $"++ creating new item '{item}' (key={itemKey})"
                    let sutilNodes = eachCtx |> build (eachItemRender view storePos storeVal)
                    let itemNode = asDomElement sutilNodes eachCtx
                    setEid itemNode

                    // Keep the stable identities: a wrapped or empty result is owned by its
                    // wrapper element, plus any anchors that need their subscriptions removed (#896).
                    let nodes =
                        match resolveNodes sutilNodes with
                        | [| n |] when isSameNode n (itemNode :> Node) -> sutilNodes
                        | _ ->
                            Array.append
                                (sutilNodes |> Array.filter (fun n -> (getBindNodes n).IsSome))
                                [| itemNode :> Node |]

                    SutilEffect.RegisterDisposable(nodes.[0],storePos)
                    SutilEffect.RegisterDisposable(nodes.[0],storeVal)
                    transitionNode itemNode trans [Key (string itemKey)] true ignore ignore

                    {
                        Key = itemKey
                        Nodes = nodes
                        Position = storePos
                        Value = storeVal
                    }
                | Some ki ->
                    ki.Position |> Store.modify (fun _ -> itemIndex)
                    ki.Value |> Store.modify (fun _ -> item)
                    if logEachEnabled "each" then log $"existing item {ki.Key}"
                    match animator with
                    | Some a->
                        let el = itemElement ki
                        if not (isNull el) then
                            clearAnimations el
                            animateNode el (el.getBoundingClientRect()) a
                    | None -> ()
                    ki
            )

            if logEachEnabled "each" then log("Remove old items")
            // Remove old items
            for oldItem in state do
                if not (newState |> CollectionWrapper.exists (fun x -> x.Key = oldItem.Key)) then
                    if logEachEnabled "each" then log($"removing key {oldItem.Key}")
                    match options.Exit with
                    | ExitOption.Default ->
                        let el = itemElement oldItem
                        if isNull el then
                            oldItem.Nodes |> Array.iter removeNode
                        else
                            fixPosition el
                            DomEdit.insertBefore anchor.parentNode el null
                            transitionNode el trans [Key (string oldItem.Key)] false
                                ignore (fun _ -> oldItem.Nodes |> Array.iter removeNode)
                    | ExitOption.Custom f ->
                        f (itemElement oldItem)

            // Reorder against the anchor: walk the new state in reverse, moving each item's
            // whole node set in front of the previously placed item (#896).
            let itemsArr = newState.ToArray()
            let mutable nextRef : Node = anchor
            for i in itemsArr.Length-1 .. -1 .. 0 do
                let expanded = expandNodes itemsArr.[i].Nodes
                if expanded.Length > 0 then
                    if not (isSameNode ((Array.last expanded).nextSibling) nextRef) then
                        if logEachEnabled "each" then log($"reordering key {itemsArr.[i].Key}")
                        expanded |> Array.iter (fun n -> DomEdit.insertBefore anchor.parentNode n nextRef)
                    nextRef <- expanded.[0]

            state <- newState
            setBindNodes anchor (itemsArr |> Array.collect (fun ki -> ki.Nodes))

            options.PostRender()
        )

        SutilEffect.RegisterUnsubscribe (anchor, Helpers.unsubify unsub)
        [| anchor |]
    )

let private duc = Observable.distinctUntilChanged

let eachiko = eachiko_wrapper

let each (items:IObservable<ICollectionWrapper<'T>>) (view : 'T -> SutilElement) (trans : TransitionAttribute list) =
    eachiko_wrapper items (Static view) (fun (i,v) -> i,v.GetHashCode()) (EachOptions.From trans)

let eachi (items:IObservable<ICollectionWrapper<'T>>) (view : (int*'T) -> SutilElement)  (trans : TransitionAttribute list) : SutilElement =
    //eachiko items (fun (index,item) -> bindElement2 (duc index) (duc item) view) fst trans
    eachiko items (StaticIndexed view) fst (EachOptions.From trans)

let eachio (items:IObservable<ICollectionWrapper<'T>>) (view : (IObservable<int>*IObservable<'T>) -> SutilElement)  (trans : TransitionAttribute list) =
    //eachiko items view fst trans
    eachiko items (LiveIndexed view) fst (EachOptions.From trans)

let internal eachk_options (items:IObservable<ICollectionWrapper<'T>>) (view : 'T -> SutilElement)  (key:'T -> 'K) (options : EachOptions) =
    eachiko
        items
        (Static view)
        (snd>>key)
        options

let eachk (items:IObservable<ICollectionWrapper<'T>>) (view : 'T -> SutilElement)  (key:'T -> 'K) (trans : TransitionAttribute list) =
    eachk_options items view key (EachOptions.From trans)

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
    SutilEffect.RegisterDisposable(ctx.Host,s)
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
    SutilEffect.RegisterDisposable( ctx.Host, unsub )
    () )

let bindElementStyle<'T> (value : IObservable<'T>) (f : HTMLElement -> CSSStyleDeclaration -> 'T -> unit) =
    SutilElement.Define( "bindStyle",
    fun ctx ->
    let style = ctx.ParentElement.style
    let unsub = value.Subscribe(f ctx.ParentElement style)
    SutilEffect.RegisterDisposable( ctx.Host, unsub )
    () )

let bindElementEffect<'T, 'E when 'E :> HTMLElement> (value : IObservable<'T>) (f : 'E -> 'T -> unit) =
    SutilElement.Define( "bindElementEffect",
    fun ctx ->
    let el = ctx.ParentElement :?> 'E
    let unsub = value.Subscribe(f el)
    SutilEffect.RegisterDisposable( ctx.Host, unsub )
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

let bindRightTop (xy : IObservable<float*float>) =
    bindStyle xy (fun style (x,y) ->
        if x <> 0.0 && y <> 0.0 then
            style.right <- x.ToString() + "px"
            style.top <- y.ToString() + "px"
    )

let bindXYWH (wh: IObservable<float*float*float*float>) =
    bindStyle wh (fun style (x,y,w,h) ->
        if w <> 0.0 && h <> 0.0 then
            style.left <- x.ToString() + "px"
            style.top <- y.ToString() + "px"
            style.width <- w.ToString() + "px"
            style.height <- h.ToString() + "px"
    )

let (|=>) store element = bindElement store element

let cssAttrsToString (cssAttrs) =
    cssAttrs |> Seq.map (fun (n,v) -> $"{n}: {v};") |> String.concat ""

let listWrap( list : 'T list ) = list.ToCollectionWrapper()
let listWrapO (list : IObservable<'T list>) = list |> Store.map listWrap

let arrayWrap( arr : 'T array ) = arr.ToCollectionWrapper()
let arrayWrapO (arr : IObservable<'T array>) = arr |> Store.map arrayWrap
