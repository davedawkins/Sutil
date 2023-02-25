/// <summary>
/// Easing functions for CSS transitions
/// </summary>
module Sutil.Easing
// Adapted from svelte/easing/index.js, which in turn are..
    (*
    Adapted from https://github.com/mattdesl
    Distributed under MIT License https://github.com/mattdesl/eases/blob/master/LICENSE.md
    *)

// For visualizations of these easing functions
// https://easings.net/

open System

let linear = id

let backInOut t =
    let s = 1.70158 * 1.525
    if (t < 0.5) then
        let tin = t * 2.0
        0.5 * (tin * tin * ((s + 1.0) * tin - s))
    else
        let tout = t - 1.0
        0.5 * (tout * tout * ((s + 1.0) * tout + s) + 2.0)

let backIn t =
    let s = 1.70158
    t * t * ((s + 1.0) * t - s)

let backOut t =
    let s = 1.70158
    let t' = t - 1.0
    t' * t' * ((s + 1.0) * t' + s) + 1.0

let cubicIn (t : float) = t * t * t

let cubicOut t =
    let f = t - 1.0
    f * f * f + 1.0

let cubicInOut t =
    if t < 0.5 then 4.0 * t * t * t else 0.5 * System.Math.Pow(2.0 * t - 2.0, 3.0) + 1.0

// todo; ported from JS, might read better if refactored. Might not run as fast though...do the refactor and see how it looks
let quadInOut t =
    let tin = t / 0.5;
    if (tin < 1.0) then
        0.5 * tin * tin // In: t < 0.5, tin = 0 .. 1
    else
        let tout = tin - 1.0  // Out: t>= 0.5, tout = 0 .. 1
        -0.5 * (tout * (tout - 2.0) - 1.0)

let quadIn (t : float) =
    t * t

let quadOut t =
    -t * (t - 2.0)

let quartIn t = Math.Pow(t,4.0)

let quartOut t =
    Math.Pow(t - 1.0, 3.0) * (1.0 - t) + 1.0;

let quartInOut t =
    if t < 0.5 then 8.0 * t * t * t * t else -8.0 * System.Math.Pow(t - 1.0, 4.0) + 1.0
    //return t < 0.5
    //    ? +8.0 * Math.pow(t, 4.0)
    //    : -8.0 * Math.pow(t - 1.0, 4.0) + 1.0;

let elasticIn t =
    Math.Sin((13.0 * t * Math.PI) / 2.0) * Math.Pow(2.0, 10.0 * (t - 1.0))

let elasticOut t =
    Math.Sin((-13.0 * (t + 1.0) * Math.PI) / 2.0) * Math.Pow(2.0, -10.0 * t) + 1.0

let quintIn (t:float) =
    t * t * t * t * t

let quintOut t =
    let t' = t - 1.0
    t' * t' * t' * t' * t' + 1.0

let expoInOut t =
    if t = 0.0 || t = 1.0 then
        t
    else if t < 0.5 then
        +0.5 * Math.Pow(2.0, 20.0 * t - 10.0)
    else
        -0.5 * Math.Pow(2.0, 10.0 - t * 20.0) + 1.0

let expoIn t =
    if t = 0.0 then t else Math.Pow(2.0, 10.0 * (t - 1.0))

let expoOut t =
    if t = 1.0 then t else 1.0 - Math.Pow(2.0, -10.0 * t)

(*
/*
Adapted from https://github.com/mattdesl
Distributed under MIT License https://github.com/mattdesl/eases/blob/master/LICENSE.md
*/
*DONE* function backInOut(t) {
    const s = 1.70158 * 1.525;
    if ((t *= 2) < 1)
        return 0.5 * (t * t * ((s + 1) * t - s));
    return 0.5 * ((t -= 2) * t * ((s + 1) * t + s) + 2);
}
*DONE* function backIn(t) {
    const s = 1.70158;
    return t * t * ((s + 1) * t - s);
}
*DONE* function backOut(t) {
    const s = 1.70158;
    return --t * t * ((s + 1) * t + s) + 1;
}
function bounceOut(t) {
    const a = 4.0 / 11.0;
    const b = 8.0 / 11.0;
    const c = 9.0 / 10.0;
    const ca = 4356.0 / 361.0;
    const cb = 35442.0 / 1805.0;
    const cc = 16061.0 / 1805.0;
    const t2 = t * t;
    return t < a
        ? 7.5625 * t2
        : t < b
            ? 9.075 * t2 - 9.9 * t + 3.4
            : t < c
                ? ca * t2 - cb * t + cc
                : 10.8 * t * t - 20.52 * t + 10.72;
}
function bounceInOut(t) {
    return t < 0.5
        ? 0.5 * (1.0 - bounceOut(1.0 - t * 2.0))
        : 0.5 * bounceOut(t * 2.0 - 1.0) + 0.5;
}
function bounceIn(t) {
    return 1.0 - bounceOut(1.0 - t);
}
function circInOut(t) {
    if ((t *= 2) < 1)
        return -0.5 * (Math.sqrt(1 - t * t) - 1);
    return 0.5 * (Math.sqrt(1 - (t -= 2) * t) + 1);
}
function circIn(t) {
    return 1.0 - Math.sqrt(1.0 - t * t);
}
function circOut(t) {
    return Math.sqrt(1 - --t * t);
}
*DONE* function cubicInOut(t) {
    return t < 0.5 ? 4.0 * t * t * t : 0.5 * Math.pow(2.0 * t - 2.0, 3.0) + 1.0;
}
*DONE* function cubicIn(t) {
    return t * t * t;
}
*DONE* function cubicOut(t) {
    const f = t - 1.0;
    return f * f * f + 1.0;
}
function elasticInOut(t) {
    return t < 0.5
        ? 0.5 *
            Math.sin(((+13.0 * Math.PI) / 2) * 2.0 * t) *
            Math.pow(2.0, 10.0 * (2.0 * t - 1.0))
        : 0.5 *
            Math.sin(((-13.0 * Math.PI) / 2) * (2.0 * t - 1.0 + 1.0)) *
            Math.pow(2.0, -10.0 * (2.0 * t - 1.0)) +
            1.0;
}
*DONE* function elasticIn(t) {
    return Math.sin((13.0 * t * Math.PI) / 2) * Math.pow(2.0, 10.0 * (t - 1.0));
}
*DONE* function elasticOut(t) {
    return (Math.sin((-13.0 * (t + 1.0) * Math.PI) / 2) * Math.pow(2.0, -10.0 * t) + 1.0);
}
function expoInOut(t) {
    return t === 0.0 || t === 1.0
        ? t
        : t < 0.5
            ? +0.5 * Math.pow(2.0, 20.0 * t - 10.0)
            : -0.5 * Math.pow(2.0, 10.0 - t * 20.0) + 1.0;
}
function expoIn(t) {
    return t === 0.0 ? t : Math.pow(2.0, 10.0 * (t - 1.0));
}
function expoOut(t) {
    return t === 1.0 ? t : 1.0 - Math.pow(2.0, -10.0 * t);
}
function quadInOut(t) {
    t /= 0.5;
    if (t < 1)
        return 0.5 * t * t;
    t--;
    return -0.5 * (t * (t - 2) - 1);
}
function quadIn(t) {
    return t * t;
}
function quadOut(t) {
    return -t * (t - 2.0);
}
function quartInOut(t) {
    return t < 0.5
        ? +8.0 * Math.pow(t, 4.0)
        : -8.0 * Math.pow(t - 1.0, 4.0) + 1.0;
}
function quartIn(t) {
    return Math.pow(t, 4.0);
}
function quartOut(t) {
    return Math.pow(t - 1.0, 3.0) * (1.0 - t) + 1.0;
}
function quintInOut(t) {
    if ((t *= 2) < 1)
        return 0.5 * t * t * t * t * t;
    return 0.5 * ((t -= 2) * t * t * t * t + 2);
}
*DONE* function quintIn(t) {
    return t * t * t * t * t;
}
*DONE* function quintOut(t) {
    return --t * t * t * t * t + 1;
}
function sineInOut(t) {
    return -0.5 * (Math.cos(Math.PI * t) - 1);
}
function sineIn(t) {
    const v = Math.cos(t * Math.PI * 0.5);
    if (Math.abs(v) < 1e-14)
        return 1;
    else
        return 1 - v;
}
function sineOut(t) {
    return Math.sin((t * Math.PI) / 2);
}



*DONE* Object.defineProperty(exports, 'linear', {
	enumerable: true,
	get: function () {
		return internal.identity;
	}
});

*)
