using ResoEngine.Core2;

namespace Tests.Core2;

public class ValueFrameTests
{
    [Fact]
    public void ValueFrame_CarriesPerspectiveAndChildMembership()
    {
        var frame = Axis.One.AsFrame();

        var child = frame.AddChild(Axis.I);

        Assert.Equal(Perspective.Dominant, frame.Perspective);
        Assert.Equal(Perspective.Dominant, child.Perspective);
        Assert.Same(frame, child.Parent);
        Assert.Single(frame.Children);
        Assert.Equal(Axis.One, frame.Encode(static value => -value));
    }

    [Fact]
    public void ValueFrame_OppositePerspectiveUsesCallerProvidedEncoding()
    {
        var frame = Axis.One.AsFrame(Perspective.Opposite);

        Assert.Equal(Axis.NegativeOne, frame.Encode(static value => -value));

        frame.OpposePerspective();

        Assert.Equal(Perspective.Dominant, frame.Perspective);
        Assert.Equal(Axis.One, frame.Encode(static value => -value));
    }
}
