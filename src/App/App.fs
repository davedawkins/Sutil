module App

open Fetch

open Feliz
open Sutil
open Sutil.Attr
open Sutil.Styling
open Sutil.DOM
open Sutil.Transition
open Browser.Types
open Types

open type Feliz.length

//
// Books
//   Chapters (Categories)
//     Pages  (Examples)
//       Sections (Sources)
//
// Books are displayed as horizontal tab menu in header (heading-type font). Eg "Examples", "Documentation"
// Chapters (categories) are headings in the left sidebar (conntents panel)
// Pages are selectable list items under each chapter
// Sections are displayed in header of the page view, as a horizontal tab menu (small font)
//
// Clicking on a book changes the view to that book's chapters and pages, and shows the default page and section
// Clicking on a page changes the view to that page's default section
//
// By convention the main page view is section "", or a section with the same name as the page title
// Other sections are alternate views in the context of that page. For an Example page, these are the source
// files for the example.
//
// For example, HelloWorld example looks like this, with two sections.
//
//     [Hello World]  [HelloWorld.fs]
//
// Click on first section to see the example
// CLick on second section to see the source
//
// A URL for the HelloWorld example would look like this:
//
//     http://host/#examples-hello-world
//
// For the source file
//     http://host/#examples-hello-world?HelloWorld.fs
//


