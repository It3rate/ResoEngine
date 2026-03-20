using Applied.Geometry.Utils;
using Core2.Branching;
using Core2.Symbolics.Dynamic;
using Core2.Symbolics.Branching;
using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public static class FriezeProgramDynamics
{
    public static DynamicTrace<FriezePathState, FriezeEnvironment, PlanarTraversalEmission> Run(
        FriezePattern pattern,
        int fireSteps)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(fireSteps);

        if (pattern.Program is null)
        {
            throw new InvalidOperationException($"Pattern '{pattern.Key}' does not define an equation program.");
        }

        var seed = new DynamicContext<FriezePathState, FriezeEnvironment>(
            FriezePathState.Origin,
            CreateEnvironment(pattern.Program.Equations));

        var graphBuilder = new BranchGraphBuilder<DynamicContext<FriezePathState, FriezeEnvironment>>();
        graphBuilder.Seed(seed, selectAsPrincipal: true);

        var runtime = pattern.Program.Equations.ToDictionary(
            equation => equation.Name,
            equation => new PlanarSegmentRuntime(equation),
            StringComparer.OrdinalIgnoreCase);

        var steps = new List<DynamicStep<FriezePathState, FriezeEnvironment, PlanarTraversalEmission>>();
        int executedFires = 0;

        foreach (var command in pattern.Program.Prelude ?? [])
        {
            if (executedFires >= fireSteps)
            {
                break;
            }

            Execute(command);
        }

        if (executedFires < fireSteps)
        {
            foreach (var command in pattern.Program.EnumerateLoopForever())
            {
                if (executedFires >= fireSteps)
                {
                    break;
                }

                Execute(command);
            }
        }

        return new DynamicTrace<FriezePathState, FriezeEnvironment, PlanarTraversalEmission>(
            seed,
            steps,
            graphBuilder.Build());

        void Execute(EquationCommand command)
        {
            switch (command.Kind)
            {
                case CommandKind.SetLaw:
                {
                    if (command.EquationName is null)
                    {
                        throw new InvalidOperationException("Boundary commands require an equation name.");
                    }

                    if (command.BoundaryPins is not null)
                    {
                        runtime[command.EquationName].SetBoundaryPins(command.BoundaryPins);
                        break;
                    }

                    if (command.Law is null)
                    {
                        throw new InvalidOperationException("Boundary commands require either explicit pins or a compatibility law.");
                    }

                    runtime[command.EquationName].SetLaw(command.Law.Value);
                    break;
                }
                case CommandKind.Commit:
                    break;
                case CommandKind.Fire:
                {
                    if (command.EquationName is null)
                    {
                        throw new InvalidOperationException("Fire commands require an equation name.");
                    }

                    ExecuteFire(command.EquationName);
                    break;
                }
                default:
                    throw new InvalidOperationException($"Unsupported equation command {command.Kind}.");
            }
        }

        void ExecuteFire(string equationName)
        {
            var frontier = GetFrontier(graphBuilder);
            if (frontier.Count == 0)
            {
                return;
            }

            var incoming = frontier[0];
            var emission = runtime[equationName].Fire();
            var proposal = new DynamicProposal<PlanarTraversalEmission>(
                equationName,
                incoming.NodeId,
                emission,
                note: pattern.CallPattern ?? equationName);

            var (nextContext, tensions, note) = ApplyEmission(incoming.Context, emission, equationName);
            var resolution = DynamicResolution<FriezePathState, FriezeEnvironment, PlanarTraversalEmission>.Commit(
                nextContext,
                [proposal],
                tensions,
                note);

            graphBuilder.ApplyFamily(resolution.Outcomes, BranchEventKind.Family);
            var outgoing = GetFrontier(graphBuilder);

            steps.Add(new DynamicStep<FriezePathState, FriezeEnvironment, PlanarTraversalEmission>(
                steps.Count,
                frontier,
                [proposal],
                resolution,
                outgoing));

            executedFires++;
        }
    }

    private static (DynamicContext<FriezePathState, FriezeEnvironment> Context, IReadOnlyList<DynamicTension> Tensions, string Note) ApplyEmission(
        DynamicContext<FriezePathState, FriezeEnvironment> incoming,
        PlanarTraversalEmission emission,
        string equationName)
    {
        var cursor = incoming.State.Cursor;
        var edges = new List<PlanarPathEdge>();
        var tensions = new List<DynamicTension>();

        foreach (var part in emission.Parts)
        {
            var delta = part.Delta;
            if (delta.IsZero)
            {
                continue;
            }

            var next = cursor + delta;
            var edge = new PlanarPathEdge(cursor, next);
            if (!incoming.Environment.Contains(next))
            {
                tensions.Add(new DynamicTension(
                    "VerticalBounds",
                    $"Move to y={next.Y} exceeds frieze bounds [{incoming.Environment.MinY}, {incoming.Environment.MaxY}].",
                    10m));
            }

            if (part.IsVisible && incoming.Environment.Contains(edge))
            {
                tensions.Add(new DynamicTension(
                    "EdgeReuse",
                    $"Segment {edge.Start} -> {edge.End} was already occupied.",
                    3m));
            }

            if (part.IsVisible)
            {
                edges.Add(edge);
            }

            cursor = next;
        }

        var nextState = new FriezePathState(
            cursor,
            incoming.State.Segments.Concat(edges).ToArray(),
            incoming.State.MacroStepValue + Proportion.One);
        var nextEnvironment = incoming.Environment.WithAddedEdges(edges);

        return (
            new DynamicContext<FriezePathState, FriezeEnvironment>(nextState, nextEnvironment),
            tensions,
            emission.IsEmpty
                ? $"{equationName} emitted no visible or hidden motion."
                : $"{equationName} advanced by {emission.NetDelta}.");
    }

    private static FriezeEnvironment CreateEnvironment(IReadOnlyList<PlanarSegmentDefinition> equations)
    {
        Proportion min = Proportion.Zero;
        Proportion max = Proportion.Zero;

        foreach (var equation in equations.Where(equation => !equation.AxisVector.Vertical.IsZero))
        {
            Proportion projection = equation.AxisVector.Vertical;
            Proportion start = equation.Segment.StartCoordinate * projection;
            Proportion end = equation.Segment.EndCoordinate * projection;
            min = Proportion.Min(min, Proportion.Min(start, end));
            max = Proportion.Max(max, Proportion.Max(start, end));
        }

        return FriezeEnvironment.Create(min, max);
    }

    private static IReadOnlyList<DynamicFrontierContext<FriezePathState, FriezeEnvironment>> GetFrontier(
        BranchGraphBuilder<DynamicContext<FriezePathState, FriezeEnvironment>> builder) =>
        builder.GetFrontierNodes()
            .Select(node => new DynamicFrontierContext<FriezePathState, FriezeEnvironment>(node.Id, node.Value))
            .ToArray();
}
