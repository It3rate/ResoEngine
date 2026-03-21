# Core 2 Resolution, Focus, And Language Proposal

This document is a rough proposal for how resolution should behave in Core 2, especially when the system is steered toward language rather than toward ordinary folded arithmetic alone.

It is intentionally compact and principle-first.
The goal is to give broad coverage without exploding into many verb-specific or operator-specific special cases.

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

## Part I. Primitive Distinctions

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

### RF4. Focus and support differ but interact

Focus is not the same thing as resolution.
But focus often determines whose support law should dominate when several structures interact.

### RF5. Perspective and focus differ

Perspective changes how a structure is read.
Focus changes which structure is treated as primary or persistent.

### RF6. Ambiguity is not failure

When several support interpretations or role assignments remain lawful, they should persist as a branch family until selection is justified.

## Part II. Proposed Resolution Laws

### RF7. Support inheritance should be the default

When one operand is host or persistent and another acts upon it, the default law should be:

- preserve the host's support
- let the applied term modify realized value, direction, or orientation

This is the likely default for:

- scalar scaling on a fixed frame
- sign flip
- opposition
- basis change
- many language-like modifier relations

### RF8. Support aggregation should be explicit

When separate supports are pooled or concatenated, support should add.

Examples:

- repeated reviews
- repeated observations
- evidence accumulation
- concatenated spans

So one hundred `4/5` reviews may aggregate to `400/500`.

### RF9. Support composition should be explicit

When two supports form an independent product space, support should compose, often multiplicatively.

Examples:

- product of independent proportions
- area-like composition
- repeated multiplicative continuation

This is the main reason denominator growth under multiplication and powers is often natural.

### RF10. Support refinement should preserve folded value

A value may be re-expressed on finer support while preserving folded value.

Example:

- `4/5 -> 8/10`

This is refinement, not a new folded value.

### RF11. Common-frame re-expression should be distinguished from aggregation

`5/10 + 1/2` may mean different things:

- common-frame addition on one shared extent: `5/10 + 5/10 = 10/10`
- pooled evidence over two supports: `(5 + 1)/(10 + 2) = 6/12`
- fold-first scalar addition: `0.5 + 0.5 = 1.0`

These should not be forced into one law.

### RF12. Pure transforms should usually preserve support

Transforms such as opposition, negation, mirror, and similar role-swaps should generally preserve support unless the transform explicitly introduces new support.

This is why expressions such as `axis * i` may deserve a more support-preserving law than generic support-composing multiplication.

### RF13. Uncertainty should propagate separately from support

Support may influence uncertainty, but uncertainty should not be identified with support.

The system should eventually allow:

- quantization uncertainty
- bias or calibration uncertainty
- branch uncertainty
- discourse uncertainty

as distinct layers.

### RF14. Fold and unfold should use support policies

Folding erases support distinctions.
Unfolding should therefore return candidate families unless a support policy is supplied.

Useful policies include:

- minimal exact support
- preserve inherited support
- fit a target frame support
- preserve aggregated sample count
- preserve instrument grain

## Part III. Language Guidance

### RF15. Core 2 should prefer a small participant schema

Language should not force many primitive special cases.
Instead, a small recurring participant schema should be preferred.

The likely recurring roles are:

- host or focused persistent structure
- applied or modifying structure
- theme or payload
- source
- goal
- carrier or medium
- controller or initiator
- observer or perspective holder

Not every sentence needs every role.
Different verb classes foreground different subsets.

### RF16. Verb alternations should usually be elaborations, not primitives

Many Levin-style and FrameNet-style alternations look like re-weightings or re-foregroundings of a smaller underlying structure.

Examples:

- `move` foregrounds theme plus path or goal
- `send` foregrounds source, goal, and transfer of a theme
- `deliver` often adds or foregrounds a carrier or successful-arrival bias

These should preferably elaborate a smaller common schema rather than each becoming a separate primitive law.

### RF17. Focus often selects the host

When several participants are present, focus often answers:

- whose structure persists?
- whose support is inherited?
- whose uncertainty dominates?
- which relation becomes primary in display?

This is why similar events can feel structurally different in language.

### RF18. Voice and emphasis can preserve event identity while changing structure

Examples:

- `I took the bus to work`
- `the bus took me to work`

The event family may be similar, but the host or focused participant differs.
Core 2 should allow a preserved underlying event with different foregrounded carriers and support inheritance.

### RF19. Structural ambiguity in language is like support or role ambiguity in math

Examples:

- `the chicken is ready to eat`
- `I never said she stole my money`

These are not merely fuzzy readings.
They are candidate families with different role assignments, focus placements, or attachment structures.

Core 2 should therefore prefer:

- preserve candidate family first
- select by context, focus, perspective, or certainty only later

## Part IV. Short Parallel Examples

### Math examples

- `2 * 5/10 -> 10/10`
  Metadata: host=`5/10`, applied=`2`, support law=`inherit`

- `5/10 pooled with 1/2 -> 6/12`
  Metadata: support law=`aggregate`

- `5/10 + 1/2 on one shared interval -> 10/10`
  Metadata: support law=`common-frame re-expression`

- `4/5 -> 8/10`
  Metadata: support law=`refine`, folded value unchanged

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

## Part V. Immediate Design Consequence

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
