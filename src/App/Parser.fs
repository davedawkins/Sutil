module Parser

// Created after reading a Scott Wlaschin post, as a learning exercise. Will probably upset you.
// TODO: Use Parsec.fs instead

open System
open System.Collections.Generic

type ParseResult<'T> =
    | Success of 'T
    | Error of string

module ParseResult =
    [<CompiledName("Map")>]
    let map f self =
        match self with
        | Error s -> Error s
        | Success t -> t |> f |> Success

    [<CompiledName("MapFst")>]
    let mapFst<'A,'B,'C> (f:'A -> 'C) (self:ParseResult<'A*'B>) : ParseResult<'C*'B> =
        self |> map (fun (a,b) -> (a|>f, b))

    [<CompiledName("MapSnd")>]
    let mapSnd f self =
        self |> map (fun (a,b) -> (a, b|>f))

    [<CompiledName("Bind")>]
    let bind f self =
        match self with
        | Error s -> Error s
        | Success t -> t |> f

type Parser<'T> = Parser of (string -> ParseResult<'T * string>)

let run parser input =
    let (Parser fn) = parser
    fn input

let skipWhite (input : string) = input.TrimStart()
let atEnd (input : string) = input = ""
let isChar c1 c2 = c1 = c2

let map f p =
    let inner input =
        let r = run p input
        match r with
        | Error msg -> Error msg
        | Success (v,i) -> Success (v |> f, i)
    Parser inner

let andThen first second =
    let inner input =
        match (run first input) with
        | Error msg -> Error msg
        | Success (value1, input') ->
            let result2 = run second input'
            match result2 with
            | Error msg -> Error msg
            | Success (value2, input'')
                -> Success((value1,value2), input'')
    Parser inner

let andThenDiscardFirst first second =
    let inner input =
        match (run first input) with
        | Error msg -> Error msg
        | Success (_, input') ->
            let result2 = run second input'
            match result2 with
            | Error msg -> Error msg
            | Success (value2, input'')
                -> Success(value2, input'')
    Parser inner

let andThenDiscardSecond first second =
    let inner input =
        match (run first input) with
        | Error msg -> Error msg
        | Success (value1, input') ->
            let result2 = run second input'
            match result2 with
            | Error msg -> Error msg
            | Success (_, input'')
                -> Success(value1, input'')
    Parser inner

let ( .>>. ) = andThen
let ( ..>. ) = andThenDiscardFirst
let ( .>.. ) = andThenDiscardSecond

let orElse first second =
    let inner input =
        match (run first input) with
        | Success result -> Success result
        | Error _ -> run second input
    Parser inner

let ( <|> ) = orElse

let parseKeyword (keyword:string) =
    let inner (input:string) =
        let input' = skipWhite input
        if input'.StartsWith(keyword) then
            //console.log(sprintf "keyword '%s', '%s'" keyword input'.[keyword.Length..])
            Success( keyword, input'.[keyword.Length..])
        else
            Error (sprintf "Keyword not found: %s" keyword)
    Parser inner

let parseCharLit (ch:char) =
    let inner (input:string) =
        if (input.Length = 0) || input.[0] <> ch then
            Error ($"Expected '{ch}'")
        else
            //console.log(sprintf "char '%c', '%s'" input.[0] input.[1..])
            Success(input.[0],input.[1..])
    Parser inner

let parseChar predicate name =
    let inner (input:string) =
        if (input.Length = 0) || not(predicate(input.[0])) then
            Error (sprintf "Not a %s" name)
        else
            //console.log(sprintf "char '%c', '%s'" input.[0] input.[1..])
            Success(input.[0],input.[1..])
    Parser inner

let parseDigit =
    parseChar Char.IsDigit "digit"

let parseWhite =
    parseChar Char.IsWhiteSpace "digit"

//let eatWhite =
//    let inner input =
//        Success((), input |> skipWhite )
//    Parser inner

let parseEnd =
    let inner input =
        if (atEnd input) then
            Success(char 0,input)
         else
            Error "Not EOF"
    Parser inner

let parseSequence elementParser =
    let rec accum state input =
        let r = run elementParser input
        match r with
        | Error msg ->
            Success (state, input)
        | Success (cmd,input') ->
            accum (state @ [cmd]) input'

    Parser (accum [])

let optional p =
    let inner input =
        let r = run p input
        match r with
        | Error _ -> Success(None, input)
        | Success (v,remainder) -> Success(Some v, remainder)
    Parser inner

let lookahead p =
    let inner input =
        run p input |> ParseResult.mapSnd (fun _ -> input)
    Parser inner

let choose parsers =
    parsers |> List.reduce (<|>)

let charsToNumber neg digits dec =
    let dval c = (int c - int '0')
    let m10 (n: float) (c : char) = n * 10.0 + (dval c |> float)
    let d10 (c : char) (n: float)  = (n  + (dval c |> float)) / 10.0

    let mant = List.foldBack d10 dec 0.0

    let n = ((digits |> List.fold m10 0.0) + mant) * (if neg then -1.0 else 1.0)
    n

let parseNatural =
    parseSequence parseDigit
    |> map (fun digits -> int (charsToNumber false digits []))

let parseInteger =
    (optional (parseChar (isChar '-') "negate"))
    .>>. (parseSequence parseDigit)
    |> map (fun (neg,digits) -> int (charsToNumber neg.IsSome digits []))

let parseNumber =
    let inner input =
        let p =
            (optional (parseChar (isChar '-') "negate"))
            .>>. (parseSequence parseDigit)
            .>>. (optional (parseChar (isChar '.') "point" .>>. parseSequence parseDigit))
            //.>>. (parseWhite <|> parseEnd)
        let r = run p (skipWhite input)
        match r with
        | Error msg -> Error msg
        | Success (((neg,digits),dec), remainder) ->
            if digits.IsEmpty then
                Error "Not a number"
            else
                let dval c = (int c - int '0')
                let m10 (n: float) (c : char) = n * 10.0 + (dval c |> float)
                let d10 (c : char) (n: float)  = (n  + (dval c |> float)) / 10.0

                let mant =
                    match dec with
                    | None -> 0.0
                    | Some (_,digits) -> List.foldBack d10 digits 0.0

                let n = ((digits |> List.fold m10 0.0) + mant) * (if neg.IsSome then -1.0 else 1.0)
                //console.log(sprintf "Number %f, '%s'" n remainder)
                Success( n, remainder)
    Parser inner

let parseString =
    let inner input =
        let q c = isChar c '"'
        let nq c = not (q c)

        let p = (parseChar q "quote") ..>. (parseSequence (parseChar nq "string character")) .>.. (parseChar q "quote")
        let r = run p (skipWhite input)
        match r with
        | Error msg -> Error msg
        | Success (chars, remainder) ->
            Success ( String( List.toArray chars ), remainder )
    Parser inner

let parseIdent =
    let inner input =
        let p = (parseChar Char.IsLetter "letter") .>>. (parseSequence (parseChar Char.IsLetterOrDigit "ident character"))
        let r = run p (skipWhite input)
        match r with
        | Error msg -> Error msg
        | Success ((first,chars) , remainder) ->
            Success (sprintf "%c%s" first (String( List.toArray chars )), remainder)
    Parser inner

let ignoreWhite p = (optional (parseSequence parseWhite)) ..>. p
