# Core 2 Symbolic Branching

This folder contains the symbolic branching layer.

Its role is to carry branch families, branch graphs, and symbolic branch projection without leaving the Core 2 representational world. It is not just borrowing a graph abstraction from elsewhere. It is trying to let symbolic multiplicity stay grounded in Core 2 branch structure and provenance.

## What Lives Here

This area handles things like:

- symbolic branch graphs
- branch events and frontiers
- branch adapters between native families and graph history
- projection and mapping support for symbolic branch results

## Mental Model

Use this folder when symbolic multiplicity needs to persist over time instead of remaining one local branch family only.

`Core2/Branching` gives the primitive branch objects. This folder gives the symbolic history and projection layer built on top of them.
