# Grade 0 And Tension

This note records two closely related Core3 directions:

- what grade-0 should be understood to mean
- how lawful non-ideal outcomes should remain inside the system rather than
  behaving like ordinary program failure

## Grade-0 Is Raw Exact Leaf Storage

`AtomicElement` should be read first as the grade-0 leaf of the graded engine.

Its two stored exact integers are the lowest-level exact payload currently
available in the engine.

Today those slots are commonly read as:

- realized value
- unit / support / resolution

That remains a good canonical interpretation, especially for proportion-like
reads.

But the deeper point is broader:

- grade-0 is still pair-structured
- its two exact slots are not permanently restricted to one named reading
- different higher-level views may reinterpret those same slots differently

Examples of lawful later readings include:

- value and unit
- two values entering an axis-like frame
- two support magnitudes being calibrated against one another
- a lower-level packed storage representation in a later backend

So `AtomicElement` is not "non-relational."
It is relational at the lowest available exact storage grade.

## Value/Unit Is Canonical, Not Exclusive

The current engine names the atomic slots `Value` and `Unit`.

That is acceptable as the present working read because:

- it matches current arithmetic and calibration behavior
- it keeps the public engine surface small
- it gives a clear exact-support interpretation

But the conceptual rule should remain:

- `Value`/`Unit` is one canonical current reading
- not the only possible deep reading of the raw grade-0 pair

This matters for later low-level implementations, packed backends, and
alternative higher-level views.

## Lower-Level Backing

The current implementation uses `long`.

That is a pragmatic current backing, not a final metaphysical commitment.

Core3 should remain conceptually open to later lower-level representations such
as:

- packed bit fields
- low-bit integer lanes
- SIMD vectors
- GPU-native numeric layouts
- partial reads of a wider machine word

Resolution is one of the reasons this matters.

If support and active distinctions are known, later runtimes may be able to use
smaller storage or denser evaluation without changing the Core3 meaning.

So the important concept to preserve is:

- grade-0 is exact raw pair storage
- current `long` storage is one implementation
- later backends may change the storage without changing the ontology

## Tension Can Create Orthogonal Lift

Tension should also be understood as one of the sources of new dimensions.

This is not only a geometric story.
It is the broader Core3 law for how a system is forced to acknowledge structure
that does not fit inside its current measurable frame.

The current geometric intuition is still useful:

- at grade 0, a positive unit stays on the current carrier
- if that unit becomes negative, the value no longer fits as a same-carrier
  reading and an orthogonal distinction is forced
- this is one way to read the emergence of the `i`-like direction in axis-like
  structure
- once axis-like segments exist, negative units on those sides can again create
  orthogonal pressure relative to that route, which can be read as area-like
  lift
- when those newly lifted values again create orthogonal pressure, volume-like
  lift becomes possible

But the important rule is not "first line, then plane, then space" as a merely
geometric sequence.

The deeper rule is:

- when a current calibration cannot honestly carry a distinction
- and the distinction remains real
- orthogonal lift is a lawful way for structure to continue

So an orthogonal value should not be thought of only as "vertical" or "some
specific known other axis."

It may be better read as:

- a real measurable distinction
- not calibratable by the currently active unit family
- not affecting the existing carrier's unit directly
- but still locatable as dominant-side or recessive-side pressure

This means orthogonal structure can be read as:

- everything real that the current unit cannot yet measure directly
- everything that must be preserved without being silently collapsed into the
  existing carrier

Even when the orthogonal content is not yet calibratable in the current system,
it is still not directionless.

Core3 should still be able to say:

- this orthogonal pressure arose from the recessive side
- this orthogonal pressure arose from the dominant side
- this lift preserves existing carrier values while adding a new independent
  distinction

So the emergence of new dimensions should be read as a lawful response to
tension, not as an arbitrary extra geometric feature pasted on afterward.

## Lift Requires A Coherent New Relation

Orthogonal pressure alone does not automatically justify a full lift.

There is an important difference between:

- sensing that the current carrier cannot hold a distinction
- knowing enough about that distinction to form a meaningful higher space

So a route or axis may lawfully carry orthogonal pressure without yet being a
plane.

Geometrically:

- one horizontal route plus upward/downward orthogonal pressure does not yet
  guarantee an area
- without lift, the structure can still only move along the available rays
- it may support several directed lines or segments
- but it cannot yet lawfully address the interior between them

The lift becomes meaningful when the orthogonal content has consolidated into a
coherent new category of relation.

Examples:

- horizontal and vertical displacement can form an area-like plane because the
  two directions jointly support ordered interior positions
- light and heat may form a meaningful lifted relation in one context, such as
  the luminous body of a match
- dripping-water sound and light do not automatically form such a plane merely
  because both are real distinctions

So Core3 should preserve this rule:

- orthogonal pressure may be real before lift
- lift is a further act or judgment
- lift should only be treated as successful when the new orthogonal relation is
  coherent enough that interior positions are meaningful

In practice, the first meaningful lift will often require a relation that
already carries both:

- a same-carrier contribution
- an orthogonal contribution

rather than a lone orthogonal component by itself.

Geometrically:

- one orthogonal ray by itself does not yet span a plane
- a mixed relation such as aligned-plus-orthogonal structure is a more natural
  seed for axis-like or area-like lift

This means a future first-pass implementation may allow attempted lift before
full coherence is provable, but the longer-term goal should still be:

- preserve lone orthogonal content as route-like or ray-like structure when it
  does not yet span a meaningful interior
