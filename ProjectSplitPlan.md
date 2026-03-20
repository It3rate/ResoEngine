# Core2 Project Split Plan

## Goal

Separate the repository into layers that match the conceptual model:

- `Core2`: pure numeric and structural law
- `Core2.Symbolics`: symbolic composition, expressions, evaluators, rewrites
- `Core2.Interpretation`: placement, traversal, analysis, folding previews, editing helpers
- `Applied`: domain-specific systems built on the lower layers
- `Visualizer.WinForms.Core2`: UI only

The aim is that a consumer can reference `Core2` alone and work with the mathematics without carrying interactive, traversal, or display-oriented overhead.

## Dependency Direction

The intended dependency direction is:

`Core2 <- Core2.Symbolics <- Core2.Interpretation <- Applied <- Visualizer`

Allowed simplification during migration:

`Core2 <- Core2.Interpretation`

until `Core2.Symbolics` is populated.

Rules:

- `Core2` must not depend on any higher layer.
- `Core2.Symbolics` may depend on `Core2`, but not on interpretation or UI concerns.
- `Core2.Interpretation` may depend on `Core2` and `Core2.Symbolics`.
- `Applied` should hold domain readings such as geometry/frieze systems, not general Core 2 helper infrastructure.
- `Visualizer` should depend on the lower projects and contain no mathematical truth of its own.

## Layer Definitions

### 1. Core2

This project owns anything that changes what is mathematically true.

It should contain:

- primitives and canonical element types
- arithmetic and canonical reductions
- pinning laws
- carrier identity and routing identity
- branching and rejoining law
- repetition, inverse continuation, and power law
- units and physical-signature law
- canonical resolution/multiscale law
- tensions as mathematical outcomes

It should not contain:

- placement/embedding helpers
- traversal state machines for editor/runtime navigation
- structural profiles intended for inspection panes
- planar or curve fold previews
- scene editing helpers
- visualization-specific copy/debug text

### 2. Core2.Symbolics

This project owns explicit symbolic structure.

It should contain:

- expression trees / ASTs
- symbolic pinning forms such as `A * B @ P`
- evaluators
- rewrite rules
- normalization / simplification passes
- symbolic branching metadata where it belongs to expression structure
- program-like composition of expressions

It should not contain:

- UI interaction
- graph preview helpers
- domain folds such as frieze composition

### 3. Core2.Interpretation

This project owns ways of reading or traversing Core 2 structures without redefining their truth.

It should contain:

- placement and embedded readings
- traversal and boundary walking helpers
- carrier graph analysis/profile views
- fold previews and embedding support
- scene editing helpers
- serialization meant for editors, copy/paste, or diagnostics

It should not contain:

- canonical arithmetic
- canonical pinning law
- domain-specific composition logic

### 4. Applied

This project owns domain systems built from the lower layers.

It should contain:

- geometry/frieze composition
- future CAD, electricity, motion, language, or similar domain packages
- domain-specific folds and runtime rules

It should not contain:

- generic Core 2 placement/traversal infrastructure that any domain would need

## Canonical Fold vs Interpretive Fold

The word "fold" is currently overloaded and should be split conceptually.

### Canonical Fold

Canonical fold belongs in `Core2`.

Examples:

- scalar / proportion / axis / area multiplication
- addition
- boolean reduction
- pinning reduced under a canonical law
- branch-family or tension preservation when a result does not collapse uniquely

These folds are part of the mathematics itself.

### Interpretive Fold

Interpretive fold belongs in `Core2.Interpretation` or `Applied`.

Examples:

- choose one planar embedding of a carrier graph
- draw a bowl as a curve
- treat a structure as a glyph stroke
- preview a tangent-based path
- render a circuit-like reading

These folds are not the mathematical truth; they are a chosen representation of it.

## Current Folder Classification

This section does not list every class, but it gives the intended direction.

### Keep in Core2

Current areas that mostly remain:

- `Core2/Elements`
  - primitive elements and canonical pinning/routing types
- `Core2/Branching`
- `Core2/Dynamic`
- `Core2/Units`
- `Core2/Resolution`
- law-level parts of `Core2/Repetition`
- law-level parts of `Core2/Support`
- `Core2/Axioms`

Examples likely to remain:

- `Scalar`, `Proportion`, `Axis`, `Area`
- `PinAxisInterpreter`, `PinAxisResolution`
- `CarrierPinSite`, `CarrierSideAttachment`, `CarrierRoute`, `CarrierIncident`
- `BranchFamily`, `BranchGraph`
- `DynamicRunner`
- `Quantity`, `UnitSignature`
- `LayeredQuantity`, `ResolutionFrame`
- `ElementArithmeticResolver`, `AlgebraTable`

### Move to Core2.Interpretation

Current areas that are strong candidates to move:

- host-relative placement helpers
- traversal definitions/states/steps
- boundary/pin traversal helpers
- graph analysis/profile types
- attachment occurrence reporting when used as a read model

Examples likely to move:

