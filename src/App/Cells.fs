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

open Sutil.Core
open Sutil.CoreElements
open Fable.Core.JsInterop
open Evaluator
open System

let log s = ()// Fable.Core.JS.console.log(s)
let formatPosition (p: Position) = sprintf "[%A:%A]" (fst p) (snd p)
let formatPositions (positions : Position seq) = "[" + (positions |> Seq.map formatPosition |> String.concat ",") + "]"

let filterSome (source : IObservable<'T option>) =
    source |> Observable.filter (fun x -> x.IsSome) |> Observable.map (fun x -> x.Value)

let nodeOfCell pos = Browser.Dom.document.querySelector($"[x-id='{positionStr pos}'") :?> Browser.Types.HTMLElement

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

module Cells =
    type CellValue<'T> = {
        mutable Value : 'T
    }
    with
        static member Create (init : 'T) = { Value = init }
        member __.Set( v : 'T ) = __.Value <- v

    // A cell's value
    let mutable cellValues : Map<Position, CellValue<string> > = Map.empty

    // A cell's input cells
    let mutable cellInputs : Map<Position,Position array> = Map.empty

    // A cell's output cells
    let mutable cellOutputs : Map<Position, Position array> = Map.empty

    let cellGet pos =
        match cellValues.TryFind pos with
        | None -> ""
        | Some s -> s.Value

    let private cellInitialise pos =
        if not (cellValues.ContainsKey pos) then
            cellValues <- cellValues.Add(pos,CellValue.Create(""))

    let private cellSet pos value =
        cellInitialise pos
        cellValues.[pos].Set(value)

    let private cellCollectCells pos  =
        let mutable cells : Map<Position,int> = Map [ (pos,1) ]
        let rec f p =
            cellOutputs.TryFind p
            |> Option.defaultValue [| |]
            |> Array.iter (fun outCell ->
                //if not (cells.Contains outCell) then
                cells <- cells.Add(outCell,1)
                f outCell
            )
        f pos
        cells |> Map.keys |> Seq.toArray

    let private cellNotify pos dispatch  =
        let cells = cellCollectCells pos
        log($"notify: {formatPositions cells}")
        cells |> Array.iter dispatch

    let private cellSetInputs (pos : Position) (inputs : Position list) =
        log($"Cell {formatPosition pos} is dependent on {formatPositions inputs}")

        cellInputs <- cellInputs.Add( pos, inputs |> Array.ofList )

        // Build outputs. FIXME: Never removes an output
        inputs |> List.iter (fun inputPos ->
            let outs = cellOutputs.TryFind inputPos |> Option.defaultValue [| |]
            let outs' =
                if outs |> Array.contains pos then outs else (outs |> Array.append [| pos |])
            cellOutputs <- cellOutputs.Add( inputPos, outs' )
            log($"Outputs for {formatPosition inputPos} -> {formatPositions outs'}")
        )

    let cellUpdate pos value dispatch =
        log ("----------------------------------------------------------")
        log($"updateValue {pos} = {value}")

        cellSetInputs pos (findTriggerCells value)
        cellSet pos value // Finally update the sheet
        cellNotify pos (dispatch << RefreshCell)

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

let sample_full = [
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

let sample_test = [
    ('B',2), "1"
    ('B',3), "1"
    ('B',4), "=B2 + B3"
    ('B',5), "=B3 + B4"
]

let sample = sample_full

let init data  =
    log("init()")

    data |> List.iter (fun (p,v) -> Cells.cellUpdate p v ignore)

    {  Rows = [1 .. 15]
       Cols = ['A'.. 'K' ]
       Active = None
       NeedsRefresh = None
    }, Cmd.batch (data |> List.map (fst >> RefreshCell >> Cmd.ofMsg))

let update (message : Message) (model : Model) : Model * Cmd<Message> =
    log $"{message}"
    match message with

    | RefreshCell p ->
        { model with NeedsRefresh = Some p }, Cmd.none //[fun d -> Cells.cellNotify p (d <<RefreshCell)]

    | UpdateValue (p,v) ->
        { model with Active = None; NeedsRefresh = None }, [Cells.cellUpdate p v]

    | StartEdit p ->
        { model with Active = Some p; NeedsRefresh = None }, []

let renderCellAt (renderfn: Position -> SutilElement) (ctx : BuildContext) (cell:Position) =
    let cellElement = cell |> renderfn
    Program.mount( nodeOfCell cell, cellElement ) |> ignore

let view () : SutilElement =

    let model, dispatch = sample |> Store.makeElmish init update ignore

    let renderPlainCell pos =
        let content = Cells.cellGet pos
        fragment [
            onClick (fun _ -> StartEdit pos |> dispatch) []
            evalCellAsString Cells.cellGet content |> text
        ]

    let renderActiveCell pos =
        let content = Cells.cellGet pos
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

        // Respond to change in dimensions of spreadsheet
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
                                Attr.custom("x-id", positionStr (col,row) )
                                renderPlainCell (col,row)
                            ]
                        ) |> fragment
                    ])
                )

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

            CoreElements.subscribe activeS <| fun ctx (value,next) ->
                value |> Option.iter (renderCellAt renderPlainCell  ctx)
                next  |> Option.iter (renderCellAt renderActiveCell ctx)

            // model.Refresh (Potiion option) indicates which cell is requesting a refresh (re-evaluation)
            // the observable stream strips out Option.None to leave only a stream of Positions
            let refreshS =  (model |> Observable.map (fun m -> m.NeedsRefresh) |> filterSome)

            CoreElements.subscribe refreshS (renderCellAt renderPlainCell)
        ])
    ] |> withStyle styleSheet
