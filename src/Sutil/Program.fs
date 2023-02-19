namespace Sutil

open Core
open Browser.Types

/// <exclude/>
module Program =
    open System

    let internal _mount ((_,eref) as mp : MountPoint) (view : SutilElement) =
        ObservableStore.Registry.initialise eref.AsElement.ownerDocument
        Core.mount view mp |> ignore

    // -----------------------------------------------------------------------------------------------------------------------------

    [<Obsolete("Use Program.mount")>]
    let mountElementOnDocumentElement (host : HTMLElement) (app : SutilElement)  =
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount")>]
    let mountElementOnDocument (doc : Document) id (app : SutilElement)  =
        let host = doc.querySelector($"#{id}") :?> HTMLElement
        //mountElementOnDocumentElement host app
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount")>]
    let mountDomElement (host : HTMLElement) (app : SutilElement)  =
        //mountElementOnDocumentElement host app
        app |> _mount (MountOp.AppendTo,ElementRef.Element host)

    [<Obsolete("Use Program.mount")>]
    let mountElement id (app : SutilElement)  =
        //mountElementOnDocument document id (exclusive app)
        app |> _mount (MountOp.AppendTo,ElementRef.Id id)

    [<Obsolete("Use Program.mountAfter")>]
    let mountElementAfter (prev : HTMLElement) (app : SutilElement) =
        app |> _mount (MountOp.InsertAfter,ElementRef.Element prev)

/// <summary>
/// Main entry points for a Sutil program
/// </summary>
/// <example>
/// For example, this will mount the <c>SutilElement</c> returned from <c>view()</c> onto the <c>div</c> with id "sutil-app"
/// <code>
/// view () |> Program.mount
/// </code>
/// </example>
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


