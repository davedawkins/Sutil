module Main

open Sutil
open Sutil.DOM
open Fable.Core
open Fable.Core.DynamicExtensions
open Fable.Core.JsInterop
open Browser.Types
open Browser.Dom
open Sutil.Attr
open System.Collections.Generic

importSideEffects "./styles.css"

let defineCustomElement (name: string, defaults: obj, el: obj) =
  importMember "./web-component.js"

type Stuff = { name: string; age: int }

[<AllowNullLiteral>]
type SampleElement =
  inherit HTMLElement
  abstract member name : string with get, set
  abstract member age : int with get, set

let view
  (props: IStore<SampleElement>)
  (attributes: Store<JS.Map<string, string>>)
  =
  let age = props .> (fun store -> store.age)

  let nameAttr =
    attributes .> (fun store -> store.get ("name"))

  let name =
    Store.zip nameAttr (props .> (fun store -> store.name))
    .> (fun (nameAttr, name) ->
      nameAttr
      |> Option.ofObj
      |> Option.orElse (Option.ofObj name)
      |> Option.defaultValue "")

  let onAttrChange =
    attributes.Subscribe(fun mp -> console.log (mp))

  Html.div [
    disposeOnUnmount [
      props
      attributes
      onAttrChange
    ]
    bindFragment2 name age
    <| (fun (name, age) -> text $"name: {name} age: {age}")
    Html.button [
      on
        "click"
        (fun _ ->
          props.Update
            (fun (el) ->
              el.age <- el.age + 1
              el))
        []
      text "Update age"
    ]
  ]

[<Emit("view")>]
let ViewRef: obj = jsNative

defineCustomElement ("my-element", { name = "Frank"; age = 0 }, ViewRef)
// same view, different component with different defaults
defineCustomElement ("my-element-2", { name = "Peter"; age = 10 }, ViewRef)
