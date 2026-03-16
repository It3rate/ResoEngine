using Core2.Elements;

namespace Core2.Repetition;

public sealed class AxisTraversalState
{
    private readonly AxisTraversalDefinition _definition;
    private readonly Proportion _stepMagnitude;
    private int _direction;

    public AxisTraversalState(AxisTraversalDefinition definition, Proportion value)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _stepMagnitude = definition.Step.Abs();
        _direction = definition.Step.Sign < 0 ? -1 : 1;
        Value = value;
        Law = definition.Law;
    }

    public Proportion Value { get; private set; }

    public BoundaryContinuationLaw Law { get; private set; }

    public AxisTraversalStep Fire()
    {
        var parts = EnumerateFire().ToArray();
        if (parts.Length == 0)
        {
            return new AxisTraversalStep(Axis.FromCoordinates(Value, Value), []);
        }

        var tensions = parts.SelectMany(part => part.Tensions).ToArray();
        return new AxisTraversalStep(
            Axis.FromCoordinates(parts[0].Start, parts[^1].End),
            tensions,
            parts[^1].BreakAfter);
    }

    public IEnumerable<AxisTraversalStep> EnumerateFire()
    {
        Proportion start = Value;
        if (_stepMagnitude.IsZero)
        {
            yield break;
        }

        if (_definition.Frame is null)
        {
            Value = start + SignedStep();
            yield return CreateStep(start, Value);
            yield break;
        }

        foreach (var part in Law switch
        {
            BoundaryContinuationLaw.ReflectiveBounce => EnumerateReflectiveFire(start),
            BoundaryContinuationLaw.PeriodicWrap => EnumerateWrappedFire(start),
            BoundaryContinuationLaw.Clamp => EnumerateClampedFire(start),
            BoundaryContinuationLaw.TensionPreserving => EnumerateTensionPreservingFire(start),
            _ => throw new ArgumentOutOfRangeException(nameof(Law), Law, null),
        })
        {
            yield return part;
        }
    }

    public IEnumerable<AxisTraversalStep> Iterate()
    {
        while (true)
        {
            yield return Fire();
        }
    }

    public IEnumerable<AxisTraversalStep> Iterate(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        for (int index = 0; index < count; index++)
        {
            yield return Fire();
        }
    }

    public void SetLaw(BoundaryContinuationLaw law) => Law = law;

    private IEnumerable<AxisTraversalStep> EnumerateReflectiveFire(Proportion start)
    {
        Proportion min = _definition.Frame!.LeftCoordinate;
        Proportion max = _definition.Frame.RightCoordinate;

        if (min == max)
        {
            Value = start;
            yield break;
        }

        Proportion current = start;
        Proportion remaining = _stepMagnitude;
        int direction = _direction;

        while (remaining > Proportion.Zero)
        {
            Proportion edge = direction > 0 ? max : min;
            Proportion distanceToEdge = (edge - current).Abs();

            if (distanceToEdge.IsZero)
            {
                direction *= -1;
                continue;
            }

            Proportion travel = Proportion.Min(remaining, distanceToEdge);
            Proportion next = direction > 0
                ? current + travel
                : current - travel;
            bool breakAfter = remaining > travel && next == edge;

            yield return CreateStep(current, next, breakAfter: breakAfter);

            current = next;
            remaining -= travel;

            if (remaining > Proportion.Zero && current == edge)
            {
                direction *= -1;
            }
        }

        _direction = direction;
        Value = current;
    }

    private IEnumerable<AxisTraversalStep> EnumerateWrappedFire(Proportion start)
    {
        Proportion min = _definition.Frame!.LeftCoordinate;
        Proportion max = _definition.Frame.RightCoordinate;

        if (min == max)
        {
            Value = start;
            yield break;
        }

        Proportion current = start;
        Proportion remaining = _stepMagnitude;

        while (remaining > Proportion.Zero)
        {
            Proportion edge = _direction > 0 ? max : min;
            Proportion distanceToEdge = (edge - current).Abs();

            if (distanceToEdge.IsZero)
            {
                current = _direction > 0 ? min : max;
                continue;
            }

            Proportion travel = Proportion.Min(remaining, distanceToEdge);
            Proportion next = _direction > 0
                ? current + travel
                : current - travel;

            yield return CreateStep(current, next);

            current = next;
            remaining -= travel;

            if (remaining > Proportion.Zero && current == edge)
            {
                current = _direction > 0 ? min : max;
            }
        }

        Value = current;
    }

    private IEnumerable<AxisTraversalStep> EnumerateClampedFire(Proportion start)
    {
        Proportion unclamped = start + SignedStep();
        Proportion next = Proportion.Max(
            _definition.Frame!.LeftCoordinate,
            Proportion.Min(_definition.Frame.RightCoordinate, unclamped));

        Value = next;
        if (start != next)
        {
            yield return CreateStep(start, next);
        }
    }

    private IEnumerable<AxisTraversalStep> EnumerateTensionPreservingFire(Proportion start)
    {
        Proportion next = start + SignedStep();
        Value = next;

        var continuation = _definition.Frame!.Continue(next, BoundaryContinuationLaw.TensionPreserving);
        yield return CreateStep(start, next, continuation.Tensions);
    }

    private Proportion SignedStep() => _direction < 0 ? -_stepMagnitude : _stepMagnitude;

    private static AxisTraversalStep CreateStep(
        Proportion start,
        Proportion end,
        IReadOnlyList<RepetitionTension>? tensions = null,
        bool breakAfter = false) =>
        new(
            Axis.FromCoordinates(start, end),
            tensions ?? [],
            breakAfter);
}
