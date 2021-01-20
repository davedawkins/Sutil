module Sutil.Easing

open System

// Adapted from svelte/easing/index.js
(*
Adapted from https://github.com/mattdesl
Distributed under MIT License https://github.com/mattdesl/eases/blob/master/LICENSE.md
*)

let linear = id

let cubicIn t = t * t * t

let cubicOut t =
    let f = t - 1.0
    f * f * f + 1.0

let cubicInOut t =
    if t < 0.5 then 4.0 * t * t * t else 0.5 * System.Math.Pow(2.0 * t - 2.0, 3.0) + 1.0

let elasticIn t =
    Math.Sin((13.0 * t * Math.PI) / 2.0) * Math.Pow(2.0, 10.0 * (t - 1.0))

let elasticOut t =
    Math.Sin((-13.0 * (t + 1.0) * Math.PI) / 2.0) * Math.Pow(2.0, -10.0 * t) + 1.0

let quintIn (t:float) =
    t * t * t * t * t

let quintOut t =
    let t' = t - 1.0
    t' * t' * t' * t' * t' + 1.0