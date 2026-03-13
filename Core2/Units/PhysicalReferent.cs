namespace Core2.Units;

/// <summary>
/// Optional physical meaning attached to a unit generator or named unit.
/// The algebra can remain abstract even when the physical referent is unknown.
/// </summary>
public sealed record PhysicalReferent(string Key, string DisplayName, string? Notes = null)
{
    public override string ToString() => DisplayName;
}
