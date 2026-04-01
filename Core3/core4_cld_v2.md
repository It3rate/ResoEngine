# Core4 Axiom System — Version 2

A physical mathematics bootstrapped from uncalibrated extents.
No real numbers, no stated axes, no assumed dimensions.
Everything constructed from ordered magnitudes through a universal lifecycle
of attachment, negotiation, commitment, and independence.

---

## Part I: Ground

### Axiom 1 — Extents and Chaos

There exist **extents**: ordered, comparable magnitudes. An extent has
no unit, no resolution, no sign. It is simply "some amount" that can be
compared to another extent by size. This is the sole assumption.

Not everything is an extent. Some phenomena are **chaotic** — not orderable,
not comparable. The system can only observe these; it cannot operate on them.
They appear as irreducible tension, background noise within the resolution
of whatever structure is present. One purpose of the system is to discover
frames that render previously chaotic phenomena orderable — but that is
never guaranteed, and some residual chaos is always acceptable.

We write an extent as a bare value: `a`, `b`, `c`.

### Axiom 2 — The Atomic Element

An **atomic element** is a pair `(v, u)` where:
- `v` is a signed integer: the **value** (how many)
- `u` is a signed integer: the **unit** (what "one" is)

The unit sign carries physical meaning (Axiom 10). The unit magnitude
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
(see Part VI). No external clock is needed.

---

## Part II: The Lifecycle

Every event in this system — from the first unit bootstrapping itself out
of uncalibrated extents, to dimensional emergence, to a pin firing during
traversal — follows a universal pattern. The pattern has four phases.
Each phase unlocks abilities that remain available in all subsequent phases.
The pattern can run forward (construction) or backward (decomposition).

Any event in the system is one of:
1. **A single phase** within a larger lifecycle
2. **A complete lifecycle** instance
3. **A composite event** built from multiple lifecycles

### Axiom 5 — The Four Phases

**Phase 1 — Attach.** A connection forms between previously independent
things. This has two sub-steps:

> **Pin:** An extent connects to another extent at an endpoint. This gives
> the connector a **location** (here), a **direction** (one way to go),
> and the **potential to interpolate** along that direction. Endpoint
> attachment is the primitive — midpoints cannot be discovered without
> calibration, so midpoint attachment requires having already exercised
> the interpolation potential from an endpoint.
>
> What the potential to interpolate means: moving along the attached extent
> produces time (ordered sequence of positions). Moving past the end
> produces tension (the structure has been exceeded).
>
> **Join:** The connector attaches to a second extent without releasing
> the first. Now there are **two direction opinions** from a single
> location. These opinions are either aligned (same direction) or opposed
> (opposite directions). This is comparison — not yet measured, but
> structurally present as a ratio.

**Phase 2 — Negotiate.** The ability to compare implies the potential for
change. Laws emerge from abilities already present at this stage — they
are not imported from outside the system:

- **Track:** Use one extent to measure changes in the other
- **Repeat:** Re-attach to the start when you reach the end
- **Stretch:** Adjust the rate of interpolation (change the ratio)
- **Negate:** Reverse direction
- **Halt:** Recognize when combined potential is exceeded; create tension

Every ability has two modes: **recognition** (you can see direction) and
**action** (you can change direction). You can recognize extent or modify
extent, attach to things to observe them or attach to things to accumulate
them. The negotiation determines which abilities are exercised and how the
two extents will relate. This is where the structure of the interaction
is worked out.

The negotiation has limits. When combined potential is exceeded — for
example, one axis repeats 3 times and the other stretches to twice its
length, giving a combined reach of 6; beyond that, the shared system
produces tension it cannot absorb — the negotiation records this as
structural tension, not failure.

**Phase 3 — Commit.** The negotiated structure is locked. Specifically,
the **denominator** is locked — the unit, the "what is one," the tick
structure. The values that participated in negotiation become measurements
*within* the new unit, not parts of the unit itself.

