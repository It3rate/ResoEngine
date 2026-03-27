# Units

This folder holds the minimal structural unit metadata for `Core3`.

The purpose is not to turn Core3 into a traditional measurement library, and it
is not to attach rich noun taxonomies too early. The purpose is to preserve a
small amount of information about a unit so the engine can later make better
choices about:

- which fold is natural
- which fold merely remains possible
- how support or resolution should be preserved
- whether a conversion is proportional, affine, or more contextual

## Why This Exists

In Core3, a denominator is not only a divisor.
It is also support or resolution.

That means the engine often needs to know more than:

- the folded value
- the current tick count

It also needs small structural hints about the unit itself.

Examples:

- adding two distances usually wants exact common-frame alignment
- multiplying two independent lengths may naturally create an area-like fold
- multiplying a distance by a time may create a compound unit rather than a
  same-space geometric fold
- changing Fahrenheit to Celsius is not the same kind of operation as changing
  miles to yards

These are not only arithmetic differences.
They are differences in what kind of fold or resolution policy is appropriate.

## Current Scope

The current unit model is intentionally small.

`UnitType` carries:

- a broad category
- whether the unit behaves more like a discrete or continuous support
- what sort of conversion law it expects
- a default hint for how resolution tends to behave
- how many dimensions the unit structure spans
- how many numeric degrees of freedom it actually has

This is enough to begin expressing distinctions such as:

- `mile` and `yard` are both `Distance`
- `Celsius` and `Fahrenheit` are both `Temperature`, but use affine conversion
- `mile per hour` has two dimensions but one degree of freedom
- a counted thing may prefer different resolution behavior than a continuous
  measure

## Folding And Resolution

This folder is meant to support later policy selection, not replace the engine's
exact arithmetic.

The rough intended relationship is:

- engine numbers preserve exact working support
- units help suggest what kind of fold is natural
- units help suggest whether support should be inherited, aggregated, composed,
  or left contextual
- explicit policy should still win when the caller knows more than the default

So unit metadata should bias later choices such as:

- should support be inherited from the host?
- should supports be pooled?
- should supports compose into a product space?
- is exact re-expression straightforward or does the conversion shift the frame?

## Not In Scope Yet

This folder does not yet provide:

- named conversion tables
- physical constants
- dimensional-analysis enforcement
- noun semantics beyond broad category
- automatic integration with `Core3.Engine`

Those can be layered in later if they turn out to be genuinely needed.

For now the goal is only to keep a small amount of unit structure available so
that later folding and resolution choices can be made more honestly.
