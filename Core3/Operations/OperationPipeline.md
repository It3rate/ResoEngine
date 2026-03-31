# Core3 Operation Pipeline

This note records the current intended unification path for operations in
`Core3`.

It is a design note, not yet a claim that the current code already fully
matches it.

The central idea is:

- simple arithmetic
- boolean partitioning
- traversal-time site laws
- branching and reverse continuation
- larger equations

should all eventually be understood as readings of one shared operation
pipeline rather than as unrelated execution systems.

## Operation As Directed Arc

Every operation should also be readable as a small directed arc:

- inbound carrier
- origin law
- outbound carrier

In the current simple cases:

- inbound means the input frame, members, grades, supports, and resolutions
- origin means the local law being enacted
- outbound means the resulting element, piece family, or updated runtime state

This is deliberately close to the directed-segment metaphor already used
elsewhere in Core3:

- inbound traversal prepares the law by consolidating grades, calibration,
  correspondence, and local carrier relation
- origin is the local site of enactment
- outbound traversal carries the surviving results onward

This scales upward.
A binary add is a very small arc.
A partitioning boolean read is a slightly richer arc with several outbound
pieces.
A larger equation is a route of such arcs with branching, merging, looping,
and repeated continuation.

So a primitive operation is not a different kind of thing from a larger
equation.
It is a small, optimized instance of the same inbound-origin-outbound pattern.

## Preserve First, Then Read

One useful current pressure in Core3 is that an operation may appear to have
several different "results":

- a consolidated result in the original grade
- a richer kernel such as `rr/dd/rd/dr`
- a co-present survivor family
- a relation held in tension because the current grade cannot settle it cleanly

The intended long-term reading is not that Core3 should accumulate many
specialized result species for each of those cases.

The intended reading is closer to:

- preserve the lawful structure first
- then read that preserved structure in different frames, grades, or family
  views

So if a multiply currently exposes a raw kernel and a folded result, that
should increasingly be understood as:

- one preserved structure
- several lawful reads of that structure

Likewise, mixed-carrier add/subtract, boolean piece families, and later route
or branch results should prefer:

- preserved structure plus provenance
- later view/cast/reframe reads

over one-off bespoke inspection helpers.

There is one important constraint:

- a later read can only reveal what was actually preserved

If the richer relation was discarded earlier, no later frame view can
reconstruct it honestly.

So Core3 should distinguish two questions:

1. What structure was preserved?
2. How is that preserved structure being read right now?

Current helpers such as raw-pair or raw-kernel inspection are therefore best
understood as temporary compatibility shells until the preserved structures are
more directly referenceable through ordinary Core3 frame/family reads.

## Family As Current Data Area

For now, `Family` should be treated pragmatically as the current data
area.

It holds:

- a frame role
- a collection of members
- an ordered or unordered hint
- temporary focus and parent provenance

Sometimes the frame is rich calibration.
Sometimes it may be little more than a neutral or zeroed holder.

That is acceptable for now.

This should also stay compatible with the higher-layer data work:

- the same family may later be read as a bag
- or an array-like span
- or a graph-like local neighborhood
- or a route slice
- or another domain-friendly collection view

The point is not to create several different ontologies for those.
The point is to keep one holder surface and let lawful views / framings
interpret it differently.

The immediate requirement is not that family already explains the whole runtime
ontology.
The requirement is that it be capable of:

- holding data
- being read in natural order
- being reordered or focused when needed
- accepting laws over the whole current collection

So the present recommendation is:

- treat family as the current operation-time data area
- do not over-define it yet
- make sure later law unification can still run through it

That means the upcoming law cleanup should keep the data-layer perspective in
mind: `Family` should remain a stable holder / data-area surface while laws and
views become cleaner about how they interpret what is being held.

## Unit Sign As First Read Posture

The experimental data layer is also a useful reminder that unit sign may guide
the first carrier interpretation of a read:

