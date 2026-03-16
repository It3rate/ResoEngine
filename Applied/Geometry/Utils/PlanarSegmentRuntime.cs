using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed class PlanarSegmentRuntime
{
    private readonly PlanarSegmentDefinition _definition;
    private readonly Axis _routeCarrier;
    private readonly AxisTraversalState _routeTraversal;
    private readonly Proportion _leadLength;
    private readonly Proportion _visibleLength;
    private readonly Proportion _routeLength;
    private readonly Proportion _startCoordinate;
    private readonly Proportion _visibleSpan;
    private readonly int _hiddenDirection;
    private readonly int _visibleDirection;
    private bool _needsStartJump;

    public PlanarSegmentRuntime(PlanarSegmentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
        _startCoordinate = definition.Segment.StartCoordinate;
        _visibleSpan = definition.Segment.CoordinateSpan;
        _leadLength = _startCoordinate.Abs();
        _visibleLength = _visibleSpan.Abs();
        _routeLength = _leadLength + _visibleLength;
        _routeCarrier = Axis.FromCoordinates(Proportion.Zero, _routeLength);
        _hiddenDirection = _startCoordinate.Sign;
        _visibleDirection = _visibleSpan.Sign;
        Proportion step = definition.ComputeStep();
        Proportion seed = step.Sign < 0 ? _routeLength : Proportion.Zero;
        _routeTraversal = new AxisTraversalDefinition(
            _routeCarrier,
            step,
            definition.Law,
            seed).CreateState();
        _needsStartJump = step.Sign < 0 && !seed.IsZero;
    }

    public PlanarTraversalEmission Fire()
        => new(IterateFire().ToArray());

    public IEnumerable<PlanarTraversalMotion> IterateFire()
    {
        if (_needsStartJump)
        {
            foreach (var motion in EnumerateInvisibleJump(_routeCarrier.StartCoordinate, _routeTraversal.Value))
            {
                yield return motion;
            }

            _needsStartJump = false;
        }

        AxisTraversalStep[] parts = _routeTraversal.EnumerateFire().ToArray();
        if (parts.Length == 0)
        {
            yield break;
        }

        Proportion? previousEnd = null;
        foreach (var part in parts)
        {
            if (previousEnd is Proportion jumpFrom && jumpFrom != part.Start)
            {
                foreach (var motion in EnumerateInvisibleJump(jumpFrom, part.Start))
                {
                    yield return motion;
                }
            }

            foreach (var motion in EnumerateRouteMotion(part.Segment, part.BreakAfter))
            {
                yield return motion;
            }

            previousEnd = part.End;
        }
    }

    public void SetLaw(BoundaryContinuationLaw law) => _routeTraversal.SetLaw(law);

    private IEnumerable<PlanarTraversalMotion> EnumerateRouteMotion(Axis route, bool endsStroke = false)
    {
        if (route.IsDegenerate || _routeLength.IsZero)
        {
            yield break;
        }

        var slices = SplitAtLeadBoundary(route).ToArray();
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

            yield return new PlanarTraversalMotion(
                _definition.Project(worldTo - worldFrom),
                slice.IsVisible,
                endsStroke && slice.IsVisible && index == slices.Length - 1);
        }
    }

    private IEnumerable<PlanarTraversalMotion> EnumerateInvisibleJump(Proportion from, Proportion to)
    {
        Proportion worldFrom = MapRouteToWorld(from);
        Proportion worldTo = MapRouteToWorld(to);
        if (worldFrom == worldTo)
        {
            yield break;
        }

        yield return PlanarTraversalMotion.Hidden(
            _definition.Project(worldTo - worldFrom));
    }

    private IEnumerable<RouteSlice> SplitAtLeadBoundary(Axis route)
    {
        Proportion from = route.StartCoordinate;
        Proportion to = route.EndCoordinate;
        Proportion low = route.LeftCoordinate;
        Proportion high = route.RightCoordinate;

        if (_leadLength <= low || _leadLength >= high)
        {
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

    private static Proportion ApplyDirection(Proportion magnitude, int direction) =>
        direction < 0 ? -magnitude : magnitude;

    private readonly record struct RouteSlice(Axis Route, bool IsVisible);
}
