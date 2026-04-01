# Core3 Data Layer Discussion

## Context

Exploring how to replace the older DataArcs numeric system
(Series / Stores / Samplers) with Core3's grade-based architecture.
The DataArcs system used float arrays with separate sampler strategies.
Core3 uses `GradedElement` values, frame-relative reads, recursive
composition, and tension-bearing outcomes.

## Key Architectural Insight: Unit Sign Drives Carrier Interpretation

The strongest idea here is still good:

- positive unit suggests aligned / calibrative reading
- negative unit suggests orthogonal / organizational reading
- zero unit suggests unresolved / generative demand

But the safest current Core3 statement is:

- unit sign gives the first carrier interpretation
- full runtime behavior still depends on grade shape, host/frame relation,
  continuation law, retention, reduction, and any explicit local law

So unit sign is a strong guide, not yet a complete runtime dispatch system by
itself.

### Positive Unit (Aligned) -> Value Reading Bias

- Frame and data share a carrier
- "Read these members as calibrated amounts in my unit space"
- Frame provides resolution and bounds
- This is measurement / calibration
- Current Core3 reading: aligned / calibrative / value-facing

### Negative Unit (Orthogonal) -> Structural Reading Bias

- Frame introduces orthogonal organizational pressure on the data
- "Organize these members along my axis"
- This rhymes with pinning: locating elements orthogonal to their own nature
- Strides are orthogonal axes imposed on flat data
- Swizzling is path selection through imposed structure
- Neighbor relationships emerge from adjacency within imposed dimensions
- Current Core3 reading: orthogonal / organizational / structural-facing

Negative unit should not yet be read as automatically choosing wrap, reflect,
clamp, or another continuation law by itself. Those remain later law choices,
even when orthogonality is what made the richer organization visible.

### Zero Unit (Unresolved) -> Generative / Evaluative Demand

- Relationship between frame and data has not settled
- "The answer does not exist yet -> derive it"
- Interpolation, curve derivation, area emergence, linguistic arcs
- Current Core3 reading: unresolved / generative demand

Zero unit can therefore signal that a generative law is needed, but zero alone
does not yet choose whether that law should be interpolation, fold, branch
expansion, curve generation, or something else. That selection still needs an
explicit source.

## Why Not An Enum

A composite frame can have mixed unit signs in its children.
The recessive side might be structural while the dominant side is evaluative.
A single frame can therefore express several reading pressures at different
depths.

The dispatch wants to be structural, not just conditional.
`CommitToCalibration` already behaves differently based on carrier relation,
and this discussion is trying to push higher collection reads in the same
direction.

Current code is not fully there yet. The data-layer view recursion still uses
some higher-level helper choices and does not yet derive every reading from a
purely native frame/law structure.

## Unification Principle

Pin, stride, and projection are all the same gesture:
impose orthogonal structure that reorganizes what is beneath it.

The main difference is scope:

- Pin -> structural organization of a single element's position
- Stride -> structural organization of a collection's topology
- Projection -> structural reorganization within elements' composite paths

## Data Classes Overview

Instead of separate classes per operation type, the `Data` folder currently
contains:

- `FamilyView` - unified reading of a family through a frame, where the frame's
  unit signs guide whether each level calibrates values, organizes structure,
  or eventually generates new structure
- `FamilyInterpolation` - weighted blend between two elements
- `FamilyBlend` - multiple family views blended by weight
- `FamilyCombine` - sequential accumulation across views with a custom law
- `FamilyStride` - multi-dimensional access through orthogonal dimensions
- `FamilyFold` - recursive pairwise reduction
- `FamilyProjection` - component selection via structural path descent

These are best understood as higher-layer experiments over the main Core3
engine, not as a second competing ontology.

## DataArcs -> Core3 Mapping

