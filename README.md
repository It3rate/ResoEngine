# ResoEngine

ResoEngine is a workspace for building and testing Core 2.

At the center of the solution is `Core2`, which is a structural math system rather than only a traditional numeric or geometry library. It is designed to preserve things that most libraries collapse early: resolution or support, directional duality, perspective, pinning before folding, branch structure, and partial resolution over time. The aim is not only to do math in the conventional sense, but to support concepts, language, design, dynamic systems, and other domains where meaning develops through structure rather than appearing as one immediate scalar answer.

Two companion libraries sit directly beside it:

- `Core2.Symbolics`
  gives Core 2 a symbolic authoring, execution, and inspection surface
- `Core2.Interpretation`
  gives Core 2 contextual readings in placement, traversal, analysis, and measured form

These libraries are not meant to merely "use" Core 2 from the outside. The deeper goal is that they remain self-contained in Core 2 terms. A branch should be representable through Core 2 branch structure. A direction change should still be encoded through Core 2 directional structure. A traversal, route, orthogonal lift, or layered reading should be described using the same native vocabulary rather than replaced with a completely separate model.

## Big Picture

In simple terms, the solution is trying to build one coherent stack:

1. `Core2`
   The primitive structural substrate.
2. `Core2.Symbolics`
   A symbolic way to author, reduce, inspect, negotiate, and commit Core 2 structure.
3. `Core2.Interpretation`
   A way to read that structure in context: placed, traversed, analyzed, or measured.
4. applied and visual layers
   Pages, demos, and experiments that exercise the system in concrete domains.

This stack matters because the end goal is larger than conventional calculation. The system is intended to be expressive enough for:

- structural geometry
- resolution-aware measurement
- traversal and flow
- partial solving over time
- dynamic proposal and negotiation
- concept and language modeling

## What Makes This Different

A traditional library usually tries to reduce quickly:

- one final value
- one coordinate model
- one unambiguous reading

ResoEngine is exploring a different bias:

- values may remain structured
- support and folded value are different things
- perspective can change the reading without changing the thing
- pinning can exist before folding
- branches and ambiguity can remain first-class
- interpretation can remain partial until more context is available

That bias is especially important for language. Language depends on focus, perspective, carried context, competing readings, and gradual commitment. Core 2 is being shaped so that those are not just external annotations, but lawful parts of the mathematical system.

## Main Areas

The most important solution folders are:

- [Core2](C:/Users/Robin/source/repos/ResoEngine/Core2)
  The primitive library and axiomatic center.
- [Core2.Symbolics](C:/Users/Robin/source/repos/ResoEngine/Core2.Symbolics)
  Symbolic terms, parsing, reduction, constraint evaluation, negotiation, and inspection.
- [Core2.Interpretation](C:/Users/Robin/source/repos/ResoEngine/Core2.Interpretation)
  Structural analysis, placement, traversal, layered reading, and contextual interpretation.
- [Applied](C:/Users/Robin/source/repos/ResoEngine/Applied)
  Applied-domain experiments and supporting higher-level utilities.
- [Visualizer.WinForms.Core2](C:/Users/Robin/source/repos/ResoEngine/Visualizer.WinForms.Core2)
  The main interactive desktop visualizer for exploring the current Core 2 system.
- [Tests.Core2](C:/Users/Robin/source/repos/ResoEngine/Tests.Core2)
  Focused test coverage for the core and symbolic behavior.

## Suggested Reading Order

If you are new to the solution, a good starting path is:

1. [Core2/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2/README.md)
2. [Core2/Axioms/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/README.md)
3. [Core2/Elements/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Elements/README.md)
4. [Core2.Symbolics/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2.Symbolics/README.md)
5. [Core2.Symbolics/Expressions/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2.Symbolics/Expressions/README.md)
6. [Core2.Interpretation/README.md](C:/Users/Robin/source/repos/ResoEngine/Core2.Interpretation/README.md)

After that, the demos and visual pages tend to read much more clearly.
