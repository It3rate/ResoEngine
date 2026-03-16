using Applied.Geometry.Utils;
using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public static class FriezeComposer
{
    public static FriezeResult ComposeToWidth(
        FriezePattern pattern,
        int minimumWidth,
        int maxSteps = 200)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        int boundedWidth = Math.Max(0, minimumWidth);
        int stepBudget = Math.Max(1, maxSteps);
        int maxRepeats = Math.Max(1, stepBudget / Math.Max(1, pattern.StepsPerRepeat));

        FriezeResult? latest = null;
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

    public static FriezeResult Compose(FriezePattern pattern, int repeats)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(repeats);

        return pattern.Program is null
            ? ComposeFromStrands(pattern, repeats)
            : ComposeFromProgram(pattern, repeats, pattern.Program);
    }

    private static FriezeResult ComposeFromStrands(FriezePattern pattern, int repeats)
    {
        var segments = new List<PlanarPathEdge>();
        var cursor = new PlanarPoint(0, 0);
        var bounds = new PlanarBoundsBuilder();
        bounds.Include(cursor);

        for (int step = 0; step < pattern.TotalSteps(repeats); step++)
        {
            var delta = PlanarOffset.Zero;
            foreach (var strand in pattern.Strands)
            {
                delta += strand.DeltaAt(step);
            }

            if (delta.IsZero)
            {
                continue;
            }

            var next = cursor + delta;
            segments.Add(new PlanarPathEdge(cursor, next));
            cursor = next;
            bounds.Include(cursor);
        }

        return new FriezeResult(pattern, new Proportion(repeats), segments, cursor, bounds.Build());
    }

    private static FriezeResult ComposeFromProgram(
        FriezePattern pattern,
        int repeats,
        EquationProgram program)
    {
        var runtime = program.Equations.ToDictionary(
            equation => equation.Name,
            equation => new PlanarSegmentRuntime(equation),
            StringComparer.OrdinalIgnoreCase);

        var segments = new List<PlanarPathEdge>();
        var cursor = new PlanarPoint(0, 0);
        var committed = cursor;
        PlanarPoint? visibleStart = null;
        var bounds = new PlanarBoundsBuilder();
        bounds.Include(cursor);

        Execute(program.Prelude);
        for (int repeat = 0; repeat < repeats; repeat++)
        {
            Execute(program.Loop);
        }

        return new FriezeResult(pattern, new Proportion(repeats), segments, committed, bounds.Build());

        void Execute(IReadOnlyList<EquationCommand>? commands)
        {
            if (commands is null)
            {
                return;
            }

            foreach (var command in commands)
            {
                switch (command.Kind)
                {
                    case CommandKind.Fire:
                    {
                        if (command.EquationName is null)
                        {
                            throw new InvalidOperationException("Fire commands require an equation name.");
                        }

                        var emission = runtime[command.EquationName].Fire();
                        foreach (var part in emission.Parts)
                        {
                            var start = cursor;
                            cursor += part.Delta;
                            bounds.Include(cursor);

                            if (part.IsVisible)
                            {
                                if (cursor != start)
                                {
                                    visibleStart ??= start;
                                }

                                if (part.EndsStroke && visibleStart is not null && cursor != visibleStart.Value)
                                {
                                    segments.Add(new PlanarPathEdge(visibleStart.Value, cursor));
                                    visibleStart = cursor;
                                }
                            }
                            else
                            {
                                if (visibleStart is not null && start != visibleStart.Value)
                                {
                                    segments.Add(new PlanarPathEdge(visibleStart.Value, start));
                                }

                                visibleStart = null;
                            }
                        }

                        break;
                    }
                    case CommandKind.Commit:
                    {
                        if (visibleStart is not null && cursor != visibleStart.Value)
                        {
                            segments.Add(new PlanarPathEdge(visibleStart.Value, cursor));
                        }

                        committed = cursor;
                        visibleStart = null;
                        break;
                    }
                    case CommandKind.SetLaw:
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
}
