# Core3

`Core3` is an experimental support library growing out of `Core2`.

It is intended to stay small and concept-first while the next semantic layer is worked out more carefully. The current direction is:

- a basic math library for concepts rather than only for measurements
- explicit treatment of resolution and calibration as later structure
- dual orientation in the elements themselves rather than as a display afterthought
- clear separation between pinning and later folding or enacted geometry

The goal is to keep the core primitives very tight, then layer interpretation, traversal, and richer graph behavior on top only when the primitive meaning is stable.

One current working intuition is that every selected non-chaotic thing must carry at least one ordered feature, even if that feature is not yet calibrated or explicitly measured. That ordered feature is what makes the thing recognizable as a thing rather than as noise. `RawExtent` is the first placeholder for that kind of recognized-but-not-yet-processed structure.
