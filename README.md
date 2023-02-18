# Sutil [![Nuget](https://img.shields.io/nuget/v/Sutil.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Sutil)

[Sutil](https://sutil.dev) is a web application framework for F#.

Its features are:

- Simple DOM builder, courtesy of [Feliz.Engine](https://github.com/alfonsogarciacaro/Feliz.Engine)
- No dependencies. Sutil is written entirely in F#, and so does not layer on top of another JS framework, such as React.
- Reactivity using IObservable and stores. Sutil does not use a virtual DOM.
- Support for Elmish (Model-View-Update) architecture.

In addition, Sutil inherits all the benefits of both Fable (F# with excellent JS interop) and F# itself.

Sutil was heavily inspired by Svelte, and imports several of its design features, such as stores and component styling. Some parts of Sutil are direct ports from Svelte.

## Development

To compile Sutil and build the main Sutil app in watch mode:

```shell
npm run start
```

To run tests:

```shell
npm run test
```

To deploy to https://sutil.dev (you will need to be Dave Dawkins for this to work). This will:
- build Sutil
- build documentation
- install to sutil.dev

```shell
npm run deploy:linode
```

## Building the REPL

Manually:

- ensure that repl/src/Fable.Repl.Lib/Fable.Repl.Lib.fsproj is up-to-date with the list of Sutil file names.
- review `repl/public/samples/samples.json` for examples that should be included or removed

Build the REPL and deploy (to sutil.dev if you're Dave Dawkins):

```shell
cd ../davedawkins/repl
./sutilbuild.sh
npm run deploy:linode
```


