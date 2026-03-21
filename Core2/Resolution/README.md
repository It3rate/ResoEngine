# Core 2 Resolution

This folder contains the primitive support and resolution laws of Core 2.

The key idea here is that folded value and support are not the same thing.

Very loosely:

- folded value is what an expression collapses to
- support or resolution is what substrate of distinctions is being preserved

## What This Folder Is For

This is where the primitive laws are being made explicit, especially around questions like:

- when support is inherited
- when supports aggregate
- when supports compose
- when a value is only being refined or re-expressed
- when a temporary common frame is used for exact alignment
- how committed result support may differ from exact alignment support

## Mental Model

Use this folder to understand the denominator side of Core 2 reasoning.

Not as “just fractions,” but as a structured resolution system that has to support:

- measurement
- evidence pooling
- scaling
- alignment
- later uncertainty-aware commitment

## Scope

This folder should stay primitive and law-oriented. It should define the small number of support behaviors the rest of the system can rely on, rather than accumulating many special-case policies.
