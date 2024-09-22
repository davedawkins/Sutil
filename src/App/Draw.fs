// An experiment with a D3-like approach to drawing SVG items. So far, it's interesting but not
// yet seeing an advantage over what I've created over just using Svg.cirle, Svg.rect etc. The
// transition function is neat, at least. This isn't a judgment of D3 itself.
//
module Draw

open Sutil
open Sutil.Core
open Sutil.CoreElements
open Sutil.Styling

open Browser.CssExtensions

open Fable.Core
open type Feliz.length

let _log s = Fable.Core.JS.console.log(s)

module DrawCanvas =
    type Style( fill : string, stroke : string, width : float ) =
        do
            ()
        member _.FillStyle = fill
        member _.Apply( ctx : Browser.Types.CanvasRenderingContext2D )=
            ctx.fillStyle <- U3.Case1 fill
            ctx.strokeStyle <- U3.Case1 stroke
            ctx.lineWidth <- width

    type Circle(x : float, y : float, r : float, s : Style) =
        member _.Draw( ctx : Browser.Types.CanvasRenderingContext2D ) =
            s.Apply(ctx)
            ctx.arc( x, y, r, 0, 360 )


    open Browser.Types

    let initContext (ctx:CanvasRenderingContext2D) =
        let w = ctx.canvas.clientWidth
        let h = ctx.canvas.clientHeight

        let scale = 1.0 //window.devicePixelRatio; // Change to 1 on retina screens to see blurry canvas.

        ctx.canvas.width <- int (float w * scale)
        ctx.canvas.height <- int (float h * scale)

        ctx.scale(scale, scale)

    let drawingCanvas (width : int) (height :int) (drawing : Circle) =
        Html.canvas [
            Attr.style [
                Css.width (px width)
                Css.height (px height)
            ]
            onMount (fun e ->
                let cvs = e.target :?> Browser.Types.HTMLCanvasElement
                let ctx = cvs.getContext_2d()

                DomHelpers.interval (fun _ ->
                    initContext ctx
                    ctx.beginPath()
                    drawing.Draw(ctx)
                    ctx.fill()
                    ctx.stroke()
                ) 100 |> ignore
            ) []
        ]


