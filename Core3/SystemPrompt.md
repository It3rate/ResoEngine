# Core3 Prompt Axioms

This file is the prompt-safe consolidated Core3 direction.

It compresses the current markdown notes into one AI-readable axiom file.
It is intentionally biased toward:

- structural reuse
- physical and metaphoric continuity
- tension-preserving lawful outputs
- native Core3 expression instead of ad hoc code structure

The purpose is not to freeze the implementation.
The purpose is to keep future work pointed toward one coherent model.

## Part I. General Orientation

P1. Core3 is a small mathematical substrate for concepts, not only for
measurement.

P2. Structure, resolution, calibration, perspective, tension, and continuation
are all first-class.

P3. Reuse one structural idea wherever several domains can lawfully share it.
Traversing a number, a carrier, an equation, a route, or a flow graph should
prefer one family of concepts rather than unrelated special mechanisms.

P4. Prefer native Core3 structure over external bookkeeping whenever the same
meaning can be carried by Core3 elements, routes, pins, families, tensions, or
attached laws.

P5. Prefer physically interpretable structure over hidden virtual state.

P6. The long-term aim is machine pattern recognition over Core3 structure, so
reused patterns are a strength, not a weakness.

P7. Operations should also be read as directed arcs:

- inbound carrier state
- origin law
- outbound carrier/result

This same pattern should scale from simple arithmetic to boolean partitioning,
route traversal, larger equations, branching flows, and conceptual domains
such as narrative or language structure.

P8. If a current implementation detail is only expressible through a code-side
enum or branch tree, first ask whether the meaning should instead live in
native Core3 structure, tensions, branch families, or pinned route/site
relations.

## Part II. Engine Ontology

E1. `GradedElement` is the engine union.

E2. Grade 0 is the leaf case.
Grade greater than 0 is recursive pair structure.

E3. `AtomicElement` is grade-0 exact pair storage.
Its current public reading is `Value` and `Unit`, but conceptually it is the
lowest exact two-slot payload, not a permanently one-interpretation object.

E4. `CompositeElement` is a same-grade recessive/dominant pair of child
elements.

E5. Grade is structural, not nominal.
Named views such as proportion-, axis-, or grid-like readings are later
interpretations over recurring graded configurations.

E6. The engine should remain small, serialized, and close to exact structure.

E7. The old named-structure layer has been retired in favor of the current
engine/runtime/binding stack. Native Core3 semantics should be expressed
directly through the current graded engine structures.

## Part III. Grade-0 Reading

G1. Grade-0 should be understood as raw exact pair storage.

G2. `Value`/`Unit` is the canonical current reading of that pair.

G3. Other lawful future readings may reinterpret the same raw exact pair.

G4. The current `long` backing is implementation, not ontology.
Later backends may use packed bits, SIMD lanes, GPU-native layouts, or other
low-level representations without changing the Core3 meaning.

G5. Resolution is one reason later lower-level packing should remain possible.

## Part IV. Orthogonal Lift And Dimensional Emergence

O1. Tension can force orthogonal lift.

O2. Orthogonal lift is not merely a geometric special case.
It is the general Core3 rule for how a real distinction continues when the
current calibration cannot honestly carry it.

O3. The geometric ladder remains a useful intuition:

- negative unit pressure at grade 0 can force the `i`-like orthogonal
  distinction needed for axis-like structure
- negative unit pressure on axis-like segment sides can again force orthogonal
  lift, which is one way to read area-like structure
- renewed orthogonal pressure on those lifted values can again force further
  lift, which is one way to read volume-like structure

O4. But the deeper rule is not specifically "line to plane to space."
The deeper rule is:

- a distinction is real
- the current units do not calibrate it
- the structure must still preserve it
- orthogonal lift is therefore lawful

O5. Orthogonal values should not be read only as some one predefined "other
axis."
They may be read more generally as real measurable distinctions that do not fit
inside the currently active unit family.

