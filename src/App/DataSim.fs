module DataSim

open Sutil

open Sutil.CoreElements

type Record = {
    Name : string
    Count : int
}

let sampleNames = [ "Alice"; "Bob"; "Claire"; "Dan"; "Emily"; "Francis"; "Greta"; "Harry"; "Isobel"; "Jack"; "Kirsten"; "Larry" ]
let sampleRecords =
    sampleNames |> List.map (fun name -> { Name = name; Count = name.Length })

let updateRecord (r : Record) =
    { r with Count = r.Count + int(Random.randomSign(1.0)) }

let view() =

    let numbers = DataSimulation.Random()

    let ints_25_75 = DataSimulation.Random(100,999,500)

    let ints_1_10 = DataSimulation.Random(10)

    let count = DataSimulation.Count(1,10,450)

    let list = DataSimulation.CountList(20,30,900)

    let records = DataSimulation.Records(sampleRecords,updateRecord,1000)
    let stocks = SampleData.stockFeed 10 1000

    Html.div [
        disposeOnUnmount [ numbers; ints_25_75; ints_1_10; count; list; records; stocks ]

        Attr.style [ Css.displayFlex; Css.flexDirectionRow ]

        Html.div [
            Attr.style [
                Css.minWidth (Feliz.length.percent 25)
            ]
            Bind.el(numbers,fun n -> sprintf "%f" n |> Html.div)
            Bind.el(ints_25_75, fun n -> sprintf "%d" n |> Html.div)
            Bind.el(ints_1_10, fun n -> sprintf "%d" n |> Html.div)
            Bind.el(numbers, fun n -> sprintf "%f" n |> Html.div)
            Bind.el(count, fun n -> sprintf "%d" n |> Html.div)
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
                    Bind.each(stocks,fun (r:SampleData.Stock) ->
                        Html.tr [
                            Html.td r.Symbol
                            Html.td (sprintf "%-5.2f" r.Price)
                        ]
                    )
        ] ] ]

    ]
