//browser.devtools.panels.create("Sutil", null, "panel.html", null);

// https://stackoverflow.com/questions/4532236/how-to-access-the-webpage-dom-rather-than-the-extension-page-dom
// http://galadriel.cs.utsa.edu/plugin_study/injected_apps/brave_injected/sources/browser-android-tabs/chrome/common/extensions/docs/templates/intros/devtools_inspectedWindow.html
// https://gist.github.com/TaijaQ/5aff8ade70b386ba8527f6328914879f

// This has been ported to Fable. See src/DevTools/Chrome.Devtools.fs

let p = chrome.devtools.panels.create(
    "Sutil",         // title
    "/icon.png",        // icon
    "/html/panel.html",
    init                // Chrome
  );

let panelDoc;

function initialisePanel(win)
{
    let doc = win.document
    panelDoc = doc;

    function injected() {
        function scan( node ) {
            let count = 0;
            if (!node) return 0;
            let ch = node.firstChild;
            while (ch) {
                count += 1 + scan(ch);
                ch = ch.nextSibling;
            }
            return count;
        }
        return {
            NumNodes: scan(document.body)
        };
    }

    function injectedGetStores() {
        let stores = document.body.__Sutil_global.stores;
        return {
            Data: Array.from(stores).map( i => { return { Id: i, Val: window.sv_get_store(i).Get } } )
        }
    }

    const inspectButton = doc.querySelector("#test-button");
    const inspectString = "inspect(document.querySelector('h1'))";
    const inspectString2 = `(${injected})()`;

    function handleError(error) {
      if (error.isError) {
        console.log(`Devtools error: ${error.code}`);
      } else {
        console.log(`JavaScript error: ${error.value}`);
      }
    }

    function handleResult(result) {
        //console.log(result);
        if (result[1]) {
            handleError(result[1]);
        }
    }

    function contentRunFn( fn ) {
        return new Promise( (resolve,reject) => {
            chrome.devtools.inspectedWindow.eval(`(${fn})()`,
                result => {
                    if (result[1]) {
                        //console.log("failed");
                        reject(result[1]);
                    }
                    else {
                        //console.log("succeeded");
                        //console.log( result )
                        resolve(result);
                    }
                } );
        });
    }

    doc.querySelector("#stores").addEventListener("click", e => {
        e.target.classList.add("active");
        contentRunFn(injectedGetStores).then( buildStoresView );
        //chrome.devtools.inspectedWindow.eval(inspectString2, handleResult);
        //.then(handleResult);
    });
}

function element(tag) {
    return panelDoc.createElement(tag);
}

function append(parent, tag) {
    let e = element(tag);
    parent.appendChild(e);
    return e;
}

function buildStoresView( result )
{
    let stores = result.Data;

    let root = panelDoc.querySelector("#sv-view");
    root.textContent = "";

    let table = append(root, "table");
    table.classList.add("table");

    let thead = append(table, "thead");
    let tr = append(thead, "tr");
    let th1 = append(tr, "th");
    let th2 = append(tr, "th");

    th1.textContent = "Id";
    th2.textContent = "Value";

    let tbody = append(table,"tbody");

    for (let i in stores) {
        let r = stores[i];
        let tr = append(tbody, "tr");
        let td1 = append(tr, "td");
        let td2 = append(tr, "td");

        td1.textContent = r.Id;
        td2.textContent = JSON.stringify( r.Val );
    }
}

function unInitialisePanel()
{
}

function init(panel) {
    panel.onShown.addListener(initialisePanel);
    panel.onHidden.addListener(unInitialisePanel);
}
