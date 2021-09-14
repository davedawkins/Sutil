module DataSim

open Sutil
open Sutil.Attr

type Record = {
    Name : string
    Count : int
}

let sampleNames = [ "Alice"; "Bob"; "Claire"; "Dan"; "Emily"; "Francis"; "Greta"; "Harry"; "Isobel"; "Jack"; "Kirsten"; "Larry" ]
let sampleRecords =
    sampleNames |> List.map (fun name -> { Name = name; Count = name.Length })

let randomInt min max =
    min + int(System.Math.Round(Interop.random() * float(max-min)))

let randomSign(n:float)=
    n * if randomInt 0 1 = 0 then 1.0 else -1.0

let updateRecord (r : Record) =
    { r with Count = r.Count + int(randomSign(1.0)) }

let updateStock (r : Sutil.SampleData.Stock) =
    { r with Price = r.Price * (1.0 + randomSign(0.01)) }

let view() =

    let numbers = DataSimulation.Random()

    let ints_25_75 = DataSimulation.Random(100,999,500)

    let ints_1_10 = DataSimulation.Random(10)

    let count = DataSimulation.Count(1,10,450)

    let list = DataSimulation.CountList(20,30,900)

    let records = DataSimulation.Records(sampleRecords,updateRecord,1000)
    let stocks = DataSimulation.Records(Sutil.SampleData.sampleStocks(10),updateStock,5,false,1000)

    Html.div [
        DOM.disposeOnUnmount [ numbers; ints_25_75; ints_1_10; count; list; records ]

        Attr.style [ Css.displayFlex; Css.flexDirectionRow ]

        Html.div [
            Attr.style [
                Css.minWidth (Feliz.length.percent 25)
            ]
            Bind.fragment numbers (fun n -> sprintf "%f" n |> Html.div)
            Bind.fragment ints_25_75 (fun n -> sprintf "%d" n |> Html.div)
            Bind.fragment ints_1_10 (fun n -> sprintf "%d" n |> Html.div)
            Bind.fragment numbers (fun n -> sprintf "%f" n |> Html.div)
            Bind.fragment count (fun n -> sprintf "%d" n |> Html.div)
        ]

        Html.div [
            Attr.style [
                Css.minWidth (Feliz.length.percent 25)
            ]
            Html.ul [
                Bind.each(list,Html.li)
            ]
        ]

        Html.div [
            Attr.style [
                Css.minWidth (Feliz.length.percent 25)
            ]
            Html.table [
                class' "table"
                Html.tbody [
                    Bind.each(records,fun r ->
                        Html.tr [
                            Html.td r.Name
                            Html.td r.Count
                        ]
                    )
        ] ] ]

        Html.div [
            Attr.style [
                Css.minWidth (Feliz.length.percent 25)
            ]
            Html.table [
                class' "table"
                Html.tbody [
                    Bind.each(stocks,fun r ->
                        Html.tr [
                            Html.td r.Symbol
                            Html.td (sprintf "%-5.2f" r.Price)
                        ]
                    )
        ] ] ]

    ]
