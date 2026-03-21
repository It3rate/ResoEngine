# Core 2 Symbolic Expressions

This folder is the symbolic expression layer for Core 2.

Its job is not to bolt a generic math AST on top of Core 2. Its job is to give Core 2 a symbolic surface that stays native to the system underneath.

That means the symbolic layer tries to preserve things like:

- host vs applied asymmetry
- pinning and attachment
- branch families and unresolved multiplicity
- support and resolution
- structural relations such as shared carriers, routes, and junctions
- later selection, commitment, and inspection

## High-Level Goals

The expression engine is meant to do a few things at once:

1. Give humans and tools a compact way to author Core 2 structure.
2. Preserve Core 2 meaning instead of flattening everything into ordinary scalar algebra.
3. Support gradual interpretation.
4. Let one expression stay symbolic, reduce partially, branch, negotiate, or resolve later when more structure is available.
5. Make the system inspectable enough that pages, tests, and later AI tools can all follow the same pipeline.

## Mental Map

The easiest way to think about this folder is as a pipeline.

### 1. Author a symbolic term

An authored term is the readable surface form.

Examples:

- `1 * i`
- `fold([3/1]i + [2/1])`
- `share(P4.u, P3.u)`
- `let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}`

At this stage the expression is still close to how a person would write or inspect it.

### 2. Parse it into symbolic structure

The surface form becomes a typed symbolic term tree.

This is tree-shaped at the authored level because that is a natural way to write expressions. But Core 2 meaning is often more graph-shaped after elaboration, because sharing, pinning, branching, and rejoining are not purely tree-like phenomena.

### 3. Elaborate it

Elaboration resolves references, rebuilds the term with the right typed substructure, and prepares it for execution or inspection.

This is where a symbolic phrase starts to become a lawful Core 2 object rather than only a surface syntax fragment.

### 4. Reduce the native executable parts

Reduction executes the parts that already have direct Core 2 meaning.

Examples:

- pure transform application
- repetition and inverse continuation
- structural reads from a carrier graph
- program flow like `let`, `commit`, and sequence

Reduction does not force everything to collapse. If something is genuinely branchful or unresolved, that structure can remain present.

### 5. Evaluate and negotiate constraints when needed

Some terms are not just values. They are relations, requirements, preferences, or competing local proposals.

Those can be:

- evaluated directly
- left unresolved when structural context is missing
- negotiated into a selected representative
- preserved as a candidate family or tension when one answer is not yet justified

### 6. Format, serialize, inspect, or commit

The same symbolic term can then be:

- shown in a readable shorthand
- serialized canonically
- traced step by step for inspection
- committed back into an environment
- used by a page or experiment as part of runtime behavior

## What Is Different From A Normal Expression Engine

This layer looks like an expression engine, but several parts are intentionally different from a conventional AST or algebra system.

### Host and applied are asymmetric

`x * t` does not mean “two peers under a generic multiplication operator.”

It means:

- `x` is the current state or host
- `t` is what acts on that host

This matters for interpretation, provenance, and now increasingly for resolution behavior.

For example, a support-preserving transform such as opposition is not the same thing as composing two independent support spaces.

### Support is not just a denominator

Core 2 treats folded value and support as different facts.

Very loosely:

- folded value says what the expression collapses to
- support or resolution says what substrate of distinctions is being preserved

So `4/5`, `8/10`, and `400/500` may fold to the same scalar while still meaning different things structurally.

This is why the symbolic layer now leans on primitive support laws such as:

- inherit
- aggregate
- compose
- refine
- common-frame alignment

And it is why exact alignment support and committed result support are not always the same thing.

### Branching is first-class

Many systems force ambiguity to disappear too early.

This one does not need to.

An expression may lawfully become:

- a single value
- a branch family
- a selected representative plus preserved alternatives
- unresolved tension

That is important for inverse continuation, structural ambiguity, later context selection, and eventually for language.

### Structural relations matter as much as values

This layer is not only about arithmetic-like expressions.

It also represents things like:

- shared carriers
- routes through sites
- junction kinds
- site flags
- carrier flags
- counts and anchor positions read from live structural context

So a symbolic term may be asking about structure, not only computing a value.

### Context can be required

Some expressions are self-contained.

Others only become decidable when they are given:

- bindings from an environment
- a structural context such as a carrier graph
- a selection rule
- a result-support policy

So “unresolved” often means “lawfully waiting for context,” not “broken.”

## Rough Folder Shape

At a high level, this folder is organized around a few roles:

- symbolic term definitions
- parser support
- elaboration
- reduction
- constraint evaluation and negotiation
- formatting and serialization
- inspection and reporting

You do not need to understand every category at once. The intended reading order is:

1. understand the kinds of terms
2. understand the parse -> elaborate -> reduce flow
3. then look at constraints, negotiation, and inspection

## Example Lifecycles

### Example 1: `1 * i`

Surface idea:

- start with the state `1`
- apply opposition

Typical lifecycle:

1. Parse the shorthand into a transform application term.
2. Elaborate it into typed value and transform nodes.
3. Reduce it through native Core 2 transform logic.
4. Produce `i` as the reduced result.

The important part is that this is not treated as a generic binary operator. It is state plus transform.

### Example 2: `share(P4.u, P3.u)`

Surface idea:

- ask whether two anchors resolve onto the same carrier

Typical lifecycle:

1. Parse it into a relation term.
2. Elaborate the anchor references.
3. Evaluate it.
4. If no structural context is present, the result may remain unresolved.
5. If a carrier graph is present, it can become satisfied or unsatisfied.

So the same symbolic term can be either a deferred question or a resolved structural fact depending on context.

### Example 3: `let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}`

Surface idea:

- create an alternative family
- express a participant preference
- negotiate and commit the result

Typical lifecycle:

1. Parse the program-like sequence.
2. Elaborate the branch family and references.
3. Reduce what can be reduced directly.
4. Evaluate the preference over the candidate family.
5. Negotiate a representative if justified.
6. Commit the chosen result, or preserve a family if a unique choice is not justified.

This is a good example of why the symbolic layer is not just “expression parsing.” It also supports distributed proposals, selection, and preserved ambiguity.

## What To Expect When Reading This Folder

A few things are worth expecting up front:

- There are many small term types on purpose.
- A lot of the complexity is in preserving meaning, not in inventing syntax.
- Some files are about authoring surface forms, while others are about lawful Core 2 execution.
- The system prefers preserving branch, support, and structural provenance instead of simplifying too early.

If a behavior looks unusual compared to a standard expression engine, it is usually because this layer is trying to stay faithful to Core 2 rather than to conventional algebra or compiler design defaults.
