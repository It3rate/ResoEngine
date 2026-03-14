namespace Core2.Geometry;

public static class StripOrnamentComposer
{
    public static StripOrnamentResult Compose(StripOrnamentPattern pattern, int repeats)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(repeats);

        var segments = new List<StripPathEdge>();
        var cursor = new StripPoint(0, 0);
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        for (int step = 0; step < pattern.TotalSteps(repeats); step++)
        {
            var delta = StripDelta.Zero;
            foreach (var strand in pattern.Strands)
            {
                delta += strand.DeltaAt(step);
            }

            if (delta.IsZero)
            {
                continue;
            }

            var next = cursor + delta;
            segments.Add(new StripPathEdge(cursor, next));
            cursor = next;

            minX = Math.Min(minX, cursor.X);
            maxX = Math.Max(maxX, cursor.X);
            minY = Math.Min(minY, cursor.Y);
            maxY = Math.Max(maxY, cursor.Y);
        }

        return new StripOrnamentResult(pattern, repeats, segments, cursor, minX, maxX, minY, maxY);
    }
}
