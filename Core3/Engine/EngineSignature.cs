namespace Core3.Engine;

/// <summary>
/// Recursive signature shape used to classify same-space versus contrastive
/// pinning after local normalization.
/// </summary>
public abstract record EngineSignature
{
    public abstract int Grade { get; }
    public abstract bool IsResolved { get; }
    public abstract EngineSignature Mirror();
}

public sealed record AtomicSignature(EngineUnit Unit) : EngineSignature
{
    public override int Grade => 0;
    public override bool IsResolved => Unit.IsResolved;

    public override EngineSignature Mirror() => new AtomicSignature(Unit.Mirror());

    public override string ToString() => Unit.ToString();
}

public sealed record CompositeSignature : EngineSignature
{
    public CompositeSignature(EngineSignature recessive, EngineSignature dominant)
    {
        ArgumentNullException.ThrowIfNull(recessive);
        ArgumentNullException.ThrowIfNull(dominant);

        if (recessive.Grade != dominant.Grade)
        {
            throw new InvalidOperationException("Composite signatures require children of the same grade.");
        }

        Recessive = recessive;
        Dominant = dominant;
    }

    public EngineSignature Recessive { get; }
    public EngineSignature Dominant { get; }

    public override int Grade => Recessive.Grade + 1;
    public override bool IsResolved => Recessive.IsResolved && Dominant.IsResolved;

    public override EngineSignature Mirror() => new CompositeSignature(
        Dominant.Mirror(),
        Recessive.Mirror());

    public override string ToString() => $"[{Recessive}] <> [{Dominant}]";
}
