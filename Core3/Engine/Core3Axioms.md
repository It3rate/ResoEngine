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

3. Pinning creates relation before reduction.
   A pin is a located relation between a recessive child and a dominant child.
   It does not by itself force addition, multiplication, boolean reduction, or
   any other fold.

4. Pinning normalizes locally.
   In the generic contrastive read, the recessive child is mirrored into the
   inbound reading and the dominant child remains outbound.
   This produces the normalized local pair for fold selection.

5. Unit relation chooses the natural fold.
   After normalization:
   - matching unit signatures indicate same-space/additive behavior
   - distinct resolved signatures indicate contrastive behavior
   - unresolved signatures preserve tension

6. Addition is a same-space fold.
   Addition is natural only when the normalized children occupy the same unit
   space.

7. Multiplication is a contrast-space fold.
   Multiplication is natural when the normalized children remain distinct in a
   lawful way after pin normalization.

8. Boolean operations are support/frame folds.
   Boolean folds compare occupancy or support and do not require arithmetic
   reduction.

9. Forced folds require lift or preserve tension.
   If a requested operation is not natural in the current space, the engine
   should not silently fake it.
   It should either:
   - preserve the contradiction as tension
   - keep the pinned relation unreduced
   - or add the smallest lawful lift, such as sequence, phase, or sheet

10. Pinning and dimensional lift are distinct.
    Pinning always creates a new relation object.
    It does not automatically create a new independent dimension.
    Dimensional lift is a later consequence of unit relation and fold choice.

11. Result grade is operation-defined.
    Pinning raises representation grade because it creates a new relation.
    Folding may preserve, lower, branch, or lift the represented result grade.

12. Current engine scope.
    The current `Core3.Engine` model is a working runtime-graded skeleton:
    - grade-0 atomic values carry a value and a unit signature
    - higher grades are composed recursively
    - pinning normalizes and classifies fold opportunity
    - addition, multiplication, boolean, tension, and lift are represented as
      fold outcomes

## Working Interpretation

The engine should be read with three distinct layers in mind:

- structure: grade and parent-child composition
- local relation: pin normalization and perspective
- fold law: what reduction, preservation, or lift is currently being requested

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
