using System.Collections.Immutable;
using Core2.Elements;
using Core2.Support;

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

    public static UnitSignature From(UnitGenerator generator, Proportion exponent)
    {
        if (!RationalExponent.TryFrom(exponent, out var rational))
        {
            throw new ArgumentException("Unit signatures require rational exponents with integer numerator and nonzero integer denominator.", nameof(exponent));
        }

        return rational.IsZero ? Dimensionless : new([new UnitPower(generator, rational.ToProportion())]);
    }

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

        return new(_powers.Select(power => new UnitPower(power.Generator, power.Exponent * new Proportion(exponent, 1))));
    }

    public UnitSignature Pow(Proportion exponent)
    {
        if (!RationalExponent.TryFrom(exponent, out var rational))
        {
            throw new ArgumentException("Unit signatures require rational exponents with integer numerator and nonzero integer denominator.", nameof(exponent));
        }

        if (rational.IsZero)
        {
            return Dimensionless;
        }

        return new(_powers.Select(power => new UnitPower(power.Generator, power.Exponent * rational.ToProportion())));
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
        var combined = new Dictionary<string, (UnitGenerator Generator, RationalExponent Exponent)>(StringComparer.Ordinal);

        foreach (var power in powers)
        {
            if (!RationalExponent.TryFrom(power.Exponent, out var exponent))
            {
                throw new ArgumentException("Unit signatures require rational exponents with integer numerator and nonzero integer denominator.", nameof(powers));
            }

            if (exponent.IsZero)
            {
                continue;
            }

            if (combined.TryGetValue(power.Generator.Id, out var existing))
            {
                combined[power.Generator.Id] = (existing.Generator, existing.Exponent.Add(exponent));
            }
            else
            {
                combined[power.Generator.Id] = (power.Generator, exponent);
            }
        }

        return combined.Values
            .Where(power => !power.Exponent.IsZero)
            .OrderBy(power => power.Generator)
            .Select(power => new UnitPower(power.Generator, power.Exponent.ToProportion()))
            .ToImmutableArray();
    }

    private static string FormatPower(UnitPower power) =>
        RationalExponent.TryFrom(power.Exponent, out var exponent) && exponent.IsInteger && exponent.Numerator == 1
            ? power.Generator.Symbol
            : FormatNonIdentityPower(power.Generator.Symbol, power.Exponent);

    private static string FormatNonIdentityPower(string symbol, Proportion exponent)
    {
        RationalExponent.TryFrom(exponent, out var rational);
        if (rational.IsInteger)
        {
            return $"{symbol}^{rational.Numerator}";
        }

        return $"{symbol}^({rational.Numerator}/{rational.Denominator})";
    }
}
