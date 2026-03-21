using Core2.Elements;
using Core2.Resolution;

namespace Tests.Core2;

public class ResolutionPrimitiveLawTests
{
    [Fact]
    public void PrimitiveSupportLaw_VocabularyRemainsSmallAndExplicit()
    {
        Assert.Equal(
            ["Inherit", "Aggregate", "Compose", "Refine", "CommonFrame"],
            Enum.GetNames<PrimitiveSupportLaw>());
    }

    [Fact]
    public void ResultSupportPolicy_VocabularyRemainsExplicitAndSeparate()
    {
        Assert.Equal(
            [
                "PreserveCoarser",
                "PreserveFiner",
                "PreserveHost",
                "PreserveExactAlignment",
                "NegotiateFromUncertainty",
                "PreserveExactStructure",
            ],
            Enum.GetNames<ResultSupportPolicy>());
    }

    [Fact]
    public void HostedScale_DefaultsToInherit()
    {
        var law = PrimitiveResolutionDefaults.ClassifyHostedScale(
            new Proportion(5, 10),
            new Proportion(2, 1));

        Assert.Equal(PrimitiveSupportLaw.Inherit, law);
    }

    [Fact]
    public void PureTransform_DefaultsToInherit()
    {
        var law = PrimitiveResolutionDefaults.ClassifyTransformApplication(
            new Axis(new Proportion(15, 5), new Proportion(10, 5)),
            Axis.I);

        Assert.Equal(PrimitiveSupportLaw.Inherit, law);
    }

    [Fact]
    public void PooledEvidence_DefaultsToAggregate()
    {
        Assert.Equal(
            PrimitiveSupportLaw.Aggregate,
            PrimitiveResolutionDefaults.ClassifyAggregation());
    }

    [Fact]
    public void IndependentProduct_DefaultsToCompose()
    {
        var law = PrimitiveResolutionDefaults.ClassifyIndependentComposition(
            new Proportion(5, 10),
            new Proportion(1, 2));

        Assert.Equal(PrimitiveSupportLaw.Compose, law);
    }

    [Fact]
    public void Refinement_DefaultsToRefine()
    {
        var law = PrimitiveResolutionDefaults.ClassifyRefinement(new Proportion(4, 5));

        Assert.Equal(PrimitiveSupportLaw.Refine, law);
    }

    [Fact]
    public void CommonFrameAddition_DefaultsToCommonFrame()
    {
        var law = PrimitiveResolutionDefaults.ClassifyCommonFrameCombination(
            new Proportion(5, 10),
            new Proportion(1, 2));

        Assert.Equal(PrimitiveSupportLaw.CommonFrame, law);
    }

    [Fact]
    public void PowerProjection_DefaultsToCompose()
    {
        var law = PrimitiveResolutionDefaults.ClassifyPowerProjection(
            Axis.I,
            new Proportion(2, 1));

        Assert.Equal(PrimitiveSupportLaw.Compose, law);
    }

    [Fact]
    public void InverseContinuationProjection_DefaultsToCompose()
    {
        var law = PrimitiveResolutionDefaults.ClassifyInverseContinuationProjection(
            new Scalar(4m),
            new Proportion(2, 1));

        Assert.Equal(PrimitiveSupportLaw.Compose, law);
    }
}
