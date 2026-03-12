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
}
