using Core2.Dynamic;

namespace Core2.Geometry;

public sealed class VerticalPulseStrand : IDynamicStrand<StripPathState, StripEnvironment, StripEffect>
{
    public string Name => "VerticalPulse";

    public IReadOnlyList<DynamicProposal<StripEffect>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context)
    {
        int phase = context.StepIndex % 6;
        return phase switch
        {
            0 => [new DynamicProposal<StripEffect>(Name, context.Current.NodeId, new StripEffect(0, 1), note: "Pulse upward.")],
            3 => [new DynamicProposal<StripEffect>(Name, context.Current.NodeId, new StripEffect(0, -1), note: "Pulse downward.")],
            _ => [],
        };
    }
}
