// This code runs in the context of the inspected window

export function injectedGetStores() {
    let stores = document.body.__sveltish_global.stores;
    return {
        Data: Array.from(stores).map( i => { return { Id: i, Val: window.sv_get_store(i).Get } } )
    }
}

