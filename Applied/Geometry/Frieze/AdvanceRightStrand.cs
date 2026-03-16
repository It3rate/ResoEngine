using Applied.Geometry.Utils;
using Core2.Dynamic;

namespace Applied.Geometry.Frieze;

public sealed class AdvanceRightStrand : IDynamicStrand<StripPathState, StripEnvironment, Orientation2D>
{
    public string Name => "AdvanceRight";

    public IReadOnlyList<DynamicProposal<Orientation2D>> Propose(
        DynamicStrandContext<StripPathState, StripEnvironment> context) =>
        [
            new DynamicProposal<Orientation2D>(
                Name,
                context.Current.NodeId,
                new Orientation2D(1, 0),
                note: "Advance one unit to the right.")
        ];
}
