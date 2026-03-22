# Letter Formation Pinning Plan

This is the next step beyond the current local-desire letter page.

## Goal

Move letter formation closer to pure Core 2 structure so that:
- endpoint pins are explicit structural objects
- attachment and detachment come from local pin compatibility
- carrier meaning stays host-relative
- higher-order tensions wake up only after prerequisite structure exists

## Near-Term Direction

1. Copy the current letter formation page into a new pin-driven page and preserve the current page as a reference.
2. Represent connectable line ends as explicit endpoint pins rather than only informal join desires.
3. Add a small structural occupancy concept for pin sides:
   - satisfied
   - open and seeking
   - optional
   - blocked
4. Keep `Axis` as the main local descriptor of what kind of attachment is lawful.
   - same-carrier vs orthogonal preference still comes from unit sign
   - zero value still means "do not add bend/travel here"
   - `0/0` remains unresolved or don't-care rather than "needs attachment"
5. Let open unsatisfied endpoints generate attach, detach, extend, and probe proposals.
6. Use graph compatibility and host-relative pin meaning to decide whether an attachment is lawful.
7. Let stronger structural tensions activate only after simpler goals are met.
   Examples:
   - once both ends of a crossbar attach, level and midline pressures become strong
   - once a bowl closes onto a stem, curvature confidence becomes strong

## Language-Oriented Positional Descriptors

The pin-driven version should keep in mind how approximate preposition-like relations work in English.
The goal is not only geometric precision. It is also a low-resolution structural vocabulary that can later support language.

Likely examples include:
- up
- down
- close to
- behind
- in front
- between
- inside
- outside
- halfway
- beside
- along
- at
- from
- to
- near

These should preferably be encodable with Core 2 structure rather than as a separate language-only enum.
A likely first path is:
- a frame axis describing the full start-to-end possibility space
- child access or child-relative placement inside that frame
- low-resolution axis descriptors expressing the needed coarse relation

Examples:
- `inside` means greater than the frame start and less than the frame end
- `between` is similar, but usually with stronger boundedness by both sides
- `in front` and `behind` may be before-start or after-end style relations with some local tolerance
- `beside` and `to the left/right of` are orthogonal side relations
- `halfway` is a midpoint-biased relation rather than one exact percentage
- `along` means oriented continuation constrained by the host span rather than one point

These descriptors do not need high numeric precision.
They should be encoded at the resolution required to distinguish their meanings.
The environment may then bias or refine them.
For example, if a given letter family places its crossbar slightly above the visual midpoint, the environment can shift how `halfway` is interpreted without requiring the descriptor itself to become artificially exact.

## English Description Exercise

Before implementing the fuller pin-driven version, it will be useful to describe each letter as if explaining it to someone who understands English but has no prior model of letter formation.

Examples of the style of description:
- start at the top left
- draw down to the bottom
- halfway up, extend to the right
- curve outward and return to the stem
- branch from the center and rise to the upper right

This matters because those descriptions naturally reveal:
- which preposition-like relations are actually needed
- which relations are coarse and approximate rather than exact percentages
- which goals only activate after earlier structure exists
- where the environment is shaping meaning rather than the descriptor itself fully specifying the result

The later pinning system should therefore be checked against two questions:
- can the structure encode these English-style positional relations directly enough to be useful?
- can the same encoding remain honest as Core 2 geometry rather than becoming a separate symbolic overlay?

## Working Principle

The new page should use as much of the Core 2 stack as possible for:
- encoding
- transform and proposal generation
- solving and tension
- topology and attachment state

The applied layer should mainly stage the experiment, not replace the underlying representation.
