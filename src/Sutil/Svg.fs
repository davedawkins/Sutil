namespace Sutil

open Sutil.Core
open Sutil.CoreElements
open CoreElements

///  <exclude />
module SvgInternal =
    let svgel (tag:string) (xs:seq<SutilElement>) =
        CoreElements.elns "http://www.w3.org/2000/svg" tag xs

    let svg xs : SutilElement = svgel "svg" xs
    let g xs : SutilElement = svgel "g" xs
    let rect xs : SutilElement = svgel "rect" xs
    let circle xs : SutilElement = svgel "circle" xs
    let pattern xs : SutilElement = svgel "pattern" xs
    let text xs : SutilElement = svgel "text" xs
    let line xs : SutilElement = svgel "line" xs

    let x obj = attr("x",obj)
    let y obj = attr("y",obj)
    let cx obj = attr("cx",obj)
    let cy obj = attr("cy",obj)
    let rx obj = attr("rx",obj)
    let ry obj = attr("ry",obj)
    let r obj = attr("r",obj)
    let x1 obj = attr("x1",obj)
    let y1 obj = attr("y1",obj)
    let x2 obj = attr("x2",obj)
    let y2 obj = attr("y2",obj)
    let width obj = attr("width",obj)
    let height obj = attr("height",obj)
    let transform obj = attr("transform",obj)


open Feliz
type SutilSvgEngine() =
    inherit SvgEngine<SutilElement>( SvgInternal.svgel, text, (fun () -> fragment []) )

    member _.x = SvgInternal.x
    member _.x1 = SvgInternal.x1
    member _.x2 = SvgInternal.x2

    member _.y = SvgInternal.y
    member _.y1 = SvgInternal.y1
    member _.y2 = SvgInternal.y2

    member _.width = SvgInternal.width
    member _.height = SvgInternal.height

    member _.transform = SvgInternal.transform

    member _.text (children : SutilElement seq) = SvgInternal.text children

[<AutoOpen>]
module SvgEngineHelpers =
    let Svg = SutilSvgEngine()
