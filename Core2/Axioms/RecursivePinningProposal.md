# Recursive Pinning and Carrier Identity Proposal

This document is a focused proposal for the next conceptual layer after positioned pinning.
It is intended to sit beside:

- [PinningProposal.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PinningProposal.md)
- [PositionedPinningProposal.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PositionedPinningProposal.md)
- [PromptAxioms.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PromptAxioms.md)

This file is a proposal, not settled doctrine.
Its purpose is to clarify how recursive pinning, loops, and shared carriers may be represented without forcing Core 2 itself to guess one geometric embedding.

## Core Claim

The central claim of this proposal is:

- Core 2 pinning should remain primarily structural, not geometric
- carriers need identity independent of their current drawn support
- several pin sides may lawfully refer to the same carrier
- recursive pinning is lawful and should not be treated as a contradiction merely because a planar fold is difficult
- pinning should be declarative before it is constructive
- geometry belongs to later fold layers, not to the primitive pinning relation itself

In this reading, the difficult part of a shape like `D`, `O`, or a looped glyph is not first "how do we draw the curve?"
The first question is:

- which local pin sides belong to the same carrier?
- which carriers pin to which other carriers at which positions?
- what tensions or unresolved fold requirements remain?

## Primitive Terms

### Carrier identity

A carrier identity is the structural sameness of a carrier across several local pin encounters.
Two pin sides may refer to the same carrier even if a later fold draws them as different local segments, as a loop, or as a curved contour.

### Support

Support is a realized geometric or display-bearing path.
A carrier may have support in a fold, but carrier identity is deeper than any one support shape.

### Carrier graph

A carrier graph is the relational structure formed by carriers, pinning events, and host-relative attachment positions.

### Recursive pinning

Recursive pinning is pinning in which carrier `A` pins to carrier `B` and carrier `B` also pins to carrier `A`, directly or through a cycle.

### Coupled structure

A coupled structure is a set of carriers whose lawful readings depend on one another and therefore cannot be resolved completely by one one-way expansion pass.

### Geometric fold

A geometric fold is a later interpretation that embeds a carrier graph into a visual or metric space.
It may choose a polyline, curve, loop, branch, lift, or other representation.

### Fixed-point resolution

Fixed-point resolution is iterative or relaxation-based stabilization of a coupled pinning structure until no further local updates are required, or until remaining contradiction is preserved as tension.

## Part I. Structural Priority

### RP1. Core 2 Owns Relation Before Geometry

Core 2 should describe structural relation before geometric embedding.
The primitive pinning layer is not required to decide the best planar or visual shape.

### RP2. Carrier Identity Is More Fundamental Than Drawn Shape

A carrier is not identical to one currently drawn segment.
The same carrier may appear at several local pins and may later fold into:

- one straight segment
- several connected supports
- a loop
- a curved contour
- a lifted or nonplanar embedding

### RP3. Same Carrier And Same Support Differ

Two local pin readings may belong to the same carrier without already sharing one simple support segment.
This distinction is essential for loops, bowls, and recursive structures.

### RP4. Pinning Is Declarative Before It Is Constructive

At the primitive level, pinning should first declare:

- which carrier is host
- which carrier is applied
- at what host-relative position the relation occurs
- what each side locally wants

It should not immediately require one explicit geometric construction.

### RP5. A Difficult Planar Embedding Is Not A Structural Failure

If a recursive or orthogonal carrier relation is hard to embed in a plane, that does not make the underlying pinning invalid.
It only means the current fold is underdetermined, tense, or in need of lift.

## Part II. Carrier Identity

### RP6. Carriers Need Explicit Identity

Carriers should be able to remain the same carrier across several pinning events.
This should be explicit in meaning, not only accidental in implementation.

### RP7. Several Pin Sides May Refer To One Carrier

Two or more pin sides may lawfully identify the same carrier even when encountered from different hosts, directions, or positions.

### RP8. Opposite Local Readings May Still Describe One Carrier

If one pin reads a carrier as `+3i` downward and another reads it as `-3i` upward, this may still be one carrier seen from two pin contexts rather than two separate carriers.

### RP9. Carrier Identity Should Survive Reframing

If a carrier is the same structure before and after a host-relative reframe, its identity should remain the same even if its local coordinates or folded support differ.

### RP10. Carrier Identity Is Independent Of Chosen Fold

The same carrier identity may admit several folds.
Changing the fold should not silently create a new carrier unless the law explicitly branches or splits it.

## Part III. Recursive Pinning

### RP11. Recursive Pinning Is Lawful

It is lawful for carrier `A` to pin to carrier `B` while carrier `B` also pins to carrier `A`.
This is not an error merely because the relation is cyclic.

### RP12. Loops Are Special Cases Of Recursive Pinning

A loop, self-capture, wrap, or return-to-origin structure may be understood as recursive pinning in which a carrier ultimately encounters itself.

### RP13. Mutual Endpoint Reference Is Lawful

It is lawful for one carrier to say:

- I pin to another carrier at my `0` and `1`

while the other says:

- I pin to the first at my `0` and `1`

This produces a coupled structure rather than an immediate impossibility.

### RP14. Recursive Pinning Does Not Require Eager Expansion

A recursive pinning relation should not be expanded naively into infinite copied geometry.
It should remain a graph relation until a fold asks for realization.

### RP15. Recursive Structures May Stabilize Iteratively

Because recursive pinning is coupled, it may need fixed-point or relaxation-style evaluation rather than one-pass forward construction.

