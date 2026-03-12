using Core2.Elements;
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
}
