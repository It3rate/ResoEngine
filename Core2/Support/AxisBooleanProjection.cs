using Core2.Elements;

namespace Core2.Support;

public enum AxisBooleanOperation
{
    False,
    True,
    TransferA,
    TransferB,
    And,
    Or,
    Nand,
    Nor,
    NotA,
    NotB,
    Implication,
    ReverseImplication,
    Inhibition,
    ReverseInhibition,
    Xor,
    Xnor,
}

public enum AxisBooleanCarrier
{
    Primary,
    Secondary,
    Frame,
}

public readonly record struct AxisBooleanPiece(
    Axis Segment,
    AxisBooleanCarrier Carrier,
    bool InPrimary,
    bool InSecondary);

public sealed record AxisBooleanResult(
    Axis Primary,
    Axis Secondary,
    Axis Frame,
    AxisBooleanOperation Operation,
    IReadOnlyList<AxisBooleanPiece> Pieces)
{
    public bool HasAny => Pieces.Count > 0;
    public IReadOnlyList<Axis> Segments => Pieces.Select(piece => piece.Segment).ToArray();
}

public static class AxisBooleanOperationExtensions
{
    public static bool Evaluate(this AxisBooleanOperation operation, bool inA, bool inB) =>
        operation switch
        {
            AxisBooleanOperation.False => false,
            AxisBooleanOperation.True => true,
            AxisBooleanOperation.TransferA => inA,
            AxisBooleanOperation.TransferB => inB,
            AxisBooleanOperation.And => inA && inB,
            AxisBooleanOperation.Or => inA || inB,
            AxisBooleanOperation.Nand => !(inA && inB),
            AxisBooleanOperation.Nor => !(inA || inB),
            AxisBooleanOperation.NotA => !inA,
            AxisBooleanOperation.NotB => !inB,
            AxisBooleanOperation.Implication => !inA || inB,
            AxisBooleanOperation.ReverseImplication => !inB || inA,
            AxisBooleanOperation.Inhibition => inA && !inB,
            AxisBooleanOperation.ReverseInhibition => !inA && inB,
            AxisBooleanOperation.Xor => inA ^ inB,
            AxisBooleanOperation.Xnor => !(inA ^ inB),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
        };
}

public static class AxisBooleanProjection
{
    public static AxisBooleanResult Resolve(
        Axis a,
        Axis b,
        AxisBooleanOperation operation,
        Axis? frame = null)
    {
        Axis actualFrame = frame ?? BuildImplicitFrame(a, b);
        decimal frameLeft = actualFrame.Left.Value;
        decimal frameRight = actualFrame.Right.Value;

        if (frameRight <= frameLeft)
        {
            return new AxisBooleanResult(a, b, actualFrame, operation, []);
        }

        var boundaries = CollectBoundaries(frameLeft, frameRight, a, b);

        var pieces = new List<AxisBooleanPiece>();
        decimal? currentLeft = null;
        decimal? currentRight = null;
        Axis? currentTemplate = null;
        AxisBooleanCarrier currentCarrier = default;
        bool currentInA = false;
        bool currentInB = false;

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
            if (!operation.Evaluate(inA, inB))
            {
                FlushCurrent();
                continue;
            }

            AxisBooleanCarrier carrier = SelectCarrier(inA, inB);
            Axis template = SelectTemplate(carrier, a, b, actualFrame);
            if (currentTemplate is not null &&
                currentRight == left &&
                currentTemplate.HasCompatibleCarrier(template))
            {
                currentRight = right;
                currentInA |= inA;
                currentInB |= inB;
                continue;
            }

            FlushCurrent();
            currentLeft = left;
            currentRight = right;
            currentTemplate = template;
            currentCarrier = carrier;
            currentInA = inA;
            currentInB = inB;
        }

        FlushCurrent();
        return new AxisBooleanResult(a, b, actualFrame, operation, pieces);

        void FlushCurrent()
        {
            if (currentLeft is null || currentRight is null || currentTemplate is null)
            {
                currentLeft = null;
                currentRight = null;
                currentTemplate = null;
                currentInA = false;
                currentInB = false;
                return;
            }

            pieces.Add(new AxisBooleanPiece(
                currentTemplate.WithBounds((Scalar)currentLeft.Value, (Scalar)currentRight.Value),
                currentCarrier,
                currentInA,
                currentInB));

            currentLeft = null;
            currentRight = null;
            currentTemplate = null;
            currentInA = false;
            currentInB = false;
        }
    }

    public static IReadOnlyList<AxisBooleanPiece> Project(
        Axis a,
        Axis b,
        AxisBooleanOperation operation,
        Axis? frame = null) =>
        Resolve(a, b, operation, frame).Pieces;

    private static Axis BuildImplicitFrame(Axis a, Axis b)
    {
        Axis template = a.HasExtent ? a : b;
        Scalar left = a.Left.Value <= b.Left.Value ? a.Left : b.Left;
        Scalar right = a.Right.Value >= b.Right.Value ? a.Right : b.Right;
        return template.WithBounds(left, right);
    }

    private static SortedSet<decimal> CollectBoundaries(decimal frameLeft, decimal frameRight, Axis a, Axis b)
    {
        var boundaries = new SortedSet<decimal> { frameLeft, frameRight };
        AddIfWithin(boundaries, a.Left.Value, frameLeft, frameRight);
        AddIfWithin(boundaries, a.Right.Value, frameLeft, frameRight);
        AddIfWithin(boundaries, b.Left.Value, frameLeft, frameRight);
        AddIfWithin(boundaries, b.Right.Value, frameLeft, frameRight);
        return boundaries;
    }

    private static void AddIfWithin(SortedSet<decimal> boundaries, decimal value, decimal frameLeft, decimal frameRight)
    {
        if (value > frameLeft && value < frameRight)
        {
            boundaries.Add(value);
        }
    }

    private static AxisBooleanCarrier SelectCarrier(bool inA, bool inB)
    {
        if (inA)
        {
            return AxisBooleanCarrier.Primary;
        }

        if (inB)
        {
            return AxisBooleanCarrier.Secondary;
        }

        return AxisBooleanCarrier.Frame;
    }

    private static Axis SelectTemplate(AxisBooleanCarrier carrier, Axis a, Axis b, Axis frame) =>
        carrier switch
        {
            AxisBooleanCarrier.Primary => a,
            AxisBooleanCarrier.Secondary => b,
            AxisBooleanCarrier.Frame => frame,
            _ => throw new ArgumentOutOfRangeException(nameof(carrier), carrier, null),
        };

    private static bool IsWithin(decimal value, Axis axis)
    {
        decimal left = axis.Left.Value;
        decimal right = axis.Right.Value;
        return value > left && value < right;
    }
}
