# Core 2 Symbolic Grammar Proposal

This document proposes a first symbolic grammar for Core 2.

Its goal is not to place a conventional expression language on top of Core 2.
Its goal is to let Core 2 describe itself symbolically in a form that can be:

- understood by AI
- serialized and edited by humans
- elaborated into executable structures
- evaluated and folded computationally

It is intended to sit beside:

- [PromptAxioms.md](../PromptAxioms.md)
- [PinningProposal.md](./PinningProposal.md)
- [PositionedPinningProposal.md](./PositionedPinningProposal.md)
- [RecursivePinningProposal.md](./RecursivePinningProposal.md)
- [CarrierRoutingProposal.md](./CarrierRoutingProposal.md)

This file is a proposal, not settled doctrine.

## Core Claim

The central claim of this proposal is:

- the symbolic layer should be Core 2 native
- symbolic terms should reuse Core 2 elements wherever possible
- weights, counts, transforms, and preferences should be encoded by Core 2 values rather than by foreign generic metadata
- authored expressions may be tree-shaped
- elaborated structural meaning will often be graph-shaped
- equations should be treated as lawful relations and constraints between Core 2 terms, not merely as arithmetic text

In this reading, the symbolic layer is not a second system.
It is a readable and executable grammar of the first system.

## Part I. Native Encoding Principle

### SG1. Symbolic grammar should reuse Core 2 elements

If Core 2 already has a lawful element for a role, the symbolic layer should use that element rather than inventing a foreign scalar or enum.

Examples:

- a weight should normally be a `Proportion`, or a higher Core 2 value if needed
- a transform should normally be encoded as a lawful transform-bearing Core 2 value such as `i`, `-1`, or `3i`
- a repeat count should not default automatically to machine `int`
- route, branch, and pin information should refer to carriers, sites, and positions already present in Core 2

### SG2. New symbolic structure should appear only when Core 2 has no direct native term

New symbolic nodes are justified for roles such as:

- reference
- binding
- relation
- constraint
- program ordering
- selection

These are needed not because Core 2 is insufficient, but because a computational grammar must name, relate, and sequence Core 2 structures.

### SG3. Symbolic convenience must not erase lawful distinctions

The symbolic grammar must not flatten:

- state and transform
- host and applied
- same carrier and same geometry
- branch family and selected member
- hard relation and soft preference

## Part II. Two Levels Of Representation

### SG4. The authored form may be tree-shaped

An authored expression may begin as a tree of terms, applications, equations, and declarations.

This is the right level for:

- text shorthands
- editor forms
- AI prompts
- serialization

### SG5. The elaborated form will often be graph-shaped

Shared carriers, recursive pinning, true crosses, rejoining, and reused named parts are not naturally represented by a pure tree.

The authored tree should therefore elaborate into a graph or structured network.

### SG6. Tree and graph are not competing truths

The tree is the authored statement.
The graph is the elaborated structural meaning.
Both are lawful and useful.

## Part III. Proposed Symbolic Sorts

### SG7. The symbolic grammar needs a small set of top-level sorts

The first useful sorts are:

- `ValueTerm`
- `TransformTerm`
- `RelationTerm`
- `ConstraintTerm`
- `ProgramTerm`

### SG8. `ValueTerm` names Core 2 structural values

`ValueTerm` may include:

- scalar literal
- proportion literal
- axis literal
- area literal
- higher-degree structural literal
- quantity literal
- named reference
- reading expression
- pinning expression
- fold expression
- branch family expression

### SG8a. The grammar should be degree-general even before higher degrees are fully named

The symbolic layer should not hard-code a ceiling at `Area`.
Even if `Volume` and higher structures are not yet fully formalized in code, the grammar should assume that:

- pinning may raise structural degree
- folding may lower structural degree
- higher-degree values may exist before they are all given settled everyday names

This keeps the symbolic layer aligned with the deeper Core 2 direction instead of freezing it at current implementation boundaries.

### SG9. `TransformTerm` names lawful Core 2 actions

`TransformTerm` may include:

- scale
- oppose
- negate
- composed transform
- repeated transform
- inverse continuation request

Where possible, these should remain expressible as Core 2 values in transform position rather than being duplicated as a disconnected operator language.

### SG10. `RelationTerm` names lawful structural relations

`RelationTerm` may include:

- equality
- attachment
- shared-carrier identity
- routing
- containment
- selection

