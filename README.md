# Sutil [![Nuget](https://img.shields.io/nuget/v/Sutil.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Sutil)

An experiment in applying the design principles from [Svelte](https://svelte.dev/) to native Fable, mixed with [Elmish](https://github.com/elmish/elmish) and [Feliz](https://github.com/Zaid-Ajaj/Feliz). Sutil has no JS dependencies (such as React).

A Feliz-style DSL is supplied by [Feliz.Engine](https://github.com/alfonsogarciacaro/Feliz.Engine).
Sutil implements Elmish by importing parts of [Elmish](https://github.com/elmish/elmish).
Transitions and most examples are ported directly from [Svelte](https://svelte.dev/).

At first I thought that we might make use of a Fable compiler plugin to generate boilerplate, but it turns out that F# does a pretty good job of that itself.

See the [Sutil website](https://sutil.dev/) for [demos](https://sutil.dev/#examples-animation) and documentation for [getting started](https://sutil.dev/#documentation-installation)
