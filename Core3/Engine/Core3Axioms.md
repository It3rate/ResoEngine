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

`RawExtent` is not itself a true engine element.
It is a descriptive bootstrap for uncalibrated support that helps a computer
hold the idea of "some extent exists here" before that support is turned into a
graded element.

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
   - an exact signed unit number
   Opposite perspective negates the value but preserves the unit.

5. Unit sign gives carrier relation.
   A positive unit preserves the current carrier.
   A negative unit expresses orthogonal force relative to the current carrier.
   A zero unit preserves unresolved carrier choice and should not be silently
   treated as either aligned or orthogonal.

6. Unit magnitude gives resolution support.
   The absolute unit magnitude is exact resolution support.
   `10` and `-10` therefore share support magnitude while differing in carrier
   relation.

7. Zero-unit behavior remains unresolved until later relation.
   A zero unit is not addable with positive or negative units.
   `0/0` is clamp-like hold.
   A zero unit with nonzero value may still emit or receive traversal pressure,
   but its carrier direction remains unresolved until pinning or later fold
   gives it a lawful read.

8. Folded reads remain exact rational.
   Folding a grade-0 element lowers it to an exact rational read `value / unit`.
   Decimal or floating display is an external approximation choice, not an
   engine value.

9. Primitive transforms remain distinct.
   The engine should expose at least:
   - negation
   - order swap
   - perspective flip
   These may share implementation machinery, but they should remain separate
   named transforms because their meanings diverge at higher grades.

10. Negation flips realized direction.
    Negation changes realized value direction without changing unit sign or local
    child order.

11. Order swap is grade-local.
    Order swap reverses the recessive/dominant order of the current composite
    grade.
    If a grade has no such local order, order swap is the identity there.

12. Opposite perspective is recursive.
   At every composite grade, opposite perspective is:
   - recursively invert each child
   - reverse child order
   The same law is used at every grade.

13. Perspective is local.
   Each observer uses the same local chart rules.
   What changes between observers is the opposite-perspective transform, not the
   local meaning of right-side versus left-side reading.

14. Unit kind is preserved under opposite perspective.
   If a unit represents aligned versus orthogonal carrier preference, that kind
   remains the same under observer inversion.
   Opposite perspective changes direction, not carrier class.

15. New grade capabilities are emergent, not separately stored.
   At grade 1, perspective change mainly appears as re-reading of the paired
   numeric structure.
   At grade 2 and above, the same recursive rule makes new orientation
   capabilities visible, such as endpoint order, sheet order, and later
   handedness.
   These are read from the ordered child numbers after transformation.

16. Reflection is later and parameterized.
   A geometric mirror is not currently treated as one generic grade-only
   primitive.
   Reflection normally requires a chosen mirror subspace:
   - a point at grade 0
   - a line at grade 1 or 2
   - a plane at grade 2 or 3
   and so on.
   The current named element layer may still expose view-specific `Mirror()`
   operations, but the generic engine should not pretend there is already one
   settled no-argument reflection law.

17. Pinning creates relation before reduction.
   A pin is a located relation between a recessive child and a dominant child.
   It does not by itself force addition, multiplication, boolean reduction, or
   any other fold.

18. Pinning normalizes locally.
    In the generic contrastive read, the recessive child is transformed into the
    inbound reading by the same opposite-perspective rule that applies anywhere
    else in the engine.
    The dominant child remains outbound.

19. Positioned pins derive local sides from subtraction.
    When a pin is given a host-relative position in the host child grade:
    - declared span = host dominant - host recessive
    - inbound side = position - host recessive
    - outbound side = host dominant - position
    These are exact derived reads, not stored annotations.

20. Outbound remainder is inbound tension.
    The host outbound remainder at a positioned pin may be read as inbound
    tension:
    - positive means excess support or pressure past the join
    - negative means deficit or open pull before the join
    - zero means exact fit

21. Same-space continuation can carry outbound tension.
    When same-space continuation exists beyond the pin, the host declared span
    may be carried forward as outbound tension on the surviving continuation.

22. Addition is a same-space fold.
    Addition is natural only when the normalized children occupy the same unit
    space after local normalization.

23. Multiplication is a contrast fold.
    Multiplication is natural when the normalized children remain distinct after
    local normalization.
    If the children collapse into the same unit space, multiplication requires a
    lift such as sequence, phase, or another ordered distinction.

24. Boolean operations are support/frame folds.
    Boolean folds compare occupancy or support and do not require arithmetic
    reduction.

25. Forced folds require lift or preserve tension.
    If a requested operation is not natural in the current normalized space, the
    engine should not silently fake it.
    It should preserve the contradiction or add the smallest lawful lift.

26. Pinning and dimensional lift are distinct.
    Pinning always creates a new relation object.
    It does not automatically create a new independent dimension.
    Dimensional lift is a later consequence of unit relation and fold choice.

27. Result grade is operation-defined.
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

The explicit named transforms should stay separate:

- `Negate()` flips realized direction
- `SwapOrder()` reverses current recessive/dominant order where that capability
  exists
- `FlipPerspective()` is the recursive observer-change transform

`FlipPerspective()` should not be confused with reciprocal-like folding.
At grade 1:

- a bare `SwapOrder()` may look reciprocal if the swapped pair is later folded
  under the same chart
- but `FlipPerspective()` is a chart change, not a reciprocal
- the slot meanings move with the chart, so the underlying object is preserved

For example, a proportion read as `1 / 3` from one perspective should not become
`3 / 1` as a different object from the opposite perspective.
It is the same object re-read in the opposite chart, where the denominator/value
roles move with the perspective.

Recursion means:

- first apply the same transform to the lower-grade children
- then apply the current grade's own structural effect, if that grade has one

This gives the current transform table:

| Grade | `Negate()` | `SwapOrder()` | `FlipPerspective()` |
| --- | --- | --- | --- |
| 0 | Flip realized value sign. | Identity. No child order yet. | Same as `Negate()` because there is no higher order to reverse yet. |
| 1 | Negate the lower-grade realized read. | Swap the pair. Under later folding this can look reciprocal-like. | Re-read the same pair from the opposite chart. The object stays the same; meanings move with the chart. |
| 2 | Negate child reads without changing local start/end order. | Reverse start/end order. | Flip both child perspectives, then reverse start/end order. |
| 3 | Negate child reads without changing current sheet order. | Reverse current grade-3 child order. | Flip both child perspectives, then reverse current grade-3 order, making sheet orientation appear inverted. |

The user-facing word "mirror" may still be useful, but at the generic engine
level it should be reserved for a later transform that takes an explicit mirror
subspace or axis rather than being derived solely from grade.

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

The unit number itself should be read numerically:

- positive unit: same-carrier support
- negative unit: orthogonal force
- zero unit: unresolved carrier choice
- absolute unit magnitude: exact support or resolution

For positioned pins on a line-like host:

- `declared span` is the host's full local claim
- `inbound side` is the realized reach from host recessive side to the pin
- `outbound side` is the host remainder beyond the pin
- `outbound side` may be read as inbound tension
- `declared span` may be read as outbound tension when continuation carries it
  forward

## Relationship To Existing Core3 Elements

`Core3.Elements` remains the practical semantic reference.
`Core3.Engine` is the more generic runtime-graded model that follows the same
ideas but does not yet replace the existing element internals.
