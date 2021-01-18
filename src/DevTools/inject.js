
export function ControlBlockVersion() {
    return document.__sveltish_cb.ControlBlockVersion;
}

export function Version() {
    return document.__sveltish_cb.Version;
}

export function GetStores() {
    let cb = document.__sveltish_cb;
    let stores = cb.GetStores();
    return {
        Data: Array.from(stores).map( i => { return { Id: i, Val: cb.GetStoreById(i).Value } } )
    }
}

export function SetOptions( options ) {
    document.__sveltish_cb.SetOptions(options);
    return true;
}

export function GetOptions() {
    return document.__sveltish_cb.GetOptions();
}

export function GetLogCategories() {
    return document.__sveltish_cb.GetLogCategories();
}

export function SetLogCategories( nameStates ) {
    document.__sveltish_cb.SetLogCategories( nameStates );
    return true;
}

export function Dollar0() {
    return { Data: $0 }
}
