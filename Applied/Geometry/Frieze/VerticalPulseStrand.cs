using Applied.Geometry.Utils;
using Core2.Dynamic;

namespace Applied.Geometry.Frieze;

public sealed class VerticalPulseStrand : IDynamicStrand<StripPathState, StripEnvironment, Orientation2D>
{
    public string Name => "VerticalPulse";

    public IReadOnlyList<DynamicProposal<Orientation2D>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context)
    {
        int phase = context.StepIndex % 6;
        return phase switch
        {
            0 => [new DynamicProposal<Orientation2D>(Name, context.Current.NodeId, new Orientation2D(0, 1), note: "Pulse upward.")],
            3 => [new DynamicProposal<Orientation2D>(Name, context.Current.NodeId, new Orientation2D(0, -1), note: "Pulse downward.")],
            _ => [],
        };
    }
}
