# Core 2 Axioms

This folder is organized by role rather than by date.
The goal is to keep one clear source of truth for settled doctrine while still preserving deeper design context and implementation history.

## Primary spec

- [PromptAxioms.md](./PromptAxioms.md)
- [PromptAxioms.txt](./PromptAxioms.txt)

These are the living Core 2 axioms.
`PromptAxioms.txt` is the prompt-safe mirror used for AI/context injection.
`PromptAxioms.md` is the human-readable version.
If a concept is considered settled enough to feed back into the AI, it belongs there first.

## Supporting folders

- `Appendices/`
  Narrow implementation-aligned rule sheets that elaborate one area without replacing the main spec. These often track focused behavior in `Core2`, `Core2.Symbolics`, or `Core2.Interpretation`.
- `Proposals/`
  Exploratory conceptual documents. These are useful design references but are not themselves the source of truth.
- `Plans/`
  Implementation planning documents.
- `Legacy/`
  Older merged source documents retained for provenance after consolidation into the main prompt axioms.

## Maintenance rule

When an idea becomes part of the settled Core 2 model, update `PromptAxioms` first.
Supporting docs should then either:

- stay as deeper context if they still add value, or
- move to `Legacy/` if they are superseded.

## Practical reading order

1. Read `PromptAxioms.md` for the current model.
2. Use `Appendices/` when one subsystem needs more detail.
3. Use `Proposals/` when tracing how a concept evolved or where it may go next.
4. Use `Plans/` when coordinating implementation work.
5. Use `Legacy/` only for provenance or recovery of older phrasing.
