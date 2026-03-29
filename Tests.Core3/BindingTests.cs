using Core3.Binding;

namespace Tests.Core3;

public sealed class BindingTests
{
    [Fact]
    public void BoundScalarTemplate_CanDescribeInheritedAndCoupledSlots()
    {
        var template = new BoundScalarTemplate
        {
            Value = new BoundSlot<long>
            {
                Literal = 25
            },
            Unit = new BoundSlot<long>
            {
                Binding = BindingSelector.Current(
                    BindingDomain.Frame,
                    BindingProjection.Unit),
                Transform = BindingTransform.OppositeOrientation
            },
            Constraints =
            [
                new BindingConstraint(
                    "unit",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("accumulator"),
                        BindingProjection.Unit),
                    BindingTransform.Identity)
            ]
        };

        Assert.Equal(25, template.Value.Literal);
        Assert.Null(template.Unit.Literal);
        Assert.NotNull(template.Unit.Binding);
        Assert.Equal(BindingDomain.Frame, template.Unit.Binding!.Domain);
        Assert.Equal(BindingProjection.Unit, template.Unit.Binding.Projection);
        Assert.Equal(BindingTransform.OppositeOrientation, template.Unit.Transform);
        Assert.Single(template.Constraints);
    }

    [Fact]
    public void OperationAttachment_CanDescribeSiteInputsAndOutputs()
    {
        var attachment = new OperationAttachment(
            new OperationSite(
                OperationSiteKind.Carrier,
                "accumulate",
                new BindingAddress.Normalized(0.5m)),
            new OperationLawReference("Add"),
            [
                new OperationInputBinding(
                    "left",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("accumulator"),
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "right",
                    new BindingSelector(
                        BindingDomain.Family,
                        new BindingAddress.Current(),
                        BindingProjection.Whole,
                        new BindingStorageTarget(BindingDomain.Context, "currentItem")))
            ],
            [
                new OperationOutputBinding(
                    "sum",
                    new BindingStorageTarget(BindingDomain.Token, "accumulator"),
                    BindingTransform.Identity)
            ]);

        Assert.Equal(OperationSiteKind.Carrier, attachment.Site.Kind);
        Assert.Equal("accumulate", attachment.Site.Name);
        Assert.Equal("Add", attachment.Law.Name);
        Assert.Equal(2, attachment.Inputs.Count);
        Assert.Single(attachment.Outputs);
        Assert.Equal(BindingDomain.Token, attachment.Outputs[0].Target.Domain);
    }

    [Fact]
    public void BindingSignal_CanComposeAxisLikeTransforms()
    {
        Assert.True(BindingSignal.Orthogonal.TryCompose(BindingSignal.Negate, out var composed));

        Assert.NotNull(composed);
        Assert.Equal(BindingSignal.OppositeOrthogonal.Value, composed!.Value);
    }

    [Fact]
    public void BindingSchema_CanDescribeAccumulatorLoopMachine()
    {
        var accumulate = new OperationAttachment(
            new OperationSite(
                OperationSiteKind.Carrier,
                "accumulate",
                new BindingAddress.Normalized(0.5m)),
            new OperationLawReference("Add"),
            [
                new OperationInputBinding(
                    "accumulator",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("accumulator"),
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "currentItem",
                    new BindingSelector(
                        BindingDomain.Family,
                        new BindingAddress.Current(),
                        BindingProjection.Whole))
            ],
            [
                new OperationOutputBinding(
                    "sum",
                    new BindingStorageTarget(BindingDomain.Token, "accumulator"),
                    BindingTransform.Identity)
            ]);

        var continueLoop = new OperationAttachment(
            new OperationSite(OperationSiteKind.Boundary, "continue"),
            new OperationLawReference("ContinueWhileMoreMembers"),
            [
                new OperationInputBinding(
                    "position",
                    new BindingSelector(
                        BindingDomain.Context,
                        new BindingAddress.Name("memberIndex"),
                        BindingProjection.Whole))
            ],
            [
                new OperationOutputBinding(
                    "route",
                    new BindingStorageTarget(BindingDomain.Result, "route"),
                    BindingTransform.Identity)
            ]);

        Assert.Equal("accumulate", accumulate.Site.Name);
        Assert.Equal("ContinueWhileMoreMembers", continueLoop.Law.Name);
        Assert.Equal(OperationSiteKind.Boundary, continueLoop.Site.Kind);
    }

    [Fact]
    public void BindingSchema_CanDescribeFibonacciRegisterUpdate()
    {
        var fibonacci = new OperationAttachment(
            new OperationSite(OperationSiteKind.Carrier, "fib-step"),
            new OperationLawReference("FibonacciStep"),
            [
                new OperationInputBinding(
                    "a",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("a"),
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "b",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("b"),
                        BindingProjection.Whole))
            ],
            [
                new OperationOutputBinding(
                    "nextA",
                    new BindingStorageTarget(BindingDomain.Token, "a"),
                    BindingTransform.Identity),
                new OperationOutputBinding(
                    "nextB",
                    new BindingStorageTarget(BindingDomain.Token, "b"),
                    BindingTransform.Identity)
            ]);

        Assert.Equal("FibonacciStep", fibonacci.Law.Name);
        Assert.Equal(2, fibonacci.Inputs.Count);
        Assert.Equal(2, fibonacci.Outputs.Count);
        Assert.Equal("a", fibonacci.Outputs[0].Target.Name);
        Assert.Equal("b", fibonacci.Outputs[1].Target.Name);
    }
}
