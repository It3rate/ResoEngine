using Core2.Dynamic;

namespace Applied.Geometry.Utils;

public sealed class ScheduledPlanarSegmentStrand<TState, TEnvironment> : IDynamicStrand<TState, TEnvironment, PlanarTraversalMotion>
{
    private readonly HashSet<int> _activePhases;
    private readonly int _period;
    private readonly PlanarSegmentDefinition _segment;
    private readonly bool _isVisible;
    private readonly string? _note;

    public ScheduledPlanarSegmentStrand(
        string name,
        PlanarSegmentDefinition segment,
        IEnumerable<int> activePhases,
        int period,
        bool isVisible = true,
        string? note = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(segment);
        ArgumentNullException.ThrowIfNull(activePhases);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(period);

        Name = name;
        _segment = segment;
        _activePhases = activePhases
            .Select(phase => NormalizePhase(phase, period))
            .ToHashSet();
        _period = period;
        _isVisible = isVisible;
        _note = note;
    }

    public string Name { get; }

    public IReadOnlyList<DynamicProposal<PlanarTraversalMotion>> Propose(
        DynamicStrandContext<TState, TEnvironment> context)
    {
        if (!_activePhases.Contains(NormalizePhase(context.StepIndex, _period)))
        {
            return [];
        }

        var state = _segment.CreateTraversal().CreateState();
        for (int step = 0; step <= context.StepIndex; step++)
        {
            if (!_activePhases.Contains(NormalizePhase(step, _period)))
            {
                continue;
            }

            var traversal = state.Fire();
            if (step != context.StepIndex)
            {
                continue;
            }

            var delta = _segment.Project(traversal.Delta);
            if (delta.IsZero)
            {
                return [];
            }

            return
            [
                new DynamicProposal<PlanarTraversalMotion>(
                    Name,
                    context.Current.NodeId,
                    new PlanarTraversalMotion(delta, _isVisible),
                    note: _note ?? _segment.DescribeTraversal())
            ];
        }

        return [];
    }

    private static int NormalizePhase(int value, int period) =>
        ((value % period) + period) % period;
}
