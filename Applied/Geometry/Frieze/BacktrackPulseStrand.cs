using Applied.Geometry.Utils;
using Core2.Dynamic;

namespace Applied.Geometry.Frieze;

public sealed class BacktrackPulseStrand : IDynamicStrand<StripPathState, StripEnvironment, Orientation2D>
{
    public string Name => "BacktrackPulse";

    public IReadOnlyList<DynamicProposal<Orientation2D>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context)
    {
        int phase = context.StepIndex % 6;
        return phase is 0 or 3
            ? [new DynamicProposal<Orientation2D>(Name, context.Current.NodeId, new Orientation2D(-1, 0), note: "Cancel the first horizontal move of the cycle.")]
            : [];
    }
}
