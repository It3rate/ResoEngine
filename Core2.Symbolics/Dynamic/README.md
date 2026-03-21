# Core 2 Symbolic Dynamic

This folder contains the symbolic dynamic execution layer.

The goal here is to let proposal, resolution, convergence, and trace-like behavior be expressed symbolically while still using Core 2 structure underneath. It is not meant to be a detached workflow engine with a thin Core 2 adapter on the side.

## What Lives Here

This area handles things like:

- symbolic dynamic contexts
- strands and proposals
- resolution inputs and outcomes
- dynamic traces
- convergence policies

## Mental Model

If `Expressions` is where symbolic structure is authored and reduced, `Dynamic` is where symbolic structure begins to act over time.

This is the place to look when one step is not enough, and the system needs to:

- propose
- branch
- negotiate
- commit
- continue

while preserving Core 2 structure along the way.