- positive unit -> aligned / calibrative / value-facing
- negative unit -> orthogonal / organizational / structural-facing
- zero unit -> unresolved / generative demand

That is useful, but it is not yet the whole law by itself.
Actual law choice still depends on things such as:

- current grade shape
- host / frame relation
- continuation mode
- retention mode
- reduction mode
- explicit local law

So unit sign should help classify the read, not replace the rest of the
operation pipeline.

## Generic Operation Pipeline

The shared operation pattern should be:

1. Source domain
   What current data area is active?
   Examples:
   - one element
   - one frame plus family
   - one route plus current mover
   - one branch frontier

2. Correspondence topology
   How are local correspondences generated?
   Examples:
   - self read
   - left to right
   - one-to-one by order
   - focused member to each other member
   - all-to-all
   - route partition to occupancy set
   - current site to attached inputs

3. Local law
   What happens at one correspondence?
   Examples:
   - add
   - subtract
   - multiply
   - compare
   - boolean truth test
   - calibration
   - continuation update

4. Retention mode
   What survives from the local law?
   Examples:
   - keep nothing
   - keep one result
   - keep all surviving pieces
   - keep alternatives as a branch family
   - keep unresolved structure as tension

5. Carrier inheritance
   What carries the surviving result?
   Examples:
   - host frame
   - left operand
   - right operand
   - focused member
   - partition carrier
   - newly created carrier

6. Reduction mode
   What happens after survivors are collected?
   Examples:
   - no reduction
   - accumulate all
   - fold one grade lower
   - lower to minimal active structure
   - keep raw kernel
   - lift to a richer grade

7. Continuation mode
   Is this a one-shot read, or part of a temporal route?
   Examples:
   - one-shot operation
   - repeated traversal step
   - branch frontier update
   - looped equation over time

This means an operation is not fundamentally "add" or "boolean."
An operation is:

- a topology
- a local law
- a retention rule
- a carrier rule
- an optional reduction rule
- an optional continuation rule

These are current descriptive slots, not a commitment to a permanent enum
surface.
Where possible, Core3 should keep pushing these distinctions downward into
native structure so that topology, retention, reduction, and continuation can
eventually be explored by changing lawful values, tensions, carriers, branch
families, or pinned structure rather than by inspecting code-only switches.

## Current Operations In This Lens

### Fold

`Fold` is:

- source domain: one element
- correspondence topology: self
- local law: canonical read of current structure
- retention mode: one result
- carrier inheritance: current element interpretation
- reduction mode: usually lower one grade

### Lift

`Lift` is:

- source domain: two peer elements
- correspondence topology: pair
- local law: preserve both as one higher-grade basis
- retention mode: one result
- carrier inheritance: new higher basis
- reduction mode: none

### Lower

`Lower` is:

- source domain: one higher-grade element
- correspondence topology: inspect active substructure
- local law: preserve minimum active carrier family
- retention mode: one result
- reduction mode: lower toward minimum required grade

### Add / Subtract

Current add/subtract are:

- source domain: two peers, or one family reduced in order
- correspondence topology: one-to-one
- local law: aligned combination
- retention mode: currently one consolidated result plus possible tension
- carrier inheritance: aligned result carrier
- reduction mode: none

Long term, add/subtract should also be explainable as the optimized reduction of
a more explicit traversal or correspondence process.
They should not be treated as fundamentally one-result-only laws.
Mixed-carrier or partially incompatible addition may still want to preserve:

- one unresolved lower-grade survivor
- several co-present outbound survivors
- a possible higher-grade lift
- tension about the relation between those readings

### Multiply

Current multiply is more mixed:

- atomic multiply keeps grade
- grade-1 multiply currently folds first
- higher-grade multiply may reduce to a lower-grade pair result or preserve a
  raw kernel

So multiply already shows why operations need explicit reduction mode instead
of being treated as one uniform one-result law.

That raw kernel matters.
In complex-like or directed-segment-like multiplication, the familiar folded
result often comes from a richer four-part activity such as:

