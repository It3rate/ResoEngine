# Core4 Axiom System

A physical mathematics bootstrapped from uncalibrated extents.
No real numbers, no stated axes, no assumed dimensions.
Everything constructed from ordered magnitudes through pinning, folding, and tension.

---

## Part I: Foundation

### Axiom 1 — Extents Exist

There exist **extents**: ordered, comparable magnitudes. An extent has
no unit, no resolution, no sign. It is simply "some amount" that can be
compared to another extent by size. This is the sole assumption.

We write an extent as a bare value: `a`, `b`, `c`.

### Axiom 2 — The Atomic Element

An **atomic element** is a pair `(v, u)` where:
- `v` is a signed integer: the **value** (how many)
- `u` is a signed integer: the **unit** (what "one" is)

The unit sign carries physical meaning (Axiom 5). The unit magnitude
`|u|` is the **resolution**: how many ticks compose one unit. When `u = 0`,
the element has magnitude but no established "one" — it is **unresolved**.

An atomic element is **grade 0**: the leaf of all structure.

### Axiom 3 — The Composite Element

A **composite element** is an ordered pair of two equal-grade elements:

    Composite(recessive, dominant)    where recessive.grade == dominant.grade

The composite's grade is `children's grade + 1`.

- **Recessive**: the reference side, the context, the "before"
- **Dominant**: the applied side, the measured, the "after"

This is the only way to build higher-grade structure. A grade-1 element
has two atomic children. A grade-2 element has two grade-1 children
(four atomic leaves). Grade N has 2^N atomic leaves.

### Axiom 4 — Ordering and Direction

Every composite has inherent direction: recessive → dominant.
This is not a convention. The recessive provides context for the dominant.
Swapping them (SwapOrder) changes the physical meaning.

Direction gives every composite a natural "before" and "after."
Traversal from recessive to dominant is the most primitive form of time
(see Part V). No external clock is needed.

---

## Part II: The Three Aspects

### Axiom 5 — Unit Sign Determines Aspect

The sign of an atomic element's unit determines its **aspect** — its
physical role in any encounter:

| Unit sign | Aspect | Meaning |
|-----------|--------|---------|
| `u > 0` (positive) | **Value** | Aligned. Shares a carrier with its context. A calibrated measurement in a known space. "One" is established. |
| `u < 0` (negative) | **Structure** | Orthogonal. A force acting on the context without being the context. Organizes, constrains, imposes a dimension. Does not contribute magnitude — contributes arrangement. |
| `u = 0` | **Generative** | Unresolved. The relationship between this element and its context hasn't settled. Has magnitude but no "one" to measure against. Between unit spaces. Demands resolution through a law. |

These are not labels applied externally. They are readable from the element
itself. You inspect the unit sign and the aspect is physically present.

### Axiom 6 — Aspects Are Relational

An element's aspect is not intrinsic — it depends on the frame through which
it is read. The same element can be value in one context, structure in another,
and generative potential in a third. What determines its role is the
relationship, not the element alone.

A composite frame has leaves with potentially mixed signs. Each leaf
contributes its own aspect independently. A single frame can simultaneously
calibrate values (positive leaves), organize structure (negative leaves),
and demand generation (zero leaves) at different positions in its tree.

### Axiom 7 — The Multiplication Table of Aspects

When two elements interact through multiplication or scaling, their unit
signs multiply. This determines the aspect of the result:

    (+1) × (+1) = +1    Value × Value = Value
    (-1) × (-1) = +1    Structure × Structure = Value
    (+1) × (-1) = -1    Value × Structure = Structure
    ( 0) × ( n) =  0    Generative × Anything = Generative

This is not metaphor. It is the actual arithmetic performed on the unit
field during `Scale` and `Multiply`. The engine computes aspect transitions
through ordinary integer multiplication of unit signs.

Key physical implications:
- Two orthogonal forces meeting produce something aligned (cross product → scalar)
- A value applied through structure produces structural position
- Anything touching the unresolved stays unresolved — generation absorbs

---

## Part III: Bootstrap

### Axiom 8 — Ratios from Extents

Take two uncalibrated extents and place them in a composite:

    Composite(a, b)

This is a **ratio**: "a relative to b." It is meaningful even though
neither `a` nor `b` has a unit. No measurement has been made. A
relationship has been created. This is the first structural act.

The ratio does not have resolution — neither child knows what "one" is.
But the relationship `a/b` is real and comparable to other ratios.

### Axiom 9 — Units from Self-Fold

A ratio can **fold**: the composite collapses into a single atomic element.

    Fold(Composite(denominator, numerator)) → AtomicElement(v, u)

The fold operation `ComposeRatio` takes the two children and produces
one atomic element where:
- The value `v` encodes the numerator's magnitude scaled by the denominator's resolution
- The unit `u` encodes the resolution, derived from the denominator's magnitude

This is the **bootstrap moment**. The ratio declares itself as its own
reference: "I am one of me." The denominator becomes the tick structure.
The numerator becomes the count within that structure.

**Self-fold is how units come into existence.** No external unit system
is assumed. Units are constructed from the relationship between two
uncalibrated extents. The denominator IS the ticks. It emerged from the
relationship, not from any external decision about resolution.

### Axiom 10 — Numbers from Alignment

Once a unit exists, other elements can be **aligned** to it:

    element.CommitToCalibration(unit) → calibrated element

Alignment reexpresses an element in terms of the unit's resolution.
The element becomes a **number**: a value expressed in terms of the
self-declared "one." Numbers are born from units, not the reverse.

The progression is complete:
1. Extents exist (Axiom 1)
2. Pin two extents → a ratio (Axiom 8)
3. Fold the ratio → a unit with resolution (Axiom 9)
4. Align to the unit → actual numbers (Axiom 10)

From "ordered magnitudes exist" alone, the entire number system is constructed.

---

## Part IV: Dimensional Emergence

### Axiom 11 — The Denominator Is "What Is One"

The unit field of an atomic element is literally "what one is."
When two elements sit in a composite with independent units, each retains
its own "one." They are co-located but not fused.

**Co-location** (Phase 1): Two elements in a composite, each with its own
unit. A width AND a height. Two measurements. Independent denominators.
Either side can be reexpressed without affecting the other.

**Fusion** (Phase 2): The two units combine into a single "one" that spans
both. Now you have area. The numerators still vary freely (measurements
within the new space), but the denominator is locked. You cannot reexpress
one side without affecting the other. The fused entity transcends its
construction — area can be circular, not just rectangular.

### Axiom 12 — The 1×1 Requirement

For dimensional fusion to produce a coherent space, the unit cell must be
**1×1**: each axis contributes exactly one of its own measure.

**Why:** Commutativity. For the fused space to behave as a single dimension,
2×3 must equal 3×2 in that space. A unit cell that is not 1×1 would have
an orientation bias that breaks commutativity.

This is visible in all compound units: miles per **one** hour. Pounds per
**one** square inch. Even liters per 100km treats 100km as **one** at that
scale. The system always normalizes each axis to contribute exactly one.

**Consequence:** Denominators must be settled before fusion can happen.
If either axis has a free denominator (u = 0), you cannot know what "1"
means for that axis, and therefore cannot construct the 1×1 cell.

    Free denominators = structure (not ready for fusion)
    Fixed denominators = ready for fusion
    Combined denominator = fused entity

In the engine: `Scale` multiplies `Unit × factor.Unit`. The product is the
resolution of the fused space. Each input contributes its full resolution
as its "one." The 1×1 cell emerges from ordinary arithmetic.

### Axiom 13 — Tension as Incomplete Fusion

When two elements attempt to fuse (through fold or multiply) and cannot
agree on a shared "one," the result carries **tension**:

    EngineElementOutcome { Result, Tension, Note }

- `Result`: the best lawful projection (what the system can produce)
- `Tension`: the unresolved structure (what couldn't settle)
- When `Tension` is null, the outcome is **exact** (fusion complete)

Tension is not failure. It is **partially-completed generation**: the
system preserving the structure that wants to become a new entity but
cannot yet. The tension tells you specifically what is unresolved and why.

Tension is never discarded. It propagates through operations via
`CombineTension`. The unresolved relationship remains inspectable and
available for later resolution.

### Axiom 14 — Directed Segments (Grade 1 Unit Cell)

At grade 0, all elements are atomic. To create grade-1 structure with
genuine **dimensional tension**, you need one aligned (+) and one
orthogonal (-) unit to combine. This is the only configuration that
creates dimensional pressure — two positives just add within the same
space; a positive meeting a negative is a genuine encounter between
incompatible orientations.

There is only **one** orthogonal direction available at grade 0: the
opposite sign. So the unit cell that emerges is:

    Composite(AtomicElement(1, -1), AtomicElement(1, +1))

One step in the negative direction, one step in the positive direction.
This is a **directed segment**: an extent with a start and an end.
Not a rotation — a directed extent. Something with "from" and "to."

A directed segment can measure anything with magnitude and polarity
along a single quality: a time interval, a temperature range, a price
spread, a potential difference, a gradient. The two directions need not
represent the same physical quantity.

**A directed segment is not yet a complex number.** A directed segment has
two positions (start and end). A complex number has magnitude and angle
from a privileged origin. The distinction matters (see Axiom 17).

### Axiom 15 — The Four-Term Kernel (Grade 2)

When two grade-1 elements (directed segments) multiply, the four-term
kernel appears. Each composite has a recessive (r) and dominant (d) child.
The multiplication produces four terms:

    rr = left.Recessive × right.Recessive
    rd = left.Recessive × right.Dominant
    dr = left.Dominant  × right.Recessive
    dd = left.Dominant  × right.Dominant

Each term has a physically distinct character determined by the dominance
pattern of its components:

| Term | Dominance pattern | Physical character |
|------|-------------------|--------------------|
| `dd` | Both expanding (dominant × dominant) | **Area creation.** Two outward pushes meeting orthogonally. The space between opens up. |
| `rr` | Both contracting (recessive × recessive) | **Anti-area.** Two inward pulls. The structural opposite of area creation. |
| `rd` | One contracting, one expanding | **Line.** Expansion and contraction cancel dimensionally. Net dimensional contribution: zero. Pure directed extent to a corner point. |
| `dr` | One expanding, one contracting | **Line.** Same cancellation, different order. |

The engine resolves this kernel into two components:

    squareDifference = rr - dd     → net area (aligned result)
    crossSum         = rd + dr     → net directed extent (orthogonal result)

This IS complex multiplication. It was not introduced — it **falls out**
of the grade system. The "real part" (rr - dd) is the net area contribution.
The "imaginary part" (rd + dr) is the net directional residue.

### Axiom 16 — Dimensional Promotion Through Symmetry Breaking

New dimensions are not stated. They **emerge** from tension that cannot
be expressed at the current dimensionality. The recipe is recursive:

**0D → 1D:** Two uncalibrated extents create tension when pinned.
The tension resolves to a directed extent. Direction emerges from the
incompatibility between the two extents trying to form a unit.

**1D → 2D:** Two directed segments interact. The four-term kernel
produces terms that cannot all be expressed as 1D lines:
- `dd` and `rr` terms are area-like — they represent enclosed space
  that has no 1D expression. The excess IS area.
- Area emerges specifically when both components are co-directional in
  dominance (both expanding or both contracting)
- `rd` and `dr` terms cancel to lines (the complex number part)

**2D → 3D:** Area has a natural flow (start size → end size). To produce
volume, area must combine with a line that **breaks the 2D symmetry**:
the line's dominance pattern must be incompatible with the area's
natural expansion/contraction. The line "pierces" the area, and the
space swept out is volume.

**General rule:** Dimension N+1 = Dimension N entity × symmetry-breaking
orthogonal extent. "Symmetry-breaking" means the new extent has a
different dominance pattern than the existing entity, forcing a genuinely
new dimensional direction.

**The search space collapses.** A machine navigating this system does not
explore arbitrary stated axes. It follows the forced path of tension
resolution. The only new dimensions that appear are those demanded by
unresolvable tension at the current grade. Dimensions are consequences,
not assumptions.

### Axiom 17 — Corner Resolution and Complex Numbers

When an orthogonal value and an aligned value meet at a corner, the corner
carries tension. Resolution depends on the dominance combination:

**Mixed dominance (rd or dr):** One axis expands, the other contracts.
They cancel dimensionally → the result is a **line** from the origin to the
corner point. The origin-anchoring comes from the cancellation of area.
This IS a complex number: a directed extent from a privileged origin.
The transition from directed segment to complex number is precisely this
corner resolution where area cancels to a line.

**Matched dominance (dd):** Both axes expand → **area** emerges.
Both are trying to create enclosed space.

**Matched recessiveness (rr):** Both axes contract → **anti-area** emerges.
Both are trying to destroy enclosed space.

Dominant area and recessive area combine to form an aligned area with
start and end — a directed area, analogous to a directed segment but
one grade higher.

The complex number (line from origin) combined with area can produce
volume — a variable-length line sweeping through a variable area.
The symmetry break (line's dominance vs area's dominance) is what forces
the third dimension into existence.

---

## Part V: Structure

### Axiom 18 — The Pin

Every structural transition happens at a **pin**. A pin relates an inbound
side to an outbound side, with an intermediate zero-denominator state where
the law acts.

A pin on a **host** (a composite span) splits the host at a position:

    Pin(host = Composite(start, end), position)
    → Inbound  = position - start
    → Outbound = end - position

The pin position is a value. The host is structure. The inbound and outbound
are **generated**: new elements that didn't exist in the inputs. The pin is
the site of generation.

### Axiom 19 — Merge and Branch

There are exactly two topological events. Everything else is traversal
along a segment between pins.

**Merge:** Two inbound segments → pin → one outbound. This is
**unit creation**: two things fuse through a law into one. Multiplication,
addition, fold, and every combining operation are merges. The
zero-denominator intermediate state at the pin is where the old units
dissolve and the new unit forms.

**Branch:** One inbound segment → pin → two outbound segments. This is
**decomposition**: one thing splits into two. Division, square roots,
factoring, and every splitting operation are branches. The inverse of merge.

The bootstrap (Axioms 8-10) is a merge: two uncalibrated extents merge
through a pin to create one unit. Decomposition (e.g., square root)
is the reverse: one value branches into two components at an anti-merge pin.

### Axiom 20 — Segments and Flow

A **segment** is a grade-1 composite: `(start, end)`. It has direction
(recessive → dominant), extent, and is traversable.

When segments meet at pins, their **flow directions** determine the
physical character of the encounter:

| Configuration | Flow | Physical character |
|---------------|------|--------------------|
| **Convergent** | → ← | Two dominants meet. Maximum tension. Boundary, wall, collision. |
| **Divergent** | ← → | Two recessives meet (shared origin). Branch, fork, source, choice point. |
| **Parallel** | → → | Co-directional. Co-traveling. The space between them is structure (offset, gap, channel). |
| **Anti-parallel** | → over ← | Same line, opposite direction. Loop or standing wave. Reflection or interference. |

### Axiom 21 — Loops

A **loop** is a segment whose dominant end is identified with a recessive
end earlier in the structure: "this end IS that beginning."

A loop creates:
- A **period**: the number of ticks around the loop
- **Repetition**: traversal naturally recurs
- **Enclosure**: a loop bounds a region. Area isn't in the 1D loop — it
  emerges from what the loop encloses (topological property)

### Axiom 22 — Boundary Conditions from Structure

When traversal reaches the end of an axis, it creates tension.
**The orthogonal structure absorbs or fails to absorb that tension.**
The boundary behavior is determined by what exists on the structural axes,
not by an external rule:

| Orthogonal structure | Boundary behavior |
|----------------------|-------------------|
| Repeat | **Wrap.** Tension restarts traversal at the beginning. |
| Mirror | **Reflect.** Tension reverses traversal direction. |
| Expand | **Extend.** The boundary moves outward to accommodate. |
| Nothing | **Clamp.** Tension stays at the boundary. Accumulates. |
| Unresolved (u = 0) | **Promote.** Tension cannot be absorbed. May become a new dimensional direction (Axiom 16). |

This is physical: the mover doesn't check a boundary flag. It creates
tension, and the surrounding structure either absorbs it or doesn't.
The behavior is readable from the structure itself.

### Axiom 23 — Dimensional Transition (Rivers and Lakes)

Structure can vary in dimensionality along traversal. A pin can connect
segments of different structural freedom:

- **Narrowing:** A 2D region (area, "lake") connects to a 1D path
  ("river") through an exit pin. Constraints tighten.
- **Widening:** A 1D path connects to a 2D region through an entry pin.
  Constraints relax.

At a widening pin, multiple exit paths become possible. The flow conditions
and available exit pins determine which path is taken. This models:
- Circuit components (wires → capacitor plates → wires)
- Decision spaces (constrained path → open choice → selected path)
- Impedance mismatch (when structural dimensions don't match at a pin,
  some flow reflects and some transmits)

---

## Part VI: Traversal and Time

### Axiom 24 — Traversal

A **mover** is an atomic element used as a cursor: `(currentTick, endTick)`.
The value is the current position. The unit is the route's extent.

Traversal is the act of advancing the mover from tick 0 to tick `endTick`.
Each step produces a new position — a new element that didn't exist at
the previous tick. Each step is a generative act.

### Axiom 25 — Time Is Traversal

Time is not external. It is what you get when you traverse any ordered
structure.

Every composite has a recessive ("before") and a dominant ("after").
Moving from recessive to dominant IS time passing. The structure IS
the clock:
- Resolution (how many ticks) = temporal granularity
- Ordering (recessive before dominant) = arrow of time
- Extent (recessive to dominant distance) = duration

No seconds, no hours — just positions within a directed structure.
But "before" and "after" are physically real. Causation follows from
ordering: what happens at tick N can depend on what happened at tick N-1,
but not on tick N+1.

### Axiom 26 — Operations as Pinned Events

Operations are **pins on the traversal route**. An operation attachment
specifies:
- A **site** (structural position along the route)
- A **law** (what happens when the mover reaches this position)
- The law produces an **outbound result** (generated value)

The traversal machine is a timeline: structural positions with generative
events that produce values as the mover passes through. The operations
fire in order of their site position — causally ordered by the structure.

---

## Part VII: Laws

### Axiom 27 — Laws Are Constrained by Unit Category

The unit category of each axis **constrains** which laws naturally apply.
Laws are not arbitrary — they are partly determined by the nature of what
the axes measure.

| Law | Character | Requires |
|-----|-----------|----------|
| **Expand** (multiply) | Continuous growth | Resolution throughout the range. Smooth. |
| **Accumulate** (add) | General combining | Works on continuous or discrete. No smoothness required. |
| **Partition** (divide/ratio) | Discrete splitting | Both endpoints known. Cannot partition without bounds. |
| **Boolean** (and/or/overlap) | Local co-presence | Only needs local observation. Works even with unknown endpoints. |
| **Repeat** | Iteration of another law | Discrete meta-law. Applies any law N times. |
| **Move** (translate) | Shift value | Changes position within a space. Other-moving. |
| **Reframe** (perspective shift) | Change the frame | Changes how values are read. Self-moving. |
| **Fold/Interpolate** (run) | Traverse and derive | Produces intermediate values. Requires ordering. |
| **Split/Merge** | Branch and join | Roots, averages, convergence, divergence. |
| **Associate** (tension) | Relate unlike forces | Preserves the relationship without resolving it. |

### Axiom 28 — Two-Level Law System

Laws operate at two levels:

**Implicit law:** Derived from the unit categories of the inbound axes.
What the axes "naturally want" to do. Two continuous-expand axes →
multiply produces area. One continuous-expand + one discrete-repeat →
multiply produces repeated addition. The law falls out from what's arriving.

**Explicit law:** Specified by the pin or operation. Can override the
implicit law. You CAN add areas. You CAN multiply discrete counts.
But when the explicit law conflicts with what the axes naturally support,
**tension arises**. The system produces the best result it can, plus
tension recording the conflict.

### Axiom 29 — The Kernel as Natural Law

At grade 2+, the four-term multiply kernel (Axiom 15) is the **natural
law** for combining two directed structures. The pin computes all four
terms; the dominance pattern determines interpretation:

- `dd` terms → area contribution (aligned)
- `rr` terms → anti-area contribution (aligned, opposite sign)
- `rd + dr` terms → directed extent contribution (orthogonal)
- Net result: `(rr - dd)` aligned + `(rd + dr)` orthogonal

The explicit law (add, multiply, etc.) is a further operation applied to
the kernel's output. The kernel is universal; the explicit law is chosen.

---

## Part VIII: Views and Frames

### Axiom 30 — A View Is Measurement Without Ownership

A **view** relates a subject to a frame:

    View(frame, subject) → read outcome

The frame is a composite. Its recessive child is the **calibration**
(the reference, the structural context). Its dominant child is the
**existing readout** (the current measurement).

Reading commits the subject to the frame's calibration. The result is
a new element: the subject as seen through the frame. This is generation —
the read outcome belongs to neither the frame nor the subject alone.

### Axiom 31 — Frame-Driven Dispatch

When reading through a composite frame, each atomic leaf in the frame
tree determines its own behavior via unit sign:

- **Positive leaf** → value calibration at that position
- **Negative leaf** → structural organization at that position
- **Zero leaf** → generative demand at that position

A grade-3 frame has 8 atomic leaves. Some may calibrate, some organize,
some demand generation. The view doesn't choose a mode — it reads through
the frame, and the structure determines what happens everywhere.

---

## Part IX: Collections

### Axiom 32 — Families

A **family** is an ordered collection of elements read in one frame:

- **Frame**: the structural context (organizes members)
- **Members**: the values (the data being organized)
- **Ordering**: whether member sequence matters

Family operations map to the trichotomy:
- Reading all members through the frame = projecting values through structure
- Sorting, focusing, reorganizing = structural operations
- Accumulation, boolean, fold = generative operations (produce new elements from existing ones)

**Focusing** shifts what counts as structure vs value: a member becomes
the new frame, and the remaining members are reorganized within it.
The same element changes role. Aspects are relational (Axiom 6).

---

## Part X: Completeness

### Theorem 1 — Construction Completeness

From Axioms 1-10, the entire number system is constructed:
- Extents → ratios → units → numbers
- No real numbers assumed. All arithmetic is exact integer operations on
  value/unit pairs.

### Theorem 2 — Dimensional Completeness

From Axioms 14-17, all dimensions emerge:
- Grade 1: directed segments (1D with direction)
- Grade 2: the four-term kernel separates into area (2D) and complex
  lines (1D with rotation)
- Grade 3+: each grade produces the next dimension through symmetry
  breaking of the previous grade's kernel
- No dimensions are assumed. Each is forced by unresolvable tension.

### Theorem 3 — Structural Completeness

From Axioms 18-23, all topology is constructed:
- Merge and branch are the only two topological events
- All structures are networks of segments connected at pins
- Loops, boundaries, and dimensional transitions follow from
  pin topology and tension absorption
- Equations are merge points where tension must reach zero
- Variables are segments with free denominators
- Recurrence relations are feedback loops in branching structure

### Theorem 4 — Temporal Completeness

From Axioms 24-26, time emerges:
- Every ordered structure contains "before" and "after"
- Traversal actualizes this potential into sequence
- Causation follows from ordering
- No external clock needed

### Theorem 5 — The Aspect Trichotomy Is Exhaustive

Every element in the system, in every context, plays exactly one of
three roles determined by its unit sign. Every operation is classifiable
as value-work, structural-work, or generative-work based on what it
preserves and what it produces. The multiplication table (Axiom 7)
governs all aspect transitions and is closed under composition.

---

## Summary: What Is Assumed vs What Is Constructed

**Assumed:**
- Ordered comparable extents exist (Axiom 1)

**Constructed (in order of emergence):**
1. Atomic elements with value/unit pairs (Axiom 2)
2. Composite structure via ordered pairing (Axiom 3)
3. Direction from ordering (Axiom 4)
4. Three aspects from unit sign (Axiom 5)
5. Ratios from pinning extents (Axiom 8)
6. Units from self-fold of ratios (Axiom 9)
7. Numbers from alignment to units (Axiom 10)
8. Directed segments from orthogonal tension (Axiom 14)
9. Area, complex numbers from the four-term kernel (Axioms 15, 17)
10. Higher dimensions from symmetry breaking (Axiom 16)
11. Topology from pins, merges, branches (Axioms 18-21)
12. Boundary behavior from structural absorption (Axiom 22)
13. Time from traversal of ordered structure (Axiom 25)
14. Laws constrained by unit category (Axiom 27)

Nothing is assumed except that ordered extents exist.
Everything else — numbers, dimensions, topology, time, area, complex
arithmetic, boundary conditions — is constructed through pinning,
folding, and the resolution or preservation of tension.
