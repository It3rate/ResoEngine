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

This should help Core3 evolve toward:

- lower-level execution backends
- richer continuation laws
- non-crashing arithmetic and calibration behavior
- machine-discoverable structural pattern reuse
