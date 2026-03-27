namespace Core3.Units;

/// <summary>
/// Whether the unit is naturally counted in discrete steps, continuous
/// refinements, or some mixed/contextual way.
/// </summary>
public enum UnitContinuity
{
    Unspecified = 0,
    Discrete,
    Continuous,
    Hybrid
}
