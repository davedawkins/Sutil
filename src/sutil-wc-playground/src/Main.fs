module Main

open Sutil
open Sutil.DOM
open Fable.Core
open Fable.Core.DynamicExtensions
open Fable.Core.JsInterop
open Browser.Types
open Browser.Dom
open Sutil.Attr

importSideEffects "./styles.css"

let defineCustomElement (name: string, defaults: obj, el: obj) =
  importMember "./web-component.js"

type Stuff = { name: string; age: int }

let view (props: Stuff) =
  let store = Store.make props
  let age = store .> (fun store -> store.age)
  let name = store .> (fun store -> store.name)

  Html.div [
    disposeOnUnmount [ store ]
    bindFragment2 name age
    <| (fun (name, age) -> text $"name: {name} age: {age}")
    Html.button [
      on
        "click"
        (fun _ ->
          Store.modify (fun store -> { store with age = store.age + 1 }) store)
        []
      text "Update age"
    ]
  ]

defineCustomElement ("my-element", { name = "Frank"; age = 0 }, view)
// same view, different component with different defaults
defineCustomElement ("my-element-2", { name = "Peter"; age = 10 }, view)
