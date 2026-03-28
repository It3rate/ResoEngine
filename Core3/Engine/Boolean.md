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

At the implementation level, this now suggests two corresponding execution
modes:

- ordered families may run binary boolean work pair by pair in family order
- unordered families may run one symmetric occupancy predicate across the whole
  family at each local partition

That keeps the engine step itself binary where appropriate, while still
allowing family-wide unordered truth when order is not part of the meaning.

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

## Focused Member As Temporary Frame

At the moment, the cleanest reading is that a focused member does not require a
new special runtime species.

Instead:

- a member is first read into its current parent frame
- that read is temporarily elevated into frame role
- the remaining members are then referenced against it

So a focused-member operation can be treated as a temporary reframing of an
existing family, not as a new ontology.

This preserves a few important Core3 principles:

- elements remain interchangeable in substance
- only their roles change
- the original family and parent frame do not need to be mutated
- the focus can be temporary, much like a read used for resolution alignment

In that sense, the focused member still remains a child of the original parent
frame, even while it is temporarily acting as the local frame for a new derived
operation family.

This gives three important operation modes without introducing extra primitive
types:

1. Frame-focused
   The frame compares against each member and receives the interpretation.

2. Member-focused
   One member is temporarily lifted into frame role and the others are compared
   against it.

3. Family-self-contained
   The family is treated as one unordered occupancy set and the result usually
   wants a new carrier or a frame-derived carrier.

The structural dual also matters:

- a derived focused family can later be collapsed back into its parent frame
- the temporarily elevated frame is reattached there as one member among the
  others
- this is another read or reframing step, not a mutation of the underlying
  elements

So temporary focusing and later collapse should be understood as two directions
of the same family-derivation mechanism.

## Ordered And Unordered Families

Families also need a small distinction between:

- ordered families, where member order is meaningful
- unordered families, where member collection matters but order does not

This matters because some operations, such as sequential pairwise comparison or
folds that preserve adjacency, care about ordering, while symmetric occupancy
predicates do not.

The family can still be stored in one list structure for now. The important
point is that "this order has meaning" and "this order is only storage" are not
the same statement.

Families should also be able to derive new orderings without mutating the
underlying members:

- sorting by one structural slot read in the current frame
- reversing or descending that same order
- shuffling into an explicitly unordered view

This fits the larger Core3 idea that many operational changes are better
understood as temporary reads or derived views than as overwriting the source
structure.

So the current recommendation is:

- do not add a special focused-member ontology yet
- allow focused-member behavior to be expressed as a convenience over existing
  frame/family/reference structure
- allow derived families to collapse back into their parent frame when that
  temporary reframing is complete
- let families carry an ordered/unordered hint rather than splitting into
  separate collection species too early
- only introduce a more explicit focused-member object later if repeated use
  shows that the temporary-frame reading is insufficient

## Present Core3 Recommendation

For now:

- keep the current binary 16 operators for explicit two-input boolean work
- treat family-wide boolean as symmetric occupancy predicates rather than trying
  to stretch all 16 into unordered form
- keep occupancy law separate from carrier inheritance
- let focused-member operation be expressed by temporarily promoting a chosen
  member into frame role rather than by inventing a new primitive structure

This should give Core3 a cleaner long-term path for:

- boolean
- add / accumulate
- multiply / all-to-all products
- boundary overflow and continuation analysis
- later equation and syntax layers
