namespace Core3.Engine;

internal static class EngineEvaluation
{
    internal static bool TryFoldToAtomic(GradedElement element, out AtomicElement? folded)
    {
        ArgumentNullException.ThrowIfNull(element);

        var current = element;

        while (current is not AtomicElement atomic)
        {
            if (!current.TryFold(out var next) || next is null)
            {
                folded = null;
                return false;
            }

            current = next;
        }

        folded = (AtomicElement)current;
        return true;
    }

    internal static bool TryComposeRatio(
        AtomicElement numerator,
        AtomicElement denominator,
        out GradedElement? folded)
    {
        ArgumentNullException.ThrowIfNull(numerator);
        ArgumentNullException.ThrowIfNull(denominator);

        if (!numerator.HasResolvedUnits ||
            !denominator.HasResolvedUnits ||
            denominator.Value == 0 ||
            Math.Sign(numerator.Unit) != Math.Sign(denominator.Unit))
        {
            folded = null;
            return false;
        }

        var value = checked(numerator.Value * Math.Abs(denominator.Unit));

        if (denominator.Value < 0)
        {
            value = checked(-value);
        }

        var unit = checked(Math.Sign(numerator.Unit) * Math.Abs(numerator.Unit) * Math.Abs(denominator.Value));
        var divisor = GreatestCommonDivisor(Math.Abs(value), Math.Abs(unit));

        folded = new AtomicElement(
            value / divisor,
            unit / divisor);
        return true;
    }

    internal static bool TryMultiplyAtomic(
        AtomicElement left,
        AtomicElement right,
        out GradedElement? product)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (!left.HasResolvedUnits || !right.HasResolvedUnits)
        {
            product = null;
            return false;
        }

        var value = checked(left.Value * right.Value);
        var unitMagnitude = checked(Math.Abs(left.Unit) * Math.Abs(right.Unit));
        var unitSign = left.IsOrthogonalUnit || right.IsOrthogonalUnit ? -1L : 1L;
        var unit = checked(unitSign * unitMagnitude);
        var divisor = GreatestCommonDivisor(Math.Abs(value), Math.Abs(unit));

        product = new AtomicElement(
            value / divisor,
            unit / divisor);
        return true;
    }

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
