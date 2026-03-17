using Core2.Elements;
using Core2.Repetition;
using Applied.Geometry.Utils;
using Applied.Geometry.Frieze;

namespace Tests.Core2;

public class FriezeCatalogTests
{
    [Fact]
    public void GalleryPatterns_ExposeExpectedSet()
    {
        Assert.Equal(7, FriezeCatalog.GalleryPatterns.Count);
        Assert.Contains(FriezeCatalog.GalleryPatterns, pattern => pattern.Key == "square-wave");
        Assert.Contains(FriezeCatalog.GalleryPatterns, pattern => pattern.Key == "interlock");
    }

    [Fact]
    public void ComposedPatterns_AreContinuousAndNonEmpty()
    {
        foreach (var pattern in FriezeCatalog.GalleryPatterns)
        {
            var result = FriezeComposer.Compose(pattern, 4);

            Assert.NotEmpty(result.Segments);
            for (int index = 1; index < result.Segments.Count; index++)
            {
                Assert.Equal(result.Segments[index - 1].End, result.Segments[index].Start);
            }
        }
    }

    [Fact]
    public void SquareWave_ComposesFromSharedStrands()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "square-wave");
        var result = FriezeComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [new PlanarOffset(0, 2), new PlanarOffset(2, 0)],
            deltas);
    }

    [Fact]
    public void ZigZag_AlternatesDiagonalSegments()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "zigzag");
        var result = FriezeComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [new PlanarOffset(2, 2), new PlanarOffset(2, -2)],
            deltas);
    }

    [Fact]
    public void Crossbar_ComposesFromBouncedEquationCycle()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "crossbar");
        var result = FriezeComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                PlanarOffset.Right,
                PlanarOffset.Up,
                PlanarOffset.Left,
                PlanarOffset.Up,
                new PlanarOffset(2, 0),
                PlanarOffset.Right,
                PlanarOffset.Down,
                PlanarOffset.Left,
                PlanarOffset.Down,
                new PlanarOffset(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Trapezoid_ComposesFromPendingEquationCommits()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "trapezoid");
        var result = FriezeComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                PlanarOffset.Right,
                new PlanarOffset(-1, 2),
                new PlanarOffset(2, 0),
                PlanarOffset.Right,
                new PlanarOffset(-1, -2),
                new PlanarOffset(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Chevron_ComposesFromPairedEquationFires()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "chevron");
        var result = FriezeComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                new PlanarOffset(1, 1),
                new PlanarOffset(-1, 1),
                new PlanarOffset(2, 0),
                new PlanarOffset(1, -1),
                new PlanarOffset(-1, -1),
                new PlanarOffset(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Interlock_UsesPrimedContinuationOnShortEquation()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "interlock");
        var result = FriezeComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                PlanarOffset.Right,
                PlanarOffset.Up,
                PlanarOffset.Left,
                PlanarOffset.Up,
                PlanarOffset.Right,
                PlanarOffset.Down,
                new PlanarOffset(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Stair_AlternatesVerticalBounceWithLongCarrier()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "stair");
        var result = FriezeComposer.Compose(pattern, 4);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                PlanarOffset.Up,
                new PlanarOffset(2, 0),
                PlanarOffset.Up,
                new PlanarOffset(2, 0),
                PlanarOffset.Down,
                new PlanarOffset(2, 0),
                PlanarOffset.Down,
                new PlanarOffset(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void ComposeToWidth_UsesWholeCyclesUntilHorizontalSpanIsReached()
    {
        var pattern = FriezeCatalog.GalleryPatterns.Single(item => item.Key == "zigzag");
        var result = FriezeComposer.ComposeToWidth(pattern, minimumWidth: 5, maxSteps: 20);

        Assert.Equal(3, result.RepeatCount);
        Assert.True(result.HorizontalSpan >= 5);
    }

    [Fact]
    public void CreateGalleryPatterns_UsesProvidedSegmentDefinitions()
    {
        PlanarSegmentDefinition[] definitions =
        [
            new PlanarSegmentDefinition(
                "X0",
                Axis.FromCoordinates(Proportion.Zero, Proportion.One),
                BoundaryContinuationLaw.ReflectiveBounce,
                PlanarOffset.Right,
                Proportion.One),
            new PlanarSegmentDefinition(
                "Y0",
                Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
                BoundaryContinuationLaw.ReflectiveBounce,
                PlanarOffset.Up,
                Proportion.One),
            new PlanarSegmentDefinition(
                "X1",
                Axis.FromCoordinates(Proportion.Zero, new Proportion(4)),
                BoundaryContinuationLaw.TensionPreserving,
                PlanarOffset.Right,
                new Proportion(4),
                UseSegmentAsFrame: false),
        ];

        var pattern = FriezeCatalog.CreateGalleryPatterns(definitions).Single(item => item.Key == "crossbar");
        var result = FriezeComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                new PlanarOffset(1, 0),
                new PlanarOffset(0, 1),
                new PlanarOffset(-1, 0),
                new PlanarOffset(0, 1),
                new PlanarOffset(4, 0),
                new PlanarOffset(1, 0),
                new PlanarOffset(0, -1),
                new PlanarOffset(-1, 0),
                new PlanarOffset(0, -1),
                new PlanarOffset(4, 0),
            ],
            deltas);
    }

    [Fact]
    public void NonZeroSegmentStart_RepositionsWithoutDrawing()
    {
        var definition = new PlanarSegmentDefinition(
            "X0",
            Axis.FromCoordinates(new Proportion(-1), Proportion.One),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            Proportion.One);

        var pattern = new FriezePattern(
            "lead-in",
            "Lead In",
            "Test pattern for non-zero starts.",
            1,
            1,
            [new FriezeStrand("X0", definition.DescribeTraversal(), "Lead-in test.", [])])
        {
            Program = new EquationProgram(
                [definition],
                [
                    EquationCommand.Fire("X0"), EquationCommand.Commit(),
                ]),
        };

        var result = FriezeComposer.Compose(pattern, 1);

        Assert.Empty(result.Segments);
        Assert.Equal(new PlanarPoint(-1, 0), result.Cursor);
    }

    [Fact]
    public void NonZeroSegmentStart_DrawsOnlyAfterSilentLeadIn()
    {
        var definition = new PlanarSegmentDefinition(
            "X0",
            Axis.FromCoordinates(new Proportion(-1), Proportion.One),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            Proportion.One);

        var pattern = new FriezePattern(
            "lead-in",
            "Lead In",
            "Test pattern for non-zero starts.",
            3,
            1,
            [new FriezeStrand("X0", definition.DescribeTraversal(), "Lead-in test.", [])])
        {
            Program = new EquationProgram(
                [definition],
                [
                    EquationCommand.Fire("X0"), EquationCommand.Commit(),
                    EquationCommand.Fire("X0"), EquationCommand.Commit(),
                    EquationCommand.Fire("X0"), EquationCommand.Commit(),
                ]),
        };

        var result = FriezeComposer.Compose(pattern, 1);
        var deltas = result.Segments
            .Select(segment => new PlanarOffset(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                PlanarOffset.Right,
                PlanarOffset.Right,
            ],
            deltas);
    }

    [Fact]
    public void ReflectiveStep_LongerThanSpan_PreservesBounceVertex()
    {
        var definition = new PlanarSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            new Proportion(3));

        var pattern = new FriezePattern(
            "overshoot",
            "Overshoot",
            "Test pattern for reflected overshoot.",
            1,
            1,
            [new FriezeStrand("X1", definition.DescribeTraversal(), "Overshoot test.", [])])
        {
            Program = new EquationProgram(
                [definition],
                [
                    EquationCommand.Fire("X1"), EquationCommand.Commit(),
                ]),
        };

        var result = FriezeComposer.Compose(pattern, 1);

        Assert.Equal(
            [
                new PlanarPathEdge(new PlanarPoint(0, 0), new PlanarPoint(2, 0)),
                new PlanarPathEdge(new PlanarPoint(2, 0), new PlanarPoint(1, 0)),
            ],
            result.Segments);
        Assert.Equal(new PlanarPoint(1, 0), result.Cursor);
    }

    [Fact]
    public void PlanarSegmentDefinition_CanReframeSummarizedBoundaryPinsOntoRouteCarrier()
    {
        var sourceFrame = Axis.FromCoordinates(Proportion.Zero, new Proportion(5));
        var routeFrame = Axis.FromCoordinates(Proportion.Zero, new Proportion(2));
        var definition = new PlanarSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            BoundaryContinuationLaw.TensionPreserving,
            PlanarOffset.Right,
            new Proportion(3),
            BoundaryPins: BoundaryPinPair.FromLaw(sourceFrame, BoundaryContinuationLaw.ReflectiveBounce));

        var pins = definition.ResolveBoundaryPins(routeFrame);

        Assert.NotNull(pins);
        Assert.Equal(BoundaryContinuationLaw.ReflectiveBounce, pins!.SummaryLaw);
        Assert.Equal(routeFrame, pins.Frame);
    }

    [Fact]
    public void PlanarSegmentDefinition_CanReframeCustomBoundaryPinsOntoRouteCarrier()
    {
        var sourceFrame = Axis.FromCoordinates(Proportion.Zero, new Proportion(5));
        var routeFrame = Axis.FromCoordinates(Proportion.Zero, new Proportion(2));
        var customPins = BoundaryPinPair.Create(
            sourceFrame,
            new LocatedPin(Proportion.Zero, Axis.PinUnit, absorbs: true, name: "Left clamp"),
            new LocatedPin(
                new Proportion(5),
                Axis.PinUnit,
                [new PinEgress(new Proportion(5), -1, name: "Right reflect")],
                name: "Right reflect"));
        var definition = new PlanarSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            BoundaryContinuationLaw.TensionPreserving,
            PlanarOffset.Right,
            new Proportion(3),
            BoundaryPins: customPins);

        var pins = definition.ResolveBoundaryPins(routeFrame);

        Assert.NotNull(pins);
        Assert.Null(pins!.SummaryLaw);
        Assert.Equal(routeFrame.LeftCoordinate, pins.LeftPin!.Location);
        Assert.True(pins.LeftPin.Absorbs);
        Assert.Equal(routeFrame.RightCoordinate, pins.RightPin!.Location);
        Assert.Equal(routeFrame.RightCoordinate, pins.RightPin.PrimaryOutput!.Start);
        Assert.Equal(-1, pins.RightPin.PrimaryOutput.DirectionSign);
    }

    [Fact]
    public void PlanarSegmentRuntime_CanUseExplicitBoundaryPins()
    {
        var definition = new PlanarSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            BoundaryContinuationLaw.TensionPreserving,
            PlanarOffset.Right,
            new Proportion(3));
        var runtime = new PlanarSegmentRuntime(definition);
        var explicitReflect = BoundaryPinPair.Create(
            Axis.FromCoordinates(Proportion.Zero, new Proportion(2)),
            new LocatedPin(Proportion.Zero, Axis.PinUnit, [new PinEgress(Proportion.Zero, +1, name: "Reflect in")], name: "Left reflect"),
            new LocatedPin(new Proportion(2), Axis.PinUnit, [new PinEgress(new Proportion(2), -1, name: "Reflect in")], name: "Right reflect"));

        runtime.SetBoundaryPins(explicitReflect);
        var emission = runtime.Fire();

        Assert.Equal(
            [
                PlanarTraversalMotion.Visible(new PlanarOffset(2, 0), endsStroke: true),
                PlanarTraversalMotion.Visible(new PlanarOffset(-1, 0)),
            ],
            emission.Parts);
    }
}
