module App

open Fetch

open Sutil

open Sutil.Styling
open Sutil.Core
open Sutil.CoreElements
open Sutil.Transition
open Browser.Types
open Types

open type Feliz.length
open Fable.Core

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
        { Category = "Introduction";Title = "Hello World";  Link = AppLink (HelloWorld.view , ["HelloWorld.fs"])}
        { Category = "Introduction";Title = "Dynamic attributes";  Link = AppLink (DynamicAttributes.view , ["DynamicAttributes.fs"])}
        { Category = "Introduction";Title = "Styling";  Link = AppLink (StylingExample.view , ["Styling.fs"])}
        { Category = "Introduction";Title = "Nested components";  Link = AppLink (NestedComponents.view , ["NestedComponents.fs"; "Nested.fs"])}
        { Category = "Introduction";Title = "HTML tags";  Link = AppLink (HtmlTags.view , ["HtmlTags.fs"])}
        { Category = "Reactivity";Title = "Reactive assignments";  Link = AppLink (Counter.view , ["Counter.fs"])}
        { Category = "Reactivity";Title = "Reactive declarations";  Link = AppLink (ReactiveDeclarations.view , ["ReactiveDeclarations.fs"]) }
        { Category = "Reactivity";Title = "Reactive statements";  Link = AppLink (ReactiveStatements.view , ["ReactiveStatements.fs"]) }
        { Category = "Logic"; Title = "If blocks"; Link = AppLink (LogicIf.view, ["LogicIf.fs"])  }
        { Category = "Logic"; Title = "Else blocks"; Link = AppLink (LogicElse.view, ["LogicElse.fs"])  }
        { Category = "Logic"; Title = "Else-if blocks"; Link = AppLink (LogicElseIf.view, ["LogicElseIf.fs"])  }
        { Category = "Logic"; Title = "Static each blocks"; Link = AppLink (StaticEachBlocks.view, ["StaticEach.fs"])  }
        { Category = "Logic"; Title = "Static each with index"; Link = AppLink (StaticEachWithIndex.view, ["StaticEachWithIndex.fs"])  }
        { Category = "Logic"; Title = "Each blocks"; Link = AppLink (EachBlocks.view, ["EachBlocks.fs"])  }
        { Category = "Logic"; Title = "Keyed-each blocks"; Link = AppLink (KeyedEachBlocks.view, ["KeyedEachBlocks.fs"])  }
        { Category = "Logic"; Title = "Await blocks"; Link = AppLink (AwaitBlocks.view, ["AwaitBlocks.fs"])  }
        { Category = "Events"; Title = "DOM events"; Link = AppLink (DomEvents.view, ["DomEvents.fs"])  }
        { Category = "Events"; Title = "Custom events"; Link = AppLink (CustomEvents.view, ["CustomEvents.fs"])  }
        { Category = "Events"; Title = "Event modifiers"; Link = AppLink (EventModifiers.view, ["EventModifiers.fs"])  }
        { Category = "Transitions"; Title = "Transition"; Link = AppLink (Transition.view, ["Transition.fs"])  }
        { Category = "Transitions"; Title = "Adding parameters"; Link = AppLink (TransitionParameters.view, ["TransitionParameters.fs"])  }
        { Category = "Transitions"; Title = "In and out"; Link = AppLink (TransitionInOut.view, ["TransitionInOut.fs"])  }
        { Category = "Transitions"; Title = "Custom CSS"; Link = AppLink (TransitionCustomCss.view, ["TransitionCustomCss.fs"])  }
        { Category = "Transitions"; Title = "Custom Code"; Link = AppLink (TransitionCustom.view, ["TransitionCustom.fs"])  }
        { Category = "Transitions"; Title = "Transition events"; Link = AppLink (TransitionEvents.view, ["TransitionEvents.fs"])  }
        { Category = "Transitions"; Title = "Animation"; Link = AppLink (Todos.view, ["Todos.fs"])  }
        { Category = "Bindings";   Title = "Text inputs";  Link = AppLink (TextInputs.view , ["TextInputs.fs"]) }
        { Category = "Bindings";   Title = "Numeric inputs";  Link = AppLink (NumericInputs.view , ["NumericInputs.fs"]) }
        { Category = "Bindings";   Title = "Checkbox inputs";  Link = AppLink (CheckboxInputs.view , ["CheckboxInputs.fs"]) }
        { Category = "Bindings";   Title = "Group inputs";  Link = AppLink (GroupInputs.view , ["GroupInputs.fs"]) }
        { Category = "Bindings";   Title = "Textarea inputs";  Link = AppLink (TextArea.view , ["TextArea.fs"]) }
        { Category = "Bindings";   Title = "File inputs";  Link = AppLink (FileInputs.view , ["FileInputs.fs"]) }
        { Category = "Bindings";   Title = "Select bindings";  Link = AppLink (SelectBindings.view , ["SelectBindings.fs"]) }
        { Category = "Bindings";   Title = "Select multiple";  Link = AppLink (SelectMultiple.view , ["SelectMultiple.fs"]) }
        { Category = "Bindings";   Title = "Dimensions";  Link = AppLink (Dimensions.view , ["Dimensions.fs"]) }
        { Category = "Svg";   Title = "Bar chart";  Link = AppLink (BarChart.view , ["BarChart.fs"]) }
        { Category = "Miscellaneous";   Title = "Spreadsheet";  Link = AppLink (Spreadsheet.view , ["Spreadsheet.fs"; "Evaluator.fs"; "Parser.fs"]) }
        { Category = "Miscellaneous";   Title = "Modal";  Link = AppLink (Modal.view , ["Modal.fs"]) }
        { Category = "Miscellaneous";   Title = "Login";  Link = AppLink (LoginExample.view , ["LoginExample.fs"; "Login.fs"]) }
        { Category = "Miscellaneous";   Title = "Drag-sortable list";  Link = AppLink (SortableTimerList.view , ["SortableTimerList.fs"; "DragDropListSort.fs"; "TimerWithButton.fs"; "TimerLogic.fs"]) }
        { Category = "Miscellaneous";   Title = "SAFE client";  Link = AppLink (SAFE.view , ["SafeClient.fs"]) }
        { Category = "Miscellaneous";   Title = "Data Simulation";  Link = AppLink (DataSim.view , ["DataSim.fs"]) }
        { Category = "Miscellaneous";   Title = "Web Components";  Link = AppLink (WebComponents.view , ["WebComponents.fs"]) }
        { Category = "Miscellaneous";   Title = "Draw";  Link = AppLink (Draw.view , ["Draw.fs"]) }
        //{ Category = "Miscellaneous";   Title = "Fragment";  Link = AppLink (Fragment.view , ["Fragment.fs"]) }
