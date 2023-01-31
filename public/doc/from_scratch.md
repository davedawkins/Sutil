Follow these steps to create a new Sutil project starting from scratch:

- Install Fable template if you haven't already (check for latest template at [https://www.nuget.org/packages/Fable.Template/](https://www.nuget.org/packages/Fable.Template/)):

```shell
% dotnet new --install Fable.Template::3.2.1
```

- Create a new Fable project:

```shell
% dotnet new fable
```

- Add `Sutil` to the main project:

```shell
% cd src
% dotnet add package Sutil --version 2.0.0
```
Note: Check https://www.nuget.org/packages/Sutil/ for latest version.

- Edit `public/index.html` and change `body` element to read as follows:

```html
<body>
    <div id="sutil-app"></div>
    <script src="bundle.js"></script>
</body>
```

- Edit `App.fs` to read as follows:

```fsharp
module App

open Sutil

let app() =
    Html.div "Hello World"

app() |> Program.mountElement "sutil-app"
```
