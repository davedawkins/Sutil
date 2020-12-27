module Sveltish.Bindings

    open System.ComponentModel
    open Styling
    open Transition
    open DOM
    open Browser.Types
    open Fable.Core
    open Browser.Dom
    open Browser.Css
    open Browser.CssExtensions

    type TransitionFn = (TransitionProp list -> HTMLElement -> unit -> Transition)

    type TransitionFactory = TransitionFn * (TransitionProp list)

    type TransitionAttribute =
        | Both of TransitionFactory
        | In of TransitionFactory
        | Out of TransitionFactory
        | InOut of (TransitionFactory * TransitionFactory)

    let log s = Logging.log "bind" s

    let bindId = CodeGeneration.makeIdGenerator()

    let isTextNode (n:Node) = n.nodeType = 3.0


    let bind<'T>  (store : Store<'T>)  (element: 'T -> NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let mutable current : Node = null

        let addReplaceChild p c =
            if (isNull current || not (parent.isSameNode(p))) then
                ctx.AppendChild p c |> ignore
            else
                if isNull current.parentElement then
                    //console.log("Uh oh - our node was removed - let's clean up")
                    // This can happen if we are generating text that then gets auto-formatted
                    // by a syntax highlighter. We're going to see more of these collisions
                    // with other libraries that are editing DOM that we created.
                    for foreignNode in children p |> List.filter (fun n -> not (Interop.exists n "_svid")) do
                        //console.log(foreignNode)
                        p.removeChild(foreignNode) |> ignore
                    ctx.AppendChild p c |> ignore
                else
                    ctx.ReplaceChild p c current |> ignore
            c

        let unsub = store.Subscribe( fun t ->
            let svId = bindId() |> string
            current <- element(t)( { ctx with AppendChild = addReplaceChild }, parent)
            Interop.set current "_svid" svId
        )
        current

    let bind2<'A,'B>  (a : Store<'A>) (b : Store<'B>)  (element: ('A*'B) -> NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let mutable current : Node = null

        let addReplaceChild p c =
            if isNull current then
                ctx.AppendChild p c |> ignore
            else
                p.replaceChild(c,current) |> ignore
            c

        let unsub = Store.subscribe2 a b (fun (a',b') ->
            current <- element(a',b')( { ctx with AppendChild = addReplaceChild }, parent)
        )

        current

    let waitAnimationEnd (el : HTMLElement) (f : unit -> unit) =
        let rec cb _ =
            el.removeEventListener("animationend",cb)
            f()
        el.addEventListener("animationend", cb)

    let animateNode (node : HTMLElement) from =
        //let from = node.getBoundingClientRect()
        Store.waitEndNotify <| fun () ->
            //createAnimation node from flip [] |> ignore
            window.requestAnimationFrame( fun _ ->
                let name = createAnimation node from flip []
                waitAnimationEnd node <| fun _ -> deleteRule node name
                ) |> ignore


    let transitionNode (el : HTMLElement) (trans : TransitionAttribute option) (transProps : TransitionProp list) (isVisible :bool) (complete: HTMLElement -> unit)=
        let mutable ruleName = ""

        let hide() =
            showEl el false
            complete el
            Transition.deleteRule el ruleName

        let rec show() =
            showEl el true
            complete el
            Transition.deleteRule el ruleName

        let tr = trans |> Option.bind (fun x ->
            match x with
            | Both t -> Some t
            | In t -> if isVisible then Some t else None
            | Out t -> if isVisible then None else Some t
            | InOut (tin,tout) -> if isVisible then Some tin else Some tout
            )

        match tr with
        | None ->
            showEl el isVisible
            complete el
        | Some (tr,trProps) ->
            deleteRule el ""
            if isVisible then
                let trans = (tr (transProps @ trProps) el)
                Store.waitEndNotify <| fun () ->
                    waitAnimationEnd el show
                    showEl el true
                    ruleName <- Transition.createRule el 0.0 1.0 trans 0
            else
                let trans = (tr transProps el)
                Store.waitEndNotify <| fun () ->
                    waitAnimationEnd el hide
                    ruleName <- Transition.createRule el 1.0 0.0 trans 0

    type Hideable = {
        predicate : Store<bool>
        element   : NodeFactory
        transOpt  : TransitionAttribute option
    }

    type HideableRuntime = {
        hideable : Hideable
        mutable target : Node
        mutable cache : bool
        mutable unsubscribe : (unit -> unit)
    }

    let createHideableRuntime h =
        {
            hideable = h
            target = null
            cache = false
            unsubscribe = fun _ -> ()
        }

    let transitionList (list : Hideable list) = fun (ctx, parent) ->
        let runtimes = list |> List.map createHideableRuntime
        for rt in runtimes do
            rt.unsubscribe <- rt.hideable.predicate.Subscribe( fun show ->
                if (isNull rt.target) then
                    rt.target <- rt.hideable.element(ctx,parent)
                    rt.cache <- not show

                if (rt.cache <> show) then
                    rt.cache <- show

                transitionNode (rt.target :?> HTMLElement) rt.hideable.transOpt [] show ignore
            )
        runtimes.Head.target

    type MatchOption<'T> = ('T -> bool) *  NodeFactory * TransitionAttribute option

    let makeHideable guard element transOpt = {
        element = element
        transOpt = transOpt
        predicate = guard
    }
    let transitionMatch<'T> (store : Store<'T>) (options : MatchOption<'T> list) =
        options |> List.map (fun (p,e,t) -> makeHideable (store |%> p) e t) |> transitionList

    let transitionOpt (trans : TransitionAttribute option) (store : Store<bool>) (element: NodeFactory) (elseElement : NodeFactory option): NodeFactory = fun (ctx,parent) ->
        let mutable target : Node = null
        let mutable cache = false

        let mutable targetElse : Node = null

        let unsub = store.Subscribe( fun isVisible ->
            if isNull target then
                target <- element(ctx,parent)
                cache <- not isVisible
                match elseElement with
                | Some e ->
                    targetElse <- e(ctx,parent)
                    //ctx.AppendChild parent targetElse |> ignore
                | None -> ()

            if cache <> isVisible then
                cache <- isVisible
                transitionNode (target :?> HTMLElement) trans [] isVisible ignore
                if not (isNull targetElse) then
                    transitionNode (targetElse :?> HTMLElement) trans [] (not isVisible) ignore
        )
        // Not sure about this. Something is wrong in the design, since we (might) have created two elements
        // We could create a container div to hold them and return that div.
        target

    // Show or hide according to a Store<bool> using a transition
    let transition<'T> (trans : TransitionAttribute) store element =
        transitionOpt (Some trans) store element None

    // Alternate between a pair of elements according to a Store<bool> with no transition
    let transitionElse<'T> (trans : TransitionAttribute) store element otherElement=
        transitionOpt (Some trans) store element (Some otherElement)

    // Show or hide according to a Store<bool> with no transition
    let show<'T> store element =
        transitionOpt None store element None

    // Alternate between a pair of elements according to a Store<bool> with no transition
    let showElse<'T> store element otherElement=
        transitionOpt None store element (Some otherElement)

    let getInputChecked el = Interop.get el "checked"
    let setInputChecked (el : Node) (v:obj) = Interop.set el "checked" v
    let getInputValue el : string = Interop.get el "value"
    let setInputValue el (v:string) = Interop.set el "value" v

    let bindGroup<'T> (store:Store<List<string>>) = fun (ctx:BuildContext,parent:Node) ->
        let name = $"store-{store.Id}"

        let getValueList() =
            let inputs = parent.ownerDocument.querySelectorAll(@$"input[name=""{name}""]")
            [0..(inputs.length-1)] |> List.map (fun i -> inputs.[i]) |> List.filter getInputChecked |> List.map getInputValue

        let updateChecked (v : List<string>) =
            setInputChecked parent ( v |> List.contains (getInputValue parent) )

        // Sync checked upon init
        let rec ready _ =
            store |> Store.get |> updateChecked
            parent.removeEventListener( Event.ElementReady, ready )

        // Update the store when the radio box is clicked on
        parent.addEventListener("input", (fun _ ->
            //log($"%A{getValueList()}")
            getValueList() |> Store.set store
        ))

        // Group this input with all other inputs that reference the same store
        Interop.set parent "name" name

        // We need to finalize checked status after all attrs have been processed for input,
        // in case 'value' hasn't been set yet
        parent.addEventListener( Event.ElementReady, ready )

        // When store changes make sure check status is synced
        let unsub = store.Subscribe(updateChecked)

        parent


    // T can realistically only be numeric or a string. We're relying (I think!) on JS's ability
    // to turn a string into an int automatically in the Store.set call (maybe it's Fable doing that)
    //
    let bindRadioGroup<'T> (store:Store<'T>) = fun (ctx:BuildContext,parent:Node) ->
        let updateChecked (v : obj) =
            setInputChecked parent ( (string v) = getInputValue parent )

        // Sync checked upon init
        let rec ready _ =
            store |> Store.get |> updateChecked
            parent.removeEventListener( Event.ElementReady, ready )

        // Update the store when the radio box is clicked on
        parent.addEventListener("input", (fun _ -> Interop.get parent "value" |> Store.set store ))

        // Group this input with all other inputs that reference the same store
        Interop.set parent "name" $"store-{store.Id}"

        // We need to finalize checked status after all attrs have been processed for input,
        // in case 'value' hasn't been set yet
        parent.addEventListener( Event.ElementReady, ready )

        // When store changes make sure check status is synced
        let unsub = store.Subscribe(updateChecked)

        parent

    // Bind a store value to an element attribute. Updates to the element are unhandled
    let bindAttrIn<'T> (attrName:string) (store : Store<'T>) = fun (ctx:BuildContext,parent:Node) ->
        let unsub = store.Subscribe( fun value -> Interop.set parent attrName value )
        parent

    // Bind a scalar value to an element attribute. Listen for onchange events and dispatch the
    // attribute's current value to the given function. This form is useful for view templates
    // where v is invariant (for example, an each that already filters on the value of v, like Todo.Done)
    let attrNotify<'T> (attrName:string) (v :'T) (onchange : obj -> unit)= fun (ctx:BuildContext,parent:Node) ->
        parent.addEventListener("input", (fun _ -> Interop.get parent attrName |> onchange ))
        Interop.set parent attrName v
        parent


    // Bind a store value to an element attribute. Listen for onchange events and dispatch the
    // attribute's current value to the given function
    let bindAttrNotify<'T> (attrName:string) (store : Store<'T>) (onchange : obj -> unit)= fun (ctx:BuildContext,parent:Node) ->
        parent.addEventListener("input", (fun _ -> Interop.get parent attrName |> onchange ))
        let unsub = store.Subscribe( Interop.set parent attrName )
        parent

    // Bind a store value to an element attribute. Listen for onchange events write the converted
    // value back to the store
    let bindAttrConvert<'T> (attrName:string) (store : Store<'T>) (convert : obj -> 'T)= fun (ctx:BuildContext,parent:Node) ->
        parent.addEventListener("input", (fun _ -> Interop.get parent attrName |> convert |> Store.set store ))
        let unsub = store.Subscribe( Interop.set parent attrName )
        parent

    // Unsure how to safely convert Element.getAttribute():string to 'T
    let convertString<'T> (v:obj) : 'T  =
        v :?> 'T

    // Bind a store to an attribute in both directions
    let bindAttr<'T> (attrName:string) (store : Store<'T>) =
        bindAttrConvert attrName store convertString<'T>

    type KeyedItem<'T,'K> = {
        Key : 'K
        Node : Node
        Position : int
        Rect: ClientRect
    }


    let each (items:Store<list<'T>>) (key:'T -> 'K) (filter:'T -> bool) (trans : TransitionAttribute) (view : 'T -> NodeFactory)  =

        fun (ctx,parent) ->
            let mutable state : KeyedItem<'T,'K> list = []
            let unsub = items.Subscribe( fun value ->

                state <- state |> List.map (fun ki -> { ki with Rect = clientRect ki.Node })

                let newItems = items.Value() |> List.filter filter
                let mutable newState  = [ ]
                let mutable enteringNodes = []

                let blockPrevNode = // First node before this collection
                    match state with
                    | [] -> null
                    | x::xs -> x.Node.previousSibling

                // I bet I can do all this in one pass, I will come back to this
                // and improve it. Let's get it working first.

                newItems |> List.mapi (fun itemIndex item ->
                    let itemKey = key item
                    let optKi = state |> List.tryFind (fun x -> x.Key = itemKey)
                    match optKi with
                    | None ->
                        let itemNode = view(item)(ctx,parent) // Item appears, maybe in wrong place
                        transitionNode (itemNode :?> HTMLElement) (Some trans) [Key (string itemKey)] true ignore
                        let newKi = { Key = itemKey; Node = itemNode; Position = itemIndex; Rect = clientRect itemNode }
                        newState <- newState @ [ newKi ]
                        enteringNodes <- newKi :: enteringNodes
                    | Some ki ->
                        let r = (ki.Node :?> HTMLElement).getBoundingClientRect()
                        newState <- newState @ [ { ki with Position = itemIndex } ]
                ) |> ignore

                // Remove old items
                for oldItem in state do
                    if not (newState |> List.exists (fun x -> x.Key = oldItem.Key)) then
                        fixPosition (asEl oldItem.Node)
                        transitionNode (asEl oldItem.Node) (Some trans) [Key (string oldItem.Key)] false
                            removeNode

                // Existence is now synced. Now to reorder

                let mutable last = blockPrevNode
                newState |> List.mapi (fun pos ki ->
                    // Can only re-order this way when all exiting nodes have been removed
                    //if pos <> ki.Position then
                    //    parent.removeChild(ki.Node) |> ignore
                    //    parent.insertBefore(ki.Node, last.nextSibling) |> ignore
                    if not (enteringNodes |> List.exists (fun en -> en.Key = ki.Key)) then
                        animateNode (ki.Node :?> HTMLElement) (ki.Rect)
                    last <- ki.Node
                    ()
                ) |> ignore

                state <- newState
            )
            parent :> Node

    let (|=>) a b = bind a b