module SvgCanvas =
    open System

    let zip3 (a : IObservable<'a>) (b : IObservable<'b>) (c : IObservable<'c>) =
        Observable.zip (Observable.zip a b) c |> Observable.map (fun ((a,b),c) -> a,b,c)

    type Val<'T> = 'T

    type StyleAttrs =
        | Fill of Val<string>
        | Stroke of Val<string>
        | StrokeWidth of Val<float>
        | Class of Val<string>
        | ToggleClass of Val<string>
    with
        member __.Name =
            match __ with
            | Fill _ -> "fill"
            | Stroke _ -> "stroke"
            | StrokeWidth _ -> "stroke-width"
            | Class _ -> "class"
            | ToggleClass _ -> "sutil-toggle-class"

        member __.Element =
            match __ with
            | Fill v -> Attr.custom( "fill", v )
            | Stroke v -> Attr.stroke v
            | StrokeWidth v -> Attr.strokeWidth (px v)
            | Class s -> Attr.className s
            | ToggleClass s -> Attr.custom("sutil-toggle-class", s)

    type Viewable =
        abstract View: unit -> SutilElement

    type Updatable<'Attr,'Event> =
        abstract Update: 'Attr seq * StyleAttrs seq * 'Event seq -> unit
        abstract GetAttrValue<'T> : string -> 'T

    type ShapeAttr<'S> =
        abstract Name : string
        abstract Element : SutilElement
        abstract AsFloat : float
        abstract AsCtor : (float -> 'S)

    type GroupAttrs =
        | Nop of Val<float>
        member __.Name =
            match __ with
            | Nop _ -> "cx"
        member __.Element =
            match __ with
            | Nop v -> Attr.custom("nop",string v)
        member __.AsFloat =
            match __ with
            | Nop v -> v
        member __.AsCtor : float -> GroupAttrs =
            match __ with
            | Nop _ -> Nop
        override __.ToString() = __.Name
        interface ShapeAttr<GroupAttrs> with
            member __.Name = __.Name
            member __.Element = __.Element
            member __.AsFloat = __.AsFloat
            member __.AsCtor = __.AsCtor

    type CircleAttrs =
        | Cx of Val<float>
        | Cy of Val<float>
        | R of Val<float>
        member __.Name =
            match __ with
            | Cx _ -> "cx"
            | Cy _ -> "cy"
            | R _ -> "r"
        member __.Element =
            match __ with
            | Cx v -> Attr.cx (px v)
            | Cy v -> Attr.cy (px v)
            | R v -> Attr.r (px v)
        member __.AsFloat =
            match __ with
            | Cx v -> v
            | Cy v -> v
            | R v -> v
        member __.AsCtor : float -> CircleAttrs =
            match __ with
            | Cx _ -> Cx
            | Cy _ -> Cy
            | R _ -> R
        override __.ToString() = __.Name
        interface ShapeAttr<CircleAttrs> with
            member __.Name = __.Name
            member __.Element = __.Element
            member __.AsFloat = __.AsFloat
            member __.AsCtor = __.AsCtor


    type RectAttrs =
        | X of Val<float>
        | Y of Val<float>
        | Width of Val<float>
        | Height of Val<float>
        | Rx of Val<float>
        | Ry of Val<float>
        member __.Name =
            match __ with
            | X _ -> "x"
            | Y _ -> "y"
            | Width _ -> "width"
            | Height _ -> "height"
            | Rx _ -> "rx"
            | Ry _ -> "ry"
        member __.Element =
            match __ with
            | X v -> Attr.x (px v)
            | Y v -> Attr.y (px v)
            | Width v -> Attr.width (px v)
            | Height v -> Attr.height (px v)
            | Rx v -> Attr.rx (px v)
            | Ry v -> Attr.ry (px v)
        member __.AsFloat =
            match __ with
            | X v -> v
            | Y v -> v
            | Width v -> v
            | Height v -> v
            | Rx v -> v
            | Ry v -> v
        member __.AsCtor : float -> RectAttrs =
            match __ with
            | X _ -> X
            | Y _ -> Y
            | Width _ -> Width
            | Height _ -> Height
            | Rx _ -> Rx
            | Ry _ -> Ry
        override __.ToString() = __.Name
        interface ShapeAttr<RectAttrs> with
            member __.Name = __.Name
            member __.Element = __.Element
            member __.AsFloat = __.AsFloat
            member __.AsCtor = __.AsCtor

    //type EventHandler<'A> = (Updatable<'A, ShapeEvent<'A> > -> unit)

    type ShapeEvent<'Shape> =
        | OnClick of ('Shape -> unit)
        | OnMouseOver of ('Shape -> unit)
        | OnMouseOut of ('Shape -> unit)
        | OnMouseEnter of ('Shape -> unit)
        | OnMouseLeave of ('Shape -> unit)
    with
        member __.Name =
            match __ with
            | OnClick _ -> "click"
            | OnMouseOver _ -> "mouseover"
            | OnMouseOut _ -> "mouseout"
            | OnMouseEnter _ -> "mouseenter"
            | OnMouseLeave _ -> "mouseleave"

    type Shape<'A,'S when 'A :> ShapeAttr<'A> and 'S :> Shape<'A,'S> >(attrs : 'A seq, style : StyleAttrs seq, events : ShapeEvent<'S> seq) =
        let mutable attrMap : Map<string,SutilElement> = Map.empty
        let updateTick = Store.make 0
        let mutable eventMap : Map<string,ShapeEvent<'S>> = Map.empty
        let mutable attrValues : Map<string,obj> = Map.empty

        let _self (this : Shape<'A,'S>) : 'S = this :?> 'S

        let redraw() =
            updateTick |> Store.modify ((+)1)

        let update (attrs : 'A seq) (style : StyleAttrs seq ) (events : ShapeEvent<'S> seq)=
            let m1 : Map<string,SutilElement> = attrs |> Seq.fold (fun s a -> s.Add( a.Name, a.Element )) attrMap
            let m2 : Map<string,SutilElement> = style |> Seq.fold (fun s a -> s.Add( a.Name, a.Element )) m1

            attrValues <- attrs |> Seq.fold (fun s a -> s.Add(a.Name,a.AsFloat)) attrValues

            attrMap <- m2
            eventMap <- events |> Seq.fold (fun s a -> s.Add(a.Name,a)) eventMap

            redraw()
        do
            update attrs style events

        // member this.Draw( el : unit -> SutilElement ) =
        //     Bind.el( this.UpdateTick, fun _ -> el() )

        member internal _.UpdateTick = updateTick
        member internal _.ChildElements = (attrMap |> Seq.map (fun kv -> kv.Value))
        member internal _.FindEvent( name : string ) = eventMap.TryFind name
        member internal _.EventElements = eventMap
        member internal _.Redraw() = redraw()

        abstract member Update: 'A seq * StyleAttrs seq * ShapeEvent<'S> seq -> unit
        default _.Update( attrs : 'A seq, style : StyleAttrs seq, events : ShapeEvent<'S> seq ) = update attrs style events

        member __.Class( c ) = __.Update( [], [ Class c ], []); _self __
        member __.ToggleClass( c ) = __.Update( [], [ ToggleClass c ], []); _self __
        member __.Stroke( color ) = __.Update( [], [ Stroke color ], []); _self __
        member __.Fill( color ) = __.Update( [], [ Fill color ], []); _self __
        member __.StrokeWidth( w ) = __.Update( [], [ StrokeWidth w ], []); _self __
        member __.OnClick(f) = __.Update( [], [], [ OnClick f ]); _self __
        member __.OnMouseEnter(f) = __.Update( [], [], [ OnMouseEnter f ]); _self __
        member __.OnMouseLeave(f) = __.Update( [], [], [ OnMouseLeave f ]); _self __
        member __.OnMouseOver(f) = __.Update( [], [], [ OnMouseOver f ]); _self __
        member __.OnMouseOut(f) = __.Update( [], [], [ OnMouseOut f ]); _self __

        member __.End() = ()

        interface Updatable<'A,ShapeEvent<'S>> with
            member _.Update( attrs : 'A seq, style : StyleAttrs seq, events : ShapeEvent<'S> seq ) = update attrs style events
            member _.GetAttrValue<'T>( name ) =
                    //_log( sprintf "get: %s -> %A" name (attrValues) )
                    attrValues.TryFind(name)
                    |> Option.map (fun ob -> ob :?> 'T)
                    |> Option.defaultValue (Unchecked.defaultof<'T>)


    let interp v  a  b  c  d  =
        ((v - a) / b) * (d - c) + c

    let transition (u : Updatable<'A,'E>) (b : float) (durationMs : int) (attr : float -> 'A ) =
        let mutable start = -1.0
        let name = (attr 0.0).ToString()
        let a : float = u.GetAttrValue(name)

        //_log("transition: name = " + name)

        let update t =
            let v = interp (Easing.quadIn t) 0.0 1.0 a b
            u.Update( [ attr v ], [], [] )

        let rec next (ts : float) =
            if start < 0 then start <- ts

            let elapsed = ts - start

            if elapsed >= durationMs then
                update 1.0
            else
                update (interp (float elapsed) 0. (float durationMs) 0.0 1.0)
                DomHelpers.raf next |> ignore

        DomHelpers.raf next |> ignore

    type Circle( attrs : CircleAttrs seq, style : StyleAttrs seq, events : ShapeEvent<Circle> seq ) =
        inherit Shape<CircleAttrs,Circle>(attrs,style,events)

        member __.Cx( cx ) = __.Update( [ Cx cx], [], []); __
        member __.Cy( cy ) = __.Update( [ Cy cy ], [], []); __
        member __.R( r )   = __.Update( [ R r ], [ ], []); __

        new() = Circle([],[],[])

        interface Viewable with
            member this.View() =
                Svg.circle [
                    Bind.el( this.UpdateTick, fun _ ->
                    fragment [
                        yield!
                            this.ChildElements
                        yield!
                            this.EventElements
                            |> Map.toSeq
                            |> Seq.map( fun (_,e) ->
                                        match e with
                                        | OnClick f -> Ev.onClick (fun _ -> f this)
                                        | OnMouseOver f -> Ev.onMouseOver (fun _ -> f this)
                                        | OnMouseOut f -> Ev.onMouseOut (fun _ -> f this)
                                        | OnMouseEnter f -> Ev.onMouseEnter (fun _ -> f this)
                                        | OnMouseLeave f -> Ev.onMouseLeave (fun _ -> f this)
                                )
                    ])
                ]


    type Rect( attrs : RectAttrs seq, style : StyleAttrs seq, events : ShapeEvent<Rect> seq ) =
        inherit Shape<RectAttrs,Rect>(attrs,style,events)

        member __.X( x ) = __.Update( [ X x], [], []); __
        member __.Y( y ) = __.Update( [ Y y ], [], []); __
        member __.Width( w ) = __.Update( [ Width w ], [], []); __
        member __.Height( h ) = __.Update( [ Height h ], [], []); __
        member __.Rx( r )   = __.Update( [ Rx r ], [ ], []); __
        member __.Ry( r )   = __.Update( [ Ry r ], [ ], []); __

        new() = Rect([],[],[])

        interface Viewable with
            member this.View() =
                Svg.rect [
                    Bind.el( this.UpdateTick, fun _ ->
                    fragment [
                        yield!
                            this.ChildElements
                        yield!
                            this.EventElements
                            |> Map.toSeq
                            |> Seq.map( fun (_,e) ->
                                        match e with
                                        | OnClick f -> Ev.onClick (fun _ -> f this)
                                        | OnMouseOver f -> Ev.onMouseOver (fun _ -> f this)
                                        | OnMouseOut f -> Ev.onMouseOut (fun _ -> f this)
                                        | OnMouseEnter f -> Ev.onMouseEnter (fun _ -> f this)
                                        | OnMouseLeave f -> Ev.onMouseLeave (fun _ -> f this)
                                )
                    ])
                ]

    type Group( attrs : GroupAttrs seq, style : StyleAttrs seq, events : ShapeEvent<Group> seq ) =
        inherit Shape<GroupAttrs,Group>(attrs,style,events)

        let mutable _children : Viewable[] = [| |]

        do ()

        member __.Nop( cx ) = __.Update( [ Nop cx], [], []); __
        member __.Children( children : Viewable seq ) = _children <- children |> Seq.toArray; __.Redraw(); __

        new() = Group([],[],[])

        interface Viewable with
            member this.View() =
                Svg.g [
                    Bind.el( this.UpdateTick, fun _ ->
                    fragment [
                        yield!
                            this.ChildElements
                        yield!
                            this.EventElements
                            |> Map.toSeq
                            |> Seq.map( fun (_,e) ->
                                        match e with
                                        | OnClick f -> Ev.onClick (fun _ -> f this)
                                        | OnMouseOver f -> Ev.onMouseOver (fun _ -> f this)
                                        | OnMouseOut f -> Ev.onMouseOut (fun _ -> f this)
                                        | OnMouseEnter f -> Ev.onMouseEnter (fun _ -> f this)
                                        | OnMouseLeave f -> Ev.onMouseLeave (fun _ -> f this)
                                )
                        yield!
                            _children |> Array.map (fun c -> c.View())
                    ])
                ]

    type CircleTransition( circle : Circle ) =
        inherit Circle([],[],[])
        let mutable duration = 0
        let mutable delay = 0

        member __.Duration( d : int ) = duration <- d; __
        member __.Delay( d : int ) = delay <- d; __
        member __.TransitionEnd() = circle

        override __.Update( attrs, style, events) =
            attrs |> Seq.iter( fun a ->
                DomHelpers.timeout (fun () -> transition circle (a.AsFloat) duration (a.AsCtor)) delay
                    |> ignore // TODO: Save this so that we can cancel if a new transition occurs?
            )
            circle.Update([],[],events)


    type RectTransition( rect : Rect ) =
        inherit Rect([],[],[])
        let mutable duration = 0
        let mutable delay = 0

        member __.Duration( d : int ) = duration <- d; __
        member __.Delay( d : int ) = delay <- d; __
        member __.TransitionEnd() = rect

        override __.Update( attrs, style, events) =
            attrs |> Seq.iter( fun a ->
                DomHelpers.timeout (fun () -> transition rect (a.AsFloat) duration (a.AsCtor)) delay
                    |> ignore // TODO: Save this so that we can cancel if a new transition occurs?
            )
            rect.Update([],[],events)

    type GroupTransition( group : Group ) =
        inherit Group([],[],[])
        let mutable duration = 0
        let mutable delay = 0

        member __.Duration( d : int ) = duration <- d; __
        member __.Delay( d : int ) = delay <- d; __
        member __.TransitionEnd() = group

        override __.Update( attrs, style, events) =
            attrs |> Seq.iter( fun a ->
                DomHelpers.timeout (fun () -> transition group (a.AsFloat) duration (a.AsCtor)) delay
                    |> ignore // TODO: Save this so that we can cancel if a new transition occurs?
            )
            group.Update([],[],events)

    [<AutoOpen>]
    module Helpers =
        type Circle with
            member __.Transition() = CircleTransition(__)
        type Rect with
            member __.Transition() = RectTransition(__)
        type Group with
            member __.Transition() = GroupTransition(__)

    let drawingCanvas (width : int) (height : int) (drawing : Viewable) =
        Svg.svg [
            Attr.style [
                Css.width (px width)
                Css.height (px height)
            ]
            (drawing.View ())
        ]

open SvgCanvas

let view() =
    //let tick = Store.make 0.0

    let rec trans0 (c : Circle) =
        c.Transition()
            .Duration(400)
            .R(10)
            .Cx(350)
            .OnClick(trans2)
            .End()

    and trans2 (c : Circle ) =
        c.Transition()
            .Duration(400)
            .R(100)
            .Cx(200)
            .OnClick(trans0)
            .End()

    let c1 = Circle()
    let c2 = Circle()

    c2
        .Cx(100)
        .Cy(200)
        .R(50)
        .Fill("lime")
        .Stroke("black")
        .StrokeWidth(2.0)
        .End()

    c1
        .Cx(200)
        .Cy(200)
        .R(100)
        .Fill("gray")
        .Stroke("black")
        .StrokeWidth(4.0)
        .OnClick(trans0)
        .OnMouseEnter( fun c -> c.Stroke("red").Fill("pink").End() )
        .OnMouseLeave( fun c -> c.Stroke("yellow").Fill("gray").End() )
        .End()

    let cardHeight = 1122.0 / 8.0

    let rec collapseCard (r : Rect) =
        r
            .OnClick( expandCard )
            .Transition()
            .Duration(200)
            .Height(20)
            .End()

    and expandCard (r : Rect) =
        r
            .OnClick( collapseCard )
            .Transition()
            .Duration(200)
            .Height(cardHeight)
            .End()

    // https://www.cia.edu/blog/2014/10/designing-playing-cards
    let card = Rect()
    card
        .Class("card")
        .Width(822.0 / 8.0)
        .Height(1122.0 / 8.0)
        .X(100)
        .Y(200)
        .Rx(10)
        .Ry(10)
        .Fill("white")
        .Stroke("gray")
        .StrokeWidth(1)
        .OnClick( fun r ->
            r
                .ToggleClass("collapsed")
                .End()
        )
        .End()

    let g = Group().Children( [ c2; c1; card ])

    let drawing = g

    Html.div [
        //disposeOnUnmount [
        //    DomHelpers.interval (fun _ -> tick |> Store.modify ((+) 1.0)) 40 |> Helpers.disposable
        //]
        drawing |> drawingCanvas 400 400
    ] |> Sutil.Styling.withStyle [
        rule ".card" [
            Css.transitionDurationMilliseconds (200)
            Css.height (px cardHeight)
        ]
        rule ".card.collapsed" [
            Css.transitionDurationMilliseconds (200)
            Css.height (px 20)
        ]
    ]
