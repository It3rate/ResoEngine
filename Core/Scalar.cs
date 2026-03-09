using ResoEngine.Support;

namespace ResoEngine;

/// <summary>
/// Wraps a long as a first-class algebraic element.
/// Dims=1, trivial algebra (multiply with self).
/// Implicit conversions make longs transparent in the algebra system.
/// </summary>
public readonly struct Scalar : IAlgebraic<long>, IValue
{
    public long Value { get; }

    public Scalar(long value) => Value = value;

    public int Dims => 1;
    public AlgebraEntry[] Algebra => [new AlgebraEntry(0, 0, 0, 1)];
    public long GetElement(int index) => Value;

    public long GetTicksByPerspective(Chirality perspective) => Value;
    public double[] GetValues() => [Value];

    public long Fold() => Value;

    public static implicit operator long(Scalar s) => s.Value;
    public static implicit operator Scalar(long v) => new(v);

    public override string ToString() => Value.ToString();
}
