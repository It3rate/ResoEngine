# Core3 Boolean Notes

This note captures the current direction of boolean and boolean-like operations
in the `Core3.Engine` model.

It is intentionally lighter and more exploratory than `Core3Axioms.md`.

## Core Split

The current direction suggests three separate questions that should not be
collapsed into one:

1. Occupancy law
   At one local partition of a frame, which members are present?

2. Pairing topology
   How are members compared?
   Examples:
   - frame to each member
   - focused member to each other member
   - all members together as one unordered family
   - all to all

3. Carrier inheritance
   If a partition survives, what carries the result?
   Examples:
   - the frame
   - a focused member
   - one contributing member
   - a newly created result carrier

This separation is important because boolean truth and result structure are not
the same thing.

## Binary And Family Modes

The classical 16 boolean operators are intrinsically binary and role-sensitive.
They assume two distinct operands, often read as `A` and `B`.

That means they remain natural when the operation is:

- frame vs member
- focused member vs one other member
- one explicit binary comparison

In those cases, dominant and recessive or primary and secondary roles still
exist, so all 16 operators can be meaningful.

When a family is treated all at once as an unordered occupancy collection, only
the symmetric operators remain natural.

For a family of booleans at one local partition, the natural family-wide reads
are things like:

- none
- any
- all
- not all
- exactly one
- odd
- even
- later: majority, exactly `k`, at least `k`, at most `k`

So the binary 16 and the unordered family operators are not competing systems.
They are two different occupancy grammars for two different structural
situations.

## Symmetric Occupancy And Numeric Operations

Symmetric occupancy rules suggest a broader unification with arithmetic-like
operations.

Boolean occupancy usually asks for a `0/1` answer at each local partition:

- keep nothing
- keep one truth
- keep the partition if the predicate passes

But one can imagine using the same local comparison pattern with a different
retention law.

Examples:

- boolean keeps one truth value
- additive accumulation keeps all true contributions and combines them
- multiplication may use a different pairing topology, such as all-to-all,
  before combining them

This suggests that boolean, addition, and multiplication may be related by a
shared higher-level pattern:

- how local correspondences are generated
- what survives from those correspondences
- how surviving contributions are folded

In this reading, boolean is not separate from arithmetic so much as the
lowest-retention member of a broader family of local comparison laws.

This should be treated as a guiding idea rather than a settled implementation
rule.

## Focused Member

The current engine does not yet have an explicit focused-member object.

What it has now is:

- a frame
- a family of referenced members
- binary operations where the compared pair is explicit

A future focused-member mode would mean:

- one member of the family is treated as the current carrier-inheritance target
- the other members are compared against it one by one
- the focused member receives modification, preservation, suppression, or
  tension according to the operation law

This is different from whole-family evaluation, where no one member is special
and the result often wants a new carrier or a frame-carried result instead.

So there are likely three important operation modes:

1. Frame-focused
   The frame compares against each member and receives the interpretation.

2. Member-focused
   One member is primary and the others modify it.

3. Family-self-contained
   The family is treated as one unordered occupancy set and the result usually
   wants a new carrier or a frame-derived carrier.

This focused-member idea is not yet explicit in the current runtime shape, but
it fits well with the existing frame/family/result structure.

## Present Core3 Recommendation

For now:

- keep the current binary 16 operators for explicit two-input boolean work
- treat family-wide boolean as symmetric occupancy predicates rather than trying
  to stretch all 16 into unordered form
- keep occupancy law separate from carrier inheritance
- leave focused-member operation as a later refinement rather than forcing it
  into the current API too early

This should give Core3 a cleaner long-term path for:

- boolean
- add / accumulate
- multiply / all-to-all products
- boundary overflow and continuation analysis
- later equation and syntax layers
