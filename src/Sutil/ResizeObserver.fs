/// <summary>
/// Support for listening and reacting to window resize events
/// </summary>
module Sutil.ResizeObserver

// Ported from Svelte

open Browser.Types
open Browser.Dom
open Browser.CssExtensions
open System
open Interop
open DomHelpers

let isCrossOrigin = false // TODO

type private ResizeSubscriber = {
    Callback: unit -> unit
    Id : int
}

type ResizeObserver( el : HTMLElement ) =
    let mutable iframe : HTMLIFrameElement = Unchecked.defaultof<_>
    let mutable subId = 0
    let mutable unsubscribe : (unit -> unit) = Unchecked.defaultof<_>
    let mutable subscribers = []

    let notify _ =
        subscribers |> List.iter (fun sub -> sub.Callback())

    do
        let computedStyle = Window.getComputedStyle(el)
        let zIndex =  (try int(computedStyle.zIndex) with |_ -> 0) - 1;

        if computedStyle.position = "static" || computedStyle.position = "" then
            el.style.position <- "relative"

        iframe <- downcast (documentOf el).createElement("iframe")
        let style = sprintf "%sz-index: %i;" "display: block; position: absolute; top: 0; left: 0; width: 100%; height: 100%; overflow: hidden; border: 0; opacity: 0; pointer-events: none;" zIndex
        iframe.setAttribute("style", style)
        iframe.setAttribute("aria-hidden", "true")
        iframe.setAttribute("tabindex", "-1")

        if isCrossOrigin then
            iframe.setAttribute("src", "data:text/html,<script>onresize=function(){parent.postMessage(0,'*')}</script>")

            unsubscribe <- DomHelpers.listen "message" window
                (fun e -> if Helpers.fastEquals (Interop.get e "source") iframe.contentWindow then notify(e))
        else
            iframe.setAttribute("src", "about:blank")
            iframe.onload <- (fun e ->
                unsubscribe <- DomHelpers.listen "resize" iframe.contentWindow notify)

        el.appendChild(iframe) |> ignore

    member _.Subscribe(callback : (unit -> unit)) =
        let sub = { Callback = callback; Id = subId }
        subId <- subId + 1
        subscribers <- sub :: subscribers
        Helpers.disposable <| fun () -> subscribers <- subscribers |> List.filter (fun s -> s.Id <> sub.Id)

    member _.Dispose() =
        try unsubscribe() with |_ -> ()
        if not (isNull iframe) then
            iframe.parentNode.removeChild(iframe) |> ignore

    interface IDisposable with
        member this.Dispose() = this.Dispose()

let getResizer (el:HTMLElement) : ResizeObserver =
    NodeKey.getCreate el NodeKey.ResizeObserver (fun () -> new ResizeObserver(el))
