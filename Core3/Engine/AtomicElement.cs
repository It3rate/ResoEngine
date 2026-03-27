namespace Core3.Engine;

/// <summary>
/// Grade-0 engine value carrying an exact signed amount and an exact signed
/// unit number. The unit sign carries carrier relation (positive aligned,
/// negative orthogonal, zero unresolved), while the unit magnitude carries
/// exact resolution support. Opposite perspective negates the realized value
/// but preserves the unit.
/// </summary>
public sealed record AtomicElement(long Value, long Unit) : GradedElement
{
    public override int Grade => 0;
    public override bool HasResolvedUnits => Unit != 0;
    public bool IsAlignedUnit => Unit > 0;
    public bool IsOrthogonalUnit => Unit < 0;
    public bool IsUnresolvedUnit => Unit == 0;
    public bool IsClampLike => Unit == 0 && Value == 0;
    public long Resolution => Math.Abs(Unit);

    public override GradedElement Negate() => new AtomicElement(-Value, Unit);

    public override GradedElement SwapOrder() => this;

    public override GradedElement FlipPerspective() => Negate();

    public override bool TryFold(out GradedElement? folded)
    {
        folded = this;
        return true;
    }

    // Resolution is preserved during exact working arithmetic.
    // Re-expression, alignment, and support commit are explicit operations so
    // the engine does not silently collapse support by coincidence.
    public bool TryReexpressToSupport(long targetResolution, out AtomicElement? reexpressed)
    {
        if (!HasResolvedUnits || targetResolution <= 0)
        {
            reexpressed = null;
            return false;
        }

        var scaledValue = checked(Value * targetResolution);

        if (scaledValue % Resolution != 0)
        {
            reexpressed = null;
            return false;
        }

        reexpressed = new AtomicElement(
            scaledValue / Resolution,
            checked(Math.Sign(Unit) * targetResolution));
        return true;
    }

    public bool TryCommitToSupport(long targetResolution, out AtomicElement? committed) =>
        TryReexpressToSupport(targetResolution, out committed);

    public bool TryAlignExact(AtomicElement other, out AtomicElement? leftAligned, out AtomicElement? rightAligned)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (!SharesUnitSpace(other))
        {
            leftAligned = null;
            rightAligned = null;
            return false;
        }

        var targetResolution = LeastCommonMultiple(Resolution, other.Resolution);

        if (TryReexpressToSupport(targetResolution, out leftAligned) &&
            other.TryReexpressToSupport(targetResolution, out rightAligned) &&
            leftAligned is not null &&
            rightAligned is not null)
        {
            return true;
        }

        leftAligned = null;
        rightAligned = null;
        return false;
    }

    public override bool SharesUnitSpace(GradedElement other) =>
        other is AtomicElement atomic &&
        HasResolvedUnits &&
        atomic.HasResolvedUnits &&
        Math.Sign(Unit) == Math.Sign(atomic.Unit);

    public override bool TryAdd(GradedElement other, out GradedElement? sum)
    {
        if (other is AtomicElement atomic &&
            TryAlignExact(atomic, out var leftAligned, out var rightAligned) &&
            leftAligned is not null &&
            rightAligned is not null)
        {
            sum = new AtomicElement(
                checked(leftAligned.Value + rightAligned.Value),
                leftAligned.Unit);
            return true;
        }

        sum = null;
        return false;
    }

    public override bool TrySubtract(GradedElement other, out GradedElement? difference)
    {
        if (other is AtomicElement atomic &&
            TryAlignExact(atomic, out var leftAligned, out var rightAligned) &&
            leftAligned is not null &&
            rightAligned is not null)
        {
            difference = new AtomicElement(
                checked(leftAligned.Value - rightAligned.Value),
                leftAligned.Unit);
            return true;
        }

        difference = null;
        return false;
    }

    public override bool TryMultiply(GradedElement other, out GradedElement? product)
    {
        if (other is AtomicElement atomic)
        {
            return EngineEvaluation.TryMultiplyAtomic(this, atomic, out product);
        }

        product = null;
        return false;
    }

    public override bool TryScale(AtomicElement factor, out GradedElement? scaled)
    {
        ArgumentNullException.ThrowIfNull(factor);

        if (!HasResolvedUnits || !factor.HasResolvedUnits)
        {
            scaled = null;
            return false;
        }

        var scaledValue = checked(Value * factor.Value);
        var scaledUnit = checked(Unit * factor.Unit);
        scaled = new AtomicElement(scaledValue, scaledUnit);
        return true;
    }

    public override string ToString() => $"{Value}/{Unit}";

    private static long LeastCommonMultiple(long left, long right) =>
        checked((left / GreatestCommonDivisor(left, right)) * right);

    private static long GreatestCommonDivisor(long left, long right)
    {
        while (right != 0)
        {
            var remainder = left % right;
            left = right;
            right = remainder;
        }

        return left == 0 ? 1 : left;
    }
}
