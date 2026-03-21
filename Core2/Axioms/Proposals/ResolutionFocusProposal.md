# Core 2 Resolution, Focus, And Language Proposal

This document is a rough proposal for how resolution should behave in Core 2, especially when the system is steered toward language rather than toward ordinary folded arithmetic alone.

It is intentionally compact and principle-first.
The goal is to get the widest coverage from the fewest laws, then let richer mathematical and linguistic structures be constructed from those laws rather than introduced as many special cases.

It should be read beside:

- [PromptAxioms.md](../PromptAxioms.md)
- [SymbolicGrammarProposal.md](./SymbolicGrammarProposal.md)
- [PinningProposal.md](./PinningProposal.md)
- [PositionedPinningProposal.md](./PositionedPinningProposal.md)

## Core Claim

The denominator in Core 2 should usually be read as support or resolution in a frame, not merely as "the number underneath."

So a value such as `4/5` carries at least two distinct facts:

- folded value: `0.8`
- support: `5 ticks`

Those facts should not be collapsed too early.

## Quick Orientation

The proposal is built around a small set of primitives.
Before the detailed sections, this is the intended short reading of each one.

- folded value
  What the expression collapses to when support is ignored.
  Example: `4/5`, `8/10`, and `400/500` all fold to `0.8`.

- support or resolution
  What substrate of distinctions is being preserved.
  Example: `4/5` preserves `5` ticks of support, while `400/500` preserves `500`.

- uncertainty
  How confidently the reading is known.
  Example: `about half a mile + exactly half a mile` is near `1 mile`, but the rougher reading still dominates confidence.

- focus or host
  Whose structure persists into the result.
  Example: in `2 * 5/10`, the working assumption is that `5/10` is the host.

- transform or applied role
  What acts on that host.
  Example: in `axis * i`, `i` acts as opposition or basis change rather than as a second independent measured object.

- ambiguity or branch family
  The candidate set before context selects.
  Example: `the chicken is ready to eat` preserves at least two lawful role assignments before context settles it.

The rest of the proposal tries to keep these primitives minimal, precise, and reusable across both mathematics and language.

## Part I. Primitive Commitments

This section states the minimum primitives that seem necessary to handle resolution well while staying compatible with language.

### RF1. Folded value and support differ

`4/5`, `8/10`, and `400/500` may fold to the same scalar while remaining different structured values if support matters.

### RF2. Support and grain differ

Support answers "how many ticks are active in the current substrate?"
Grain answers "what is the size of one tick in the active frame?"

The same object may preserve support while changing grain, or preserve grain while changing support.

### RF3. Resolution and uncertainty differ

Resolution is not the same thing as uncertainty.

Examples:

- a value may be carried on fine support and still be wrong because of bias
- a value may be carried on coarse support and still be exact relative to that support

### RF4. Host or focus is primitive

When structures interact, one structure is often treated as primary or persistent.
This role should be primitive because it strongly predicts:

- whose support is inherited
- whose continuity matters
- whose change is foregrounded
- what the result is "about"

Example:

- `2 * 5/10 -> 10/10`

The result is still most naturally "about" the original `5/10`, now acted on by `2`.

### RF5. Applied or modifying role is primitive

The acting or modifying structure is also primitive.
It need not contribute new support.
Sometimes it only changes orientation, sign, basis, emphasis, or selection.

Example:

- `axis * i`

Here `i` is acting on the axis.
It need not be introducing a second support substrate.

### RF6. Perspective differs from focus

Perspective changes how a structure is read.
Focus changes which structure is treated as primary.

This matters because a perspective flip may transform the measuring system or reading convention without transforming the measured object itself.

Example:

- one observer reads a board in centimeters
- another reads it in inches

The board has not changed.
The reading frame has.

### RF7. Branch family is primitive

When several support interpretations, role assignments, or readings remain lawful, they should persist as a branch family until selection is justified.

This is essential both for inverse continuation and for language ambiguity.

Example:

- `sqrt(4)` yields at least `{2, -2}`
- `the chicken is ready to eat` yields at least `{chicken eats, chicken is eaten}`

### RF7a. Units and referents should bias defaults, not replace the primitive laws

