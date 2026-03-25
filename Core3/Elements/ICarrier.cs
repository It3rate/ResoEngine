namespace Core3.Elements;

/// <summary>
/// A carrier is a measurable directional thing that can be mirrored or negated.
/// The current Core3 bootstrap uses long carriers, but higher grades can later
/// supply richer carrier implementations.
/// </summary>
public interface ICarrier
{
    CarrierSide Side { get; }
    long RawValue { get; }
    long Value { get; }
    bool IsZero { get; }
    bool IsPositive { get; }
    bool IsNegative { get; }
    bool IsCompatibleWith(ICarrier other);
    ICarrier Subtract(ICarrier other);
    ICarrier PositionAt(ICarrier end, Proportion proportion);
    ICarrier Negate();
    ICarrier Mirror();
    ICarrier AsInbound();
    ICarrier AsOutbound();
}