- promote to a lifted higher basis when the relation is rich enough to support
  lawful interior positions

This means lift is not just "there is another direction."
It is also:

- there is a meaningful ordered relation along that direction
- the new basis supports lawful traversal or measurement
- the resulting higher object can address more than the original rays alone

## Lift Creates New Positive Basis Units

When lift does occur, the new units should not remain merely as negative forms
of the prior carrier.

They become the native positive basis units of the higher grade.

So:

- a negative line-unit pressure may help generate an axis-like orthogonal
  distinction
- but once that distinction is lifted and stabilized, it should be treated as
  its own positive axis basis
- later negative pressure on that newer basis is what drives the next lift

Likewise for area-like lift:

- the new area quanta should be treated as genuine area units
- not as unresolved products of prior x-like and y-like line units
- even when they remain historically related to those generating axes

That means the lifted unit family should also be usable independently of the
particular constructive mechanism that first exposed it.

For example:

- an area unit generated through an x/y-style rectangular construction should
  still be able to measure circles, irregular regions, or relative area
  changes
- it keeps provenance from that earlier grid-like construction
- but it should not be trapped inside that construction as its only meaning

This is part of why a lifted higher-grade object can again behave like a
one-degree-of-freedom quantity in its own folded measure.

For example:

- an area-like object may still preserve two-sided provenance, growth span, or
  constructive relation to generating axes
- but its folded content is now area-measure, not merely "a pair of lines"

So lift both:

- increases structural dimension
- creates a new native unit family at that higher grade

## Null, Zero, And Unresolved Are Different

Core3 should keep these distinct.

- `null`
  absence, omission, non-existence in the current structure, or not supplied
- `0`
  a present value read at the current resolution as zero-like
- unresolved / tension
  a present but not fully settled or not fully representable condition

These are not interchangeable.

In particular:

- `0` should not mean "missing"
- `null` should not mean "small but real"
- unresolved structure should not be silently collapsed into either one

## Zero Is Not Program Failure

A zero denominator or zero-like support should not be treated as an automatic
crash-shaped event.

In Core3, zero is still a present structural fact.

It may mean:

- below active resolution
- centered around the current readout origin
- a bounded near-zero interval under a chosen law
- extremely high tension under the current operation

But it is still part of the system.

So operations such as division-like folds, reciprocal-like reads, or
scale-normalizations should eventually prefer:

- a bounded or tension-bearing result
- a branch family
- a lifted representation
- an explicitly unresolved outcome

over "the system crashes because a programmer asked the wrong question."

## Core3 Should Prefer Lawful Output

The long-term aim is:

- every lawful input should prefer a lawful output
- that output may contain tension, ambiguity, bounded intervals, or unresolved
  structure
- exceptions should be reserved for malformed ontology or broken invariants

Examples of malformed ontology:

- a composite built from mismatched grades
- a route object with impossible internal shape
- a serializer being asked to write an unsupported runtime object

Examples of lawful but difficult outcomes:

- zero-like denominator
- unresolved unit relation
- incompatible but informative comparison
- under-resolution read
- branch multiplicity with no justified single selection

These should remain inside Core3 as information.

## Tension Is The Right Carrier For Non-Ideal Outcomes

When an operation cannot cleanly settle under the current frame, support, or
law, the preferred result is not failure.

The preferred result is:

- preserved tension
- preserved multiplicity
- preserved bounded uncertainty
- preserved unresolved carrier or calibration meaning

This aligns with the larger Core3 principle that non-fit is often informative
rather than erroneous.

## Present Recommendation

For current Core3 work:

- keep `AtomicElement` as the grade-0 leaf type
- interpret it conceptually as raw exact pair storage
- treat `Value`/`Unit` as the canonical current public reading, not the only
  possible one
- keep `null`, `0`, and unresolved tension sharply distinct
- prefer lawful tension-bearing outcomes over failure/null where the event is
  mathematically meaningful
- reserve exceptions for malformed structure, not ordinary structural stress

## First Migration Slice

The first concrete migration now lives in:

- grade-1 ratio folding
- support re-expression
- calibration
- exact alignment

That path now has two layers:

- exact-only `TryComposeRatio`
- lawful `ComposeRatio` returning `EngineElementOutcome`

When a ratio cannot fold honestly onto one resolved support yet still clearly
contains information, the current first-pass behavior is:

- preserve an unresolved atomic result with `Unit == 0`
- preserve the original ratio structure as tension provenance
- keep the old exact-only `Try...` surface available where callers still want a
  strict settled-result test

The same first-pass law now also applies to:

- inexact support re-expression
- calibration that cannot settle onto one resolved support
- alignment that cannot settle onto one resolved support
- addition that cannot settle onto one resolved support
- subtraction that cannot settle onto one resolved support
- multiplication that cannot settle onto one resolved support
- scaling that cannot settle onto one resolved support

In those cases the current behavior is again:

- preserve an unresolved atomic projection with `Unit == 0`
- preserve the original compared pair or source structure as tension
  provenance
- keep the old exact-only `Try...` surfaces available alongside the newer
  lawful-outcome surfaces

This is intentionally conservative.
It is not the final law for all zero-like or under-resolution behavior.
It is the first concrete step away from crash-shaped or null-shaped outcomes.

This should help Core3 evolve toward:

- lower-level execution backends
- richer continuation laws
- non-crashing arithmetic and calibration behavior
- machine-discoverable structural pattern reuse
