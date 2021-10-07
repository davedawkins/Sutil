module SevenGuisCells

// Based on http://tomasp.net/blog/2018/write-your-own-excel/
// Modified to only recalculate cells that are dependent, according to 7Guis specification
//
// Still being refactored. Gets better on each pass
//
open type Feliz.borderStyle
open type Feliz.length
open Sutil
open Sutil.Styling
open Sutil.Attr
open Sutil.DOM
open Fable.Core.JsInterop
open Evaluator
//open Browser.Dom
//open Browser.Types
open System

let log s = () //console.log(s)

let filterSome (source : IObservable<'T option>) =
    source |> Observable.filter (fun x -> x.IsSome) |> Observable.map (fun x -> x.Value)

let nodeOfCell pos = Browser.Dom.document.querySelector($"[x-id='{positionStr pos}'") :> Browser.Types.Node

type Message =
  | UpdateValue of Position * string
  | RefreshCell of Position
  | StartEdit of Position

// Because Cells contains a set of IStores, it makes it easier to treat it as
// a remote database. In such an app, update in the MVU will execute update
// commands against the database, and we can also listen for updates from the
// database
// So far: cellSet, updateValue are write operations
//         RefreshCell is a message we might receive from the database
//

type Sheet = Map<Position, Sutil.IStore<string> >

module Cells =
    let mutable cellDb : Sheet = Map.empty

    let valAt pos =
        match cellDb.TryFind pos with
        | None -> ""
        | Some s -> s.Value

    let cellSet pos value =
        cellDb.[pos] <~ value

    let cellListen toPos dispatch =
        let store = cellDb.[toPos]
        let unsub = store |> Store.subscribe dispatch
        ()

    let cellNotify pos =
        valAt pos |> cellSet pos

    let cellInitialise (dispatch : Position -> unit) pos =
        if not (cellDb.ContainsKey pos) then
            cellDb <- cellDb.Add(pos,Store.make "")

let bindCellRefresh pos whenPosUpdated dispatch =
    Cells.cellListen whenPosUpdated (fun _ -> pos |> dispatch)

let bindRefresh targetCell dependsOnCells dispatch =
    for dep in dependsOnCells do
        bindCellRefresh targetCell dep dispatch

type Model =
  { Rows : int list
    Cols : char list
    Active : Position option
    NeedsRefresh : Position option }

let rows m = m.Rows
let cols m = m.Cols

let styleSheet = [
    rule "table" [
        Css.borderSpacing zero
        Css.borderBottom(px 1, solid, "#e0e0e0")
        Css.borderRight(px 1, solid, "#e0e0e0")
    ]
    rule "td, th" [
        Css.minWidth 50
        Css.borderLeft(px 1, solid, "#e0e0e0")
        Css.borderTop(px 1, solid, "#e0e0e0")
        Css.padding 5
    ]
    rule "td.selected" [
        Css.padding 0
    ]
    rule "td div" [
        Css.displayFlex
        Css.flexDirectionRow
    ]
    rule "td input" [
        Css.flex 1
        Css.width 56
        Css.height 22
    ]
    rule "td.active" [
        Css.backgroundColor "red"
    ]
]

let sample = [
    ('B',1), "Fibonacci"
    ('B',2), "1"
    ('B',3), "1"
    ('B',4), "=B2 + B3"
    ('B',5), "=B3 + B4"
    ('B',6), "=B4 + B5"
    ('B',7), "=B5 + B6"
    ('B',8), "=B6 + B7"
    ('B',9), "=B7 + B8"
    ('E',3), "Convert:" ; ('F',3), "0"                ; ('G',3), "°C";
    ('E',4), "Result:"  ; ('F',4), "=32 + F3 * 9 / 5" ; ('G',4), "°F" ]

let sample2 = [
    ('A',1), "Fibonacci"
]

let init() =
    log("init()")
    {  Rows = [1 .. 15]
       Cols = ['A'.. 'K' ]
       Active = None
       NeedsRefresh = None }, Cmd.batch (sample |> List.map (UpdateValue >> Cmd.ofMsg))

let updateValue pos value d =
    log("updateValue")
    let tcells = findTriggerCells value // Examine expression for cells pos is dependent on
    let refresh = (d << RefreshCell)
    (pos :: tcells) |> List.iter (Cells.cellInitialise refresh) // Make sure all cells involved have stores
    bindRefresh pos tcells refresh // Make sure pos gets a refresh when any of tcells update
    Cells.cellSet pos value // Finally update the sheet
    refresh pos

let update (message : Message) (model : Model) : Model * Cmd<Message> =
    log $"{message}"
    match message with

    | RefreshCell p ->
        { model with NeedsRefresh = Some p }, [fun _ -> Cells.cellNotify p]

    | UpdateValue (p,v) ->
        { model with Active = None; NeedsRefresh = None }, [updateValue p v]

    | StartEdit p ->
        { model with Active = Some p; NeedsRefresh = None }, []

let makeStore = Store.makeElmish init update ignore

//
// Render a NodeFctory at a given cell
//
let renderCellAt (renderfn: Position -> SutilElement) (ctx : BuildContext) (cell:Position) =
    log($"renderCellAt {cell}")
    let nodeFactory = renderfn >> exclusive
    build (nodeFactory cell) (ctx |> ContextHelpers.withParent (DomNode (nodeOfCell cell))) |> ignore

let view () : SutilElement =

    let model, dispatch = makeStore()

    let renderPlainCell pos =
        let content = Cells.valAt pos
        fragment [
            onClick (fun _ -> StartEdit pos |> dispatch) []
            evalCellAsString Cells.valAt content |> text
        ]

    let renderActiveCell pos =
        let content = Cells.valAt pos
        Html.div [
            Html.input [
                type' "text"
                Attr.value content
                autofocus
                onKeyDown (fun me -> if me.key = "Enter" then (pos,me.target?value) |> UpdateValue |> dispatch) []
            ]
        ]

    //
    // Root element
    //
    Html.div [
        let rowsXcols = Observable.zip (model .> rows) (model .> cols) |> Observable.distinctUntilChanged

        Bind.el(rowsXcols, fun (rows,cols) -> Html.table [
            Html.thead [
                Html.tr [
                    Html.th [ text "" ]
                    cols |> List.map (fun col -> Html.th [ col |> string |> text ]) |> fragment
                ]
            ]

            Html.tbody
                (rows |> List.map (fun row ->
                    Html.tr [
                        Html.td [ row |> string |> text ]
                        cols |> List.map (fun col ->
                            Html.td [
                                attr("x-id", positionStr (col,row) )
                                renderPlainCell (col,row)
                            ]
                        ) |> fragment
                    ])
                )

            log("Installing bindings for active cell")

            //
            // It's hard to make this section look beautiful. I'll keep working at it
            //

            // Pairs of (Position option, Position option). Each pair represents a change
            // in the active cell.
            // value : the cell being vacated, must be rendered as normal
            // next  : the cell becoming active.

            let activeS = (model |> Observable.map (fun m -> m.Active) |> Observable.pairwise)

            // bindSub at this location in the DOM feeds us the context from this location
            // In this view, this will include the styling applied further up

            bindSub activeS <| fun ctx (value,next) ->
                value |> Option.iter (renderCellAt renderPlainCell  ctx)
                next  |> Option.iter (renderCellAt renderActiveCell ctx)

            // model.Refresh (Potiion option) indicates which cell is requesting a refresh (re-evaluation)
            // the observable stream strips out Option.None to leave only a stream of Positions
            let refreshS =  (model |> Observable.map (fun m -> m.NeedsRefresh) |> filterSome)

            bindSub refreshS (renderCellAt renderPlainCell)
        ])
    ] |> withStyle styleSheet
