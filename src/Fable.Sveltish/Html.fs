module Sveltish.Html

open Sveltish.DOM
open Browser.Types

let div xs = el "div" xs
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

let app (xs : seq<NodeFactory>) : NodeFactory = fun (ctx,parent) ->
    let mutable last : Node = parent
    for x in xs do
        last <- x(ctx,parent)
    last

