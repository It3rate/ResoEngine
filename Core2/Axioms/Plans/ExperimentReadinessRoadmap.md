# Experiment Readiness Roadmap

This roadmap is the near-term technical sequence for getting `Core2`, `Core2.Symbolics`, and `Core2.Interpretation` into a tighter shape before starting a broad set of unrelated experiments.

The point is not to freeze the system before learning from it.
The point is to remove the largest avoidable sources of confusion first, so the experiments reveal Core 2 questions instead of library accidents.

## Current decision

The chosen order is:

1. finish the primitive resolution pass
2. modularize symbolic/runtime logic by operation family
3. reassess whether category/prototype work or n-dimensional generalization should come next
4. begin broader experimental surfaces with a tighter technical base

Category/prototype work remains important.
Arbitrary-dimensional generalization remains important.
Neither is first because the current highest-leverage risk is still that support behavior and symbolic runtime logic are too implicit.

## Phase 1. Finish Primitive Resolution

Goal:

- make support behavior explicit, executable, and shared between `Core2` and `Core2.Symbolics`

Main targets:

- complete the split between:
  - exact value
  - exact alignment support
  - committed result support
- route host-focused scale through explicit commitment logic
- keep pure transforms support-preserving where the primitive law is `Inherit`
- make the current demo pages report the same support story the runtime is actually using

Exit criteria:

- host/applied scale is no longer silently borrowing generic product support behavior
- `axis * i` and similar pure transforms remain support-preserving
- exact alignment support is no longer implicitly treated as the final committed support
- result-support policy exists in runtime, not only in docs and demo notes

## Phase 2. Modularize By Operation Family

Goal:

- reduce the amount of global special knowledge concentrated in a few large symbolic files

Primary pressure points:

- `Core2.Symbolics/Expressions/SymbolicParser.cs`
- `Core2.Symbolics/Expressions/SymbolicReducer.cs`
- `Core2.Symbolics/Expressions/SymbolicConstraintEvaluator.cs`

Target family split:

- transform application and pure transform laws
- value arithmetic and resolution-sensitive combination
- repetition, power, and inverse continuation
- structural queries and structural-context evaluation
- constraints and negotiation
- program/binding/commit flow

Exit criteria:

- adding one new operation family no longer requires editing several unrelated regions in one file
- the symbolic reducer is easier to reason about by domain
- resolution-sensitive rules live near each other instead of being mixed into unrelated paths

## Phase 3. Reassess The Next Structural Gap

After phases 1 and 2, choose between:

- category/prototype engine
- arbitrary-dimensional design

Likely selection rule:

- if the first experiments are mostly flow, forces, electricity, music, and movement, category can wait a little longer
- if the first experiments are mainly design interpretation or language interpretation, category/prototype work becomes urgent
- if the first experiments keep running into hard-coded `Axis -> Area` ceilings, move n-dimensional design forward immediately

## Category / Prototype Work

Not first, but important.

Goal:

- classify by weighted structural participation, ranges, and preferences without demanding one rigid defining value

The expected direction is to build this on top of the existing constraint/preference machinery rather than inventing a detached second system.

## Arbitrary-Dimensional Design

Also not first, but important.

Goal:

- keep friendly names like `Axis`, `Area`, and `Volume`
- introduce a more dimension-agnostic underlying structure so folding, opposition, pinning, repetition, and inversion are not hard-coded to the current ceiling

This should begin as a design pass before code.
The likely benefit is not only higher dimensions.
It may also simplify existing code by revealing the shared recursive pattern more clearly.

## Practical Success Condition

This roadmap is working if:

- new experiments mostly expose Core 2 model questions
- fewer experiments are blocked by implicit support behavior
- fewer changes require touching the biggest symbolic files
- category and n-dimensional work can be approached from a cleaner base instead of as emergency fixes
