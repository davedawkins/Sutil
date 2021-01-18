console.log("Sveltish content page loaded");

chrome.runtime.onConnect.addListener(function (port) {
    alert("Content.onConnect " + port.name);
});

let backgroundPageConnection = chrome.runtime.connect( { "name": "content-page" } );

backgroundPageConnection.onMessage.addListener( msg => {
    console.log("Content.onMessage")
    console.dir(msg)
});