let allExamples = [
        { Category = "Introduction";Title = "Hello World";  Create = HelloWorld.view ; Sections = ["HelloWorld.fs"]}
        { Category = "Introduction";Title = "Dynamic attributes";  Create = DynamicAttributes.view ; Sections = ["DynamicAttributes.fs"]}
        { Category = "Introduction";Title = "Styling";  Create = StylingExample.view ; Sections = ["Styling.fs"]}
        { Category = "Introduction";Title = "Nested components";  Create = NestedComponents.view ; Sections = ["NestedComponents.fs"; "Nested.fs"]}
        { Category = "Introduction";Title = "HTML tags";  Create = HtmlTags.view ; Sections = ["HtmlTags.fs"]}
        { Category = "Reactivity";Title = "Reactive assignments";  Create = Counter.view ; Sections = ["Counter.fs"]}
        { Category = "Reactivity";Title = "Reactive declarations";  Create = ReactiveDeclarations.view ; Sections = ["ReactiveDeclarations.fs"]}
        { Category = "Reactivity";Title = "Reactive statements";  Create = ReactiveStatements.view ; Sections = ["ReactiveStatements.fs"]}
        { Category = "Logic"; Title = "If blocks"; Create = LogicIf.view; Sections = ["LogicIf.fs"] }
        { Category = "Logic"; Title = "Else blocks"; Create = LogicElse.view; Sections = ["LogicElse.fs"] }
        { Category = "Logic"; Title = "Else-if blocks"; Create = LogicElseIf.view; Sections = ["LogicElseIf.fs"] }
        { Category = "Logic"; Title = "Static each blocks"; Create = StaticEachBlocks.view; Sections = ["StaticEach.fs"] }
        { Category = "Logic"; Title = "Static each with index"; Create = StaticEachWithIndex.view; Sections = ["StaticEachWithIndex.fs"] }
        { Category = "Logic"; Title = "Each blocks"; Create = EachBlocks.view; Sections = ["EachBlocks.fs"] }
        { Category = "Logic"; Title = "Keyed-each blocks"; Create = KeyedEachBlocks.view; Sections = ["KeyedEachBlocks.fs"] }
        { Category = "Logic"; Title = "Await blocks"; Create = AwaitBlocks.view; Sections = ["AwaitBlocks.fs"] }
        { Category = "Events"; Title = "DOM events"; Create = DomEvents.view; Sections = ["DomEvents.fs"] }
        { Category = "Events"; Title = "Custom events"; Create = CustomEvents.view; Sections = ["CustomEvents.fs"] }
        { Category = "Events"; Title = "Event modifiers"; Create = EventModifiers.view; Sections = ["EventModifiers.fs"] }
        { Category = "Transitions"; Title = "Transition"; Create = Transition.view; Sections = ["Transition.fs"] }
        { Category = "Transitions"; Title = "Adding parameters"; Create = TransitionParameters.view; Sections = ["TransitionParameters.fs"] }
        { Category = "Transitions"; Title = "In and out"; Create = TransitionInOut.view; Sections = ["TransitionInOut.fs"] }
        { Category = "Transitions"; Title = "Custom CSS"; Create = TransitionCustomCss.view; Sections = ["TransitionCustomCss.fs"] }
        { Category = "Transitions"; Title = "Custom Code"; Create = TransitionCustom.view; Sections = ["TransitionCustom.fs"] }
        { Category = "Transitions"; Title = "Transition events"; Create = TransitionEvents.view; Sections = ["TransitionEvents.fs"] }
        { Category = "Transitions"; Title = "Animation"; Create = Todos.view; Sections = ["Todos.fs"] }
        { Category = "Bindings";   Title = "Text inputs";  Create = TextInputs.view ; Sections = ["TextInputs.fs"]}
        { Category = "Bindings";   Title = "Numeric inputs";  Create = NumericInputs.view ; Sections = ["NumericInputs.fs"]}
        { Category = "Bindings";   Title = "Checkbox inputs";  Create = CheckboxInputs.view ; Sections = ["CheckboxInputs.fs"]}
        { Category = "Bindings";   Title = "Group inputs";  Create = GroupInputs.view ; Sections = ["GroupInputs.fs"]}
        { Category = "Bindings";   Title = "Textarea inputs";  Create = TextArea.view ; Sections = ["TextArea.fs"]}
        { Category = "Bindings";   Title = "File inputs";  Create = FileInputs.view ; Sections = ["FileInputs.fs"]}
        { Category = "Bindings";   Title = "Select bindings";  Create = SelectBindings.view ; Sections = ["SelectBindings.fs"]}
        { Category = "Bindings";   Title = "Select multiple";  Create = SelectMultiple.view ; Sections = ["SelectMultiple.fs"]}
        { Category = "Bindings";   Title = "Dimensions";  Create = Dimensions.view ; Sections = ["Dimensions.fs"]}
        { Category = "Svg";   Title = "Bar chart";  Create = BarChart.view ; Sections = ["BarChart.fs"]}
        { Category = "Miscellaneous";   Title = "Spreadsheet";  Create = Spreadsheet.view ; Sections = ["Spreadsheet.fs"; "Evaluator.fs"; "Parser.fs"]}
        { Category = "Miscellaneous";   Title = "Modal";  Create = Modal.view ; Sections = ["Modal.fs"]}
        { Category = "Miscellaneous";   Title = "Login";  Create = LoginExample.view ; Sections = ["LoginExample.fs"; "Login.fs"]}
        { Category = "Miscellaneous";   Title = "Drag-sortable list";  Create = SortableTimerList.view ; Sections = ["SortableTimerList.fs"; "DragDropListSort.fs"; "TimerWithButton.fs"; "TimerLogic.fs"]}
        { Category = "Miscellaneous";   Title = "SAFE client";  Create = SAFE.view ; Sections = ["SafeClient.fs"]}
        { Category = "Miscellaneous";   Title = "Data Simulation";  Create = DataSim.view ; Sections = ["DataSim.fs"]}
        //{ Category = "Miscellaneous";   Title = "Fragment";  Create = Fragment.view ; Sections = ["Fragment.fs"]}
        { Category = "7Guis";   Title = "Cells";  Create = SevenGuisCells.view ; Sections = ["Cells.fs"]}
        { Category = "7Guis";   Title = "CRUD";  Create = CRUD.view ; Sections = ["CRUD.fs"]}
    ]

let initBooks = [
    { Title = "Examples"; Pages = allExamples }
//    { Title = "Documentation"; Pages = allDocs }
]

