using Core2.Elements;
using Core2.Resolution;

namespace Tests.Core2;

public class PrimitiveResultSupportRuntimeTests
{
    [Fact]
    public void ScaleHostedValue_PreservesHostSupportWhenExact()
    {
        var result = PrimitiveResultSupportRuntime.ScaleHostedValue(
            new Proportion(5, 10),
            new Proportion(2, 1));

        Assert.Equal(ResultSupportPolicy.PreserveHost, result.Policy);
        Assert.Equal(new Proportion(10, 10), result.ExactValue);
        Assert.Equal(10, result.RequestedSupport);
        Assert.Equal(new Proportion(10, 10), result.CommittedValue);
        Assert.True(result.UsesRequestedSupportExactly);
    }

    [Fact]
    public void ScaleHostedValue_RaisesActualSupportWhenHostSupportCannotCarryExactResult()
    {
        var result = PrimitiveResultSupportRuntime.ScaleHostedValue(
            new Proportion(5, 10),
            new Proportion(3, 4));

        Assert.Equal(new Proportion(15, 40), result.ExactValue);
        Assert.Equal(10, result.RequestedSupport);
        Assert.Equal(new Proportion(15, 40), result.CommittedValue);
        Assert.False(result.UsesRequestedSupportExactly);
    }

    [Fact]
    public void AddInCommonFrame_PreserveCoarserRequestsCoarseSupportWithoutForcingInexactCollapse()
    {
        var result = PrimitiveResultSupportRuntime.AddInCommonFrame(
            new Proportion(1, 3),
            new Proportion(1, 6),
            ResultSupportPolicy.PreserveCoarser);

        Assert.Equal(new Proportion(3, 6), result.ExactValue);
        Assert.Equal(3, result.RequestedSupport);
        Assert.Equal(new Proportion(3, 6), result.CommittedValue);
        Assert.False(result.UsesRequestedSupportExactly);
    }

    [Fact]
    public void CommitExactValue_NegotiateFromUncertaintyEstimatesRequestedSupportFromInputs()
    {
        var exact = PrimitiveProportionResolution.CommonFrameAdd(
            new Proportion(2, 4),
            new Proportion(50, 100));

        var result = PrimitiveResultSupportRuntime.CommitExactValue(
            exact,
            ResultSupportPolicy.NegotiateFromUncertainty,
            null,
            new Proportion(2, 4),
            new Proportion(50, 100));

        Assert.Equal(4, result.RequestedSupport);
        Assert.Equal(exact, result.CommittedValue);
    }
}
