# Core 2 Elements

This folder contains the native structural objects of Core 2.

If someone wants to understand what Core 2 fundamentally manipulates, this is one of the first folders to read after `Axioms`.

What makes this element system different from a traditional math or geometry library is that its primitives are not trying to be neutral containers for already-solved numbers. Core 2 treats many things that are usually thrown away as part of the object itself: direction, perspective, support or resolution, pinning intent, unresolved tension, and later folding. A value here is often not "just a number." It is a structured reading that still remembers how it is situated and what distinctions are being preserved.

This also means the elements are designed for more than conventional computation. They are meant to support concepts, language, and partial interpretation over time. Resolution can vary without collapsing to one scalar too early. Perspective can flip without changing the underlying thing. Pinning can describe local structural relation before a final folded result is chosen. And the dimensional chain from proportion to axis to area, and later beyond, is meant to carry richer structure rather than immediately flattening everything back to a single measure.

## What Lives Here

This is where the core element types and local pinning structures are defined.

That includes ideas like:

- scalar and proportion
- axis and area
- perspective
- pinning and pinned pairs
- carrier identity and pin sites
- local pin resolution descriptors

## Mental Model

Read this folder as the native object model of the system.

Very roughly:

- `Axioms` says what the system claims
- `Elements` says what the native objects are
- `Algebra` says how they act

In simple terms:

- `Proportion` is not only a fraction-like value; it also carries support or resolution
- `Axis` is not only a directed scalar; it carries the dual dominant/recessive reading
- `Area` is not just "more multiplication"; it is the next structural lift in the chain
- pinning is the local relation between structures
- folding is the later act of collapsing or enacting that relation

That separation is important. A traditional system often wants one immediate answer. Core 2 often wants to preserve what is being related, what is unresolved, and how a later interpretation should be chosen.

## Important Bias

These types are not meant to be generic geometry wrappers or ordinary numeric containers. They carry Core 2 structure directly, including:

- support and variable resolution
- directional duality rather than one flat sign convention
- host-relative readings
- pinning before folding
- dominant/recessive roles
- carrier and pinning meaning
- unresolved or tension-bearing states

Another important bias is that the dimensional chain is meant to stay structural. The current code begins with scalar, proportion, axis, and area, but the design direction is broader than that. The long-term goal is not a grab bag of unrelated shape types. It is a graded family of increasingly structured elements where higher-dimensional forms grow naturally out of the same core logic.

## Scope

If a concept feels like a primitive or near-primitive Core 2 object, it probably belongs here. If it is mainly a higher-level symbolic, display, or interpretation concern, it probably belongs in a higher layer.
