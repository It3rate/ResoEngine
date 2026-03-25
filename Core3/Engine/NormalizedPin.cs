namespace Core3.Engine;

/// <summary>
/// Local pin-space read after inbound normalization of the recessive child.
/// </summary>
public sealed record NormalizedPin
{
    public NormalizedPin(GradedElement inbound, GradedElement outbound, GradedElement? position = null)
    {
        ArgumentNullException.ThrowIfNull(inbound);
        ArgumentNullException.ThrowIfNull(outbound);

        if (inbound.Grade != outbound.Grade)
        {
            throw new InvalidOperationException("Normalized pins require equal-grade inbound and outbound children.");
        }

        Inbound = inbound;
        Outbound = outbound;
        Position = position;
    }

    public GradedElement Inbound { get; }
    public GradedElement Outbound { get; }
    public GradedElement? Position { get; }
    public int ChildGrade => Inbound.Grade;
    public bool SharesUnitSpace => Inbound.Signature == Outbound.Signature;
    public bool HasResolvedUnits => Inbound.HasResolvedSignature && Outbound.HasResolvedSignature;

    public override string ToString() => $"in {Inbound} | out {Outbound}";
}
