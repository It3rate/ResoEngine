namespace Core3.Units;

/// <summary>
/// Default hint for how resolution tends to behave when this unit participates
/// in an equation. This is a bias for later engine policy, not a hard law.
/// </summary>
public enum UnitResolutionBehavior
{
    Contextual = 0,
    Inherit,
    Aggregate,
    Compose
}
