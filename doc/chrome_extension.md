## Creating a Chrome Extension in F# Using Sutil ##

This is the documentation for the template project found at [sutil-template-chromex](https://github.com/davedawkins/sutil-template-chromex).

Learn more about Sutil at [sutil.dev](https://sutil.dev).

This example was derived from [Getting Started with Chrome Extensions](https://developer.chrome.com/docs/extensions/mv3/getstarted/).

### Getting Started

The extension is ready-built in the `./dist` folder. Clone the repo and try it out as follows:

(By the way, these are the same instructions you'll find [here](https://developer.chrome.com/docs/extensions/mv3/getstarted/))

- In Chrome, go to `chrome://extensions`
- Click `Load unpacked`
- Select the `dist` folder

If all is well, you'll be able test the extension as follows:

![Sutil Chrome Extension](https://user-images.githubusercontent.com/285421/135159208-eaaa6fbc-7611-4a9f-94a5-53135e191f89.gif)

### Developing the Extension

- Clone the repo

- Initialize the environment

```bash
$ npm install
$ dotnet tool restore
```

- Developing the UI

You can treat `Popup.fs` as a regular (Sutil) web application. In fact, you don't *have* to use Sutil here at all. You could just use the `Browser.Dom` APIs and go on a `document.createElement(..)` bender, though I feel you'll have more fun and an easier time using Sutil :-)

The modules `Background.fs` and `Popup.fs` do play different roles in the extension though, and this is best explained by the Chrome documentation. You'll see that `Background.fs` is where we can catch the `onInstalled` event.

- To build

```
$ npm run build
```

Upon each build, go back to the `chrome://extensions` page and `refresh` the extension.

<img width="539" alt="image" src="https://user-images.githubusercontent.com/285421/135159835-bd6ec235-612f-4d2f-a72b-0a07466338d4.png">

### Console Output

Output from `Background.fs` will be visible in the developer window accessible from `Inspect views:
service worker` link, found on the `chrome://extensions` page.

Output from `Popup.fs` will be visible from the developer window accessible when you `right-click -> Inspect` inside the window created from `popup.html` (i.e., where the button appears when you activate the extension)

### Browser APIs

Currently, there is minimal support for the `chrome.storage.*` and `chrome.runtime.*` etc APIs. This will be a work-in-progress, but it's fairly easy to move your project forward in the meantime, thanks to Fable's awesome interop capabilities.

### Support for other Browsers

Currently, Sutil is exporting a `Chrome.*` API, but there is a polyfill that will allow me to unify this to a more useful `browser.*` API, and then it should be possible to create extensions for other browsers too.
