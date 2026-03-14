using Core2.Geometry;

namespace Tests.Core2;

public class SquareWaveDynamicsTests
{
    [Fact]
    public void SquareWaveDynamics_ProducesOrthogonalSquareWaveProgression()
    {
        var trace = SquareWaveDynamics.Run(12);

        Assert.Equal(12, trace.Steps.Count);
        Assert.NotNull(trace.SelectedContext);

        var state = trace.SelectedContext!.State;
        Assert.Equal(new StripPoint(8, 0), state.Cursor);
        Assert.Equal(12, state.Segments.Count);
        Assert.Equal(12, state.MacroStep);

        Assert.Equal(new StripPathEdge(new StripPoint(0, 0), new StripPoint(0, 1)), state.Segments[0]);
        Assert.Equal(new StripPathEdge(new StripPoint(0, 1), new StripPoint(1, 1)), state.Segments[1]);
        Assert.Equal(new StripPathEdge(new StripPoint(1, 1), new StripPoint(2, 1)), state.Segments[2]);
        Assert.Equal(new StripPathEdge(new StripPoint(2, 1), new StripPoint(2, 0)), state.Segments[3]);
        Assert.Equal(new StripPathEdge(new StripPoint(2, 0), new StripPoint(3, 0)), state.Segments[4]);
        Assert.Equal(new StripPathEdge(new StripPoint(3, 0), new StripPoint(4, 0)), state.Segments[5]);
        Assert.Equal(new StripPathEdge(new StripPoint(4, 0), new StripPoint(4, 1)), state.Segments[6]);
        Assert.Equal(new StripPathEdge(new StripPoint(4, 1), new StripPoint(5, 1)), state.Segments[7]);
    }

    [Fact]
    public void SquareWaveDynamics_KeepsThePathInsideTheStrip()
    {
        var trace = SquareWaveDynamics.Run(18);

        var state = trace.SelectedContext!.State;
        Assert.All(state.Segments, edge =>
        {
            Assert.InRange(edge.Start.Y, 0, 1);
            Assert.InRange(edge.End.Y, 0, 1);
        });

        Assert.All(trace.Steps, step => Assert.DoesNotContain(step.Resolution.Tensions, tension => tension.Kind == "VerticalBounds"));
    }
}
