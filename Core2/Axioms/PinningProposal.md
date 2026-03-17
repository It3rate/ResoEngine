# Pinning Proposal

This document is a provisional proposal for a broader pinning-centered rewrite of Core 2 behavior.
It is intentionally more thorough than the narrower appendix files:

- [PinningAxisRules.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PinningAxisRules.md)
- [PinsAsDynamicLocalEvents.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PinsAsDynamicLocalEvents.md)

It is not yet part of the consolidated axioms in:

- [PromptAxioms.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PromptAxioms.md)

Its purpose is to define a conceptual target for a later implementation and for eventual integration into the main Core 2 axioms.

## Status

This file is a proposal, not settled doctrine.
It intentionally reaches further than the current implementation.
Where it disagrees with current code, it should be read as a direction for refactoring rather than as a claim that the code already behaves this way.

## Core Intuition

The central claim of this proposal is:

- pinning is not a side feature of units
- pinning is not merely a static relation label
- pinning is a fundamental local mechanism by which structure is attached, emitted, redirected, interrupted, lifted, folded, branched, or rejoined

In this proposal, a pin is the local place where structure meets decision.

The current unit-level and axis-level pinning work is then understood as the simplest visible case of a more general mechanism.

## Relationship To Existing Core 2

This proposal attempts to preserve the spirit of the current axioms:

- the primitive object remains the ordered interval
- readings still arise from frame and perspective
- tension remains informative rather than an error to be erased
- lift and branch remain lawful responses to overload
- unit and value remain distinct
- folding and structure remain distinct

What changes is the place where many behaviors are considered primitive.

Under this proposal:

- wrap, reflect, continue, clamp, branch, corner, loop, and join are not primitive law names
- they are contextual readings of encountered pins

That is the deepest shift.

## Primitive Terms

The proposal uses the following terms.

### Pin

A pin is a localized structural event described by a binary descriptor and read in context.

### Pin descriptor

The current `Axis` is the canonical descriptor of a pin.
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

### Carrier

A carrier is the path, line, sheet, lifted direction, or other structure along which realization can occur.

### Native carrier

A native carrier is the carrier a side would occupy before contradiction, lift, or compromise alters it.

### Located pin

A pin must be locatable in some frame, carrier, sheet, or relation.
Location and attachment are not identical.

### Attachment

Attachment is structural joining.
A pin may be located without yet being attached.

### Encounter

An encounter is the local meeting of:

- a pin
- a current carrier or state
- surrounding tension
- local frame conditions

### Lift

A lift is a lawful increase in representational capacity used to preserve distinctions that the current carrier cannot hold.

### Sheet

A sheet is a place of orientation and interaction distinct from observer perspective.
Pins and carriers may live in or act across sheets.

### Tension

Tension is preserved unresolved information, contradiction, overload, or incompatibility that the system has not collapsed away.

## Part I. Foundational Pinning Claims

### P1. Pinning Is Fundamental

Pinning is a foundational structural mechanism of Core 2.
It is not merely an ornament on top of intervals, units, or geometry.

### P2. One Pin Mechanism

There should not be many primitive named pin types.
There is one pin mechanism whose meaning is derived from descriptor plus context.

### P3. Pinning Is Local

A pin is a local event, not a global law.
It is interpreted where it is encountered.

### P4. Pinning Is Binary

A pin is locally binary.
It joins, opposes, redirects, or relates two sides or two structural roles.

### P5. Higher Structures Arise By Nesting Or Sequencing

More complex structures arise from:

- nested pinning
- repeated pinning
- encountered pin sequences
- branch families of pin resolutions

and not from inventing fundamentally different multi-way pin primitives.

### P6. A Pin Is A Goal, Not A Guarantee

The descriptor within a pin expresses what local structure wants to happen.
It does not guarantee exact realization.

### P7. Pinning And Folding Differ

Pinning creates structural relation.
Folding is a later interpretation or collapse of that structure.

### P8. Pinning And Display Differ