//        { Category = "7Guis";   Title = "Cells";  Link = AppLink (SevenGuisCells.view , ["Cells.fs"]) }
        { Category = "7Guis";   Title = "CRUD";  Link = AppLink (CRUD.view , ["CRUD.fs"]) }
    ]

let initBooks = [
    { Title = "Examples"; Pages = allExamples }
//    { Title = "Documentation"; Pages = allDocs }
]

let urlBase = "https://raw.githubusercontent.com/davedawkins/Sutil/main/src/App"

// View as specified by the URL
type ViewRequest = {
    BookName : string
    PageName : string
    SectionName : string
}

type BookPageView = {
    Book : Book
    Page : Page
    Section : string
    Source : string // For an "Example" Page, a Section is the name of a source file. Contents are fetched to Source
}

type MainView =
    | FrontPage
    | PageView of BookPageView

type Model = {
    ShowContents : bool
    IsMobile : bool
    Books : Book list
    View : MainView
    ViewRequest : ViewRequest option
}

let bindOpt (model : System.IObservable<'T option>) (view : 'T -> SutilElement) : SutilElement =
    Bind.el(model,fun optVal ->
        match optVal with
        | None -> fragment []
        | Some v -> view v)


let getSource (v : BookPageView) = v.Source
let getPage (v : BookPageView) = v.Page
let getBook (v : BookPageView) = v.Book
let getSection (v : BookPageView) = v.Section

