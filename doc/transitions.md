This is a nice feature borrowed from Svelte.

A Sutil transition defines the behaviour of an element in response to a change in its visibility.

A simple example:

```
let view() =
    let visible = Store.make true

    Html.div [
        disposeOnUnmount [visible]

        Html.label [
            Html.input [
                type' "checkbox"
                Bind.attr ("checked",visible)
            ]
            text " visible"
        ]
        transition [InOut fade] visible <|
            Html.p [ text "Fades in and out" ]
    ]
```

Let's break down the transition here:

```
        transition [InOut fade] visible <|
            Html.p [ text "Fades in and out" ]
```

The format of `transition` is:

```
Transition.transition options visibility element
```

where:

| Name | Type | Options |
|----------------|---------------------------------|----------------------------------|
| `options` | `TransitionAttribute list` | Transition options, can be empty |
| `visibility` | `IObservable<bool>` | Visibility of the element over time |
| `element` | `SutilElement` | Element whose visibility is being controlled |


In the above example then, we have:

| Name | Value | Comments|
|------|-------|---------|
| `options`    | `[InOut fade` | Use `fade` for both `In` and `Out` transitions |
| `visibility` | `visible` | Declared as `let visible = Store.make true` |
| `element`    | `Html.p [ text "..." ]` | A simple text message wrapped in `<p>...</p>`|

An `In` transition is fired when `visibility` changes from `false` to `true`

An `Out` transition is fired when `visibility` changes from `true` to `false`

We can also see from our example that `visibilty` is toggled by the checkbox:

```
    Html.input [
        type' "checkbox"
        Bind.attr ("checked",visible)
    ]
```

So in summary, when we check the box, the `visibilty` changes.

The `transition` reacts to this by applying the `fade` effect to the `Html.p [ ... ]` element. The end result is that we see text fade in and out.

### Transition Functions

| Transition Function | Description |
|--|--|
| `fade`      | Uses `opacity` to effect the transition |
| `slide`     | A drop-down effect |
| `draw`      | For use with `SVGPathElement`. Appears to draw the SVG picture |
| `fly`       | Element flies in from a given start location |


Transition generators create pairs of `In` and `Out` transition functions.

| Transition Generators | Description |
|--|--|
| `crossfade` | Returns an `(in, out)` pair of transition functions. See `Todos.fs` in the `Animations` example |

Also, see [Svelte's crossfade docs](https://svelte.dev/docs#run-time-svelte-transition-crossfade)

### Transition Function Modifiers

Any transition function can be modified as follow:

```
    let flyIn = fly |> withProps [ Duration 2000.0; Y 200.0 ]
```

This can then be used as follows:
```
    transition [ InOut flyin ] visibility (Html.p "Hello world")
```

### TODO
```
TODO: Replicate this table in each transition function to show which apply to that function. These generalized descriptions don't work.
TODO: Document `flip`
TODO: Document `crossfade`
TODO: Document `animations`
```

| Property | Description |
|----------|-------------|
| `Key of string`                        | Used by `crossfade` to relate elements that are cross-fading |
| `X of float`                           | Used by `fly` to define initial X location |
| `Y of float`                           | Used by `fly` to define initial Y location |
| `Opacity of float`                     | Target opacity for `fly` |
| `Delay of float`                       | Milliseconds before effect starts |
| `Duration of float`                    | Duration of effect in milliseconds |
| `DurationFn of (float -> float)`       | Calculate duration from distance. Overrides `Duration`. Currently only used by `flip` animation. See below for brief note on animations |
| `Ease of (float -> float)`             | Easing function, such as `cubicInOut`, etc |
| `CssGen of (float -> float -> string)` | Custom CSS override. Function maps `t -> (1.0 - t) -> css` where `t` is the time variable from `[0..1` and `css` is a CSS style block. See example `TransitionCustomCss.fs` in example `Custom CSS` |
| `Tick of (float -> float -> unit)`     | Callback for clock ticks as transition executes. See example `Custom Code` |
| `Speed of float`                       | Speed of transition, though currently only used by `draw`. See example `Custom Code` for custom usage |
| `Fallback of TransitionBuilder`        | Override the transition used by `crossfade` (defaults to `fade`)|


### Transition Attributes

| Attribute | Description |
|--|--|
| `In`     | Transition function for `In` (visibility `false` to `true`) |
| `Out`    | Transition function for `Out` (visibility `true` to `false`) |
| `InOut`  | Transition function for both `In` and `Out` |

### Transition Events

| Event | Description |
|--|--|
| `"introstart"` | Start of `In` transition |
| `"introend"` | End of `In` transition |
| `"outrostart"` | Start of `Out` transition |
| `"outroend"` | End of `Out` transition |

Example (see `Transition events` for complete example):

```
        transition [fly |> withProps [ Duration 2000.0; Y 200.0 ] |> InOut] visible <|
            Html.p [
                on "introstart" (fun _ -> status <~ "intro started") []
                on "introend" (fun _ -> status <~ "intro ended") []
                on "outrostart" (fun _ -> status <~ "outro started") []
                on "outroend" (fun _ -> status <~ "outro ended") []
                text "Flies in and out"
            ]
```

### Animations

(to do - used by `Bind.each`. Document `flip` - `First, Last, Invert, Play`).

For now see


### Easing Functions

Easing functions specify the rate of change of a parameter over time. For example:

```
let quadIn t = t * t
```

See [mattdesl's eases](https://github.com/mattdesl/eases) for original source of these functions.

See [easings.net](https://easings.net) for visualizations

| Name | Description |
|--|--|
| `linear` | |
| `backInOut` |  |
| `backIn` |  |
| `backOut` |  |
| `cubicIn` | |
| `cubicOut` |  |
| `cubicInOut` |  |
| `quadInOut` |  |
| `quadIn` |  |
| `quadOut` |  |
| `quartInOut` |  |
| `quartInOut` |  |
| `quartOut` |  |
| `elasticInOut` |  |
| `elasticIn` |  |
| `elasticOut` |  |
| `elasticInOut` |  |
| `quintIn` |  |
| `quintOut` |  |
| `expoInOut` |  |
| `expoIn` |  |
| `expoOut` |  |


