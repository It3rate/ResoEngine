using Core2.Elements;
using Core2.Resolution;

namespace Tests.Core2;

public class PrimitiveProportionResolutionTests
{
    [Fact]
    public void RefineToSupport_PreservesFoldWhileIncreasingSupport()
    {
        var refined = PrimitiveProportionResolution.RefineToSupport(new Proportion(4, 5), 10);

        Assert.Equal(new Proportion(8, 10), refined);
        Assert.Equal(new Scalar(0.8m), refined.Fold());
    }

    [Fact]
    public void RefineToSupport_UsesLcmWhenRequestedSupportDoesNotDivideExactly()
    {
        var refined = PrimitiveProportionResolution.RefineToSupport(new Proportion(1, 3), 10);

        Assert.Equal(new Proportion(10, 30), refined);
        Assert.Equal(new Scalar(1m / 3m), refined.Fold());
    }

    [Fact]
    public void CommonFrameAdd_AlignsBySharedSupportInsteadOfProductSupport()
    {
        var result = PrimitiveProportionResolution.CommonFrameAdd(
            new Proportion(5, 10),
            new Proportion(1, 2));

        Assert.Equal(new Proportion(10, 10), result);
    }

    [Fact]
    public void Aggregate_PoolsNumeratorsAndSupports()
    {
        var result = PrimitiveProportionResolution.Aggregate(
            new Proportion(5, 10),
            new Proportion(1, 2));

        Assert.Equal(new Proportion(6, 12), result);
    }
}
