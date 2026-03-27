namespace Core3.Units;

/// <summary>
/// Broad human-facing category for a unit family.
/// This is a hint for interpretation and defaults, not a full ontology.
/// </summary>
public enum UnitCategory
{
    Generic = 0,
    Count,
    Distance,
    Area,
    Volume,
    Time,
    Mass,
    Temperature,
    Angle,
    Rate,
    Force,
    Energy,
    Pressure,
    Charge,
    Information,
    Currency,
    Probability
}