A displayed shape is not the same thing as the structural pin event that generated it.

### P9. Pinning May Remain Unfolded

A pin may remain structurally valid and dynamically active without resolving to one unique folded reading.

### P10. Tension Must Be Preserved

Contradiction or ambiguity encountered in pinning should not be normalized away merely to recover a simpler display.

## Part II. The Axis As Pin Descriptor

### P11. Axis Is The Current Canonical Pin Descriptor

The existing `Axis` is the current best descriptor of a pin.
This does not erase its existing directed-segment meaning.

### P12. The Four Slots Remain Distinct

The four slots of the descriptor remain distinct:

- recessive unit
- recessive value
- dominant unit
- dominant value

### P13. Units Describe Carrier Preference

Unit slots describe native carrier preference, contradiction, or unresolved carrier assignment.

### P14. Values Describe Realization

Value slots describe extent, realized travel, or directional enactment on the chosen carrier.

### P15. Unit And Value Must Not Collapse Prematurely

Unit and value should not be collapsed into one sign or one scalar too early.
They carry different kinds of information.

### P16. Positive Unit Preserves Native Carrier

If a unit is positive, its side prefers to stay on its native carrier.

### P17. Negative Unit Contradicts Native Carrier

If a unit is negative, its side contradicts its native carrier assignment.
This contradiction must be preserved as meaningful structure.

### P18. Zero Unit Preserves Unresolved Carrier Information

A zero unit is not mere emptiness.
It may indicate:

- noise
- uncertainty
- indifference
- unresolved carrier choice
- latent structure
- held tension

### P19. Positive Value Realizes With Carrier Orientation

Once a carrier has been selected, a positive value realizes with the positive orientation of that carrier.

### P20. Negative Value Realizes Against Carrier Orientation

Once a carrier has been selected, a negative value realizes against the positive orientation of that carrier.

### P21. Zero Value Preserves Unresolved Realization

A zero value means no realized travel has yet been committed on that side.
It may remain latent, noisy, or tension-held.

### P22. One-Sided Noise Is Lawful

One side of a pin may remain unresolved while the other side resolves.

### P23. Two-Sided Noise Is Lawful

If both sides remain unresolved, the pin may remain as pure held tension or noisy potential rather than forcing a false structure.

## Part III. Resolved Relations From Unit And Value

### P24. Opposed Sides Yield Directed Segment Structure

If the resolved sides occupy one carrier and point against each other, the result is directed-segment structure.

### P25. Same-Direction Sides Yield Reinforcement

If the resolved sides occupy one carrier and point in the same direction, the result is reinforcement, acceleration-like continuation, or sequential same-direction passage.

### P26. Orthogonal Sides Yield Area-Like Structure

If the resolved sides occupy perpendicular carriers, the result is area-like, corner-like, or sheet-like structure.

### P27. Orthogonal Lift Is Preferred, Not Mandatory

When a contradiction is resolved geometrically, the preferred first geometric response is the smallest lawful orthogonal lift available at the current grade.
This is a preference, not an absolute decree.

### P28. Lift Availability Depends On Structural Grade

The smallest available breakout depends on the current structural depth or grade.
What is orthogonal at one grade may require leaving a sheet at the next.

### P29. Parallel, Opposed, And Orthogonal Are Derived Readings

Parallel, opposed, orthogonal, reflected, wrapped, or branched should be treated as readings of pin resolution rather than primitive behavior enums.

### P30. Relation Names Are Reports

Pin relation names are best understood as reports of what was resolved, not as the source of truth from which the system begins.

## Part IV. Pins Are Located But Need Not Be Attached

### P31. Every Pin Requires Location

A pin must be locatable in some frame, sheet, relation, or carrier context.

### P32. Location And Attachment Differ

A pin may be located without being structurally attached.

### P33. Pins Need Not Be At Zero

A pin may occur:

- at a start point
- at an endpoint
- at an interior landmark
- off a carrier
- between carriers
- between sheets

