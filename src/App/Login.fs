module Login

//
// Conversion of https://codepen.io/stevehalford/pen/YeYEOR into an example Sutil Login component
// See LoginExample.fs for usage
//

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Bulma

type LoginDetails = {
    Username: string
    Password: string
    RememberMe: bool }
with
    static member Default = { Username = ""; Password = ""; RememberMe = false }

type private Model = {
    Message : string
    Details : LoginDetails }

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

module EventHelpers =
    open Browser.Types

    let inputElement (target:EventTarget) = target |> asElement<HTMLInputElement>

    let validity (e : Event) =
        inputElement(e.target).validity

// View function. Responsibilities:
// - Arrange for cleanup of model on dispose
// - Present model to user
// - Handle input according to Message API

let private defaultView model dispatch =
    bulma.hero [
        disposeOnUnmount [ model ]

        hero.isInfo

        bulma.heroBody [
            bulma.container [
                bulma.columns [
                    columns.isCentered
                    bulma.column [
                        column.tabletIs 10; column.desktopIs 8; column.widescreenIs 6
                        bulma.formBox [
                            on "submit" (fun _ -> AttemptLogin |> dispatch) [PreventDefault]
                            Attr.action ""

                            bulma.field [
                                class' "has-text-danger"
                                Bind.fragment (model .> message) text
                            ] |> Transition.showIf (model .> messageIsSet)

                            bulma.field [
                                bulma.label "Email"
                                bulma.control [
                                    control.hasIconsLeft
                                    bulma.email [

                                        bindEvent "input" (fun e -> EventHelpers.validity(e).valid |> not) (fun s -> bindClass s "is-danger")

                                        Attr.placeholder "Hint: sutil@gmail.com"
                                        Bind.attr ("value", model .> username , SetUsername >> dispatch)
                                        Attr.required true
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
                                        Attr.placeholder "Hint: abc123"
                                        Bind.attr("value", model .> password, SetPassword >> dispatch)
                                        Attr.required true ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "lock"
                                    ]
                                ]
                            ]
                            bulma.field [
                                bulma.labelCheckbox " Remember me" [
                                    Bind.attr("checked", model .> rememberMe, SetRememberMe >> dispatch)
                                ]
                            ]
                            bulma.field [
                                field.isGrouped
                                bulma.control [
                                    bulma.button [
                                        button.isSuccess
                                        text "Login"
                                    ]
                                ]
                                bulma.control [
                                    bulma.button [
                                        text "Cancel"
                                        onClick (fun _ -> dispatch CancelLogin) [PreventDefault]
                                    ]
                                ]
                            ]
                        ] ] ] ] ] ]

let private createWithView initDetails login cancel view =

    let update msg model =
        match msg with
            |SetUsername name -> { model with Details = { model.Details with Username = name }}, Cmd.none
            |SetPassword pwd -> { model with Details = { model.Details with Password = pwd}}, Cmd.none
            |SetRememberMe z -> { model with Details = { model.Details with RememberMe = z }}, Cmd.none
            |AttemptLogin -> model, Cmd.OfFunc.attempt login model.Details (fun ex -> SetMessage ex.Message)
            |SetMessage m -> { model with Message = m }, Cmd.none
            |CancelLogin -> model, Cmd.OfFunc.perform cancel () (fun _ -> SetMessage "")

    let model, dispatch = initDetails |> Store.makeElmish init update ignore

    view model dispatch

let create initDetails login cancel =
    createWithView initDetails login cancel defaultView