## Part IV. Tension, Stabilization, and Resolution

### RP16. Circular Dependency Is Not The Same As Inconsistency

A circular dependency may simply indicate that several local readings must be solved together.
Only failure to find a lawful stable reading in the chosen fold should count as unresolved tension.

### RP17. Fixed-Point Resolution Is Preferred For Coupled Pinning

When several carriers mutually constrain one another, the system should prefer iterative stabilization toward a low-tension state instead of demanding an immediate exact one-pass solution.

### RP18. Stabilization May End In Tension

If no complete low-tension fold is found in the current representation, the structure may remain with preserved tension rather than being forced into a false collapse.

### RP19. A Stable Relation Need Not Have One Stable Geometry

The relational graph may be stable even if several geometric folds remain lawful.
This is not a defect; it is legitimate multiplicity.

### RP20. Branching May Arise At The Fold Layer

If a recursive pinning graph admits several lawful embeddings or continuations, that multiplicity should be preserved as a branch family at the fold layer rather than erased by arbitrary geometric choice.

## Part V. Pinning And Geometry

### RP21. Geometry Is A Later Fold

Geometric drawing should be treated as a later fold of recursive carrier relations rather than as the primitive content of the pinning itself.

### RP22. Curves Are Not Required To Exist In Primitive Core 2

Core 2 does not need a primitive curve object to describe a structure whose later visual fold may appear curved.
It only needs the carrier identities, positions, and local pin relations that a curve fold would later interpret.

### RP23. Several Folds May Represent The Same Carrier Graph

The same recursive carrier graph may later be folded as:

- a polyline
- a rounded contour
- a straight bridge
- a lifted sheet path
- a symbolic graph with no Euclidean geometry at all

### RP24. Geometric Difficulty Does Not Decide Structural Truth

If one fold seems awkward, impossible, or self-crossing in a plane, that does not mean the underlying carrier graph is wrong.
It only means that fold is not yet ideal, or that a richer fold is required.

### RP25. Core 2 Should Not Guess Aesthetic Geometry Too Early

Questions such as:

- should this side curve?
- should it bow outward?
- should it round off?
- should it bridge directly?

belong to fold interpretation, style, or energy minimization, not to primitive pinning doctrine.

## Part VI. Glyph-Oriented Consequences

### RP26. Glyph Skeletons Fit Naturally

Capital-letter and glyph-like forms such as `L`, `T`, `Y`, `D`, and eventually `O` may be represented first as carrier graphs with recursive or shared carrier identity.

### RP27. A Stem And A Bowl Need Not Be Two Unrelated Lines

In a glyph such as `D`, the vertical stem may be one carrier while the bowl side is another single carrier whose local readings occur at two or more pins.
The bowl should not have to be represented first as two unrelated segments.

### RP28. Shared Carrier Identity Solves The First Problem

Before solving curvature, the first real requirement for glyph formation is to let several local pin sides say:

- these belong to the same carrier

This is the essential first step.

### RP29. Curvature May Be A Consequence Of Fold Pressure

If a shared carrier is constrained by several orthogonal pin readings, a later geometric fold may choose a curve-like contour as one low-tension realization.
That should be treated as a later interpretation, not as the primitive meaning of the carrier.

## Part VII. Working Interpretation

### RP30. The Primitive Question Is "What Is The Same Thing?"

In recursive pinning, the first question is not "how do we draw it?"
It is:

- which pin sides are readings of the same carrier?

### RP31. Pin Graph First, Geometry Later

The correct order of interpretation is:

1. establish carrier identities
2. establish host-relative pin positions
3. establish recursive or mutual pin relations
4. stabilize the relation graph
5. apply one or more folds

### RP32. One Carrier May Have Several Local Faces

One carrier may look like:

- upward at one pin
- downward at another
- orthogonal to one host
- same-carrier to another

without ceasing to be one carrier.

### RP33. Loop, Wrap, And Bowl Are Relatives

Loop-like, wrap-like, and bowl-like structures all point toward the same deeper concept:

- one carrier may revisit or re-identify itself across several local pin encounters

### RP34. Low-Energy Solution Is A Fold-Level Goal

The system may move toward a low-energy or low-tension stabilization, but it does not need to know the final geometry at the primitive pinning layer.
That pressure belongs to stabilization and fold selection.

## Short Examples

### Example 1. Shared vertical carrier

Two pins on different hosts may each read the same carrier with opposite local sign:

- top pin sees `+3i`
- bottom pin sees `-3i`

This need not create two separate carriers.
It may be two local readings of one vertical carrier.

### Example 2. Bowl side of `D`

Two endpoint pins on a stem may each emit onto the same outward carrier identity.
That does not yet decide whether the later fold is:

- a straight bridge
- a curve
- a rounded side
- a lifted path

It only says the same carrier is constrained at both ends.

### Example 3. Recursive mutual pinning

Carrier `A` pins to carrier `B` at `0` and `1`.
Carrier `B` pins to carrier `A` at `0` and `1`.

This should be treated as a coupled pin graph, not as an instruction to expand infinitely copied geometry.

## Summary

The main point of this proposal is:

- Core 2 should represent recursive pinning and shared carriers as structural identity and relation
- it should not force an immediate geometric answer
- cycles are lawful
- stabilization may be iterative
- geometry is a later fold

This gives the system room to represent loops, bowls, returns, wraps, and glyph skeletons without making primitive pinning depend on one privileged Euclidean construction.