| DataArcs | Core3 Equivalent |
|----------|------------------|
| Series (float[]) | `Family` with atomic/composite members |
| Store (sampler + capacity) | `FamilyView` |
| BlendStore | `FamilyBlend` |
| FunctionalStore | `FamilyCombine` |
| GridSampler / HexSampler | `FamilyStride` |
| Slot enum + swizzle | `FamilyProjection` |
| BezierSampler | `FamilyFold` with de Casteljau |
| CombineFunction enum | `GradedElement` laws such as `Add` / `Multiply` |
| BakeData() | `FamilyView.Materialize()` |
| ParametricSeries | `TraversalMover` |

## Revised Architecture (Round 2): Frame-Driven Dispatch

### FamilyView - the unified reader

`ReadThroughFrame()` inspects the frame and dispatches:

- aligned atomic leaf -> `ReadValueCalibrated()`
- orthogonal atomic leaf -> `ReadStructural()`
- unresolved atomic leaf -> currently falls back to value read
- composite frame -> currently uses a coarser composite read, with fuller
  per-child recursion still aspirational

`GetNeighbors()` extracts orthogonal leaves from the frame to determine
structural dimensions.

`Materialize()` bakes virtual positions into actual members.

### FamilyStride - now a frame builder, not a sampler

- Changed from a wrapper class to a static helper that builds structural frames
- `CreateStride(width)` -> `AtomicElement(width, -width)`
- `CreateGrid(cols, rows)` -> `CompositeElement(colStride, rowStride)`
- `CreateStructuredWithCalibration()` -> mixed frame with structural and
  aligned roles
- `CreateNDimensional()` -> nested composites from N dimension widths
- `ApplyTo()` -> sets a family's frame to a structural frame and returns a
  `FamilyView`

### FamilyProjection - structural reorganization within elements

- Same gesture as stride but operating within each member rather than across
  the family
- Path descent through composite tree = orthogonal structure selection
- Still path-backed in code for now, not yet frame-native

### FamilyBlend - boundary between structural and generative

- Selection of adjacent views = structural organization across views
- Interpolation between them = generative evaluation
- Current sketches still simplify tension/provenance more than mature Core3
  probably should

### FamilyCombine - primarily value-facing

- Each view reads through its own frame
- Results are then combined with an arithmetic law
- Current sketches still simplify tension/provenance more than mature Core3
  probably should

### FamilyInterpolation / FamilyFold - generative layer

- Both are still parked as generative sketches
- A zero-unit frame leaf could eventually invoke fold/interpolation to resolve
  tension
- Fold intermediate levels are real derived structure, not disposable
  computation

## Design Principle: Three Layers, One Mechanism

The three layers are not meant to become a hard enum or mode switch.
They are what the frame is currently asking for:

- positive unit -> value calibration
- negative unit -> structural organization
- zero unit -> generative demand

A composite frame can mix all three at different depths.

This is why the most promising long-term reading is:

- one holder
- one framing / viewing story
- one tension-bearing result model
- several lawful reads over preserved structure

## Current Fit With Mainline Core3

The strongest parts of this discussion already fit the main engine well:

- `Family` is a good neutral holder / data-area surface
- `EngineView` is already the single-element version of "read through a frame"
- `TraversalMover` is already a good parametric cursor for collection reads
- `OperationPipeline.md` already treats family as the operation-time data area
- negative unit already means orthogonal pressure in the engine, which matches
  the structural-reading intuition here

So the long-term story can stay coherent:

- holder
- frame / view
- mover
- law
- tension-bearing result

This folder is therefore best read as a higher friendly layer over that core,
not as a separate engine underneath the engine.

## Where This Sketch Is Still Ahead Of The Code

Some parts of the discussion are directional rather than fully implemented:

- `FamilyView` comments describe deeper recursive frame-driven dispatch than the
  current composite implementation actually performs
- orthogonal structure currently suggests organization, but it does not yet
  automatically provide a lawful continuation behavior such as wrapping
- zero-unit generative demand does not yet carry a settled native law-selection
  mechanism
- `FamilyProjection` is still path-backed rather than frame-native
- blend / combine / fold sketches are not yet preserving full accumulated
  provenance and tension the way mature Core3 results likely should

