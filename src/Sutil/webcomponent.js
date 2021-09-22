
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

export function makeWebComponent(name, ctor, initModel) {
    console.log("makeWebComponent " + name);

    function init(_) {
        _.attachShadow({ mode: 'open' });

        _.sutilCallbacks = ctor(_);

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
                    _.sutilCallbacks.GetModel()[key],
                set: (value) => {
                    let m = _.sutilCallbacks.GetModel()
                    m[key] = convert(value);
                    _.sutilCallbacks.SetModel(m);
                }
            });
        }

        return _;
    }

    let attributes = []
    for (const key of Object.keys(initModel)) {
        attributes.push(key.toLowerCase())
    }

    // Let the class name magic happen.
    let className = name.replace(/-/g, '');
    // let ctorJs = `
    //     let ${className} = function() {
    //         let _ = Reflect.construct(HTMLElement,[], ${className});
    //         return init(_, attributes);
    //     };
    //     ${className}.observedAttributes = attributes;
    //     ${className};
    // `;
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
    }

    classCtor.prototype.disconnectedCallback = function () {
        this.sutilCallbacks.Dispose();
    }

    customElements.define(name, classCtor);
}
