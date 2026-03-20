# Positioned Pinning Proposal

This document is a focused proposal for one specific extension of the newer pinning model:

- pinning as the deeper operation
- `Axis` as the intrinsic local descriptor carried by pinned operands
- proportional location of the pin on a host object
- a consistent host/applied asymmetry for expressions such as `A * B @ P`

It is intended to sit beside, not replace, the broader pinning documents:

- [PinningProposal.md](./PinningProposal.md)
- [PinsAsDynamicLocalEvents.md](./PinsAsDynamicLocalEvents.md)

This file is a proposal, not final doctrine.
It tries to capture the direction of the system before the next major code integration.

## Core Claim

The central claim of this proposal is:

- pinning is itself a primitive operation
- point pinning creates a local placed relation
- folding is a later interpretation of the pinned structure
- folding may enact distributed pinning across a support
- the location of a pin matters
- the pinned operands carry their own intrinsic local frames
- the pinned operands carry their own `Axis` descriptors
- pinning aligns those local structures rather than inventing bend from outside
- dominant directions line up with dominant directions at the pinning site
- the sign of the descriptor gives structural type
- the magnitude of the descriptor gives weighted participation or strength

In this reading, an expression like:

- `A * B @ P`

means:

- take `A` as the host
- locate point `P` on `A`
- attach the local zero of `B` there by aligning host and applied dominant directions
- interpret the pin using the intrinsic descriptors already carried by `A` and `B`
- fold the resulting pinned structure into whatever reading is being requested

## Primitive Terms

### Host

The host is the object being pinned onto.
It provides the carrier, frame, and local place of attachment for the pinning event.

### Applied element

The applied element is the object being pinned onto the host.
It contributes its own local descriptor and directional proposal.

### Pinning event

A pinning event is the local operation by which a host and an applied element are aligned and joined at some location.

### Pin descriptor

The current canonical descriptor carried by a pinned operand is `Axis`.
Its four slots are:

- recessive unit
- recessive value
- dominant unit
- dominant value

These are often abbreviated as:

- `i`
- `iV`
- `u`
- `uV`

### Pin position

The pin position is the location on the host at which the applied element is attached.

### Local zero

The local zero of the applied element is the point of that element used as the default attachment point when pinning occurs.

### Intrinsic local frame

Each pinned object carries its own intrinsic local frame.
Its units and values describe extension relative to that local frame before any later embedding.

### Point pinning

Point pinning is one local attachment event at one host-relative position.
It is the simplest potential form of pinning.

### Distributed pinning

Distributed pinning is a family of local pinning events across a support, span, or piece family.
It is often the enacted form of a fold such as multiplication, boolean comparison, or other support-wide combination.

### Dominant-direction alignment

Pinning aligns dominant direction with dominant direction at the attachment site.
Negative or bent units remain negative or bent relative to that aligned local carrier.

### Carrier

A carrier is the path, line, sheet, or other structural support along which realization may occur.

### Fold

A fold is a later interpretation of pinned structure into a simpler or task-specific result.

### Encounter

An encounter is the local meeting of:

- a current continuation or state
- a pin
- a host carrier
- surrounding tension
- local frame conditions

### Transport

Transport is realized directional passage on a carrier.

### Tension

Tension is preserved unresolved contradiction, opposition, overload, or unrealized potential.

## Part I. Pinning As Operation

### PP1. Pinning Is Primitive

Pinning is not merely a display aid or a later annotation on a result.
It is a primitive structural operation.

### PP2. Pinning Precedes Folding

The direct result of a point pinning event is pinned structure.
Any number-like, geometric, boolean, or dynamic result appears only after folding.

### PP3. `A * B @ P` Is A Pinning Expression

The expression `A * B @ P` should be read first as a pinning event rather than immediately as ordinary scalar multiplication.

### PP4. The Left Operand Is The Host

In `A * B @ P`, the left operand `A` is the host object.
It is the object on which the pinning event is located.

### PP5. The Right Operand Is The Applied Element

In `A * B @ P`, the right operand `B` is the applied element.
It supplies its own local descriptor and acts on the host at the pin location.

### PP6. The Pin Position Belongs To The Host

The location `P` is read in the frame and support of the host.
It is not primarily a coordinate of the applied element.

