Follow these steps to create a new Sutil project starting from scratch:

- Install Fable template if you haven't already (check for latest template at [https://www.nuget.org/packages/Fable.Template/](https://www.nuget.org/packages/Fable.Template/)):

```shell
% dotnet new --install Fable.Template
```

- Create a new Fable project:

```shell
% mkdir myapp
% cd myapp
% dotnet new fable
```

- Add `Sutil` to the main project:

```shell
% cd src
% dotnet add package Sutil
```

If you experience build errors, then you may need to bring the Fable template packages up-to-date with those being referenced by
Sutil, or even just remove them (again from the `src` directory):

```shell
% dotnet remove package Fable.Browser.Dom
% dotnet remove package Fable.Core
```

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

app() |> Program.mount
```

- Build and run from the project root folder

```shell
npm install
npm run start
```

- View the app at http://localhost:8080
