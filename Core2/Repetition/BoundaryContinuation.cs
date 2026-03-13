using Core2.Elements;

namespace Core2.Repetition;

public static class BoundaryContinuation
{
    public static BoundaryContinuationResult Continue(Axis frame, Scalar value, BoundaryContinuationLaw law)
    {
        decimal min = Math.Min(frame.Start.Value, frame.End.Value);
        decimal max = Math.Max(frame.Start.Value, frame.End.Value);
        return Continue(min, max, value.Value, law);
    }

    public static BoundaryContinuationResult Continue(decimal min, decimal max, decimal value, BoundaryContinuationLaw law)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (min == max)
        {
            return new BoundaryContinuationResult(min, []);
        }

        if (value >= min && value <= max)
        {
            return new BoundaryContinuationResult(value, []);
        }

        return law switch
        {
            BoundaryContinuationLaw.TensionPreserving => new BoundaryContinuationResult(
                value,
                [
                    new RepetitionTension(
                        RepetitionTensionKind.BoundaryExceeded,
                        $"Value {value:0.###} exceeded frame [{min:0.###}, {max:0.###}] and was preserved as tension.")
                ]),
            BoundaryContinuationLaw.PeriodicWrap => new BoundaryContinuationResult(Wrap(min, max, value), []),
            BoundaryContinuationLaw.ReflectiveBounce => new BoundaryContinuationResult(Reflect(min, max, value), []),
            _ => throw new ArgumentOutOfRangeException(nameof(law), law, null),
        };
    }

    private static decimal Wrap(decimal min, decimal max, decimal value)
    {
        decimal span = max - min;
        decimal shifted = value - min;
        decimal wrapped = shifted % span;
        if (wrapped < 0m)
        {
            wrapped += span;
        }

        return min + wrapped;
    }

    private static decimal Reflect(decimal min, decimal max, decimal value)
    {
        decimal span = max - min;
        decimal doubled = span * 2m;
        decimal shifted = value - min;
        decimal reflected = shifted % doubled;
        if (reflected < 0m)
        {
            reflected += doubled;
        }

        return reflected <= span
            ? min + reflected
            : max - (reflected - span);
    }
}