### PP7. Default Attachment Uses The Applied Local Zero

Unless otherwise stated, the default pinning anchor attaches the local zero of the applied element to the host point `P`.

### PP8. Origin Pinning Is Only A Special Case

Pinning at `0` is only the simplest special case of pinning.
It should not be treated as the only meaningful form of pinning.

### PP9. Pinning May Occur At Interior, Endpoint, Or Exterior Positions

The host position `P` may lie:

- at the start
- at the end
- at an interior point
- beyond the currently realized span

Each case may change the meaning of the pinning event.

### PP10. Pinning Is Locally Binary

A pinning event locally joins exactly two structural roles:

- the host role
- the applied role

More complex structure arises through nested or sequenced pinning events.

### PP11. Point Pinning And Distributed Pinning Are One Mechanism At Different Extents

Point pinning and distributed pinning should be treated as one mechanism acting at different extents.
The difference is not primitive kind, but how broadly the pinning relation is enacted.

## Part II. Host, Applied, Recessive, And Dominant

### PP12. Host Is Canonically Recessive

The host role is canonically recessive.
It is the substrate on which the local event is placed.

### PP13. Applied Is Canonically Dominant

The applied role is canonically dominant.
It contributes the directional or structural directive of the local event.

### PP14. Each Operand Carries An Intrinsic Local Frame

Both host and applied operands may already be straight, bent, inverted, or unresolved before pinning.
Those properties belong to the operands themselves, not to the pin as an added bend-generator.

### PP15. Pinning Aligns Dominant With Dominant

When two objects are pinned, they are aligned through their dominant directions at the attachment site.
Their recessive and orthogonal consequences then follow from their own intrinsic descriptors.

### PP16. Dominance Tracks Directive, Not Mere Persistence

Dominance should track the actively applied directive rather than simply the object that appears to remain visible afterward.

### PP17. Persistence And Dominance Need Not Coincide

The host may remain the larger surviving carrier while the applied element remains the dominant local directive.
These two asymmetries should not be collapsed together.

### PP18. This Matches State-Transform Asymmetry

The host/applied distinction aligns with the broader Core 2 asymmetry:

- left as current substrate or state
- right as acting or directive side

### PP19. Rightward Dominance Remains The Chosen Convention

As elsewhere in Core 2, the dominant role is a chosen convention and should be stated explicitly rather than left implicit.

## Part III. Proportional Positioning

### PP20. Line-Like Hosts Use Proportional Position

For line-like hosts, the pin position `P` should normally be represented by `Proportion`.

### PP21. Proportional Position Is Host-Relative

The proportion used for `P` is measured in the host's frame and support, not in a detached neutral frame.

### PP22. Position Is Rational Before Folding

Before folding, the pin position should remain an exact proportional location whenever possible rather than collapsing immediately to scalar form.

### PP23. Position May Carry Resolution

Because `P` is proportional, it can preserve support, tick resolution, and relative location rather than only a collapsed decimal distance.

### PP24. Position And Scale Differ

The pin position says where pinning occurs on the host.
It should not be confused with the magnitude of the descriptor values, which say how strongly each pin side participates.

### PP25. Position May Later Participate In Affine Interpretations

A pin position may later contribute to offset-like or bias-like interpretations, but this is a later fold or adapter reading rather than the primitive meaning of the pin position itself.

### PP26. Higher-Degree Hosts Need Higher-Degree Locators

If the host is not line-like, the locator for `P` should eventually rise with the host.
For example:

- a line host may use `Proportion`
- an area host may require an `Axis`-like coordinate
- higher hosts may require richer locators

## Part IV. The Axis Inside A Pin

### PP27. `Axis` Is The Current Canonical Intrinsic Descriptor

The existing `Axis` is the current best descriptor carried by a pinned operand.
It is not required to belong to the pin as a separate extra object.

### PP28. The Four Slots Remain Distinct

The four slots of the descriptor remain distinct:

- recessive unit
- recessive value
- dominant unit
- dominant value

### PP29. Unit Describes Carrier Preference Relative To The Current Carrier

The unit part of a pin side describes what kind of carrier that side prefers, contradicts, or leaves unresolved.

### PP30. Value Describes Realized Travel

The value part of a pin side describes how much realized travel or enactment that side contributes on its chosen carrier.

