# Core 2 Branching

This folder contains the shared data structures for multiplicity in Core 2.

Branching is not treated here as an error case or a temporary parsing artifact. It is a real part of the system.

## What This Folder Holds

This is where Core 2 records things like:

- alternative candidates
- co-present result families
- branch provenance
- selection state
- branch semantics and direction
- preserved tensions around unresolved choice

## Mental Model

Use this folder when you want to answer:

- what candidates exist
- where they came from
- whether one is selected
- whether they are competing or jointly present

This is the structural vocabulary of multiplicity. It does not itself decide how a branch arose or which one should win.

## Scope

Generation, evaluation, and negotiation of branches usually happen elsewhere. This folder provides the common branch-shaped objects those other layers share.
