using Core2.Branching;

namespace Core2.Dynamic;

public sealed class DynamicMachine<TState, TEnvironment, TEffect>
{
    private readonly IReadOnlyList<IDynamicStrand<TState, TEnvironment, TEffect>> _strands;
    private readonly IDynamicResolver<TState, TEnvironment, TEffect> _resolver;
    private readonly IConvergencePolicy<TState, TEnvironment, TEffect> _convergencePolicy;
    private readonly DynamicContext<TState, TEnvironment> _seed;
    private readonly BranchGraphBuilder<DynamicContext<TState, TEnvironment>> _graphBuilder = new();
    private readonly List<DynamicStep<TState, TEnvironment, TEffect>> _steps = [];
    private bool _isCompleted;

    public DynamicMachine(
        DynamicContext<TState, TEnvironment> seed,
        IReadOnlyList<IDynamicStrand<TState, TEnvironment, TEffect>> strands,
        IDynamicResolver<TState, TEnvironment, TEffect> resolver,
        IConvergencePolicy<TState, TEnvironment, TEffect> convergencePolicy)
    {
        ArgumentNullException.ThrowIfNull(seed);
        ArgumentNullException.ThrowIfNull(strands);
        ArgumentNullException.ThrowIfNull(resolver);
        ArgumentNullException.ThrowIfNull(convergencePolicy);

        _seed = seed;
        _strands = strands;
        _resolver = resolver;
        _convergencePolicy = convergencePolicy;
        _graphBuilder.Seed(seed, selectAsPrincipal: true);
    }

    public bool IsCompleted => _isCompleted;

    public int StepCount => _steps.Count;

    public IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> CurrentFrontier =>
        GetFrontier();

    public bool Step()
    {
        if (_isCompleted)
        {
            return false;
        }

        var frontier = GetFrontier();
        if (frontier.Count == 0)
        {
            _isCompleted = true;
            return false;
        }

        var convergenceState = new DynamicConvergenceState<TState, TEnvironment, TEffect>(_seed, _steps, frontier);
        if (_convergencePolicy.ShouldStop(convergenceState))
        {
            _isCompleted = true;
            return false;
        }

        int stepIndex = _steps.Count;
        var proposals = CollectProposals(stepIndex, frontier);
        var resolution = _resolver.Resolve(new DynamicResolutionInput<TState, TEnvironment, TEffect>(stepIndex, frontier, proposals));
        BranchEventKind eventKind = resolution.Kind == DynamicResolutionKind.Lifted
            ? BranchEventKind.Lift
            : BranchEventKind.Family;

        _graphBuilder.ApplyFamily(resolution.Outcomes, eventKind);
        var outgoing = GetFrontier();
        _steps.Add(new DynamicStep<TState, TEnvironment, TEffect>(stepIndex, frontier, proposals, resolution, outgoing));

        if (outgoing.Count == 0)
        {
            _isCompleted = true;
        }

        return true;
    }

    public DynamicTrace<TState, TEnvironment, TEffect> RunToCompletion()
    {
        while (Step())
        {
        }

        return Snapshot();
    }

    public DynamicTrace<TState, TEnvironment, TEffect> Snapshot() =>
        new(_seed, _steps.ToArray(), _graphBuilder.Build());

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

    private IReadOnlyList<DynamicFrontierContext<TState, TEnvironment>> GetFrontier() =>
        _graphBuilder.GetFrontierNodes()
            .Select(node => new DynamicFrontierContext<TState, TEnvironment>(node.Id, node.Value))
            .ToArray();
}