let urlBase = "https://raw.githubusercontent.com/davedawkins/Sutil/main/src/App"

// View as specified by the URL
type PageView = {
    BookName : string
    PageName : string
    SectionName : string
}

type Model = {
    Source : string // For an "Example" Page, a Section is the name of a source file. Contents are fetched to Source
    ShowContents : bool
    IsMobile : bool
    Books : Book list
    CurrentBook : Book
    CurrentPage : Page
    CurrentSection : string
    PageView : PageView
}

let source m = m.Source
let books m = m.Books
let currentBook m = m.CurrentBook
let currentPage m = m.CurrentPage
let currentSection m = m.CurrentSection
let isMobile m = m.IsMobile
let showContents m = m.ShowContents

type Message =
    | SetSource of string
    | SetIsMobile of bool
    | AddBook of Book
    | SetPageView of PageView
    | ToggleShowContents

///
/// URL format
///  #book-page?section
///
let sanitize (title:string) = title.ToLower().Replace(" ", "-")
let makeHref book page section =
    "#" + (sanitize book + "-" + sanitize page + (if page = section || section = "" then "" else "?" + section))
let makeBookHref bk = makeHref bk.Title bk.defaultPage.Title ""
let findPage bk title = bk.Pages |> List.tryFind (fun p -> sanitize p.Title = sanitize title)
let defaultBook (books : Book list) = books.Head
let findBookPage (books : Book list) (pv : PageView) =
    let defBk = defaultBook books
    let bookP =
        books
            |> List.tryFind (fun b -> sanitize b.Title = sanitize pv.BookName)
            |> Option.map (fun bk -> bk, findPage bk pv.PageName)
    match bookP with
    | Some (bk, Some pg) -> bk, pg
    | Some (bk, None) -> bk, bk.defaultPage
    | _ -> defBk, defBk.defaultPage


let compareBook (a:Book) (b:Book) = (a.Title = b.Title)
let comparePage (a:Page) (b:Page) = (a.Title = b.Title)
let onBookChange source = source |> Observable.distinctUntilChangedCompare compareBook
let onPageChange source = source |> Observable.distinctUntilChangedCompare comparePage
let pageCategories (all : Page list) = all |> List.map (fun p -> p.Category) |> List.distinct

let fetchSource file dispatch =
    let url = sprintf "%s/%s" urlBase file
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (SetSource >> dispatch)
    |> ignore

let init() =
    let currentBook = defaultBook initBooks
    let currentPage = currentBook.defaultPage
    {
        Source = ""
        ShowContents = false
        IsMobile = false
        Books = initBooks
        CurrentBook = currentBook
        CurrentPage = currentPage
        CurrentSection = ""
        PageView = {
            BookName = currentBook.Title
            PageName = currentPage.Title
            SectionName = ""
        }
    }, []

let update msg model : Model * Cmd<Message> =
    Browser.Dom.console.log($"update {msg}")
    match msg with
    | SetIsMobile m ->
        { model with IsMobile = m }, Cmd.none

    | AddBook book ->
        { model with Books = book :: model.Books },
            Cmd.ofMsg (SetPageView model.PageView)
            // Cmd.ofMsg (SetPageView {
            //     BookName = model.CurrentBook.Title
            //     PageName = model.CurrentPage.Title
            //     SectionName = model.CurrentSection
            //     })

    | SetPageView pv ->
        let book, page = pv |> findBookPage model.Books
        let loadingString = "[ Loading ]"
        let section = if (page.Sections |> List.contains pv.SectionName) then pv.SectionName else ""

        // Fetch the source file for section. Non-empty section refers to source file for example
        // This may change!
        let cmd, src =
            if (section <> "" && section <> model.CurrentSection && model.Source <> loadingString) then
                [ fetchSource section ], loadingString
                //Cmd.none, loadingString
            else
                Cmd.none, ""
        { model with
            CurrentPage = page;
            CurrentBook = book;
            CurrentSection = section;
            ShowContents = false;
            PageView = pv
            Source = src} , cmd

    | ToggleShowContents ->
        { model with ShowContents = not model.ShowContents }, Cmd.none

    | SetSource content ->
        { model with Source = content }, Cmd.none

