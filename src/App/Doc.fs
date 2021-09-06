module Doc

open Sutil
open type Feliz.length
open Sutil.DOM
open Fetch
open Sutil.Styling
open Types
open Fable.Formatting.Markdown

let parsed md =
    try
        let doc = Markdown.Parse(md)
        let html = Markdown.ToHtml(doc)
        html
    with
        | x -> $"<pre>{x}</pre>"

let urlBase = ""//"https://raw.githubusercontent.com/davedawkins/Sutil/main/src/App"

let fetchSource tab  =
    //let url = sprintf "%s%s" urlBase tab
    fetch tab []
    |> Promise.bind (fun res -> res.text())

let view (src : string) () =
    Html.div [
        Html.div [
            html $"{parsed src}"
        ] |> withStyle Markdown.style
    ]

type FoldType = {
    Category : string
    Pages : Page list
}

let pageView title source () =
    let content = Store.make "Loading.."

    fetchSource ("doc/" + source) |> Promise.map (Store.set content) |> ignore

    Html.div [
        disposeOnUnmount [ content ]
        Html.h2 [ text title ]
        Html.div [
            Html.span [
                Bind.fragment content <| fun t ->
                    html $"{parsed t}"
            ] |> withStyle Markdown.style
        ]
    ]

// Not the worst parser you've ever seen, but pretty close
let parseLink (src : string)=
    let items = src.Replace("[","").Replace("(","").Replace(")","").Split(']')
    items.[0].Trim(), items.[1].Trim()

let parseIndex (src:string) =
    let foldFn accum (line : string) =
        match line.[0] with
        | '#' -> { accum with FoldType.Category = line.[1..] }
        | '-' ->
            let title, pageSrc = parseLink line.[1..]
            let page = {
                    Category = accum.Category;
                    Title = title;
                    Create = pageView title pageSrc
                    Sections = [] }
            { accum with Pages = accum.Pages @ [ page ] }
        | _ -> accum
    let pages =
        src.Split('\n')
        |> Array.filter (fun s -> s <> "")
        |> Array.fold foldFn { Category = ""; Pages = [] }

    {
        Title = "Documentation";
        Pages = pages.Pages
    }

let getBook() =
        promise {
            let! indexSrc = fetchSource "doc/index.md"
            let b = parseIndex indexSrc
            return b
        }

