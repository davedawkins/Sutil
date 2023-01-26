module DragDropListSort

/// DragDropListSort
///    Wrapper for Bindings.eachk that allows user to drag-drop sort the list elements
///
///    let create  (items:IObservable<list<'T>>)
///                (slot : 'T -> SutilElement)
///                (key:'T -> 'K) (trans : TransitionAttribute list)
///                (dispatch : DragOperation -> unit)
///
///     items:     data items
///     slot:      item template
///     key:       return key value for data item
///     dispatch:  callback to commit the insert operation
///
/// Classes
///    Element being dragged
///    .dragging-init - momentarily set, allows (some) browser(s) to capture a styled version of the drag element
///    .dragging      - set while dragging
///
///    Element being dragged over
///    .drag-over
///       .insert-before  Insert before this element
///       .insert-after   Insert after this element
///
/// Issues:
/// - Can't drag to end of list. For now, drag to n-1, then move nth up 1 place

open Sutil
open type Feliz.length

open Sutil.Core
open Sutil.CoreElements
open Browser.Types
open Fable.Core.JsInterop

let log s = Browser.Dom.console.log(s)

type DragOperation =
    | InsertBefore of (int * int)
    | InsertAfter of (int * int)
    | Nothing

module private Private =
    let fromTarget (e : Browser.Types.EventTarget) = e :?> Browser.Types.Node
    let toElement (e : Node) = e :?> HTMLElement

    let iterElem (f: HTMLElement -> unit)(node : Node option)  = node |> Option.iter (toElement >> f)

    let addClasses node classes = node |> iterElem (Core.addToClasslist classes)
    let removeClasses node classes = node |> iterElem (Core.removeFromClasslist classes)

    let getKey (n : Node) : int = Interop.get n "_key"

    type DragState() =
        let mutable draggingNode : Browser.Types.Node option = None
        let mutable overNode : Browser.Types.Node option = None
        let mutable dragOp : DragOperation = Nothing

        let getDragOp () =
            match draggingNode, overNode with
            | Some dn, Some ov ->
                let sourceId = getKey dn
                let targetId = getKey ov
                InsertBefore (sourceId,targetId)
            | _ -> Nothing

        let rec siblingOfDragging (node : Node) : Node option =
            match draggingNode with
            | None -> None
            | Some dn ->
                match node with
                | null -> None
                | n when dn.parentNode.isSameNode n.parentNode -> if n.isSameNode dn then None else Some n
                | n -> siblingOfDragging n.parentNode

        let removeDragOverClasses() =
            removeClasses overNode "drag-over insert-after insert-before"

        let addDragInProcessClasses() =
            removeClasses draggingNode "dragging-init"
            addClasses draggingNode "dragging"

        let removeDragClasses() =
            removeClasses draggingNode "dragging-init dragging"

        // Event handlers
        member _.dragOver (e : Browser.Types.Event) =
            e?dropEffect <- "move"

            removeDragOverClasses()
            overNode <- e.target |> fromTarget |> siblingOfDragging
            addClasses overNode "drag-over insert-before"

            dragOp <- getDragOp()

        member _.dragLeave (e : Browser.Types.Event) =
            removeDragOverClasses()
            overNode <- None
            dragOp <- Nothing
            ()

        member _.dragEnd (e : Browser.Types.Event) =
            removeDragOverClasses()
            removeDragClasses()
            ()

        member _.drop dispatch (e : Browser.Types.Event) =
            dragOp |> dispatch

        member _.dragStart (e : Browser.Types.Event) =
            e?effectAllowed <- "move"

            draggingNode <- e.target |> fromTarget |> Some

            draggingNode |> Option.iter (fun dn ->
                e?dataTransfer?setData("text/plain", getKey dn |> string))

            // Allow browser to grab this style as the drag image
            // Works on: Firefox MacOS, Safari MacOS
            // NO effect on: Chrome MacOS
            addClasses draggingNode "dragging-init"

            Core.rafu addDragInProcessClasses // class .dragging

    let slotWrapper (state : DragState) slot dispatch item=
        slot item |> inject [
            Attr.draggable true
            on "dragstart" state.dragStart []
            on "dragover" state.dragOver [PreventDefault] // Causes drop to fire
            on "dragenter" ignore [PreventDefault]
            on "dragleave" state.dragLeave []
            on "dragend" state.dragEnd []
            on "drop" (state.drop dispatch) [PreventDefault]
        ]

open Private
open System
open Sutil.Transition

let create  (items:IObservable<list<'T>>)
            (slot : 'T -> SutilElement)
            (key:'T -> 'K)
            (trans : TransitionAttribute list)
            (dispatch : DragOperation -> unit) =
    let state = DragState()
    Bind.each(items, (slotWrapper state slot dispatch), key, trans)

