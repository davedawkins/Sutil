
function toInt(s) {
    return parseInt(s);
}

function toFloat(s) {
    return parseFloat(s);
}

function toBool(s) {
    switch (s.toLowerCase()) {
        case "on":
        case "yes":
        case "true": return true;
        default: return false;
    }
}

function id(s) { return s; }

let instanceId = 0

export function makeWebComponent(name, ctor, initModel) {
    function init(_) {
        _.attachShadow({ mode: 'open' });

        let cb = ctor(_);

        _.sutilCallbacks = cb;

        for (const key of Object.keys(initModel)) {
            let val = initModel[key]

            let convert =
                typeof val === "number"
                    ? (Number.isInteger(val) ? toInt : toFloat)
                    : typeof val === "boolean"
                        ? toBool
                        : id

            Object.defineProperty(_, key, {
                configurable: false,
                enumerable: true,
                get: () =>
                    cb.GetModel()[key],
                set: (value) => {
                    let m0 = cb.GetModel()
                    let m1 = Object.assign({}, m0, {[key]:convert(value)})
                    cb.SetModel(m1);
                }
            });
        }

        return _;
    }

    let attributes = []
    for (const key of Object.keys(initModel)) {
        attributes.push(key.toLowerCase())
    }
    let classCtor = function() {
        let _ = Reflect.construct(HTMLElement,[], classCtor);
        return init(_, attributes);
    };
    classCtor.observedAttributes = attributes;
    Object.setPrototypeOf(classCtor.prototype, HTMLElement.prototype);
    Object.setPrototypeOf(classCtor, HTMLElement);

    classCtor.prototype.attributeChangedCallback = function (
        name,
        oldVal,
        newVal
    ) {
        this[name] = newVal
    }

    classCtor.prototype.connectedCallback = function () {
        for (const key of attributes) {
            if (this.hasAttribute(key)) {
                this[key] = this.getAttribute(key);
            }
        }
        this.sutilCallbacks.OnConnected()
    }

    classCtor.prototype.disconnectedCallback = function () {
        this.sutilCallbacks.OnDisconnected();
    }

    customElements.define(name, classCtor);
}