let getView (m : Model) = m.View

let currentBook m = match m.View with PageView v -> Some v.Book| _ -> None
let currentPage m = match m.View with PageView v -> Some v.Page| _ -> None
let currentSection m = match m.View with PageView v -> Some v.Section| _ -> None

let books m = m.Books
let isMobile m = m.IsMobile
let showContents m = m.ShowContents

type Message =
    | SetSource of string
    | SetIsMobile of bool
    | AddBook of Book
    | SetPageView of ViewRequest
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
let findBookPage (books : Book list) (pv : ViewRequest) =
    let defBk = defaultBook books
    let bookP =
        books
            |> List.tryFind (fun b -> sanitize b.Title = sanitize pv.BookName)
            |> Option.map (fun bk -> bk, findPage bk pv.PageName)
    match bookP with
    | Some (bk, Some pg) -> bk, pg
    | Some (bk, None) -> bk, bk.defaultPage
    | _ -> defBk, defBk.defaultPage

let ducc = Observable.distinctUntilChangedCompare
let duc = Observable.distinctUntilChangedCompare
//let compareBook (a:Book option) (b:Book option) = match (a,b) with Some a', Some b' -> (a'.Title = b'.Title) | _ -> false
//let comparePage (a:Page option) (b:Page option) = match (a,b) with Some a', Some b' -> (a'.Title = b'.Title) | _ -> false
let compareSection (a:string option) (b:string option) = a = b
//let onBookChange source = source |> ducc compareBook
//let onPageChange source = source |> ducc comparePage
let pageCategories (all : Page list) = all |> List.map (fun p -> p.Category) |> List.distinct

let compareBook (a : BookPageView) (b : BookPageView) =
    a.Book.Title = b.Book.Title

let compareBookPage (a : BookPageView) (b : BookPageView) =
    a.Book.Title = b.Book.Title && a.Page.Title = b.Page.Title

let fetchSource file dispatch =
    let url = sprintf "%s/%s" urlBase file
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (SetSource >> dispatch)
    |> ignore

let defaultBookPageView() =
    let currentBook = defaultBook initBooks
    let currentPage = currentBook.defaultPage
    {
        Book = currentBook
        Page = currentPage
        Section = ""
        Source = ""
    }

let init() =
    let defaultBpv = defaultBookPageView()
    {
        ShowContents = false
        IsMobile = false
        Books = initBooks
        View = FrontPage //PageView defaultBpv
        ViewRequest = None
        // {
        //     BookName = defaultBpv.Book.Title
        //     PageName = defaultBpv.Page.Title
        //     SectionName = ""
        // }
    }, []

let update msg model : Model * Cmd<Message> =
    Browser.Dom.console.log($"update {msg}")
    match msg with
    | SetIsMobile m ->
        { model with IsMobile = m }, Cmd.none

    | AddBook book ->
        let cmd = model.ViewRequest |> Option.map (Cmd.ofMsg << SetPageView) |> Option.defaultValue Cmd.none
        { model with Books = book :: model.Books },
            cmd

    | SetPageView request ->
        if request.BookName = "" then
            { model with View = FrontPage; ViewRequest = None }, Cmd.none
        else
            let book, page = request |> findBookPage model.Books
            let section =
                match (page.Link) with
                | AppLink (creator, sections) ->
                    if (sections |> List.contains request.SectionName) then request.SectionName else ""
                | Url _ -> ""

            // Fetch the source file for section. Non-empty section refers to source file for example
            // This may change!
            let cmd, src =
                let loadingString = "[ Loading ]"
                match model.View with
                | PageView bpv when (section <> "" && section <> bpv.Section && bpv.Source <> loadingString) ->
                    [ fetchSource section ], loadingString
                | _ -> Cmd.none, ""

            { model with
                View = PageView {
                    Page = page
                    Book = book
                    Section = section
                    Source = src
                }
                ShowContents = false
                ViewRequest = Some request} , cmd

    | ToggleShowContents ->
        { model with ShowContents = not model.ShowContents }, Cmd.none

    | SetSource content ->
        match model.View with
        | PageView pageView ->
            { model with View = PageView { pageView with Source = content } }, Cmd.none
        | _ -> model,Cmd.none

