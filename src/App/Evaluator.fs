module Evaluator

#nowarn "40" // Suppress warning about delayed reference for checking recursion

open Parser
type Position = char * int

let positionStr p = $"{fst p}{snd p}"

type Expr =
  | Number of float
  | Reference of Position
  | Binary of Expr * char * Expr
  | Sub of Expr

let parseNumber = ignoreWhite parseNumber |> map Number
let parseReference = (parseChar (fun c -> c >= 'A' && c <= 'Z') "col") .>>. parseNatural |> map Reference
let parseMulOp = ignoreWhite (parseCharLit '*' <|> parseCharLit '/' <|> parseCharLit '%')
let parseAddOp = ignoreWhite (parseCharLit '+' <|> parseCharLit '-')

let private buildBinTree<'T> r : ParseResult<Expr*string> =
    let mapper (f,optTail) =
            match optTail with
                | None -> f // Was just factor like 1.0 or x
                | Some tail -> tail |> (List.fold (fun a (op,b) -> Binary(a,op,b)) f)
    r |> ParseResult.mapFst mapper

let rec parseFactor =
    ignoreWhite (parseNumber <|> parseReference <|> parseSubExpr)

and parseSubExpr = parseCharLit '(' ..>. parseExpr .>.. parseCharLit ')'
and parseMulExpr =
    let inner input =
        let p = parseFactor .>>. ((parseMulOp .>>. parseFactor) |> parseSequence |> optional)
        run p input |> buildBinTree
    Parser inner
and parseAddExpr =
    let inner input =
        let p = parseMulExpr
                    .>>. ((parseAddOp .>>. parseMulExpr) |> parseSequence |> optional)
        run p input |> buildBinTree
    Parser inner
and parseExpr =
    ignoreWhite parseAddExpr

// Handle '= ..." expressions and treat everything else as a numeric literal
let parseCellExpr =
    (parseCharLit '=' ..>. parseExpr) <|> parseNumber

// Railway-nadic bind adapter
let (>>=) ma f = ParseResult.bind f ma

// Only allow success when all input has been consumed. Eg, "10 {" parses "10" and gives us " {" as a remainder
let parseToEnd p =
    let inner input =
        run p input >>= (fun (e,r) ->
                            if r |> skipWhite |> atEnd
                                then Success (e,r)
                                else ParseResult.Error $"Syntax error at {r}")
    Parser inner

let parseToEndWith p input =
    run (parseToEnd p) input >>= (fun (e,r) -> Success e)

let rec evalExpr (sheet : Position -> string) e =
    match e with
    | Sub e -> evalExpr sheet e
    | Number n -> Success n
    | Binary (a,op,b) ->
        let ops = dict [ '+', (+); '-', (-); '*', (*); '/', (/); '%', (%) ]
        (evalExpr sheet a) >>= (fun av -> evalExpr sheet b >>= (fun bv -> Success (ops.[op] av bv)))
    | Reference pos ->
        let content = sheet pos
        run (parseToEnd parseCellExpr) content >>= (fun (e,_) -> evalExpr sheet e)

let evalCellAsString sheet content =
    let result = run (parseToEnd parseCellExpr) content >>= (fun (e,r) -> evalExpr sheet e)
    match result with
    | Success f -> string f
    | ParseResult.Error _ -> content // TODO: Show errors if '= <junk>' seen


let findTriggerCells expr =
    let ast = Parser.run parseCellExpr expr
    match ast with
    | Parser.Error _ -> []
    | Parser.Success (e,_) ->
        let rec walk e result =
            match e with
            | Number _ -> result
            | Reference p -> p :: result
            | Sub e -> walk e result
            | Binary (left,_,right) -> walk left (walk right result)
        walk e []
