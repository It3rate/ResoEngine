namespace Core2.Propagation;

public sealed record PropagationFrame(
    string Key,
    decimal Minimum,
    decimal Maximum,
    bool AllowsReverse = true,
    string? WrapTargetKey = null)
{
    public decimal Span => Maximum - Minimum;

    public bool Contains(decimal position) => position >= Minimum && position <= Maximum;

    public decimal BoundaryPosition(PropagationBoundaryKind boundaryKind) =>
        boundaryKind switch
        {
            PropagationBoundaryKind.Minimum => Minimum,
            PropagationBoundaryKind.Maximum => Maximum,
            _ => decimal.Clamp(Minimum, Minimum, Maximum),
        };

    public decimal WrapPosition(PropagationBoundaryKind boundaryKind) =>
        boundaryKind switch
        {
            PropagationBoundaryKind.Minimum => Maximum,
            PropagationBoundaryKind.Maximum => Minimum,
            _ => BoundaryPosition(boundaryKind),
        };
}
