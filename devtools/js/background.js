
let devtoolsPort;
let contentPort;

chrome.runtime.onConnect.addListener(function (port) {
    // alert("background.onConnect from " + port.name);

    if (port.name == "content-page") {
        contentPort = port;
        console.log("background: connected content-page");

        if (devtoolsPort) {
            devtoolsPort.postMessage( {
                "name": "content-page-connected"
            });
        }
        port.onMessage.addListener(function(msg) {
        });

        port.onDisconnect.addListener(function(port) {
            console.log("background: disconnected content-page");
        });

        return;
    }

    if (port.name == "devtools-page") {
        devtoolsPort = port;
        console.log("background: connected devtools-page");

        port.onMessage.addListener(function (msg) {

        });

        port.onDisconnect.addListener(function(port) {
            console.log("background: disconnected devtools-page");
        });
    }
});

