module Sutil.Html

open Sutil.DOM
open Browser.Types

let div xs : NodeFactory = el "div" xs
let textarea xs = el "textarea" xs
let h1  xs = el "h1" xs
let h2  xs = el "h2" xs
let h3  xs = el "h3" xs
let h4  xs = el "h4" xs
let h5  xs = el "h5" xs
let hr  xs = el "hr" xs
let pre  xs = el "pre" xs
let code  xs = el "code" xs
let p  xs = el "p" xs
let span xs = el "span" xs
let button  xs = el "button" xs
let input  xs = el "input" xs
let label  xs = el "label" xs
let a  xs = el "a" xs
let ul  xs = el "ul" xs
let li xs = el "li" xs
let img xs = el "img" xs
let option xs = el "option" xs
let select xs = el "select" xs
let form xs = el "form" xs
let table xs = el "table" xs
let tbody xs = el "tbody" xs
let thead xs = el "thead" xs
let tr xs = el "tr" xs
let th xs = el "th" xs
let td xs = el "td" xs

let app (xs : seq<NodeFactory>) : NodeFactory = DOM.fragment xs
