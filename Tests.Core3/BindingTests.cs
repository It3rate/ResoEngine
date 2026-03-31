using Core3.Binding;
using Core3.Engine;
using Core3.Operations;

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
    public void TraversalRuntime_CanMaterializeLiteralRegistersIntoInitialTokenState()
    {
        var machine = CreateAccumulatorLoopMachine(endTick: 3);

        var state = TraversalRuntime.CreateInitial(machine);

        Assert.True(state.IsExact);
        Assert.Equal(machine.Mover, state.Mover);
        Assert.Equal(new AtomicElement(0, 1), Assert.IsType<AtomicElement>(state.Token["accumulator"]));
        Assert.Empty(state.Context);
        Assert.Empty(state.Result);
    }

    [Fact]
    public void TraversalRuntime_CanStepAccumulatorLoopAcrossFamilyMembers()
    {
        var machine = CreateAccumulatorLoopMachine(endTick: 3);
        var family = new Family(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(2, 1));
        family.AddMember(new AtomicElement(3, 1));

        var initial = TraversalRuntime.CreateInitial(machine);

        Assert.True(TraversalRuntime.TryStep(initial, family, out var firstStep));

        var first = Assert.IsType<TraversalStepResult>(firstStep);
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(first.State.Token["accumulator"]));
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(first.State.Context["currentItem"]));
        Assert.Equal(new AtomicElement(2, 1), Assert.IsType<AtomicElement>(first.State.Context["nextItem"]));
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(first.State.Result["route"]));
        Assert.Equal(new AtomicElement(1, 3), first.State.Mover.Position);
        Assert.Equal(2, first.Encounters.Count);
        Assert.Equal("Add", first.Encounters[0].Attachment.Law.Name);
        Assert.Equal("ContinueWhileNextMemberExists", first.Encounters[1].Attachment.Law.Name);
        Assert.Equal(2, first.Encounters[0].Outputs.Count);
        Assert.Equal(BindingDomain.Context, first.Encounters[0].Outputs[0].Target.Domain);
        Assert.Equal("currentItem", first.Encounters[0].Outputs[0].Target.Name);
        Assert.Equal(BindingDomain.Token, first.Encounters[0].Outputs[1].Target.Domain);
        Assert.Equal("accumulator", first.Encounters[0].Outputs[1].Target.Name);
        Assert.Equal(2, first.Encounters[1].Outputs.Count);
        Assert.Equal(BindingDomain.Context, first.Encounters[1].Outputs[0].Target.Domain);
        Assert.Equal("nextItem", first.Encounters[1].Outputs[0].Target.Name);
        Assert.Equal(BindingDomain.Result, first.Encounters[1].Outputs[1].Target.Domain);
        Assert.Equal("route", first.Encounters[1].Outputs[1].Target.Name);

        Assert.True(TraversalRuntime.TryStep(first.State, family, out var secondStep));

        var second = Assert.IsType<TraversalStepResult>(secondStep);
        Assert.Equal(new AtomicElement(3, 1), Assert.IsType<AtomicElement>(second.State.Token["accumulator"]));
        Assert.Equal(new AtomicElement(2, 1), Assert.IsType<AtomicElement>(second.State.Context["currentItem"]));
        Assert.Equal(new AtomicElement(3, 1), Assert.IsType<AtomicElement>(second.State.Context["nextItem"]));
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(second.State.Result["route"]));
        Assert.Equal(new AtomicElement(2, 3), second.State.Mover.Position);
    }

    [Fact]
    public void TraversalRuntime_PreservesUnsupportedLawAsNote()
    {
        var machine = new TraversalMachineDefinition(
            "unknown-law",
            "mystery",
            new TraversalMover("cursor", new AtomicElement(0, 1)),
            [],
            [
                new OperationAttachment(
                    new OperationSite(OperationSiteKind.Carrier, "mystery"),
                    new OperationLawReference("UnimplementedLaw"),
                    [],
                    [])
            ]);

        var initial = TraversalRuntime.CreateInitial(machine);

        Assert.True(TraversalRuntime.TryStep(initial, family: null, out var step));

        var result = Assert.IsType<TraversalStepResult>(step);
        Assert.Single(result.Encounters);
        Assert.Equal("UnimplementedLaw", result.Encounters[0].Attachment.Law.Name);
        Assert.Empty(result.Encounters[0].Outputs);
        Assert.Contains("no handler", result.State.Note);
        Assert.Equal(initial.Mover, result.State.Mover);
    }

    [Fact]
    public void TraversalRuntime_AllowsLaterAttachmentToReadContextWrittenEarlierInSameStep()
    {
        var machine = new TraversalMachineDefinition(
            "context-chain",
            "accumulate",
            new TraversalMover("family-cursor", new AtomicElement(0, 2)),
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
                            BindingSelector.Named(
                                BindingDomain.Context,
                                "currentItem",
                                BindingProjection.Whole))
                    ],
                    [
                        new OperationOutputBinding(
                            "route",
                            new BindingStorageTarget(BindingDomain.Result, "route"),
                            BindingTransform.Identity)
                    ])
            ]);
        var family = new Family(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(2, 1));

        var initial = TraversalRuntime.CreateInitial(machine);

        Assert.True(TraversalRuntime.TryStep(initial, family, out var step));

        var result = Assert.IsType<TraversalStepResult>(step);
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(result.State.Context["currentItem"]));
        Assert.Equal(new AtomicElement(1, 1), Assert.IsType<AtomicElement>(result.State.Result["route"]));
        Assert.Equal(new AtomicElement(1, 2), result.State.Mover.Position);
        Assert.Equal(
            new AtomicElement(1, 1),
            Assert.IsType<AtomicElement>(result.Encounters[1].Inputs["nextItem"]));
        Assert.Single(result.Encounters[1].Outputs);
        Assert.Equal(BindingDomain.Result, result.Encounters[1].Outputs[0].Target.Domain);
        Assert.Equal("route", result.Encounters[1].Outputs[0].Target.Name);
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

    private static TraversalMachineDefinition CreateAccumulatorLoopMachine(long endTick) =>
        new(
            "sum-loop",
            "accumulate",
            new TraversalMover("family-cursor", new AtomicElement(0, endTick)),
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
}

