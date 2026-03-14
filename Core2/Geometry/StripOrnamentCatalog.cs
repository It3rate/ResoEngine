namespace Core2.Geometry;

public static class StripOrnamentCatalog
{
    private static readonly IReadOnlyList<StripEquationDefinition> SharedEquations =
    [
        new StripEquationDefinition("X0", StripDelta.Right, StripEquationMode.Bounce, 1),
        new StripEquationDefinition("Y0", StripDelta.Up, StripEquationMode.Bounce, 2),
        new StripEquationDefinition("XLong", new StripDelta(2, 0), StripEquationMode.Continue, 1),
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
            "A short bouncing horizontal equation, a two-step vertical bounce, and a longer rightward carry combine into the repeating crossbar cell.",
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
            "The bounced short equation makes the slanted walls while the long carrier closes the top and bottom rails.",
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
            "Two paired fires of the short horizontal and vertical equations create the pointed chevrons before the longer glide resets the spacing.",
            3,
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
            "A primed short equation is switched to continue mode so the lattice can carry one extra horizontal tooth inside each interlocking cell.",
            6,
            8,
            [
                SharedX0(),
                SharedY0(),
                SharedXLong(),
            ])
        {
            CallPattern = "Prelude X0 then Continue; XLong; Y0,Y0; XLong; Y0; X0; Y0",
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
            "The vertical bounce fires on its own and the long horizontal carrier follows, producing a clean stepped ascent and descent.",
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
            "R, L, R, L, ...",
            "A one-step horizontal equation that remembers its current facing and bounces on every call.",
            []);

    private static StripOrnamentStrand SharedY0() =>
        new(
            "Y0",
            "U, U, D, D, ...",
            "A one-step vertical equation that holds direction for two calls before bouncing.",
            []);

    private static StripOrnamentStrand SharedXLong() =>
        new(
            "XLong",
            "2R, 2R, 2R, ...",
            "A longer horizontal carrier that does not bounce and always continues to the right.",
            []);

    private static StripEquationProgram Program(
        IReadOnlyList<StripEquationCommand> loop,
        IReadOnlyList<StripEquationCommand>? prelude = null) =>
        new(SharedEquations, loop, prelude);

    private static StripEquationCommand Fire(string equationName) =>
        StripEquationCommand.Fire(equationName);

    private static StripEquationCommand Commit() =>
        StripEquationCommand.Commit();

    private static StripEquationCommand SetMode(string equationName, StripEquationMode mode) =>
        StripEquationCommand.SetMode(equationName, mode);
}
