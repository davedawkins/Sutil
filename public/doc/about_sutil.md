## What is Sutil [![Nuget](https://img.shields.io/nuget/v/Sutil.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Sutil)

Sutil is a web application framework for F#.

Its features are:
- Simple DOM builder, courtesy of [Feliz.Engine](https://github.com/alfonsogarciacaro/Feliz.Engine)
- No dependencies. Sutil is written entirely in F#, and so does not layer on top of another JS framework, such as React.
- Reactivity using IObservable and stores. Sutil does not use a virtual DOM.
- Support for Elmish (Model-View-Update) architecture.

In addition, Sutil inherits all the benefits of both Fable (F# with excellent JS interop) and F# itself.



Sutil was heavily inspired by Svelte, and imports several of its design features, such as stores and component styling. Some parts of Sutil are direct ports from Svelte.



Sutil is currently in alpha, which means it's safe enough for you to play around with, but expect breaking changes.
