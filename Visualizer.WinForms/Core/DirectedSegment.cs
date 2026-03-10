namespace ResoEngine.Visualizer.Core;

/// <summary>
/// A directed segment: a 1D value with imaginary (start/dot) and real (end/arrow) extents.
/// Maps to Proportion/Axis in ResoEngine — imaginary is Con, real is Pro.
/// </summary>
public class DirectedSegment
{
    public float Imaginary { get; set; }
    public float Real { get; set; }
    public string Label { get; set; }

    public float Span => Real - Imaginary;

    public DirectedSegment(float imaginary, float real, string label = "")
    {
        Imaginary = imaginary;
        Real = real;
        Label = label;
    }

    public DirectedSegment Clone() => new(Imaginary, Real, Label);
}
