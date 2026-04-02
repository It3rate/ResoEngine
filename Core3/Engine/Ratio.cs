namespace Core3.Engine;

/// <summary>
/// A raw pinned ratio relation between two uncalibrated extents.
/// Each extent contributes only a magnitude here; sign is read from which
/// endpoint is pinned at the shared join:
/// - pinned start -> positive
/// - pinned end -> negative
/// Numerator and denominator are chosen later by the reader.
/// </summary>
public sealed record Ratio
{
    public Ratio(
        long extentP,
        long extentQ,
        bool pPinsStart,
        bool qPinsStart)
    {
        if (extentP < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(extentP),
                "Raw ratio extents must be non-negative magnitudes.");
        }

        if (extentQ < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(extentQ),
                "Raw ratio extents must be non-negative magnitudes.");
        }

        ExtentP = extentP;
        ExtentQ = extentQ;
        PPinsStart = pPinsStart;
        QPinsStart = qPinsStart;
    }

    public long ExtentP { get; }
    public long ExtentQ { get; }
    public bool PPinsStart { get; }
    public bool QPinsStart { get; }

    public int GetSign(RatioTerm term) =>
        IsPinnedStart(term) ? 1 : -1;

    public long GetSignedExtent(RatioTerm term) =>
        checked(GetExtent(term) * GetSign(term));

    public long GetExtent(RatioTerm term) =>
        term switch
        {
            RatioTerm.P => ExtentP,
            RatioTerm.Q => ExtentQ,
            _ => throw new ArgumentOutOfRangeException(nameof(term))
        };

    public RatioTerm GetOtherTerm(RatioTerm term) =>
        term == RatioTerm.P
            ? RatioTerm.Q
            : RatioTerm.P;

    public float ToFloat(RatioTerm denominatorTerm) =>
        (float)ToDouble(denominatorTerm);

    public double ToDouble(RatioTerm denominatorTerm)
    {
        var denominator = GetExtent(denominatorTerm);
        var numerator = GetExtent(GetOtherTerm(denominatorTerm));
        return (double)numerator / denominator;
    }

    private bool IsPinnedStart(RatioTerm term) =>
        term switch
        {
            RatioTerm.P => PPinsStart,
            RatioTerm.Q => QPinsStart,
            _ => throw new ArgumentOutOfRangeException(nameof(term))
        };
}

public enum RatioTerm
{
    P,
    Q
}
