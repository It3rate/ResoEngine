using Core2.Elements;

namespace Core2.Repetition;

public sealed class AxisTraversalState
{
    private readonly AxisTraversalDefinition _definition;
    private readonly Proportion _stepMagnitude;
    private readonly IReadOnlyList<LocatedPin> _pins;
    private Axis? _activeFrame;
    private BoundaryPinPair? _boundaryPins;
    private int _direction;

    public AxisTraversalState(AxisTraversalDefinition definition, Proportion value)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _stepMagnitude = definition.Step.Abs();
        _pins = definition.Pins ?? [];
        _direction = definition.Step.Sign < 0 ? -1 : 1;
        _activeFrame = definition.Frame ?? definition.BoundaryPins?.Frame;
        _boundaryPins = definition.ResolveBoundaryPins();
        Value = value;
        Law = _boundaryPins?.SummaryLaw ?? definition.Law;
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
        if (_stepMagnitude.IsZero)
        {
            yield break;
        }

        var advance = PinBoundaryTraversal.Advance(
            Value,
            _stepMagnitude,
            _direction,
            _activeFrame,
            _boundaryPins,
            _pins);

        Value = advance.FinalValue;
        _direction = advance.FinalDirection;
        _activeFrame = advance.ActiveFrame;
        _boundaryPins = advance.ActivePins;

        foreach (var part in advance.Fragments)
        {
            yield return CreateStep(part.Start, part.End, part.Tensions, part.BreakAfter);
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

    public void SetLaw(BoundaryContinuationLaw law)
    {
        _activeFrame = _definition.Frame;
        _boundaryPins = _definition.Frame is null
            ? null
            : law == BoundaryContinuationLaw.TensionPreserving
                ? BoundaryPinPair.Open(_definition.Frame)
                : BoundaryPinPair.FromLaw(_definition.Frame, law);
        Law = _boundaryPins?.SummaryLaw ?? law;
    }

    public void SetBoundaryPins(BoundaryPinPair? boundaryPins)
    {
        _boundaryPins = boundaryPins;
        _activeFrame = boundaryPins?.Frame ?? _definition.Frame;
        Law = boundaryPins?.SummaryLaw ?? _definition.Law;
    }

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
