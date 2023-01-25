module Login

//
// Conversion of https://codepen.io/stevehalford/pen/YeYEOR into an example Sutil Login component
// See LoginExample.fs for usage
//

open Sutil
open Sutil.CoreElements
open Sutil.DomHelpers
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

        color.isInfo

        bulma.heroBody [
            bulma.container [
                bulma.columns [
                    columns.isCentered
                    bulma.column [
                        column.is10Tablet; column.is8Desktop; column.is6Widescreen
                        bulma.box [
                            on "submit" (fun _ -> AttemptLogin |> dispatch) [PreventDefault]
                            Attr.action ""

                            bulma.field.div [
                                class' "has-text-danger"
                                Html.text (model .> message)
                            ] |> Transition.showIf (model .> messageIsSet)

                            bulma.field.div [
                                bulma.label "Email"
                                bulma.control.div [
                                    control.hasIconsLeft
                                    bulma.input.email [
                                        let isInvalid = Store.make false
                                        disposeOnUnmount [ isInvalid ]

                                        Bind.toggleClass(isInvalid, "is-danger")

                                        on "input" (fun e -> EventHelpers.validity(e).valid |> not |> Store.set isInvalid) []

                                        Attr.placeholder "Hint: sutil@gmail.com"
                                        Attr.value (model .> username , SetUsername >> dispatch)
                                        Attr.required true
                                    ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "envelope"
                                    ]
                                ]
                            ]
                            bulma.field.div [
                                bulma.label "Password"
                                bulma.control.div [
                                    control.hasIconsLeft
                                    bulma.input.password [
                                        Attr.placeholder "Hint: abc123"
                                        Attr.value (model .> password, SetPassword >> dispatch)
                                        Attr.required true ]
                                    bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        fa "lock"
                                    ]
                                ]
                            ]
                            bulma.field.div [
                                bulma.inputLabels.checkbox [
                                    bulma.input.checkbox [
                                        Bind.attr("checked", model .> rememberMe, SetRememberMe >> dispatch)
                                    ]
                                    Html.text " Remember me"
                                ]
                                //labelCheckbox " Remember me" [
                                //    Bind.attr("checked", model .> rememberMe, SetRememberMe >> dispatch)
                                //]
                            ]
                            bulma.field.div [
                                field.isGrouped
                                bulma.control.div [
                                    bulma.button.button [
                                        color.isSuccess
                                        Html.text "Login"
                                        onClick (fun _ -> dispatch AttemptLogin) [PreventDefault]
                                    ]
                                ]
                                bulma.control.div [
                                    bulma.button.button [
                                        Html.text "Cancel"
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
