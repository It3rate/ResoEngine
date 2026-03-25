namespace Core3.Engine;

/// <summary>
/// A minimal unit signature for the engine working model.
/// Symbol names the family; orientation captures aligned, orthogonal, or
/// unresolved polarity as -1, +1, or 0.
/// </summary>
public readonly record struct EngineUnit
{
    public EngineUnit(string symbol, int orientation = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        Symbol = symbol;
        Orientation = Math.Sign(orientation);
    }

    public string Symbol { get; }
    public int Orientation { get; }
    public bool IsResolved => Orientation != 0;

    public EngineUnit Mirror() => new(Symbol, -Orientation);

    public override string ToString() => Orientation switch
    {
        > 0 => Symbol,
        < 0 => $"-{Symbol}",
        _ => $"?{Symbol}",
    };
}
