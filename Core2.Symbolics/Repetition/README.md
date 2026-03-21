# Core 2 Symbolic Repetition

This folder contains the symbolic repetition layer.

Its job is to let symbolic terms talk about repetition, power, and inverse continuation while still reducing through the native Core 2 repetition engines. So this area is not a separate exponentiation library. It is the symbolic surface for repetition laws already present in Core 2.

## What Lives Here

This area handles things like:

- symbolic power support
- inverse continuation support
- repetition traces
- quantity-aware power resolution
- extension methods that connect symbolic terms to native repetition behavior

## Mental Model

Use this folder when a symbolic expression needs to say:

- continue
- repeat
- raise to a power
- reverse a continuation
- preserve all candidates or choose a principal one

This is where symbolic authoring and native repetition law meet.
