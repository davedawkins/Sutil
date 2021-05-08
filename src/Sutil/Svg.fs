module Sutil.Svg

open Sutil.DOM

let svgel (tag:string) (xs:seq<SutilElement>) =
    elns "http://www.w3.org/2000/svg" tag xs

let svg xs : SutilElement = svgel "svg" xs
let g xs : SutilElement = svgel "g" xs
let rect xs : SutilElement = svgel "rect" xs
let text xs : SutilElement = svgel "text" xs
let line xs : SutilElement = svgel "line" xs

let x obj = attr("x",obj)
let y obj = attr("y",obj)
let x1 obj = attr("x1",obj)
let y1 obj = attr("y1",obj)
let x2 obj = attr("x2",obj)
let y2 obj = attr("y2",obj)
let width obj = attr("width",obj)
let height obj = attr("height",obj)
let transform obj = attr("transform",obj)
