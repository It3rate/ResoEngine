# Core3

`Core3` is an experimental support library growing out of `Core2`.

It is intended to stay small and concept-first while the next semantic layer is worked out more carefully. The current direction is:

- a basic math library for concepts rather than only for measurements
- explicit treatment of resolution and calibration as later structure
- dual orientation in the elements themselves rather than as a display afterthought
- clear separation between pinning and later folding or enacted geometry

The goal is to keep the core primitives very tight, then layer interpretation, traversal, and richer graph behavior on top only when the primitive meaning is stable.
