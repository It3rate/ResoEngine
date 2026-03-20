using Core2.Branching;
using Core2.Algebra;
using Core2.Elements;
using Core2.Boolean;

namespace Tests.Core2;

public class AxisBooleanProjectionTests
{
    public static IEnumerable<object[]> TruthTableCases()
    {
        yield return [AxisBooleanOperation.False, Array.Empty<(decimal Start, decimal End)>()];
        yield return [AxisBooleanOperation.True, new[] { ((decimal)(-1m), (decimal)4m) }];
        yield return [AxisBooleanOperation.TransferA, new[] { ((decimal)0m, (decimal)2m) }];
        yield return [AxisBooleanOperation.TransferB, new[] { ((decimal)1m, (decimal)3m) }];
        yield return [AxisBooleanOperation.NotA, new[] { ((decimal)(-1m), (decimal)0m), ((decimal)2m, (decimal)4m) }];
        yield return [AxisBooleanOperation.NotB, new[] { ((decimal)(-1m), (decimal)1m), ((decimal)3m, (decimal)4m) }];
        yield return [AxisBooleanOperation.And, new[] { ((decimal)1m, (decimal)2m) }];
        yield return [AxisBooleanOperation.Nand, new[] { ((decimal)(-1m), (decimal)1m), ((decimal)2m, (decimal)4m) }];
        yield return [AxisBooleanOperation.Or, new[] { ((decimal)0m, (decimal)3m) }];
        yield return [AxisBooleanOperation.Nor, new[] { ((decimal)(-1m), (decimal)0m), ((decimal)3m, (decimal)4m) }];
        yield return [AxisBooleanOperation.Implication, new[] { ((decimal)(-1m), (decimal)0m), ((decimal)1m, (decimal)4m) }];
        yield return [AxisBooleanOperation.ReverseImplication, new[] { ((decimal)(-1m), (decimal)2m), ((decimal)3m, (decimal)4m) }];
        yield return [AxisBooleanOperation.Inhibition, new[] { ((decimal)0m, (decimal)1m) }];
        yield return [AxisBooleanOperation.ReverseInhibition, new[] { ((decimal)2m, (decimal)3m) }];
        yield return [AxisBooleanOperation.Xor, new[] { ((decimal)0m, (decimal)1m), ((decimal)2m, (decimal)3m) }];
        yield return [AxisBooleanOperation.Xnor, new[] { ((decimal)(-1m), (decimal)0m), ((decimal)1m, (decimal)2m), ((decimal)3m, (decimal)4m) }];
    }

    [Fact]
    public void Or_UsesPrimaryDirection_WhenOverlapMakesTruthAmbiguous()
    {
        var a = Axis.FromCoordinates((Scalar)0m, (Scalar)3m);
        var b = Axis.FromCoordinates((Scalar)1m, (Scalar)5m);

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Or);

