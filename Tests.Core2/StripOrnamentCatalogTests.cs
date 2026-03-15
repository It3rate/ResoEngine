using Core2.Geometry;
using Core2.Elements;
using Core2.Repetition;

namespace Tests.Core2;

public class StripOrnamentCatalogTests
{
    [Fact]
    public void GalleryPatterns_ExposeExpectedSet()
    {
        Assert.Equal(7, StripOrnamentCatalog.GalleryPatterns.Count);
        Assert.Contains(StripOrnamentCatalog.GalleryPatterns, pattern => pattern.Key == "square-wave");
        Assert.Contains(StripOrnamentCatalog.GalleryPatterns, pattern => pattern.Key == "interlock");
    }

    [Fact]
    public void ComposedPatterns_AreContinuousAndNonEmpty()
    {
        foreach (var pattern in StripOrnamentCatalog.GalleryPatterns)
        {
            var result = StripOrnamentComposer.Compose(pattern, 4);

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
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "square-wave");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [new StripDelta(0, 2), new StripDelta(2, 0)],
            deltas);
    }

    [Fact]
    public void ZigZag_AlternatesDiagonalSegments()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "zigzag");
        var result = StripOrnamentComposer.Compose(pattern, 2);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [new StripDelta(2, 2), new StripDelta(2, -2)],
            deltas);
    }

    [Fact]
    public void Crossbar_ComposesFromBouncedEquationCycle()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "crossbar");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                StripDelta.Right,
                StripDelta.Up,
                StripDelta.Left,
                StripDelta.Up,
                new StripDelta(2, 0),
                StripDelta.Right,
                StripDelta.Down,
                StripDelta.Left,
                StripDelta.Down,
                new StripDelta(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Trapezoid_ComposesFromPendingEquationCommits()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "trapezoid");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                StripDelta.Right,
                new StripDelta(-1, 2),
                new StripDelta(2, 0),
                StripDelta.Right,
                new StripDelta(-1, -2),
                new StripDelta(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Chevron_ComposesFromPairedEquationFires()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "chevron");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                new StripDelta(1, 1),
                new StripDelta(-1, 1),
                new StripDelta(2, 0),
                new StripDelta(1, -1),
                new StripDelta(-1, -1),
                new StripDelta(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Interlock_UsesPrimedContinuationOnShortEquation()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "interlock");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                StripDelta.Right,
                StripDelta.Up,
                StripDelta.Left,
                StripDelta.Up,
                StripDelta.Right,
                StripDelta.Down,
                new StripDelta(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void Stair_AlternatesVerticalBounceWithLongCarrier()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "stair");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                StripDelta.Up,
                new StripDelta(2, 0),
                StripDelta.Up,
                new StripDelta(2, 0),
                StripDelta.Down,
                new StripDelta(2, 0),
                StripDelta.Down,
                new StripDelta(2, 0),
            ],
            deltas);
    }

    [Fact]
    public void ComposeToWidth_UsesWholeCyclesUntilHorizontalSpanIsReached()
    {
        var pattern = StripOrnamentCatalog.GalleryPatterns.Single(item => item.Key == "zigzag");
        var result = StripOrnamentComposer.ComposeToWidth(pattern, minimumWidth: 5, maxSteps: 20);

        Assert.Equal(3, result.RepeatCount);
        Assert.True(result.HorizontalSpan >= 5);
    }

    [Fact]
    public void CreateGalleryPatterns_UsesProvidedSegmentDefinitions()
    {
        StripSegmentDefinition[] definitions =
        [
            new StripSegmentDefinition(
                "X0",
                Axis.FromCoordinates(Scalar.Zero, 1),
                BoundaryContinuationLaw.ReflectiveBounce,
                StripDelta.Right,
                Scalar.One),
            new StripSegmentDefinition(
                "Y0",
                Axis.FromCoordinates(Scalar.Zero, 2),
                BoundaryContinuationLaw.ReflectiveBounce,
                StripDelta.Up,
                Scalar.One),
            new StripSegmentDefinition(
                "X1",
                Axis.FromCoordinates(Scalar.Zero, 4),
                BoundaryContinuationLaw.TensionPreserving,
                StripDelta.Right,
                new Scalar(4m),
                UseSegmentAsFrame: false),
        ];

        var pattern = StripOrnamentCatalog.CreateGalleryPatterns(definitions).Single(item => item.Key == "crossbar");
        var result = StripOrnamentComposer.Compose(pattern, 1);

        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                new StripDelta(1, 0),
                new StripDelta(0, 1),
                new StripDelta(-1, 0),
                new StripDelta(0, 1),
                new StripDelta(4, 0),
                new StripDelta(1, 0),
                new StripDelta(0, -1),
                new StripDelta(-1, 0),
                new StripDelta(0, -1),
                new StripDelta(4, 0),
            ],
            deltas);
    }

    [Fact]
    public void NonZeroSegmentStart_RepositionsWithoutDrawing()
    {
        var definition = new StripSegmentDefinition(
            "X0",
            Axis.FromCoordinates(-1, 1),
            BoundaryContinuationLaw.ReflectiveBounce,
            StripDelta.Right,
            Scalar.One);

        var pattern = new StripOrnamentPattern(
            "lead-in",
            "Lead In",
            "Test pattern for non-zero starts.",
            1,
            1,
            [new StripOrnamentStrand("X0", definition.DescribeTraversal(), "Lead-in test.", [])])
        {
            Program = new StripEquationProgram(
                [definition],
                [
                    StripEquationCommand.Fire("X0"), StripEquationCommand.Commit(),
                ]),
        };

        var result = StripOrnamentComposer.Compose(pattern, 1);

        Assert.Empty(result.Segments);
        Assert.Equal(new StripPoint(-1, 0), result.Cursor);
    }

    [Fact]
    public void NonZeroSegmentStart_DrawsOnlyAfterSilentLeadIn()
    {
        var definition = new StripSegmentDefinition(
            "X0",
            Axis.FromCoordinates(-1, 1),
            BoundaryContinuationLaw.ReflectiveBounce,
            StripDelta.Right,
            Scalar.One);

        var pattern = new StripOrnamentPattern(
            "lead-in",
            "Lead In",
            "Test pattern for non-zero starts.",
            3,
            1,
            [new StripOrnamentStrand("X0", definition.DescribeTraversal(), "Lead-in test.", [])])
        {
            Program = new StripEquationProgram(
                [definition],
                [
                    StripEquationCommand.Fire("X0"), StripEquationCommand.Commit(),
                    StripEquationCommand.Fire("X0"), StripEquationCommand.Commit(),
                    StripEquationCommand.Fire("X0"), StripEquationCommand.Commit(),
                ]),
        };

        var result = StripOrnamentComposer.Compose(pattern, 1);
        var deltas = result.Segments
            .Select(segment => new StripDelta(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y))
            .ToArray();

        Assert.Equal(
            [
                StripDelta.Right,
                StripDelta.Right,
            ],
            deltas);
    }

    [Fact]
    public void ReflectiveStep_LongerThanSpan_PreservesBounceVertex()
    {
        var definition = new StripSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.ReflectiveBounce,
            StripDelta.Right,
            new Scalar(3m));

        var pattern = new StripOrnamentPattern(
            "overshoot",
            "Overshoot",
            "Test pattern for reflected overshoot.",
            1,
            1,
            [new StripOrnamentStrand("X1", definition.DescribeTraversal(), "Overshoot test.", [])])
        {
            Program = new StripEquationProgram(
                [definition],
                [
                    StripEquationCommand.Fire("X1"), StripEquationCommand.Commit(),
                ]),
        };

        var result = StripOrnamentComposer.Compose(pattern, 1);

        Assert.Equal(
            [
                new StripPathEdge(new StripPoint(0, 0), new StripPoint(2, 0)),
                new StripPathEdge(new StripPoint(2, 0), new StripPoint(1, 0)),
            ],
            result.Segments);
        Assert.Equal(new StripPoint(1, 0), result.Cursor);
    }
}