### P34. Pins Need Not Live On Existing Carriers

Pins may be off-carrier and still be meaningful.
They may influence, attract, request alignment, or await attachment.

### P35. Relative And Absolute Location Are Both Lawful

A pin may be located:

- absolutely in a sheet
- relatively to a carrier
- relatively to another pin
- by mixed absolute and relative information

### P36. A Root Pin May Be Weakly Located

At the very beginning of a system, a pin may exist in only a weak or unresolved frame.
Its initial location may itself be tension-bearing.

## Part V. Pins Can Create Carriers

### P37. A Pin May Attach To Existing Carriers

A pin may read and redirect a carrier that already exists.

### P38. A Pin May Create A Carrier

A pin may emit or initiate a new carrier.
This is fundamental, not exceptional.

### P39. A Pin May Create And Then Attach

A pin may emit a new carrier as a feeler, exploratory branch, or provisional path, then later attach it to another carrier or pin.

### P40. Join And Growth Are Not Fully Separate

Carrier growth and carrier join may be two readings of one local process.
A created carrier that later attaches may be read as growth becoming join or join discovered through growth.

### P41. Pins Can Create Orthogonal Branches

A pin may create new carriers in orthogonal directions rather than merely redirecting along an existing line.

### P42. T-Shapes, L-Shapes, And Branches Follow Naturally

Once pins can create and attach carriers, structures such as:

- `L`
- `T`
- corners
- branches
- loops

need not be primitive special cases.

## Part VI. Endpoint And Landmark Behavior

### P43. Open Endpoint Is Not Clamp

If a carrier reaches an endpoint without an explicit resolving pin, this is not a stop.
It is open termination.

### P44. Open Termination Raises Tension

Open termination raises uncertainty, tension, or unresolved continuation pressure.

### P45. Clamp Is Explicit

Clamp is an explicit pin reading in which incoming continuation is absorbed, held, or arrested.

### P46. Continuation Is An Endpoint Pin Reading

Continuation is not best modeled as a primitive global law.
It is a reading of how an encountered pin or lack of opposing pin handles onward motion.

### P47. Reflection Is An Endpoint Pin Reading

Reflection is a reading in which encountered endpoint structure redirects continuation back along a carrier.

### P48. Wrap Is A Self-Capture Reading

Wrap may be understood as one carrier and one pin whose forward continuation identifies with itself or finds itself again.

### P49. Loop Is A Self-Join Reading

A loop is a self-join in which continuation returns to the same pin, the same earlier landmark, or the same carrier origin under a lawful relation.

### P50. Endpoint Meaning Depends On Frame

The same endpoint descriptor may read differently depending on surrounding carriers, available lift, and local tension.

### P51. Midpoint Meaning Depends On Frame

A midpoint or interior pin may:

- reinforce
- reverse
- branch
- redirect
- emit a new carrier
- attach to another carrier

depending on local context.

### P52. Landmark Meaning Is Contextual

There is no need for separate primitive landmark pin names.
Location plus descriptor plus environment determine behavior.

## Part VII. Outputs, Encounter, And Local Logic

### P53. The Input Of A Pin Is Encounter

The meaningful input of a pin is not merely a value passing through.
It is the encounter of:

- current structure
- incoming continuation
- frame conditions
- nearby pins
- accumulated tension

### P54. A Pin May Have Zero Local Outputs

A pin may absorb, hold, or fail to emit further continuation.

### P55. A Pin May Have One Local Output

A pin may continue, turn, or attach into a single outgoing result.

### P56. A Pin May Have Two Local Outputs

A pin may branch, preserve a carrier while emitting another, or produce a split.

### P57. Larger Multiplicity Should Normally Lift

If more than two local outputs appear necessary, the system should usually model this as:

- several nearby pins
- a branch family
- a lift into richer representation

rather than as one overloaded primitive pin.

### P58. Branching Is A Lawful Pin Outcome

