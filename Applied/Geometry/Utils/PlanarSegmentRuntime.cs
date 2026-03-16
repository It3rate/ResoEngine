using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed class PlanarSegmentRuntime
{
    private readonly PlanarSegmentDefinition _definition;
    private readonly Axis _routeCarrier;
    private readonly Scalar _leadLength;
    private readonly Scalar _visibleLength;
    private readonly Scalar _routeLength;
    private readonly Scalar _stepMagnitude;
    private readonly Scalar _startCoordinate;
    private readonly Scalar _endCoordinate;
    private readonly int _hiddenDirection;
    private readonly int _visibleDirection;
    private bool _needsStartJump;
    private Scalar _routePosition;
    private int _direction;
    private BoundaryContinuationLaw _law;

    public PlanarSegmentRuntime(PlanarSegmentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _startCoordinate = definition.Segment.Start;
        _endCoordinate = definition.Segment.End;
        _leadLength = _startCoordinate.Abs();
        _visibleLength = (_endCoordinate - _startCoordinate).Abs();
        _routeLength = _leadLength + _visibleLength;
        _routeCarrier = Axis.FromCoordinates(Scalar.Zero, _routeLength);
        _stepMagnitude = definition.ComputeStep().Abs();
        _hiddenDirection = _startCoordinate.Sign;
        _visibleDirection = (_endCoordinate - _startCoordinate).Sign;
        _direction = definition.ComputeStep().Sign < 0 ? -1 : 1;
        _routePosition = _direction < 0 ? _routeLength : Scalar.Zero;
        _needsStartJump = _direction < 0 && !_routePosition.IsZero;
        _law = definition.Law;
    }

    public PlanarTraversalEmission Fire()
    {
        if (_stepMagnitude.IsZero && !_needsStartJump)
        {
            return PlanarTraversalEmission.Empty;
        }

        var parts = new List<PlanarTraversalMotion>();
        if (_needsStartJump)
        {
            AddInvisibleJump(Scalar.Zero, _routePosition, parts);
            _needsStartJump = false;
        }

        Scalar remaining = _stepMagnitude;

        while (remaining > Scalar.Zero)
        {
            if (_law == BoundaryContinuationLaw.ReflectiveBounce)
            {
                if (_routeLength.IsZero)
                {
                    break;
                }

                Scalar edge = _direction > 0 ? _routeCarrier.Right : _routeCarrier.Left;
                Scalar distanceToEdge = (edge - _routePosition).Abs();
                if (distanceToEdge.IsZero)
                {
                    _direction *= -1;
                    continue;
                }

                Scalar travel = Scalar.Min(remaining, distanceToEdge);
                Scalar next = _direction > 0
                    ? _routePosition + travel
                    : _routePosition - travel;
                bool endsStroke = remaining > travel && next == edge;
                AddRouteMotion(_routePosition, next, parts, endsStroke);
                _routePosition = next;
                remaining -= travel;

                if (remaining > Scalar.Zero && _routePosition == edge)
                {
                    _direction *= -1;
                }

                continue;
            }

            if (_law == BoundaryContinuationLaw.PeriodicWrap)
            {
                if (_routeLength.IsZero)
                {
                    break;
                }

                Scalar edge = _direction > 0 ? _routeCarrier.Right : _routeCarrier.Left;
                Scalar distanceToEdge = (edge - _routePosition).Abs();
                Scalar travel = Scalar.Min(remaining, distanceToEdge);
                Scalar next = _direction > 0
                    ? _routePosition + travel
                    : _routePosition - travel;
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining -= travel;

                if (remaining > Scalar.Zero && _routePosition == edge)
                {
                    Scalar wrapped = _direction > 0 ? _routeCarrier.Left : _routeCarrier.Right;
                    AddInvisibleJump(_routePosition, wrapped, parts);
                    _routePosition = wrapped;
                }

                continue;
            }

            if (_law == BoundaryContinuationLaw.Clamp)
            {
                Scalar unclamped = _direction > 0
                    ? _routePosition + remaining
                    : _routePosition - remaining;
                Scalar next = unclamped.Clamp(_routeCarrier.Left, _routeCarrier.Right);
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining = Scalar.Zero;
                continue;
            }

            Scalar continued = _direction > 0
                ? _routePosition + remaining
                : _routePosition - remaining;
            AddRouteMotion(_routePosition, continued, parts);
            _routePosition = continued;
            remaining = Scalar.Zero;
        }

        return new PlanarTraversalEmission(parts);
    }

    public void SetLaw(BoundaryContinuationLaw law) => _law = law;

    private void AddRouteMotion(Scalar from, Scalar to, List<PlanarTraversalMotion> parts, bool endsStroke = false)
    {
        if (from == to || _routeLength.IsZero)
        {
            return;
        }

        var slices = SplitAtLeadBoundary(from, to).ToArray();
        for (int index = 0; index < slices.Length; index++)
        {
            var segment = slices[index];
            if (segment.From == segment.To)
                {
                    continue;
                }

                Scalar worldFrom = MapRouteToWorld(segment.From);
                Scalar worldTo = MapRouteToWorld(segment.To);
                if (worldFrom == worldTo)
                {
                    continue;
                }

                parts.Add(new PlanarTraversalMotion(
                    _definition.Project(worldTo - worldFrom),
                    segment.IsVisible,
                    endsStroke && segment.IsVisible && index == slices.Length - 1));
            }
    }

    private void AddInvisibleJump(Scalar from, Scalar to, List<PlanarTraversalMotion> parts)
    {
        Scalar worldFrom = MapRouteToWorld(from);
        Scalar worldTo = MapRouteToWorld(to);
        if (worldFrom == worldTo)
        {
            return;
        }

        parts.Add(PlanarTraversalMotion.Hidden(
            _definition.Project(worldTo - worldFrom)));
    }

    private IEnumerable<RouteSlice> SplitAtLeadBoundary(Scalar from, Scalar to)
    {
        bool ascending = to > from;
        Scalar low = ascending ? from : to;
        Scalar high = ascending ? to : from;

        if (_leadLength <= low || _leadLength >= high)
        {
            yield return new RouteSlice(from, to, IsVisibleFor(from, to));
            yield break;
        }

        yield return new RouteSlice(from, _leadLength, IsVisibleFor(from, _leadLength));
        yield return new RouteSlice(_leadLength, to, IsVisibleFor(_leadLength, to));
    }

    private bool IsVisibleFor(Scalar from, Scalar to)
    {
        Scalar midpoint = Axis.FromCoordinates(from, to).Midpoint;
        return midpoint >= _leadLength && !_visibleLength.IsZero;
    }

    private Scalar MapRouteToWorld(Scalar route)
    {
        if (route <= _leadLength)
        {
            return ApplyDirection(route, _hiddenDirection);
        }

        if (_visibleLength.IsZero)
        {
            return _startCoordinate;
        }

        Scalar beyondVisibleStart = route - _leadLength;
        return _startCoordinate + ApplyDirection(beyondVisibleStart, _visibleDirection);
    }

    private static Scalar ApplyDirection(Scalar magnitude, int direction) =>
        direction < 0 ? -magnitude : magnitude;

    private readonly record struct RouteSlice(Scalar From, Scalar To, bool IsVisible);
}
