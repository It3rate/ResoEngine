# Core 2 Support

This folder is reserved for shared support-layer helpers in Core 2.

At the moment it is intentionally light. That does not mean the idea is unimportant. It means the right shared helper surface is still being discovered as the core libraries tighten.

## Intended Role

This folder is the likely home for small cross-cutting support utilities that do not belong to one specific domain like:

- elements
- units
- branching
- repetition
- symbolic expressions

## Mental Model

Think of this as infrastructure support, not semantic support.

If something is:

- widely reusable
- low-level
- not itself a primitive Core 2 concept

it may eventually belong here.

## Current Status

It is normal for this folder to stay sparse until more of the shared patterns are genuinely stable.
