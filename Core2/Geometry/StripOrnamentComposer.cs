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

                        cursor += runtime[command.EquationName].Fire();
                        minX = Math.Min(minX, cursor.X);
                        maxX = Math.Max(maxX, cursor.X);
                        minY = Math.Min(minY, cursor.Y);
                        maxY = Math.Max(maxY, cursor.Y);
                        break;
                    }
                    case StripEquationCommandKind.Commit:
                    {
                        if (cursor != committed)
                        {
                            segments.Add(new StripPathEdge(committed, cursor));
                            committed = cursor;
                        }

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
        private readonly AxisTraversalState _state;
        private bool _needsLeadIn;

        public RuntimeSegment(StripSegmentDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _definition = definition;
            _state = definition.CreateTraversal().CreateState();
            _needsLeadIn = !_definition.Segment.Start.IsZero;
        }

        public StripDelta Fire()
        {
            if (_needsLeadIn)
            {
                _needsLeadIn = false;
                return _definition.Project(_definition.Segment.Start);
            }

            var step = _state.Fire();
            return _definition.Project(step.Delta);
        }

        public void SetLaw(BoundaryContinuationLaw law) => _state.SetLaw(law);
    }
}