### SG10a. Boolean combination should remain a first-class structural relation

Boolean behavior should not be treated merely as composed transform syntax.
It is a lawful structural comparison and partitioning process over occupancy, support, carrier inheritance, and frame.

A symbolic form for boolean work should therefore live beside other relation and fold terms rather than being hidden inside generic transform composition.

### SG11. `ConstraintTerm` names hard and soft requirements

`ConstraintTerm` may include:

- require
- prefer
- preserve
- select
- minimize tension

### SG12. `ProgramTerm` names executable order

`ProgramTerm` may include:

- let or bind
- sequence
- fire
- commit
- iterate
- converge

This is the point where equation-like symbolic description begins to touch control flow and procedural execution.

## Part IV. Core 2 Native Parameters

### SG13. Weights should be Core 2 values

A preference weight should normally be a `Proportion`.

Examples:

- `2/1`
- `4/2`
- `1/2`

These may later be rendered as words such as low, medium, or high, but the stored symbolic form should remain native.

### SG14. Transform codes should be Core 2 values

Negate, oppose, and scaled opposition should remain expressible through the same transform-bearing value logic already present in the axioms.

Examples:

- `-1` for negation
- `i` for opposition
- `3i` for scaled opposition

### SG15. Repeat indices should be Core 2 values

A repetition index should not be assumed to be a plain cardinal count.

Examples:

- `3` may mean ordinary triple continuation
- `-3` may mean reverse or opposed continuation under some law
- `3i` may mean repetition with phase, opposition, or orbit displacement

The exact meaning depends on the repetition law.
That is a feature, not a defect.

### SG15a. Branch weighting and execution weighting may share the same native value vocabulary

The same Core 2 parameter may be read differently under different lawful interpretations.

Examples:

- in geometric or structural reading, a branch value such as `9/1` against `2/1` may shape the branch itself
- in execution or flow reading, that same asymmetry may bias selection frequency, priority, or confidence

This should not force two separate weighting systems.
It should instead be treated as one native parameter vocabulary whose effect depends on the active law and fold.

### SG16. Constraint strength and transform direction should remain separate

A strong preference is not the same thing as an opposed direction.
Both may be expressed with Core 2 values, but they play different symbolic roles and should not be collapsed.

## Part V. Proposed First Native Nodes

### SG17. The grammar should avoid generic binary operators where Core 2 has stronger named structure

Prefer:

- `ApplyTransform(state, transform)`
- `Pin(host, applied, at)`
- `Fold(kind, source)`
- `Repeat(source, index, law)`
- `InverseContinue(source, law, rule)`

over a generic node such as:

- `BinaryOp("*", left, right)`

### SG18. Literal nodes should wrap real Core 2 values

Examples:

- `ScalarLiteral(3)`
- `ProportionLiteral(2/1)`
- `AxisLiteral([3/1]i + [12/-1])`

### SG19. Named references are essential

To support recursive pinning, shared carriers, equations, and programs, the symbolic layer needs explicit references.

Examples:

- `Reference("Stem")`
- `Reference("Bowl")`
- `Reference("P4.u")`

### SG20. Pinning should be a first native symbolic form

The symbolic layer should not hide pinning behind generic relation syntax.

A first-class form like:

- `Pin(host, applied, at)`

matches the current Core 2 direction much better.

### SG21. Branch family should be a first native symbolic form

Branching is not just a display artifact.
If a law produces several candidates, the symbolic form should preserve that multiplicity directly.

### SG21a. Participant proposals should be preservable

When several structures each contribute constraints, desires, or local opinions, the symbolic layer should be able to preserve those inputs before final compromise.

This matters for:

- letterbox and glyph negotiation
- conflicting landmarks
- dynamic environments
- distributed evaluation

## Part VI. Equations And Constraints

### SG22. Equations should not be limited to arithmetic equality

The system needs at least:

- exact equality
- structural relation
- identity of carriers
- attachment
- routing
- hard constraints
- soft preferences

### SG23. Soft preference should remain lawful

Examples of symbolic preference:

- fit this glyph to this letterbox
- keep this crossbar near the midline
- maximize width
- minimize added tension

These should be representable without abandoning Core 2 terms.

### SG24. Letterbox behavior is a constraint problem, not merely a display hint