This is always a denominator lock, even at higher grades. A vertical 3
and a horizontal 4 combine into an area of 12, but the unit created is
1² (one-squared), not 12. The 3 and 4 become values measured by that
unit. The unit commits to not changing size even if the values that
created it later adjust themselves. Units are local — they are bound
to the place and structure where they were committed.

**Phase 4 — Independence.** The committed unit separates from the values
that created it. It becomes a **symbol**: a portable, self-sufficient
entity that can calibrate other things. The unit transcends its
construction — area is more than two segments, speed is more than
distance and time. The values remain as a remainder or reattach
elsewhere; the unit is free.

This is the mechanism by which the system creates abstractions. A symbol
carries the memory of its formation (direction, resolution, structure)
but has no ongoing dependency on the specific values that formed it.
A different value can attach to the unit and be measured by it. The symbol
can travel to other parts of the structure and serve as a reference there.

### Axiom 6 — Abilities Accumulate

Abilities unlocked at any phase remain available in all subsequent phases
and in all subsequent lifecycle instances at higher grades. Nothing is lost.

After **Pin**: location, direction, interpolation potential, perspective
(the world seen from this position).

After **Join**: comparison, two direction opinions, ratio, the distinction
between aligned and opposed. The earlier abilities (location, direction,
interpolation) are now exercisable in a richer environment — you had the
concept of interpolation before, but now you have two extents to
interpolate between, which enables uses that a single attachment could not.

After **Negotiate**: change, laws (track, repeat, stretch, negate, halt),
tension boundaries, rate. All earlier abilities (location, comparison,
interpolation) are available as inputs to negotiation.

After **Commit**: unit, measurement, denominator stability. Comparison
now becomes measurable comparison (numbers, not just ratios). Interpolation
now has resolution (ticks).

After **Independence**: symbol, portability, calibration of others.
Everything from prior phases is available *and* the symbol carries a
compressed version of all of it.

### Axiom 7 — The Reverse Lifecycle

The lifecycle runs backward for decomposition:

**Capture:** A free-running symbol is bound — attached to a specific
context. (Reverse of Independence.)

**Cleave:** The committed structure is broken — the denominator lock
is released, exposing the internal negotiation. (Reverse of Commit.)

**Separate:** Values are distinguished from units by sorting — the
negotiated relationships are unwound into their component opinions.
(Reverse of Negotiate.)

**Disconnect:** The pin releases. Two previously joined extents become
independent again. (Reverse of Attach.)

In mathematics: taking a square root is capturing an area symbol, cleaving
its unit, separating the axis values, and disconnecting them into
independent extents. Factoring is the same pattern applied to multiplication.
Analysis — in the broad sense — is the reverse lifecycle.

### Axiom 8 — Aligned and Opposed Directions

When two extents join at a point, their directions from that point are
either **aligned** (same direction) or **opposed** (opposite directions).
This distinction is structural — it exists before any measurement.

**Aligned directions** create parallel flow. Two streams going the same
way. They cannot create a new dimension (there is no tension between them
that demands one), but they create something a single stream cannot:
parallel force, co-travel, the concept of width between two co-directional
flows. This is the seed of structure within a dimension.

**Opposed directions** create orthogonal potential. From the join point,
there is a *third* interpolation path that didn't exist in either extent
alone: through the join point from one extent's endpoint to the other's.
This path crosses both extents and requires acknowledging both directions
simultaneously. This is why orthogonal forces create new dimensions —
the tension between opposed directions opens a path that neither extent
contains by itself.

---

## Part III: The Three Aspects

### Axiom 9 — Unit Sign Determines Aspect

The sign of an atomic element's unit determines its **aspect** — its
physical role in any encounter:

| Unit sign | Aspect | Meaning |
|-----------|--------|---------|
| `u > 0` (positive) | **Value** | Aligned. Shares a carrier with its context. A calibrated measurement in a known space. "One" is established. |
| `u < 0` (negative) | **Structure** | Orthogonal. A force acting on the context without being the context. Organizes, constrains, imposes a dimension. Does not contribute magnitude — contributes arrangement. |
| `u = 0` | **Generative** | Unresolved. The relationship between this element and its context hasn't settled. Has magnitude but no "one" to measure against. Between unit spaces. Demands resolution through a law. |

