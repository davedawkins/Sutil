module CRUD

open Sutil
open Feliz
open Sutil.DOM
open Sutil.Attr
open Sutil.Bulma

module DbSchema =
    type Name = {
        Id : int
        Name : string
        Surname : string
    }

[<RequireQualifiedAccess>]
module Db =
    open DbSchema

    let initDb = [
        { Id = 1; Name = "Hans"; Surname = "Emil" }
        { Id = 2; Name = "Max"; Surname = "Mustermann" }
        { Id = 3; Name = "Roman"; Surname = "Tisch" }
        { Id = 4; Name = "Steve"; Surname = "Miller" }
    ]

    let mutable nextId = 1 + (initDb |> List.map (fun n -> n.Id) |> List.max)
    let mutable db = initDb

    let fetchAll() : list<Name> = db

    let assertExists id =
        db |> List.findIndex (fun p -> p.Id = id) |> ignore

    let removeId id =
        assertExists id
        db <- db |> List.filter (fun p -> p.Id <> id)
        Browser.Dom.console.log($"{db}")
        id

    let create (p : Name) =
        if p.Name = "" && p.Surname = "" then failwith "Invalid name"
        let p' = { p with Id = nextId }
        nextId <- nextId + 1
        db <- db @ [ p' ]
        p'

    let update (p : Name) =
        assertExists p.Id
        db <- db |> List.map (fun x -> if x.Id = p.Id then p else x)
        Browser.Dom.console.log($"{db}")
        p

open Sutil.Styling
open System
open DbSchema

type Model = {
    Names : Name list;
    Name : string
    Surname : string
    Filter : string
    Selected : Name option
    Error : string
}

type Message =
    | Create
    | Update
    | Delete
    | Created of Name
    | Updated of Name
    | Deleted of int
    | SetFilter of string
    | SetName of string
    | SetSurname of string
    | RequestAllNames
    | AllNames of Name list
    | Select of id:int
    | ClearSelection
    | Error of string
    | Exception of Exception
    | ClearError

let filter m = m.Filter
let name m = m.Name
let surname m = m.Surname
let error m = m.Error
let names m = m.Names

let selection m = match m.Selected with |None->[] |Some n -> List.singleton n.Id
let matchName filter (name : Name) = filter = "" || name.Surname.StartsWith(filter)
let filteredNames m = m.Names |> List.filter (matchName m.Filter)

let canCreate m = m.Name <> "" || m.Surname <> ""
let canUpdate m = m.Selected.IsSome && (m.Name <> "" || m.Surname <> "")
let canDelete m = m.Selected.IsSome

let init () =
    { Names = []; Selected = None; Filter = ""; Name = ""; Surname = ""; Error = "" }, Cmd.ofMsg RequestAllNames

let update msg model =
    match msg with
    | ClearError ->
        { model with Error = "" }, Cmd.none
    | Error msg ->
        { model with Error = msg}, Cmd.none
    | Exception x ->
        { model with Error = x.Message}, Cmd.none
    | Created n ->
        { model with Names = model.Names @ [n] }, Cmd.batch [ Cmd.ofMsg ClearError; Cmd.ofMsg (Select n.Id) ]
    | Updated n ->
        let updatedNames = model.Names |> List.map (fun x -> if x.Id = n.Id then n else x)
        { model with Names = updatedNames }, Cmd.ofMsg ClearError
    | Deleted id ->
        let updatedNames = model.Names |> List.filter (fun x -> x.Id <> id)
        let selnMsg = if updatedNames.IsEmpty then ClearSelection else Select (updatedNames.Head.Id)
        { model with Names = updatedNames }, Cmd.batch [ Cmd.ofMsg ClearError; Cmd.ofMsg selnMsg ]
    | ClearSelection ->
        { model with Name = ""; Surname = ""; Selected = None}, Cmd.none
    | Select id ->
        let n = model.Names |> List.find (fun n -> n.Id = id)
        { model with Selected = Some n }, Cmd.batch [ Cmd.ofMsg (SetName n.Name); Cmd.ofMsg (SetSurname n.Surname)]
    | Create ->
        let n = { Name = model.Name; Surname = model.Surname; Id = 0 }
        model, Cmd.OfFunc.either Db.create n Created Exception
    | Update ->
        match model.Selected with
        | None -> model, Cmd.ofMsg (Error "No record selected to update")
        | Some n -> model, Cmd.OfFunc.either Db.update { n with Name = model.Name; Surname = model.Surname } Updated Exception
    | Delete  ->
        match model.Selected with
        | None -> model, Cmd.ofMsg (Error "No record selected to delete")
        | Some n -> model, Cmd.OfFunc.either Db.removeId n.Id Deleted Exception
    | SetFilter f ->
        { model with Filter = f }, Cmd.ofMsg ClearSelection
    | SetName n ->
        { model with Name = n }, Cmd.none
    | SetSurname n ->
        { model with Surname = n }, Cmd.none
    | RequestAllNames ->
        model, Cmd.OfFunc.perform Db.fetchAll () AllNames
    | AllNames names ->
        { model with Names = names }, Cmd.none


let appStyle = [
    rule "div.select, select, .width100" [
        Css.width (length.percent 100) // Streatch list and text box to fit column, looks nicer right aligned
    ]
    rule ".field-label" [
        Css.flexGrow 2 // Allow more space for field label
    ]
    rule "label.label" [
        Css.textAlignLeft // To match 7GUI spec
    ]
]

let view() =
    let model, dispatch = () |> Store.makeElmish init update ignore

    let labeledField label model dispatch =
        bulma.field.div [
            field.isHorizontal
            bulma.fieldLabel [ bulma.label [ DOM.text label ] ]
            bulma.fieldBody [
                bulma.control.div [
                    class' "width100"
                    bulma.input.text [
                        Attr.value (model,dispatch)
                    ]]]]

    let button label enabled message =
        bulma.control.p [
            bulma.button.button [
                Attr.disabled (model .> (enabled >> not))
                DOM.text label
                onClick (fun _ -> dispatch message) []
                ] ]

    bulma.container [
        bulma.columns [
            bulma.column [
                column.is6
                labeledField "Filter prefix:" (model .> filter) (dispatch << SetFilter)
            ]
        ]

        bulma.columns [
            bulma.column [
                column.is6

                Sutil.Bulma.Helpers.selectList [ // FIXME: Feliz.BulmaEngine should provide this
                    Attr.size 6

                    let viewNames =
                        model .> filteredNames |> Observable.distinctUntilChanged

                    Bind.each(viewNames,(fun n ->
                        Html.option [
                            Attr.value n.Id
                            (sprintf "%s, %s" n.Surname n.Name) |> DOM.text
                            ]))

                    Bind.selected (model .> selection, List.exactlyOne >> Select >> dispatch)
                ]
            ]
            bulma.column [
                column.is6
                labeledField "Name:" (model .> name) (dispatch << SetName)
                labeledField "Surname:" (model .> surname) (dispatch << SetSurname)
            ]
        ]

        bulma.field.div [
            field.isGrouped
            button "Create" canCreate Create
            button "Update" canUpdate Update
            button "Delete" canDelete Delete
        ]

        bulma.field.div [
            color.hasTextDanger
            Bind.el (model .> error, DOM.text)
        ]

    ] |> withStyle appStyle

view() |> Program.mountElement "sutil-app"