### PP31. Sign Gives Structural Type

The sign of the unit and value gives the coarse structural type of the pin side:

- positive unit: preserve native carrier
- negative unit: contradict native carrier and prefer orthogonal lift relative to the current carrier
- zero unit: carrier unresolved
- positive value: realize with carrier orientation
- negative value: realize against carrier orientation
- zero value: realized travel not yet committed

### PP32. Bend Is Intrinsic To The Operand

A negative unit already bends an operand relative to its intrinsic current carrier before later pinning.
Pinning does not have to invent that bend from outside.

### PP33. Magnitude Gives Weighted Participation

The magnitude of the descriptor should be read as weighted participation or local strength rather than as a new primitive kind of pin.

### PP34. Magnitude Is Not Just Decoration

Values beyond `-1`, `0`, and `1` should not be treated as meaningless extras.
They indicate stronger, weaker, or more weighted local participation.

### PP35. Magnitude May Express Transmission Strength

A larger same-direction magnitude may indicate stronger transmission, reinforcement, or gain-like continuation.

### PP36. Magnitude May Express Opposition Strength

A larger opposed magnitude may indicate stronger opposition, blockage, backpressure, or reflection potential.

### PP37. Magnitude May Express Branch Strength

A larger orthogonal magnitude may indicate stronger branch emission, redirection, or side-channel transport.

### PP38. Magnitude May Express Held Potential

If realization is incomplete or unresolved, magnitude may indicate stored or latent transport potential rather than visible motion.

### PP39. Magnitude Should Not Yet Be Over-Interpreted

Although magnitudes may later support adapter, bias, gain, or calibration readings, those should be treated as later folds rather than as the primitive meaning of the pin descriptor.

## Part V. Local Resolutions Of A Pin

### PP40. Opposed Collinear Resolution Yields Directed Segment Structure

If the two resolved pin sides occupy one carrier and point against each other, the local structure is directed-segment-like.

### PP41. Same-Direction Collinear Resolution Yields Reinforcement

If the two resolved pin sides occupy one carrier and point in the same direction, the local structure is reinforcement-like or acceleration-like.

### PP42. Orthogonal Resolution Yields Corner Or Area-Like Structure

If the two resolved pin sides occupy perpendicular carriers, the local structure is corner-like, area-like, or branch-like.

### PP43. Zero Unit Preserves Carrier Ambiguity

A zero unit preserves unresolved carrier choice and may therefore produce noisy or ambiguous directional display.

### PP44. Zero Value Preserves Realization Ambiguity

A zero value means travel has not been committed on that side, even if the carrier preference is already known.

### PP45. One-Sided Resolution Is Lawful

One side of a pin may resolve while the other remains noisy, latent, or unresolved.

### PP46. Unresolved Sides Must Not Erase Resolved Sides

If one side remains unresolved, the resolved side should still remain visible and meaningful.

## Part VI. Position Changes Meaning

### PP47. Endpoint Pinning Is Not The Same As Midpoint Pinning

The same descriptor axis at an endpoint and at an interior point should not automatically mean the same thing.

### PP48. Endpoint Context Suggests Boundary Readings

When pinning occurs at a host endpoint, the event may naturally read as:

- reflection
- clamp
- wrap
- open termination
- loop capture

depending on descriptor and context.

### PP49. Interior Context Suggests Landmark Readings

When pinning occurs at an interior host point, the event may naturally read as:

- branch
- turn
- reinforcement
- blockage
- reversal
- local redirection

depending on descriptor and context.

### PP50. Open Endpoint Differs From Clamp

If a host reaches an endpoint without an explicit resolving pin, this should not automatically be read as clamp.
It is open termination and raises tension.

### PP51. Reflection Is A Reading, Not The Only Opposition Outcome

Opposition at or near an endpoint may lead to reflection, but opposition itself is more primitive than reflection.
Other lawful outcomes include clamp, stored tension, dissipation, or redirection.

### PP52. The Same Descriptor May Produce Different Readings In Different Locations

The descriptor expresses local desire.
Location, nearby structure, and existing flow determine the encounter reading.

## Part VII. Pinning And Folding

### PP53. Pinned Objects And Folded Objects Must Be Distinguished

The two objects being pinned, the pinning event itself, and the final folded result are three distinct layers and should not be collapsed prematurely.

