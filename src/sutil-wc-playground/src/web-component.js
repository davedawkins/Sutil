import { build, makeContext, asDomElement } from '../Sutil/src/Sutil/DOM.fs.js';
import { iterate, head, empty } from './.fable/fable-library.3.1.9/List.js';

export function customElFactory(defaultValues, viewFn) {
    return class extends HTMLElement {

        static get observedAttributes() { return Object.keys(defaultValues); }

        constructor() {
            super();
            this.attachShadow({ mode: 'open' });
            const ctx = makeContext(this.shadowRoot);
            this.__sutilNode = asDomElement(build(viewFn(defaultValues), ctx), ctx);

            for (const key of Object.keys(defaultValues)) {
                Object.defineProperty(this, key, {
                    configurable: false,
                    enumerable: true,
                    get: () => this.__getStorePropValue(key),
                    set: value => this.attributeChangedCallback(key, this.__getStorePropValue(key), value)
                });
            }
        }

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

        attributeChangedCallback(name, oldVal, newVal) {
            this.__setStorePropValue(name, newVal);
        }

        disconnectedCallback() {
            iterate(item => item?.Dispose(), this.shadowRoot?.firstChild?.__sutil_disposables);
            iterate(item => item?._dispose(), this.shadowRoot?.firstChild?.__sutil_groups);
        }
    };
}

export function customElFactoryNoProps(viewFn) {
    return class extends HTMLElement {
        constructor() {
            super();
            this.attachShadow({ mode: 'open' });
            const attributeNames = new Set(this.getAttributeNames());
            const props = {};
            for (const key of attributeNames.values()) {
                props[key] = this.getAttribute(key);
            }
            const sutilNode = viewFn(props);
            const ctx = makeContext(this.shadowRoot);
            asDomElement(build(sutilNode, ctx), ctx);
            // attach the content to the shadow DOM
        }
        disconnectedCallback() {
            iterate(item => item?.Dispose(), this.shadowRoot?.firstChild?.__sutil_disposables);
            iterate(item => item?._dispose(), this.shadowRoot?.firstChild?.__sutil_groups);
        }
    };
}

export function defineCustomElement(name, props, viewFn) {
    customElements.define(name, customElFactory(props, viewFn));
}