If one pin encounter admits several lawful resolutions, the result should be preserved as a branch family when the system needs to remember multiplicity.

### P59. Selection Is Secondary

Selecting one reading of a pin should not erase the full family of lawful possibilities unless explicit collapse is required.

## Part VIII. Dynamic Pinning

### P60. Pins Are Dynamic

Pins are not frozen declarations.
Their effective reading may be altered by local conditions.

### P61. Descriptor Axes Can Be Influenced

The axis inside a pin may itself be influenced by:

- attraction
- repulsion
- certainty
- confidence
- compromise
- accumulated tension
- nearby structure

### P62. Pins Carry Desire Rather Than Certainty

A pin says what local structure prefers, not what must occur exactly.

### P63. Tension Can Bend A Pin

A pin whose descriptor suggests one carrier may compromise toward another if surrounding structure strongly pulls on it.

### P64. Compromise Is Lawful

Compromise between descriptor and environment is lawful when it preserves more structure than rigid refusal would.

### P65. History Matters

How a pin resolves may depend on prior continuation, prior branches, or prior attachments.

### P66. Neighborhood Matters

Nearby carriers and pins may alter the realized reading of a pin.

### P67. Confidence And Weight Are Future-Compatible

The system should remain open to confidence, weighting, and certainty data as modifiers of pin behavior.

## Part IX. Sheets, Fields, And Cross-Sheet Influence

### P68. Sheets Are Not Observer Perspective

The proposed spaces in which pins and carriers coexist must be distinguished from the existing observer `+/-` perspective.

### P69. Pins May Exist In Sheets

Pins may be located in sheets even when not yet structurally attached to carriers in that sheet.

### P70. Pins May Influence Across Sheets

A pin in one sheet may emit tension or influence into another sheet.

### P71. Carriers May Align Across Sheets

A carrier in one sheet may gradually align toward a pin or landmark in another sheet without immediate attachment.

### P72. Cross-Sheet Capture Is Lawful

A created carrier or feeler may discover a compatible structure in another sheet and consolidate into a join.

### P73. Off-Carrier Control Is A Pin Reading

Curve control, attractors, off-line landmarks, and Bezier-like handles may be understood as off-carrier pins rather than as wholly separate primitive object classes.

## Part X. Consequences For Existing Continuation Laws

### P74. Continuation Laws Become Readings

Reflect, wrap, continue, clamp, oscillate, and similar named laws are better treated as readings of encountered pin configurations.

### P75. Boundary Laws Become Endpoint Context

Boundary behavior should eventually be read from endpoint pins, endpoint absence, and self-capture conditions rather than from a top-level continuation enum alone.

### P76. Open End And Clamp Must Stay Distinct

Open end means unresolved continuation tension.
Clamp means explicit absorbing instruction.
These must not collapse into one idea.

### P77. Reflection Need Not Be Primitive

Reflection can be derived from endpoint pin structure and encountered direction.

### P78. Wrap Need Not Be Primitive

Wrap can be derived from self-capture or self-identification under continuation search.

### P79. Continue Need Not Be Primitive

Continue can be derived from the absence of contradiction or the presence of same-direction onward pinning.

## Part XI. Consequences For Geometry, Curves, And Growth

### P80. Polylines Are Pin Sequences

A polyline may be understood as a carrier encountering a sequence of pins.

### P81. Curves Are Pin-Influenced Continuations

Curves may be understood as continuation under off-carrier or local landmark pin influence.

### P82. Branching Curves Follow Naturally

Once pins may create carriers and may live away from zero, branching curves require no special primitive beyond pinning plus traversal.

### P83. Loops Follow Naturally

Loops may be understood as self-capture or self-join under pin resolution.

### P84. CAD-Like Constraints Fit Pinning

Corners, tangencies, offsets, joins, and local constraints may all be interpreted as contextual readings of pins.

### P85. Glyph Skeletons Fit Pinning

Stems, joins, branches, loops, terminals, and crossings in glyph growth can be described as pin events and carrier resolutions.

