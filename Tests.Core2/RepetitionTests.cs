using Core2.Elements;
using Core2.Repetition;

namespace Tests.Core2;

public class RepetitionTests
{
    [Fact]
    public void AdditiveAndMultiplicativeRepetitionUseDifferentIdentityStates()
    {
        var additive = RepetitionEngine.RepeatAdditive(new Scalar(2), 4);
        var multiplicative = RepetitionEngine.RepeatMultiplicative(new Scalar(2), 4);

        Assert.Equal(RepetitionKind.Additive, additive.Kind);
        Assert.Equal(new Scalar(0), additive.States[0]);
        Assert.Equal(new Scalar(8), additive.Result);

        Assert.Equal(RepetitionKind.Multiplicative, multiplicative.Kind);
        Assert.Equal(new Scalar(1), multiplicative.States[0]);
        Assert.Equal(new Scalar(16), multiplicative.Result);
    }

    [Fact]
    public void TransformRepetition_TracesTheAxisOppositionCycle()
    {
        var trace = RepetitionEngine.RepeatTransform(Axis.One, Axis.I, 4);

        Assert.Equal(
            [Axis.One, Axis.I, Axis.NegativeOne, Axis.NegativeI, Axis.One],
            trace.States);
    }

    [Fact]
    public void RecursiveRepetition_CanExpressFibonacciStyleGeneration()
    {
        var trace = RepetitionEngine.RepeatRecursive(
            [new Scalar(0), new Scalar(1)],
            5,
            states => states[^1] + states[^2]);

        Assert.Equal(
            [new Scalar(0), new Scalar(1), new Scalar(1), new Scalar(2), new Scalar(3), new Scalar(5), new Scalar(8)],
            trace.States);
    }

    [Fact]
    public void BoundaryContinuation_CanWrapReflectOrPreserveTension()
    {
        var frame = Axis.FromCoordinates(0, 10);

        var wrapped = frame.Continue(new Proportion(11), BoundaryContinuationLaw.PeriodicWrap);
        var reflected = frame.Continue(new Proportion(11), BoundaryContinuationLaw.ReflectiveBounce);
        var clamped = frame.Continue(new Proportion(11), BoundaryContinuationLaw.Clamp);
        var preserved = frame.Continue(new Proportion(11), BoundaryContinuationLaw.TensionPreserving);

        Assert.Equal(new Proportion(1), wrapped.Value);
        Assert.Equal(new Proportion(9), reflected.Value);
        Assert.Equal(new Proportion(10), clamped.Value);
        Assert.Equal(new Proportion(11), preserved.Value);
        Assert.Contains(preserved.Tensions, tension => tension.Kind == RepetitionTensionKind.BoundaryExceeded);
    }

    [Fact]
    public void BoundaryContinuation_WrapsAcrossMultiplePeriods()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var wrappedPositive = frame.Continue(new Proportion(15), BoundaryContinuationLaw.PeriodicWrap);
        var wrappedNegative = frame.Continue(new Proportion(-2), BoundaryContinuationLaw.PeriodicWrap);

