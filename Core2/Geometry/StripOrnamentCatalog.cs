using Core2.Elements;
using Core2.Repetition;

namespace Core2.Geometry;

public static class StripOrnamentCatalog
{
    private static readonly IReadOnlyList<StripSegmentDefinition> SharedEquations =
    [
        new StripSegmentDefinition(
            "X0",
            new AxisTraversalDefinition(Axis.FromCoordinates(Scalar.Zero, Scalar.One), Scalar.One, BoundaryContinuationLaw.ReflectiveBounce),
            StripDelta.Right),
        new StripSegmentDefinition(
            "Y0",
            new AxisTraversalDefinition(Axis.FromCoordinates(Scalar.Zero, 2), Scalar.One, BoundaryContinuationLaw.ReflectiveBounce),
            StripDelta.Up),
        new StripSegmentDefinition(
            "XLong",
            new AxisTraversalDefinition(null, 2),
            StripDelta.Right),
    ];

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
            "The same three segment definitions form the lattice by firing the short horizontal, short vertical, and long horizontal carrier through a full mirrored cell.",
            10,
            8,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "X0, Y0, X0, Y0, XLong, then the mirrored descent before repeating",
            Program = Program(
                [
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                ]),
        },
        new StripOrnamentPattern(
            "trapezoid",
            "Trapezoid Wave",
            "The same bounced short equations now commit in grouped bursts, so the walls slope while the long carrier closes the top and bottom rails.",
            6,
            9,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "X0; Y0 + X0 + Y0; XLong, then the mirrored descent half",
            Program = Program(
                [
                    Fire("X0"), Commit(),
                    Fire("Y0"),
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"),
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                ]),
        },
        new StripOrnamentPattern(
            "chevron",
            "Chevron Drift",
            "Two paired fires of the bounced short equations create the pointed chevrons, and the long carrier spaces the next point cleanly.",
            6,
            10,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "X0 + Y0; X0 + Y0; XLong, then the opposite-slope half",
            Program = Program(
                [
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("X0"),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                ]),
        },
        new StripOrnamentPattern(
            "interlock",
            "Interlock",
            "A primed short segment switches from reflective bounce into tension-preserving continuation so the interlocking tooth can carry through the middle of the cell.",
            6,
            8,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "Prelude X0 then continue-with-tension; XLong; Y0,Y0; XLong; Y0; X0; Y0",
            Program = Program(
                [
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                ],
                [
                    //Fire("X0"),
                    //SetMode("X0", StripEquationMode.Continue),
                ]),
        },
        new StripOrnamentPattern(
            "stair",
            "Stair Step",
            "The vertical bounced segment fires on its own and the long carrier follows, producing a full up-up-down-down stair cycle in one ornament cell.",
            8,
            7,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "Y0; XLong across one full up-up-down-down bounce cycle",
            Program = Program(
                [
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                ]),
        },
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

    private static StripOrnamentStrand SharedX0() =>
        new(
            "X0",
            "frame [0,1] · step +1 · reflect",
            "A one-step horizontal segment defined on axis [0,1] with reflective bounce. Its facing flips automatically at the frame edge.",
            []);

    private static StripOrnamentStrand SharedY0() =>
        new(
            "Y0",
            "frame [0,2] · step +1 · reflect",
            "A one-step vertical segment defined on axis [0,2] with reflective bounce. It rises twice, then descends twice.",
            []);

    private static StripOrnamentStrand SharedXLong() =>
        new(
            "XLong",
            "unbounded · step +2",
            "A longer horizontal carrier with no active boundary frame, so it continues to the right without wrapping or bouncing.",
            []);

    private static StripEquationProgram Program(
        IReadOnlyList<StripEquationCommand> loop,
        IReadOnlyList<StripEquationCommand>? prelude = null) =>
        new(SharedEquations, loop, prelude);

    private static StripEquationCommand Fire(string equationName) =>
        StripEquationCommand.Fire(equationName);

    private static StripEquationCommand Commit() =>
        StripEquationCommand.Commit();

    private static StripEquationCommand SetLaw(string equationName, BoundaryContinuationLaw law) =>
        StripEquationCommand.SetLaw(equationName, law);
}
