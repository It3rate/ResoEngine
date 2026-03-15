using Core2.Elements;

namespace Core2.Repetition;

public sealed class AxisTraversalState
{
    private readonly AxisTraversalDefinition _definition;
    private readonly Scalar _stepMagnitude;
    private int _direction;

    public AxisTraversalState(AxisTraversalDefinition definition, Scalar value)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _stepMagnitude = new Scalar(decimal.Abs(definition.Step.Value));
        _direction = definition.Step.Value < 0m ? -1 : 1;
        Value = value;
        Law = definition.Law;
    }

    public Scalar Value { get; private set; }

    public BoundaryContinuationLaw Law { get; private set; }

    public AxisTraversalStep Fire()
    {
        Scalar start = Value;
        if (_stepMagnitude.IsZero)
        {
            return new AxisTraversalStep(start, start, Scalar.Zero, []);
        }

        if (_definition.Frame is null)
        {
            Value = start + SignedStep();
            return new AxisTraversalStep(start, Value, Value - start, []);
        }

        return Law switch
        {
            BoundaryContinuationLaw.ReflectiveBounce => Reflect(start),
            BoundaryContinuationLaw.PeriodicWrap or BoundaryContinuationLaw.Clamp or BoundaryContinuationLaw.TensionPreserving => ContinueWithLaw(start),
            _ => throw new ArgumentOutOfRangeException(nameof(Law), Law, null),
        };
    }

    public void SetLaw(BoundaryContinuationLaw law) => Law = law;

    private AxisTraversalStep ContinueWithLaw(Scalar start)
    {
        Scalar candidate = start + SignedStep();
        var continuation = _definition.Frame!.Continue(candidate, Law);
        Value = continuation.Value;
        return new AxisTraversalStep(start, Value, Value - start, continuation.Tensions);
    }

    private AxisTraversalStep Reflect(Scalar start)
    {
        decimal min = Math.Min(_definition.Frame!.Start.Value, _definition.Frame.End.Value);
        decimal max = Math.Max(_definition.Frame.Start.Value, _definition.Frame.End.Value);

        if (min == max)
        {
            Value = start;
            return new AxisTraversalStep(start, Value, Scalar.Zero, []);
        }

        decimal current = start.Value;
        decimal remaining = _stepMagnitude.Value;
        int direction = _direction;

        while (remaining > 0m)
        {
            decimal edge = direction > 0 ? max : min;
            decimal distanceToEdge = decimal.Abs(edge - current);

            if (distanceToEdge == 0m)
            {
                direction *= -1;
                continue;
            }

            if (remaining <= distanceToEdge)
            {
                current += remaining * direction;
                remaining = 0m;
            }
            else
            {
                current = edge;
                remaining -= distanceToEdge;
                direction *= -1;
            }
        }

        _direction = direction;
        Value = new Scalar(current);
        return new AxisTraversalStep(start, Value, Value - start, []);
    }

    private Scalar SignedStep() => new(_stepMagnitude.Value * _direction);
}
