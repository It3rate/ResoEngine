using Applied.Geometry.Frieze;
using Applied.Geometry.Utils;

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
        Assert.Equal(new PlanarPoint(8, 0), state.Cursor);
        Assert.Equal(12, state.Segments.Count);
        Assert.Equal(12, state.MacroStep);

        Assert.Equal(new PlanarPathEdge(new PlanarPoint(0, 0), new PlanarPoint(0, 1)), state.Segments[0]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(0, 1), new PlanarPoint(0, 2)), state.Segments[1]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(0, 2), new PlanarPoint(2, 2)), state.Segments[2]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(2, 2), new PlanarPoint(2, 1)), state.Segments[3]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(2, 1), new PlanarPoint(2, 0)), state.Segments[4]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(2, 0), new PlanarPoint(4, 0)), state.Segments[5]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(4, 0), new PlanarPoint(4, 1)), state.Segments[6]);
        Assert.Equal(new PlanarPathEdge(new PlanarPoint(4, 1), new PlanarPoint(4, 2)), state.Segments[7]);
    }

    [Fact]
    public void SquareWaveDynamics_KeepsThePathInsideTheFrieze()
    {
        var trace = SquareWaveDynamics.Run(18);

        var state = trace.SelectedContext!.State;
        Assert.All(state.Segments, edge =>
        {
            Assert.InRange(edge.Start.Y, 0, 2);
            Assert.InRange(edge.End.Y, 0, 2);
        });

        Assert.All(trace.Steps, step => Assert.DoesNotContain(step.Resolution.Tensions, tension => tension.Kind == "VerticalBounds"));
    }
}