O6. Orthogonal content does not have to alter the existing carrier's unit in
order to be real.
It may instead preserve the existing carrier while adding an independent
dimension of variation.

O7. Even when orthogonal content is not yet calibratable by the current unit,
it is still not directionless.
Core3 should remain able to distinguish whether the orthogonal pressure arose
from dominant-side or recessive-side structure.

O8. New dimensions are therefore not arbitrary add-ons.
They are one lawful way tension becomes preserved structure.

O9. Orthogonal pressure alone does not automatically justify a full lift.
There is a difference between:

- a real distinction that the current carrier cannot hold
- a coherent higher relation whose interior positions are meaningful

O10. A route with orthogonal pressure may still remain only a route-like
structure with several rays or directed segments.
Without a successful lift it does not yet gain the lawful interior of a higher
space.

O11. Lift should be treated as successful when the new orthogonal relation is
coherent enough to support meaningful ordered traversal or measurement in the
new interior.

O12. So lift is not only "another direction exists."
It is also:

- the new direction forms a meaningful relation with the old one
- the resulting higher object can address more than the original rays
- the new basis supports lawful interior positions

O12a. In practice, a first meaningful lift will often require a relation that
already carries both same-carrier and orthogonal contributions rather than a
lone orthogonal component by itself.
A lone orthogonal component may remain ray-like until it participates in a
richer relation that genuinely spans a higher interior.

O13. When lift does occur, the new basis units should become positive native
units of the higher grade.
They should not remain only as negative or unresolved remnants of the prior
carrier.

O14. Historical relation still matters.
The higher-grade basis may preserve provenance from the lower-grade structure
that generated it, but it should still be treated as a genuinely new unit
family at the lifted grade.

O15. A lifted unit family should remain usable beyond the particular mechanism
that first generated it.
For example, an area basis exposed through one rectangular x/y-style
construction should still be able to measure circles, irregular regions, or
relative area changes rather than remaining trapped inside that original grid.

## Part V. Null, Zero, And Tension

N1. `null`, `0`, and unresolved tension are different.

N2. `null` means absence, omission, or non-existence in the current structure.

N3. `0` means a present zero-like reading at the current resolution.

N4. Unresolved tension means present structure that does not yet settle cleanly
under the current law, support, or frame.

N5. Do not silently collapse one of these into another.

N6. Zero should not be treated as automatic program failure.

N7. A zero-like denominator or support should eventually yield a lawful bounded,
lifted, branching, or tension-bearing outcome rather than a crash-shaped
result.

N8. Core3 should prefer lawful output over failure when the situation is
mathematically meaningful.

N9. Exceptions remain acceptable for malformed ontology or broken invariants,
not for ordinary structural stress.

## Part VI. Tension And Non-Failure

T1. Tension is informative structure, not merely error.

T2. When a law cannot settle cleanly, the preferred outcomes are:

- preserved tension
- preserved multiplicity
- preserved bounded uncertainty
- preserved unresolved carrier or calibration meaning
- smallest lawful lift

T3. `Try...` non-success is better than throwing for expected structural
non-settlement.

T4. Long term, even `Try...` plus `null` is not rich enough for all Core3
outcomes. Future outcome forms should remain able to carry tension rather than
erasing it.

T5. The system should not "crash because nature was awkward."

## Part VII. Pinning, Reading, And Frames

F1. Pinning is structural encounter.

F2. View is observational reading through an existing frame or site.

F3. A pin creates or exposes local structure.

F4. A view lends calibration without making the subject owned structure
inside that frame.

F5. Pins, boundaries, and operation sites should increasingly be read as one
family of located encounters on routes, while still preserving their distinct
roles.

F6. Frames are not only calibrations; they also define active bounded spans and
boundary meaning.

F7. Runtime family/context structures are temporary operational roles, not a
second mathematical ontology.

## Part VIII. Route Model

R1. The current consolidation target is:

- routes
- sites on routes
- movers on routes
- reads at sites
- laws at sites
- carried data