Those are good research directions, but they should stay marked as such so the
docs remain honest about what current code is doing.

## Round 3: The Trichotomy Across All Layers

### The Core Pattern: Value / Structure / Generation as 1 / -1 / 0

The three aspects map to unit sign arithmetic that's already in the engine:

- **Value** (aligned, +1): What something measures. The actual, the present.
- **Structure** (orthogonal, -1): What constrains and organizes values.
  Forces that act on values without being values themselves.
  The frame, the boundary, the relationship. You can't measure a frame —
  you measure *through* a frame.
- **Generation** (unresolved, 0): What becomes. What appears when values
  meet structure. Not in either input — emergent from the encounter.
  Tension is partially-completed generation.

The multiplication table is physically real in the engine:
- Value × Value = Value (1 × 1 = 1)
- Structure × Structure = Value (-1 × -1 = 1) — two orthogonal forces
  meeting produce something aligned
- Value × Structure = Structure (1 × -1 = -1)
- Anything × Unresolved = Unresolved (n × 0 = 0) — generation absorbs

### How It Manifests Per Layer

**Engine:**
- AtomicElement literally carries the trichotomy in every instance via unit sign
- CompositeElement pairing is itself structural (it organizes two children)
- Fold is the simplest generative act: two values meet across a structural
  boundary and a ratio appears that wasn't in either child
- Tension is partially-completed generation — structure that wants to emerge
  but can't yet
- CommitToCalibration is value work
- Align is structural work (reconciling two frames)
- Multiply with mixed signs produces the aspect the sign arithmetic predicts

**Operations (Family):**
- Frame = structure (organizes members without contributing data)
- Members = values (the things being organized)
- Accumulation (AddAll, MultiplyAll) = generative (new element from combining)
- Sorting = structural reorganization of values
- Focusing = shifts what counts as structure vs value — same element, different
  role. This is relational, not intrinsic: elements don't have a permanent
  aspect, they play different roles depending on what frame they're read through
- Boolean operations = generative (truth values emerge from structural overlap
  patterns). The 4-bit truth table IS the generative law.

**Binding (Traversal):**
- TraversalMover position = value, route extent = structure, each step = generation
- The route IS the clock. No external time needed. Traversal from recessive to
  dominant IS time passing. Resolution (EndTick) = temporal granularity.
  Ordering (recessive before dominant) = arrow of time.
- OperationAttachments = generative events pinned to structural positions.
  They're literally pins: positioned (structure) carrying operations (law)
  producing results (generated values) as the mover passes through.
- BoundTemplates = structure waiting for values. Shape is structural, holes
  are where values go, materialized result is generated.

