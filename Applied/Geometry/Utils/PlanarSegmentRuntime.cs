using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed class PlanarSegmentRuntime
{
    private readonly PlanarSegmentDefinition _definition;
    private readonly Axis _routeCarrier;
    private readonly Proportion _leadLength;
    private readonly Proportion _visibleLength;
    private readonly Proportion _routeLength;
    private readonly Proportion _stepMagnitude;
    private readonly Proportion _startCoordinate;
    private readonly Proportion _endCoordinate;
    private readonly Proportion _visibleSpan;
    private readonly int _hiddenDirection;
    private readonly int _visibleDirection;
    private bool _needsStartJump;
    private Proportion _routePosition;
    private int _direction;
    private BoundaryContinuationLaw _law;

    public PlanarSegmentRuntime(PlanarSegmentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _startCoordinate = definition.Segment.StartCoordinate;
        _endCoordinate = definition.Segment.EndCoordinate;
        _visibleSpan = definition.Segment.CoordinateSpan;
        _leadLength = _startCoordinate.Abs();
        _visibleLength = _visibleSpan.Abs();
        _routeLength = _leadLength + _visibleLength;
        _routeCarrier = Axis.FromCoordinates(Proportion.Zero, _routeLength);
        _stepMagnitude = definition.ComputeStep().Abs();
        _hiddenDirection = _startCoordinate.Sign;
        _visibleDirection = _visibleSpan.Sign;
        _direction = definition.ComputeStep().Sign < 0 ? -1 : 1;
        _routePosition = _direction < 0 ? _routeLength : Proportion.Zero;
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
            AddInvisibleJump(Proportion.Zero, _routePosition, parts);
            _needsStartJump = false;
        }

        Proportion remaining = _stepMagnitude;

        while (remaining > Proportion.Zero)
        {
            if (_law == BoundaryContinuationLaw.ReflectiveBounce)
            {
                if (_routeLength.IsZero)
                {
                    break;
                }

                Proportion edge = _direction > 0 ? _routeCarrier.EndCoordinate : _routeCarrier.StartCoordinate;
                Proportion distanceToEdge = (edge - _routePosition).Abs();
                if (distanceToEdge.IsZero)
                {
                    _direction *= -1;
                    continue;
                }

                Proportion travel = Proportion.Min(remaining, distanceToEdge);
                Proportion next = _direction > 0
                    ? _routePosition + travel
                    : _routePosition - travel;
                bool endsStroke = remaining > travel && next == edge;
                AddRouteMotion(_routePosition, next, parts, endsStroke);
                _routePosition = next;
                remaining -= travel;

                if (remaining > Proportion.Zero && _routePosition == edge)
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

                Proportion edge = _direction > 0 ? _routeCarrier.EndCoordinate : _routeCarrier.StartCoordinate;
                Proportion distanceToEdge = (edge - _routePosition).Abs();
                Proportion travel = Proportion.Min(remaining, distanceToEdge);
                Proportion next = _direction > 0
                    ? _routePosition + travel
                    : _routePosition - travel;
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining -= travel;

                if (remaining > Proportion.Zero && _routePosition == edge)
                {
                    Proportion wrapped = _direction > 0 ? _routeCarrier.StartCoordinate : _routeCarrier.EndCoordinate;
                    AddInvisibleJump(_routePosition, wrapped, parts);
                    _routePosition = wrapped;
                }

                continue;
            }

            if (_law == BoundaryContinuationLaw.Clamp)
            {
                Proportion unclamped = _direction > 0
                    ? _routePosition + remaining
                    : _routePosition - remaining;
                Proportion next = ClampToRoute(unclamped);
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining = Proportion.Zero;
                continue;
            }

            Proportion continued = _direction > 0
                ? _routePosition + remaining
                : _routePosition - remaining;
            AddRouteMotion(_routePosition, continued, parts);
            _routePosition = continued;
            remaining = Proportion.Zero;
        }

        return new PlanarTraversalEmission(parts);
    }

    public void SetLaw(BoundaryContinuationLaw law) => _law = law;

    private void AddRouteMotion(Proportion from, Proportion to, List<PlanarTraversalMotion> parts, bool endsStroke = false)
    {
        if (from == to || _routeLength.IsZero)
        {
            return;
        }

        var slices = SplitAtLeadBoundary(from, to).ToArray();
        for (int index = 0; index < slices.Length; index++)
        {
            var slice = slices[index];
            if (slice.Route.IsDegenerate)
            {
                continue;
            }

            Proportion worldFrom = MapRouteToWorld(slice.Route.StartCoordinate);
            Proportion worldTo = MapRouteToWorld(slice.Route.EndCoordinate);
            if (worldFrom == worldTo)
            {
                continue;
            }

            parts.Add(new PlanarTraversalMotion(
                _definition.Project(worldTo - worldFrom),
                slice.IsVisible,
                endsStroke && slice.IsVisible && index == slices.Length - 1));
        }
    }

    private void AddInvisibleJump(Proportion from, Proportion to, List<PlanarTraversalMotion> parts)
    {
        Proportion worldFrom = MapRouteToWorld(from);
        Proportion worldTo = MapRouteToWorld(to);
        if (worldFrom == worldTo)
        {
            return;
        }

        parts.Add(PlanarTraversalMotion.Hidden(
            _definition.Project(worldTo - worldFrom)));
    }

    private IEnumerable<RouteSlice> SplitAtLeadBoundary(Proportion from, Proportion to)
    {
        bool ascending = to > from;
        Proportion low = ascending ? from : to;
        Proportion high = ascending ? to : from;

        if (_leadLength <= low || _leadLength >= high)
        {
            var route = Axis.FromCoordinates(from, to);
            yield return new RouteSlice(route, IsVisibleFor(route));
            yield break;
        }

        var leadSlice = Axis.FromCoordinates(from, _leadLength);
        yield return new RouteSlice(leadSlice, IsVisibleFor(leadSlice));

        var visibleSlice = Axis.FromCoordinates(_leadLength, to);
        yield return new RouteSlice(visibleSlice, IsVisibleFor(visibleSlice));
    }

    private bool IsVisibleFor(Axis routeSegment)
    {
        if (_visibleLength.IsZero)
        {
            return false;
        }

        return routeSegment.StartCoordinate >= _leadLength &&
               routeSegment.EndCoordinate >= _leadLength;
    }

    private Proportion MapRouteToWorld(Proportion route)
    {
        if (route <= _leadLength)
        {
            return ApplyDirection(route, _hiddenDirection);
        }

        if (_visibleLength.IsZero)
        {
            return _startCoordinate;
        }

        Proportion beyondVisibleStart = route - _leadLength;
        return _startCoordinate + ApplyDirection(beyondVisibleStart, _visibleDirection);
    }

    private Proportion ClampToRoute(Proportion value)
    {
        if (value < _routeCarrier.StartCoordinate)
        {
            return _routeCarrier.StartCoordinate;
        }

        if (value > _routeCarrier.EndCoordinate)
        {
            return _routeCarrier.EndCoordinate;
        }

        return value;
    }

    private static Proportion ApplyDirection(Proportion magnitude, int direction) =>
        direction < 0 ? -magnitude : magnitude;

    private readonly record struct RouteSlice(Axis Route, bool IsVisible);
}
