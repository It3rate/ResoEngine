using Core2.Elements;

namespace Core2.Units;

/// <summary>
/// Named presentation/conversion choice for a unit signature.
/// The structural algebra stays on the quantity; this object chooses how the signature is named.
/// </summary>
public sealed record UnitChoice(
    string Name,
    string Symbol,
    UnitSignature Signature,
    Scalar CanonicalScale,
    PhysicalReferent? Referent = null)
{
    public bool CanExpress(UnitSignature signature) => Signature.Equals(signature);

    public override string ToString() => Symbol;
}
