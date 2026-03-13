namespace Core2.Units;

/// <summary>
/// One basis generator raised to an integer power in the free abelian unit signature.
/// </summary>
public readonly record struct UnitPower(UnitGenerator Generator, int Exponent);
