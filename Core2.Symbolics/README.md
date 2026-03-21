# Core 2 Symbolics

`Core2.Symbolics` is the symbolic authoring, execution, and inspection layer for Core 2.

Its purpose is not to translate Core 2 into an ordinary generic AST and then forget where it came from. Its purpose is to give Core 2 a symbolic surface that stays faithful to the structure underneath. That means this layer tries to preserve things that a more traditional expression engine would often collapse too early, such as host versus applied asymmetry, support or resolution, branch families, structural relations, partial reduction, and later commitment.

Just as importantly, this library is not only "using" Core 2 from the outside. It is trying to remain self-contained in Core 2 terms. A branch is not merely a foreign control-flow idea attached afterward; it is represented through Core 2 branch structure. A transform is not a detached operator language; it is expressed through Core 2 elements acting in transform position. Even metaphorical or higher-level behaviors are meant to be encoded using the same native structural vocabulary wherever possible.

This matters because Core 2 is not only trying to support ordinary math expressions. It is trying to support concepts, language, design, dynamic systems, and other domains where the "right answer" may not appear as one immediate fully solved scalar. In language especially, meaning depends on focus, perspective, carried structure, ambiguity, and partial interpretation. The symbolic layer is where those kinds of authored structures can live without being flattened too early.

## What This Library Does

At a high level, this library lets Core 2 be:

- written as symbolic terms
- parsed and elaborated into typed structure
- reduced through native Core 2 behavior
- evaluated against structural context
- negotiated when several participants or candidates are involved
- inspected, formatted, serialized, and committed

So this library is both:

- a symbolic surface for authored expressions
- a runtime-support layer for executing the symbolic subset that already has native Core 2 meaning

## What Makes It Different

Several things here are intentionally unlike a conventional expression engine.

### Expressions are Core 2-native, not generic first

This layer prefers explicit Core 2 forms such as:

- transform application
- pinning
- branch families
- constraint sets
- continuation and inverse continuation
- structural relations like shared carriers and routes

instead of beginning from one generic binary operator model and trying to reinterpret it later.

The goal is that the symbolic layer can speak about higher-level things while still using Core 2 itself as the representation substrate. In other words, it should not merely annotate Core 2 objects from outside; it should try to express symbolic meaning through Core 2 structure directly.

### Reduction can remain partial

A symbolic term does not need to become one final scalar immediately.

It may lawfully reduce to:

- a value
- a branch family
- a constrained unresolved relation
- a negotiated representative
- a committed environment binding

That behavior is important for ambiguity, inverse continuation, and concept or language work.

### Context is often part of the meaning

Some symbolic terms are self-contained. Others need:

- a symbolic environment
- a carrier graph or structural context
- a selection rule
- a support policy

So "unresolved" often means "waiting for lawful context," not "failed."

## Mental Map

A good way to think about this library is as a pipeline:

1. author a symbolic expression
2. parse it into symbolic terms
3. elaborate references and structure
4. reduce the parts that already have native Core 2 behavior
5. evaluate constraints or structural relations if needed
6. negotiate or commit when one representative is required
7. format, serialize, inspect, or display the result

The library is split into a few broad areas:

- `Expressions`
  The main symbolic term system, parser, reducer, evaluator, formatter, serializer, and inspection tools.
- `Repetition`
  Symbolic power and inverse-continuation support over the native repetition engines.
- `Branching`
  Branch-graph helpers and symbolic branch projection support.
- `Dynamic`
  Symbolic dynamic execution support for proposal, resolution, and trace-like behavior.

## Relationship To Core 2

Very roughly:

- `Core2` defines the native object model and primitive laws
- `Core2.Symbolics` gives those laws a symbolic surface
- `Core2.Interpretation` turns those structures into placed, traversed, analyzed, or measured interpretations built from the same underlying Core 2 structures

So this library lives between the primitive substrate and the applied/demo/editor layers.

## Suggested Reading Order

If you are new to this library, a good path is:

1. the root [README.md](C:/Users/Robin/source/repos/ResoEngine/Core2/README.md) in `Core2`
2. [Expressions/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2.Symbolics/Expressions/README.md)
3. then the term definitions and the parse -> elaborate -> reduce flow
4. then constraints, negotiation, and inspection

That order usually makes the rest of the symbolic files much easier to understand.
