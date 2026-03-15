using Core2.Elements;
using Core2.Repetition;

namespace Core2.Geometry;

public sealed record StripSegmentDefinition(
    string Name,
    AxisTraversalDefinition Traversal,
    StripDelta AxisVector)
{
    public StripDelta Project(Scalar delta)
    {
        int amount = decimal.ToInt32(delta.Value);
        return new StripDelta(AxisVector.Dx * amount, AxisVector.Dy * amount);
    }
}
