using ResoEngine.Visualizer.Core;

namespace ResoEngine.Visualizer.Input;

/// <summary>Describes what was hit and how to constrain the drag.</summary>
public record DragTarget(
    ISegmentValue Segment,
    DragZone Zone,
    Rendering.SegmentOrientation Axis);
