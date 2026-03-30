# Core3 Coding Style

This note records the current implementation style for `Core3`.

It is not meant to be a generic C# style guide.
It is meant to keep the code aligned with Core3's structural aims.

## Main Principle

Prefer code that makes structure and invariants easy to see.

Avoid code that mainly protects against misuse by adding repeated visual noise
without adding much explanatory value.

## Records And Passive Shapes

When a type is mainly a passive structural carrier, prefer plain records or
small constructors.

Examples:

- context wrappers
- operation metadata
- binding selectors
- result containers
- serialization-facing schema objects

These should usually not carry many repeated null checks or string guards.
If the type is just holding shape, let the shape stay visible.

## Guards And Throws

Keep explicit guards where they express a real invariant.

Good reasons to keep a throw:

- a mover position is outside its lawful range
- a composite is built from children of mismatched grade
- a boolean result is being read from the wrong structural shape
- an internal conversion cannot be represented exactly
- a supposedly impossible state has actually occurred

Less useful reasons:

- repeating `ThrowIfNull` on every passive record constructor
- repeating the same writer/object null checks through every serializer method
- guarding internal helper paths where the surrounding type system already
  makes the intended contract clear

In short:

- keep throws for broken invariants and impossible states
- do not use them as routine visual padding

## Try-Style Failures

If failure is an expected part of ordinary operation, prefer `Try...` style
results over exceptions.

Examples:

- alignment may fail
- a fold may not exist
- a selector may not resolve
- a traversal may already be at its stop

These are usually not exceptional in the Core3 sense. They are ordinary
structural outcomes.

## Serializer Style

`Core3.Serialization` is intentionally manual and explicit.

That should not turn into repetitive boilerplate.

Prefer:

- one clear structural walk
- readable emitted shape
- only the checks that still protect a meaningful boundary

Avoid turning each write overload into the same two guard lines plus one small
body.

## Physicality

When several implementations are possible, prefer the one that preserves the
physical or structural reading.

Examples:

- a mover should look like a real located iterator
- a route should read like a traversable structure
- selector parameters should prefer numeric/Core3-shaped meanings when
  possible
- attached laws should remain distinct from the structure they act on

This keeps the code closer to later visualization, traversal, and tension-based
adjustment.

## Present Working Rule

For current Core3 work:

- keep passive schema types light
- keep semantic invariants explicit
- prefer `Try...` for expected non-success
- reserve exceptions for genuine invariant breaks
- avoid boilerplate that makes the code harder to read without teaching
  anything

If a future edit adds a lot of repeated guard code, that is usually a signal to
pause and ask whether the check belongs at a smaller number of real boundaries
instead.
