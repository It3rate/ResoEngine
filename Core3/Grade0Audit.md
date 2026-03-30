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

- `AtomicElement.TryReexpressToSupport`
- `AtomicElement.TryCommitToSupport`
- `AtomicElement.TryCommitToCalibration`
- `AtomicElement.TryAlignExact`
- `AtomicElement.TryAdd`
- `AtomicElement.TrySubtract`
- `AtomicElement.TryScale`
- `EngineFamily.TryReadAll`
- family-wide add / multiply loops

That does not make them wrong.
It means they are still expressing "exactly settled under the current law" more
than "always produce a lawful Core3 output."

## First Migrated Path

The first migrated path is the grade-1 ratio-like fold:

- `CompositeElement.FoldWithTension`
- `EngineEvaluation.ComposeRatio`

This now distinguishes two layers:

- `TryComposeRatio`
  exact-only compatibility surface
- `ComposeRatio`
  lawful result surface returning `EngineElementOutcome`

Under this first pass:

- exact same-carrier fold still returns the current atomic ratio result
- zero-like denominator no longer disappears into failure
- carrier-contrast fold no longer disappears into failure
- these non-ideal cases produce an unresolved atomic result with `Unit == 0`
- the original ratio structure is preserved as the held tension source

## Present Working Law

The current first-pass law is conservative:

- if the ratio can fold exactly, do so
- otherwise preserve an unresolved atomic result
- keep the originating ratio as tension provenance

This is not the final law for all under-resolution or inverse-support behavior.
It is the first step away from silent non-result.

## Recommended Next Migrations

The next best places to move are:

1. support re-expression and calibration
2. exact alignment
3. add / subtract
4. multiply where unresolved unit relations matter
5. family-wide operation surfaces so tension can travel with provenance

## Intent

The goal is not to remove `Try...` immediately.

The goal is:

- keep exact-only surfaces where they are still useful
- add lawful outcome surfaces beside them
- move mathematically meaningful non-fit out of `false` / `null`
- keep malformed ontology and broken invariants as the narrower place where
  exceptions still belong
