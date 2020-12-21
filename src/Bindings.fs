module Sveltish.Bindings

    open Stores
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

    let bind<'T>  (store : Store<'T>)  (element: unit -> NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let mutable current : Node = null

        let addReplaceChild p c =
            if isNull current then
                ctx.AppendChild p c |> ignore
            else
                p.replaceChild(c,current) |> ignore
            c

        let unsub = store.Subscribe( fun t ->
            current <- element()( { ctx with AppendChild = addReplaceChild }, parent)
        )
        current


    let waitAnimationEnd (el : HTMLElement) (f : unit -> unit) =
        let rec cb _ =
            el.removeEventListener("animationend",cb)
            f()
        el.addEventListener("animationend", cb)

    let animateNode (node : HTMLElement) from =
        //let from = node.getBoundingClientRect()
        Stores.waitEndNotify <| fun () ->
            //createAnimation node from flip [] |> ignore
            window.requestAnimationFrame( fun _ ->
                let name = createAnimation node from flip []
                waitAnimationEnd node <| fun _ -> deleteRule node name
                ) |> ignore


    let transitionNode (el : HTMLElement) (trans : TransitionAttribute option) (transProps : TransitionProp list) (isVisible :bool) (complete: HTMLElement -> unit)=
        let mutable ruleName = ""

        let hide() =
            showEl el false
            Transition.deleteRule el ruleName
            complete el

        let rec show() =
            showEl el true
            Transition.deleteRule el ruleName
            complete el

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
            if isVisible then
                let trans = (tr transProps el)
                Stores.waitEndNotify <| fun () ->
                    waitAnimationEnd el show
                    showEl el true
                    ruleName <- Transition.createRule el 0.0 1.0 trans 0
            else
                fixPosition el
                let trans = (tr transProps el)
                Stores.waitEndNotify <| fun () ->
                    waitAnimationEnd el hide
                    ruleName <- Transition.createRule el 1.0 0.0 trans 0

    let transitionOpt<'T> (trans : TransitionAttribute option) (store : Store<bool>) (element: NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let mutable target : Node = null
        let unsub = store.Subscribe( fun isVisible ->
            if isNull target then
                target <- element(ctx,parent)

            transitionNode (target :?> HTMLElement) trans [] isVisible ignore
        )
        target

    let transition<'T> (trans : TransitionAttribute) store element =
        transitionOpt (Some trans) store element

    let show<'T> store element =
        transitionOpt None store element

    [<Emit("$0[$1]")>]
    let jsGet obj name = jsNative

    [<Emit("$0[$1] = $2")>]
    let jsSet obj name value = jsNative

    let bindAttr attrName (store : Store<obj>) = fun (ctx,parent:Node) ->
        // Fixme:
        // Can't assume what element type or attribute is being bound
        //
        let input = parent :?> HTMLInputElement
        parent.addEventListener("change", (fun e ->
            log <| sprintf "%s changed: %A" attrName (jsGet input attrName)
            store.Set( jsGet input attrName )) )
        let unsub = store.Subscribe( fun value ->
            jsSet input attrName value
        )
        parent

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
                        log(sprintf "%f,%f -> %f,%f  %s"
                                ki.Rect.left ki.Rect.top
                                r.left r.top
                                (ki.Node :?> HTMLElement).innerText)
                ) |> ignore

                // Remove old items
                for oldItem in state do
                    if not (newState |> List.exists (fun x -> x.Key = oldItem.Key)) then
                        transitionNode (oldItem.Node :?> HTMLElement) (Some trans) [Key (string oldItem.Key)] false
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

                //for n in enteringNodes do
                //    transitionNode (n :?> HTMLElement) (Some trans) [ Key (fun () -> ) ] true ignore

                state <- newState |> List.map (fun ki -> { ki with Rect = clientRect ki.Node })
            )
            parent :> Node

