
export function ControlBlockVersion() {
    return document.__sutil_cb.ControlBlockVersion;
}

export function Version() {
    return document.__sutil_cb.Version;
}

export function GetStores() {
    let cb = document.__sutil_cb;
    let stores = cb.GetStores();
    return {
        Data: Array.from(stores).map( i => { return { Id: i, Val: cb.GetStoreById(i).Value } } )
    }
}

export function SetOptions( options ) {
    document.__sutil_cb.SetOptions(options);
    return true;
}

export function GetOptions() {
    return document.__sutil_cb.GetOptions();
}

export function GetLogCategories() {
    return document.__sutil_cb.GetLogCategories();
}

export function GetMountPoints() {
    return document.__sutil_cb.GetMountPoints();
}

export function Remount( id ) {
    let mps = document.__sutil_cb.GetMountPoints();
    const mp = mps.find(x => x.Id === id);
    mp.Remount();
    return true;
}

export function SetLogCategories( nameStates ) {
    document.__sutil_cb.SetLogCategories( nameStates );
    return true;
}

export function Dollar0() {
    return { Data: $0 }
}