let mainStyleSheet = [

    rule ".app-main" [
        Css.height (percent 100)
    ]

    rule ".app-heading" [
        Css.displayFlex
        Css.flexDirectionRow
        Css.justifyContentSpaceBetween
        Css.positionFixed
        Css.width (vw 100)
        Css.backgroundColor "white"
        Css.paddingLeft (px 12)
        Css.paddingTop (rem 0.7)
        Css.paddingBottom (px 6)
        Css.boxShadow "-0.4rem 0.01rem 0.3rem rgba(0,0,0,.5)"
        Css.marginBottom 4
        Css.zIndex 1   // Messes with .modal button
        Css.height (rem 3.0)
    ]

    rule ".app-heading h1" [
       Css.marginBottom 0
    ]

    rule ".app-contents" [
        Css.backgroundColor "#164460"//"#676778"
        Css.color "white"
        Css.overflowAuto
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
        Css.paddingTop (rem 3.0)
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
        Css.marginRight (rem 1.0)
    ]

    rule ".app-tab-menu a" [
        Css.marginRight 24
    ]
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

    let parsePageView (loc:Location) : ViewRequest =
        let hash, query = (parseUrl loc)
        let book, page = parseBookPage hash

        {
            BookName = book
            PageName = page
            SectionName = query
        }

let Section (tab:Book) (name:string) = fragment [
    Html.h5 [ class' "title is-6"; text (name.ToUpper()) ]
    Html.ul [
        for page in tab.Pages |> List.filter (fun x -> x.Category = name) do
            Html.li [
                Html.a [
                    match page.Link with
                    | AppLink (_,_) ->
                        Attr.href <| makeHref tab.Title  page.Title ""
                    | Url url ->
                        Attr.href url
                    text page.Title
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

let viewSource (bookPage : System.IObservable<BookPageView>) =
    Html.div [
        Html.pre [
            Html.code [
                class' "language-fsharp"
                Bind.el(bookPage .> getSource, exclusive << text)
            ]
        ]
    ]

let viewPage (view:System.IObservable<BookPageView>) =
    let compareSectionSource a b =
        compareBookPage a b && a.Section = b.Section && a.Source = b.Source

    let sectionView = view |> Observable.distinctUntilChangedCompare compareSectionSource
    Html.div [

        class' "column app-page"
        Bind.el (sectionView, fun bpv ->
            let page = getPage bpv
            match bpv.Section with
            | "" ->
                try
                    match page.Link with
                    | AppLink (creator, sections) ->
                        creator()

                    // We shouldn't ever reach here. It means the user somehow clicked on
                    // a link that didn't take them immediately to the URL, but instead
                    // managed to update the current page.
                    // A different and simpler domain model for the books and pages would
                    // eliminate this.
                    | Url url -> Html.a [ Attr.href url; text url ]
                with
                    |x -> Html.div[ text $"Creating example {page.Title}: {x.Message}" ]
            | _ -> viewSource view)
    ]


let viewPageWithHeader (view:System.IObservable<BookPageView>) =
    let book = view |> Store.current |> getBook

    // Only sees changes to book x page
    let bookPageOnly = view |> Observable.distinctUntilChangedCompare compareBookPage

    Bind.el( bookPageOnly, fun bpv' ->
        let page = bpv'.Page
        Html.div [
            class' "column app-page-section"

            match page.Link with
            | AppLink (_, sections) when sections <> [] ->
                Html.div [
                    class' "app-toolbar"
                    Html.ul [
                        class' "app-tab"
                        sectionItem book page page.Title
                        sections |> List.map (sectionItem book page) |> fragment
                    ]
                ]

            | _ ->
                fragment []

            viewPage view
        ]
    )

let viewBook showContents (bookPageView : System.IObservable<BookPageView>) =
    let book = bookPageView |> Store.current |> getBook
    Html.div [
        class' "columns app-main-section"

        transition [fly |> withProps [ Duration 500.0; X -500.0 ] |> In] showContents <|
            Html.div [
                class' "column is-one-quarter app-contents"

                book.Pages |> pageCategories |> List.map (fun title -> Section book title) |> fragment
            ]

        viewPageWithHeader bookPageView
    ]

let articleTile cls title subtitle =
    Html.div [
        class' "tile is-parent"
        Html.article [
            class' ("tile is-child box " + cls)
            Html.p [
                class' "title has-text-white"
                text title
            ]
            Html.p [
                class' "subtitle has-text-white"
                text subtitle
            ]
        ]
    ]

let frontPageRules = [
    rule "div.front-page" [
        Css.paddingTop (rem 3.0)
    ]
    rule ".hero p" [
        Css.fontSize (percent 150.0)
    ]
    rule ".tile.is-parent" [
        Css.padding (rem 0.75)
    ]
    rule ".tile.is-ancestor" [
        Css.marginLeft (rem 0.75)
        Css.marginRight (rem 0.75)
        Css.marginTop (rem 0.75)
    ]

    rule ".color-1" [
        Css.backgroundColor ("hsl(327, 48%, 39%)")
    ]

    rule ".color-2" [
        Css.backgroundColor ("hsl(210, 48%, 39%)")
    ]

    rule ".color-3" [
        Css.backgroundColor ("hsl(120, 48%, 39%)")
    ]
]

let tiles = [

    Html.div [
        class' "tile is-ancestor"
        style [ Css.positionAbsolute ]
        articleTile
            "color-1"
            "Tiny Footprint"
            "No dependencies on other frameworks - Sutil is pure F#"
        articleTile
            "color-2"
            "Awesome Transitions"
            "One-liner transitions"
        articleTile
            "color-3"
            "Easy DOM"
            "Build content with code-completion and tooltips"
    ]

    Html.div [
        class' "tile is-ancestor"
        style [ Css.positionAbsolute ]
        articleTile
            "color-2"
            "Batteries Included"
            "Everything you need to build an awesome application"
        articleTile
            "color-3"
            "Fully Reactive"
            "Connect your data directly to your view"
        articleTile
            "color-1"
            "Integration Friendly"
            "Plays well with other frameworks"
    ]
    Html.div [
        class' "tile is-ancestor"
        style [ Css.positionAbsolute ]
        articleTile
            "color-3"
            "Component Styling"
            "Easily define stylesheets that apply to your component"
        articleTile
            "color-1"
            "Supports MVU/Elmish"
            "Blend in aspects of MVU/Elmish with reactive programming as needed"
        articleTile
            "color-2"
            "Typesafe Development"
            "The compiler is your best buddy. String-based programming only pretends to be your friend."
    ]
]

let slideshow interval trans (elements : SutilElement list) =
    let currentTileIndex = Store.make 0
    let numElements = elements.Length

    let ticker =
        DomHelpers.interval
            (fun _ -> currentTileIndex |> Store.modify (fun x -> (x + 1) % numElements))
            interval

    let transitionElements =
        elements
        |> List.mapi (fun i tile ->  transition trans (currentTileIndex |> Store.map ((=) i)) tile)

    let cleanupElements = [
        disposeOnUnmount [ currentTileIndex ]
        unsubscribeOnUnmount [ ticker ]
    ]

    transitionElements @ cleanupElements |> fragment

let viewFrontPage() =
    let tileFade = fade |> withProps [Duration 1000.0]

    Html.div [
        class' "app-main-section"

        Html.div [
            class' "front-page"
            Html.section [
                class' "hero"
                Html.div [
                    class' "hero-body has-text-centered"
                    Html.p [
                        class' "title is-size-1"
                        Html.img [
                            Attr.src "https://sutil.dev/images/logo-wide.png"
                            style [
                                Css.width (px 300)
                            ]
                        ]
                        //text "sutil"
                    ]
                    Html.p [
                        class' "subtitle"
                        text "A pure F# web application framework"
                    ]
                ]
            ]

            slideshow 5000 [ InOut tileFade ] tiles
        ]
    ] |> withStyle frontPageRules

let appMain () =
    let model, dispatch = () |> Store.makeElmish init update ignore

    let showContents = model .> (fun m -> not m.IsMobile || m.ShowContents)

    let umedia = Media.listenMedia( "(max-width: 768px)", dispatch << SetIsMobile)
    let upage  = Navigable.listenLocation(UrlParser.parsePageView, dispatch << SetPageView)

    let defaultBpv = defaultBookPageView()
    Doc.getBook() |> Promise.map (dispatch << AddBook) |> ignore

    Html.div [
        class' "app-main"

        unsubscribeOnUnmount [ umedia; upage ]
        disposeOnUnmount [ model ]

        Html.div [
            class' "app-heading"
            Html.span [
                Html.a [
                    Attr.href "https://sutil.dev"
                    Html.img [
                        Attr.src "images/logo-wide.png"
                        Attr.style [ Css.height (px 25) ]
                        ]
                ]
            ]

            Html.span [
                Bind.el (model .> books,fun books ->
                    Html.span [
                        class' "app-tab-menu"
                        books |> List.map (fun bk -> Html.a [ Attr.href <| makeBookHref bk; text bk.Title ]) |> fragment
                        Html.a [ text "REPL"; Attr.href "https://sutil.dev/repl" ]
                    ])

                fragment [

                    Html.a [
                        Attr.href "https://github.com/davedawkins/Sutil"
                        Html.i [
                            Attr.style [ Css.color "rgb(61,80,80)"; Css.marginRight (rem 1.0); Css.verticalAlignMiddle ]
                            class' "fa fa-github"
                        ]
                    ]

                    Html.a [
                        Attr.href "https://www.nuget.org/packages/Sutil"
                        Html.img [
                            Attr.style [ Css.marginRight (rem 1.0); Css.verticalAlignMiddle ]
                            Attr.src "https://img.shields.io/nuget/vpre/Sutil?color=rgb%2861%2C80%2C80%29&label=%20nuget&style=social"
                        ]
                    ]

                ] |> transition [] (model .> (not << isMobile))
            ]

            transition [InOut fade] (model .> isMobile) <| Html.a [
                class' "show-contents-button"
                Attr.href "#"
                Html.i [ class' "fa fa-bars" ]
                onClick (fun _ -> ToggleShowContents |> dispatch) [ PreventDefault ]
            ]
        ]

        // Using a keyed binding prevents the book from being redrawn when different pages and sections
        // are selected. This is only noticable if the contents window is scrolled. You want
        // the contents (part of the book rendering) to remain unchanged while clicking into
        // different pages

        let keyForView = function FrontPage -> "FrontPage" | PageView v -> v.Book.Title

        Bind.el (model |> Store.map getView, keyForView, fun (view:System.IObservable<MainView>) ->
            // Because of key, only called when book changes or we transition to/from front page
            // We won't be called for any other model changes (such as the page or section changing)

            let pageView =
                view
                |> Store.map (function PageView pv -> pv|_ -> failwith "unreachable")

            match (view |> Store.current) with
            | FrontPage ->
                viewFrontPage()
            | PageView _ ->
                viewBook showContents pageView
        )

    ]

let app () =
    Html.app [
        // Page title
        headTitle "sutil"

        appMain() |> withStyle mainStyleSheet |> Bulma.withBulmaHelpers
    ]

let main() =
    app() |> Program.mount

main()
//open Fable.Core.JsInterop
//Browser.Dom.window?main <- main
