namespace Core3.Units;

/// <summary>
/// Broad human-facing category for a unit family.
/// This is a hint for interpretation and defaults, not a full ontology.
/// This is the first steps towards language, and therefore is non-mathematical, though categories
/// do tell us mathematical things (angles wrap, temperature has a lower bound, probability is bayesian, etc).
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
