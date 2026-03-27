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
   A pin is a local inbound/outbound relation.
   It does not by itself force addition, multiplication, boolean reduction, or
   any other fold.

18. Pinning has two canonical construction modes.
   A pin may be built either as:
   - a hosted split: one host plus a position
   - an explicit local join: one inbound side plus one outbound side
   These are two ways of forming the same local pin relation.

19. Hosted pin positions may be resolved or ratio-bearing.
   In hosted mode, a pin position may be supplied either as:
   - an already resolved host-local coordinate in the host child grade
   - a graded element that can fold to one exact ratio
   Ratio-based pinning should remain exact and should not use decimal
   interpolation.

20. Ratio-based pinning scales to host resolution.
    Given host span `S` and exact ratio `r = a / b`, exact positioning uses
    ratio arithmetic:
    - read `r` as an exact folded ratio
    - scale the host span by that ratio
    - add the resulting offset to the host recessive side
    This is equivalent to scaling one resolution to the other, but should be
    implemented through exact structure rather than decimal rounding.

21. Hosted pins derive local sides from subtraction.
    When a hosted pin is given a host-relative position in the host child grade:
    - declared span = host dominant - host recessive
    - inbound side = position - host recessive
    - outbound side = host dominant - position
    These are exact derived reads, not stored annotations.

22. Explicit joins supply local sides directly.
    In explicit local-join mode, the inbound and outbound sides are already
    present and the pin position is implicit at their join site.

23. Outbound remainder is inbound tension.
    The host outbound remainder at a hosted pin may be read as inbound
    tension:
    - positive means excess support or pressure past the join
    - negative means deficit or open pull before the join
    - zero means exact fit

24. Same-space continuation can carry outbound tension.
    When same-space continuation exists beyond the pin, the host declared span
    may be carried forward as outbound tension on the surviving continuation.

25. Later fold laws may reinterpret local roles.
    A pin constructor only establishes or derives local inbound/outbound sides.
    Later fold laws may still re-express those sides for additive, contrastive,
    or other readings.

26. Addition is a same-space fold.
    Addition is natural only when the normalized children occupy the same unit
    space after local normalization.

27. Multiplication is a contrast fold.
    Multiplication is natural when the normalized children remain distinct after
    local normalization.
    If the children collapse into the same unit space, multiplication requires a
    lift such as sequence, phase, or another ordered distinction.

28. Boolean operations are support/frame folds.
    Boolean folds compare occupancy or support and do not require arithmetic
    reduction.

29. Forced folds require lift or preserve tension.
    If a requested operation is not natural in the current normalized space, the
    engine should not silently fake it.
    It should preserve the contradiction or add the smallest lawful lift.

30. Named views are opinionated common cases.
    One structural grade may admit several lawful recurring configurations.
    For example, grade 2 may support both an axis-like directed pair and a
    grid-like two-axis origin system.
    Named views should construct one canonical configuration on top of the
    generic engine, but should remain reducible back to the underlying graded
    element without loss.

31. Exact ratio view is generic, not limited to grade 1.
    Any graded element that can lawfully fold its recessive and dominant sides
    to exact ratios may expose one exact ratio read:
    - inbound or recessive fold becomes the denominator side
    - outbound or dominant fold becomes the numerator side
    The common named proportion remains the grade-1 view, but exact ratio
    reading is not restricted to that grade.

32. Pinning and dimensional lift are distinct.
    Pinning always creates a new relation object.
    It does not automatically create a new independent dimension.
    Dimensional lift is a later consequence of unit relation and fold choice.

33. Result grade is operation-defined.
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

Exact ratio-based positioning should be read the same way:

- a named proportion is one common grade-1 view over the generic engine
- but any graded element may contribute a placement ratio if it can fold to one
  exact ratio lawfully
- host placement should use that exact ratio view
- the host span is scaled by that ratio as a ratio of ratios
- no decimal interpolation is needed or desired

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

## Carrier Polarity And Products

When denominator polarity is used structurally, it should not be collapsed into
ordinary scalar negation.

In conventional algebra, expressions such as `1 / (-3)` and `1 / (-i)` are
already lawful and reduce by moving the scalar minus sign out of the
denominator. For example:

- `1 / (-3) = -1 / 3`
- `1 / (-i) = -1 / i`
- in ordinary complex arithmetic, `1 / (-i) = i`

Core3 should not silently reuse that rule when the denominator sign is being
used to encode carrier polarity rather than scalar sign. In this engine:

- `-1 / i` means negated or reversed value on carrier `i`
- `1 / -i` means positive value on an orthogonally polarized `i` carrier

Those are not the same state and should not be normalized into each other.

When clarity is needed, it is better to name the denominator-polarity state as
something like `perp(i)` rather than writing `-i` and inviting ordinary scalar
algebra to erase the distinction. In the same spirit:

- `rev(i)` is a helpful reading for `-1 / i`
- `perp(i)` is a helpful reading for `1 / -i`

This is similar in spirit to how ordinary mathematics introduced `i` as a new
named object rather than leaving it forever as an awkward "negative-unit
placeholder." If a denominator-polarity state survives as an irreducible and
lawful participant in operations, giving it a distinct name is a strength, not
a weakness.

Products should therefore be read in three layers.

### 1. Value-Sign Layer

Numerator value signs multiply exactly as usual:

| left | right | result |
| --- | --- | --- |
| `+` | `+` | `+` |
| `+` | `-` | `-` |
| `-` | `+` | `-` |
| `-` | `-` | `+` |

### 2. Carrier Layer

The raw structural product should preserve carrier information instead of
forcing early collapse.

If `a / c1` and `b / c2` are multiplied, the raw product kernel is:

`(a / c1) * (b / c2) = (ab) / carrier_product(c1, c2)`

where `carrier_product(...)` is a carrier product, not ordinary scalar
denominator arithmetic.

At this layer:

- aligned carrier times aligned carrier yields a same-space square candidate
- aligned carrier times orthogonal carrier yields a contrast candidate
- orthogonal carrier times aligned carrier yields an ordered contrast candidate
- orthogonal carrier times orthogonal carrier yields an orthogonal-family square
  candidate

The important point is that orthogonality should not disappear in the raw
product merely because two signs multiplied to `+`.

### 3. Unit-Signature Layer

The carrier product may later fold into a unit-signature product or a
higher-grade geometric reading.

Examples:

- distance times distance may fold to distance squared
- east-west times north-south may fold to an area-like sheet
- distance times mass may fold to a compound signature such as ton-mile

This means that aligned times orthogonal does not preserve the original carrier
family. It creates a new composite candidate whose later fold depends on
context, view, and requested operation.

The same raw multiplication law may therefore support several later named
readings:

- complex-like
- axis-like sequential
- area-like
- compound-unit

What varies is not the distributive expansion itself, but the later reduction
or fold law.

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
