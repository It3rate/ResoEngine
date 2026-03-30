using Core3.Binding;
using Core3.Engine;

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
                Binding = BindingSelector.At(
                    BindingDomain.Frame,
                    0,
                    projection: BindingProjection.Unit),
                Transform = BindingTransform.OppositeOrientation
            },
            Constraints =
            [
                new BindingConstraint(
                    "unit",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "accumulator",
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
                BindingAddress.At(1, 2)),
            new OperationLawReference("Add"),
            [
                new OperationInputBinding(
                    "left",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "accumulator",
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "right",
                    BindingSelector.At(
                        BindingDomain.Family,
                        0,
                        projection: BindingProjection.Whole,
                        storeTarget: new BindingStorageTarget(BindingDomain.Context, "currentItem")))
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
                BindingAddress.At(1, 2)),
            new OperationLawReference("Add"),
            [
                new OperationInputBinding(
                    "accumulator",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "accumulator",
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "currentItem",
                    BindingSelector.At(
                        BindingDomain.Family,
                        0,
                        projection: BindingProjection.Whole))
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
                    BindingSelector.Named(
                        BindingDomain.Context,
                        "memberIndex",
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
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "a",
                        BindingProjection.Whole)),
                new OperationInputBinding(
                    "b",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "b",
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

    [Fact]
    public void TraversalMachineDefinition_CanDescribeAccumulatorLoop()
    {
        var mover = new TraversalMover(
            "family-cursor",
            new AtomicElement(0, 4));

        var machine = new TraversalMachineDefinition(
            "sum-loop",
            "accumulate",
            mover,
            [
                new TraversalRegister(
                    "accumulator",
                    new BoundScalarTemplate
                    {
                        Value = new BoundSlot<long> { Literal = 0 },
                        Unit = new BoundSlot<long> { Literal = 1 }
                    })
            ],
            [
                new OperationAttachment(
                    new OperationSite(
                        OperationSiteKind.Carrier,
                        "accumulate",
                        BindingAddress.At(1, 2)),
                    new OperationLawReference("Add"),
                    [
                        new OperationInputBinding(
                            "accumulator",
                            BindingSelector.Named(
                                BindingDomain.Token,
                                "accumulator",
                                BindingProjection.Whole)),
                        new OperationInputBinding(
                            "currentItem",
                            BindingSelector.At(
                                BindingDomain.Family,
                                0,
                                projection: BindingProjection.Whole,
                                storeTarget: new BindingStorageTarget(BindingDomain.Context, "currentItem")))
                    ],
                    [
                        new OperationOutputBinding(
                            "sum",
                            new BindingStorageTarget(BindingDomain.Token, "accumulator"),
                            BindingTransform.Identity)
                    ]),
                new OperationAttachment(
                    new OperationSite(OperationSiteKind.Boundary, "continue"),
                    new OperationLawReference("ContinueWhileNextMemberExists"),
                    [
                        new OperationInputBinding(
                            "nextItem",
                            BindingSelector.At(
                                BindingDomain.Family,
                                1,
                                projection: BindingProjection.Whole,
                                storeTarget: new BindingStorageTarget(BindingDomain.Context, "nextItem")))
                    ],
                    [
                        new OperationOutputBinding(
                            "route",
                            new BindingStorageTarget(BindingDomain.Result, "route"),
                            BindingTransform.Identity)
                    ])
            ]);

        Assert.Equal("sum-loop", machine.Name);
        Assert.Equal("accumulate", machine.EntrySiteName);
        Assert.Equal("family-cursor", machine.Mover.Name);
        Assert.Equal(new AtomicElement(0, 4), machine.Mover.Position);
        Assert.Single(machine.Registers);
        Assert.Equal("accumulator", machine.Registers[0].Name);
        Assert.Equal(2, machine.Attachments.Count);
        Assert.Equal("Add", machine.Attachments[0].Law.Name);
        Assert.Equal("ContinueWhileNextMemberExists", machine.Attachments[1].Law.Name);
    }

    [Fact]
    public void TraversalMover_CanAdvanceOneTickAtATimeUntilDenominatorStop()
    {
        var mover = new TraversalMover(
            "quarter-cursor",
            new AtomicElement(0, 4));

        Assert.Equal(new AtomicElement(0, 4), mover.Position);
        Assert.Equal(0, mover.CurrentTick);
        Assert.Equal(4, mover.EndTick);
        Assert.False(mover.IsAtStop);

        Assert.True(mover.TryAdvance(out var first));
        Assert.NotNull(first);
        Assert.Equal(new AtomicElement(1, 4), first!.Position);
        Assert.Equal(1, first.CurrentTick);

        Assert.True(first.TryAdvance(out var second));
        Assert.NotNull(second);
        Assert.Equal(new AtomicElement(2, 4), second!.Position);
        Assert.Equal(2, second.CurrentTick);

        Assert.True(second.TryAdvance(out var third));
        Assert.NotNull(third);
        Assert.Equal(new AtomicElement(3, 4), third!.Position);
        Assert.Equal(3, third.CurrentTick);

        Assert.True(third.TryAdvance(out var fourth));
        Assert.NotNull(fourth);
        Assert.Equal(new AtomicElement(4, 4), fourth!.Position);
        Assert.Equal(4, fourth.CurrentTick);
        Assert.True(fourth.IsAtStop);
        Assert.False(fourth.TryAdvance(out _));
    }

    [Fact]
    public void TraversalMover_CanStartMidwayAndAdvanceByOneTick()
    {
        var mover = new TraversalMover(
            "offset-cursor",
            new AtomicElement(20, 100));

        Assert.True(mover.TryAdvance(out var first));
        Assert.NotNull(first);
        Assert.Equal(new AtomicElement(21, 100), first!.Position);
        Assert.Equal(21, first.CurrentTick);
        Assert.Equal(100, first.EndTick);
        Assert.False(first.IsAtStop);
    }

    [Fact]
    public void TraversalMover_StopsWhenNumeratorReachesDenominator()
    {
        var mover = new TraversalMover(
            "final-cursor",
            new AtomicElement(99, 100));

        Assert.True(mover.TryAdvance(out var final));
        Assert.NotNull(final);
        Assert.Equal(new AtomicElement(100, 100), final!.Position);
        Assert.True(final.IsAtStop);
        Assert.False(final.TryAdvance(out _));
    }
}
