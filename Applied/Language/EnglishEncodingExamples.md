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
- already appear in dynamic equations and iterative control
- can be reused in language without becoming arbitrary tags

If the same families keep reappearing, then the metaphor is probably doing real work.

## One Shared Pattern Across Domains

Here is one moderately structured pattern that uses traversal, comparison, branching, and a stopping band.

Approximate shared structure:

```text
state := source
repeat:
  state := advance(state)
  if state > gate then branch(high_path, low_path)
  if state inside target_band then stop
```

This is not meant to be parser syntax.
It is a rough schematic.

### As a dynamic equation

```text
x_0 := -3
advance(x_t) := x_t + 2
if x_t > 4 then branch(cool_down, continue_rise)
stop when x_t inside [5, 6]
```

Reading:
- `x_t` traverses an ordered carrier
- `x_t > 4` is a boundary comparison
- `branch(...)` is a continuation split
- `[5, 6]` is a bounded target region

### As a spatial route

```text
walker := trail_start
repeat:
  walker := move_along(trail)
  if walker beyond ridge_gate then branch(high_ridge, low_valley)
  if walker inside camp_band then stop
```

Reading:
This is almost the same structure with a different host frame.

### As a narrative or story pattern

```text
hero := opening_state
repeat:
  hero := advance_toward(goal)
  if pressure > risk_gate then branch(bold_attempt, cautious_detour)
  if hero inside resolution_window then stop
```

Reading:
The story still has source, traversal, threshold, split, and closure band.

### As a circuit or control pattern

```text
signal := source
repeat:
  signal := propagate(signal)
  if signal > threshold then branch(alarm_line, normal_line)
  if signal inside stable_band then hold
```

Reading:
This is the same comparison-gate-plus-branch structure in a circuit-flavored frame.

## Equation Commentary

Dynamic equations are especially close to this whole approach because they already expose:
- a carrier of traversal
- a current state on that carrier
- continuation laws
- comparison gates such as `>`, `<`, and interval tests
- boolean occupancy such as `and`, `or`, and `not`
- repetition through powers or iterative update
- reverse continuation through inverse or branch recovery

So an equation flow can often be read as:
- traversal on a carrier
- comparison as boundary or sidedness
- boolean combination as framed occupancy over regions
- repetition as lawful continuation
- inverse as branch-family recovery

That is one reason it feels so mathematically close to the other domains.

## Comparison and Interval-Test Examples

### `x > 0`

Approximate encoding:

```text
x > 0 := locate(x, positive_side(boundary(0)))
```

Reading:
This is a sidedness test relative to a boundary on an ordered carrier.

### `a <= x <= b`

Approximate encoding:

```text
a <= x <= b := locate(x, interior(span(a, b)))
```

Reading:
This is a bounded admissible region, not just two unrelated comparisons.

### `(x > 0) and (x < 10)`

Approximate encoding:

```text
(x > 0) and (x < 10) := positive_side(boundary(0)) AND negative_side(boundary(10))
```

Reading:
Boolean conjunction reconstructs one interior band from two half-space conditions.

### `not inside target_band`

Approximate encoding:

```text
not_inside(state, target_band) := NOT locate(state, interior(target_band))
```

Reading:
Boolean negation here is exterior occupancy relative to an active admissible region.

## Why These Are Good Bridge Cases

These comparison and interval examples are useful because they sit very near both math and language.

- `x > 0` is like `above zero`, `after the boundary`, or `beyond the gate`
- `a <= x <= b` is like `between`, `during`, or `inside the acceptable window`
- `(x > 0) and (x < 10)` is like reconstructing `between 0 and 10` from two edges
- `not inside target_band` is like `outside`, `beyond`, or `not yet within range`

So this is a very good narrow slice for testing whether the metaphor is doing real work.
