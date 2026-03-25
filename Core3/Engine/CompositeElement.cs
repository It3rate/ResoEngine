namespace Core3.Engine;

/// <summary>
/// Generic higher-grade element built from two equal-grade children.
/// This is the grade-first engine analogue of the named element layer.
/// </summary>
public sealed record CompositeElement : GradedElement
{
    public CompositeElement(GradedElement recessive, GradedElement dominant)
    {
        ArgumentNullException.ThrowIfNull(recessive);
        ArgumentNullException.ThrowIfNull(dominant);

        if (recessive.Grade != dominant.Grade)
        {
            throw new InvalidOperationException("Composite elements require children of the same grade.");
        }

        Recessive = recessive;
        Dominant = dominant;
    }

    public GradedElement Recessive { get; }
    public GradedElement Dominant { get; }

    public override int Grade => Recessive.Grade + 1;
    public override EngineSignature Signature => new CompositeSignature(
        Recessive.Signature,
        Dominant.Signature);

    public override GradedElement Mirror() => new CompositeElement(
        Dominant.Mirror(),
        Recessive.Mirror());

    public override string ToString() => $"<{Recessive} | {Dominant}>";
}
