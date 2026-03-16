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
                Axis.FromCoordinates(Scalar.Zero, 1),
                BoundaryContinuationLaw.ReflectiveBounce,
                PlanarOffset.Right,
                Scalar.One),
            new PlanarSegmentDefinition(
                "Y0",
                Axis.FromCoordinates(Scalar.Zero, 2),
                BoundaryContinuationLaw.ReflectiveBounce,
                PlanarOffset.Up,
                Scalar.One),
            new PlanarSegmentDefinition(
                "X1",
                Axis.FromCoordinates(Scalar.Zero, 4),
                BoundaryContinuationLaw.TensionPreserving,
                PlanarOffset.Right,
                new Scalar(4m),
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
            Axis.FromCoordinates(-1, 1),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            Scalar.One);

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
            Axis.FromCoordinates(-1, 1),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            Scalar.One);

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
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.ReflectiveBounce,
            PlanarOffset.Right,
            new Scalar(3m));

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
}
