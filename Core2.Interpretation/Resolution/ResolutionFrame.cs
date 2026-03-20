using Core2.Elements;
using Core2.Units;

namespace Core2.Interpretation.Resolution;

public sealed record ResolutionFrame
{
    public ResolutionFrame(
        string name,
        string symbol,
        UnitSignature signature,
        Scalar grain,
        UnitChoice? unit = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentNullException.ThrowIfNull(signature);

        if (grain.IsZero || grain.Value < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(grain), "Resolution grain must be positive.");
        }

        if (unit is not null && !unit.CanExpress(signature))
        {
            throw new ArgumentException(
                $"Resolution frame unit {unit.Symbol} does not match signature {signature}.",
                nameof(unit));
        }

        Name = name;
        Symbol = symbol;
        Signature = signature;
        Grain = grain;
        Unit = unit;
    }

    public string Name { get; }
    public string Symbol { get; }
    public UnitSignature Signature { get; }
    public Scalar Grain { get; }
    public UnitChoice? Unit { get; }

    public bool CanRead(UnitSignature signature) => Signature.Equals(signature);

    public static ResolutionFrame FromUnitChoice(UnitChoice unit) =>
        new(unit.Name, unit.Symbol, unit.Signature, unit.CanonicalScale, unit);

    public override string ToString() => Symbol;
}
