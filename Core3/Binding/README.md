# Core3 Binding Notes

This folder is an experimental home for `Core3.Binding`.

It exists to explore the layer between:

- durable engine structure
- temporary runtime context
- executable operation laws

The current working idea is that many things that feel like "selection" are
better described as **binding to context**.

Selection is then one case of binding:

- choose a source domain
- address it locally
- extract one aspect of it
- decide what to do if the read is missing or ambiguous
- optionally store the read for later steps

That makes `Binding` the broader and more useful namespace name.

Another important current aim is that binding descriptors should live in a
**navigable Core3 value space** rather than only in opaque enum space.

That means the long-term preference is:

- serialize descriptor meaning as Core3-shaped numeric structure
- keep named enum-like labels as convenience views where helpful
- allow nearby variants to be explored by adjustment rather than only by code
  branching

## Scope

`Core3.Binding` is not intended to replace:

- `Core3.Engine`
- `Core3.Runtime`
- `Core3.Operations`

Instead it provides a place to experiment with:

- bound literals
- contextual defaults
- explicit slot coupling
- operation attachment metadata
- input/output binding descriptions

without forcing those concerns down into the serialized engine ontology too
early.

## Binding Axioms

### B1. Binding is contextual, not core ontology.

A binding describes how something is supplied, inherited, coupled, or attached
within a current context.

A bound literal is therefore not yet the same thing as a fully materialized
`GradedElement`.

### B2. Null means "not specified here."

For binding templates:

- `null` means the slot is absent in the template and may be supplied by
  context
- `null` does not mean the same thing as engine-level unresolved structure

This is why binding-time omission should not reuse the engine's zero-unit
meaning.

### B3. Explicit zero-unit meaning remains real.

Inside engine ontology, a zero unit still means genuine unresolved or
hold-like carrier meaning.

It should not be silently reused as the shorthand for "please inherit from the
frame."

### B4. Binding decomposes into four parts.

The smallest useful binding pattern has:

- a source domain
- a local address or parameter
- a transform
- a materialization rule

This pattern is broad enough to cover:

- reference-like reads
- coupled slots
- attached operations
- later routing and continuation metadata

without claiming that all such cases are one concrete type.

### B4b. A mover should supply the current site.

When traversal is stateful, the current encounter should preferably come from a
real moving container, cursor, or trolley rather than from a separate virtual
"current" selector token.

That means a selector should usually describe:

- which domain is being read
- what numeric parameter or named alias is being used there

while the mover supplies what "here" means.

### B4a. Descriptor meaning should prefer numeric Core3 structure.

Where practical, selector and transform descriptors should be represented by
small Core3-shaped numeric values rather than only by enum ordinals.

The reason is not aesthetics alone. A numeric descriptor:

- can be serialized as actual structure
- can be traversed and perturbed
- can admit nearby nonstandard variants
- can later participate in tension-style adjustment

Enums may still exist as convenience aliases, but they should not be the only
canonical meaning carrier when a small Core3 value can do that job.

### B5. Coupling is real structure reduction.

If one slot inherits or mirrors another slot by rule, that is not merely
display convenience.

It reduces degrees of freedom and should be represented explicitly as a binding
constraint.

Examples:

- copy a sibling unit magnitude
- preserve magnitude while inverting direction
- orthogonalize a borrowed carrier
- default a missing value from the active frame

### B6. Resolution order must be explicit.

The current recommended order is:

1. fill missing slots from bindings and defaults
2. apply coupling transforms
3. materialize a full engine-shaped value
4. if needed, read or commit that value into the active frame

This keeps frame inheritance and later calibration distinct.

### B7. Materialization should prefer immutable derivation.

Binding should usually materialize:

- once per context derivation
- once per execution step
- or once per explicit read

and then produce a new object.

Hidden mutable caches that silently change when frames change should be avoided
until there is a strong performance reason to add them.

### B8. Operations are attached, not embedded.

Operation laws should remain separate from:

- pins
- carriers
- boundaries
- containers or tokens

The current recommendation is to attach operations at sites rather than storing
the law as an intrinsic property of the structural object itself.

### B9. Site kinds are structural.

The current natural site classes are:

- pin sites
- carrier sites
- boundary sites
- token sites

Pins are natural for branch and join encounters.
Carriers are natural for transport, transform, and accumulation.
Boundaries are natural for continuation decisions.
Tokens are natural for local state updates.

### B10. Input binding decomposes into domain, address, and projection.

This is the current recommended selection grammar.

- domain
  Which pool are we reading from?
- address
  Which local numeric parameter or named alias is being used?
- projection
  What aspect do we take?

The current pass assumes that the active mover and domain decide how a numeric
parameter is interpreted:

- snap to discrete slot
- choose nearest
- interpolate along a path
- apply boundary or continuation law

Richer attention-style miss handling is left for a later layer.

An optional storage target can retain the selected value for later use.

