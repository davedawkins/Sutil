module SevenGuisCells

// Based on http://tomasp.net/blog/2018/write-your-own-excel/
// Modified to only recalculate cells that are dependent, according to 7Guis specification
//
// Still being refactored. Gets better on each pass
//
open Sveltish
open Sveltish.Styling
open Sveltish.Attr
open Sveltish.DOM
open Sveltish.Bindings
open Fable.Core.JsInterop
open Evaluator
open Browser.Dom
open Browser.Types
open System

let log s = ()
let filterSome (source : IObservable<'T option>) =
    source |> Observable.filter (fun x -> x.IsSome) |> Observable.map (fun x -> x.Value)
let elementOfCell pos = document.querySelector($"[x-id='{positionStr pos}'") :?> HTMLElement

type Sheet = Map<Position, Sveltish.IStore<string> >

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
let mutable Cells : Sheet = Map.empty

let valAt pos =
    match Cells.TryFind pos with
    | None -> ""
    | Some s -> s.Value

let cellSet pos value =
    Cells.[pos] <~ value

let cellListen toPos dispatch =
    let store = Cells.[toPos]
    let unsub = Store.subscribe store <| fun value ->
        log($"Cell {toPos} was updated to {value}")
        dispatch value
    ()

let cellNotify pos =
    valAt pos |> cellSet pos

let bindCellRefresh pos whenPosUpdated dispatch =
    cellListen whenPosUpdated (fun _ -> pos |> dispatch)

let bindRefresh targetCell dependsOnCells dispatch =
    for dep in dependsOnCells do
        bindCellRefresh targetCell dep dispatch

let cellInitialise (dispatch : Position -> unit) pos =
    if not (Cells.ContainsKey pos) then
        log($"create cell store {pos}")
        Cells <- Cells.Add(pos,Store.make "")

type Model =
  { Rows : int list
    Cols : char list
    Active : Position option
    NeedsRefresh : Position option }

let rows m = m.Rows
let cols m = m.Cols

let styleSheet = [
    rule "table" [
        borderSpacing "0px"
        borderBottom "1px solid #e0e0e0"
        borderRight "1px solid #e0e0e0"
    ]
    rule "td, th" [
        minWidth "50px"
        borderLeft "1px solid #e0e0e0"
        borderTop "1px solid #e0e0e0"
        padding "5px"
    ]
    rule "td.selected" [
        padding "0px"
    ]
    rule "td div" [
        display "flex"
        flexDirection "row"
    ]
    rule "td input" [
        flex "1"
        width "56px"
        height "22px"
    ]
    rule "td.active" [
        backgroundColor "red"
    ]
]

let renderPlainCell dispatch pos =
    let content = valAt pos
    log($"rendering cell {pos} '{content}'")
    fragment [
        onClick (fun _ -> StartEdit pos |> dispatch) []
        evalCellAsString valAt content |> text
    ]

let renderActiveCell dispatch pos =
    let content = valAt pos
    Html.div [
        Html.input [
            type' "text"
            value content
            autofocus
            onKeyDown (fun me -> if me.key = "Enter" then (pos,me.target?value) |> UpdateValue |> dispatch) []
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
    {  Rows = [1 .. 15]
       Cols = ['A'.. 'K' ]
       Active = None
       NeedsRefresh = None }, Cmd.batch (sample |> List.map (UpdateValue >> Cmd.ofMsg))

let updateValue pos value dispatch =
    let tcells = findTriggerCells value // Examine expression for cells pos is dependent on
    let refresh = (dispatch << RefreshCell)
    (pos :: tcells) |> List.iter (cellInitialise refresh) // Make sure all cells involved have stores
    bindRefresh pos tcells refresh // Make sure pos gets a refresh when any of tcells update
    cellSet pos value // Finally update the sheet
    refresh pos

let update (message : Message) (model : Model) : Model * Cmd<Message> =
    log $"{message}"
    match message with

    | RefreshCell p ->
        { model with NeedsRefresh = Some p }, [fun _ -> cellNotify p]

    | UpdateValue (p,v) ->
        { model with Active = None; NeedsRefresh = None }, [updateValue p v]

    | StartEdit p ->
        { model with Active = Some p; NeedsRefresh = None }, []

let makeStore = Store.makeElmish init update ignore

//
// Render a NodeFctory at a given cell
//
let renderCellAt (renderfn: Position -> NodeFactory) (ctx : BuildContext) (cell:Position) =
    let nodeFactory = renderfn >> exclusive
    (nodeFactory cell)(ctx, (elementOfCell cell) :> Node) |> ignore
    ()

//
// Listen for cells that want a refresh, and render them with 'renderPlainCell'
let bindRefreshCell (cellSource : IObservable<Position>) dispatch : NodeFactory = fun (ctx,_) ->
    let unsub = Store.subscribe cellSource (renderCellAt (renderPlainCell dispatch) ctx)
    unitResult()

//
// Listen for changes to active cell and render previous active cell with plain, and new with active
let bindActiveCell (activeSource : IObservable<Position option*Position option>) dispatch : NodeFactory = fun (ctx,_) ->
    let unsub = Store.subscribe activeSource <| fun (value,next) ->
        value |> Option.iter (renderCellAt (renderPlainCell  dispatch) ctx)
        next  |> Option.iter (renderCellAt (renderActiveCell dispatch) ctx)
    unitResult()

let view () : NodeFactory =
    let model, dispatch = makeStore()

    Html.div [
        bind2e (model .> rows) (model .> cols) <| fun (rows,cols) -> Html.table [
            //let m = model.Value
            do log("Render table")

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
                                renderPlainCell dispatch (col,row)
                            ]
                        ) |> fragment
                    ])
                )

            log("Installing bindings for active cell")
            bindActiveCell (model |> Observable.map (fun m -> m.Active) |> Observable.pairwise) dispatch
            bindRefreshCell (model |> Observable.map (fun m -> m.NeedsRefresh) |> filterSome) dispatch
        ]
    ] |> withStyle styleSheet
