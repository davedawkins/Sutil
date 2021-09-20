module Doc

open Sutil
open type Feliz.length
open Sutil.DOM
open Fetch
open Sutil.Styling
open Types
open Fable.Formatting.Markdown
open Fable.Core.Util
open Fable.Core
open Browser.Types
open Browser.CssExtensions
open Browser.Dom

[<ImportAll("./highlight.min.js")>]
let hljs : obj = jsNative;
type ILzString =
    abstract member compressToEncodedURIComponent : string -> string

[<ImportAll("lz-string")>]
let lzString : ILzString = jsNative;

let inline toEl (node : Browser.Types.Node) = node :?> Browser.Types.HTMLElement

let indexHtml = """<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="preload" href="https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css" as="style">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css">
    <title>Sutil</title>
    <style>
      </style>
</head>
<body>
    <div id="sutil-app"></div>
</body>
</html>
"""

let styleCs = """
    body {
      margin: 12px;
    }
"""

let openSutil = """open Sutil
open Sutil.Styling
open Sutil.DOM
open Sutil.Attr
open Feliz"""

let buildReplQuery (names : string array) (codes : string array) (html:string) (css:string) =
    let queryData : Map<string,string> =
        Map.empty
            .Add("names", lzString.compressToEncodedURIComponent(JS.JSON.stringify(names)))
            .Add("codes", lzString.compressToEncodedURIComponent(JS.JSON.stringify(codes)))
            .Add("html",  lzString.compressToEncodedURIComponent(html))
            .Add("css",   lzString.compressToEncodedURIComponent(css))

    queryData
        |> Map.toList
        |> List.map (fun (k,v) -> k + "=" + v)
        |> String.concat "&"

//JS.console.dir(hljs)

let parsed md =
    try
        let doc  = Markdown.Parse(md)
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

//
// Builds the element and passes to post-processing function
//
let postProcess (f : Browser.Types.HTMLElement -> unit) (view : SutilElement) : SutilElement = nodeFactory <| fun ctx ->
    let result = DOM.build view ctx
    ctx.Parent.AsDomNode |> applyIfElement f
    result

//
// Create a seq<'T> from a NodeListOF<'T>
//
let seqOfNodeList<'T> (nodes: Browser.Types.NodeListOf<'T>) =
    seq {
        for i in [0..nodes.length-1] do
            yield nodes.[i]
    }

//
// Find all descendant nodes that match the given selector
//
let querySelectorAll selector (node : Browser.Types.HTMLElement) =
    node.querySelectorAll(selector) |> seqOfNodeList

//
// Handle //[no]repl directives
//
let processReplDirectives (preCode : Browser.Types.Element) : bool =
    let cmd (e : Browser.Types.HTMLElement) =
        match e.innerText with
        | "//norepl" ->
            e.parentNode.removeChild(e) |> ignore
            false
        |_ ->
            true

    preCode.querySelectorAll("span.hljs-comment")
    |> seqOfNodeList
    |> Seq.filter (fun e -> (toEl e).innerText.Contains "repl")
    |> Seq.map (cmd << toEl)
    |> Seq.contains false
    |> not
//
// Find all "<pre><code>...</code></pre>" blocks
//
let findPreCode (node : Browser.Types.HTMLElement) =
    node |> querySelectorAll "pre code.language-fsharp"

let makeExample (code:string) =
    let codeWithOpens (c:string) =
        if c.Contains("open Sutil") then c else openSutil + "\n\n" + c

    let codeWithMount (c:string) =
        if c.Contains("mountElement") then c
        else
            c + " |> Program.mountElement \"sutil-app\""

    code.TrimEnd() |> codeWithOpens |> codeWithMount
//
// Make an "Open in REPL" button for the given code example
//
let replButton (wantExpandButton : bool) (code : Browser.Types.HTMLElement) =
    Html.span [
        Html.a [
            Sutil.Attr.style [ Css.fontSize (Feliz.length.percent 75)]
            text "Open in REPL"
            Sutil.Attr.onClick (fun _ ->
                let q =
                    buildReplQuery
                        [| "Main.fs" |]
                        [| makeExample code.innerText |]
                        indexHtml
                        styleCs
                Browser.Dom.window.location.href <- "https://sutil.dev/repl/#?" + q
            ) []
        ]
        if wantExpandButton then
            let expanded = Store.make false
            Bind.el expanded (fun isExpanded ->
                Html.a [
                    Sutil.Attr.style [
                        Css.fontSize (Feliz.length.percent 75)
                        Css.marginLeft (Feliz.length.rem 0.5)
                        ]
                    text (if isExpanded then "Collapse" else "Expand")
                    Sutil.Attr.onClick (fun _ ->
                        if isExpanded then
                            code.classList.add("more")
                            code.classList.remove("full")
                        else
                            code.classList.add("full")
                            code.classList.remove("more")
                        expanded |> Store.modify not
                    ) []

                    disposeOnUnmount [expanded]
                ])
    ]
//
// Append a REPL button immediately after the <pre><code> block
//
let addReplButton (preCode : Browser.Types.HTMLElement) =
    let wantExpandButton = preCode.clientHeight > 182.0
    if wantExpandButton then
        preCode.classList.add( "more" )
    mountAfter (replButton wantExpandButton preCode) (preCode.parentElement)

//
// Add "Open in REPL" buttons to all <pre><code> example code blocks
//
let addReplButtons (markdown : Browser.Types.HTMLElement) =
    markdown
        |> findPreCode
        |> Seq.filter processReplDirectives
        |> Seq.iter (toEl >> addReplButton >> ignore)

let pageView title source () =
    let content = Store.make "Loading.."

    fetchSource ("doc/" + source) |> Promise.map (Store.set content) |> ignore

    Html.div [
        disposeOnUnmount [ content ]
        Html.h2 [ text title ]
        Html.div [
            Html.span [
                Bind.el content <| fun t ->
                    html $"{parsed t}"
                        |> postProcess addReplButtons
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

