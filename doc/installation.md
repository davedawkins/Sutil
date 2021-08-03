You have several options in getting started with Sutil.

## Clone a Sutil starter template

### Hello World

The simplest Sutil application. Instructions on installation here:

https://github.com/davedawkins/sutil-template-helloworld

### Elmish MVU

The recommended approach for any kind of real application

https://github.com/davedawkins/sutil-template-elmish

### App

A template that incorporates commonly required features

https://github.com/davedawkins/sutil-template-app

### From Scratch

- Install Fable template if you haven't already (check for latest template at https://www.nuget.org/packages/Fable.Template/):
```
% dotnet new --install Fable.Template::3.2.1
```

- Create a new Fable project:
```
% dotnet new fable
```

- Add `Sutil` to the main project:
```
% cd src
% dotnet add package Sutil --version 1.0.0-alpha-006
```
Note: Sutil is still in alpha, so a specific version must be specified. Check https://www.nuget.org/packages/Sutil/ for latest version.

- Edit `public/index.html` and change `body` element to read as follows:
```
<body>
    <div id="sutil-app"></div>
    <script src="bundle.js"></script>
</body>
```

- Edit `App.fs` to read as follows:
```
    module App

    open Sutil

    let app() =
        Html.div "Hello World"

    app() |> DOM.mountElement "sutil-app"
```

*Note* `mountElement` has moved from `DOM` to `Program` in versions after alpha-006
