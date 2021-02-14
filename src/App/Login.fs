module Login

// Converssion of https://codepen.io/stevehalford/pen/YeYEOR
// Study of how real components look in Sutil. First it was converted to plain Html (see viewHtml() below),
// and then a Bulma module was created to reduce the noise. See viewBulma(). I will leave the viewHtml() function
// for comparison.
//
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Bulma
open Sutil.Bulma.FontAwesome

//
// Using Bulma module
//

type LoginDetails = {
    Username: string
    Password: string
    RememberMe: bool }
with
    static member Default = { Username = ""; Password = ""; RememberMe = false }

type private Model = {
    Message : string
    Details : LoginDetails
}

let private username m = m.Details.Username
let private password m = m.Details.Password
let private rememberMe m = m.Details.RememberMe
let private message m = m.Message
let private messageIsSet m = m.Message <> ""

type private Message =
    | SetUsername of string
    | SetPassword of string
    | SetRememberMe of bool
    | AttemptLogin
    | SetMessage of string
    | CancelLogin

let private init details =
    {
        Message = ""
        Details = details
    }, Cmd.none

// View function. Responsibilities:
// - Arrange for cleanup of model on dispose
// - Present model to user
// - Handle input according to Message API

let private defaultView model dispatch =
    bulma.hero [
        disposeOnUnmount [ model ]

        hero.isInfo
        //hero.isFullheight

        bulma.heroBody [
            bulma.container [
                bulma.columns [
                    columns.isCentered
                    bulma.column [
                        column.tabletIs 10; column.desktopIs 8; column.widescreenIs 6
                        bulma.formBox [
                            on "submit" (fun _ -> AttemptLogin |> dispatch) [PreventDefault]
                            action ""

                            bulma.field [
                                class' "has-text-danger"
                                Bindings.bind (model .> message) text
                            ] |> Transition.showIf (model .> messageIsSet)

                            bulma.field [
                                bulma.label "Email"
                                bulma.control [
                                    control.hasIconsLeft
                                    bulma.email [
                                        placeholder "Hint: sutil@gmail.com"
                                        Bindings.bindAttrNotify "value" (model .> username) (SetUsername >> dispatch)
                                        required
                                    ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "envelope"
                                    ]
                                ]
                            ]
                            bulma.field [
                                bulma.label "Password"
                                bulma.control [
                                    control.hasIconsLeft
                                    bulma.password [
                                        placeholder "Hint: abc123"
                                        Bindings.bindAttrNotify "value" (model .> password) (SetPassword >> dispatch)
                                        required ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "lock"
                                    ]
                                ]
                            ]
                            bulma.field [
                                bulma.labelCheckbox " Remember me" [
                                    Bindings.bindAttrNotify "checked" (model .> rememberMe) (SetRememberMe >> dispatch)
                                ]
                            ]
                            bulma.button [
                                button.isSuccess
                                text "Login"
                            ]
                            bulma.button [
                                style [ Css.marginLeft "24px" ]
                                text "Cancel"
                                onClick (fun _ -> dispatch CancelLogin) [PreventDefault]
                            ]
                        ] ] ] ] ] ]

let private createWithView initDetails login cancel view =

    // Local to create so we can reference parameter 'login'
    let update msg model =
        match msg with
            |SetUsername name -> { model with Details = { model.Details with Username = name }}, Cmd.none
            |SetPassword pwd -> { model with Details = { model.Details with Password = pwd}}, Cmd.none
            |SetRememberMe z -> { model with Details = { model.Details with RememberMe = z }}, Cmd.none
            |AttemptLogin -> model, Cmd.OfFunc.attempt login model.Details (fun ex -> SetMessage ex.Message)
            |SetMessage m -> { model with Message = m }, Cmd.none
            |CancelLogin -> model, Cmd.OfFunc.perform cancel () (fun _ -> SetMessage "")
    let model, dispatch = initDetails |> Store.makeElmish init update ignore

    // In theory we could use a user-supplied view function that knows how to
    // - display LoginDetails
    // - dispatch input events
    // - register model for cleanup when disposed
    view model dispatch

let create initDetails login cancel =
    createWithView initDetails login cancel defaultView

