module ConstructStyleSheetsPolyfill

open Fable.Core.JsInterop

importSideEffects "./adoptedStyleSheets.js"

let register() = ()