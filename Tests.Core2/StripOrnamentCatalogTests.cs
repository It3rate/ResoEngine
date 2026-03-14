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
}
