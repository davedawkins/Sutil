namespace Sveltish
open Browser.Dom
open System
open Browser.Types

module CodeGeneration =
    let makeIdGenerator() =
        let mutable id = 0
        fun () ->
            let r = id
            id <- id+1
            r

module DOM =
    open Browser.Dom

    type BuildContext = {
        StyleName : string
        AppendChild: (Node -> Node -> Node)
        ReplaceChild: (Node -> Node -> Node -> Node)
        SetAttribute: (HTMLElement->string->string->unit)
        MakeName : (string -> string)
    }

    let makeContext =
        let gen = CodeGeneration.makeIdGenerator()
        {
            StyleName = ""
            AppendChild = (fun parent child -> parent.appendChild(child))
            ReplaceChild = (fun parent newChild oldChild -> parent.replaceChild(newChild, oldChild))
            SetAttribute = (fun parent name value -> parent.setAttribute(name,value))
            MakeName = fun baseName -> sprintf "%s-%d" baseName (gen())
        }

    type NodeFactory = (BuildContext * HTMLElement) -> Node

    let appendAttribute (e:Element) attrName attrValue =
        if (attrValue <> "") then
            let currentValue = e.getAttribute(attrName)
            e.setAttribute(attrName,
                if ((isNull currentValue) || currentValue = "")
                    then attrValue
                    else (sprintf "%s %s" currentValue attrValue))

    let el tag (xs : seq<NodeFactory>) : NodeFactory = fun (ctx,parent) ->
        let e  = document.createElement tag

        ctx.AppendChild (parent :> Node) (e:>Node) |> ignore

        for x in xs do x(ctx,e) |> ignore

        if ctx.StyleName <> "" then
            appendAttribute e "class" ctx.StyleName

        e :> Node

    let attr (name,value) : NodeFactory = fun (ctx,e) ->
        ctx.SetAttribute e name value
        e :> Node

    let text value : NodeFactory =
        fun (ctx,e) -> ctx.AppendChild (e :> Node) (document.createTextNode(value) :> Node)

    let idSelector = sprintf "#%s"
    let classSelector = sprintf ".%s"
    let findElement selector = document.querySelector(selector)

    let rec mountElement selector (app : NodeFactory)  =
        let host = idSelector selector |> findElement :?> HTMLElement
        (app (makeContext,host)) |> ignore

open DOM

module Html =
    let div xs = el "div" xs
    let h2  xs = el "h2" xs
    let p  xs = el "p" xs
    let span xs = el "span" xs
    let button  xs = el "button" xs
    let input  xs = el "input" xs
    let label  xs = el "label" xs

module Attr =
    let makeAttr = id
    let className n = attr ("class",n)
    let on event fn : NodeFactory = fun (_,el) ->
        el.addEventListener(event, fn)
        el :> Node
    // Styles
    let margin (n:obj)         = makeAttr("margin",n)
    let marginTop (n:obj)      = makeAttr("margin-top",n)
    let marginLeft (n:obj)     = makeAttr("margin-left",n)
    let marginBottom (n:obj)   = makeAttr("margin-bottom",n)
    let marginRight (n:obj)    = makeAttr("margin-right",n)
    let backgroundColor(n:obj) = makeAttr("background-color",n)
    let borderColor (n:obj)    = makeAttr("border-color",n)
    let borderWidth (n:obj)    = makeAttr("border-width",n)
    let color (n:obj)          = makeAttr("color",n)
    let cursor (n:obj)         = makeAttr("cursor",n)
    let justifyContent (n:obj) = makeAttr("justify-content",n)
    let paddingBottom (n:obj)  = makeAttr("padding-bottom",n)
    let paddingLeft (n:obj)    = makeAttr("padding-left",n)
    let paddingRight (n:obj)   = makeAttr("padding-right",n)
    let paddingTop (n:obj)     = makeAttr("padding-top",n)
    let textAlign (n:obj)      = makeAttr("text-align",n)
    let whiteSpace (n:obj    ) = makeAttr("white-space",n)
    let alignItems     (n:obj) = makeAttr("align-items",n)
    let border         (n:obj) = makeAttr("border",n)
    let borderRadius   (n:obj) = makeAttr("border-radius",n)
    let boxShadow      (n:obj) = makeAttr("box-shadow",n)
    let display        (n:obj) = makeAttr("display",n)
    let fontSize       (n:obj) = makeAttr("font-size",n)
    let fontFamily     (n:obj) = makeAttr("font-family",n)
    let width          (n:obj) = makeAttr("width",n)
    let maxWidth       (n:obj) = makeAttr("max-width",n)
    let height         (n:obj) = makeAttr("height",n)
    let lineHeight     (n:obj) = makeAttr("line-height",n)
    let position       (n:obj) = makeAttr("position",n)
    let verticalAlign  (n:obj) = makeAttr("vertical-align",n)
    let fontWeight     (n:obj) = makeAttr("font-height",n)
    let ``float``      (n:obj) = makeAttr("float",n)
    let padding        (n:obj) = makeAttr("padding",n)
    let boxSizing      (n:obj) = makeAttr("box-sizing",n)
    let userSelect     (n:obj) = makeAttr("user-select",n)
    let top            (n:obj) = makeAttr("top",n)
    let left           (n:obj) = makeAttr("left",n)
    let opacity        (n:obj) = makeAttr("opacity",n)
    let transition     (n:obj) = makeAttr("transition",n)