These are not labels applied externally. They are readable from the element
itself. You inspect the unit sign and the aspect is physically present.

### Axiom 10 — Aspects Are Relational

An element's aspect is not intrinsic — it depends on the frame through which
it is read. The same element can be value in one context, structure in another,
and generative potential in a third. What determines its role is the
relationship, not the element alone.

A composite frame has leaves with potentially mixed signs. Each leaf
contributes its own aspect independently. A single frame can simultaneously
calibrate values (positive leaves), organize structure (negative leaves),
and demand generation (zero leaves) at different positions in its tree.

### Axiom 11 — The Multiplication Table of Aspects

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

## Part IV: Bootstrap — The First Lifecycle Instance

The number system is the first complete run of the lifecycle.

### Axiom 12 — Ratios from Extents (Attach Phase)

Take two uncalibrated extents and **join** them in a composite:

    Composite(a, b)

This is the Attach phase: two extents are pinned together. The result is
a **ratio**: "a relative to b." It is meaningful even though neither `a`
nor `b` has a unit. No measurement has been made. A relationship has been
created. This is the first structural act.

The ratio does not have resolution — neither child knows what "one" is.
But the relationship `a/b` is real and comparable to other ratios.

At this point the abilities from Attach are live: location (within the
ratio), direction (recessive → dominant), the potential to interpolate
between the two extents, and comparison (their relative sizes).

### Axiom 13 — Unit Formation (Negotiate + Commit Phases)

The ratio can **fold**: the Negotiate and Commit phases run together.

    Fold(Composite(denominator, numerator)) → AtomicElement(v, u)

The negotiation proceeds through four steps:

**Step 1 — Attachment gives direction.** The unit-to-be connects to the
value at one endpoint. Before this, the unit-to-be is directionless — just
an extent with magnitude. The attachment point determines the sign
relationship. If the unit-to-be and value face the same direction from the
attachment point, the unit is **aligned** (positive). If they face opposite
directions, the unit is **opposed** (negative). There are two physically
distinct configurations; the other two apparent configurations are the same
situations viewed from the other side.

**Step 2 — Negotiation of "one."** The value provides calibration: *where*
the attachment point sits, and its own natural direction. The unit-to-be
provides resolution: what "one" will mean, the tick structure. Together
they negotiate a shared version of 1. The laws used in this negotiation
are the abilities already available from Attach: comparison, ratio,
direction.

**Step 3 — Commitment.** The unit-to-be commits to **not changing size**.
It becomes the fixed denominator. This is irreversible — the unit now
carries direction (from Step 1), magnitude (from its original extent),
and the agreement about what "one" means (from Step 2).

**Step 4 — Independence.** The unit separates. It retains the information
it received from the value (direction, attachment point, resolution), but
the value itself could detach. A different value could join. The unit has
become a measuring stick that remembers its formation but has no ongoing
dependency on the specific value that formed it.

The fold operation `ComposeRatio` performs this entire sequence:
- The value `v` encodes the numerator's magnitude scaled by the denominator's resolution
- The unit `u` encodes the resolution, derived from the denominator's magnitude

The atomic case is special: the full value does not survive into the final
unit. It donates calibration information (position, direction) and some of
that lives on in the unit nondestructively, but the value field is then
free for other use. All units — at every grade — form through these same
four steps.

### Axiom 14 — Numbers from Alignment (Using the Independent Unit)

Once a unit exists and is independent, other elements can be **aligned**
to it:

    element.CommitToCalibration(unit) → calibrated element

Alignment reexpresses an element in terms of the unit's resolution.
The element becomes a **number**: a value expressed in terms of the
self-declared "one." Numbers are born from units, not the reverse.

This is the independent unit doing what independent units do: traveling
to other parts of the structure and calibrating what it finds there.
The unit is a symbol; alignment is the act of applying that symbol.

