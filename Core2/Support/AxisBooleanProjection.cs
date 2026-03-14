using Core2.Elements;

namespace Core2.Support;

public enum AxisBooleanOperation
{
    And,
    Or,
    Nand,
    Nor,
    NotA,
    NotB,
    Xor,
    Xnor,
}

public readonly record struct AxisBooleanPiece(Axis Segment, Axis Source);

public static class AxisBooleanProjection
{
    public static IReadOnlyList<AxisBooleanPiece> Project(Axis a, Axis b, AxisBooleanOperation operation)
    {
        decimal frameLeft = Math.Min(Math.Min(a.Start.Value, a.End.Value), Math.Min(b.Start.Value, b.End.Value));
        decimal frameRight = Math.Max(Math.Max(a.Start.Value, a.End.Value), Math.Max(b.Start.Value, b.End.Value));

        if (frameRight <= frameLeft)
        {
            return [];
        }

        var boundaries = new SortedSet<decimal>
        {
            frameLeft,
            frameRight,
            a.Start.Value,
            a.End.Value,
            b.Start.Value,
            b.End.Value,
        };

        var pieces = new List<AxisBooleanPiece>();
        decimal? currentLeft = null;
        decimal? currentRight = null;
        Axis? currentSource = null;

        foreach (var pair in boundaries.Zip(boundaries.Skip(1)))
        {
            decimal left = pair.First;
            decimal right = pair.Second;
            if (right <= left)
            {
                continue;
            }

            decimal mid = (left + right) / 2m;
            bool inA = IsWithin(mid, a);
            bool inB = IsWithin(mid, b);
            if (!Evaluate(operation, inA, inB))
            {
                FlushCurrent();
                continue;
            }

            Axis source = SelectSource(operation, inA, inB, a, b);
            if (currentSource is not null &&
                currentRight == left &&
                HaveSameDirection(currentSource, source))
            {
                currentRight = right;
                continue;
            }

            FlushCurrent();
            currentLeft = left;
            currentRight = right;
            currentSource = source;
        }

        FlushCurrent();
        return pieces;

        void FlushCurrent()
        {
            if (currentLeft is null || currentRight is null || currentSource is null)
            {
                currentLeft = null;
                currentRight = null;
                currentSource = null;
                return;
            }

            pieces.Add(new AxisBooleanPiece(
                BuildDirectedPiece(currentLeft.Value, currentRight.Value, currentSource),
                currentSource));

            currentLeft = null;
            currentRight = null;
            currentSource = null;
        }
    }

    private static Axis BuildDirectedPiece(decimal left, decimal right, Axis source)
    {
        bool pointsRight = source.End.Value >= source.Start.Value;
        decimal start = pointsRight ? left : right;
        decimal end = pointsRight ? right : left;
        return Axis.FromCoordinates((Scalar)start, (Scalar)end);
    }

    private static bool HaveSameDirection(Axis left, Axis right) =>
        (left.End.Value >= left.Start.Value) == (right.End.Value >= right.Start.Value);

    private static Axis SelectSource(AxisBooleanOperation operation, bool inA, bool inB, Axis a, Axis b) =>
        operation switch
        {
            AxisBooleanOperation.And => a,
            AxisBooleanOperation.Or => inA ? a : b,
            AxisBooleanOperation.Nand => !inA ? a : b,
            AxisBooleanOperation.Nor => a,
            AxisBooleanOperation.NotA => a,
            AxisBooleanOperation.NotB => b,
            AxisBooleanOperation.Xor => (inA && !inB) ? a : b,
            AxisBooleanOperation.Xnor => a,
            _ => a,
        };

    private static bool Evaluate(AxisBooleanOperation operation, bool inA, bool inB) =>
        operation switch
        {
            AxisBooleanOperation.And => inA && inB,
            AxisBooleanOperation.Or => inA || inB,
            AxisBooleanOperation.Nand => !(inA && inB),
            AxisBooleanOperation.Nor => !(inA || inB),
            AxisBooleanOperation.NotA => !inA,
            AxisBooleanOperation.NotB => !inB,
            AxisBooleanOperation.Xor => inA ^ inB,
            AxisBooleanOperation.Xnor => !(inA ^ inB),
            _ => false,
        };

    private static bool IsWithin(decimal value, Axis axis)
    {
        decimal left = Math.Min(axis.Start.Value, axis.End.Value);
        decimal right = Math.Max(axis.Start.Value, axis.End.Value);
        return value >= left && value <= right;
    }
}
