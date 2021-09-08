import { build, makeContext, asDomElement, unmount } from '../../Sutil/DOM.fs.js';
import { Store_make } from '../../Sutil/Store.fs.js';

function Component({ properties, attributes, useLightDOM, renderFunction }) {

    return class extends HTMLElement {
        // allow a specific amount of attributes to be observed
        // in this case we will observe the keys of our initial values.
        // this could be further configured to be a separate array of attributes
        // rather than the default values.
        static get observedAttributes() { return attributes || []; }

        constructor() {
            super();
            // decide between light DOM or shadow DOM
            if (useLightDOM) {
                var ctx = makeContext(this);
            } else {
                // create a shadowRoot and make it the context
                // open means we can access it from javascript as well (this is the ideal most of the time)
                this.attachShadow({ mode: 'open' });
                ctx = makeContext(this.shadowRoot);
            }
            const attrMap = new Map();
            for (const attr of this.attributes) {
                attrMap.set(attr.name, attr.value);
            }

            // track the attribute changes in a store
            this.__attrStore = Store_make(attrMap);
            // track the props in a different object which we will access
            // via getters and setters
            this.__hostStore = Store_make({ ...this, ...properties });

            // generate a SutilNode build and asDomElement already do this
            this.__sutilNode = asDomElement(build(renderFunction(this.__hostStore, this.__attrStore), ctx), ctx);
            // implement getter/setters for properties   to ensure we update it in a safe maner
            // we will only react to changes of our default properties
            // but we could also use a proxy to react to different properties in the HTMLElement
            for (const key of Object.keys(properties)) {
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
                // defining an interface for the HTMLElement in the F# code
                // allows us to do this, and prevent bugs from people relying on F# equality
                return { ...props, ...this };
            });
        }

    };
}

export function defineCustomElement(name, options) {
    customElements.define(name, Component(options));
}