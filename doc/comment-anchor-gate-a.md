# Gate A catalogue — comment-anchor rewrite (fsimgo #896)

This rewrite deletes `SutilGroup` and the `SutilEffect` DU. The group machinery is reviewed, hardened
code: several of its members exist because a past defect forced them into existence. This catalogue
tables every such guard, the anchor-model construct that replaces it, and the test that proves the
protection survives. It lands before any source change, per the `/second-system` discipline.

The anchor model in one sentence: every binding owns one comment node that lives in the DOM for the
binding's whole life, its current output is a plain `Node[]`, and every insertion is
`insertBefore(node, anchor)`.

| # | Guard in the group machinery | What it protected against | Anchor-model equivalent | Proving test |
|---|---|---|---|---|
| 1 | `PrevDomNode` recursive fallback through parent groups (`Core.fs:361`) | A binding with no rendered nodes losing its place: an empty group must ask its previous sibling, then its parent group, for a real DOM position | The anchor is a real DOM node that exists while the binding renders nothing; position is `insertBefore(_, anchor)`, no search | Empty-render binding holds its position between two static siblings across empty/refill cycles |
| 2 | `ChildAfter`'s `IsSameNode` comparisons (`Core.fs:476`) | Structural F# equality misidentifying distinct DOM nodes or groups with equal content; identity had to be node identity, not value equality | No child list exists to search; nodes are held in an array and compared only by reference where needed (`isSameNode` in the eachiko reorder walk) | Keyed each reorder test; repeated-rebuild constant-count test |
| 3 | `ReplaceChild`'s `child.Id <- oldChild.Id` identity transfer (`Core.fs:569`) | Future replaces failing to find the child after a rebuild changed its node; the group tracked children by sv-id across rebuilds | The binding's current output is replaced wholesale (`current <- newNodes`); there is no lookup to go stale | Multi-value rebuild sequence keeps replacing correctly (bind rebuild tests) |
| 4 | `ReplaceChild`'s `assertTrue "Child not found"` (`Core.fs:564`) | Silent corruption when a replace targeted a child no longer in the list | Removal iterates the stored array; membership cannot desynchronise because the array is the single source | Same as row 3 |
| 5 | `deleteOldNodes` null-parent tolerance (`Core.fs:554`) | External code (the fsimgo JSX patcher, #892) removing bound DOM behind Sutil's back; a hard failure here took down every later rebuild | `unmount`/`removeNode` keep the null-parent check: a node already detached is cleaned but not re-removed | Externally-removed content followed by a rebuild neither throws nor duplicates |
| 6 | Static `ReplaceGroup` insert-before computation from the last existing node (`Core.fs:219`) | Replacement content drifting to the end of the parent instead of the old content's position | Content lands at `insertBefore(_, anchor)` and the anchor never moves | Position-holding test (row 1) plus multi-node replace test |
| 7 | `updateChildrenPrev` after every mutation (`Core.fs:307`) | The prev-chain silently rotting after add/insert/replace/remove, which produced misplaced later inserts | No prev-chain exists; ordering is derived from the DOM itself plus the anchor | Whole binding suite; specifically interleaved sibling bindings |
| 8 | `SutilEffect.Register` + `CleanupGroups` on the parent node (`Core.fs:72`) | Group disposables leaking when the owning DOM node unmounted; groups are not DOM children, so `cleanupDeep` could not see them without registration | The anchor is a real DOM child; `cleanupDeep` visits it like any node and fires its disposables | Unmounting an ancestor disposes the binding's subscription |
| 9 | `_childGroups` disposal cascade (`Core.fs:606`) | Nested bindings leaking their subscriptions when an outer binding rebuilt | `removeNode` recurses through anchors found in the outgoing `Node[]`; anchors nested deeper are descendants and `cleanupDeep` reaches them | Outer rebuild disposes inner binding's subscription |
| 10 | `bindElement`'s `NOT CONNECTED` console diagnostic (`Bindings.fs:145`) | Rebuilds firing against a parent no longer in the document — the visible symptom trail for #895 | Kept: the check now tests the anchor's connectedness and prints the binding name without a child list | fsimgo E2E console check: text no longer prints a growing list |
| 11 | `findCurrentNode`/`findCurrentElement` sv-id re-lookup in eachiko (`Bindings.fs:504`) | Item elements replaced under the each block by inner bindings, leaving stale references | Kept unchanged; sv-id recovery is orthogonal to positioning | Existing `BindArray.each` internal-state test |
| 12 | `notifySutilEvents` gating on parent connectedness, single `Mount` per element (`Core.fs:669`) | Mount events double-firing or firing for detached subtrees | Kept with `Node`-typed parent; the once-guard via `_onmount` is untouched | Existing mount/unmount event tests |
| 13 | `asDomNode` error node for fragment-rooted each items (`Bindings.fs:529`) | `each` silently mis-tracking an item that rendered multiple roots | Kept: items still normalise to a single element with a visible error node otherwise | Existing each tests |
| 14 | `transitionOpt` builds branches once and only toggles visibility (`Transition.fs:452`) | Rebuild-per-toggle destroying transition state | Kept: branches build once into `Node[]` and toggle visibility; the arrays never rebuild | `showIf`/`showIfElse`/`transitionList` tests |
| 15 | Fragment group registered on the parent for disposal (`CoreElements.fs:430`) | `disposeOnUnmount` inside a `fragment` losing its disposables because the fragment owned no node | Fragment-level disposables attach to `ctx.Parent` (a real node) exactly as they already do via `RegisterDisposable(ctx.Parent, _)` | `disposeOnUnmount` inside a fragment fires on unmount |

Rows 1 and 2 look like clutter in the original source and are not; they are the reason this table
exists. Any guard listed as "kept" must appear in the rewrite in recognisable form; any listed as
replaced must have its proving test green before the PR opens.
