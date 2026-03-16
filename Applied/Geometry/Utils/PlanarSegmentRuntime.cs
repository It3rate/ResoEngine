using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed class PlanarSegmentRuntime
{
    private readonly PlanarSegmentDefinition _definition;
    private readonly decimal _leadLength;
    private readonly decimal _visibleLength;
    private readonly decimal _routeLength;
    private readonly decimal _stepMagnitude;
    private readonly decimal _startCoordinate;
    private readonly decimal _endCoordinate;
    private readonly int _hiddenDirection;
    private readonly int _visibleDirection;
    private bool _needsStartJump;
    private decimal _routePosition;
    private int _direction;
    private BoundaryContinuationLaw _law;

    public PlanarSegmentRuntime(PlanarSegmentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _startCoordinate = definition.Segment.Start.Value;
        _endCoordinate = definition.Segment.End.Value;
        _leadLength = decimal.Abs(_startCoordinate);
        _visibleLength = decimal.Abs(_endCoordinate - _startCoordinate);
        _routeLength = _leadLength + _visibleLength;
        _stepMagnitude = decimal.Abs(definition.ComputeStep().Value);
        _hiddenDirection = Math.Sign(_startCoordinate);
        _visibleDirection = Math.Sign(_endCoordinate - _startCoordinate);
        _direction = Math.Sign(definition.ComputeStep().Value) < 0 ? -1 : 1;
        _routePosition = _direction < 0 ? _routeLength : 0m;
        _needsStartJump = _direction < 0 && _routePosition != 0m;
        _law = definition.Law;
    }

    public PlanarTraversalEmission Fire()
    {
        if (_stepMagnitude == 0m && !_needsStartJump)
        {
            return PlanarTraversalEmission.Empty;
        }

        var parts = new List<PlanarTraversalMotion>();
        if (_needsStartJump)
        {
            AddInvisibleJump(0m, _routePosition, parts);
            _needsStartJump = false;
        }

        decimal remaining = _stepMagnitude;

        while (remaining > 0m)
        {
            if (_law == BoundaryContinuationLaw.ReflectiveBounce)
            {
                if (_routeLength == 0m)
                {
                    break;
                }

                decimal edge = _direction > 0 ? _routeLength : 0m;
                decimal distanceToEdge = decimal.Abs(edge - _routePosition);
                if (distanceToEdge == 0m)
                {
                    _direction *= -1;
                    continue;
                }

                decimal travel = Math.Min(remaining, distanceToEdge);
                decimal next = _routePosition + _direction * travel;
                bool endsStroke = remaining > travel && next == edge;
                AddRouteMotion(_routePosition, next, parts, endsStroke);
                _routePosition = next;
                remaining -= travel;

                if (remaining > 0m && _routePosition == edge)
                {
                    _direction *= -1;
                }

                continue;
            }

            if (_law == BoundaryContinuationLaw.PeriodicWrap)
            {
                if (_routeLength == 0m)
                {
                    break;
                }

                decimal edge = _direction > 0 ? _routeLength : 0m;
                decimal distanceToEdge = decimal.Abs(edge - _routePosition);
                decimal travel = Math.Min(remaining, distanceToEdge);
                decimal next = _routePosition + _direction * travel;
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining -= travel;

                if (remaining > 0m && _routePosition == edge)
                {
                    decimal wrapped = _direction > 0 ? 0m : _routeLength;
                    AddInvisibleJump(_routePosition, wrapped, parts);
                    _routePosition = wrapped;
                }

                continue;
            }

            if (_law == BoundaryContinuationLaw.Clamp)
            {
                decimal next = decimal.Clamp(_routePosition + remaining, 0m, _routeLength);
                AddRouteMotion(_routePosition, next, parts);
                _routePosition = next;
                remaining = 0m;
                continue;
            }

            decimal continued = _routePosition + remaining;
            AddRouteMotion(_routePosition, continued, parts);
            _routePosition = continued;
            remaining = 0m;
        }

        return new PlanarTraversalEmission(parts);
    }

    public void SetLaw(BoundaryContinuationLaw law) => _law = law;

    private void AddRouteMotion(decimal from, decimal to, List<PlanarTraversalMotion> parts, bool endsStroke = false)
    {
        if (from == to || _routeLength == 0m)
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

            decimal worldFrom = MapRouteToWorld(segment.From);
            decimal worldTo = MapRouteToWorld(segment.To);
            if (worldFrom == worldTo)
            {
                continue;
            }

            parts.Add(new PlanarTraversalMotion(
                _definition.Project(new Scalar(worldTo - worldFrom)),
                segment.IsVisible,
                endsStroke && segment.IsVisible && index == slices.Length - 1));
        }
    }

    private void AddInvisibleJump(decimal from, decimal to, List<PlanarTraversalMotion> parts)
    {
        decimal worldFrom = MapRouteToWorld(from);
        decimal worldTo = MapRouteToWorld(to);
        if (worldFrom == worldTo)
        {
            return;
        }

        parts.Add(PlanarTraversalMotion.Hidden(
            _definition.Project(new Scalar(worldTo - worldFrom))));
    }

    private IEnumerable<RouteSlice> SplitAtLeadBoundary(decimal from, decimal to)
    {
        bool ascending = to > from;
        decimal low = ascending ? from : to;
        decimal high = ascending ? to : from;

        if (_leadLength <= low || _leadLength >= high)
        {
            yield return new RouteSlice(from, to, IsVisibleFor(from, to));
            yield break;
        }

        yield return new RouteSlice(from, _leadLength, IsVisibleFor(from, _leadLength));
        yield return new RouteSlice(_leadLength, to, IsVisibleFor(_leadLength, to));
    }

    private bool IsVisibleFor(decimal from, decimal to)
    {
        decimal midpoint = (from + to) * 0.5m;
        return midpoint >= _leadLength && _visibleLength > 0m;
    }

    private decimal MapRouteToWorld(decimal route)
    {
        if (route <= _leadLength)
        {
            return _hiddenDirection * route;
        }

        if (_visibleLength == 0m)
        {
            return _startCoordinate;
        }

        decimal beyondVisibleStart = route - _leadLength;
        return _startCoordinate + _visibleDirection * beyondVisibleStart;
    }

    private readonly record struct RouteSlice(decimal From, decimal To, bool IsVisible);
}