R2. A route is a traversable whole with beginning, end, and possible
intermediate landmarks or branching.

R3. A site is a located encounter on a route.

R4. A mover is a real cursor/trolley/container whose current position is part
of runtime state.

R5. A read is a site- or frame-relative observation into local or nonlocal
data.

R6. A law is an attached operation or continuation enacted at a site.

R7. Carried data is the runtime payload: registers, bound literals, family
members, history, scratch, or result slots.

R8. The current implementation already approximates this model, but route and
located-site state are not yet fully unified first-class runtime objects.

## Part IX. Movers

M1. A mover should be physical, not merely a virtual selector token.

M2. The current `TraversalMover` is a small friendly wrapper over one exact
atomic iterator.

M3. Its current `Position` stores:

- numerator = current route position
- denominator = route end / resolution

M4. The current default continuation is `+1` on the numerator until the
denominator is reached, then clamp-like stop.

M5. This is a temporary first step, not the final traversal runtime.

M6. Future mover advancement should also relocate the mover's bound encounter
physically on a route, not only increment the iterator.

M7. Long term, mover stepping should be driven by partitionable Core3
expressions and attached continuation laws.

## Part X. Reads

R9. `EngineView` is best understood as a lens, not as a mover.

R10. The mover tells us where "here" is.
The view tells us how to look from there.

R11. References, selector addresses, and route positions should all prefer
numeric/Core3-shaped meaning where practical.

R12. Names are useful aliases but should not be the deepest canonical selector
mechanism when a lawful numeric address exists.

## Part XI. Laws

L1. Laws should remain attached, not embedded.

L2. Structure, reads, movers, and carried data should remain distinguishable
even when they meet at one site.

L3. Addition, multiplication, boolean occupancy, continuation, branch, and
later equations should all be understood as laws acting over contextualized
structure.

L4. Reuse law patterns where possible.
Boolean, accumulation, traversal, and branch routing should prefer shared
underlying mechanics with different retention or fold laws.

L5. Avoid introducing a new special primitive when an existing Core3 relation
can be re-read lawfully.

L6. Prefer one shared operation pipeline.
Operations should increasingly be understood in terms of:

- source domain
- correspondence topology
- local law
- retention mode
- carrier inheritance
- reduction mode
- continuation mode

Simple arithmetic, boolean partitioning, traversal-time site laws, branch
families, and later equations should be different presets of that larger
pipeline rather than unrelated execution systems.

L7. The operation pipeline should also preserve the inbound-origin-outbound
reading:

- inbound setup or traversal
- local law enactment
- outbound result or survivor family

This should remain true whether the output is one result, many co-present
pieces, a branch family, or a continued runtime state.

L8. Retention, reduction, and continuation should not be treated as permanent
enum-only code categories if Core3 can eventually carry them more natively
through lawful values, tensions, carriers, branch structure, or pinned route
relations.

L9. When one operation seems to expose several different "results," prefer to
think in terms of one preserved structure and several lawful reads of it.
If a richer relation such as a kernel, survivor family, or unresolved pair is
preserved, later reference/cast/family views should increasingly be preferred
over bespoke inspection helpers.

L10. A later read cannot recover structure that was not preserved.
So Core3 should preserve meaningful structure first, and only then rely on
frame/family reads to expose different views of it.

## Part XII. Binding

B1. Binding is contextual, not core ontology.

B2. Bound literals are not yet the same thing as fully materialized engine
elements.

B3. Binding should prefer native Core3 numeric descriptor structure over opaque
enum-like metadata where lawful.

B4. Binding descriptors should remain navigable by nearby numeric adjustment,
not only by code branching.

B5. Selection should be treated as one case of contextual binding.

B6. Binding should remain explicit about:

- source domain
- address or parameter
- projection or transform
- storage target if retained

B7. Missing binding-time slots use `null`.
This must not be collapsed into engine-level zero or unresolved-unit meanings.

