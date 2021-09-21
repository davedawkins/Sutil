import { build, makeContext, asDomElement, unmount } from '../../Sutil/DOM.fs.js';
import { Store_make } from '../../Sutil/Store.fs.js';

function Component({ properties, attributes, useLightDOM, renderFunction }) {
    function Cls() {
        let _ = Reflect.construct(HTMLElement, [], Cls);
        if (useLightDOM) {
            var ctx = makeContext(_);
        } else {
            // create a shadowRoot and make it the context
            // open means we can access it from javascript as well (_ is the ideal most of the time)
            _.attachShadow({ mode: 'open' });
            ctx = makeContext(_.shadowRoot);
        }
        const attrMap = new Map();
        for (const attr of _.attributes) {
            attrMap.set(attr.name, attr.value);
        }

        // track the attribute changes in a store
        _.__attrStore = Store_make(attrMap);
        // track the props in a different object which we will access
        // via getters and setters
        _.__hostStore = Store_make({ ..._, ...properties });

        // generate a SutilNode build and asDomElement already do _
        _.__sutilNode = asDomElement(build(renderFunction(_.__hostStore, _.__attrStore, _), ctx), ctx);
        // implement getter/setters for properties   to ensure we update it in a safe maner
        // we will only react to changes of our default properties
        // but we could also use a proxy to react to different properties in the HTMLElement
        for (const key of Object.keys(properties)) {
            Object.defineProperty(_, key, {
                configurable: false,
                enumerable: true,
                get: () => _.__hostStore?.Value?.[key],
                // use the attribute changedCallback from the observedAttributes
                set: value => _.__updatePropStore(key, value),
            });
        }
        return _;
    }
    Cls.observedAttributes = attributes || [];
    Cls.prototype.attributeChangedCallback = function(name, oldVal, newVal) {
        attrMap.set(name, newVal);
    };

    Cls.prototype.disconnectedCallback = function() {
        unmount(this.__sutilNode);
    };

    Cls.prototype.attributeChangedCallback = function(name, _, newVal) {
        this.__attrStore.Update(map => {
            // update the map and return the same map
            map.set(name, newVal);
            return map;
        });
    };

    Cls.prototype.__updatePropStore = function(key, value) {
        // do the updates in a safe manner
        this.__hostStore.Update(props => {
            props[key] = value;
            // defining an interface for the HTMLElement in the F# code
            // allows us to do this, and prevent bugs from people relying on F# equality
            return { ...props, ...this };
        });
    };
    Object.setPrototypeOf(Cls.prototype, HTMLElement.prototype);
    Object.setPrototypeOf(Cls, HTMLElement);
    return Cls;
}

export function defineCustomElement(name, options) {
    customElements.define(name, Component(options));
}
