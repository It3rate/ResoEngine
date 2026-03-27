namespace Core3.Units;

/// <summary>
/// Describes how one named unit in a family converts to another.
/// Proportional covers meter-to-yard style conversions.
/// Affine covers Celsius-to-Fahrenheit style conversions.
/// Contextual is a placeholder for later richer mappings.
/// </summary>
public enum UnitConversionKind
{
    Proportional = 0,
    Affine,
    Contextual
}
