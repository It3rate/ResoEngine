# Route And Mover

This note records the current Core3 direction for route-like structure and
located traversal.

The immediate aim is not to finish execution semantics. It is to make one
small thing clear:

- route-like structure should stay physical
- the mover should be a real located object
- movement should be an exact equation over time

This note intentionally ignores the named `Elements` folder. It is about the
current engine/runtime/binding stack.

## Core Route Picture

The current working picture has six roles.

- route
  A traversable whole with a beginning, an end, and possible intermediate
  landmarks or branches
- site
  A located encounter on that route
- mover
  A real cursor/trolley/container whose current position is part of the state
- read
  A perspective from one site or mover position into local or nonlocal data
- law
  An attached operation or continuation rule enacted at a site
- payload
  Registers, bound data, family members, history, or result slots carried or
  consulted during traversal

These are related, but they should not be flattened into one object.

## Route

A route is fundamental.

For the current experiment, the route is not yet a standalone engine type.
Instead it is implied by:

- a `TraversalMachineDefinition`
- its named sites
- its attached laws
- its mover's current iterator position

That is enough to begin treating a machine as one traversable whole.

Longer term, a route may be:

- a simple carrier
- a branching flow
- an equation graph
- a family traversal
- a more general graph of pins, carriers, and joins

The important point is that the whole can still be viewed as one path-like
thing with its own local origin and extent.

## Mover

The mover is the present way to make "current position" physical.

`TraversalMover` is intentionally small:

- `Position`

The iterator is one exact atomic element:

- the numerator is the current route position
- the denominator is the route end/resolution

So:

- `0/4` means start of a four-step traversal
- `3/4` means the third step of that same traversal
- `20/100` means 20% of the way through a hundred-step traversal

This means movement is not mysterious. The trolley is just a very small exact
equation over time.

The current pass uses the built-in continuation:

`position := position + 1`

repeated until the denominator is reached.

That is deliberately small. A later pass can lift this into richer equation or
repetition structure once that runtime exists.

That supports several useful readings:

- `0/1` is whole-route start in one-step resolution
- `0/4`, `1/4`, `2/4`, `3/4`, `4/4` is quarter-resolution traversal
- `20/100` starts partway through a finer traversal
- a branch family later means several movers or several route continuations,
  not a hidden nonphysical "current selector"

The present continuation is therefore equivalent to a clamp-like endpoint law:
once the numerator reaches the denominator, the mover stops advancing.

## Sites

Pins, boundaries, carrier landmarks, and operation placements all rhyme as
located sites on a route.

The current binding layer still keeps some of these as separate records:

- `OperationSite`
- `EnginePin`
- `EngineBoundary`

That separation is still useful, but conceptually they belong to one family of
located encounters.

The important distinction is:

- a pin creates or exposes local structure
- a boundary summarizes endpoint encounter structure
- an operation attachment places a law at a site
- a reference reads from a site or frame without creating local structure

## Reads

`EngineReference` is best read as a lens or viewpoint, not as a mover.

A reference says:

- use this frame or site as calibration
- read that subject through it

In a route-oriented picture, that means references are the observational layer
for traversal.

The mover tells us where "here" is.
The reference tells us how to look from there.

## Laws

Operations remain attached, not embedded.

That means:

- the route provides sites
- the mover supplies the present encounter
- selectors bind inputs relative to the current situation
- attached laws transform values, route signals, or stored payloads

This keeps structure, reads, and execution distinct even when they meet at the
same location.

## Payload

Registers and family/context/history reads are the carried or consulted data.

The current machine experiment uses:

- `TraversalRegister`
- `BindingSelector`
- `BoundTemplate`

to describe payload shape and how it is read or written.

This is still declarative, but it is enough to make a loop machine inspectable.

## Present Recommendation

For the current Core3 pass:

- keep route meaning physical
- treat current position as a real mover state, not as a virtual selector mode
- express motion as an exact Core3 iterator/equation over time
- treat pins as structural landmarks
- treat references as reads from a located perspective
- treat operation attachments as laws placed at sites
- let branching later produce multiple route continuations or multiple movers

This gives a cleaner bridge between:

- simple number traversal
- family loops
- branch and rejoin flow
- equation-like execution
- later visualization as a graph or drawing
