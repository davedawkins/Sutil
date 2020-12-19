namespace Sveltish
module Bindings =

    open Stores
    open Styling
    open Transition
    open DOM
    open Browser.Types

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

    //let transition<'T> (trans : TransitionAttribute) (store : Store<bool>) (e : Fvelize.ElementChild) =
    //    console.log(sprintf "Binding transition of %A to store #%d" e (store.Id))
    //    Fvelize.BindingVisibility (
    //            e,
    //            store.Subscribe,
    //            Some trans )

    let nextRuleId = CodeGeneration.makeIdGenerator()

    let transitionNode (el : HTMLElement) (trans : TransitionAttribute option) (transProps : TransitionProp list) (isVisible :bool) (complete: HTMLElement -> unit)=
        let mutable ruleName = ""

        let rec hide = fun _ ->
            addStyleAttr el "display" "none"
            removeStyleAttr el "animation"
            el.removeEventListener( "animationend", hide )
            Transition.deleteRule ruleName
            complete el

        let rec show = fun _ ->
            removeStyleAttr el "display"
            removeStyleAttr el "animation"
            el.removeEventListener( "animationend", show )
            Transition.deleteRule ruleName
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
            if isVisible then
                removeStyleAttr el "display"
            else
                addStyleAttr el "display" "none"
            complete el
        | Some (tr,trProps) ->
            if isVisible then
                log (sprintf "showing %s" el.innerText )
                let trans = (tr transProps el)
                Stores.waitEndNotify <| fun () ->
                    el.addEventListener( "animationend", show )
                    removeStyleAttr el "display"
                    ruleName <- Transition.createRule el 0.0 1.0 trans (nextRuleId())
            else
                log (sprintf "hiding %s" el.innerText )
                let trans = (tr transProps el)
                Stores.waitEndNotify <| fun () ->
                    el.addEventListener( "animationend", hide )
                    ruleName <- Transition.createRule el 1.0 0.0 trans (nextRuleId())

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

    let bindAttr attrName (store : Store<obj>) = fun (ctx,parent:Node) ->
        // Fixme:
        // Can't assume what element type or attribute is being bound
        //
        let input = parent :?> HTMLInputElement
        parent.addEventListener("change", (fun e ->
            store.Set(input.``checked`` :> obj)) )
        let unsub = store.Subscribe( fun value ->
            input.``checked`` <- (string value = "true")
        )
        parent

    type KeyedItem<'T,'K> = {
        Key : 'K
        Node : Node
        Position : int
    }

    let each (items:Store<list<'T>>) (key:'T -> 'K) (filter:'T -> bool) (trans : TransitionAttribute) (view : 'T -> NodeFactory)  =
        let scheduleTask f = f()
    //            Stores.waitEndNotify f

        fun (ctx,parent) ->
            let mutable state : KeyedItem<'T,'K> list = []
            let unsub = items.Subscribe( fun value ->

                let newItems = items.Value() |> List.filter filter
                let mutable newState  = [ ]
                //let mutable enteringNodes = []

                let blockPrevNode = // First node before this collection
                    match state with
                    | [] -> null
                    | x::xs -> x.Node.previousSibling

                // I bet I can do all this in one pass, I will come back to this
                // and improve it.

                newItems |> List.mapi (fun itemIndex item ->
                    let itemKey = key item
                    let optKi = state |> List.tryFind (fun x -> x.Key = itemKey)
                    match optKi with
                    | None ->
                        let itemNode = view(item)(ctx,parent) // Item appears, maybe in wrong place
                        //enteringNodes <- itemNode :: enteringNodes
                        transitionNode (itemNode :?> HTMLElement) (Some trans) [Key (string itemKey)] true ignore
                        newState <- newState @ [ { Key = itemKey; Node = itemNode; Position = itemIndex } ]
                    | Some ki ->
                        newState <- newState @ [ { ki with Position = itemIndex } ]
                ) |> ignore

                // Remove old items
                for oldItem in state do
                    if not (newState |> List.exists (fun x -> x.Key = oldItem.Key)) then
                        transitionNode (oldItem.Node :?> HTMLElement) (Some trans) [Key (string oldItem.Key)] false
                            (fun e -> parent.removeChild( oldItem.Node ) |> ignore)

                // Existence is now synced. Now to reorder

                let mutable last = blockPrevNode
                newState |> List.mapi (fun pos ki ->
                    //if pos <> ki.Position then
                        //parent.removeChild(ki.Node) |> ignore
                        //parent.insertBefore(ki.Node, last.nextSibling) |> ignore
                    log( sprintf "new state: %A" ki )
                    last <- ki.Node
                    ()
                ) |> ignore

                //for n in enteringNodes do
                //    transitionNode (n :?> HTMLElement) (Some trans) [ Key (fun () -> ) ] true ignore

                state <- newState
            )
            parent :> Node