### PP54. Folding Is Question-Dependent

The same pinned structure may be folded differently depending on what result is being asked for.

### PP55. Point Pinning Is Potential, Distributed Pinning Is Enactment

Point pinning gives the local potential relation at one site.
Distributed pinning is the enactment of that relation across a support, span, or piece family during folding.

### PP56. Multiplication May Be A Distributed Pinning Fold

What ordinary notation writes as multiplication may often be read more deeply as a pinning event whose folded result is multiplicative in character.

### PP57. Boolean Operations May Be Carrier-Wide Distributed Pinning Folds

Carrier-wide boolean relations such as `AND`, `OR`, `XOR`, and `NOT` may be understood as comparing or pinning entire supports and then folding the result into surviving piece families.

### PP58. Addition-Like Folds And Span-Like Folds Differ

Some folds read the pin as continuation, accumulation, perimeter, or path composition.
Other folds read the same pin as span, area, or higher-grade structure.

### PP59. The Pin Should Preserve Provenance Across Folds

A folded result should preserve where the pin occurred, what descriptor was used, and what host/applied asymmetry produced it whenever possible.

### PP60. Not Every Pin Needs Immediate Collapse

A pinned structure may remain partially unfolded if no single fold is yet justified.

## Part VIII. Working Interpretation

### PP61. Pinning Is The Local Answer To "How Do These Meet Here?"

The most basic conceptual meaning of a pin is:

- this is where two things meet
- this is how each side wants to continue from that meeting

### PP62. The Operand Axes Are Local Instruction Cards

The `Axis` carried by each operand should be understood as a local instruction card for the join.
It says what kind of carriers that operand wants and how much directed realization it wants on those carriers.

### PP63. The Host Carries Location, The Applied Element Carries Directive

The host gives the place and substrate of the event.
The applied element gives the local directive of the event.

### PP64. Pinned Structure Is Deeper Than Display

The drawn line, corner, area, branch, or overlap seen later is a display or folded reading of a deeper pinned structure.

### PP65. Exactness Requires Layering

To remain exact, the system should distinguish:

- the host and applied operands
- the pin position
- the intrinsic descriptors already carried by the operands
- the encounter context
- the folded interpretation

and should not flatten these into one prematurely simplified object.

## Short Examples

### Example 1. Origin pinning

`A * B @ 0`

means:

- pin `B` onto `A`
- at the host origin
- using the local zero of `B`
- by aligning dominant direction with dominant direction

### Example 2. Midpoint branch

`A * B @ 1/2`

with an orthogonal descriptor in `B` means:

- the host `A` is pinned at its midpoint
- the applied element `B` requests orthogonal local structure there
- a branch or corner-like fold may result

If `A` was already bent, that bend remains intrinsic to `A` and is not erased by the pinning.

### Example 3. Endpoint opposition

`A * B @ 1`

with opposing local flow in `B` means:

- the host end is encountered
- the applied pin expresses opposition there
- a reflection, clamp, stored tension, or redirected fold may result depending on context

### Example 4. Carrier-wide boolean comparison

`A XOR B`

may be read as:

- compare the supports of `A` and `B`
- enact a distributed pinning across those supports
- preserve the surviving pieces
- fold them into a co-present structural family

The boolean result is therefore a fold of a broader relational encounter, not merely a truth-table fact detached from support.

## Final Proposal Statement

This proposal recommends that Core 2 adopt the following interpretation:

- pinning is a primitive local operation
- `A * B @ P` means pin the dominant applied element `B` onto the recessive host `A` at host-relative position `P`
- both `A` and `B` may already carry intrinsic bend, inversion, or unresolved orientation
- pinning aligns dominant direction with dominant direction at the attachment site
- `P` should normally be exact and proportional on line-like hosts
- the operands' intrinsic `Axis` descriptors describe carrier preference and realized travel
- sign gives structural type, magnitude gives weighted participation
- the direct output of pinning is pinned structure, not yet a collapsed result
- point pinning gives local potential
- folding may enact distributed pinning across support
- multiplication, addition-like continuation, boolean splitting, span, and other outcomes are folds of that deeper pinned structure

That interpretation seems to preserve the current direction of Core 2 while clarifying how pinning, proportional location, and later folding should relate.
