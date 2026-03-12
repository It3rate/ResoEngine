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

        Assert.Equal(new Scalar(3), reading.Recessive);
        Assert.Equal(new Scalar(2), reading.Dominant);
    }

    [Fact]
    public void Read_OppositePerspective_NegatesDominantReading()
    {
        var frame = new MeasurementFrame(0, 2, 4);
        var interval = new DirectedInterval(-6, 8);

        var reading = frame.Read(interval, Perspective.Opposite);

        Assert.Equal(new Scalar(-3), reading.Recessive);
        Assert.Equal(new Scalar(-2), reading.Dominant);
    }

    [Fact]
    public void Perspective_Oppose_IsBinaryFlip()
    {
        Assert.Equal(Perspective.Opposite, Perspective.Dominant.Oppose());
        Assert.Equal(Perspective.Dominant, Perspective.Opposite.Oppose());
    }
}