## Part XII. Proposed Computational Direction

### P86. Pins Should Become First-Class Runtime Objects

The system should move toward explicit located pins rather than only static relation summaries.

### P87. Relation Enums Should Become Derived Summaries

Current pin relation enums remain useful for:

- debugging
- display
- tests

but they should gradually cease to be the primitive source of behavior.

### P88. Continuation Enums Should Become Compatibility Surfaces

Current continuation-law enums may remain temporarily as compatibility surfaces while pin-centered traversal is introduced.

### P89. Pin Resolution Should Be Contextual

Pin resolution should eventually depend on:

- descriptor axis
- location
- current carrier
- nearby pins
- nearby carriers
- sheets
- active tensions
- branch history

### P90. Branch Families Belong In Pin Resolution

If a pin encounter has multiple lawful outcomes, branch families should be part of the core execution model rather than a bolt-on afterthought.

## Working Examples

### Example A. Straight Directed Segment

If the two resolved sides remain collinear and opposed, the pin yields directed-segment structure.

### Example B. Reinforcement Or Acceleration

If the two resolved sides remain collinear and point the same way, the pin yields reinforcement or acceleration-like continuation.

### Example C. Orthogonal Breakout

If one side contradicts native carrier strongly enough to lift orthogonally, the result becomes area-like or corner-like.

### Example D. Open Termination

A carrier ending without explicit pin instruction does not clamp.
It produces unresolved tension at the end.

### Example E. Clamp

A clamp pin absorbs continuation and holds or arrests it.

### Example F. Reflection

An endpoint pin may redirect incoming continuation back along the carrier and thus read as reflection.

### Example G. Wrap

A carrier whose forward continuation identifies with its own earlier location or own endpoint pin may read as wrap without requiring two separately named endpoints.

### Example H. Branch

An interior pin may preserve a current carrier while emitting another one orthogonally or laterally and thereby create a branch.

### Example I. Feeler And Join

A pin may emit a provisional carrier, discover another carrier, and consolidate into a join.

### Example J. Off-Carrier Influence

A pin located away from a carrier may influence the continuation of that carrier and thereby behave like a curve control or attractor.

## Proposed Integration Direction

This proposal suggests the following broad future changes.

### I1. Keep

The following ideas remain central:

- interval as primitive object
- frame and perspective
- tension as informative
- lift as lawful
- branch families as lawful multiplicity
- `Axis` as the current pin descriptor language

### I2. Reinterpret

The following should gradually be reinterpreted as derived readings:

- `reflect`
- `wrap`
- `continue`
- `clamp`
- pin relation labels such as parallel or orthogonal

### I3. Replace Over Time

The following kinds of mechanisms should gradually be replaced or demoted:

- continuation-law-as-source-of-truth
- rigid relation enums as the source of pin meaning
- isolated pin resolution without contextual encounter

### I4. Expand Into

This proposal should expand into:

- traversal
- geometry
- curve control
- landmark systems
- branching growth
- glyph formation
- CAD-like constraints
- sheet interaction

## Final Proposal Statement

Pinning is proposed as the general local mechanism by which Core 2 structure is attached, emitted, contradicted, redirected, reinforced, lifted, absorbed, looped, wrapped, branched, or joined.

The `Axis` is the current descriptor of this mechanism, but the meaning of a pin is not fixed by its descriptor alone.
Its meaning arises from descriptor, location, current carrier, available lift, active tensions, neighboring structure, and branch history.

Under this proposal, many behaviors currently expressed as separate directives become contextual readings of encountered pins.
This should simplify the conceptual foundation of Core 2 while broadening its power.

## Next Step

If this proposal is accepted in spirit, the next step should be to derive an implementation plan that:

- introduces explicit located pins
- makes current pin relations derived summaries
- reinterprets continuation laws as endpoint or landmark pin readings
- integrates branch families into pin resolution
- preserves existing behavior during migration where possible
