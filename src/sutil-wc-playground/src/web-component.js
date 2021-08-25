import { build, makeContext, asDomElement, mountOn } from '../../Sutil/DOM.fs.js';
import { iterate, head, empty } from './.fable/fable-library.3.2.11/List.js';
import haunted from 'haunted';

export const { component: Component, } = haunted({
    render(result, container) {
        mountOn(result, container);
    }
});

export function defineCustomElement(name, renderFn, options) {
    customElements.define(name, Component(renderFn, options));
}