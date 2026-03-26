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
    public override bool HasResolvedUnits => Recessive.HasResolvedUnits && Dominant.HasResolvedUnits;

    public override GradedElement Negate() => new CompositeElement(
        Recessive.Negate(),
        Dominant.Negate());

    public override GradedElement Mirror() => new CompositeElement(
        Recessive.Mirror(),
        Dominant.Mirror());

    public override GradedElement SwapOrder() => new CompositeElement(
        Dominant,
        Recessive);

    public override GradedElement FlipPerspective() => new CompositeElement(
        Dominant.FlipPerspective(),
        Recessive.FlipPerspective());

    public override bool SharesUnitSpace(GradedElement other) =>
        other is CompositeElement composite &&
        HasResolvedUnits &&
        composite.HasResolvedUnits &&
        Recessive.SharesUnitSpace(composite.Recessive) &&
        Dominant.SharesUnitSpace(composite.Dominant);

    public override bool TryAdd(GradedElement other, out GradedElement? sum)
    {
        if (other is CompositeElement composite &&
            Recessive.TryAdd(composite.Recessive, out var recessiveSum) &&
            Dominant.TryAdd(composite.Dominant, out var dominantSum) &&
            recessiveSum is not null &&
            dominantSum is not null)
        {
            sum = new CompositeElement(recessiveSum, dominantSum);
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        if (other is CompositeElement composite &&
            Recessive.TrySubtract(composite.Recessive, out var recessiveDifference) &&
            Dominant.TrySubtract(composite.Dominant, out var dominantDifference) &&
            recessiveDifference is not null &&
            dominantDifference is not null)
        {
            difference = new CompositeElement(recessiveDifference, dominantDifference);
            return true;
        }

        difference = null;
        return false;
    }

    public override string ToString() => $"<{Recessive} | {Dominant}>";
}
