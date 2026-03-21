# Core 2 Units

This folder contains the unit and quantity layer of Core 2.

Core 2 separates structural degree from unit-kind. This folder is where unit-kind and quantity interpretation live.

## What Lives Here

This is where the system represents things like:

- unit generators
- unit signatures
- quantities
- unit tensions
- physical referents
- presentation choices for named units

## Mental Model

Read this folder as the measurement and interpretation layer that sits beside structure, not inside it.

In other words:

- `Elements` says what kind of structural thing something is
- `Units` says how it is measured and interpreted physically

## Important Distinctions

This layer tries to keep separate:

- structure vs unit-kind
- signature vs named unit
- exact algebra vs physical meaning
- lawful combination vs informative tension

That separation is important for later experiments in science, design, language, and mixed-domain modeling.

## Scope

If a problem is mainly about signatures, quantities, physical referents, or unit compatibility, it likely belongs here. If it is mainly about branch, pinning, or symbolic execution, it likely belongs elsewhere.
