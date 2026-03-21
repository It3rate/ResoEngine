# Core 2 Algebra

This folder contains the low-level arithmetic and transform laws used by Core 2 elements.

Its role is not to be a general symbolic algebra system. Its role is to answer questions like:

- how native element types combine
- how transform-position values act on host-position values
- how basis and opposition cycles are represented
- how arithmetic dispatch stays lawful across Core 2 element kinds

## What To Expect

This is the place where Core 2's non-symmetric arithmetic gets encoded.

In particular, this folder is concerned with things like:

- host/applied asymmetry
- basis-aware multiplication
- rational exponents and related arithmetic support
- dispatch rules between element types

## Mental Model

If `Elements` defines what the native objects are, `Algebra` defines how they act on one another.

Read this folder as:

- arithmetic law
- transform law
- dispatch law

rather than as a general-purpose math toolkit.

## Scope

This folder should stay fairly small and law-focused.

If something is mainly about:

- syntax
- inspection
- negotiation
- interpretation of larger structures

it probably belongs elsewhere.