- recessive with recessive
- recessive with dominant
- dominant with recessive
- dominant with dominant

Those parts may later reduce to one familiar result, but the kernel is often
important structure in its own right and should not be treated as accidental
intermediate bookkeeping.

### Boolean

Boolean currently looks separate in code, but structurally it is:

- source domain: frame plus compared members
- correspondence topology: route partitions
- local law: occupancy truth test at one partition
- retention mode: keep zero, one, or many surviving pieces
- carrier inheritance: frame or contributing member
- reduction mode: optional merge of compatible adjacent pieces

So boolean is not special because it is boolean.
It is special because it currently uses:

- partition topology
- piece-family retention

Those same ideas will also matter for:

- reverse continuation candidates
- branch families
- partial route traversal
- boundary overflow analysis
- later equation stepping

### Traversal Runtime

Traversal runtime is the same pattern with explicit continuation:

- source domain: route plus mover plus carried data
- correspondence topology: encountered sites in current local order
- local law: site attachment law
- retention mode: carried updates, notes, tension, maybe later branch frontiers
- carrier inheritance: site/route/runtime storage targets
- continuation mode: repeated stepping over time

So traversal is not "other than operations."
It is operations with explicit continuation and site encounter.

## Core3-Native Bias

The implementation should keep resisting helper explosion.

The first instinct should be:

- express the behavior in native Core3 elements
- reuse existing route, site, mover, family, pin, branch, and tension patterns
- only add wrapper structure when provenance or temporary runtime bookkeeping
  truly needs it

Friendly names and convenience helpers are fine.
But the deeper mechanism should prefer Core3 structure over fixed code trees.

This is especially important for:

- retention
- reduction
- continuation
- branch preservation
- merge / rejoin

If the current implementation temporarily uses a specialized subsystem, it
should leave a note that this wants later consolidation into the shared
operation/branch runtime.

## Why Boolean Feels Parallel Now

Boolean currently lives in a separate subsystem mainly because it already
implements:

- partition topology
- multi-piece retention
- carrier inheritance over surviving pieces

while arithmetic currently mostly assumes:

- direct pair topology
- one retained result

So boolean is ahead in one direction and behind in another.

It is ahead because it already thinks in partitions and surviving piece
families.
It is behind because it still uses a specialized projection subsystem rather
than one generic law surface.

## Current Recommendation

The next consolidation target should not be "make boolean look like add" or
"make add look like boolean."

The target should be:

- one shared operation pipeline
- with different topology/retention/reduction choices

That means:

- add becomes a one-result accumulation law
- multiply becomes a kernel/reduction law
- boolean becomes a partition/piece law
- traversal becomes the same law family with explicit continuation

## Immediate Practical Recommendation

For the next implementation pass:

1. Keep `Family` as the current data area.
2. Do not over-commit family semantics yet.
3. Define one generic operation descriptor in terms of:
   - topology
   - local law
   - retention
   - carrier inheritance
   - reduction
4. Treat existing simple operations as optimized named presets of that deeper
   structure.
5. Treat boolean as the first explicit multi-piece preset, not as a permanent
   separate subsystem.
6. Keep the holder / view split compatible with the experimental data layer, so
   law cleanup does not hard-code one narrow collection interpretation into
   `Family`.

That descriptor does not need to become a large new class family.
It may begin as a very small internal shape, or as shared helpers over the
existing operation context and result types, as long as the deeper pattern is
kept visible.

## Long-Term Direction

Long term, even a simple addition should be conceptually traversable.

That means:

- it can be stepped
- it can be visualized as growing over time
- it can be interrupted, branched, or lowered at intermediate states
- it can share a deep structural explanation with larger equations

So a primitive binary operation is not a separate privileged kind of thing.
It is a small, optimized, easy-to-read instance of the same larger dynamic law
system that later supports:

- routes
- sites
- movers
- branches
- loops
- repeated equations
- syntax-like flows