        Assert.Equal(Proportion.Zero, wrappedPositive.Value);
        Assert.Equal(new Proportion(3), wrappedNegative.Value);
    }

    [Fact]
    public void BoundaryContinuation_ReflectsAcrossMultipleBounces()
    {
        var frame = Axis.FromCoordinates(0, 5);

        var reflectedPositive = frame.Continue(new Proportion(15), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedOver = frame.Continue(new Proportion(17), BoundaryContinuationLaw.ReflectiveBounce);
        var reflectedNegative = frame.Continue(new Proportion(-2), BoundaryContinuationLaw.ReflectiveBounce);

        Assert.Equal(new Proportion(5), reflectedPositive.Value);
        Assert.Equal(new Proportion(3), reflectedOver.Value);
        Assert.Equal(new Proportion(2), reflectedNegative.Value);
    }

    [Fact]
    public void BoundaryContinuation_CanResolveThroughExplicitBoundaryPins()
    {
        var frame = Axis.FromCoordinates(0, 5);
        BoundaryPinPair wrapPins = BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.PeriodicWrap);
        BoundaryPinPair reflectPins = BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.ReflectiveBounce);
        BoundaryPinPair clampPins = BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.Clamp);

        Assert.Equal(
            frame.Continue(new Proportion(17), BoundaryContinuationLaw.PeriodicWrap).Value,
            frame.Continue(new Proportion(17), wrapPins).Value);
        Assert.Equal(
            frame.Continue(new Proportion(17), BoundaryContinuationLaw.ReflectiveBounce).Value,
            frame.Continue(new Proportion(17), reflectPins).Value);
        Assert.Equal(
            frame.Continue(new Proportion(17), BoundaryContinuationLaw.Clamp).Value,
            frame.Continue(new Proportion(17), clampPins).Value);
    }

    [Fact]
    public void BoundaryPinPair_CanBeCreatedDirectlyFromExplicitPins()
    {
        var frame = Axis.FromCoordinates(0, 5);
        var explicitWrap = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, [new PinEgress(new Proportion(5), -1, name: "Wrap to right")], name: "Left wrap"),
            new LocatedPin(new Proportion(5), Axis.PinUnit, [new PinEgress(Proportion.Zero, +1, name: "Wrap to left")], name: "Right wrap"));

        var result = frame.Continue(new Proportion(17), explicitWrap);

        Assert.Equal(frame.Continue(new Proportion(17), BoundaryContinuationLaw.PeriodicWrap).Value, result.Value);
        Assert.Equal(BoundaryContinuationLaw.PeriodicWrap, explicitWrap.SummaryLaw);
    }

    [Fact]
    public void BoundaryPinPair_SummaryLawRecognizesExplicitReflectAndClampPins()
    {
        var frame = Axis.FromCoordinates(0, 5);
        var reflect = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, [new PinEgress(Proportion.Zero, +1, name: "Reflect in")], name: "Left reflect"),
            new LocatedPin(new Proportion(5), Axis.PinUnit, [new PinEgress(new Proportion(5), -1, name: "Reflect in")], name: "Right reflect"));
        var clamp = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, absorbs: true, name: "Left clamp"),
            new LocatedPin(new Proportion(5), Axis.PinUnit, absorbs: true, name: "Right clamp"));

        Assert.Equal(BoundaryContinuationLaw.ReflectiveBounce, reflect.SummaryLaw);
        Assert.Equal(BoundaryContinuationLaw.Clamp, clamp.SummaryLaw);
    }

    [Fact]
    public void BoundaryPinPair_SummaryLawIsNullForCustomBoundaryPins()
    {
        var frame = Axis.FromCoordinates(0, 5);
        var custom = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, [new PinEgress(new Proportion(2), +1, name: "Short attach")], name: "Left custom"),
            null);

        Assert.Null(custom.SummaryLaw);
    }

    [Fact]
    public void BoundaryPinPair_SummaryLawRecognizesImplicitReflectiveBoundaryPins()
    {
        var frame = Axis.FromCoordinates(0, 5);
        var implicitReflect = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, name: "Implicit left"),
            new LocatedPin(new Proportion(5), Axis.PinUnit, name: "Implicit right"));

        Assert.Equal(BoundaryContinuationLaw.ReflectiveBounce, implicitReflect.SummaryLaw);
    }

    [Fact]
    public void LocatedPin_CanAttachItsAppliedAxisToAHostFrame()
    {
        var frame = Axis.FromCoordinates(new Proportion(-3), new Proportion(7));
        var applied = new Axis(new Proportion(3, -1), new Proportion(2, 1));
        var pin = new LocatedPin(new Proportion(4), applied, name: "Bent landmark");

        var pointPin = pin.AttachTo(frame);
        var placed = pin.PlaceApplied();

        Assert.Equal(frame, pointPin.Host);
        Assert.Equal(applied, pointPin.Applied);
        Assert.Equal(new Proportion(4), pointPin.Position);
        Assert.True(pin.IsHostRelativeTo(frame));
        Assert.Equal(new Proportion(4), placed.EmbeddedOrigin);
        Assert.Equal(1, placed.RecessiveSide.CarrierRank);
        Assert.Equal(0, placed.DominantSide.CarrierRank);
    }

    [Fact]
    public void AxisTraversalDefinition_CanExposePinsAsPointPinningAndPlacedAxes()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var first = new LocatedPin(new Proportion(3), new Axis(new Proportion(1), new Proportion(2)), name: "P1");
        var second = new LocatedPin(new Proportion(7), new Axis(new Proportion(1, -1), new Proportion(2)), name: "P2");
        var definition = new AxisTraversalDefinition(frame, Proportion.One, Pins: [first, second]);

        var pointPins = definition.ResolvePointPins();
        var placedAxes = definition.ResolvePlacedAppliedAxes();

        Assert.Equal(2, pointPins.Count);
        Assert.Equal(frame, pointPins[0].Host);
        Assert.Equal(new Proportion(3), pointPins[0].Position);
        Assert.Equal(new Proportion(7), pointPins[1].Position);
        Assert.Equal(2, placedAxes.Count);
        Assert.Equal(new Proportion(3), placedAxes[0].EmbeddedOrigin);
        Assert.Equal(new Proportion(7), placedAxes[1].EmbeddedOrigin);
        Assert.Equal(1, placedAxes[1].RecessiveSide.CarrierRank);
    }

    [Fact]
    public void BoundaryPinPair_CanExposeBoundaryPinsAsPointPinning()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(5));
        var pair = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, absorbs: true, name: "Left clamp"),
            new LocatedPin(new Proportion(5), Axis.PinUnit, [new PinEgress(new Proportion(5), -1, name: "Reflect in")], name: "Right reflect"));

        var pointPins = pair.ResolvePointPins();

        Assert.Equal(2, pointPins.Count);
        Assert.All(pointPins, pinning => Assert.Equal(frame, pinning.Host));
        Assert.Equal(Proportion.Zero, pointPins[0].Position);
        Assert.Equal(new Proportion(5), pointPins[1].Position);
    }

    [Fact]
    public void AxisTraversal_ImplicitSameDirectionLandmark_IsTransparent()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var pin = new LocatedPin(new Proportion(5), Axis.PinUnit, name: "Implicit continue");
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [pin]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), Axis.FromCoordinates(new Proportion(5), new Proportion(7))],
            parts.Select(part => part.Segment).ToArray());
        Assert.All(parts, part => Assert.Empty(part.Tensions));
        Assert.All(parts, part => Assert.False(part.BreakAfter));
        Assert.Equal(new Proportion(7), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_ImplicitSameDirectionLandmark_IsTransparentWhenApproachedFromRight()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var pin = new LocatedPin(new Proportion(5), Axis.PinUnit, name: "Implicit continue");
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(-7),
            Seed: new Proportion(10),
            Pins: [pin]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(new Proportion(10), new Proportion(5)), Axis.FromCoordinates(new Proportion(5), new Proportion(3))],
            parts.Select(part => part.Segment).ToArray());
        Assert.Contains(parts[0].Tensions, tension => tension.Kind == RepetitionTensionKind.PinFlowBlocked);
        Assert.Empty(parts[1].Tensions);
        Assert.All(parts, part => Assert.False(part.BreakAfter));
        Assert.Equal(new Proportion(3), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_ImplicitOrthogonalLandmark_IsDeferredAsTension()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var pin = new LocatedPin(new Proportion(5), new Axis(new Proportion(1), new Proportion(2, -1)), name: "Implicit bent");
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [pin]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Single(parts);
        Assert.Equal(Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), parts[0].Segment);
        Assert.Contains(parts[0].Tensions, tension => tension.Kind == RepetitionTensionKind.PinBehaviorDeferred);
        Assert.Equal(new Proportion(5), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_ImplicitTransparentLandmark_CanBlockFarSideContinuation()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var pin = new LocatedPin(new Proportion(5), new Axis(new Proportion(-1, 1), new Proportion(1, 1)), name: "Implicit block");
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [pin]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Single(parts);
        Assert.Equal(Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), parts[0].Segment);
        Assert.Contains(parts[0].Tensions, tension => tension.Kind == RepetitionTensionKind.PinFlowBlocked);
        Assert.Equal(new Proportion(5), traversal.Value);
    }

    [Fact]
    public void BoundaryContinuation_CanDeriveLeftBoundaryReflectionFromImplicitPinDescriptor()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(5));
        var implicitLeft = BoundaryPinPair.Create(
            frame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, name: "Implicit left"),
            null);

        var result = frame.Continue(new Proportion(-2), implicitLeft);

        Assert.Equal(new Proportion(2), result.Value);
    }

    [Fact]
    public void BoundaryContinuation_CanDeriveRightBoundaryReflectionFromImplicitPinDescriptor()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(5));
        var implicitRight = BoundaryPinPair.Create(
            frame,
            null,
            new LocatedPin(new Proportion(5), Axis.PinUnit, name: "Implicit right"));

        var result = frame.Continue(new Proportion(7), implicitRight);

        Assert.Equal(new Proportion(3), result.Value);
    }

    [Fact]
    public void AxisTraversal_EnumeratesReflectionAndWrapAsExactStepSequences()
    {
        var reflective = new AxisTraversalDefinition(
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            new Proportion(3),
            BoundaryContinuationLaw.ReflectiveBounce,
            Proportion.Zero).CreateState();

        var reflectiveParts = reflective.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(new Proportion(2), Proportion.One)],
            reflectiveParts.Select(part => part.Segment).ToArray());
        Assert.True(reflectiveParts[0].BreakAfter);
        Assert.Equal(new Proportion(1), reflective.Value);

        var wrapped = new AxisTraversalDefinition(
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            new Proportion(5),
            BoundaryContinuationLaw.PeriodicWrap,
            Proportion.Zero).CreateState();

        var wrappedParts = wrapped.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, Proportion.One)],
            wrappedParts.Select(part => part.Segment).ToArray());
        Assert.Equal(new Proportion(1), wrapped.Value);
    }

    [Fact]
    public void AxisTraversal_CanUseExplicitBoundaryPins()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(2));

        var reflective = new AxisTraversalDefinition(
            frame,
            new Proportion(3),
            Seed: Proportion.Zero,
            BoundaryPins: BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.ReflectiveBounce)).CreateState();

        var reflectiveParts = reflective.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(new Proportion(2), Proportion.One)],
            reflectiveParts.Select(part => part.Segment).ToArray());
        Assert.True(reflectiveParts[0].BreakAfter);

        var wrapped = new AxisTraversalDefinition(
            frame,
            new Proportion(5),
            Seed: Proportion.Zero,
            BoundaryPins: BoundaryPinPair.FromLaw(frame, BoundaryContinuationLaw.PeriodicWrap)).CreateState();

        var wrappedParts = wrapped.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, new Proportion(2)), Axis.FromCoordinates(Proportion.Zero, Proportion.One)],
            wrappedParts.Select(part => part.Segment).ToArray());
        Assert.Equal(new Proportion(1), wrapped.Value);
    }

    [Fact]
    public void AxisTraversal_WithoutBoundaryPinsFallsOpenIntoTensionAndThenContinuesUnbounded()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(2));
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(3),
            BoundaryContinuationLaw.TensionPreserving,
            Proportion.Zero).CreateState();

        var first = traversal.Fire();
        var second = traversal.Fire();

        Assert.Equal(Axis.FromCoordinates(Proportion.Zero, new Proportion(3)), first.Segment);
        Assert.Contains(first.Tensions, tension => tension.Kind == RepetitionTensionKind.BoundaryExceeded);
        Assert.Equal(Axis.FromCoordinates(new Proportion(3), new Proportion(6)), second.Segment);
    }

    [Fact]
    public void AxisTraversal_CanReflectFromAnInteriorPin()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var landmark = new LocatedPin(
            new Proportion(5),
            Axis.PinUnit,
            [new PinEgress(new Proportion(5), -1, name: "Turn back")],
            name: "Interior reflect");

        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [landmark]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), Axis.FromCoordinates(new Proportion(5), new Proportion(3))],
            parts.Select(part => part.Segment).ToArray());
        Assert.True(parts[0].BreakAfter);
        Assert.Equal(new Proportion(3), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_CanAbsorbAtAnInteriorPin()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var clamp = new LocatedPin(new Proportion(5), Axis.PinUnit, absorbs: true, name: "Interior clamp");

        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [clamp]).CreateState();

        var step = traversal.Fire();

        Assert.Equal(Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), step.Segment);
        Assert.Equal(new Proportion(5), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_CanShortCircuitToAnotherCarrierPositionFromInteriorPin()
    {
        var frame = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));
        var pin = new LocatedPin(
            new Proportion(5),
            Axis.PinUnit,
            [new PinEgress(new Proportion(8), -1, name: "Attach elsewhere")],
            name: "Interior attach");

        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(7),
            Seed: Proportion.Zero,
            Pins: [pin]).CreateState();

        var parts = traversal.EnumerateFire().ToArray();

        Assert.Equal(
            [Axis.FromCoordinates(Proportion.Zero, new Proportion(5)), Axis.FromCoordinates(new Proportion(8), new Proportion(6))],
            parts.Select(part => part.Segment).ToArray());
        Assert.True(parts[0].BreakAfter);
        Assert.Equal(new Proportion(6), traversal.Value);
    }

    [Fact]
    public void AxisTraversal_DegenerateReflectiveFrame_StopsInsteadOfLooping()
    {
        var frame = Axis.FromCoordinates(new Proportion(5), new Proportion(5));
        var traversal = new AxisTraversalDefinition(
            frame,
            new Proportion(2),
            BoundaryContinuationLaw.ReflectiveBounce,
            new Proportion(5)).CreateState();

        var step = traversal.Fire();

        Assert.True(step.IsDegenerate);
        Assert.Equal(new Proportion(5), traversal.Value);
        Assert.Contains(step.Tensions, tension => tension.Kind == RepetitionTensionKind.PinStalled);
    }
}
