# Core3 Axioms

This folder holds the independent `Core3.Engine` working model.

It is intentionally separate from `Core3.Elements`.
The current elements remain the source of truth for semantics, while the engine
explores the more generic grade-first structure that those elements can later
adopt internally.

## Aim

The engine should be:

- grade-first
- pin-first
- small
- runtime-graded rather than name-driven

Names such as `RawExtent`, `Proportion`, `Axis`, and later `Area` are treated as
helpful language views over recurring engine configurations, not as the deepest
primitive categories.

## Axioms

1. Grade is structural.
   A composite object of grade `g > 0` is built from two children of grade `g - 1`.

2. Carrier is a role, not a separate ontology.
   A carrier is simply a lower-grade element acting as recessive support,
   dominant support, boundary, or readout inside a higher-grade relation.

3. The engine is numerical first.
   The engine should not store separate handedness, clockwise, or orientation
   flags when those can be read from the ordered numeric child structure itself.

4. Grade-0 is the leaf numeric pair.
   A grade-0 engine element carries:
   - a signed realized value
   - a unit number
   Opposite perspective negates the value but preserves the unit.

5. Opposite perspective is recursive.
   At every composite grade, opposite perspective is:
   - recursively invert each child
   - reverse child order
   The same law is used at every grade.

6. Perspective is local.
   Each observer uses the same local chart rules.
   What changes between observers is the opposite-perspective transform, not the
   local meaning of right-side versus left-side reading.

7. Unit kind is preserved under opposite perspective.
   If a unit represents aligned versus orthogonal carrier preference, that kind
   remains the same under observer inversion.
   Opposite perspective changes direction, not carrier class.

8. New grade capabilities are emergent, not separately stored.
   At grade 1, perspective change mainly appears as re-reading of the paired
   numeric structure.
   At grade 2 and above, the same recursive rule makes new orientation
   capabilities visible, such as endpoint order, sheet order, and later
   handedness.
   These are read from the ordered child numbers after transformation.

9. Pinning creates relation before reduction.
   A pin is a located relation between a recessive child and a dominant child.
   It does not by itself force addition, multiplication, boolean reduction, or
   any other fold.

10. Pinning normalizes locally.
    In the generic contrastive read, the recessive child is transformed into the
    inbound reading by the same opposite-perspective rule that applies anywhere
    else in the engine.
    The dominant child remains outbound.

11. Addition is a same-space fold.
    Addition is natural only when the normalized children occupy the same unit
    space after local normalization.

12. Multiplication is a contrast fold.
    Multiplication is natural when the normalized children remain distinct after
    local normalization.
    If the children collapse into the same unit space, multiplication requires a
    lift such as sequence, phase, or another ordered distinction.

13. Boolean operations are support/frame folds.
    Boolean folds compare occupancy or support and do not require arithmetic
    reduction.

14. Forced folds require lift or preserve tension.
    If a requested operation is not natural in the current normalized space, the
    engine should not silently fake it.
    It should preserve the contradiction or add the smallest lawful lift.

15. Pinning and dimensional lift are distinct.
    Pinning always creates a new relation object.
    It does not automatically create a new independent dimension.
    Dimensional lift is a later consequence of unit relation and fold choice.

16. Result grade is operation-defined.
    Pinning raises representation grade because it creates a new relation.
    Folding may preserve, lower, branch, or lift the represented result grade.

## Working Interpretation

The engine should be read with three distinct layers in mind:

- structure: grade and parent-child composition
- local relation: pin normalization and perspective
- fold law: what reduction, preservation, or lift is currently being requested

Perspective should be read recursively:

- grade 0: negate value, preserve unit
- grade 1: recursively re-read the pair in opposite order
- grade 2: recursively re-read the pair in opposite order, which now appears as
  endpoint-role reversal
- grade 3 and above: the same rule continues, and higher-order orientation
  features are read from the transformed child order rather than from stored
  flags

This allows the engine to say:

- these two things are pinned
- these two things normalize into the same space
- they are naturally addable
- but they may still remain unreduced if the fold has not been requested

or:

- these two things are pinned
- they normalize into contrastive space
- multiplication is the natural fold
- addition would require contradiction handling or lift

## Relationship To Existing Core3 Elements

`Core3.Elements` remains the practical semantic reference.
`Core3.Engine` is the more generic runtime-graded model that follows the same
ideas but does not yet replace the existing element internals.