**Pin as microcosm:**
- Host = structure (the containing span)
- Position = value (where within the span)
- Inbound/Outbound = generated (new composites that weren't inputs)
- You can inspect this physically: look at the pin result, see two new elements
  that weren't in the host or the position. That's the test for generation.

**View as microcosm:**
- Frame.Recessive (Calibration) = structural reference
- Frame.Dominant (ExistingReadout) = value reference
- Read outcome = generated (subject as seen through frame)
- Tension = generation that couldn't fully resolve
- GetBoundaryAxis = what's unresolved between reading and frame

### Time as Inherent in Ordering

Time is not bolted onto the system. It's what you get when you take any ordered
structure and traverse it. A grade-1 composite already contains "before"
(recessive) and "after" (dominant). A mover traversing it makes that temporal
potential actual.

The VALUE aspect of time = current position
The STRUCTURAL aspect of time = the route/extent
The GENERATIVE aspect of time = what happens at each step

### Avoiding Overfitting

The pattern is relational, not intrinsic. The same element can be value in
one context, structure in another, and generative potential in a third.
What determines its role is the frame it's being read through.

Not every recessive/dominant pair is "structure/value." Sometimes both
children are values (a 2D point). Sometimes both are structural (a grid frame).
Recessive/dominant is about perspective priority (reference vs applied), not
about which aspect.

The physical test: **can you observe it without asserting it?**
- Look at unit signs → read the aspect
- Look at operation results → see if new elements appeared (generation)
- Look at tension → see unfinished generation
- Look at frame structure → see what's organizing what

Don't add AspectType enums. Add inspection methods that read what's already
there. The trichotomy is in the arithmetic, not in labels.

### What This Suggests For Future Development

1. Inspection methods on GradedElement for reading aspects from leaf signs
2. Comments/docs naming the aspects where they're already happening
3. The Data layer as proving ground for explicit frame-driven dispatch
4. Trust the multiplication table — unit sign arithmetic already produces
   the correct aspect for operation results
5. Generative layer law selection (how zero-unit frames choose their law)
   remains the key open design question

## Round 4: Denominators, Dimensional Fusion, and the Generative Moment

### The Denominator as "What Is One"

The Unit in AtomicElement is literally "what is one." When two elements sit
in a composite with independent units, each still knows its own "one."
They're pinned together structurally but haven't agreed on a shared "one."

The generative moment — when area appears from two lengths — is when two
independent "ones" combine into a single "one" that is neither.
A 1m × 1ft unit is a valid "one" for area (unusual but legitimate).
The key: it's ONE thing, not two things sitting next to each other.

### Two Phases of Composition

**Phase 1: Structural co-location (orthogonal, independent units)**
Two things placed in a composite. Each retains its own denominator.
Width AND height. Two measurements. The composite is a container, not a fusion.
Numerators AND denominators can vary independently. The relationship is
positional (next to each other) but not dimensional (no shared space).

**Phase 2: Dimensional fusion (combined denominator, new entity)**
The two denominators combine into a shared "one." Now you have area.
The numerators still vary freely (they're measurements within the new space).
But the denominator is locked — you can't reexpress just one side because
the "one" is a combined "one."

The critical difference: in Phase 1, recessive and dominant are peers with
different rulers. In Phase 2, there's a single ruler spanning both.

### Area Is Unrelated to Two Orthogonal Lines

A circle has area. A curved surface has area. Area doesn't require axes.
So the generative moment isn't just "combine the units." It's the moment
where the SPACE ITSELF becomes the entity, and the original axes become
merely one possible measurement frame for that space.

Before fusion: the axes ARE the structure.
After fusion: the axes are just one way to READ the structure.
The structure has its own existence independent of how you decomposed it.

In Core3: a grade-1 composite with independent units = two things in a
structural relationship. A folded atomic with combined unit = one thing
that you COULD decompose but doesn't have to be. The fold didn't just
merge numbers — it created an entity that transcends its construction.

### The Progression of Denominators

1. **Both denominators free** → Pure structure. A grid, skeleton, topology.
   Nothing measured yet, just relationships. Everything rearrangeable.
2. **One fixed, one free** → Partially calibrated. One dimension committed.
   Like having a timeline but not yet a spatial resolution.
3. **Both fixed but independent** → Fully structured, not fused.
   Width in meters and height in feet. Still "a width AND a height."
4. **Denominators combined into one** → Fused entity. Area. Velocity.
   A new thing that transcends its construction. Numerators are readings
   within this entity. The entity could be decomposed in different ways.

Structure crystallizes as denominators commit.
Generation happens at the final combination step.

### Refined Trichotomy Definition

**Value (+1 unit):** Numerator free, denominator settled. A measurement
in a known space. "One" is established. Reading values within that space.

**Structure (-1 unit):** Denominators independent. Multiple dimensions
aware of each other but not fused. Each has its own "one." You can
reorganize, pin, stride — but you're working with relationships between
things, not with a fused entity.

**Generation (0 unit):** Denominators in the process of combining.
The old "ones" are gone, the new "one" hasn't stabilized. Specifically
"between unit spaces." The value has magnitude but no "one" to measure
against. This is the state between "two independent ones" and
"one combined one."

### Fold and Multiply as Dimensional Fusion

Fold and Multiply are doing something more significant than arithmetic —
they're attempting dimensional fusion.

- Multiply on two orthogonal atomics: unit signs combine (-1 × -1 = +1).
  The sign change IS the fusion. "Two orthogonal ones" → "one aligned one."
- ComposeRatio in Fold: composite → single atomic. Two independent units →
  one unit. If it succeeds, dimensional fusion happened. If tension remains,
  the fusion is incomplete.
- Tension from failed fold = "these want to become one entity but can't
  agree on a shared 'one' yet."

This is already happening mechanically in the engine. The interpretation
adds: this isn't just number manipulation — it's the creation of new
dimensional spaces from the agreement of independent measurement frames.

## Round 5: Laws, Unit Categories, Structure Mechanics, and the Axiom System

### Why the Unit Cell Must Be 1×1

For a fused dimensional space to be coherent, the unit cell must be symmetric
under exchange of axes (commutativity: 2×3 = 3×2). The unique cell satisfying
this is 1×1 in whatever the respective axis units are. This is visible in all
compound units: mph (per 1 hour), PSI (per 1 square inch), N (per 1 kg·m/s²).
Even "liters per 100km" treats 100km as a single "one" at that scale.

In Core3 terms: Scale multiplies Unit * factor.Unit. The product is the
resolution of the fused space, and each input contributes its full resolution
as "one." The 1×1 cell emerges naturally.

This is WHY denominators must be settled before fusion. If either axis has
a free denominator, you don't know what "1" means for that axis, so you can't
construct the 1×1 cell. Free denominators = structure. Fixed = ready for
fusion. Combined = fused entity.

This also resolves why ticks/resolution seem necessary even though bare longs
should suffice. In one dimension, unit=1 works fine. But fusion requires
knowing each axis's "one" independently to build the product "one." Bare
longs lose the dimensional structure — you can't distinguish "3×A times 2×B"
from "6 of the product space."

### Laws and Unit Categories

Fundamental observation: the unit category on each axis CONSTRAINS which laws
are valid. Laws are not arbitrary choices — they're partly determined by what
the axes are.

Identified fundamental laws (a small set covering most mathematical operations):

1. **Expand** (multiply) — continuous, needs resolution throughout. Stretching.
2. **Accumulate/Get** (add) — general, works on continuous or discrete.
3. **Partition** (divide/ratio) — needs endpoints, discrete structure.
   Cannot have midpoint without knowing both ends.
4. **Boolean** (and/or) — local, needs only co-presence, not endpoints.
   Can partially resolve even with unknown endpoints.
5. **Repeat** — iteration of another law. Discrete meta-law.
6. **Move** (translate value) — shift within a space. Other-moving.
7. **Shift perspective** (reframe) — change the frame, not the value. Self-moving.
8. **Run** (fold/interpolate) — traverse and derive.
9. **Split/Merge** — roots, averages, branching, joining.
10. **Associate** (tension) — relate unlike forces.

Plus: **Unknown/random** — leftover tension once things settle.

### Laws Live on the Axis, Not Just the Pin

Key insight from the 3×2 apples example:

If X axis = "get stuff" (accumulate) and Y axis = "repetition" (repeat),
then fold gives 6 apples LINEARLY — repeated addition, not area expansion.
Even though repeats could be modeled on Y, the meaning is "repeat the
operation of X axis, which is addition."

If X axis = "expand" and Y axis = "expand", then fold gives 6 square units
with uniform growth — area. Same scalar result, completely different geometry
and interpolation behavior.

So: **the axis category carries operational semantics**, not just descriptive
metadata. The law comes from WHAT THE AXES ARE, not from an arbitrary choice.

Two-level law system:
1. **Implicit/default law** derived from unit categories of the inbound axes.
   What the axes "want" to do.
2. **Explicit law** from the pin or operation. What you're actually asking for.
   Can override the default. Tension arises when explicit law conflicts with
   what the axes naturally support.

### Pin Structure at Dimensional Fusion

When two defined axes fold, the pin looks like:

- Numerator side: the combined value (measurement in fused space)
- Denominator side: zero — the fused space's "one" is new, needs establishing
- The LAW that resolves the zero determines what KIND of fusion

The pin doesn't carry the law as arbitrary choice. The law is DERIVED from
the inbound axis categories. But you CAN override (add areas, multiply
counts). Override produces tension when it conflicts with natural law.

### Directed Segments and the Complex Unit Cell

The complex i+1 unit cell is to directed segments what 1×1 is to area.
Both are fused 2D spaces measuring different things:

- Real 1×1 measures unsigned area (magnitude of 2D extent)
- Complex i+1 measures oriented extent (magnitude AND direction)

What the complex unit measures beyond aligned segments:
- Rotation/phase — a point on the unit circle is pure direction
- Oscillation — tracing the circle is periodic motion
- Interference — directed segments can cancel (complex addition)
- Signed area / orientation — imaginary part of complex product = determinant
- Conformal mapping — complex multiplication preserves angles

Question: what other unit cell structures exist beyond real (1×1) and
complex (i+1)? Quaternions? Split-complex? What do they measure?

### Structure Mechanics

**Segments**: A grade-1 composite (start, end). Directed. Traversable.

**Branches**: A pin producing two outbound segments from one inbound.
The pin position is where the choice happens. Before: one path. After: two.

**Merges**: Two inbound segments meeting at a pin, producing one outbound.
If compatible (same unit space): exact. If not: tension.

**Loops**: A segment whose outbound connects to an ancestor's inbound.
Topological identification: "this end IS that beginning."
Creates period and enables repetition.

**Flow direction matters physically:**
- Convergent (→←): merge/collision. Maximum tension point. Boundary/wall.
- Divergent (←→): branch from common origin. Choice/fork/source.
- Parallel (→→): co-traveling. Space between is the structure (offset, gap).
- Anti-parallel (→ over ←): loop or standing wave. Reflection/interference.

**Area from 1D structure**: Area isn't IN the structure — it emerges from
topology. A loop encloses area even though the loop is 1D. The area is
what the loop BOUNDS. Structure is always paths/segments/routes, but
the topology of connections can enclose higher-dimensional spaces.

**Equations**: Both sides flow into a shared merge point. The equation is
SATISFIED when tension at the merge is zero. Variables are segments with
free denominators (can be reexpressed). Solving = finding the reexpression
that makes the merge exact.

**Structural variables (Fibonacci etc.)**: A branch producing two outputs
where one feeds back as input to a later instance of the same branch
pattern. The variable is the value in the feedback loop. Each traversal
step produces the next value. The structure IS the recurrence relation.

### Toward the Axiom System

Proposed minimal axiom set:

1. **Elements**: Atomic (value/unit pair), Composite (recessive/dominant, equal grade)
2. **Three aspects**: aligned (+1), orthogonal (-1), unresolved (0) via unit sign
3. **Aspect multiplication table**: (+1)(+1)=+1, (-1)(-1)=+1, (+1)(-1)=-1, (n)(0)=0
4. **Denominator progression**: free → fixed → independent → combined (1×1)
5. **Dimensional fusion**: fold/multiply as transition from independent to
   combined. Requires commutativity (1×1 unit cell).
6. **Laws**: small finite set, constrained by unit category.
   Default law from axes, explicit law from pin/operation.
7. **Structure**: segments (directed grade-1), pins (positioned on segments),
   branches/merges/loops from pin topology.
8. **Flow**: direction of segments determines encounter type at pins.
   Convergent/divergent/parallel/anti-parallel.
9. **Tension**: incomplete fusion, incompatible merge, conflicting laws.
   Always preserved, never discarded. Is partially-completed generation.
10. **Traversal**: ordered structure implies interpolation. Interpolation
    implies before/after. Before/after IS time. The structure IS the clock.

Areas needing more crystallization before axiom formalization:
- Exact relationship between unit categories and default laws
- Flow direction mechanics at branch/merge points
- How equations and variables emerge from structural topology
- Whether the law set is complete or needs additions
- How higher-dimensional spaces (area, volume) emerge from 1D topology

## Round 6: Bootstrap, Directed Segments, and Dimensional Promotion

### The Bootstrap from Nothing

The system constructs itself from minimal assumptions:

**Step 0 — Uncalibrated measures exist.** Ordered magnitudes. No units,
no resolution, just "this is bigger than that."

**Step 1 — Pin two measures into a ratio.** Place any two in a composite:
(a | b). This is meaningful ("a relative to b") even though neither is
calibrated. No number yet — just a relationship. The purest structural act.

**Step 2 — Self-fold creates a unit.** The ratio commits to itself as its
own reference. "I am one of me." After fold: an atomic with Value and Unit.
The unit IS the ticks. It emerged from the relationship between two
uncalibrated measures, not from any external decision about resolution.

**Step 3 — Alignment creates numbers.** With a unit established, other
uncalibrated measures can be committed to it. They become actual numbers —
values expressed in terms of the self-declared "one." Numbers are born
FROM a unit, not pre-existing things a unit gets applied to.

Assumptions: ordered magnitudes exist. Everything else is constructed.
Ratios → units → numbers. Complete.

### Grade 2: The Directed Segment

At grade 1, for genuine tension (dimensional pressure), you need one
aligned (+) and one orthogonal (-) unit. Two positives just add. Two
negatives are structurally like two positives. But positive meeting
negative: genuine encounter between incompatible orientations.

There is only ONE orthogonal direction at this point — the opposite
direction. So the unit cell is: one step positive + one step negative =
a directed segment. Start and end. Not rotation — directed extent.

**A directed segment is not a complex number.** A directed segment has
a start and end (two positions). A complex number has magnitude and angle
from zero. The directed segment doesn't privilege the origin.

**What else can the directed segment unit measure?**
Anything with "from" and "to" along a single quality:
- Time interval (from 2pm to 5pm)
- Temperature range (from 20°C to 35°C — "warming" has direction)
- Price spread (bid to ask)
- Potential difference (voltage between two points)
- Gradient (low concentration to high)
- Musical interval (C to G, ascending vs descending)
- The two directions need not be the same physical quantity

### The Four-Term Kernel and the Emergence of Complex Arithmetic

At grade 2: a composite of two directed segments. Four atomic leaves:
((rr | rd) | (dr | dd)). When these multiply/fold, the four-term kernel
appears. Each term has physically distinct meaning:

**dd (dominant × dominant):** Both expanding. Two outward pushes meeting
orthogonally → AREA CREATION. The space between opens up.

**rr (recessive × recessive):** Both contracting. Two inward pulls →
ANTI-AREA. The structural opposite of area creation.

**rd and dr (mixed):** One expanding, one contracting. These CANCEL
dimensionally — one pushes out, the other pulls in. Net dimensional
contribution: zero. What's left is a LINE — directed extent without
enclosed space. Extent to the corner point.

CompositeElement.Multiply computes exactly:
- squareDifference = rr - dd → NET AREA (anti-area minus area)
- crossSum = rd + dr → NET DIRECTED EXTENT (the two line terms)

This IS complex multiplication. Not introduced — it FALLS OUT of the
grade system. The "real" part is the net area contribution (aligned).
The "imaginary" part is the net directional residue (orthogonal).

### Corner Resolution: From Directed Segment to Complex Number

When one orthogonal and one aligned value meet at a corner:
- The corner creates tension (incompatible orientations)
- If one creates and one destroys (rd or dr): they cancel → LINE from
  origin to corner point. This is when you get a complex number — a
  directed extent FROM the origin. The origin-anchoring comes from the
  cancellation of area.
- If both dominant (dd): both creating → AREA emerges
- If both recessive (rr): both destroying → ANTI-AREA

The transition from directed segment to complex number IS the corner
resolution where area cancels to a line.

### Dimensional Promotion Through Symmetry Breaking

The recipe for creating a new dimension is NOT "state that a new axis
exists." It is: **tension between incompatible orientations fails to cancel,
and the excess that can't be expressed in the current dimensionality
becomes the new dimension.**

**0D → 1D:** Two uncalibrated values create tension when pinned. The
tension resolves to a directed extent. Direction emerges.

**1D → 2D:** Two directed segments create a four-term kernel. The dd and
rr terms cannot be expressed as lines — the excess IS area. Area emerges
from failed cancellation.

**2D → 3D (hypothesis):** Area has a natural flow (start size → end size,
like a directed segment but 2D). To get volume:
- Area is dd-like (both dominant, both creating)
- Combine with the complex/line term (rd/dr — one creating one destroying)
- The line "pierces" the area orthogonally
- The space swept out is volume
- Volume = 2D area × 1D symmetry-breaking extent

The pattern: Dimension N+1 = Dimension N entity × orthogonal extent that
BREAKS the symmetry of the N-dimensional entity. "Breaking" means: the
new extent has a different dominance pattern than the existing entity.

Two expansions give area. Area + line gives volume. Not area + area
(that's "more area" or growing area, still 2D). The symmetry break is
essential.

This means dimensions are not arbitrary — they're forced by the structure
of how tension resolves. The machine doesn't explore arbitrary axes.
It follows the forced path of tension resolution. The search space
collapses from "all possible dimensions" to "the specific dimensions
that emerge from unresolved tension."

### The Pin at Dimensional Fusion (Refined)

The pin naturally produces ALL FOUR kernel terms. The law isn't a single
choice — the pin computes the full kernel, and the dominance structure
determines interpretation:

- dd terms → area contribution → goes to aligned/real result
- rr terms → anti-area → goes to aligned/real (with opposite sign)
- rd/dr terms → line contribution → goes to orthogonal/imaginary result
- Net result: (rr - dd) aligned + (rd + dr) orthogonal

The EXPLICIT law (add, multiply, etc.) is a FURTHER operation on top of
the natural kernel. Natural law gives the kernel. Explicit law says what
to DO with it.

### Implications for the Axiom System

The bootstrap and dimensional promotion stories should be central axioms:

1. Ordered uncalibrated measures are the only assumption
2. Pinning creates ratios (structure from values)
3. Self-fold creates units (calibration from structure)
4. Alignment creates numbers (values from calibration)
5. Orthogonal tension creates directed segments (grade 1)
6. The four-term kernel is universal for grade 2+
7. Dominance patterns determine dimensional character of each term
8. Excess terms that can't be expressed at the current dimension
   become the new dimension (dimensional promotion)
9. Complex arithmetic is emergent, not introduced
10. Each higher dimension follows the same symmetry-breaking pattern

## Open Questions

1. The law set — is it complete? What's missing? Are some of these
   really the same law in different contexts?
2. What other unit cell structures exist beyond directed segment and
   area? Does the grade system predict quaternion-like structures at
   grade 3?
3. How exactly do flow directions at pins determine merge/branch behavior?
   Is there a small truth table like the boolean 4-bit table?
4. Can structural variables (feedback loops) represent all recurrence
   relations, or only linear ones?
5. When a loop encloses area: how does the system "know" about the
   enclosed area? Is it detectable from the structural topology alone?
6. The equation-as-merge-point idea: how do multi-variable equations
   work? Multiple free-denominator segments all meeting?
7. What determines when tension promotes to a new dimension vs when
   it just stays as unresolved residue?
8. The grade 3 → volume argument needs verification: does the
   dominance pattern at grade 3 (8 leaves) actually produce the
   expected dimensional behavior?
9. Structure mechanics: branching, merging, looping, flow direction.
   This is the biggest remaining gap before the axiom system can be
   written. Needs its own focused round.
