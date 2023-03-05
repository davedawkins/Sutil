namespace Sutil

open Core
open Browser.Types

/// <exclude/>
module Program =
    open System

    let internal _mount ((_,eref) as mp : MountPoint) (view : SutilElement) : IDisposable =
        ObservableStore.Registry.initialise eref.AsElement.ownerDocument
        Core.mount view mp

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
        app |> CoreElements.exclusive |> _mount (MountOp.AppendTo,ElementRef.Id id)

    [<Obsolete("Use Program.mountAfter")>]
    let mountElementAfter (prev : HTMLElement) (app : SutilElement) =
        app |> _mount (MountOp.InsertAfter,ElementRef.Element prev)

open System
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
    /// Mount application on element with given id. Existing children at that node will be removed. Return value can be disposed to unmount and clean up.
    ///</summary>
    static member mount (id : string, app : SutilElement) : IDisposable =
        app |> CoreElements.exclusive |> Program._mount (MountOp.AppendTo, Id id)

    ///<summary>
    /// Mount application on given HTMLElement. Existing children at that node will be removed. Return value can be disposed to unmount and clean up.
    ///</summary>
    static member mount (host : HTMLElement, app : SutilElement) : IDisposable=
        app |> CoreElements.exclusive |> Program._mount (MountOp.AppendTo, ElementRef.Element host)

    ///<summary>
    /// Mount application on element with id "sutil-app". Existing children at that node will be removed. Return value is <c>unit</c>, so use alternate version <c>mount( id, app )</c>
    /// if you need to unmount explicitly.
    ///</summary>
    static member mount (app : SutilElement) : unit =
        Program.mount( "sutil-app", app ) |> ignore

    ///<summary>
    /// Mount application on element with given id from specific document. Existing children at that node will be removed. Return value can be disposed to unmount and clean up.
    ///</summary>
    static member mount (doc : Document, id : string, app : SutilElement) : IDisposable =
        let host = doc.querySelector($"#{id}") :?> HTMLElement
        Program.mount( host, app )

    ///<summary>
    /// Mount application after given HTMLElement as a sibling. Return value can be disposed to unmount and clean up.
    ///</summary>
    static member mountAfter (prev : HTMLElement, app : SutilElement) : IDisposable =
        app |> Program._mount (MountOp.InsertAfter, ElementRef.Element prev)

    ///<summary>
    /// Mount application at given element, appending as last child and preserving existing children. Return value can be disposed to unmount and clean up.
    ///</summary>
    static member mountAppend (prev : HTMLElement, app : SutilElement) : IDisposable =
        app |> Program._mount (MountOp.AppendTo, ElementRef.Element prev)

    ///<summary>
    /// Remove this node, cleaning up all related Sutil resources. By design, it should be rare that you need to use this, but it provides
    /// a Sutil-safe way to remove nodes from the DOM. One use case is a Modal component that could be appended to the body element, and then
    /// unmounted upon close. Even in that case, you might find it's neater to use a Bind.el() or Bind.visible() (or a combination) to control
    /// the content and visibility of the modal.
    ///</summary>
    static member unmount( node : Node ) =
        DomHelpers.unmount node
