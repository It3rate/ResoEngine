# Core3 Serialization Notes

`Core3.Serialization` currently uses a manual JSON writer.

This is deliberate.

At the current stage of `Core3`, the structural shapes are still being refined.
Sprinkling serializer attributes across engine, runtime, binding, and operation
types would make the code harder to read while the on-wire format is still
moving.

So the present direction is:

- keep serialization logic centralized and explicit
- keep engine and runtime types free of serializer annotations
- make the JSON shape readable enough for inspection and debugging
- keep the traversal over objects explicit so a later binary writer can reuse
  the same structural walk

## Current Shape

The JSON writer currently handles:

- core graded elements
- pins
- references
- operation contexts and families
- operation results
- boolean piece results
- bound templates
- direct binding primitives such as addresses, storage targets, selectors,
  projections, transforms, signals, and sites
- traversal machine definitions and registers
- operation attachments

## Minimal Versus Derived

`Core3JsonSerializerOptions` currently exposes:

- `Indented`
- `IncludeDerived`

The idea is to keep a distinction between:

- the stored structural relation itself
- extra derived reads that are useful for debugging

For example:

- a reference can serialize just `frame` and `subject`
- or also derived `calibration`, `existingReadout`, and a current `read`

That helps expose how much of the serialized shape is fundamental structure and
how much is explanatory runtime elaboration.

## Why Manual JSON First

The immediate goal is not to lock in a final serializer framework.

The immediate goal is to see:

- what the Core3 objects look like on the wire
- how much structural wording they require
- where the true repeated patterns are

Because the writer is manual and local, it is also easier to later map the same
structural walk onto:

- a compact binary format
- a long-oriented numeric encoding
- a streaming or incremental writer

without first unwinding a web of attributes and implicit serializer rules.
