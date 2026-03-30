# Traversal-Relative Binding

This note records the current move toward a more physical binding model.

The core idea is:

- a machine should have a real moving carrier or container
- that mover supplies the current encounter site
- binding should read relative to that physical state instead of carrying a
  separate virtual "current" token inside every selector

## Mover First

The active loop position, history position, path position, or program-counter
like position should preferably be represented by a real moving object.

Examples:

- a trolley moving along a flow graph
- a token advancing through a family
- a cursor moving along a carrier
- a history reader moving across prior outputs

In that picture:

- the mover is what is "current"
- the structure it is attached to provides the local encounter
- selectors do not need a separate abstract current-address case

## Numeric Position

The address part of binding should prefer one numeric parameter rather than a
large menu of special-purpose cases.

Examples:

- `0/1` means the mover's current site
- `1/1` means one step or one slot ahead in a discrete reading
- `-1/1` means one step back when the domain supports that
- `1/2` means halfway along a path-like reading
- `110/200` means a finer 55% style query

This keeps discrete and continuous cases closer together:

- a discrete domain may snap or choose nearest
- a path-like domain may interpolate
- a boundary law may wrap, clamp, reflect, or leave tension

The parameter is the same kind of thing even when the interpretation differs.

## Named Alias

Names still remain useful.

A binding may still refer to:

- token slot `a`
- token slot `accumulator`
- context field `currentItem`

But those names are better treated as authoring or schema aliases than as the
deepest canonical selector structure.

The long-term preference is:

- coarse domain bucket stays explicit
- fine selector parameter is numeric/Core3-shaped
- names remain a useful alias layer

## Present Direction

So the current experimental direction is:

- domain remains explicit
- address becomes either numeric position or named alias
- current state is supplied by the mover, not by a dedicated selector token
- relative behavior comes from signed position plus the mover/domain/frame
  rather than from a separate offset type

This is not yet the final execution model.

It is a better intermediate shape because it keeps selectors closer to:

- physical traversal
- numeric perturbation
- pressure/tension style exploration
- later visualization of machine flow
