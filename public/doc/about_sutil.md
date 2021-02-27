## What is Sutil [![Nuget](https://img.shields.io/nuget/v/Sutil.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Sutil)

Sutil was originally named Sveltish, and that's because it was inspired by [Svelte](https://svelte.dev/).

Sutil's features are:

- [Feliz](https://github.com/Zaid-Ajaj/Feliz)-flavoured DOM builder
- [Elmish](https://github.com/elmish/elmish)
- No dependencies (such as React)
- Easy Svelte transitions
- Reactivity based on IObservable and Stores
- Component styling

Sutil is currently in alpha, which means it's safe enough for you to play around with, but expect breaking changes.

A Feliz-style DSL is supplied by [Feliz.Engine](https://github.com/alfonsogarciacaro/Feliz.Engine)
Sutil implements Elmish by importing parts of [Elmish](https://github.com/elmish/elmish)
Transitions and most examples are ported directly from [Svelte](https://svelte.dev/)

At first I thought that we might make use of a Fable compiler plugin to generate boilerplate, but it turns out that F# does a pretty good job of that itself.

See the [Sutil website](https://davedawkins.github.io/Sutil/) for [demos](https://davedawkins.github.io/Sutil/#examples-animation) and documentation for [getting started](https://davedawkins.github.io/Sutil/#documentation-installation)

*(more coming soon, I'm actively working on building this doc tree into the example app)*