let mainStyleSheet = Bulma.withBulmaHelpers [

    rule ".app-main" [
        Css.height (percent 100)
    ]

    rule ".app-heading" [
        Css.displayFlex
        Css.flexDirectionRow
        Css.justifyContentSpaceBetween
        Css.positionFixed
        Css.width (length.vw 100)
        Css.backgroundColor "white"
        Css.paddingLeft (px 12)
        Css.paddingTop (px 6)
        Css.paddingBottom (px 6)
        Css.boxShadow "-0.4rem 0.01rem 0.3rem rgba(0,0,0,.5)"
        Css.marginBottom 4
        Css.zIndex 1   // Messes with .modal button
    ]

    rule ".app-heading h1" [
       Css.marginBottom 0
    ]

    rule ".app-contents" [
        Css.backgroundColor "#164460"//"#676778"
        Css.color "white"
        Css.overflowScroll
    ]

    rule ".app-contents ul" [
        Css.paddingLeft 20
    ]

    rule ".app-contents .title" [
        Css.color "white"
        Css.marginLeft 12
        Css.marginBottom 8
        Css.marginTop 16
    ]

    rule ".app-contents a" [
        Css.cursorPointer
        Css.color "white"
        Css.textDecorationNone
    ]

    rule ".app-contents a:hover" [
        Css.color "white"
        Css.textDecorationUnderline
    ]

    rule ".app-main-section" [
        Css.marginTop 0
        Css.paddingTop (px 44)
        Css.height (percent 100)
    ]

    rule ".app-page-section" [
        Css.displayFlex
        Css.flexDirectionColumn
        Css.height (percent 100)
    ]

    rule ".app-page" [
        Css.backgroundColor "white"
        Css.overflowYScroll
    ]

    rule ".app-heading a" [
        Css.color "#676778"
    ]

    rule ".app-toolbar a" [
        Css.color "#676778"
        Css.fontSize (percent 80)
        Css.padding 12
    ]

    rule ".app-toolbar ul" [
        Css.displayInlineElement
    ]

    rule ".app-toolbar li" [
        Css.displayInlineElement
    ]

    rule "pre" [
        Css.padding 0
        Css.backgroundColor "white"
    ]

    rule ".slogo" [
        Css.displayInlineFlex
        Css.fontFamily "'Coda Caption'"
        Css.alignItemsCenter
        Css.justifyContentCenter
        Css.width 32
        Css.height 24
        Css.backgroundColor "#444444"
        Css.color "white"
    ]

    rule ".show-contents-button" [
        Css.fontSize 18
    ]

    rule ".app-tab-menu a" [
        Css.marginRight 24
    ]
]

