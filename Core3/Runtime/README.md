# Core3 Runtime Notes

This note describes where the runtime layer fits inside `Core3`.

It is intentionally short and structural.

## Three Layers

`Core3` is currently easiest to understand as three cooperating layers:

1. Engine core
   The serialized mathematical substrate.

2. Runtime layer
   The temporary framed context in which engine elements are read, grouped,
   reordered, focused, compared, and returned with provenance.

3. Operation layer
   The concrete laws that act on runtime contexts, such as addition,
   multiplication, boolean occupancy, and later equations.

## Engine Core

The engine core is the durable mathematical ontology.

This includes things like:

- `GradedElement`
- `AtomicElement`
- `CompositeElement`
- `EnginePin`
- `EngineView`
- `EngineBoundary`

These are the things that should remain closest to serialization and the
smallest stable mathematical description of the system.

The engine core should avoid taking on temporary UI, traversal, authoring, or
operation-session state unless that state is truly part of the mathematical
object itself.

## Runtime Layer

The runtime layer is where a stable engine object is placed into temporary
operational context.

This includes things like:

- a frame role
- a family of members read in that frame
- ordered or unordered family interpretation
- temporary promotion of one member into frame role
- collapse back into a parent frame
- provenance about which members contributed to a result piece

The runtime layer is not a second mathematical ontology beside the engine core.
It is a contextual layer used when the engine is being *run*.

That means the runtime layer is the right place for:

- operation setup
- temporary reframing
- grouping and ordering hints
- derived reads
- result provenance

But it is not the right place for replacing the engine's actual element space.
A runtime result should still remain an ordinary `GradedElement` whenever
possible.

In the current API direction, the intended flow is:

1. build or derive an `OperationContext`
2. run an operation against that context
3. inspect the resulting element and its provenance
4. optionally use that result as the basis of a new context

The current helper surface on `OperationContext` is intentionally small.
It exposes a few significant derived-context operations such as:

- ordered / unordered view changes
- focus into temporary frame role
- collapse back to parent frame
- sort by frame-read slot
- shuffled unordered copies

More convenience helpers can be added later without changing the underlying
runtime pattern.

## Operation Layer

The operation layer applies concrete laws to runtime contexts.

Examples:

- addition over a framed family
- multiplication over a framed family
- binary boolean comparison
- family occupancy predicates

The operation layer should be free to return both:

- an element result
- runtime provenance about how that result was obtained

So the runtime layer is the bridge between the durable engine core and the
concrete operation laws that run on it.

## Why This Split Matters

This separation helps with several things at once:

- engine objects stay small and mathematically stable
- runtime helpers can grow without pretending to be core ontology
- operations can become more expressive without bloating the core element model
- later serialization can focus on the engine core first, then decide how much
  runtime provenance should also be persisted

In short:

- engine core says what the object is
- runtime layer says how it is being used right now
- operation layer says what law is being applied to that runtime situation

That is the current intended fit of `Core3.Runtime`.

