using Core2.Dynamic;

namespace Core2.Geometry;

public sealed class AdvanceRightStrand : IDynamicStrand<StripPathState, StripEnvironment, StripEffect>
{
    public string Name => "AdvanceRight";

    public IReadOnlyList<DynamicProposal<StripEffect>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context) =>
        [
            new DynamicProposal<StripEffect>(
                Name,
                context.Current.NodeId,
                new StripEffect(1, 0),
                note: "Advance one unit to the right.")
        ];
}
