# Core 2 Interpretation Traversal

This folder contains interpreted traversal and encountered-pin behavior.

Traversal here is not a memoryless cursor moving over plain coordinates. It is stateful continuation over Core 2 carriers, with located pins, boundary encounters, reversals, tension, and later commitment.

## What Lives Here

This area handles things like:

- axis traversal definitions and state
- traversal stepping
- boundary-pin continuation support
- located pins
- boundary pin pairs
- pin-aware traversal behavior

## Mental Model

If `Core2/Repetition` defines continuation law in the abstract, this folder shows what continuation looks like when a carrier is actually being traversed and encountered structure matters.

This is where "continue," "reflect," "wrap," and related behaviors become concrete traversals through interpreted Core 2 structure.
