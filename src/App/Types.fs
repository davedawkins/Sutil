module Types

open Sutil.Core
open Sutil.CoreElements

type ViewCreator = unit -> SutilElement
type Sections = string list

type PageLink =
    | AppLink of (ViewCreator * Sections)
    | Url of (string)

/// A Page is an entry in a Book.
/// For book "Examples", each page is a demo min-app. Its sections are the mini-app and the mini-app source
/// For book "Documentation", each page is either
/// - a markdown file loaded from "/doc"
/// - an external URL

type Page = {
    Title : string
    Category : string
    Link : PageLink
    // Create : (unit -> SutilElement)
    // Sections : string list
}

/// The app currently has two books, with titles "Examples" and "Documentation"
/// The "Examples" book is constructed in App.fs and is compiled in
/// The "Documentation" book is constructed from /doc/index.md
type Book = {
    Title : string
    Pages : Page list
} with
    member this.defaultPage = this.Pages.Head
