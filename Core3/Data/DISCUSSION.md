# Core3 Data Layer Discussion

## Context
Exploring how to replace the DataArcs numeric system (Series/Stores/Samplers) with Core3's grade-based architecture. The DataArcs system uses float arrays with separate sampler strategies; Core3 uses GradedElements with value/unit pairs and recursive composition.

## Key Architectural Insight: Unit Sign Determines Reading Mode

The three categories of operation map directly onto AtomicElement's existing unit-sign semantics:

### Positive Unit (Aligned) → Value Reading
- Frame and data share a carrier
- "Read these members as calibrated amounts in my unit space"
- Frame provides resolution and bounds
- This is measurement/calibration
- **Active when:** frame children have positive units

### Negative Unit (Orthogonal) → Structural Reading
- Frame forces a dimension lift on the data
- "Organize these members along my axis"
- This IS what pinning does — locating elements orthogonal to their own nature
- Strides are orthogonal axes imposed on flat data
- Swizzling is path selection through imposed structure
- Neighbor relationships emerge from adjacency within imposed dimensions
- **Active when:** frame children have negative units

### Zero Unit (Unresolved) → Generative/Evaluative Reading
- Relationship between frame and data hasn't settled
- "The answer doesn't exist yet — derive it"
- Interpolation, curve derivation, area emergence, linguistic arcs
- The law that resolves this is TBD — needs more work before implementing
- **Active when:** frame children have zero units

## Why Not an Enum

A composite frame can have **mixed unit signs** in its children. The recessive might be structural (organizing into rows) while the dominant is evaluative (generating interpolated values). A single frame does both simultaneously at different levels of its tree.

The dispatch is structural, not conditional. `CommitToCalibration` already behaves differently based on unit signs. The view recursively descends the frame tree, and each leaf's unit sign determines the operation at that position.

## Unification Principle

Pin, stride, and projection are all the same gesture: **impose an orthogonal axis that reorganizes what's beneath it.** The difference is only scope:
- Pin → structural organization of a single element's position
- Stride → structural organization of a collection's topology
- Projection → structural reorganization within elements' composite paths

## Data Classes Overview

Instead of separate classes per operation type (like DataArcs' Series/Store/Sampler split), the Data folder contains:

- **FamilyView** — unified reading of a family through a frame, where the frame's unit signs determine whether each level calibrates values, organizes structure, or (eventually) generates new structure
- **FamilyInterpolation** — weighted blend between two elements (generative layer, parked for now)
- **FamilyBlend** — multiple FamilyViews blended by weight
- **FamilyCombine** — sequential accumulation across views with a custom law
- **FamilyStride** — multi-dimensional access (structural layer via orthogonal dimensions)
- **FamilyFold** — recursive pairwise reduction (generative layer, parked for now)
- **FamilyProjection** — component selection via structural path descent

## DataArcs → Core3 Mapping

| DataArcs | Core3 Equivalent |
|----------|------------------|
| Series (float[]) | EngineFamily with atomic/composite members |
| Store (sampler + capacity) | FamilyView (frame determines virtual capacity) |
| BlendStore | FamilyBlend |
| FunctionalStore | FamilyCombine |
| GridSampler/HexSampler | FamilyStride with orthogonal dimensions |
| Slot enum + swizzle | FamilyProjection via structural descent paths |
| BezierSampler | FamilyFold with de Casteljau |
| CombineFunction enum | GradedElement operations (Add/Multiply/etc.) |
| BakeData() | FamilyView.Materialize() |
| ParametricSeries | TraversalMover (tick/endTick = rational position) |

## Revised Architecture (Round 2): Frame-Driven Dispatch

The classes were revised so that FamilyView is the unified reader, dispatching based on what the frame IS rather than on separate strategy classes:

### FamilyView — the unified reader
- `ReadThroughFrame()` inspects the frame and dispatches:
  - **Aligned atomic leaf** → `ReadValueCalibrated()` — maps mover to member, commits to frame's resolution
  - **Orthogonal atomic leaf** → `ReadStructural()` — frame's resolution = stride width, wrapping is natural
  - **Unresolved atomic leaf** → falls through to value read for now (generative parked)
  - **Composite frame** → recurses, each child handles its own layer
- `GetNeighbors()` extracts orthogonal leaves from the frame to determine structural dimensions
- `Materialize()` bakes virtual positions into actual members

### FamilyStride — now a frame builder, not a sampler
- Changed from a wrapper class to a static helper that BUILDS structural frames
- `CreateStride(width)` → `AtomicElement(width, -width)` — negative unit = orthogonal = structural
- `CreateGrid(cols, rows)` → `CompositeElement(col_stride, row_stride)` — two orthogonal axes
- `CreateStructuredWithCalibration()` → mixed frame: orthogonal recessive + aligned dominant
- `CreateNDimensional()` → nested composites from N dimension widths
- `ApplyTo()` → sets a family's frame to a structural frame, returns a FamilyView

### FamilyProjection — structural reorganization within elements
- Unchanged in mechanism but reframed conceptually
- Same gesture as stride but operating within each member rather than across the family
- Path descent through composite tree = orthogonal structure selection

### FamilyBlend — annotated as boundary between structural and generative
- Selection of adjacent views = structural (orthogonal organization across views)
- Interpolation between them = generative (unresolved, needs evaluation)

### FamilyCombine — primarily value-layer
- Each view reads through its own frame, then results combined with arithmetic law

### FamilyInterpolation / FamilyFold — generative layer (parked)
- Both annotated as generative operations
- Connection noted: a zero-unit frame leaf could invoke fold/interpolation to resolve tension
- Fold intermediate levels ARE the derived structure, not disposable computation

## Design Principle: Three Layers, One Mechanism

The three layers (value/structural/generative) are NOT modes or enum values.
They are what the frame IS, determined by unit signs at the atomic leaves:

- **Positive unit** → value calibration (measurement)
- **Negative unit** → structural organization (pinning/strides)
- **Zero unit** → generative evaluation (interpolation/folds) — parked

A composite frame mixes all three at different depths. No dispatch table needed —
the recursive descent through the frame tree naturally encounters each layer
where the frame's own structure says it belongs.

This unifies pin (structural, single element), stride (structural, collection),
projection (structural, within elements), and eventually fold/interpolation
(generative) under the same frame-reading mechanism.

## Open Questions

1. How does the generative layer carry its specific law? Does it emerge from structure, or does the unresolved frame need to reference a law somehow?
2. How do temporal operations (animation, deltaTime) map? Probably a mover with a velocity composite, but details TBD.
3. Should projection also be expressible as a frame (orthogonal frame applied within members), or is descent-path selection fundamentally different from stride organization?
