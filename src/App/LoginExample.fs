module LoginExample

open Sutil
open Sutil.Bulma
open Sutil.DOM
open Sutil.Attr
open Sutil.Bindings
open Sutil.Transition

open Login
open Sutil.Styling

module MockServer =
    // TODO: Make this function asynchronous
    // What's an idiomatic return type?  Eg
    //   Async<Result<'AuthToken,'Error>> ?
    let login username password =
        if (username = "sutil@gmail.com" && password = "abc123") then
            "anAuthToken"
        else
            failwith "Login details not found or incorrect"

// App views
type Page = Main | Login

// MVU types
type AuthorisedUser = {
    Name : string
    AuthToken : string }

type Message =
    | SetUser of string * string
    | SignIn
    | SignOut
    | CancelSignIn

type Model = {
    User : AuthorisedUser option
    Page : Page }

// Model helpers
let loggedInName defaultName m =
    match m.User with
    | Some u -> u.Name
    | _ -> defaultName

let isLoggedIn m = m.User.IsSome
let page m = m.Page

let private appStyleSheet = [
    rule "a" [
        Css.color "gray"
    ]
]

// MVU / Elmish functions
let private init() = { User = None; Page = Main }

let private update msg model =
    match msg with
    | SetUser (name,token) ->
        { model with Page = Main; User = Some { Name = name; AuthToken = token }}
    | SignIn ->
        { model with Page = Login }
    | SignOut ->
        { model with Page = Main; User = None }
    | CancelSignIn ->
        { model with Page = Main }

let create() =
    let model, dispatch = () |> Store.makeElmishSimple init update ignore

    bulma.container [
        disposeOnUnmount [ model ]

        bind (model .> page) <| fun p ->
            match p with
            | Main ->
                bulma.section [

                    bind model <| fun m ->
                        match m.User with
                        | Some u ->
                            Html.span [
                                text u.Name
                                text " "
                                Html.a [
                                    text "sign out"
                                    href "#"
                                    onClick (fun _ -> dispatch SignOut) [PreventDefault]
                                ]
                            ]
                        | None ->
                            Html.span [
                                text "guest "
                                Html.a [
                                    text "sign in"
                                    href "#"
                                    onClick (fun _ -> dispatch SignIn) [PreventDefault]
                                ]
                            ]
                ]
            | Login ->
                let onLogin details =
                    let authToken = MockServer.login details.Username details.Password
                    (details.Username, authToken) |> SetUser |> dispatch
                let onCancel() = dispatch CancelSignIn
                Html.div [
                    bind model <| fun m ->
                        let init = { LoginDetails.Default with Username = loggedInName "" m }
                        Login.create init onLogin onCancel
                ]
        ] |> withStyle appStyleSheet