let Section tab (name:string) = fragment [
    Html.h5 [ class' "title is-6"; text (name.ToUpper()) ]
    Html.ul [
        for d in tab.Pages |> List.filter (fun x -> x.Category = name) do
            Html.li [
                Html.a [
                    Attr.href <| makeHref tab.Title  d.Title ""
                    text d.Title
                ]
            ]
        ]
    ]

let sectionItem tab (page:Page) name  =
    Html.li [
        Html.a [
            Attr.href <| makeHref tab.Title page.Title (if name = page.Title then "" else name)
            text name
        ]
    ]

let viewSource (model : IStore<Model>) =
    Html.div [
        Html.pre [
            Html.code [
                class' "language-fsharp"
                Bind.el (model .> source) (exclusive << text)
            ]
        ]
    ]

let viewPage page model =
    Html.div [
        class' "column app-page"
        Bind.el (model .> currentSection |> Observable.distinctUntilChanged) <| fun section ->
            match section with
            | "" ->
                try
                    page.Create()
                with
                    |x -> Html.div[ text $"Creating example {page.Title}: {x.Message}" ]
            | _ -> viewSource model
    ]

module UrlParser =
    let parseHash (location: Location) =
        let hash =
            if location.hash.Length > 1 then location.hash.Substring 1
            else ""
        if hash.Contains("?") then
            let h = hash.Substring(0, hash.IndexOf("?"))
            h, hash.Substring(h.Length+1)
        else
            hash, ""

    let parseUrl (location: Location) =
        parseHash location

    let parseBookPage (hash:string) =
        let items = hash.Split( [|'-'|], 2 )
        match items.Length with
        | 0 -> "", ""
        | 1 -> "", items.[0]
        | _ -> items.[0], items.[1]

    let parsePageView (loc:Location) : PageView =
        let hash, query = (parseUrl loc)
        let book, page = parseBookPage hash

        {
            BookName = book
            PageName = page
            SectionName = query
        }

let appMain () =
    let model, dispatch = () |> Store.makeElmish init update ignore

    let showContents = model .> (fun m -> not m.IsMobile || m.ShowContents)

    let umedia = MediaQuery.listenMedia "(max-width: 768px)" (dispatch << SetIsMobile)
    let upage  = Navigable.listenLocation UrlParser.parsePageView (dispatch << SetPageView)

    let currentBook = model .> currentBook |> onBookChange
    let currentPage = model .> currentPage |> onPageChange

    Doc.getBook() |> Promise.map (dispatch << AddBook) |> ignore

    Html.div [
        class' "app-main"

        unsubscribeOnUnmount [ umedia; upage ]
        disposeOnUnmount [ model ]

        Html.div [
            class' "app-heading"
            Html.a [
                Attr.href "https://sutil.dev"
                Html.img [
                    Attr.src "images/logo-wide.png"
                    Attr.style [ Css.height (px 25) ]
                    ]
            ]
            // Html.h1 [
            //     class' "title is-4"
            //     Html.a [
            //         Attr.href "https://github.com/davedawkins/Sutil"
            //         Html.div [ class' "slogo"; Html.span [ text "<>" ] ]
            //         text " SUTIL"
            //     ]
            // ]

            Bind.el (model .> books) <| fun books ->
                Html.span [
                    class' "app-tab-menu"
                    books |> List.map (fun bk -> Html.a [ Attr.href <| makeBookHref bk; text bk.Title ]) |> fragment
                ]

            transition [InOut fade] (model .> isMobile) <| Html.a [
                class' "show-contents-button"
                Attr.href "#"
                Html.i [ class' "fa fa-bars" ]
                onClick (fun _ -> ToggleShowContents |> dispatch) [ PreventDefault ]
            ]
        ]

        Bind.el currentBook <| fun tab ->
            Html.div [
                class' "columns app-main-section"

                transition [fly |> withProps [ Duration 500.0; X -500.0 ] |> In] showContents <|
                    Html.div [
                        class' "column is-one-quarter app-contents"

                        tab.Pages |> pageCategories |> List.map (fun title -> Section tab title) |> fragment
                    ]

                Bind.el currentPage <| fun page ->
                    Html.div [
                        class' "column app-page-section"

                        if (page.Sections <> []) then
                            Html.div [
                                class' "app-toolbar"
                                Html.ul [
                                    class' "app-tab"
                                    sectionItem tab page page.Title
                                    page.Sections |> List.map (sectionItem tab page) |> fragment
                                ]
                            ]

                        viewPage page model
                    ]
            ]
    ]

let app () =
    Html.app [
        // Page title
        headTitle "sutil"

        // Bulma style framework
        headStylesheet "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css"

        appMain() |> withStyle mainStyleSheet
    ]

let main() =
    app() |> Program.mountElement "sutil-app"

main()
//open Fable.Core.JsInterop
//Browser.Dom.window?main <- main