        var piece = Assert.Single(pieces);
        Assert.Equal(AxisBooleanCarrier.Primary, piece.Carrier);
        Assert.True(piece.Segment.PointsRight);
        Assert.Equal(0m, piece.Segment.Start.Value);
        Assert.Equal(5m, piece.Segment.End.Value);
    }

    [Fact]
    public void Xor_PreservesSourceDirection_PerPiece()
    {
        var a = Axis.FromCoordinates((Scalar)5m, (Scalar)(-1m));
        var b = Axis.FromCoordinates((Scalar)2m, (Scalar)(-4m));

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Xor);

        Assert.Equal(2, pieces.Count);
        Assert.All(pieces, piece => Assert.False(piece.Segment.PointsRight));
        Assert.Equal(-1m, pieces[0].Segment.Start.Value);
        Assert.Equal(-4m, pieces[0].Segment.End.Value);
        Assert.Equal(5m, pieces[1].Segment.Start.Value);
        Assert.Equal(2m, pieces[1].Segment.End.Value);
    }

    [Fact]
    public void Xor_SplitsAMiddleSegmentOutOfALongerSegment()
    {
        var middle = Axis.FromCoordinates((Scalar)3m, (Scalar)5m);
        var frameCarrier = Axis.FromCoordinates((Scalar)0m, (Scalar)10m);

        var result = AxisBooleanProjection.Resolve(middle, frameCarrier, AxisBooleanOperation.Xor);

        Assert.Equal(2, result.Pieces.Count);
        Assert.Equal(0m, result.Pieces[0].Segment.Start.Value);
        Assert.Equal(3m, result.Pieces[0].Segment.End.Value);
        Assert.Equal(5m, result.Pieces[1].Segment.Start.Value);
        Assert.Equal(10m, result.Pieces[1].Segment.End.Value);
        Assert.All(result.Pieces, piece => Assert.Equal(AxisBooleanCarrier.Secondary, piece.Carrier));
        Assert.Equal(BranchOrigin.Component, result.Branches.Origin);
        Assert.Equal(BranchSemantics.CoPresent, result.Branches.Semantics);
        Assert.Equal(BranchDirection.Structural, result.Branches.Direction);
        Assert.False(result.Branches.Selection.HasSelection);
        Assert.All(result.Branches.Members, member =>
        {
            Assert.True(member.TryGetAnnotation<AxisBooleanPieceAnnotation>(out var annotation));
            Assert.Equal(AxisBooleanCarrier.Secondary, annotation.Carrier);
        });
    }

    [Fact]
    public void ExplicitFrame_AllowsComplementOutsideBothInputs_AndUsesFrameDirection()
    {
        var a = Axis.FromCoordinates((Scalar)8m, (Scalar)6m);
        var b = Axis.FromCoordinates((Scalar)3m, (Scalar)2m);
        var frame = Axis.FromCoordinates((Scalar)10m, (Scalar)0m);

        var result = AxisBooleanProjection.Resolve(a, b, AxisBooleanOperation.Nor, frame);

        Assert.Equal(3, result.Pieces.Count);
        Assert.All(result.Pieces, piece => Assert.False(piece.Segment.PointsRight));
        Assert.Equal(2m, result.Pieces[0].Segment.Start.Value);
        Assert.Equal(0m, result.Pieces[0].Segment.End.Value);
        Assert.Equal(6m, result.Pieces[1].Segment.Start.Value);
        Assert.Equal(3m, result.Pieces[1].Segment.End.Value);
        Assert.Equal(10m, result.Pieces[2].Segment.Start.Value);
        Assert.Equal(8m, result.Pieces[2].Segment.End.Value);
        Assert.All(result.Pieces, piece => Assert.Equal(AxisBooleanCarrier.Frame, piece.Carrier));
    }

    [Fact]
    public void Or_MergesAdjacentCompatibleTruthRegions()
    {
        var a = Axis.FromCoordinates((Scalar)0m, (Scalar)2m);
        var b = Axis.FromCoordinates((Scalar)2m, (Scalar)4m);

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Or);

        var piece = Assert.Single(pieces);
        Assert.Equal(0m, piece.Segment.Start.Value);
        Assert.Equal(4m, piece.Segment.End.Value);
    }

    [Fact]
    public void Or_KeepsAdjacentRegionsSeparate_WhenTheirCarriersDifferStructurally()
    {
        var a = Axis.FromCoordinates((Scalar)0m, (Scalar)2m, basis: AxisBasis.Complex);
        var b = Axis.FromCoordinates((Scalar)2m, (Scalar)4m, basis: AxisBasis.SplitComplex);

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Or);

        Assert.Equal(2, pieces.Count);
        Assert.Equal(AxisBasis.Complex, pieces[0].Segment.Basis);
        Assert.Equal(AxisBasis.SplitComplex, pieces[1].Segment.Basis);
    }

    [Fact]
    public void BOnlyPieces_PreserveCarrierSupportAndBasis()
    {
        var a = new Axis(0, 2, 6, 3);
        var b = new Axis(new Proportion(-28, 7), new Proportion(66, 11), AxisBasis.SplitComplex);

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Or);

        Assert.Equal(2, pieces.Count);
        Assert.Equal(AxisBasis.SplitComplex, pieces[1].Segment.Basis);
        Assert.Equal(new Scalar(7m), pieces[1].Segment.Recessive.Recessive);
        Assert.Equal(new Scalar(11m), pieces[1].Segment.Dominant.Recessive);
    }

    [Theory]
    [MemberData(nameof(TruthTableCases))]
    public void ExplicitFrame_RespectsBooleanTruthTables(
        AxisBooleanOperation operation,
        IReadOnlyList<(decimal Start, decimal End)> expected)
    {
        var a = Axis.FromCoordinates((Scalar)0m, (Scalar)2m);
        var b = Axis.FromCoordinates((Scalar)1m, (Scalar)3m);
        var frame = Axis.FromCoordinates((Scalar)(-1m), (Scalar)4m);

        var result = AxisBooleanProjection.Resolve(a, b, operation, frame);

        Assert.Equal(expected.Count, result.Pieces.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Start, result.Pieces[i].Segment.Start.Value);
            Assert.Equal(expected[i].End, result.Pieces[i].Segment.End.Value);
        }
    }
}