### B11. Storage should be explicit.

Selected or computed values may be stored into:

- token slots
- local context slots
- result slots
- history
- transient scratch

This should remain explicit in the schema rather than hidden inside an
operation.

### B12. Routing may be interpreted from structured results.

The long-term aim is that routing need not be encoded only as fixed enum
commands.

An operation result may itself carry directional or branching structure whose
later interpretation determines whether execution:

- continues
- branches
- rejoins
- terminates

The binding layer should therefore describe inputs and outputs clearly while
leaving room for later route-interpretation laws.

### B13. Machines should remain scrubbable.

Even when execution is optimized, a machine should remain conceptually
traversable from start to end.

This supports:

- debugging
- explanation
- visualization
- ornamental and glyph-like rendering
- language-like dynamic interpretation

## Current Conceptual Schema

The first experimental schema is intentionally small.

### `BindingSelector`

Describes how a value is chosen from context:

- `BindingDomain`
- `BindingAddress`
- `BindingProjection`
- optional `BindingStorageTarget`

Common projections may still be given friendly names, but the current direction
is that the projection's underlying meaning should be a Core3 numeric signal.

`BindingAddress` is currently being simplified toward:

- numeric mover-relative position
- named alias

instead of many partly overlapping address kinds.

### `BoundTemplate`

Represents a partially specified runtime literal that can later bind into a
full engine-shaped value.

Current forms:

- `BoundScalarTemplate`
- `BoundCompositeTemplate`

These are templates, not serialized engine elements.

Slots inside these templates are represented by `BoundSlot<T>` so literal value,
context binding, and transform signal can all be adjusted within one repeated
shape rather than being split into unrelated properties.

### `BindingConstraint`

Represents explicit coupling between one target path and one selected source.

This is the first-pass place to express:

- sibling coupling
- inherited units
- opposite or orthogonal borrowing
- frame defaults

### `OperationAttachment`

Describes one law attached to one site together with:

- named input bindings
- named output bindings

This keeps structure, data, and executable law separate while still making
their contact points explicit.

### `TraversalMachineDefinition`

Represents one small declarative machine made from:

- one exact traversal mover
- named traversal registers
- one entry site name
- a list of attached operation laws

This is still definition-time structure rather than a full execution runtime,
but it provides a convenient serializable bundle for inspecting loop shapes,
branch shapes, and later visualizing flow.

### `TraversalMover`

Represents one physical cursor/trolley carried by one exact atomic iterator:

- `Position`

The numerator is the current route position.
The denominator is the current route end/resolution.

The current pass uses a default continuation of `+1` on the numerator until the
denominator is reached, then stops with clamp-like endpoint behavior.

## Numeric Descriptor Direction

The current experiment uses small axis-like numeric signals for:

- `BindingTransform`
- `BindingProjection`

These signals are currently stored as grade-2 axis-like engine values rather
than enum ordinals.

This is the current preferred direction because it keeps descriptor meaning:

- serializable as Core3 structure
- adjustable by nearby numeric variation
- open to nonstandard but still lawful cases

and therefore more compatible with later tension-driven exploration than a
closed enum list would be.

## Traversal-Relative Note

See [TraversalBinding.md](TraversalBinding.md) for the
current note on physical movers, trolley-style traversal, and mover-relative
numeric addressing.

See [RouteAndMover.md](RouteAndMover.md) for the current route/site/mover/read
picture across the current Core3 engine, binding, and runtime layers.

## Examples

### Frame-local data literal

A family member may provide:

- value `25`
- unit unspecified

and bind the missing unit from the active frame rather than pretending the
literal is already a standalone engine value.

### Coupled axis-like child units

One child may specify its unit explicitly while the opposing child binds its
unit from the first by an opposite-orientation transform.

That is not just shorthand. It records that the two child units are coupled by
rule and therefore do not vary independently.

### Sum loop

One operation attachment at a carrier site may read:

- accumulator slot from the token
- current family member from the active frame

and store the result back into the accumulator slot.

A separate boundary or continuation site can decide whether the loop continues.

### Fibonacci

One operation attachment may read token slots `a` and `b`, compute `next`,
then store:

- old `b` into slot `a`
- `next` into slot `b`

The machine can remain structurally simple while the input/output binding
describes how results become later inputs.

## Present Recommendation

For now:

- keep engine values fully specified
- let bound literals and coupling live in the binding layer
- use `null` for missing binding-time slots rather than overloading engine
  zero-unit meaning
- prefer small Core3 numeric descriptor values over enum ordinals where that
  meaning can be expressed structurally
- keep operation laws attached to sites, not embedded in the structural objects
- let the binding layer describe how context is read and where results are
  retained

This should give `Core3` a good place to explore:

- contextual defaults
- coupled slots
- machine-style input/output binding
- later attention-like selection

without having to commit the engine core to those decisions yet.
