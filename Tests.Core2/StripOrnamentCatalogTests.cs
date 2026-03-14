using Core2.Geometry;

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
            [StripDelta.Up, StripDelta.Right, StripDelta.Right, StripDelta.Down, StripDelta.Right, StripDelta.Right],
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
            [StripDelta.UpRight, StripDelta.DownRight, StripDelta.UpRight, StripDelta.DownRight],
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
                new StripDelta(3, 0),
                new StripDelta(0, 2),
                new StripDelta(2, 0),
                new StripDelta(0, -1),
                StripDelta.Right,
                new StripDelta(0, -1),
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
}
