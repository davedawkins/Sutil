module Sveltish.Program

    let makeProgram host init update view =
        let model = init()

        let makeDispatcher update =
            (fun msg -> update msg model)

        Sveltish.DOM.mountElement host <| view model (makeDispatcher update)