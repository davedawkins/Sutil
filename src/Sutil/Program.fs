namespace Sutil

open Core
open CoreElements
open Browser.Types
open Browser.Dom

/// <summary>
/// Main entry points for a Sutil program
/// </summary>
module Program =
    open System

    let internal _mount ((_,eref) as mp : MountPoint) (view : SutilElement) =
        ObservableStore.Registry.initialise eref.AsElement.ownerDocument
        Core.mount view mp |> ignore

    // -----------------------------------------------------------------------------------------------------------------------------

    [<Obsolete("Use Program.mount( host, app )")>]
    let mountElementOnDocumentElement (host : HTMLElement) (app : SutilElement)  =
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount( doc, id, app )")>]
    let mountElementOnDocument (doc : Document) id (app : SutilElement)  =
        let host = doc.querySelector($"#{id}") :?> HTMLElement
        //mountElementOnDocumentElement host app
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount( host, app )")>]
    let mountDomElement (host : HTMLElement) (app : SutilElement)  =
        //mountElementOnDocumentElement host app
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount( id, app )")>]
    let mountElement id (app : SutilElement)  =
        //mountElementOnDocument document id (exclusive app)
        app |> _mount (MountOp.AppendTo,ElementRef.Id id)

    [<Obsolete("Use Program.mountAfter( host, app )")>]
    let mountElementAfter (prev : HTMLElement) (app : SutilElement) =
        app |> _mount (MountOp.InsertAfter,ElementRef.Element prev)


type Program() =
    ///<summary>
    /// Mount application on element with given id.
    ///</summary>
    static member mount (id : string, app : SutilElement) =
        app |> Program._mount (MountOp.AppendTo, Id id)

    ///<summary>
    /// Mount application on given HTMLElement
    ///</summary>
    static member mount (host : HTMLElement, app : SutilElement) =
        app |> Program._mount (MountOp.AppendTo, ElementRef.Element host)

    ///<summary>
    /// Mount application on element with id "sutil-app"
    ///</summary>
    static member mount (app : SutilElement) =
        Program.mount( "sutil-app", app )

    ///<summary>
    /// Mount application on element with given id from specific document
    ///</summary>
    static member mount (doc : Document, id : string, app : SutilElement) =
        let host = doc.querySelector($"#{id}") :?> HTMLElement
        Program.mount( host, app )

    ///<summary>
    /// Mount application after given HTMLElement
    ///</summary>
    static member mountAfter (prev : HTMLElement, app : SutilElement) =
        app |> Program._mount (MountOp.InsertAfter, ElementRef.Element prev)


