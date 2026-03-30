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

- `AtomicElement.TryAdd`
- `AtomicElement.TrySubtract`
- `AtomicElement.TryScale`
- `EngineFamily.TryReadAll`
- family-wide add / multiply loops

That does not make them wrong.
It means they are still expressing "exactly settled under the current law" more
than "always produce a lawful Core3 output."

## First Migrated Paths

The first migrated paths are:

- `CompositeElement.FoldWithTension`
- `EngineEvaluation.ComposeRatio`
- `AtomicElement.ReexpressToSupportWithTension`
- `AtomicElement.CommitToCalibrationWithTension`
- `AtomicElement.AlignWithTension`
- `AtomicElement.AddWithTension`
- `AtomicElement.SubtractWithTension`
- recursive composite calibration/alignment outcome surfaces
- recursive composite add/subtract outcome surfaces

This now distinguishes two layers:

- `TryComposeRatio`
  exact-only compatibility surface
- `ComposeRatio`
  lawful result surface returning `EngineElementOutcome`
- exact-only `TryCommit...` / `TryAlign...`
  compatibility surfaces
- lawful `...WithTension(...)` outcome surfaces
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
- those calibration/alignment cases preserve unresolved outputs together with
  the originating pair as held tension
- those add/subtract cases preserve unresolved outputs together with the
  originating pair as held tension

## Present Working Law

The current first-pass law is conservative:

- if the ratio can fold exactly, do so
- if support, calibration, or alignment can settle exactly, do so
- if addition or subtraction can settle exactly, do so
- otherwise preserve unresolved atomic results with `Unit == 0`
- keep the originating source pair or structure as tension provenance

This is not the final law for all under-resolution or inverse-support behavior.
It is the first step away from silent non-result.

## Recommended Next Migrations

The next best places to move are:

1. scale and multiply where unresolved unit relations matter
2. family-wide operation surfaces so tension can travel with provenance
3. reference reads and frame borrowing
4. hosted pin positioning and route encounter logic
5. route/site encounter execution

## Intent

The goal is not to remove `Try...` immediately.

The goal is:

- keep exact-only surfaces where they are still useful
- add lawful outcome surfaces beside them
- move mathematically meaningful non-fit out of `false` / `null`
- keep malformed ontology and broken invariants as the narrower place where
  exceptions still belong
