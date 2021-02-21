module Types

open Sutil.DOM

type Page = {
    Title : string
    Category : string
    Create : (unit -> NodeFactory)
    Sections : string list
}

type Book = {
    Title : string
    Pages : Page list
} with
    member this.defaultPage = this.Pages.Head