The complete bootstrap lifecycle:
1. **Attach:** Pin two extents → a ratio (Axiom 12)
2. **Negotiate:** Compare, establish direction and shared "one" (Axiom 13, Steps 1-2)
3. **Commit:** Lock the denominator (Axiom 13, Step 3)
4. **Independence:** Unit separates, calibrates others → numbers (Axiom 13 Step 4, Axiom 14)

From "ordered magnitudes exist" alone, the entire number system is constructed.

---

## Part V: Dimensional Emergence — The Second Lifecycle Instance

Dimensions are the second lifecycle running at grade 1 and above.
The pattern is the same: attach, negotiate, commit, independence.
But the objects are richer (directed segments, not bare extents),
so the negotiation produces richer structures (area, complex numbers,
higher dimensions).

### Axiom 15 — The Denominator Is "What Is One"

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

The transition from co-location to fusion IS a lifecycle run: the two
units attach (co-locate), negotiate (compare their "ones"), commit
(lock a shared denominator), and the fused entity becomes independent
(area as a concept, not just width-times-height).

### Axiom 16 — The 1×1 Requirement and Degrees of Freedom

For dimensional fusion to produce a coherent space, the unit cell must be
**1×1**: each axis contributes exactly one of its own measure.

**Why:** Predictable scaling under interchangeable degrees of freedom.

Every fused entity has exactly **one degree of freedom** — it is one thing.
Speed is one thing. Area is one thing. But the *tools* used to construct
fused entities may expose more than one adjustable axis (a rectangle has
width and height). The 1×1 requirement governs what those tools may do.

**When the fused axes are interchangeable** (both inches in a square-inch
unit cell): the construction tool gets **multiple degrees of freedom**.
You can scale width or height, and the area changes predictably without
needing to know which axis changed. The axes are symmetric with respect
to scaling — "I tripled one side" fully determines the new area. This
extra freedom in the tool is only possible because the axes are
indistinguishable within the fused unit.

**When the fused axes are not interchangeable** (miles and hours in a
speed unit): the fusion is equally real — speed is a genuine entity, not
two measurements sitting side by side. But the fusion itself **imposes a
restriction**: values may only attach freely to one axis. The tool gets
exactly **one degree of freedom**. You cannot drag the hours around
independently without destroying the concept of speed. The denominator
axis (hours) is locked to one, and only the numerator axis (miles) is
free. This restriction is not external — it is the cost of fusing
non-interchangeable components into a single entity.

**The non-fused alternative** is co-location without fusion: "I drove 80
miles in 2 hours." You can compare such ratios — 20 miles in half an hour
gives an equivalent ratio — but you do not have a *concept* of speed.
There is no fused entity, no single degree of freedom, no unit that
persists independently of the specific values that created it. You are
stuck describing instances. Fusion is what turns a ratio into a thing.

**The 1×1 cell** is the mechanism that makes all of this work. Each axis
contributes exactly "one of itself." When the axes are interchangeable,
this makes them symmetric in the tool. When they are not, this is what
forces one axis to lock (it contributes its one, and that one is fixed).
In both cases, the fused entity has predictable behavior: adjusting any
*available* degree of freedom produces a result you can compute without
additional information about which axis moved.

**Consequence:** Denominators must be settled before fusion can happen.
If either axis has a free denominator (u = 0), you cannot know what "1"
means for that axis, and therefore cannot construct the 1×1 cell.

    Free denominators = structure (not ready for fusion)
    Fixed denominators = ready for fusion
    Combined denominator = fused entity

In the engine: `Scale` multiplies `Unit × factor.Unit`. The product is the
resolution of the fused space. Each input contributes its full resolution
as its "one." The 1×1 cell emerges from ordinary arithmetic.

### Axiom 17 — Tension as Incomplete Fusion

When two elements attempt to fuse (through fold or multiply) and cannot
agree on a shared "one," the result carries **tension**:

    EngineElementOutcome { Result, Tension, Note }