The letterbox should be able to participate as a structural actor with its own preferences.
The glyph should do the same.
The result is a negotiated structure, not one passive box and one active letter.

### SG24a. Constraint evaluation is distributed rather than centrally imposed

Any participating structure may contribute:

- fixed requirements
- agnostic regions
- weighted preferences
- certainty or confidence
- tension-bearing objections

The symbolic layer should therefore support multi-party contribution rather than assuming one global objective function speaking alone.

### SG25. Constraints should lower to lawful tension-bearing evaluation

A violated requirement or preference should not disappear into parser metadata.
It should become structural tension, unresolved multiplicity, or a lawful compromise.

## Part VII. Textual Shorthand

### SG26. There should be a compact textual shorthand

The symbolic layer should support a readable shorthand that can be:

- typed by humans
- pasted into AI prompts
- serialized from editors
- parsed back into symbolic terms

### SG27. The shorthand should preserve Core 2 asymmetries

Examples:

- `x * i` should remain state acted on by transform
- `A * B @ P` should preserve host/applied asymmetry
- side links such as `P4.u == P3.u` should remain explicit

### SG28. Surface syntax may be conventional where harmless and unusual where necessary

There is no requirement to imitate conventional math or programming languages if doing so would erase Core 2 structure.

### SG29. Canonical serialized form and friendly shorthand may differ

The system may keep:

- a canonical serialized form for exact replay
- a friendlier shorthand for human use

as long as both map to the same symbolic structure.

## Part VIII. Example Shapes

### SG30. Example: state-transform application

Surface form:

`1 * i`

Native symbolic meaning:

- `ApplyTransform(ScalarLiteral(1), TransformLiteral(i))`

### SG31. Example: positioned pinning

Surface form:

`A * B @ 1/2`

Native symbolic meaning:

- `Pin(Reference("A"), Reference("B"), ProportionLiteral(1/2))`

### SG32. Example: linked sides

Surface form:

`P4.u == P3.u`

Native symbolic meaning:

- exact side identity relation between two named side references

### SG33. Example: weighted preference

Surface form:

`prefer(crossbar ~= midline, 2/1)`

Native symbolic meaning:

- a soft preference with native `Proportion` strength

### SG34. Example: branch family

Surface form:

`sqrt(4)`

Native symbolic meaning:

- an inverse continuation request under squaring law
- yielding a branch family rather than a prematurely collapsed single result

### SG34a. Inverse continuation often reads as unfold, but is broader

One common human reading of inverse continuation is "unfold."
That is often a good intuition.

But the deeper meaning is:

- reverse a lawful continuation or repetition

So:

- roots
- logarithms
- unwrapping
- structural unfolding

are all special cases of inverse continuation.

## Part IX. Recommended Build Order

### SG35. Start with object-model grammar, not parser grammar

The first implementation should be:

- symbolic node types
- builders
- elaboration into structural graphs
- pretty-printers

Text parsing should come after the symbolic object model is stable.

### SG36. Reuse existing Core 2 values directly

Do not start by inventing parallel symbolic numeric classes.
Wrap and reuse existing Core 2 elements whenever possible.

### SG37. Add equations before full programs

The best early order is:

1. value and transform terms
2. pinning and relation terms
3. equation and constraint terms
4. branch and selection terms
5. program ordering and execution terms

### SG38. Let the first grammar clarify the axioms

The first symbolic grammar does not need to solve every future domain.
It only needs to make the current axioms computationally legible without distorting them.

### SG39. Default pinning may use host position plus applied origin

For a first symbolic grammar, it is acceptable to keep the current default:

- the host supplies the explicit pin position
- the applied element is pinned by its local origin

This matches the present `A * B @ P` direction.

### SG40. Future grammar should allow explicit anchor selection

As structures gain several internal landmarks or pins, the grammar will eventually need a way to say not only where on the host to pin, but which internal anchor of the applied structure is being pinned.

That future extension may look more like:

- host position plus applied anchor
- pin-to-pin relation
- anchor-addressed attachment

but it does not need to block the first symbolic layer.

## Summary

The symbolic layer should be:

- native to Core 2
- value-driven rather than metadata-driven
- tree-shaped when authored
- graph-shaped when elaborated
- expressive enough for equations, constraints, branching, and later programs

The first success condition is not "it parses math."
The first success condition is "it states Core 2 structure faithfully enough to be executed, folded, and reasoned about."
