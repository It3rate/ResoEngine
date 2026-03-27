namespace Core3.Units;

/// <summary>
/// Minimal structural description of a named unit family member.
/// It deliberately stops short of carrying nouns, calibration tables, or full
/// conversion machinery. The goal is to expose the few attributes most likely
/// to affect resolution and operation choice later.
/// </summary>
public sealed record UnitType
{
    public UnitType(
        string name,
        UnitCategory category,
        UnitContinuity continuity = UnitContinuity.Unspecified,
        UnitConversionKind conversionKind = UnitConversionKind.Proportional,
        UnitResolutionBehavior resolutionBehavior = UnitResolutionBehavior.Contextual,
        int dimensions = 1,
        int degreesOfFreedom = 1,
        string? symbol = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit name is required.", nameof(name));
        }

        if (dimensions <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dimensions), "Dimensions must be positive.");
        }

        if (degreesOfFreedom <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom), "Degrees of freedom must be positive.");
        }

        if (degreesOfFreedom > dimensions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(degreesOfFreedom),
                "Degrees of freedom cannot exceed dimensions.");
        }

        Name = name;
        Category = category;
        Continuity = continuity;
        ConversionKind = conversionKind;
        ResolutionBehavior = resolutionBehavior;
        Dimensions = dimensions;
        DegreesOfFreedom = degreesOfFreedom;
        Symbol = string.IsNullOrWhiteSpace(symbol) ? null : symbol;
    }

    public string Name { get; }
    public string? Symbol { get; }
    public UnitCategory Category { get; }
    public UnitContinuity Continuity { get; }
    public UnitConversionKind ConversionKind { get; }
    public UnitResolutionBehavior ResolutionBehavior { get; }

    /// <summary>
    /// Number of independent unit axes carried by the quantity.
    /// Example: miles-per-hour has two dimensions.
    /// </summary>
    public int Dimensions { get; }

    /// <summary>
    /// Number of free numeric choices available once the unit structure is
    /// active. Example: miles-per-hour has one degree of freedom.
    /// </summary>
    public int DegreesOfFreedom { get; }

    public bool IsDiscrete => Continuity == UnitContinuity.Discrete;
    public bool IsContinuous => Continuity == UnitContinuity.Continuous;
    public bool IsAffine => ConversionKind == UnitConversionKind.Affine;
    public bool IsCompound => Dimensions > 1;

    public override string ToString() => Symbol is null ? Name : $"{Name} ({Symbol})";
}