- `Result`: the best lawful projection (what the system can produce)
- `Tension`: the unresolved structure (what couldn't settle)
- When `Tension` is null, the outcome is **exact** (fusion complete)

Tension is not failure. It is a lifecycle that hasn't completed — the
Negotiate phase produced structure that the Commit phase cannot lock.
The tension records specifically what is unresolved and why. It preserves
the partially-completed generation for later resolution.

Tension is never discarded. It propagates through operations via
`CombineTension`. The unresolved relationship remains inspectable and
available for later resolution.

### Axiom 18 — Directed Segments (Grade 1 Unit Cell)

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
from a privileged origin. The distinction matters (see Axiom 21).

### Axiom 19 — The Four-Term Kernel (Grade 2)

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

    squareDifference = rr + dd     → net area (aligned result)
    crossSum         = rd + dr     → net directed extent (orthogonal result)

**Both equations are addition.** The traditional formulation writes
`rr - dd` because conventional mathematics only acknowledges positive
area — it needs an explicit subtraction to account for cancellation.
In this system, `rr` IS anti-area: structurally negative, the opposite
of `dd`. Adding anti-area to area cancels naturally, the same way
adding a negative number to a positive one does. The subtraction is
in the structure of `rr` itself, not in the equation. The operation
is always addition; the signs are carried by the terms.

This IS complex multiplication. It was not introduced — it **falls out**
of the grade system. The "real part" (rr + dd) is the net area contribution.
The "imaginary part" (rd + dr) is the net directional residue.

### Axiom 20 — Dimensional Promotion Through Symmetry Breaking

New dimensions are not stated. They **emerge** from tension that cannot
be expressed at the current dimensionality. The recipe is recursive and
each step is a lifecycle instance:

**0D → 1D:** Two uncalibrated extents **attach** (join). Their directions
are opposed — they create tension when pinned. The **negotiation** resolves
to a directed extent. The **commitment** locks the directed segment as a
unit cell. The segment is **independent** — direction emerges from the
incompatibility between the two extents.

**1D → 2D:** Two directed segments **attach** (multiply). The four-term
kernel is the **negotiation** — it produces terms that cannot all be
expressed as 1D lines:
- `dd` and `rr` terms are area-like — they represent enclosed space
  that has no 1D expression. The excess IS area.
- Area emerges specifically when both components are co-directional in
  dominance (both expanding or both contracting)
- `rd` and `dr` terms cancel to lines (the complex number part)
The **commitment** locks the area unit. The area is **independent** — a
fused entity that transcends the segments that created it.

**2D → 3D:** Area has a natural flow (start size → end size). It **attaches**
to a line that **breaks the 2D symmetry**: the line's dominance pattern is
incompatible with the area's natural expansion/contraction. The
**negotiation** forces volume — space swept by an area along a
symmetry-breaking line. The **commitment** locks the volume unit. Volume
is **independent**.

**General rule:** Dimension N+1 = Dimension N entity × symmetry-breaking
orthogonal extent. "Symmetry-breaking" means the new extent has a
different dominance pattern than the existing entity, forcing a genuinely
new dimensional direction.

**The search space collapses.** A machine navigating this system does not
explore arbitrary stated axes. It follows the forced path of tension
resolution. The only new dimensions that appear are those demanded by
unresolvable tension at the current grade. Dimensions are consequences,
not assumptions.

### Axiom 21 — Corner Resolution and Complex Numbers

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

## Part VI: Structure — Lifecycle Composition

Structural topology is what happens when multiple lifecycles compose.
A pin is a lifecycle site. A segment is the span between lifecycle sites.
A network of pins and segments is a network of lifecycle instances
interleaving and feeding into each other.

### Axiom 22 — The Pin

Every structural transition happens at a **pin**. A pin is a lifecycle
site: the place where attachment, negotiation, commitment, or independence
occurs.

A pin on a **host** (a composite span) splits the host at a position:

    Pin(host = Composite(start, end), position)
    → Inbound  = position - start
    → Outbound = end - position

The pin position is a value. The host is structure. The inbound and outbound
are **generated**: new elements that didn't exist in the inputs. The pin is
the site of generation.

A pin at the intermediate (zero-denominator) state is in the Negotiate
phase — the old units have dissolved and the new unit hasn't committed yet.
This is where the law acts.

### Axiom 23 — Merge and Branch

There are exactly two topological events. Everything else is traversal
along a segment between pins.

**Merge:** Two inbound segments → pin → one outbound. This is a
**forward lifecycle**: two things attach, negotiate, commit, and produce
one independent result. Multiplication, addition, fold, and every
combining operation are merges. The zero-denominator intermediate state
at the pin is the Negotiate phase where the old units dissolve and the
new unit forms.

**Branch:** One inbound segment → pin → two outbound segments. This is
the **reverse lifecycle**: one thing is captured, cleaved, separated,
and disconnected into two. Division, square roots, factoring, and every
splitting operation are branches.

The bootstrap (Axioms 12-14) is a merge: two uncalibrated extents merge
through a pin to create one unit. Decomposition (e.g., square root)
is the reverse: one value branches into two components at an anti-merge pin.

### Axiom 24 — Segments and Flow

A **segment** is a grade-1 composite: `(start, end)`. It has direction
(recessive → dominant), extent, and is traversable. A segment is the
span between two lifecycle sites (pins) — the interval where no event
is happening, only flow.

When segments meet at pins, their **flow directions** determine the
physical character of the encounter:

| Configuration | Flow | Physical character |
|---------------|------|--------------------|
| **Convergent** | → ← | Two dominants meet. Maximum tension. Boundary, wall, collision. |
| **Divergent** | ← → | Two recessives meet (shared origin). Branch, fork, source, choice point. |
| **Parallel** | → → | Co-directional. Co-traveling. The space between them is structure (offset, gap, channel). |
| **Anti-parallel** | → over ← | Same line, opposite direction. Loop or standing wave. Reflection or interference. |

These are the same aligned/opposed distinction from Axiom 8, now
manifested at the structural level between segments rather than between
bare extents.

### Axiom 25 — Loops

A **loop** is a segment whose dominant end is identified with a recessive
end earlier in the structure: "this end IS that beginning."

A loop creates:
- A **period**: the number of ticks around the loop
- **Repetition**: traversal naturally recurs (the Repeat ability from
  Negotiate, now structurally embodied)
- **Enclosure**: a loop bounds a region. Area isn't in the 1D loop — it
  emerges from what the loop encloses (topological property)

A loop is a lifecycle that feeds its Independence phase back into its
Attach phase. The symbol produced by one cycle becomes the extent
attached to the next. This is self-sustaining structure.

### Axiom 26 — Boundary Conditions from Structure

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
| Unresolved (u = 0) | **Promote.** Tension cannot be absorbed. May become a new dimensional direction (Axiom 20). |

This is physical: the mover doesn't check a boundary flag. It creates
tension, and the surrounding structure either absorbs it or doesn't.
The behavior is readable from the structure itself.

Each boundary condition is a specific resolution of a lifecycle at the
edge: the tension **attaches** to the orthogonal structure, **negotiates**
using whatever law that structure carries, and either **commits** to a
boundary behavior (wrap, reflect, extend, clamp) or fails to commit
(**promote** — a new lifecycle begins at a higher dimension).

### Axiom 27 — Dimensional Transition (Rivers and Lakes)

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

## Part VII: Traversal and Time

### Axiom 28 — Traversal

A **mover** is an atomic element used as a cursor: `(currentTick, endTick)`.
The value is the current position. The unit is the route's extent.

Traversal is the act of advancing the mover from tick 0 to tick `endTick`.
Each step produces a new position — a new element that didn't exist at
the previous tick. Each step is a generative act.

Traversal is the interpolation potential (from the Attach phase) being
*exercised*. What was potential becomes actual, one tick at a time.

### Axiom 29 — Time Is Traversal

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

### Axiom 30 — Operations as Pinned Events

Operations are **pins on the traversal route**. An operation attachment
specifies:
- A **site** (structural position along the route)
- A **law** (what happens when the mover reaches this position)
- The law produces an **outbound result** (generated value)

The traversal machine is a timeline: structural positions with lifecycle
events that produce values as the mover passes through. The operations
fire in order of their site position — causally ordered by the structure.

Each operation is a complete lifecycle: the mover **attaches** to the
operation site, the law **negotiates** between the mover's state and the
operation's structure, the result **commits** to a value, and that value
is **independent** — available downstream as input to later operations.

---

## Part VIII: Laws

### Axiom 31 — Laws Are Abilities from the Negotiate Phase

Laws are not external rules applied to the system. They are the abilities
that emerge during the Negotiate phase (Axiom 5), formalized and named.
Every law derives from capabilities already present when two extents
are joined:

| Law | Derived from | Character |
|-----|-------------|-----------|
| **Expand** (multiply) | Stretch ability | Continuous growth. Adjust the rate/ratio of extension. |
| **Accumulate** (add) | Attach ability | General combining. Attach more of the same. |
| **Partition** (divide/ratio) | Comparison | Discrete splitting. Uses ratio to divide. |
| **Boolean** (and/or/overlap) | Location + comparison | Co-presence at a position. |
| **Repeat** | Re-attach ability | Iteration. Return to start and run again. |
| **Move** (translate) | Direction change | Shift position. Other-moving. |
| **Reframe** (perspective) | Perspective from location | Change how values are read. Self-moving. |
| **Fold/Interpolate** (run) | Interpolation potential | Traverse and derive intermediate values. |
| **Split/Merge** | Attach/disconnect | Branch and join. Roots, averages, convergence. |
| **Associate** (tension) | Halt/tension | Relate unlike forces. Preserve without resolving. |

More complex laws are compositions of these primitives, possibly sequenced
across multiple lifecycle instances. But the primitive laws all trace back
to abilities that exist at the Negotiate phase.

### Axiom 32 — Two-Level Law System

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

### Axiom 33 — The Kernel as Natural Law

At grade 2+, the four-term multiply kernel (Axiom 19) is the **natural
law** for combining two directed structures. The pin computes all four
terms; the dominance pattern determines interpretation:

- `dd` terms → area contribution (aligned)
- `rr` terms → anti-area contribution (aligned, opposite sign)
- `rd + dr` terms → directed extent contribution (orthogonal)
- Net result: `(rr + dd)` aligned + `(rd + dr)` orthogonal

The explicit law (add, multiply, etc.) is a further operation applied to
the kernel's output. The kernel is universal; the explicit law is chosen.

---

## Part IX: Views, Frames, and Collections

### Axiom 34 — A View Is Measurement Without Ownership

A **view** relates a subject to a frame:

    View(frame, subject) → read outcome

The frame is a composite. Its recessive child is the **calibration**
(the reference, the structural context). Its dominant child is the
**existing readout** (the current measurement).

Reading commits the subject to the frame's calibration. The result is
a new element: the subject as seen through the frame. This is generation —
the read outcome belongs to neither the frame nor the subject alone.

A view is a lifecycle instance: the subject **attaches** to the frame,
the calibration **negotiates** how the subject should be expressed, the
result **commits** to a measured value, and that value is **independent**
of both the original subject and the frame.

### Axiom 35 — Frame-Driven Dispatch

When reading through a composite frame, each atomic leaf in the frame
tree determines its own behavior via unit sign:

- **Positive leaf** → value calibration at that position
- **Negative leaf** → structural organization at that position
- **Zero leaf** → generative demand at that position

A grade-3 frame has 8 atomic leaves. Some may calibrate, some organize,
some demand generation. The view doesn't choose a mode — it reads through
the frame, and the structure determines what happens everywhere.

### Axiom 36 — Families

A **family** is an ordered collection of elements read in one frame:

- **Frame**: the structural context (organizes members)
- **Members**: the values (the data being organized)
- **Ordering**: whether member sequence matters

Family operations map to the trichotomy:
- Reading all members through the frame = projecting values through structure
- Sorting, focusing, reorganizing = structural operations
- Accumulation, boolean, fold = generative operations (produce new elements
  from existing ones)

**Focusing** shifts what counts as structure vs value: a member becomes
the new frame, and the remaining members are reorganized within it.
The same element changes role. Aspects are relational (Axiom 10).

---

## Part X: Completeness

### Theorem 1 — Construction Completeness

From the bootstrap lifecycle (Axioms 12-14), the entire number system
is constructed:
- Extents → ratios → units → numbers
- No real numbers assumed. All arithmetic is exact integer operations on
  value/unit pairs.

### Theorem 2 — Dimensional Completeness

From the dimensional lifecycle (Axioms 18-21), all dimensions emerge:
- Grade 1: directed segments (1D with direction)
- Grade 2: the four-term kernel separates into area (2D) and complex
  lines (1D with rotation)
- Grade 3+: each grade produces the next dimension through symmetry
  breaking of the previous grade's kernel
- No dimensions are assumed. Each is forced by unresolvable tension.

### Theorem 3 — Structural Completeness

From lifecycle composition (Axioms 22-27), all topology is constructed:
- Merge and branch are the only two topological events (forward and
  reverse lifecycle)
- All structures are networks of segments connected at pins (lifecycle
  sites connected by flow)
- Loops, boundaries, and dimensional transitions follow from
  pin topology and tension absorption
- Equations are merge points where tension must reach zero
- Variables are segments with free denominators (incomplete Negotiate phase)
- Recurrence relations are loops (Independence feeding back to Attach)

### Theorem 4 — Temporal Completeness

From traversal (Axioms 28-30), time emerges:
- Every ordered structure contains "before" and "after"
- Traversal actualizes interpolation potential into sequence
- Causation follows from ordering
- No external clock needed
- Operations fire as lifecycle instances at pinned positions

### Theorem 5 — The Aspect Trichotomy Is Exhaustive

Every element in the system, in every context, plays exactly one of
three roles determined by its unit sign. Every operation is classifiable
as value-work, structural-work, or generative-work based on what it
preserves and what it produces. The multiplication table (Axiom 11)
governs all aspect transitions and is closed under composition.

### Theorem 6 — Lifecycle Universality

Every event in the system is an instance of the four-phase lifecycle
(Axiom 5) or a composition of such instances:
- The bootstrap is a lifecycle (Axioms 12-14)
- Dimensional emergence is a lifecycle per dimension (Axiom 20)
- Every merge is a forward lifecycle, every branch a reverse (Axiom 23)
- Every operation is a lifecycle at a pin (Axiom 30)
- Every view/measurement is a lifecycle (Axiom 34)
- Every boundary resolution is a lifecycle (Axiom 26)
- Loops are self-feeding lifecycles (Axiom 25)

The lifecycle is the universal pattern because its phases correspond
to the only things that *can* happen between extents: connection,
comparison, commitment, and separation. These exhaust the structural
possibilities.

---

## Summary: What Is Assumed vs What Is Constructed

**Assumed:**
- Ordered comparable extents exist (Axiom 1)

**Constructed (in order of emergence):**
1. Atomic elements with value/unit pairs (Axiom 2)
2. Composite structure via ordered pairing (Axiom 3)
3. Direction from ordering (Axiom 4)
4. The four-phase lifecycle from the possibilities between extents (Axiom 5)
5. Three aspects from unit sign (Axiom 9)
6. Ratios from pinning extents (Axiom 12)
7. Units from the negotiate/commit lifecycle (Axiom 13)
8. Numbers from alignment to independent units (Axiom 14)
9. Directed segments from orthogonal tension (Axiom 18)
10. Area, complex numbers from the four-term kernel (Axioms 19, 21)
11. Higher dimensions from symmetry breaking (Axiom 20)
12. Topology from pins, merges, branches (Axioms 22-25)
13. Boundary behavior from structural absorption (Axiom 26)
14. Time from traversal of ordered structure (Axiom 29)
15. Laws from Negotiate-phase abilities (Axiom 31)

Nothing is assumed except that ordered extents exist (and that not
everything is ordered). Everything else — numbers, dimensions, topology,
time, area, complex arithmetic, boundary conditions, laws, symbols,
and the lifecycle itself — is constructed through attachment, negotiation,
commitment, and independence.
