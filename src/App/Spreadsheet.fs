module Spreadsheet

// Based on http://tomasp.net/blog/2018/write-your-own-excel/

open Feliz
open type Feliz.length
open type Feliz.borderStyle
open Sutil
open Sutil.Styling
open Sutil.Attr
open Sutil.DOM
open Fable.Core.JsInterop
open Evaluator

type Cell = string

type Sheet = Map<Position,Cell>

type Message =
  | UpdateValue of Position * string
  | StartEdit of Position

type Model =
  { Rows : int list
    Cols : char list
    Active : Position option
    Cells : Sheet }

let styleSheet = [
    rule "table" [
        Css.borderSpacing zero
        Css.borderBottom( px 1, solid, "#e0e0e0")
        Css.borderRight( px 1, solid, "#e0e0e0")
    ]
    rule "td, th" [
        Css.minWidth "50px"
        Css.borderLeft( px 1, solid, "#e0e0e0" )
        Css.borderTop( px 1, solid, "#e0e0e0" )
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
]


let sample =
    let c s = s // { Value = s; Evaluated = ""; NeedsEval = true }
    Map.ofList [
        ('B',1), c "Fibonacci"
        ('B',2), c "1"
        ('B',3), c "1"
        ('B',4), c "=B2 + B3"
        ('B',5), c "=B3 + B4"
        ('B',6), c "=B4 + B5"
        ('B',7), c "=B5 + B6"
        ('B',8), c "=B6 + B7"
        ('B',9), c "=B7 + B8"
        ('E',3), c "Convert:" ; ('F',3), c "0"                ; ('G',3), c "°C";
        ('E',4), c "Result:"  ; ('F',4), c "=32 + F3 * 9 / 5" ; ('G',4), c "°F" ]

let init() =
    {  Rows = [1 .. 15]
       Cols = ['A'.. 'K' ]
       Active = None
       Cells = sample }

let update (message : Message) (model : Model) : Model =
    match message with
    | UpdateValue (p,v) -> { model with Cells = model.Cells.Add(p,v); Active = None }
    | StartEdit p -> { model with Active = Some p }

let makeStore = Store.makeElmishSimple init update ignore

let cellValue (sheet:Sheet) (pos:Position) = sheet.[pos]

let renderCell m dispatch pos =
    let content = m.Cells.TryFind pos |> Option.defaultValue ""
    if Some pos = m.Active then
        Html.div [
            Html.input [
                type' "text"
                value content
                autofocus
                onKeyDown (fun me -> if me.key = "Enter" then (pos,me.target?value) |> UpdateValue |> dispatch) []
            ]
        ]
    else
        fragment [
            onClick (fun _ -> StartEdit pos |> dispatch) []
            evalCellAsString (cellValue m.Cells) content |> text
        ]

let view () : NodeFactory =
    let model, dispatch = makeStore()

    Html.div [
        Bind.fragment model <| fun m -> Html.table [

            Html.thead [
                Html.tr [
                    Html.th [ text "" ]
                    m.Cols |> List.map (fun col -> Html.th [ col |> string |> text ]) |> fragment
                ]
            ]

            Html.tbody
                (m.Rows |> List.map (fun row ->
                    Html.tr [
                        Html.td [ row |> string |> text ]
                        m.Cols |> List.map (fun col ->
                            Html.td [ renderCell m dispatch (col,row) ]
                        ) |> fragment
                    ])
                )
        ]
    ] |> withStyle styleSheet
