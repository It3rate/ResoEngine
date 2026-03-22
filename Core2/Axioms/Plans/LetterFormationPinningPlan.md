# Letter Formation Pinning Plan

This plan treats letter formation as a test bed for a more general Core 2 structural encoding.
The aim is not only to build better glyphs.
It is to push toward a small set of primitives that can also describe:
- prepositions and approximate spatial language
- graph traversal and flow control
- story and sentence progression
- electrical routing and branching
- temporal spans and remembered intervals

## Core Claim

The pin-driven version should move away from "the system already knows the whole letter."
It should instead let local structural elements carry:
- their own axis-descriptor preferences
- their own open or satisfied attachment state
- their own local tensions
- their own later-activating constraints

Larger recognizable form should emerge from those local pressures plus the environment.

## Structural Commitments

1. A site is the basic local landmark.
   A site may be:
   - attached to a carrier
   - floating in another sheet or space
   - acting as a frame landmark
   - acting as a join, branch, terminal, or closure point

2. An anchor is not a wholly separate primitive from a site.
   It is better understood as a specialized site whose main role is landmarking or frame attachment rather than active continuation.

3. Multiple spaces or sheets are allowed.
   Some sites may live on the active letter carrier sheet.
   Others may live on frame lines, control sheets, or nearby host spaces.

4. `Axis` remains the main local descriptor.
   The descriptor should continue to carry:
   - carrier preference through unit sign
   - realized or latent travel through value sign
   - magnitude as weighted participation, pressure, or confidence

5. Open sides should be structural, not numeric hacks.
   A missing attachment should not be encoded as a fake special number.
   It should be encoded as an unsatisfied side or open occupancy state on a site.

6. Folding is later interpretation.
   Curves, bowls, corners, loops, and areas may be readable from the pinned structure without every folded result being explicitly authored up front.

## Near-Term Direction

1. Copy the current dynamic letter page into a new pin-driven page and preserve the current page as a reference.
2. Represent connectable line ends as explicit endpoint sites.
3. Let site sides carry occupancy state such as:
   - satisfied
   - open and seeking
   - optional
   - blocked
4. Use host-relative compatibility to decide whether attachments, detachments, orthogonal breakouts, and mergers are lawful.
5. Let unresolved sites emit attach, detach, extend, probe, and possibly sprout proposals.
6. Let stronger constraints activate only after simpler structure has formed.

Examples:
- once both ends of a crossbar attach, level and midline pressures become strong
- once a returning bowl closes onto a stem, curvature confidence becomes strong
- once a branch is established, symmetry or equal-arm pressures may become strong

## Boolean and Segment Reading of Prepositions

Preposition-like relations should be considered not only as labels, but also as structural readings over carriers and frames.

A useful direction is:
- choose an active frame segment
- place a target segment inside or relative to that frame
- derive other coarse regions by boolean operations
- later select one location, one route, or one occupied region if needed

Example intuition:
- let `target` be a small segment near the origin
- let `cover` be a larger segment around it
- then `near` can be read as `cover NOT target`

This is powerful because it distinguishes:
- the whole admissible region
- a later selected location inside that region
- the path or traversal that reaches it

So:
- `surround` or `around` may preserve the whole family of lawful surrounding positions
- `nearby` may imply one chosen or occupied member from that family
- `inside`, `between`, `beside`, `in front of`, and `behind` can often be read as coarse boolean or boundary-relative relations on active frames or orthogonal lifts

Some cases will require more than one carrier or sheet.
For example:
- `around` may need a pinned 2D reading
- `between` may need two bounding landmarks plus a selected interior region

But the boolean and directed-segment interpretation should still be treated as a major source of meaning rather than a side trick.

## Temporal and Event Encoding

The same physical reading should be considered valid for time.

Example intuition:
- "last week I was hoping to go on a 3 day holiday"
- may be represented as a directed temporal span from seven days in the past to four days in the past

That matters because:
- past and future remain spatially readable along a time carrier
- intervals, expectations, delays, and windows can be read as structured spans
- boolean relations like overlap, exclusion, inside, before, after, and between remain meaningful in time as well

This should be taken seriously.
The aim is not arbitrary symbolism.
The aim is a physically relevant conceptual encoding where structure in the representation roughly matches structure in the thing being thought about.

## Language-Oriented Positional Vocabulary

The later pin-driven version should be checked against approximate relational terms such as:
- up
- down
- left
- right
- near
- beside
- between
- inside
- outside
- in front of
- behind
- halfway
- along
- across
- from
- to
- around

These should preferably be encodable through:
- frame-relative sites
- child-relative placement
- boolean-derived regions
- axis descriptors at coarse resolution

The environment may then bias those meanings without forcing false exactness.
For example:
- `halfway` may bias slightly above visual center in one letter family
- `near` may expand or tighten based on available support or local crowding

## English Description Exercise

The system should still be tested by describing letters as if explaining them to someone with no prior model of letter formation.

Examples:
- start near the top left
- draw downward toward the baseline
- from halfway up, extend to the right
- curve outward and return to the stem
- branch from the center and rise to both upper sides

This matters because those descriptions reveal:
- which relations are actually primitive
- which are only coarse and approximate
- which constraints are activated only after earlier structure exists
- where frame landmarks and boolean regions are doing real work

## Working Principle

The pin-driven experiment should use as much of the Core 2 stack as possible for:
- encoding
- transform and proposal generation
- tension and confidence
- topology and occupancy
- branching and joining
- folding and later interpretation

The applied layer should mainly stage the experiment.
It should not replace the underlying representation with a parallel non-Core-2 ontology.

## Interpretation Principle

Letters should be treated as evidence that these structural patterns are already intelligible to us at a very primitive level.

Their intelligibility is not only conventional.
It likely depends on deep recurring patterns such as:
- continuation
- termination
- branching
- merging
- return
- closure
- cornering
- containment
- crossing
- adjacency
- centrality
- directed movement

The same patterns appear in:
- spatial description
- temporal description
- program flow
- circuit routing
- narrative structure
- ordinary physical reasoning

The long-term goal is therefore not "a letter engine."
It is a small abstract structural vocabulary that can generate letters as one special case of a much broader conceptual system.
