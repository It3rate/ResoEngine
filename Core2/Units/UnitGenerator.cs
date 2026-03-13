namespace Core2.Units;

/// <summary>
/// Abstract basis generator for dimensional algebra.
/// Equality is by stable Id so the generator can later be mapped to physical meaning.
/// </summary>
public sealed class UnitGenerator : IEquatable<UnitGenerator>, IComparable<UnitGenerator>
{
    public UnitGenerator(string id, string symbol, PhysicalReferent? referent = null)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Generator id cannot be blank.", nameof(id)) : id;
        Symbol = string.IsNullOrWhiteSpace(symbol) ? throw new ArgumentException("Generator symbol cannot be blank.", nameof(symbol)) : symbol;
        Referent = referent;
    }

    public string Id { get; }
    public string Symbol { get; }
    public PhysicalReferent? Referent { get; }

    public bool Equals(UnitGenerator? other) => other is not null && string.Equals(Id, other.Id, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is UnitGenerator other && Equals(other);

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Id);

    public int CompareTo(UnitGenerator? other) => other is null ? 1 : StringComparer.Ordinal.Compare(Id, other.Id);

    public override string ToString() => Symbol;
}
