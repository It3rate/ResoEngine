namespace Core2.Geometry;

public static class StripOrnamentCatalog
{
    public static IReadOnlyList<StripOrnamentPattern> GalleryPatterns { get; } =
    [
        new StripOrnamentPattern(
            "square-wave",
            "Square Wave",
            "Steady drift, a vertical pulse, and a backtrack pulse combine into the resolved orthogonal carrier.",
            6,
            9,
            [
                Strand("Advance", "R", "Carry the path forward one unit each micro-step.", Repeat(6, StripDelta.Right)),
                Strand("Vertical", "U . . D . .", "Pulse up on the first beat and down halfway through the cycle.",
                    StripDelta.Up, StripDelta.Zero, StripDelta.Zero, StripDelta.Down, StripDelta.Zero, StripDelta.Zero),
                Strand("Backtrack", "L . . L . .", "Cancel drift on the rise and fall beats so the turns become vertical.",
                    StripDelta.Left, StripDelta.Zero, StripDelta.Zero, StripDelta.Left, StripDelta.Zero, StripDelta.Zero),
            ]),
        new StripOrnamentPattern(
            "zigzag",
            "Zigzag",
            "A simple rightward drift and alternating lift create a balanced diagonal weave.",
            2,
            11,
            [
                Strand("Advance", "R", "Move one unit right on every beat.", Repeat(2, StripDelta.Right)),
                Strand("Tilt", "U D", "Alternate upward and downward lift while drift continues.", StripDelta.Up, StripDelta.Down),
            ]),
        new StripOrnamentPattern(
            "crossbar",
            "Crossbar Lattice",
            "Top and bottom cap pulses ride on a shared rail, producing a repeating I-beam rhythm.",
            8,
            8,
            [
                Strand("Pillar", "U . . D D . . U", "Raise to the top cap, drop through the stem, then recover to the baseline.",
                    StripDelta.Up, StripDelta.Zero, StripDelta.Zero, StripDelta.Down, StripDelta.Down, StripDelta.Zero, StripDelta.Zero, StripDelta.Up),
                Strand("Rail", ". R R . . R R .", "Carry the top and bottom cap spans across the strip.",
                    StripDelta.Zero, StripDelta.Right, StripDelta.Right, StripDelta.Zero, StripDelta.Zero, StripDelta.Right, StripDelta.Right, StripDelta.Zero),
            ]),
        new StripOrnamentPattern(
            "trapezoid",
            "Trapezoid Wave",
            "A falling ramp, flat span, rising ramp, and recovery span repeat as one strip cell.",
            4,
            9,
            [
                Strand("Drift", "R R R R", "Advance one unit right on each beat.", Repeat(4, StripDelta.Right)),
                Strand("Ramp", "D . U .", "Drop into the cell, hold, then climb back to the baseline.",
                    StripDelta.Down, StripDelta.Zero, StripDelta.Up, StripDelta.Zero),
            ]),
        new StripOrnamentPattern(
            "chevron",
            "Chevron Drift",
            "Each repeat resolves into a pointed chevron followed by a brief glide.",
            3,
            10,
            [
                Strand("Drift", "R R R", "Carry the chevron train to the right.", Repeat(3, StripDelta.Right)),
                Strand("Point", "U D .", "Lift and return on adjacent beats to form the chevron point.",
                    StripDelta.Up, StripDelta.Down, StripDelta.Zero),
            ]),
        new StripOrnamentPattern(
            "interlock",
            "Interlock",
            "Alternating upper and lower sockets share one baseline and produce an interlocking rectilinear rhythm.",
            8,
            8,
            [
                Strand("Socket", "U . D . D . U .", "Open an upper socket, then a lower one, before returning to baseline.",
                    StripDelta.Up, StripDelta.Zero, StripDelta.Down, StripDelta.Zero, StripDelta.Down, StripDelta.Zero, StripDelta.Up, StripDelta.Zero),
                Strand("Bridge", ". R . R . R . R", "Span across each socket to keep the strip continuous.",
                    StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right),
            ]),
        new StripOrnamentPattern(
            "stair",
            "Stair Step",
            "A stepped ascent and descent sit on a longer tread so the motif lands cleanly at the next start point.",
            10,
            7,
            [
                Strand("RiseFall", "U . U . D . D . . .", "Climb in two short risers, then descend in two matching risers.",
                    StripDelta.Up, StripDelta.Zero, StripDelta.Up, StripDelta.Zero, StripDelta.Down, StripDelta.Zero, StripDelta.Down, StripDelta.Zero, StripDelta.Zero, StripDelta.Zero),
                Strand("Tread", ". R . R . R . R R R", "Lay out the treads between risers and extend the recovery run.",
                    StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right, StripDelta.Zero, StripDelta.Right, StripDelta.Right, StripDelta.Right),
            ]),
    ];

    private static StripOrnamentStrand Strand(
        string name,
        string rhythm,
        string description,
        params StripDelta[] cycle) =>
        new(name, rhythm, description, cycle);

    private static StripDelta[] Repeat(int count, StripDelta delta)
    {
        var values = new StripDelta[count];
        Array.Fill(values, delta);
        return values;
    }
}