Units and referents should contribute small default hints rather than a second law system.
The most useful hints appear to be:

- count vs mass
- discrete vs continuous
- extensive vs intensive
- intrinsic vs frame-relative
- bounded vs open scale

Default choice should still happen in this order:

1. operation
2. host or focus
3. unit or referent hint

So units help break ties, reject bad defaults, and preserve language-friendly readings without taking over from the primitive support laws themselves.

## Part II. Primitive Resolution Laws

This section proposes the minimum law set needed to explain most observed resolution behavior.

### RF8. Support inheritance is the default law

When one operand is host or persistent and another acts upon it, the default should be:

- preserve the host's support
- let the applied term modify realized value, direction, orientation, or emphasis

This is the likely default for:

- scalar scaling on a fixed frame
- sign flip
- opposition
- mirror
- basis change
- many modifier-like language relations

Example:

- `2 * 5/10 -> 10/10`

The host support survives.
The value changes.
This should be the default law unless another law is clearly required.

### RF9. Support aggregation is explicit

When separate supports are pooled or concatenated, support adds.

Examples:

- repeated reviews
- repeated measurements
- accumulated evidence
- repeated observations
- concatenated spans

So one hundred `4/5` reviews may aggregate to `400/500`.

This law should be used only when supports are actually being pooled, not just compared or transformed.

### RF10. Support composition is explicit

When two supports form an independent product space, support composes, often multiplicatively.

Examples:

- product of independent proportions
- area-like composition
- repeated multiplicative continuation

This is the main reason denominator growth under multiplication and powers is often natural.

But it should be applied only when the operation genuinely creates a product space, not whenever a `*` symbol happens to appear.

### RF11. Support refinement preserves folded value

A value may be re-expressed on finer support while preserving folded value.

Example:

- `4/5 -> 8/10`

This is refinement, not a new folded value.

It is useful when the same object is being described more finely, not when several objects are being pooled.

### RF12. Common-frame re-expression differs from aggregation

`5/10 + 1/2` may mean different things:

- common-frame addition on one shared extent: `5/10 + 5/10 = 10/10`
- pooled evidence over two supports: `(5 + 1)/(10 + 2) = 6/12`
- fold-first scalar addition: `0.5 + 0.5 = 1.0`

These should not be forced into one law.

This distinction matters because language often leaves it implicit whether speakers mean "combine in one frame" or "pool separate evidence."

### RF12a. Exact alignment support and committed result support differ

The support used to align values exactly for one operation is not automatically the support that should be committed as the result's lasting reading.

So there are three distinct layers:

- re-expression support
  A finer exact restatement of one value, such as `4/5 -> 8/10`

- exact alignment support
  A temporary exact frame used to combine values without loss

- committed result support
  The support later chosen for display, continuation, or interpretation

Committed result support should be chosen by policy rather than silently inherited from exact alignment.
Useful policies include:

- preserve coarser support
- preserve finer support
- preserve host support
- preserve exact alignment support
- negotiate support from uncertainty or confidence
- defer one committed support and preserve the exact structure instead

### RF13. Pure transforms usually preserve support

Transforms such as opposition, negation, mirror, and perspective or basis change should usually preserve support unless the transform explicitly introduces new support.

This is why expressions such as `axis * i` may deserve a more support-preserving law than generic support-composing multiplication.

This law is especially important for language-like reframing, emphasis change, voice change, and perspective shifts.

### RF14. Uncertainty propagates separately from support

Support may influence uncertainty, but uncertainty should not be identified with support.

The system should eventually allow:

- quantization uncertainty
- bias or calibration uncertainty
- branch uncertainty
- discourse uncertainty

as distinct layers.

### RF15. Fold and unfold require support policies

Folding erases support distinctions.
Unfolding should therefore return candidate families unless a support policy is supplied.

Useful policies include:

- minimal exact support
- preserve inherited support
- fit a target frame support
- preserve aggregated sample count
- preserve instrument grain

Without such policies, the same folded value can unfold into many equally lawful supports and the system becomes underspecified.

## Part III. Constructible From Primitives

This section lists richer structures that seem important but do not yet need to be treated as separate primitives.

