using System.Collections.Immutable;

namespace Core2.Units;

/// <summary>
/// Dimensional signature modeled as a free abelian group over unit generators.
/// Multiplication adds exponents; division subtracts them.
/// </summary>
public sealed class UnitSignature : IEquatable<UnitSignature>
{
    private readonly ImmutableArray<UnitPower> _powers;

    public static UnitSignature Dimensionless { get; } = new([]);

    public UnitSignature(IEnumerable<UnitPower> powers)
    {
        _powers = Normalize(powers);
    }

    public IReadOnlyList<UnitPower> Powers => _powers;
    public bool IsDimensionless => _powers.IsDefaultOrEmpty;
    public IReadOnlyList<PhysicalReferent> Referents =>
        _powers
            .Select(power => power.Generator.Referent)
            .Where(referent => referent is not null)
            .Cast<PhysicalReferent>()
            .Distinct()
            .ToArray();

    public static UnitSignature From(UnitGenerator generator, int exponent = 1) =>
        exponent == 0 ? Dimensionless : new([new UnitPower(generator, exponent)]);

    public bool Matches(UnitSignature other) => Equals(other);

    public UnitSignature Multiply(UnitSignature other) =>
        new(_powers.Concat(other._powers));

    public UnitSignature Divide(UnitSignature other) =>
        new(_powers.Concat(other._powers.Select(power => new UnitPower(power.Generator, -power.Exponent))));

    public UnitSignature Reciprocal() =>
        new(_powers.Select(power => new UnitPower(power.Generator, -power.Exponent)));

    public UnitSignature Pow(int exponent)
    {
        if (exponent == 0)
        {
            return Dimensionless;
        }

        return new(_powers.Select(power => new UnitPower(power.Generator, power.Exponent * exponent)));
    }

    public bool Equals(UnitSignature? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || _powers.Length != other._powers.Length)
        {
            return false;
        }

        for (int i = 0; i < _powers.Length; i++)
        {
            var left = _powers[i];
            var right = other._powers[i];
            if (!left.Generator.Equals(right.Generator) || left.Exponent != right.Exponent)
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is UnitSignature other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var power in _powers)
        {
            hash.Add(power.Generator);
            hash.Add(power.Exponent);
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        if (IsDimensionless)
        {
            return "1";
        }

        return string.Join(" ", _powers.Select(FormatPower));
    }

    private static ImmutableArray<UnitPower> Normalize(IEnumerable<UnitPower> powers)
    {
        var combined = new Dictionary<string, UnitPower>(StringComparer.Ordinal);

        foreach (var power in powers)
        {
            if (power.Exponent == 0)
            {
                continue;
            }

            if (combined.TryGetValue(power.Generator.Id, out var existing))
            {
                combined[power.Generator.Id] = new UnitPower(existing.Generator, existing.Exponent + power.Exponent);
            }
            else
            {
                combined[power.Generator.Id] = power;
            }
        }

        return combined.Values
            .Where(power => power.Exponent != 0)
            .OrderBy(power => power.Generator)
            .ToImmutableArray();
    }

    private static string FormatPower(UnitPower power) =>
        power.Exponent == 1
            ? power.Generator.Symbol
            : $"{power.Generator.Symbol}^{power.Exponent}";
}
