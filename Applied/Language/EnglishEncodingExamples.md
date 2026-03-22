# English Encoding Examples

These examples are intentionally loose.
They are not parser syntax.
They are draft "Core 2-shaped" readings meant to make the encoding approach inspectable.

The guiding question is:
"If this English expression is not arbitrary, what recurring structural relation is it pointing at?"

## Locative and Prepositional Readings

### `inside`

English idea:
The figure occupies the interior of a host.

Approximate encoding:

```text
inside(figure, ground) := locate(figure, interior(frame(ground)))
```

Reading:
The `ground` provides a frame or boundary, and `figure` is committed to the lawful interior region.

### `near`

English idea:
The figure is in a surrounding admissible band, but not occupying the target itself.

Approximate encoding:

```text
near(figure, target) := choose(site in cover(target) NOT target)
```

Reading:
This is close to the boolean reading you suggested.
The `cover(target)` region is broader than the `target` itself, and `near` selects from the difference.

### `between`

English idea:
The figure is inside the bounded region defined by two anchors.

Approximate encoding:

```text
between(figure, left, right) := locate(figure, interior(span(left, right)))
```

Reading:
This is not mere nearness to either anchor.
It is a bounded-interval relation.

## Temporal Readings

### `during`

English idea:
An event occupies an interval.

Approximate encoding:

```text
during(event, interval) := occupy(event, interior(interval.start -> interval.end))
```

Reading:
The temporal carrier is treated very much like a spatial host span.

### `until`

English idea:
Continue toward a boundary and stop or change there.

Approximate encoding:

```text
until(process, boundary) := continue(process) while before(boundary)
```

Reading:
This is a temporal boundary or gate.
It also overlaps with control flow and obstruction.

### Past span example

English:
`last week I was hoping to go on a 3 day holiday`

Approximate encoding:

```text
hope_span := (7i - 4)
```

Reading:
This is the kind of physical temporal encoding you described.
The span begins seven days in the past and runs forward three days, ending four days in the past.
The sentence is then not just "hope" plus a date label, but a structured remembered interval.

## Motion and Path Readings

### `approach`

English idea:
Move with decreasing distance toward a goal before full contact is reached.

Approximate encoding:

```text
approach(agent, goal) := continue(agent) with distance(goal) decreasing
```

Reading:
This is source-goal plus adjacency pressure.

### `through`

English idea:
Enter, occupy the interior, and exit.

Approximate encoding:

```text
through(path, region) := enter(region) -> occupy(interior(region)) -> exit(region)
```

Reading:
That same structure works for spatial regions, temporal phases, guarded program states, and circuit gates.

### `around`

English idea:
Preserve a surrounding route family before one exact folded path is chosen.

Approximate encoding:

```text
around(path, target) := choose(loop in surround(target))
```

Reading:
This is useful because `around` is often less one exact path than a lawful family of surrounding positions.

## Scalar and Hierarchy Readings

### `very tall`

English idea:
Take an extent reading and increase its intensity rather than inventing a new primitive.

Approximate encoding:

```text
very_tall(x) := weight(high, tall(x))
```

Reading:
`tall(x)` is an extent relation on an active vertical dimension.
`very` is a weighting or intensity overlay.

### `upper management`

English idea:
Use vertical organization to encode abstract rank.

Approximate encoding:

```text
upper_management := management @ upper_band(hierarchy(company))
```

Reading:
This treats hierarchy as a frame with oriented bands rather than as a list of arbitrary tags.

### `close friend`

English idea:
Reuse proximity in social space.

Approximate encoding:

```text
close_friend(person) := friend(person) in near_band(self.social_frame)
```

Reading:
The word `close` does not have to be purely metaphorical in the bad sense.
It can be a lawful reuse of adjacency in a different host frame.

## Why This Matters

The goal is not to force English into geometric diagrams for their own sake.
The goal is to find a small set of structural relations that:
- already make sense in Core 2
- already appear in letters, graphs, and circuits
- can be reused in language without becoming arbitrary tags

If the same families keep reappearing, then the metaphor is probably doing real work.
