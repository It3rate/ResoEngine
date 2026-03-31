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

## Open Questions

1. How does the generative layer carry its specific law? Does it emerge from
   structure, or does the unresolved frame need to reference a law somehow?
2. How do temporal operations such as animation or delta-time map?
   Probably a mover with a velocity composite, but details are still open.
3. Should projection also be expressible as a frame, or is descent-path
   selection fundamentally different from stride organization?
