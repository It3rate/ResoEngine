using Core2.Dynamic;

namespace Core2.Geometry;

public sealed class BacktrackPulseStrand : IDynamicStrand<StripPathState, StripEnvironment, StripEffect>
{
    public string Name => "BacktrackPulse";

    public IReadOnlyList<DynamicProposal<StripEffect>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context)
    {
        int phase = context.StepIndex % 6;
        return phase is 0 or 3
            ? [new DynamicProposal<StripEffect>(Name, context.Current.NodeId, new StripEffect(-1, 0), note: "Cancel the first horizontal move of the cycle.")]
            : [];
    }
}
