using Core2.Elements;
using Core2.Support;

namespace Tests.Core2;

public class AxisBooleanProjectionTests
{
    [Fact]
    public void Or_UsesADirection_WhenOverlapMakesTruthAmbiguous()
    {
        var a = Axis.FromCoordinates((Scalar)(-7.5m), (Scalar)(-3m));
        var b = Axis.FromCoordinates((Scalar)(-4.5m), (Scalar)(-1m));

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Or);

        Assert.NotEmpty(pieces);
        Assert.All(pieces, piece => Assert.True(piece.Source.End.Value < piece.Source.Start.Value));
        Assert.Equal((decimal)(-1m), pieces[0].Segment.Start.Value);
        Assert.Equal((decimal)(-7.5m), pieces[0].Segment.End.Value);
    }

    [Fact]
    public void Xor_PreservesSourceDirection_PerPiece()
    {
        var a = Axis.FromCoordinates((Scalar)(-7.5m), (Scalar)(-3m));
        var b = Axis.FromCoordinates((Scalar)(-4.5m), (Scalar)(-1m));

        var pieces = AxisBooleanProjection.Project(a, b, AxisBooleanOperation.Xor);

        Assert.Equal(2, pieces.Count);
        Assert.All(pieces, piece => Assert.True(piece.Segment.End.Value < piece.Segment.Start.Value));
        Assert.Equal((decimal)(-4.5m), pieces[0].Segment.Start.Value);
        Assert.Equal((decimal)(-7.5m), pieces[0].Segment.End.Value);
        Assert.Equal((decimal)(-1m), pieces[1].Segment.Start.Value);
        Assert.Equal((decimal)(-3m), pieces[1].Segment.End.Value);
    }
}
