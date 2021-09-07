import { build, makeContext, asDomElement, unmount } from '../../Sutil/DOM.fs.js';
import { Store_make } from '../../Sutil/Store.fs.js';

export function Component(defaultValues, viewFn) {
    return class extends HTMLElement {
        // allow property names to be observed attributes
        static get observedAttributes() { return Object.keys(defaultValues); }
        constructor() {
            super();
            // create a shadowRoot and make it the context
            this.attachShadow({ mode: 'open' });
            const ctx = makeContext(this.shadowRoot);
            // generate a SutilNode build and asDomElement already do this
            const attrMap = new Map();
            for (const attr of this.attributes) {
                attrMap.set(attr.name, attr.value);
            }

            // track the attribute changes in a store
            this.__attrStore = Store_make(attrMap);
            // track the props in a different object which we will access
            // via getters and setters
            this.__hostStore = Store_make({ ...this, ...defaultValues });

            // build the node
            this.__sutilNode = asDomElement(build(viewFn(this.__hostStore, this.__attrStore), ctx), ctx);
            // implement getter/setters for properties  to ensure we update it in a safe maner
            // we will only react to changes of our default properties
            // but we could also use a proxy to react to different properties in the HTMLElement
            for (const key of Object.keys(defaultValues)) {
                Object.defineProperty(this, key, {
                    configurable: false,
                    enumerable: true,
                    get: () => this.__hostStore?.Value?.[key],
                    // use the attribute changedCallback from the observedAttributes
                    set: value => this.__updatePropStore(key, value),
                });
            }
        }

        disconnectedCallback() {
            unmount(this.__sutilNode);
        }

        attributeChangedCallback(name, _, newVal) {
            this.__attrStore.Update(map => {
                // update the map and return the same map
                map.set(name, newVal);
                return map;
            });
        }

        __updatePropStore(key, value) {
            // do the updates in a safe manner
            this.__hostStore.Update(props => {
                props[key] = value;
                return { ...props, ...this };
            });
        }

    };
}

export function defineCustomElement(name, props, viewFn) {
    customElements.define(name, Component(props, viewFn));
}