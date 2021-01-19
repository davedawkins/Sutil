module Sveltish.Program

    open Sveltish.DOM

    //let makeComponent name (element : NodeFactory) : NodeFactory = fun (ctx,parent) ->
    //    element( { ctx with StyleName = "" }, parent )

    //
    // Sveltish Elmish
    // The model mutates in Sveltish, so the function signatures are slightly different to Elmish.
    // This approach isn't necessary, but it could be helpful in that it encourages the view to
    // dispatch messages that are then processed only in the update function.
    //
    let makeProgram host init update view =
        let doc = Browser.Dom.document
        let model = init()

        let makeDispatcher update =
            (fun msg ->
                update msg model
                DOM.Event.notifyUpdated doc)

        Sveltish.DOM.mountElementOnDocument doc host <| view model (makeDispatcher update)