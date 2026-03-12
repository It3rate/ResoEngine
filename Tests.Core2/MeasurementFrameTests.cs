using ResoEngine.Core2;

namespace Tests.Core2;

public class MeasurementFrameTests
{
    [Fact]
    public void Read_DominantPerspective_UsesLeftForStartAndRightForEnd()
    {
        var frame = new MeasurementFrame(0, 2, 4);
        var interval = new DirectedInterval(-6, 8);

        var reading = frame.Read(interval);

        Assert.Equal(new Proportion(6, 2), reading.Recessive);
        Assert.Equal(new Proportion(8, 4), reading.Dominant);
    }

    [Fact]
    public void Read_OppositePerspective_NegatesDominantReading()
    {
        var frame = new MeasurementFrame(0, 2, 4);
        var interval = new DirectedInterval(-6, 8);

        var reading = frame.Read(interval, Perspective.Opposite);

        Assert.Equal(new Proportion(-6, 2), reading.Recessive);
        Assert.Equal(new Proportion(-8, 4), reading.Dominant);
    }

    [Fact]
    public void Perspective_Oppose_IsBinaryFlip()
    {
        Assert.Equal(Perspective.Opposite, Perspective.Dominant.Oppose());
        Assert.Equal(Perspective.Dominant, Perspective.Opposite.Oppose());
    }

    [Fact]
    public void Read_UsesStoredPerspectiveAndUnitRoleTransforms()
    {
        var interval = new DirectedInterval(-6, 8);
        var frame = new MeasurementFrame(0, 2, 4, Perspective.Opposite);

        var oppositeReading = frame.Read(interval);
        var swappedUnitsReading = frame
            .WithPerspective(Perspective.Dominant)
            .SwapUnitRoles()
            .Read(interval);

        Assert.Equal(new Proportion(-6, 2), oppositeReading.Recessive);
        Assert.Equal(new Proportion(-8, 4), oppositeReading.Dominant);
        Assert.Equal(new Proportion(6, 4), swappedUnitsReading.Recessive);
        Assert.Equal(new Proportion(8, 2), swappedUnitsReading.Dominant);
    }
}
