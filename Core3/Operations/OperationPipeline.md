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

## Family As Current Data Area

For now, `EngineFamily` should be treated pragmatically as the current data
area.

It holds:

- a frame role
- a collection of members
- an ordered or unordered hint
- temporary focus and parent provenance

Sometimes the frame is rich calibration.
Sometimes it may be little more than a neutral or zeroed holder.

That is acceptable for now.

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
- retention mode: one result plus possible tension
- carrier inheritance: aligned result carrier
- reduction mode: none

Long term, add/subtract should also be explainable as the optimized reduction of
a more explicit traversal or correspondence process.

### Multiply

Current multiply is more mixed:

- atomic multiply keeps grade
- grade-1 multiply currently folds first
- higher-grade multiply may reduce to a lower-grade pair result or preserve a
  raw kernel

So multiply already shows why operations need explicit reduction mode instead
of being treated as one uniform one-result law.

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

1. Keep `EngineFamily` as the current data area.
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
