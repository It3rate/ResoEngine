using Core2.Elements;
using Core2.Support;
using ResoEngine.Core2;

namespace Tests.Core2;

public class AreaTests
{
    [Fact]
    public void Area_IsConstructedFromTwoAxes_AndSupportsOpposition()
    {
        var area = new Area(Axis.One, Axis.I);

        var opposed = area.ApplyOpposition();

        Assert.Equal(Axis.I, opposed.Recessive);
        Assert.Equal(Axis.NegativeOne, opposed.Dominant);
    }

    [Fact]
    public void Area_UnaryTransforms_RecurseAcrossAxisComponents()
    {
        var recessive = new Axis(new Proportion(3, 1), new Proportion(2, 1));
        var dominant = new Axis(new Proportion(5, 1), new Proportion(7, 1));
        var area = new Area(recessive, dominant);

        Assert.Equal(new Area(dominant, recessive), area.Mirror());
        Assert.Equal(area.Mirror(), area.SwapUnitRoles());

        var projected = area.ProjectRecessiveIntoDominant();

        Assert.Equal(Axis.Zero, projected.Recessive);
        Assert.Equal(dominant + (-recessive), projected.Dominant);
    }

    [Fact]
    public void ExpandTerms_ExposesTheFourQuadrantProductsBeforeFold()
    {
        var area = new Area(
            new Axis(new Proportion(3, 1), new Proportion(5, 1)),
            new Axis(new Proportion(2, 1), new Proportion(4, 1)));

        var quadrants = area.Expand();
        var terms = area.ExpandTerms();

        Assert.Equal(new Proportion(6, 1), quadrants.Ii);
        Assert.Equal(new Proportion(12, 1), quadrants.Ir);
        Assert.Equal(new Proportion(10, 1), quadrants.Ri);
        Assert.Equal(new Proportion(20, 1), quadrants.Rr);
        Assert.Equal(quadrants.Fold(), area.Fold());
        Assert.Equal(quadrants.Ii, terms.ii);
        Assert.Equal(quadrants.Ir, terms.ir);
        Assert.Equal(quadrants.Ri, terms.ri);
        Assert.Equal(quadrants.Rr, terms.rr);
        Assert.Equal(new Axis(new Proportion(22, 1), new Proportion(14, 1)), area.Fold());
    }

    [Fact]
    public void Area_Value_IsTheFoldedOneDimensionalResult()
    {
        var area = new Area(
            new Axis(new Proportion(3, 1), new Proportion(5, 1)),
            new Axis(new Proportion(2, 1), new Proportion(4, 1)));

        Assert.Equal(area.Fold(), area.Value);
        Assert.Equal(AxisBasis.Complex, area.Basis);
    }

    [Fact]
    public void Area_EnvelopeAndIntersection_RecurseAcrossBothAxes()
    {
        var left = new Area(
            new Axis(new Proportion(3, 1), new Proportion(5, 1)),
            new Axis(new Proportion(2, 1), new Proportion(6, 1)));
        var right = new Area(
            new Axis(new Proportion(1, 1), new Proportion(3, 1)),
            new Axis(new Proportion(4, 1), new Proportion(5, 1)));

        var intersection = left.Intersect(right);

        Assert.Equal(new Axis(new Proportion(1, 1), new Proportion(3, 1)), intersection.Recessive);
        Assert.Equal(new Axis(new Proportion(2, 1), new Proportion(5, 1)), intersection.Dominant);
        Assert.Equal(new Area(left.Recessive.Envelope(right.Recessive), left.Dominant.Envelope(right.Dominant)), left.Envelope(right));
        Assert.Equal(left.Envelope(right), left.Union(right));
    }

    [Fact]
    public void FlipPerspective_IsExplicitAtTheAreaLevel()
    {
        var area = new Area(Axis.One, Axis.I);

        Assert.Equal(-area, area.FlipPerspective());
    }
}
