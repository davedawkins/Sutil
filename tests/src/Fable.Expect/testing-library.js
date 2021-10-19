// Helpers extracted from https://github.com/testing-library/dom-testing-library/

/*
The MIT License (MIT)
Copyright (c) 2017 Kent C. Dodds

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

import { elementRoles } from 'aria-query'

// Constant node.nodeType for text nodes, see:
// https://developer.mozilla.org/en-US/docs/Web/API/Node/nodeType#Node_type_constants
const TEXT_NODE = 3

/**
 * @param {Element} node
 */
export function getNodeText(node) {
    if (
        node.matches('input[type=submit], input[type=button], input[type=reset]')
    ) {
        return node.value
    }

    return Array.from(node.childNodes)
        .filter(child => child.nodeType === TEXT_NODE && Boolean(child.textContent))
        .map(c => c.textContent)
        .join('')
}

const elementRoleList = buildElementRoleList(elementRoles)

/**
 * @param {Element} element -
 * @returns {boolean} - `true` if `element` and its subtree are inaccessible
 */
export function isSubtreeInaccessible(element) {
  if (element.hidden === true) {
    return true
  }

  if (element.getAttribute('aria-hidden') === 'true') {
    return true
  }

  const window = element.ownerDocument.defaultView
  if (window.getComputedStyle(element).display === 'none') {
    return true
  }

  return false
}

/**
 * Partial implementation https://www.w3.org/TR/wai-aria-1.2/#tree_exclusion
 * which should only be used for elements with a non-presentational role i.e.
 * `role="none"` and `role="presentation"` will not be excluded.
 *
 * Implements aria-hidden semantics (i.e. parent overrides child)
 * Ignores "Child Presentational: True" characteristics
 *
 * @param {Element} element -
 * @param {object} [options] -
 * @param {function (element: Element): boolean} options.isSubtreeInaccessible -
 * can be used to return cached results from previous isSubtreeInaccessible calls
 * @returns {boolean} true if excluded, otherwise false
 */
export function isInaccessible(element, options = {}) {
  const {
    isSubtreeInaccessible: isSubtreeInaccessibleImpl = isSubtreeInaccessible,
  } = options
  const window = element.ownerDocument.defaultView
  // since visibility is inherited we can exit early
  if (window.getComputedStyle(element).visibility === 'hidden') {
    return true
  }

  let currentElement = element
  while (currentElement) {
    if (isSubtreeInaccessibleImpl(currentElement)) {
      return true
    }

    currentElement = currentElement.parentElement
  }

  return false
}

export function getImplicitAriaRoles(currentNode) {
  // eslint bug here:
  // eslint-disable-next-line no-unused-vars
  for (const {match, roles} of elementRoleList) {
    if (match(currentNode)) {
      return [...roles]
    }
  }

  return []
}

function buildElementRoleList(elementRolesMap) {
  function makeElementSelector({name, attributes}) {
    return `${name}${attributes
      .map(({name: attributeName, value, constraints = []}) => {
        const shouldNotExist = constraints.indexOf('undefined') !== -1
        if (shouldNotExist) {
          return `:not([${attributeName}])`
        } else if (value) {
          return `[${attributeName}="${value}"]`
        } else {
          return `[${attributeName}]`
        }
      })
      .join('')}`
  }

  function getSelectorSpecificity({attributes = []}) {
    return attributes.length
  }

  function bySelectorSpecificity(
    {specificity: leftSpecificity},
    {specificity: rightSpecificity},
  ) {
    return rightSpecificity - leftSpecificity
  }

  function match(element) {
    return node => {
      let {attributes = []} = element
      // https://github.com/testing-library/dom-testing-library/issues/814
      const typeTextIndex = attributes.findIndex(
        attribute =>
          attribute.value &&
          attribute.name === 'type' &&
          attribute.value === 'text',
      )
      if (typeTextIndex >= 0) {
        // not using splice to not mutate the attributes array
        attributes = [
          ...attributes.slice(0, typeTextIndex),
          ...attributes.slice(typeTextIndex + 1),
        ]
        if (node.type !== 'text') {
          return false
        }
      }

      return node.matches(makeElementSelector({...element, attributes}))
    }
  }

  let result = []

  // eslint bug here:
  // eslint-disable-next-line no-unused-vars
  for (const [element, roles] of elementRolesMap.entries()) {
    result = [
      ...result,
      {
        match: match(element),
        roles: Array.from(roles),
        specificity: getSelectorSpecificity(element),
      },
    ]
  }

  return result.sort(bySelectorSpecificity)
}

export function getRoles(container, {hidden = false} = {}) {
  function flattenDOM(node) {
    return [
      node,
      ...Array.from(node.children).reduce(
        (acc, child) => [...acc, ...flattenDOM(child)],
        [],
      ),
    ]
  }

  return flattenDOM(container)
    .filter(element => {
      return hidden === false ? isInaccessible(element) === false : true
    })
    .reduce((acc, node) => {
      let roles = []
      // TODO: This violates html-aria which does not allow any role on every element
      if (node.hasAttribute('role')) {
        roles = node.getAttribute('role').split(' ').slice(0, 1)
      } else {
        roles = getImplicitAriaRoles(node)
      }

      return roles.reduce(
        (rolesAcc, role) =>
          Array.isArray(rolesAcc[role])
            ? {...rolesAcc, [role]: [...rolesAcc[role], node]}
            : {...rolesAcc, [role]: [node]},
        acc,
      )
    }, {})
}