### RF16. Carrier or medium is constructible

`Carrier` appears to be a very important recurring structure, but it may be constructible from:

- a persistent host
- a payload or theme
- a continuation law
- source and goal structure
- routing or placement
- selective shielding of what inherits change

In this reading, the carrier is not invariant in every respect.
Its position, route, and continuation state may change constantly.
What matters is that the carried thing may remain comparatively protected and be released later without being the primary transformed structure.

### RF17. Transfer and delivery are constructible

Structures such as:

- move
- send
- carry
- deliver

should probably elaborate a small schema rather than each becoming its own primitive law.

Likely recurring roles include:

- host or focused persistent structure
- applied or modifying structure
- theme or payload
- source
- goal
- carrier or medium
- controller or initiator
- observer or perspective holder

### RF18. Voice and focus shift are constructible

Examples:

- `I took the bus to work`
- `the bus took me to work`

The event family may be similar, but the focused or host structure differs.
That difference can often be constructed by changing focus and support inheritance rather than by inventing a separate primitive event type.

### RF19. Structural ambiguity in language is constructible from branch plus focus

Examples:

- `the chicken is ready to eat`
- `I never said she stole my money`

These are candidate families with different role assignments, focus placements, or attachment structures.
They do not require a special ambiguity primitive if branch family plus focus/perspective already exist.

### RF20. Measurement alignment is constructible from perspective plus re-expression

If one person measures in centimeters and another in inches, the board does not change.
The reading systems differ.

So many "conversion" situations can be understood as:

- perspective or frame difference
- common-frame re-expression
- support-preserving comparison

rather than as object transformation.

### RF21. Protected transport is constructible

Examples:

- envelope carrying a letter
- bus carrying a passenger
- packet carrying a message
- recursive state carrying a payload through continuation

This can often be built from carrier plus payload plus route plus release, without adding a new primitive kind.

## Part IV. Language Guidance

### RF22. Language should bias law choice

When several mathematically possible support laws exist, Core 2 should strongly prefer the law that gives better linguistic coverage with fewer primitives.

This does not override correctness.
It chooses among several lawful system designs.

### RF23. Focus often selects whose support persists

When several participants are present, focus often answers:

- whose structure persists?
- whose support is inherited?
- whose uncertainty dominates?
- which relation becomes primary in display?

This is why similar events can feel structurally different in language.

### RF24. Carrier-like readings are often second-level, not first-level

Carrier is very common and very important, but it may often be a second-level reading built from:

- host persistence
- payload isolation
- route continuation
- later release

That is a good example of something that deserves strong coverage without automatically becoming primitive.

## Part V. Short Parallel Examples

### Math examples

- `2 * 5/10 -> 10/10`
  Metadata: host=`5/10`, applied=`2`, support law=`inherit`

- `5/10 pooled with 1/2 -> 6/12`
  Metadata: support law=`aggregate`

- `5/10 + 1/2 on one shared interval -> 10/10`
  Metadata: support law=`common-frame re-expression`

- `4/5 -> 8/10`
  Metadata: support law=`refine`, folded value unchanged

- `axis * i`
  Metadata: host=`axis`, applied=`i`, likely support law=`pure transform / preserve support`

### Language examples

- `the chicken is ready to eat`
  Metadata: branch family over role assignment `{chicken eats, chicken is eaten}`

- `I never said she stole my money`
  Metadata: one surface string, several focus placements, several selected relations

- `I took the bus to work`
  Metadata: host or focus=`I`, carrier=`bus`, goal=`work`

- `the bus took me to work`
  Metadata: host or focus=`bus`, payload=`me`, goal=`work`

- `deliver the package to Robin`
  Metadata: theme=`package`, goal=`Robin`, carrier or transfer-medium foregrounded more strongly than in a simpler `move` event

## Part VI. Immediate Design Consequence

The system should probably stop asking one global question such as:

- "what does the denominator do under multiplication?"

and instead ask:

- is support being inherited?
- aggregated?
- composed?
- refined?
- re-expressed in a common frame?
- or merely carried through a pure transform?

That move appears to align better with both mathematical structure and language structure.
