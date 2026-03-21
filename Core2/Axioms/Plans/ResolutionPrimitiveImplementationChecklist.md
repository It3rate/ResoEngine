# Resolution Primitive Implementation Checklist

This checklist turns the primitive half of [ResolutionFocusProposal.md](../Proposals/ResolutionFocusProposal.md) into a concrete implementation plan.

It is intentionally narrow.
It does not try to implement the richer "constructible from primitives" layer yet.
The only goal here is to make primitive support and resolution behavior explicit, minimal, and executable.

## Primitive-only Target

The implementation should cover only these primitive distinctions:

- folded value
- support or resolution
- uncertainty
- host or focus
- transform or applied role
- branch family or ambiguity

And only these primitive support laws:

- `Inherit`
- `Aggregate`
- `Compose`
- `Refine`
- `CommonFrame`

Everything else should be deferred unless one of these primitives truly cannot express it.

## Quick Orientation

These examples should stay fixed while implementation is changing.
They are the shortest guide to what the primitive laws are supposed to mean.

- `2 * 5/10 -> 10/10`
  Host support is inherited.
  This is the default host/applied reading.

- `4/5 -> 8/10`
  Folded value is preserved while support is refined.

- one hundred `4/5` reviews -> `400/500`
  Supports aggregate because evidence is being pooled.

- `5/10 + 1/2 -> 10/10`
  This is common-frame re-expression on one shared interval, not pooled evidence.

- `5/10 * 1/2 -> 5/20`
  Supports compose because two supports are forming an independent product.

- `axis * i`
  `i` acts as opposition or basis change.
  This should default to support preservation, not generic denominator explosion.

## Scope

In scope:

- primitive support-law classification
- primitive operation defaults
- explicit separation of support from uncertainty
- explicit separation of common-frame addition from pooled aggregation
- explicit separation of exact alignment support from committed result support
- support-aware fold and unfold defaults
- symbolic adoption of the primitive laws

Out of scope:

- carrier as an explicit language-role object
- delivery, transfer, or transport semantics
- verb classes or FrameNet-style role inventories
- rich focus-tracking beyond host/applied defaulting
- discourse-level ambiguity machinery
- new high-level language constructs

## Guiding Constraints

- prefer the smallest law set that covers the widest range
- prefer existing Core 2 asymmetries over new ad hoc abstractions
- prefer host-support inheritance when a host/applied reading is already present
- treat pure transforms as support-preserving unless they clearly create new support
- do not normalize support away just to imitate folded scalar arithmetic
- do not add operation-specific exceptions when a primitive support law will do
- bias ties toward the interpretation that seems more reusable for later language encoding
- do not silently treat temporary exact alignment support as the final committed support

## Minimal Passes

The primitive layer should be implemented in two passes.
Anything more fragmented than this will likely create unnecessary churn.
Anything more compressed will likely blur law design and code change together.

## Pass 1. Freeze The Primitive Law Table

Goal:

- make the primitive support laws explicit
- classify the current primitive operations under those laws
- lock in golden examples before behavior changes

Checklist:

- define one small primitive support-law vocabulary in code and docs
  Target set:
  - `Inherit`
  - `Aggregate`
  - `Compose`
  - `Refine`
  - `CommonFrame`

- write one operation-to-law table for the current primitive layer
  Initial intended classification:
  - negate -> `Inherit`
  - opposition / `i` in transform position -> `Inherit`
  - mirror / basis flip / perspective-like transforms -> `Inherit`
  - scalar scale on a hosted value -> `Inherit`
  - repeated pooled observations -> `Aggregate`
  - independent multiplicative composition -> `Compose`
  - degree-raising composition such as axis-axis to area-like structure -> `Compose`
  - re-expression on finer support -> `Refine`
  - addition after shared-frame denominator alignment -> `CommonFrame`

- identify where current code paths blur those laws together
  Minimum review targets:
  - `Proportion`
  - `Axis`
  - generic multiply paths that currently treat all `*` as one kind of support behavior
  - power and inverse-continuation projection
  - symbolic reduction paths that currently inherit assumptions from the generic arithmetic

