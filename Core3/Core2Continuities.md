# Core2 Continuities

This file records the Core2 prompt directions that are not yet fully present in
the current Core3 engine/runtime/binding stack, but still matter as continuity
targets.

It is intentionally not the main Core3 prompt file.

The goal here is to preserve the larger trajectory without pretending every
piece is already implemented.

## 1. Inverse Continuation

Core2 continuity to preserve:

- reverse continuation is not mere arithmetic inversion
- candidate sets should be determined before forced selection
- ambiguity may remain as tension
- principal branch and all-candidates modes should both be possible
- reverse paths should preserve continuity and degree-specific role when
  possible

Why this matters for Core3:

- current Core3 folds but does not yet really invert
- route and traversal machinery should eventually support reverse-path
  selection
- equation stepping and branch families will need this to remain coherent

Current status:

- conceptually relevant now
- not yet implemented as a general engine/runtime capability

## 2. Branch Families

Core2 continuity to preserve:

- branching is general
- co-present branches differ from alternatives
- branch tension is informative
- selection should not erase the whole family
- lift, rejoin, and frontier are explicit structure

Why this matters for Core3:

- the route/site/mover/read/law/data model naturally leads to branching
- boolean already preserves co-present piece families in a limited way
- traversal machines and later equations will need explicit split/rejoin logic

Current status:

- small hints are present
- full branch-family runtime is not yet present

## 3. Branch Graphs

Core2 continuity to preserve:

- local branch events differ from the long-range branch graph
- frontier is the current active set
- rejoin preserves provenance
- crossing does not imply joining
- selection may change activity without deleting structure

Why this matters for Core3:

- if branch families arrive, provenance and graph identity should not be an
  afterthought
- later visualization work will likely need this directly

Current status:

- future-facing
- not yet represented in current Core3 runtime structures

## 4. Dynamic Execution

Core2 continuity to preserve:

- execution is state plus environment
- strands/proposals may be partial
- resolution is a first-class step
- tension is executable information
- stop conditions are part of the law

Why this matters for Core3:

- this is very relevant to the route/site/mover direction
- operation attachments and carried data are already pushing toward it
- a future unified traversal state will likely want this language

Current status:

- partly relevant now as design guidance
- not yet implemented as an execution layer

Recommendation:

- keep this as continuity guidance, not yet as a claim about current runtime

## 5. Full Continuation Laws

Core2 continuity to preserve:

- wrap, reflect, clamp, continue, oscillate, and similar labels are summaries
  of deeper continuation structure
- boundary behavior persists across firings
- encountered landmarks can redirect or absorb traversal before outer
  boundaries are reached

Why this matters for Core3:

- current mover uses only a tiny `+1` with clamp-like stop
- the route model will eventually want explicit continuation laws

Current status:

- strongly relevant
- only minimally implemented so far

## 6. Stateful Carriers

Core2 continuity to preserve:

- traversal is not memoryless
- seed, offset, facing, and previous encounters matter
- hidden lead versus visible span may matter
- one named equation may yield different outcomes at different internal states

Why this matters for Core3:

- current mover is deliberately too small to express all of this
- it should stay on the roadmap as the runtime becomes more physical

Current status:

- conceptually aligned
- not yet materially present

## 7. Multiscale Resolution

Core2 continuity to preserve:

- layered readings across several grains
- coarse representative plus residual
- signed digits and decompositions across a ladder
- exact structure preserved beneath coarser reads

Why this matters for Core3:

- Core3 already cares deeply about support and resolution
- future lower-level storage and machine exploration will likely benefit from
  explicit layered resolution

Current status:

- only partially present in current exact-support thinking
- not yet a full runtime or engine capability

## 8. Unit Signatures And Physical Referents

Core2 continuity to preserve:

- structural degree and unit-kind differ
- units may carry signatures, referents, and constraints
- structural algebra and unit algebra compose
- unit tension is informative

Why this matters for Core3:

- `Core3.Units` is already a small step in this direction
- later laws and folds will want richer unit honesty than the current engine
  alone provides

Current status:

- partially present
- still far from the full Core2 vision

## 9. General Execution Selection

Core2 continuity to preserve:

- candidate family first
- selection later
- continuity and minimal added tension matter
- one displayed path should not erase the others

Why this matters for Core3:

- route and mover execution will eventually need principled choice
- machine learning and pattern discovery will also want preserved alternatives

Current status:

- not yet implemented directly
- should remain a standing design constraint

## 10. Present Reading

These Core2 continuities should currently be read as:

- design pressure
- semantic guidance
- future runtime requirements

not as claims that the present Core3 engine already implements all of them.

The most immediately useful of them for upcoming Core3 work are:

- inverse continuation
- branch families
- dynamic execution
- fuller continuation laws
- stateful carriers

Those are the places where the current route/site/mover/read/law/data model is
most likely to grow next.
