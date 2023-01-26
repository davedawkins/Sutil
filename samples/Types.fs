module Types

open Sutil.Core
open Sutil.CoreElements

type Page = {
    Title : string
    Category : string
    Create : (unit -> SutilElement)
    Sections : string list
}

type Book = {
    Title : string
    Pages : Page list
} with
    member this.defaultPage = this.Pages.Head
