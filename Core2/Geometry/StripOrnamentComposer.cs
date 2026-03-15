using Core2.Repetition;

namespace Core2.Geometry;

public static class StripOrnamentComposer
{
    public static StripOrnamentResult ComposeToWidth(
        StripOrnamentPattern pattern,
        int minimumWidth,
        int maxSteps = 200)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        int boundedWidth = Math.Max(0, minimumWidth);
        int stepBudget = Math.Max(1, maxSteps);
        int maxRepeats = Math.Max(1, stepBudget / Math.Max(1, pattern.StepsPerRepeat));

        StripOrnamentResult? latest = null;
        for (int repeats = 1; repeats <= maxRepeats; repeats++)
        {
            latest = Compose(pattern, repeats);
            if (latest.HorizontalSpan >= boundedWidth)
            {
                return latest;
            }
        }

        return latest ?? Compose(pattern, 1);
    }

    public static StripOrnamentResult Compose(StripOrnamentPattern pattern, int repeats)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(repeats);

        return pattern.Program is null
            ? ComposeFromStrands(pattern, repeats)
            : ComposeFromProgram(pattern, repeats, pattern.Program);
    }

    private static StripOrnamentResult ComposeFromStrands(StripOrnamentPattern pattern, int repeats)
    {
        var segments = new List<StripPathEdge>();
        var cursor = new StripPoint(0, 0);
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        for (int step = 0; step < pattern.TotalSteps(repeats); step++)
        {
            var delta = StripDelta.Zero;
            foreach (var strand in pattern.Strands)
            {
                delta += strand.DeltaAt(step);
            }

            if (delta.IsZero)
            {
                continue;
            }

            var next = cursor + delta;
            segments.Add(new StripPathEdge(cursor, next));
            cursor = next;

            minX = Math.Min(minX, cursor.X);
            maxX = Math.Max(maxX, cursor.X);
            minY = Math.Min(minY, cursor.Y);
            maxY = Math.Max(maxY, cursor.Y);
        }

        return new StripOrnamentResult(pattern, repeats, segments, cursor, minX, maxX, minY, maxY);
    }

    private static StripOrnamentResult ComposeFromProgram(
        StripOrnamentPattern pattern,
        int repeats,
        StripEquationProgram program)
    {
        var runtime = program.Equations.ToDictionary(
            equation => equation.Name,
            equation => new RuntimeSegment(equation),
            StringComparer.OrdinalIgnoreCase);

        var segments = new List<StripPathEdge>();
        var cursor = new StripPoint(0, 0);
        var committed = cursor;
        StripPoint? visibleStart = null;
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        Execute(program.Prelude);
        for (int repeat = 0; repeat < repeats; repeat++)
        {
            Execute(program.Loop);
        }

        return new StripOrnamentResult(pattern, repeats, segments, committed, minX, maxX, minY, maxY);

        void Execute(IReadOnlyList<StripEquationCommand>? commands)
        {
            if (commands is null)
            {
                return;
            }

            foreach (var command in commands)
            {
                switch (command.Kind)
                {
                    case StripEquationCommandKind.Fire:
                    {
                        if (command.EquationName is null)
                        {
                            throw new InvalidOperationException("Fire commands require an equation name.");
                        }

                        var motion = runtime[command.EquationName].Fire();
                        foreach (var part in motion.Parts)
                        {
                            var start = cursor;
                            cursor += part.Delta;
                            minX = Math.Min(minX, cursor.X);
                            maxX = Math.Max(maxX, cursor.X);
                            minY = Math.Min(minY, cursor.Y);
                            maxY = Math.Max(maxY, cursor.Y);

                            if (part.IsVisible)
                            {
                                if (cursor != start)
                                {
                                    visibleStart ??= start;
                                }
                            }
                            else
                            {
                                if (visibleStart is not null && start != visibleStart.Value)
                                {
                                    segments.Add(new StripPathEdge(visibleStart.Value, start));
                                }

                                visibleStart = null;
                            }
                        }

                        break;
                    }
                    case StripEquationCommandKind.Commit:
                    {
                        if (visibleStart is not null && cursor != visibleStart.Value)
                        {
                            segments.Add(new StripPathEdge(visibleStart.Value, cursor));
                        }

                        committed = cursor;
                        visibleStart = null;
                        break;
                    }
                    case StripEquationCommandKind.SetLaw:
                    {
                        if (command.EquationName is null || command.Law is null)
                        {
                            throw new InvalidOperationException("Law commands require an equation name and law.");
                        }

                        runtime[command.EquationName].SetLaw(command.Law.Value);
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Unsupported equation command {command.Kind}.");
                }
            }
        }
    }

    private sealed class RuntimeSegment
    {
        private readonly StripSegmentDefinition _definition;
        private readonly decimal _leadLength;
        private readonly decimal _visibleLength;
        private readonly decimal _routeLength;
        private readonly decimal _stepMagnitude;
        private readonly decimal _startCoordinate;
        private readonly decimal _endCoordinate;
        private readonly int _hiddenDirection;
        private readonly int _visibleDirection;
        private decimal _routePosition;
        private int _direction;
        private BoundaryContinuationLaw _law;

        public RuntimeSegment(StripSegmentDefinition definition)
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
            _routePosition = 0m;
            _direction = 1;
            _law = definition.Law;
        }

        public RuntimeMotion Fire()
        {
            if (_stepMagnitude == 0m)
            {
                return new RuntimeMotion([]);
            }

            var parts = new List<RuntimeMotionPart>();
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
                    AddRouteMotion(_routePosition, next, parts);
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

            return new RuntimeMotion(parts);
        }

        public void SetLaw(BoundaryContinuationLaw law) => _law = law;

        private void AddRouteMotion(decimal from, decimal to, List<RuntimeMotionPart> parts)
        {
            if (from == to)
            {
                return;
            }

            if (_routeLength == 0m)
            {
                return;
            }

            foreach (var segment in SplitAtLeadBoundary(from, to))
            {
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

                parts.Add(new RuntimeMotionPart(
                    _definition.Project(new Core2.Elements.Scalar(worldTo - worldFrom)),
                    segment.IsVisible));
            }
        }

        private void AddInvisibleJump(decimal from, decimal to, List<RuntimeMotionPart> parts)
        {
            decimal worldFrom = MapRouteToWorld(from);
            decimal worldTo = MapRouteToWorld(to);
            if (worldFrom == worldTo)
            {
                return;
            }

            parts.Add(new RuntimeMotionPart(
                _definition.Project(new Core2.Elements.Scalar(worldTo - worldFrom)),
                IsVisible: false));
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
    }

    private readonly record struct RuntimeMotion(IReadOnlyList<RuntimeMotionPart> Parts);
    private readonly record struct RuntimeMotionPart(StripDelta Delta, bool IsVisible);
    private readonly record struct RouteSlice(decimal From, decimal To, bool IsVisible);
}