module Stores =
    let newStoreId = CodeGeneration.makeIdGenerator()

    type Store<'T> = {
        Id : int
        Value : (unit -> 'T)
        Set   : ('T -> unit)

        // Subscribe takes a callback that will be called immediately upon
        // subscription, and when the value changes
        // Result is an unsubscription function
        Subscribe : ('T -> unit) -> (unit -> unit)
    }

    type Subscriber<'T> = {
        Id : int
        Set : ('T -> unit)
    }

    let newSubId = CodeGeneration.makeIdGenerator()

    let makeGetSetStore<'T> (get : unit -> 'T) (set : 'T -> unit) =
        let mutable subscribers : Subscriber<'T> list = []
        let myId = newStoreId()
        console.log(sprintf "New store #%d = %A" myId (get()))
        {
            Id = myId
            Value = get
            Set  = fun (v : 'T) ->
                set(v)
                console.log(sprintf "%d: Notify %d subscribers of value %A" myId subscribers.Length v)
                for s in subscribers do
    //                console.log( sprintf "%d: Notifying %A" myId v )
                    s.Set(get())
            Subscribe = (fun notify ->
                let id = newSubId()
                let unsub = (fun () ->
                    subscribers <- subscribers |> List.filter (fun s -> s.Id = id)
                )
                subscribers <- { Id = id; Set = notify } :: subscribers
                console.log(sprintf "%d: %d subscribers" myId subscribers.Length)
                notify(get())
                unsub
            )
        }

    let forceNotify (store : Store<'T>) =
        console.log(sprintf "%d: forceNotify %A" store.Id  (store.Value()))
        store.Value() |> store.Set

    let makeStore<'T> (v : 'T) =
        // Storage is separated from Store<T> so that it doesn't leak
        // through the abstraction.
        let mutable value = v
        let get() = value
        let set(v) = value <- v
        makeGetSetStore get set

    open Fable.Core

    type Lens<'T> = {
            Get : unit -> 'T
            Set : 'T -> unit
        }

    [<Emit("() => ($0)[$1]")>]
    let getter obj name = jsNative

    [<Emit("value => { ($0)[$1] = value; }")>]
    let setter obj name = jsNative

    let makeLens obj name : Lens<obj> =
        {
            Get = getter obj name
            Set = setter obj name
        }

    let makePropertyStore obj name =
        let get = getter obj name
        let set = setter obj name
        makeGetSetStore get set

    let makeExpressionStore<'T> (expr : (unit -> 'T)) =
        let mutable cache : 'T = expr()
        makeGetSetStore
            (fun () -> cache)

            // This setter will be called by forceNotify. We don't about the incoming
            // value (which will have been from our getter() anyway), and so we use
            // this opportunity to recache the expression value.
            (fun v ->
                let vreal = expr()
                console.log(sprintf "expression set : eval = %A" vreal)
                cache <- expr())

    let exprStore = makeExpressionStore

    let propagateNotifications (fromStore : Store<'T1>) (toStore : Store<'T2>) =
        let mutable init = false
        fromStore.Subscribe( fun _ ->
            console.log( sprintf "Received update from %d" fromStore.Id)
            if init then forceNotify toStore else init <- true
            ) |> ignore
        toStore

    let (|~>) a b = propagateNotifications a b |> ignore; b
    let (<~|) a b = propagateNotifications a b |> ignore; a

    // Subscribe wants a setter (T->unit), this converts that into a notifier (unit -> unit)
    let makeNotifier store = (fun callback -> store.Subscribe( fun _ -> callback() )  |> ignore)


