import { build, makeContext, asDomElement } from '../Sutil/src/Sutil/DOM.fs.js';
import { iterate, head, empty } from './.fable/fable-library.3.1.9/List.js';

export function customElFactory(defaultValues, viewFn) {
    return class extends HTMLElement {
        // allow property names to be observed attributes
        static get observedAttributes() { return Object.keys(defaultValues); }
        constructor() {
            super();
            // create a shadowRoot and make it the context
            this.attachShadow({ mode: 'open' });
            const ctx = makeContext(this.shadowRoot);
            // generate a SutilNode build and asDomElement already do this
            this.__sutilNode = asDomElement(build(viewFn(defaultValues), ctx), ctx);
            // implement getter/setters for properties 
            for (const key of Object.keys(defaultValues)) {
                Object.defineProperty(this, key, {
                    configurable: false,
                    enumerable: true,
                    get: () => this.__getStorePropValue(key),
                    // use the attribute changedCallback from the observedAttributes
                    set: value => this.attributeChangedCallback(key, this.__getStorePropValue(key), value)
                });
            }
        }
        attributeChangedCallback(name, oldVal, newVal) {
            this.__setStorePropValue(name, newVal);
        }
        disconnectedCallback() {
            iterate(item => item?.Dispose(), this.shadowRoot?.firstChild?.__sutil_disposables);
            iterate(item => item?._dispose(), this.shadowRoot?.firstChild?.__sutil_groups);
        }
        // utility methods to get/set values from the store
        get __sutilFistElStore() {
            return head(this.shadowRoot?.firstChild?.__sutil_disposables || empty());
        }
        __getStorePropValue(key) {
            const store = this.__sutilFistElStore;
            return store?.Value?.[key];
        }
        __setStorePropValue(key, value) {
            const store = this.__sutilFistElStore;
            if (!key || !store) return;
            store.Update(store => ({ ...store, [key]: value }));
        }
    };
}

export function defineCustomElement(name, props, viewFn) {
    customElements.define(name, customElFactory(props, viewFn));
}