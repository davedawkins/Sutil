module Expect.Dom

open System
open Fable.Core
open Fable.Core.JsInterop
open Browser
open Browser.Types

type Queries =
    abstract getByRole: Element * role: string * accessibleNamePattern: string -> Element
    abstract getByText: Element * pattern: string -> Element

[<ImportAll("./queries.bundle.js")>]
let private queries: Queries = jsNative

type Element with
    member this.cast<'El when 'El :> Element>() =
        this :?> 'El

    member this.asInput =
        this.cast<HTMLInputElement>()

    member this.asButton =
        this.cast<HTMLButtonElement>()

    member this.asHTML =
        this.cast<HTMLElement>()

    /// Automatically creates an element with the given tag name ("div" by default),
    /// appends it as a child of this element and returns it.
    member this.appendChild(?tagName: string) =
        let child = document.createElement(defaultArg tagName "div")
        this.appendChild(child) |> ignore
        child

    /// Runs querySelector on element (or shadowRoot if present) and returns None if nothing is found
    member el.tryGetSelector(selector: string) =
        let el = if isNull el.shadowRoot then el else el.shadowRoot :> _
        el.querySelector(selector) |> Option.ofObj

    /// Runs querySelector on element (or shadowRoot if present) and throw error if nothing is found
    member this.getSelector(selector: string) =
        match this.tryGetSelector(selector) with
        | None -> failwith $"""Cannot find element with selector "{selector}"."""
        | Some v -> v

    /// Return first child that has given role and whose accessible name matches the pattern, or throw error
    /// Pattern becomes an ignore-case regular expression.
    member el.getByRole(role: string, accessibleNamePattern: string) =
        let el = if isNull el.shadowRoot then el else el.shadowRoot :> _
        queries.getByRole(el, role, accessibleNamePattern)

    /// Same as getByRole("button", accessibleNamePattern).
    member this.getButton(accessibleNamePattern: string) =
        this.getByRole("button", accessibleNamePattern) :?> HTMLButtonElement

    /// Same as getByRole("checkbox", accessibleNamePattern).
    /// Matches `input` elements of type "text" or `checkbox`
    member this.getCheckbox(accessibleNamePattern: string) =
        this.getByRole("checkbox", accessibleNamePattern) :?> HTMLInputElement

    /// Same as getByRole("textbox", accessibleNamePattern).
    /// Matches `input` elements of type "text" or `textarea`.
    member this.getTextInput(accessibleNamePattern: string) =
        this.getByRole("textbox", accessibleNamePattern) :?> HTMLInputElement

    /// Return first text node child with text matching given pattern, or throw error.
    /// Pattern becomes an ignore-case regular expression.
    member el.getByText(pattern: string) =
        let el = if isNull el.shadowRoot then el else el.shadowRoot :> _
        queries.getByText(el, pattern) :?> HTMLElement

type Container =
    inherit IDisposable
    abstract El: HTMLElement

[<AutoOpen>]
module ContainerExtensions =
    type Container with
        /// Creates an HTML element with the specified tag an puts it in `document.body`.
        /// When disposed, the element will be removed from `document.body`.
        static member New(?tagName: string) =
            let el = document.createElement(defaultArg tagName "div")
            document.body.appendChild(el) |> ignore
            { new Container with
                member _.El = el
                member _.Dispose() =
                    document.body.removeChild(el) |> ignore }

[<RequireQualifiedAccess>]
module Promise =
    let awaitAnimationFrame() =
        Promise.create(fun resolve _ ->
            window.requestAnimationFrame(fun _ -> resolve()) |> ignore)

[<RequireQualifiedAccess>]
module Expect =
    /// <summary>
    /// Checks the text content of an element
    /// </summary>
    let innerText (expected: string) (el: Element) =
        let el = el :?> HTMLElement
        if not(el.innerText = expected) then
            let description = $"{el.tagName.ToLower()}.innerText"
            AssertionError.Throw("equal", description=description, actual=el.innerText, expected=expected)

    /// <summary>
    /// Registers an event listener for a particular event name, use the action callback to make your component fire up the event.
    /// The function will return a promise that resolves once the element dispatches the specified event
    /// </summary>
    /// <param name="eventName">The name of the event to listen to.</param>
    /// <param name="action">A callback to make the element dispatch the event. this callback will be fired immediately after the listener has been registered</param>
    /// <param name="el">Element to monitor for dispatched events.</param>
    let dispatch eventName (action: unit -> unit) (el: HTMLElement) =
        Promise.create
            (fun resolve reject ->
                el.addEventListener (
                    eventName,
                    fun _ ->
                        resolve()
                )

                action())
        // The event should be fired immediately so we set a small timeout
        |> Expect.beforeTimeout $"dispatch {eventName}" 100

    /// <summary>
    /// Registers an event listener for a particular event name, use the action callback to make your component fire up the event.
    /// The function will return a promise that resolves the detail of the custom event once the element dispatches the specified event
    /// </summary>
    /// <param name="eventName">The name of the event to listen to.</param>
    /// <param name="action">A callback to make the element dispatch the event. this callback will be fired immediately after the listener has been registered</param>
    /// <param name="el">Element to monitor for dispatched events.</param>
    /// <returns>The detail that was provided by the custom event </returns>
    let dispatchCustom<'T> eventName (action: unit -> unit) (el: HTMLElement) =
        Promise.create
            (fun resolve reject ->
                el.addEventListener (
                    eventName,
                    fun (e: Event) ->
                        let custom = e :?> CustomEvent<'T>
                        resolve custom.detail
                )

                action())
        // The event should be fired immediately so we set a small timeout
        |> Expect.beforeTimeout $"dispatch {eventName}" 100