- `PositionedAxis`
- `CarrierPinGraphAnalysis`
- `CarrierAttachmentOccurrence`
- `AxisTraversalDefinition`
- `AxisTraversalState`
- `AxisTraversalStep`
- `LocatedPin`
- `PinBoundaryTraversal`
- parts of `BoundaryPinPair` and `BoundaryContinuation` that exist mainly to support traversal reading rather than law

### Split Between Core2 and Interpretation

Some current files likely need to be divided, not moved whole.

Examples:

- `BoundaryPinPair`
  - canonical boundary pairing may remain in `Core2`
  - traversal-facing helper behavior may move out
- `BoundaryContinuation`
  - canonical continuation laws remain in `Core2`
  - path-walking helpers may move out
- `CarrierPinGraph`
  - structural graph truth remains in `Core2`
  - profile/summary generation may move out

### Move to Core2.Symbolics Later

Nothing substantial exists yet, but this is where future work should go:

- expression ASTs
- symbolic pinning expressions
- boolean expression objects
- evaluators/rewriters
- programmatic symbolic branching

This project should be created early even if it is initially sparse, so the dependency shape is already visible.

## Proposed Folder Structures

### Core2

- `Primitives`
- `Algebra`
- `Pinning`
- `Carriers`
- `Branching`
- `Repetition`
- `Dynamics`
- `Units`
- `Resolution`
- `Axioms`

### Core2.Symbolics

- `Expressions`
- `Pinning`
- `Boolean`
- `Evaluation`
- `Rewriting`
- `Programs`

### Core2.Interpretation

- `Placement`
- `Traversal`
- `Analysis`
- `Folding`
- `Serialization`
- `Editing`

### Applied

- `Geometry`
- `CAD`
- `Electricity`
- `Motion`
- `Language`

The `Applied` folders should remain domain-facing rather than becoming a dumping ground for generic helpers.

## Migration Strategy

Do not perform the split in one large move.

### Phase 0: Boundary Lock

- agree on project names
- agree on dependency direction
- agree on the canonical vs interpretive fold distinction

No behavior change should happen here.

### Phase 1: Create the New Projects

- add `Core2.Symbolics`
- add `Core2.Interpretation`
- add project references with the intended direction
- keep all code where it is initially

This phase only establishes the architecture.

### Phase 2: Move the Cleanest Interpretation Types

Move the least controversial read-only interpretation types first:

- `PositionedAxis`
- `CarrierPinGraphAnalysis`
- `CarrierAttachmentOccurrence`

Update `Visualizer` and `Applied` to reference `Core2.Interpretation`.

This phase should prove the project boundaries without destabilizing the mathematics.

### Phase 3: Move Traversal Surfaces

Move traversal-oriented infrastructure:

- `AxisTraversalDefinition`
- `AxisTraversalState`
- `AxisTraversalStep`
- `LocatedPin`
- `PinBoundaryTraversal`

Then evaluate whether boundary helpers should split further.

This is likely the biggest non-symbolic move.

### Phase 4: Split Mixed Types

Refactor the types that currently contain both law and interpretation responsibilities:

- `BoundaryPinPair`
- `BoundaryContinuation`
- possibly `CarrierPinGraph`

The outcome should be:

- canonical law in `Core2`
- traversal/view/profiling helpers in `Core2.Interpretation`

### Phase 5: Introduce Symbolics

Add the first real symbolic layer:

- expression nodes
- symbolic pinning forms
- evaluators
- rewrite/simplify passes

Do this after the interpretation split, so expressions do not inherit the current project clutter.

### Phase 6: Namespace and Folder Cleanup

Only after behavior is stable:

- rename namespaces to match the final project structure
- reorganize folders inside each project
- update tests to reflect the new boundaries

## First PR Slice

The safest first code-moving PR should be:

1. create `Core2.Symbolics`
2. create `Core2.Interpretation`
3. move `PositionedAxis`
4. move `CarrierPinGraphAnalysis`
5. move `CarrierAttachmentOccurrence`
6. update references in `Applied`, `Visualizer`, and tests

Why this slice:

- it is meaningful but small
- it tests the new boundary quickly
- it does not yet force a decision on the harder traversal split

## Decision Rules During Refactor

When deciding where a type belongs, use these rules:

1. If removing it would change what values, pins, routes, branches, or tensions are lawful, it belongs in `Core2`.
2. If removing it would only remove a way of placing, traversing, profiling, previewing, editing, or serializing those lawful structures, it belongs in `Core2.Interpretation`.
3. If it represents explicit symbolic composition rather than evaluated structure, it belongs in `Core2.Symbolics`.
4. If it chooses a domain-specific reading such as geometric composition, it belongs in `Applied`.

## Expected Benefits

- `Core2` becomes readable as a mathematical library.
- symbolic work can grow without being mixed into traversal/editing infrastructure.
- UI helpers stop polluting the pure ontology.
- `Applied` becomes a true consumer of the lower layers.
- future domains can reuse interpretation infrastructure without depending on the visualizer.

## Main Risks

- mixed files that currently contain both law and interpretation logic
- temporary namespace churn
- circular dependency pressure if traversal is moved before the boundaries are clean
- accidental movement of canonical fold logic out of `Core2`

The migration plan above is designed to reduce these risks by moving the least controversial interpretation types first.
