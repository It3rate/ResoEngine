# Grade-0 Audit

This note records the first concrete audit of where current Core3 grade-0
behavior is still exact-only and where the first tension-preserving migration
has now begun.

## Current Grade-0 Roles

`AtomicElement` is currently doing three jobs at once:

- raw exact grade-0 pair storage
- canonical public `Value` / `Unit` reading
- exact arithmetic leaf for fold, calibration, alignment, and scale

The first two are coherent.
The third is where most of the remaining cleanup pressure currently sits.

## Areas Still Mostly Exact-Only

These paths still use `Try...` plus `false` / `null` as their main language for
non-ideal outcomes:

- exact-only family wrappers such as `TryReadAll`, `TryAddAll`, and
  `TryMultiplyAll`
- exact-only provenance wrappers such as `TryAddAllWithProvenance` and
  `TryMultiplyAllWithProvenance`

That does not make them wrong.
It means the compatibility shell is still exact-first even though the richer
lawful-outcome surface now exists beside it.

## First Migrated Paths

The first migrated paths are:

- `CompositeElement.Fold`
- `EngineEvaluation.ComposeRatio`
- `AtomicElement.ReexpressToSupportWithTension`
- `AtomicElement.CommitToCalibration`
- `AtomicElement.Align`
- `AtomicElement.Add`
- `AtomicElement.Subtract`
- `AtomicElement.Multiply`
- `AtomicElement.Scale`
- recursive composite calibration/alignment outcome surfaces
- recursive composite add/subtract outcome surfaces
- recursive composite multiply/scale outcome surfaces
- `EngineReadResult`
- family-wide read / add / multiply outcome surfaces
- binary and family boolean outcome surfaces
- pairwise adjacent boolean traversal outcome surfaces
- `EngineView.Read`
- `EngineView.MeasureOnCalibration`
- `EnginePin.ResolveHostedWithTension`

This now distinguishes two layers:

- `TryComposeRatio`
  exact-only compatibility surface
- `ComposeRatio`
  lawful result surface returning `EngineElementOutcome`
- exact-only `TryCommit...` / `TryAlign...`
  compatibility surfaces
- lawful plain operation surfaces
  preserving unresolved results and provenance

Under this first pass:

- exact same-carrier fold still returns the current atomic ratio result
- zero-like denominator no longer disappears into failure
- carrier-contrast fold no longer disappears into failure
- these non-ideal cases produce an unresolved atomic result with `Unit == 0`
- the original ratio structure is preserved as the held tension source
- inexact support re-expression no longer disappears into failure
- incompatible calibration no longer disappears into failure
- incompatible alignment no longer disappears into failure
- incompatible addition no longer disappears into failure
- incompatible subtraction no longer disappears into failure
- unresolved-unit multiply no longer disappears into failure
- unresolved-unit scale no longer disappears into failure
- family reads no longer disappear into failure
- family add / multiply no longer disappear into failure
- boolean and occupancy projection no longer silently collapse unresolved
  segment endpoints into exact zero-like boundaries
- adjacent-pair boolean traversal can now preserve unresolved pair reads
- direct view reads no longer disappear into failure
- hosted pin placement no longer has to be inspected only through constructor failure
- those calibration/alignment cases preserve unresolved outputs together with
  the originating pair as held tension
- those add/subtract cases preserve unresolved outputs together with the
  originating pair as held tension
- multiply/scale now do the same at the leaf and recursive composite level
- the family layer now carries those tensions upward into runtime provenance

## Present Working Law

The current first-pass law is conservative:

- if the ratio can fold exactly, do so
- if support, calibration, or alignment can settle exactly, do so
- if addition or subtraction can settle exactly, do so
- if scale or multiply can settle exactly, do so
- otherwise preserve unresolved atomic results with `Unit == 0`
- keep the originating source pair or structure as tension provenance

This is not the final law for all under-resolution or inverse-support behavior.
It is the first step away from silent non-result.

## Recommended Next Migrations

The next best places to move are:

1. route/site encounter execution
2. hosted pin positioning moving from result-shells toward native located-site elements
3. view and pin results collapsing into native higher-grade elements
4. route and mover execution adopting the same lawful-outcome surface
5. eventual collapse of all sidecar outcomes into native higher-grade elements

## Intent

The goal is not to remove `Try...` immediately.

The goal is:

- keep exact-only surfaces where they are still useful
- add lawful outcome surfaces beside them
- move mathematically meaningful non-fit out of `false` / `null`
- keep malformed ontology and broken invariants as the narrower place where
  exceptions still belong