module Styling =

    type StyleRule = {
            Selector : string
            Style : (string*obj) list
        }

    type StyleSheet = StyleRule list

    let parseStyleAttr (style : string) =
        style.Split([|';'|], StringSplitOptions.RemoveEmptyEntries)
            |> Array.collect (fun entry ->
                            entry.Split([|':'|],2)
                            |> Array.chunkBySize 2
                            |> Array.map (fun pair -> pair.[0].Trim(), pair.[1].Trim()))

    let emitStyleAttr (keyValues : (string * string) array) =
        keyValues
            |> Array.map (fun (k,v) -> sprintf "%s:%s;" k v )
            |> String.concat ""

    let filterStyleAttr name style =
        parseStyleAttr style
            |> Array.filter (fun (k,v) -> console.log(sprintf "%s=%s %A %s" k v (k <> name) name); k <> name)
            |> emitStyleAttr

    let getStyleAttr (el : HTMLElement) =
        match el.getAttribute("style") with
        | null -> ""
        | s -> s

    let addStyleAttr (el : HTMLElement) name value =
        let style = getStyleAttr el |> filterStyleAttr name
        el.setAttribute( "style", sprintf "%s%s:%s;" style name value )

    let removeStyleAttr (el : HTMLElement) name =
        console.log( sprintf "filter by %s: %A -> %A" name (getStyleAttr el) (getStyleAttr el |> filterStyleAttr name) )
        el.setAttribute( "style", getStyleAttr el |> filterStyleAttr name )

    let newStyleElement =
        let head = "head" |> findElement
        let style = document.createElement("style")
        head.appendChild(style :> Node) |> ignore
        style

    let splitMapJoin (delim:char) (f : string -> string) (s:string) =
        s.Split([| delim |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map f
            |> fun values -> String.Join(string delim, values)

    let isPseudo s =
        s = "hover" || s = "active" || s = "visited" || s = "link" || s = "before" || s = "after" || s = "checked"

    let isGlobal s = s = "body" || s = "html"

    let specifySelector (styleName : string) (selectors : string) =
        let trans s = if isPseudo s || isGlobal s then s else sprintf "%s.%s" s styleName  // button -> button.styleA
        splitMapJoin ',' (splitMapJoin ' ' (splitMapJoin ':' trans)) selectors

    let addStyleSheet styleName (styleSheet : StyleSheet) =
        let style = newStyleElement
        for rule in styleSheet do
            let styleText = String.Join ("", rule.Style |> Seq.map (fun (nm,v) -> sprintf "%s: %A;" nm v))
            [ specifySelector styleName rule.Selector; " {"; styleText; "}" ] |> String.concat "" |> document.createTextNode |> style.appendChild |> ignore

    let style styleSheet (element : NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let name = ctx.MakeName "sveltish"
        addStyleSheet name styleSheet
        element({ ctx with StyleName = name },parent)

    let rule name style = {
        Selector = name
        Style = style
    }

module Bindings =
    open Stores
    open Styling
    open Sveltish.Transition
    type TransitionFactory = HTMLElement -> (TransitionProp list) -> Transition

    type TransitionAttribute =
        | Both of TransitionFactory
        | In of TransitionFactory
        | Out of TransitionFactory
        | InOut of (TransitionFactory * TransitionFactory)

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


    let transitionNode (el : HTMLElement) (trans : TransitionAttribute option) (isVisible :bool) (complete: HTMLElement -> unit)=
        let mutable ruleName = ""

        let rec hide = fun _ ->
            addStyleAttr el "display" "none"
            removeStyleAttr el "animation"
            el.removeEventListener( "animationend", hide )
            //Transition.deleteRule ruleName
            complete el

        let rec show = fun _ ->
            removeStyleAttr el "display"
            removeStyleAttr el "animation"
            el.removeEventListener( "animationend", show )
            //Transition.deleteRule ruleName
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
        | Some tr ->
            if isVisible then
                el.addEventListener( "animationend", show )
                removeStyleAttr el "display"
                ruleName <- Transition.createRule el 0.0 1.0 (tr el []) (nextRuleId())
            else
                el.addEventListener( "animationend", hide )
                ruleName <- Transition.createRule el 1.0 0.0 (tr el []) (nextRuleId())

    let transitionOpt<'T> (trans : TransitionAttribute option) (store : Store<bool>) (element: NodeFactory) : NodeFactory = fun (ctx,parent) ->
        let mutable target : Node = null
        let unsub = store.Subscribe( fun isVisible ->
            if isNull target then
                target <- element(ctx,parent)

            transitionNode (target :?> HTMLElement) trans isVisible (fun _ -> ())
        )
        target

    let transition<'T> (trans : TransitionAttribute) store element =
        transitionOpt (Some trans) store element

    let show<'T> store element =
        console.log("show")
        transitionOpt None store element

    let bindAttr attrName (store : Store<obj>) = fun (ctx,parent:Node) ->
        // Fixme:
        // Can't assume what element type or attribute is being bound
        //
        let input = parent :?> HTMLInputElement
        parent.addEventListener("change", (fun e ->
            console.log(sprintf "attr changed on element %A" input.``checked``)
            store.Set(input.``checked`` :> obj)) )
        let unsub = store.Subscribe( fun value ->
            console.log(sprintf "attr source changed: %A" value)
            input.``checked`` <- (string value = "true")
        )
        parent

    type KeyedItem<'T,'K> = {
        Key : 'K
        Node : Node
        Position : int
    }

    let each (items:Store<list<'T>>) (key:'T -> 'K) (filter:'T -> bool) (trans : TransitionAttribute) (view : 'T -> NodeFactory)  =
        fun (ctx,parent : HTMLElement) ->
            let mutable state : KeyedItem<'T,'K> list = []
            let unsub = items.Subscribe( fun value ->
                console.log("BindingList: sub callback")

                let newItems = items.Value() |> List.filter filter
                let mutable newState  = [ ]
                let mutable enteringNodes = []

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
                        enteringNodes <- itemNode :: enteringNodes
                        newState <- newState @ [ { Key = itemKey; Node = itemNode; Position = itemIndex } ]
                    | Some ki ->
                        newState <- newState @ [ { ki with Position = itemIndex } ]
                ) |> ignore

                // Remove old items
                for oldItem in state do
                    if not (newState |> List.exists (fun x -> x.Key = oldItem.Key)) then
                        transitionNode (oldItem.Node :?> HTMLElement) (Some trans) false
                            (fun e -> parent.removeChild( oldItem.Node ) |> ignore)

                // Existence is now synced. Now to reorder

                let mutable last = blockPrevNode
                newState |> List.mapi (fun pos ki ->
                    //if pos <> ki.Position then
                    //    parent.removeChild(ki.Node) |> ignore
                    //    parent.insertBefore(ki.Node, last.nextSibling) |> ignore
                    last <- ki.Node
                    ()
                ) |> ignore

                for n in enteringNodes do
                    transitionNode (n :?> HTMLElement) (Some trans) true ignore

                state <- newState
            )
            parent :> Node

(*

    let bindAttr (store : Store<obj>) attrName =
        Fvelize.BindingAttribute (attrName,
            store.Subscribe,
            store.Set )

    let each<'T,'K> (items:Store<list<'T>>) (key:'T -> 'K) (filter:'T -> bool) (view:'T -> ElementChild) =
        Fvelize.BindingList (
                (fun () -> items.Value() |> List.filter filter |> List.map view),
                makeNotifier items
        )

    type KeyedItem<'T,'K> = {
        Key : 'K
        Node : Node
        Position : int
    }
        //for todo in todos.Value() |> List.filter filter view
    let each2<'T,'K> (items:Store<list<'T>>) (key:'T -> 'K) (filter:'T -> bool) (view:'T -> ElementChild) =
        let initList = items.Value() |> List.filter filter

        let mutable elements : KeyedItem<'T,'K> array =
            initList
            |> List.mapi (fun i t -> { Key = key t; Node = null; Position = i } )
            |> List.toArray

        let appendChild (ec : ElementChild) (p:Node) c = p.appendChild(c) |> ignore

        let unsub = items.Subscribe( fun newList -> () )

        console.log("each2")

        Fvelize.Block (
                initList |> List.map view,
                appendChild
        )
*)