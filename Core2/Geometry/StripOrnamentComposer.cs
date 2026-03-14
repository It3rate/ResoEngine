namespace Core2.Geometry;

public static class StripOrnamentComposer
{
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
            equation => new RuntimeEquation(equation),
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
                    case StripEquationCommandKind.SetMode:
                    {
                        if (command.EquationName is null || command.Mode is null)
                        {
                            throw new InvalidOperationException("Mode commands require an equation name and mode.");
                        }

                        runtime[command.EquationName].SetMode(command.Mode.Value);
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Unsupported equation command {command.Kind}.");
                }
            }
        }
    }

    private sealed class RuntimeEquation
    {
        private readonly StripDelta _delta;
        private readonly int _holdCount;
        private int _direction = 1;
        private int _stepsInDirection;
        private bool _flipPending;

        public RuntimeEquation(StripEquationDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _delta = definition.Delta;
            _holdCount = Math.Max(1, definition.HoldCount);
            Mode = definition.Mode;
        }

        public StripEquationMode Mode { get; private set; }

        public StripDelta Fire()
        {
            if (_flipPending && Mode == StripEquationMode.Bounce)
            {
                _direction *= -1;
                _flipPending = false;
                _stepsInDirection = 0;
            }

            var emitted = new StripDelta(_delta.Dx * _direction, _delta.Dy * _direction);
            if (Mode == StripEquationMode.Bounce)
            {
                _stepsInDirection++;
                if (_stepsInDirection >= _holdCount)
                {
                    _flipPending = true;
                }
            }

            return emitted;
        }

        public void SetMode(StripEquationMode mode)
        {
            Mode = mode;
            if (mode != StripEquationMode.Bounce)
            {
                _flipPending = false;
                _stepsInDirection = 0;
            }
        }
    }
}