B8. Coupling is real structure reduction, not display sugar.

## Part XIII. Native Core3 Exploration

X1. Meanings should prefer to live in native Core3 structure.

X2. A machine should be inspectable and, where lawful, perturbable by adjusting
Core3 values, supports, tensions, positions, and descriptors rather than by
inspecting code-only branching structure.

X3. The system should increasingly support exploration by:

- tension pushes
- nearby numeric descriptor changes
- changed route positions
- changed carrier relations
- changed bindings
- changed continuation laws

X4. If a meaning can only be changed by editing code words or enum branches,
that is usually a signal that the meaning is not yet expressed deeply enough in
Core3 terms.

X5. When temporary helper layers are necessary, they should stay thin and point
back toward native Core3 structure rather than becoming a second deep
mechanism.

## Part XIV. Coding Guidance

C1. Prefer code that makes structure and invariants easy to see.

C2. Passive schema types should stay light.

C3. Use exceptions for broken invariants and malformed ontology, not for
ordinary lawful tension.

C4. Prefer `Try...` for expected structural non-settlement.

C5. Avoid repetitive guard boilerplate that does not add explanatory value.

C6. Prefer physically interpretable implementations over hidden virtual
machinery.

## Part XV. Serialization

S1. Serialization should remain explicit and local while shapes are still
moving.

S2. Serialized forms should prefer structural clarity over framework magic.

S3. The current manual JSON layer is a staging form for later binary / compact
encodings.

S4. Minimal serialization and derived/debug serialization should remain
distinguishable.

## Part XVI. Present Recommendation

PR1. Keep `AtomicElement` and `CompositeElement` as the current clean leaf /
branch split of the graded engine.

PR2. Continue consolidating Core3 toward:

- routes
- sites
- movers
- reads
- laws
- carried data

PR3. Treat mover advancement, route traversal, equation traversal, and flow
traversal as members of one conceptual family.

PR4. Prefer lawful outputs with tension over null/failure when the situation is
structurally meaningful.

PR5. Reuse metaphors aggressively but lawfully.
If a stretched metaphor allows the machine to see the same pattern in numbers,
paths, equations, glyphs, language, and branching structures, that is
desirable.

PR6. Keep pushing meaning downward into native Core3 elements and relations so
future machines can discover patterns in the structure itself rather than only
in surrounding code.

## Part XVII. Core2 Continuities In Scope

K1. Preserve the Core2 duality intuition:

- encode versus act
- dominant versus recessive
- same versus opposite
- local description versus later placement

when those distinctions still map cleanly onto current Core3 structure.

K2. Preserve the Core2 rule that structure comes first and later folds or
readings interpret that structure afterward.

K3. Preserve the Core2 rule that repetition is general.
Traversal, iteration, continuation, looping, equation stepping, and boundary
behavior should be read as one family of continuation structure.

K4. Preserve the Core2 rule that traversal is stateful.
Current position, facing, continuation law, and seed/history should remain part
of the lawful state rather than being treated as display-only setup.

K5. Preserve the Core2 rule that boundary labels are usually summaries of
encountered structure rather than the deepest primitive laws.

K6. Preserve the Core2 rule that pinning is fundamental and folding is later.

K7. Preserve the Core2 boolean/frame insight:
evaluate inside a frame, partition by relevant boundaries, preserve co-present
pieces, and carry provenance separately from the elements themselves.

K8. Preserve the Core2 tension rule:
when a distinction cannot settle honestly in the current representation, prefer
tension or lift over premature collapse.

K9. Preserve the Core2 pattern that temporary operational roles do not require
a second ontology.
Promotion into frame role, temporary focusing, temporary derived ordering, and
similar runtime acts should prefer role change over substance change.

K10. Preserve the Core2 reuse principle:
if the same structural pattern can explain number stepping, route traversal,
equation traversal, ornament, glyph formation, or language branching, that is
preferable to introducing unrelated mechanisms.
