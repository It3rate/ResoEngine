# Pinning Axis Implementation Plan

This is a rough migration plan for making pinning a first-class Core 2 mechanism driven by an `Axis` descriptor rather than by enum-only relation tags.

## Goal

Treat an `Axis` as the canonical descriptor of a binary pin:

- recessive unit
- recessive value
- dominant unit
- dominant value

and derive pin behavior from that structure.

The current `PinRelation` enums then become:

- compatibility aliases
- display summaries
- fallback defaults

rather than the long-term source of truth.

## Design Principles

1. Keep pinning binary.
2. Preserve the existing positive-positive pin as the default straight segment case.
3. Distinguish unit-sign from value-sign.
4. Treat negative or zero units as tension-bearing structure, not normalization errors.
5. Resolve contradiction by the smallest lawful lift available at the current grade.
6. Keep folding separate from pin structure.
7. Migrate through compatibility layers so existing tests and displays keep working.

## Recommended Phases

### Phase 1. Add A Pin Interpretation Layer

Introduce a small interpretation layer that reads an `Axis` as a pin descriptor.

Likely new types:

- `PinAxisInterpreter`
- `PinDescriptorView`
- `PinSideDescriptor`
- `PinSideResolution`
- `PinResolutionResult`
- `PinLiftKind`

Responsibilities:

- expose the four slots of the pin descriptor
- determine each side's native carrier
- distinguish unit-sign from value-sign
- preserve zero-unit uncertainty
- report tensions instead of forcing immediate collapse

This phase should not change the existing math behavior yet.

### Phase 2. Make Relation Derived Instead Of Primary

Refactor the current relation layer so that:

- `PinRelation` is a derived summary of a resolved pin
- enum values remain for compatibility and display
- relation is no longer the only stored source of meaning

Likely affected files:

- [PinRelation.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinRelation.cs)
- [PinRelationMode.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinRelationMode.cs)
- [PinPolarityMode.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinPolarityMode.cs)
- [PinHandednessMode.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinHandednessMode.cs)
- [PinContactMode.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinContactMode.cs)

The likely outcome is:

- keep these for now
- stop treating them as first principles
- derive them from the interpreted pin axis

### Phase 3. Teach Proportion-to-Proportion Pinning To Use The Descriptor

Extend proportion pinning so that the current `Axis` is one resolved case of a richer pin.

Likely affected files:

- [Proportion.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/Proportion.cs)
- [Axis.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/Axis.cs)
- [PinnedPair.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinnedPair.cs)

Likely additions:

- an overload or helper such as `Pin(Proportion other, Axis descriptor)`
- a resolved representation that distinguishes:
  - collinear-opposed
  - collinear-same
  - orthogonal breakout
  - unresolved / noisy carrier

At this stage the default positive-positive descriptor should still produce the existing segment behavior.

### Phase 4. Teach Axis-to-Axis Pinning To Use The Same Logic

Apply the same binary interpretation law at the next grade.

Likely affected files:

- [Axis.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/Axis.cs)
- [Area.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/Area.cs)
- any future `Volume` wrapper

Key idea:

- the preferred breakout direction should be driven by nesting depth or current grade
- a contradiction that could not stay in the first line may move into the plane
- a contradiction at the next grade may leave the plane

This phase is where `Area` should stop meaning only one hard-coded orthogonal enum case and instead become a named view of a resolved pin.

### Phase 5. Add Landmark Pinning

Allow pins away from zero to act as local relation changes.

Initial uses:

- polyline vertices
- branch landmarks
- local continuation changes
- future curve controls

This phase mostly affects Applied geometry later, but the Core 2 pin model should support it from the start.

### Phase 6. Add Volume Carefully

Once `Axis` and `Area` pinning both derive from the same mechanism, add `Volume` as a named wrapper over a higher-grade binary pin such as:

- `Area pin Area`
- `Area pin Axis`

Do not force the fold law too early.
First make the structure representable, inspectable, and drawable.

## Suggested File Additions

- [PinAxisInterpreter.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinAxisInterpreter.cs)
- [PinDescriptorView.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinDescriptorView.cs)
- [PinSideDescriptor.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinSideDescriptor.cs)
- [PinSideResolution.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinSideResolution.cs)
- [PinResolutionResult.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinResolutionResult.cs)
- [PinLiftKind.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/PinLiftKind.cs)

## Suggested Test Areas

- positive-positive descriptor resolves to the current segment unit
- same-direction resolution yields reinforcement / acceleration-like behavior
- negative unit produces contradiction instead of silent sign normalization
- zero unit preserves unresolved carrier information
- nesting level changes the preferred breakout direction
- relation summaries remain compatible with current displays

Likely test files:

- [PinningTests.cs](/Users/Robin/source/repos/ResoEngine/Tests.Core2/PinningTests.cs)
- [AxisTests.cs](/Users/Robin/source/repos/ResoEngine/Tests.Core2/AxisTests.cs)
- [AreaTests.cs](/Users/Robin/source/repos/ResoEngine/Tests.Core2/AreaTests.cs)

## Open Questions

1. Should the pin descriptor be the existing `Axis` directly, or a dedicated wrapper over `Axis`?
2. Should zero-unit behavior mean uncertainty, stochastic spread, unresolved noise, or a tagged family of those cases?
3. When same-direction pinning yields acceleration-like continuation, should the result stay structural, or fold immediately to a higher-order rate interpretation?
4. Should orthogonal breakout always be geometric first, or should phase and glide be available as equal first-class lifts?
5. How much of the current `Axis` multiplication table should remain hard-coded once relation is descriptor-driven?

## Short Recommendation

Use the current enum-based relation system as a compatibility shell while moving the true meaning of pinning into an `Axis`-driven interpretation layer.
That should let us preserve current behavior, replace the enum source-of-truth gradually, and prepare cleanly for `Volume`, landmarks, and later curve/surface work.
