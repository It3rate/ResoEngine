namespace Core3.Engine;

/// <summary>
/// Raw structural product of two folded reads.
/// This preserves value multiplication and carrier-family relation without
/// forcing a scalar collapse.
/// </summary>
public sealed class FoldedProduct
{
    public FoldedProduct(FoldedRatio left, FoldedRatio right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        Right = right;
    }

    public FoldedRatio Left { get; }
    public FoldedRatio Right { get; }
    public long SignedValueProduct => checked(Left.Numerator * Right.Numerator);
    public long CarrierMagnitudeProduct => checked(Left.CarrierMagnitude * Right.CarrierMagnitude);
    public bool IsSameSpaceSquareCandidate => Left.HasAlignedCarrier && Right.HasAlignedCarrier;
    public bool IsContrastCandidate => Left.CarrierPolarity != Right.CarrierPolarity;
    public bool IsOrthogonalFamilySquareCandidate => Left.HasOrthogonalCarrier && Right.HasOrthogonalCarrier;

    public override string ToString() =>
        $"product(value {SignedValueProduct}, carrier {Left.Denominator} x {Right.Denominator})";
}
