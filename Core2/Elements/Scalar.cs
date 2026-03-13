using Core2.Repetition;
using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 0: a plain scalar with implicit unit 1.000...
/// This is the non-resolution value layer that sits below Proportion.
/// </summary>
public readonly record struct Scalar(decimal Value) : IElement
{
    public static Scalar Zero => new(0m);
    public static Scalar One => new(1m);
    public static Scalar PositiveOverflow => new(decimal.MaxValue);
    public static Scalar NegativeOverflow => new(decimal.MinValue);

    internal static IArithmetic<Scalar> Arithmetic { get; } = new ScalarArithmetic();

    public bool IsZero => Value == 0m;
    public int Degree => 0;

    public static implicit operator Scalar(int value) => new(value);
    public static implicit operator Scalar(long value) => new(value);
    public static implicit operator Scalar(decimal value) => new(value);
    public static explicit operator Scalar(double value) => new((decimal)value);

    public static implicit operator decimal(Scalar value) => value.Value;
    public static explicit operator double(Scalar value) => (double)value.Value;

    public static Scalar operator +(Scalar left, Scalar right) => new(left.Value + right.Value);
    public static Scalar operator -(Scalar value) => new(-value.Value);
    public static Scalar operator -(Scalar left, Scalar right) => new(left.Value - right.Value);
    public static Scalar operator *(Scalar left, Scalar right) => new(left.Value * right.Value);

    public static Scalar operator /(Scalar left, Scalar right)
    {
        if (right.IsZero)
        {
            if (left.IsZero)
            {
                return Zero;
            }

            return left.Value > 0m ? PositiveOverflow : NegativeOverflow;
        }

        return new(left.Value / right.Value);
    }

    public InverseContinuationResult<Scalar> InverseContinue(
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null) =>
        InverseContinuationEngine.InverseContinue(this, degree, rule, reference);

    public override string ToString() => Value switch
    {
        decimal.MaxValue => "Infinity",
        decimal.MinValue => "-Infinity",
        _ => Value.ToString("0.###"),
    };

    private sealed class ScalarArithmetic : IArithmetic<Scalar>
    {
        public Scalar Zero => Scalar.Zero;
        public Scalar Add(Scalar left, Scalar right) => left + right;
        public Scalar Multiply(Scalar left, Scalar right) => left * right;
        public Scalar Negate(Scalar value) => -value;
    }
}
