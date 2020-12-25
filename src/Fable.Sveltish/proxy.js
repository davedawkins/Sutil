
function validator( setCb ) {
    return {
        set: function (obj, prop, value) {

            // The default behavior to store the value
            obj[prop] = value;
            setCb(value);
            // Indicate success
            return true;
        },
        get: function (obj, prop) {
            return obj[prop];
        }
    }
};


export function makeProxy(obj, handler) {
    return new Proxy(obj, validator(handler));
}
