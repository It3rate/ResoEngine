# Conceptual Schema Guided Form Plan

This plan treats the emerging conceptual-schema vocabulary as a possible guiding layer for:
- letter creation
- ornamental pattern generation
- later diagram, route, and graph-like construction

The goal is not to force every shape into a rigid symbolic taxonomy.
The goal is to see whether a small abstract set of recurring schemas can guide construction more cleanly than a large pile of letter-specific special cases.

## Working Claim

Many recognizable forms can be described as combinations of:
- carriers
- sites
- frame anchors
- source-goal traversal
- bounded intervals
- branch and merge
- closure and cycle
- center-periphery organization
- support, adjacency, and containment

Letters are one visible test bed.
Ornamental patterns are another, because they often amplify:
- repetition
- symmetry
- alternation
- closure
- around-ness
- center-periphery
- branching or lattice-like crossing

## Letter-Oriented Readings

The aim is not that the symbolic schema alone completely draws the letter.
The aim is that it guides which local structures should be created and which constraints should wake up.

Examples:

- `A`
  two source-goal diagonals that meet at an apex, plus a bounded interval crossbar between them
- `H`
  two vertical side carriers with one bounded interval crossbar
- `T`
  one vertical stem plus one top bounded interval bar
- `Y`
  one branch site splitting into two upper arms and one lower continuation
- `D`
  one vertical stem plus one return or closure-like bowl
- `G`
  one open surrounding arc plus one inner cornered support stroke
- `O`
  one closure or cycle around a center-periphery field
- `S`
  two opposed curve regions with a center handoff or transition site

That suggests a later builder vocabulary more like:
- `CreateStem`
- `CreateBoundedBar`
- `CreateMeet`
- `CreateBranch`
- `CreateReturnArc`
- `CreateClosureLoop`
- `CreateCorner`
- `CreateCenterPeripheryGuide`

rather than one bespoke constructor per glyph feature.

## Ornament-Oriented Readings

Ornamental patterns seem even more likely to benefit from this, because they are often built from a smaller family of recurring structural motifs.

Examples:

- border or meander
  path coincidence plus cycle and cornered return
- braid or interlace
  crossing plus alternation plus closure
- rosette
  center-periphery plus surround-closure plus repetition
- lattice
  crossing plus bounded interval plus branch-merge network
- vine or tendril
  source-goal plus branch plus closure plus around-ness
- fan or burst
  center-periphery plus directional offset plus bounded angular bands

## Near-Term Implementation Direction

1. Keep the current letter pages as working references.
2. Add a second builder vocabulary that names schema-like motifs rather than only letter-specific parts.
3. Let each motif still produce ordinary letter-formation carriers and sites underneath.
4. Use activation gates so some constraints strengthen only after prerequisite attachments exist.
5. Reuse the same motif vocabulary for one ornamental demo page afterward.

That means the schema layer would guide:
- what local pieces are created
- what frame anchors matter
- what tensions activate later
- what repetition or closure rules are expected

without replacing the actual Core 2 carrier and site substrate.

## First Concrete Targets

The most practical first targets look like:

1. letters with obvious branch or bounded-bar structure
   `A`, `H`, `T`, `Y`
2. letters with return or closure structure
   `D`, `G`, `O`, `S`
3. one ornamental border or rosette page
   enough to test cycle, center-periphery, around-ness, and repetition

## Success Criteria

This direction is probably a win if:
- the builder code gets shorter or clearer
- new letters reuse more motifs instead of duplicating site logic
- ornamental patterns can be expressed with the same motif names
- the resulting vocabulary still feels close to the language and conceptual mappings rather than arbitrary

It is probably bureaucracy if:
- the motif layer only renames the old constructors
- it introduces lots of exceptions for each letter
- it stops corresponding to the conceptual mappings in the language work

The test is not elegance alone.
The test is whether one structural vocabulary can guide letters, ornaments, traversal, and language without becoming vague or arbitrary.
