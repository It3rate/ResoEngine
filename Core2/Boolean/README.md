# Core 2 Boolean

This folder contains Core 2 boolean behavior.

Boolean operations here are not treated as purely abstract truth tables detached from structure. They are interpreted in terms of occupied pieces, carriers, inherited direction, and active frames.

## What Makes It Different

In Core 2, boolean combination is often:

- frame-relative
- piece-producing
- carrier-aware
- support-preserving where possible

So a boolean result may be:

- one surviving piece
- several co-present pieces
- a result carried by one of the inputs
- a result carried by the frame itself

## Mental Model

Think of this folder as:

- truth on partitions
- occupancy over a shared frame
- projection back into Core 2 structure

not as a generic `bool` utility layer.

## Scope

This folder should hold the core boolean projection laws and related helpers. Richer symbolic boolean authoring and inspection live higher up in the symbolic layer.
