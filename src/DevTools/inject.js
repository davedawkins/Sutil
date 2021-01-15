// This code runs in the context of the inspected window

export function injectedGetStores() {
    let stores = document.body.__sveltish_global.stores;
    return {
        Data: Array.from(stores).map( i => { return { Id: i, Val: window.sv_get_store(i).Get } } )
    }
}

export function injectedSetOptions( options ) {
    console.log("injectedSetOptions");
    console.dir(options);
    document.__sveltish_cb.SetOptions(options);
    return true;
}

export function injectedGetOptions() {
    return document.__sveltish_cb.GetOptions();
}

export function injectedDollar0() {
    return {
        Data: $0
    }
}
