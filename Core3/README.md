# Core3 Overview

`Core3` is an attempt to build a very small system from which a much wider
range of patterns can emerge.

All things are indistinguishable, or ordered in some way. 
Order begins in mathematics, but it is not only for mathematics. The hope is to
make a system that can talk about magnitude, resolution, structure, change,
ambiguity, tension, and perspective. In that sense it is meant
to be useful not only for numbers and geometry, but also for concepts and
language. A physics for concepts.

Two ideas matter especially early:

- resolution belongs inside the structure, not just in the display
- all information is created by tension between two opposing perspectives

That means a value is not only "how much." It also carries something about how
finely it is being distinguished, and from which perspective it holds. Those
two facts turn out to create a surprising amount of later structure.

## A System Of Relations

At its heart, `Core3` grows by intentionally coupling two things.

A simple number, a measured interval, a directed span, a reference frame, a
folded result, even a tension that has not yet settled, can all be understood
as different readings of structure that came from putting two things into
relation and deciding how to read the result.

This is why the system keeps returning to pairs:

- two sides
- two perspectives
- two compared things
- two structures under one relation
- inbound and outbound flows

From that binary relation, a new thing can be created. And in the other direction,
what looks like one thing may be unfolded back into the pair that made it.

## Grades

This is what grade means in `Core3`.

A higher grade is not just "more data." It is a new level of relation made from
two lower ones. Each time that happens, new kinds of symmetry become available.
Something can now be read not only as an amount, but as a direction, a span, a
frame, a surface, a branching relation, or something else still forming.

The important point is that these richer behaviors are not meant to be pasted
on from outside as extra labels - they can be read directly from the
structure itself. A lot of expressive power falls out of
very small rules applied repeatedly.

## Folding And Unfolding

One of the main distinctions in `Core3` is the difference between creating a
relation and deciding what it means.

First, things are brought together.
Then, later, they may be folded into a more committed reading. Think joining
two values into a multiplicative relation. You can use these two values as
a XY graph, and later you can fold them into an area. These are distinct structures.

Sometimes that fold gives a lower-grade result.
Sometimes it preserves the current structure.
Sometimes it branches.
Sometimes it cannot be completed honestly without lifting into a richer form.

That is why the system cares so much about reverse continuation (e.g. square roots) as well. If a
structure can fold down, it should also make sense to ask how it might unfold
again, or how several valid antecedents might still remain present. An area could 
unfold into two equal sides, or you could preserve both the area and graph's X and Y.

This is part of why the system feels close to language. Meaning also works this
way. A phrase can stay open for a while at a low resolution, then later settle into 
one reading, or split into several lawful ones.

## Tension Is Real Structure

`Core3` does not want contradiction to disappear too early.

When two related things do not fit cleanly in the same frame, that mismatch is
not always an error. Sometimes it is the very thing that creates a new
direction, a new grade, or a new kind of space.

So tension is not only a problem to remove. It is often a record that something
important has happened but has not yet been fully resolved. In mathematics this
can look like a lift into a new structure. In language it can look like
ambiguity, emphasis, pressure, or competing interpretations that are still
alive. Seeing 2+3=7 from some perspective is wrong, but it is also information. And
we don't need to commit to the 7 being the problem, it might be the 2.

## Measurement And Shared Frames

Another central idea is that one structure can be used to measure another.

A graph can be used to place data.
A ruler can measure an external object.
A scale can weigh something placed on it.
A dog can be compared to a cat.
A reference frame can lend its calibration without being copied into every
thing it measures.

So `Core3` is not only about objects. It is also about the spaces, frames, and
local origins by which objects become readable.

This is part of why pinning, reference, and shared calibration matter so much.
They are all ways of saying that one thing may help another thing become
measurable, locatable, or meaningful.

## Resolution And Bounds

Resolution matters because it tells us what the smallest active distinction is.
But every active frame also has a largest meaningful span.

So the system is bounded in both directions:

- there is a lower bound on what can be distinctly resolved
- there is an upper bound on what the current frame can still honestly carry

This is important because it keeps infinity from being the silent default.
Things can run out of room in the small direction and the large direction. 
Both facts shape behaviour at the edge - wrapping, reflecting, clamping, or 
generating new tension.

## More Than Math

All of this clearly relates to mathematics, but the intention is broader.

The hope behind `Core3` is that the same structural ideas can help describe how
concepts behave, how language shifts meaning with perspective and emphasis, how
ambiguity stays alive before selection, and how tensions between readings can
produce new structure rather than just failure.

In that sense, `Core3` is aiming at a mathematics that can become a kind of
physics for concepts: something structured enough to go beyond just describing,
to also run, evalutate, and predict.

## Current Status

`Core3` is still deliberately small.

The goal is not to define every category early. The goal is to get the
primitive moves clear enough that richer mathematics, richer geometry, and
richer conceptual behavior can all grow from the same small foundation.

An experimental `Core3.Binding` namespace is also being used to explore bound
literals, contextual defaults, slot coupling, and site-attached operation
metadata without pushing those concerns into the engine core too early.

The current direction there is to prefer small navigable Core3 value spaces for
descriptor meaning, so later adjustment and exploration can happen through
structure rather than only through opaque enum choices.
It is also moving toward a more physical traversal picture where a real moving
container or trolley supplies the current encounter and bindings use numeric
positions relative to that mover rather than a pile of special-case selector
tokens. The current `Core3.Binding` experiment now also treats that mover as a
small exact atomic iterator, so loop progress stays inspectable as structure
rather than becoming hidden iterator bookkeeping.

`Core3.Serialization` now also provides a manual JSON writer so the current
engine, runtime, binding, and operation shapes can be inspected on the wire
without committing the codebase to serializer attributes too early.

A short implementation note for how we want Core3 code to feel and where we do
and do not want heavy guard boilerplate now lives in
[CodingStyle.md](CodingStyle.md).
