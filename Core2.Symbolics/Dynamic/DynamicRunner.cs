using Core2.Branching;

namespace Core2.Symbolics.Dynamic;

public sealed class DynamicRunner<TState, TEnvironment, TEffect>
{
    private readonly IReadOnlyList<IDynamicStrand<TState, TEnvironment, TEffect>> _strands;
    private readonly IDynamicResolver<TState, TEnvironment, TEffect> _resolver;
    private readonly IConvergencePolicy<TState, TEnvironment, TEffect> _convergencePolicy;

    public DynamicRunner(
        IReadOnlyList<IDynamicStrand<TState, TEnvironment, TEffect>> strands,
        IDynamicResolver<TState, TEnvironment, TEffect> resolver,
        IConvergencePolicy<TState, TEnvironment, TEffect> convergencePolicy)
    {
        ArgumentNullException.ThrowIfNull(strands);
        ArgumentNullException.ThrowIfNull(resolver);
        ArgumentNullException.ThrowIfNull(convergencePolicy);

        _strands = strands;
        _resolver = resolver;
        _convergencePolicy = convergencePolicy;
    }

    public DynamicTrace<TState, TEnvironment, TEffect> Run(DynamicContext<TState, TEnvironment> seed)
    {
        ArgumentNullException.ThrowIfNull(seed);

        var graphBuilder = new BranchGraphBuilder<DynamicContext<TState, TEnvironment>>();
        graphBuilder.Seed(seed, selectAsPrincipal: true);

        var steps = new List<DynamicStep<TState, TEnvironment, TEffect>>();

        while (true)
        {
            var frontier = GetFrontier(graphBuilder);
            if (frontier.Count == 0)
            {
                break;
            }

            var convergenceState = new DynamicConvergenceState<TState, TEnvironment, TEffect>(seed, steps, frontier);
            if (_convergencePolicy.ShouldStop(convergenceState))
            {
                break;
            }

            int stepIndex = steps.Count;
            var proposals = CollectProposals(stepIndex, frontier);
            var resolution = _resolver.Resolve(new DynamicResolutionInput<TState, TEnvironment, TEffect>(stepIndex, frontier, proposals));

            BranchEventKind eventKind = resolution.Kind == DynamicResolutionKind.Lifted
                ? BranchEventKind.Lift
                : BranchEventKind.Family;

            graphBuilder.ApplyFamily(resolution.Outcomes, eventKind);
            var outgoingFrontier = GetFrontier(graphBuilder);

            steps.Add(new DynamicStep<TState, TEnvironment, TEffect>(
                stepIndex,
                frontier,
                proposals,
                resolution,
                outgoingFrontier));
        }

        return new DynamicTrace<TState, TEnvironment, TEffect>(seed, steps, graphBuilder.Build());
    }

    private IReadOnlyList<DynamicProposal<TEffect>> CollectProposals(
        int stepIndex,
        IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> frontier)
    {
        var proposals = new List<DynamicProposal<TEffect>>();
        foreach (var strand in _strands)
        {
            foreach (var current in frontier)
            {
                var context = new DynamicStrandContext<TState, TEnvironment>(stepIndex, current, frontier);
                proposals.AddRange(strand.Propose(context));
            }
        }

        return proposals;
    }

    private static IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> GetFrontier(
        BranchGraphBuilder<DynamicContext<TState, TEnvironment>> builder) =>
        builder.GetFrontierNodes()
            .Select(node => new DynamicFrontierContext<TState, TEnvironment>(node.Id, node.Value))
            .ToArray();
}