- freeze the primitive example set as tests or golden expectations
  Required examples:
  - `2 * 5/10 -> 10/10`
  - `4/5 -> 8/10`
  - pooled `4/5` reviews -> `400/500`
  - `5/10 + 1/2 -> 10/10`
  - `5/10 * 1/2 -> 5/20`
  - `axis * i` preserving semantic support instead of going through generic support composition

- record that exact alignment support is a temporary scaffold, not automatically the final support
  Example:
  - `2/4 + 50/100` may align exactly on support `100`, but a later committed result support may be chosen by a different policy

- explicitly mark unresolved questions instead of smearing them into the first implementation
  Keep open, but named:
  - fold-first versus structure-preserving multiplication
  - support policy when unfolding from only a folded value
  - how uncertainty layers will later be attached without collapsing them into support

Done when:

- every primitive operation under review has one provisional support law
- the golden example set exists and is readable
- the remaining ambiguity is clearly named rather than hidden in code paths

## Pass 2. Route Primitive Operations Through The Law Table

Goal:

- make the classified support laws executable in Core 2
- let `Core2.Symbolics` consume the same primitive behavior directly

Checklist:

- introduce support-law-aware paths in the primitive layer
  Minimum implementation targets:
  - `Proportion`
  - `Axis`
  - primitive transform application
  - power and inverse-continuation support projection where current behavior is too aggressively compositional

- ensure transform-like operations preserve support by default
  Especially:
  - opposition
  - negation
  - mirror-like transforms
  - perspective or basis alignment changes

- expose explicit operations for the non-default primitive laws rather than burying them in one multiply/add path
  Minimum named behaviors:
  - aggregate support
  - compose support
  - refine support
  - common-frame re-expression

- formalize committed result support as a later policy choice, not as part of exact alignment itself
  Minimum expectation:
  - the code should be able to distinguish "temporary exact support used to combine values" from "support later chosen for display or continuation"

- keep unfold branchful unless a support policy is supplied
  Do not force one canonical denominator too early.

- route the symbolic layer through the primitive law table instead of maintaining a second competing interpretation
  Minimum symbolic targets:
  - `MultiplyValuesTerm`
  - `DivideValuesTerm`
  - `PowerTerm`
  - `InverseContinueTerm`
  - any formatter or inspector text that currently reports results without indicating the chosen primitive law when that matters

- verify the language-leaning defaults
  Required expectations:
  - host/applied readings prefer `Inherit` where that reading already exists naturally
  - pure reading-frame changes do not falsely imply new measurement power
  - support pooling is not confused with same-frame comparison

- verify the existing symbolic demo pages against the new primitive behavior
  Especially the pages already showing resolution-sensitive outputs:
  - `AxisPinningGeometryPage`
  - `FractionalPowerPage`
  - `BoundaryRepetitionPage`
  - `UnitsQuantityPage`

Done when:

- primitive operations actually follow the classified laws
- transform-like operations stop using accidental support composition
- symbolic evaluation reflects the same laws as core execution
- the current denominator growth is reduced where the law is `Inherit` and preserved where the law is truly `Compose`

## Definition Of Success

This primitive pass is successful when all of the following are true:

- Core 2 has one small explicit support-law vocabulary
- primitive operations are classified by that vocabulary instead of by scattered special cases
- the host/applied asymmetry meaningfully guides support behavior
- support, folded value, and uncertainty are no longer implicitly conflated
- symbolic inspection is reading the same primitive rules that core execution uses
- the richer language layer is still deferred rather than prematurely baked into the primitive math

## Deferred Until After Primitive Stability

Do not expand into these until the primitive layer feels trustworthy:

- carrier as an explicit protected-transport role
- delivery or transfer schemas
- detailed focus shift machinery
- discourse-scale ambiguity handling
- richer event-role inventories

Those belong to the constructible layer, not to this primitive implementation pass.
