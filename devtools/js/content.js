console.log("* Sutil content page loaded");

chrome.runtime.onConnect.addListener(function (port) {
    alert("Content.onConnect " + port.name);
});

let backgroundPageConnection = chrome.runtime.connect( { "name": "content-page" } );

backgroundPageConnection.onMessage.addListener( msg => {
    console.log("Content.onMessage");
    console.dir(msg);
});

window.document.addEventListener( "sutil-new-store", e => {
    //backgroundPageConnection.
    console.log("New store *");
    backgroundPageConnection.postMessage({
        "name": "sutil-new-store"
    });
})
