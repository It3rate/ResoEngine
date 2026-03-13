using Core2.Elements;

namespace Core2.Units;

/// <summary>
/// One basis generator raised to a rational power in the unit signature.
/// </summary>
public readonly record struct UnitPower(UnitGenerator Generator, Proportion Exponent)
{
    public UnitPower(UnitGenerator generator, int exponent)
        : this(generator, new Proportion(exponent, 1))
    {
    }
}
