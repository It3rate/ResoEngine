using Core2.Elements;
using Core2.Repetition;

namespace Core2.Geometry;

public static class StripOrnamentCatalog
{
    public static IReadOnlyList<StripOrnamentPattern> GalleryPatterns { get; } = CreateGalleryPatterns();

    public static IReadOnlyList<StripSegmentDefinition> CreateDefaultSegments() =>
    [
        new StripSegmentDefinition(
            "X0",
            Axis.FromCoordinates(Scalar.Zero, Scalar.One),
            BoundaryContinuationLaw.ReflectiveBounce,
            StripDelta.Right),
        new StripSegmentDefinition(
            "Y0",
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.ReflectiveBounce,
            StripDelta.Up),
        new StripSegmentDefinition(
            "XLong",
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.TensionPreserving,
            StripDelta.Right,
            StripSegmentStepMode.Span,
            UseSegmentAsFrame: false),
    ];

    public static IReadOnlyList<StripOrnamentPattern> CreateGalleryPatterns(
        IReadOnlyList<StripSegmentDefinition>? sharedEquations = null)
    {
        var equations = (sharedEquations ?? CreateDefaultSegments()).ToArray();
        var byName = equations.ToDictionary(equation => equation.Name, StringComparer.OrdinalIgnoreCase);

        return
        [
            new StripOrnamentPattern(
                "square-wave",
                "Square Wave",
                "Two bounced vertical pulses commit into a rise or drop, and the long continuous carrier advances the next square step.",
                2,
                9,
                [
                    SharedSegment(byName["X0"], "A short horizontal carrier is defined but not used in this cell."),
                    SharedSegment(byName["Y0"], "The bounced vertical carrier climbs two steps, then descends two steps."),
                    SharedSegment(byName["XLong"], "The continuous long carrier advances the next square block."),
                ])
            {
                CallPattern = "Y0, Y0; XLong",
                Program = Program(
                    equations,
                    [
                        Fire("Y0"),
                        Fire("Y0"), Commit(),
                        Fire("XLong"), Commit(),
                    ]),
            },
            new StripOrnamentPattern(
                "zigzag",
                "Zigzag",
                "The same vertical bounce is now committed together with the long carrier, so each cell resolves as a diagonal instead of a square corner.",
                1,
                11,
                [
                    SharedSegment(byName["X0"], "A short horizontal carrier is defined but not used in this cell."),
                    SharedSegment(byName["Y0"], "The bounced vertical carrier supplies the alternating rise and fall."),
                    SharedSegment(byName["XLong"], "The continuous long carrier closes each diagonal stride."),
                ])
            {
                CallPattern = "Y0, Y0, XLong",
                Program = Program(
                    equations,
                    [
                        Fire("Y0"),
                        Fire("Y0"),
                        Fire("XLong"), Commit(),
                    ]),
            },
            new StripOrnamentPattern(
                "crossbar",
                "Crossbar Lattice",
                "The same three segment definitions form the lattice by firing the short horizontal, short vertical, and long horizontal carrier through a full mirrored cell.",
                10,
                8,
                [
                    SharedSegment(byName["X0"], "A bounced short horizontal carrier."),
                    SharedSegment(byName["Y0"], "A bounced short vertical carrier."),
                    SharedSegment(byName["XLong"], "A long horizontal carrier that spaces the next cell."),
                ])
            {
                CallPattern = "X0, Y0, X0, Y0, XLong, then the mirrored descent before repeating",
                Program = Program(
                    equations,
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
                    SharedSegment(byName["X0"], "A bounced short horizontal carrier."),
                    SharedSegment(byName["Y0"], "A bounced short vertical carrier."),
                    SharedSegment(byName["XLong"], "A long horizontal carrier that closes the rails."),
                ])
            {
                CallPattern = "X0; Y0 + X0 + Y0; XLong, then the mirrored descent half",
                Program = Program(
                    equations,
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
                    SharedSegment(byName["X0"], "A bounced short horizontal carrier."),
                    SharedSegment(byName["Y0"], "A bounced short vertical carrier."),
                    SharedSegment(byName["XLong"], "A long horizontal carrier that spaces the next point."),
                ])
            {
                CallPattern = "X0 + Y0; X0 + Y0; XLong, then the opposite-slope half",
                Program = Program(
                    equations,
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
                "A primed short carrier switches into tension-preserving continuation so the tooth can pass through the middle of the cell before the next lock.",
                6,
                8,
                [
                    SharedSegment(byName["X0"], "A short horizontal carrier that primes the tooth."),
                    SharedSegment(byName["Y0"], "A bounced vertical carrier that locks the hook in place."),
                    SharedSegment(byName["XLong"], "A long horizontal carrier that bridges the next tooth."),
                ])
            {
                CallPattern = "Prelude X0 then continue+tension; XLong; Y0,Y0; XLong; Y0; X0; Y0",
                Program = Program(
                    equations,
                    [
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("XLong"), Commit(),
                    ]),
            },
            new StripOrnamentPattern(
                "stair",
                "Stair Step",
                "The vertical bounced segment fires on its own and the long carrier follows, producing a full up-up-down-down stair cycle in one ornament cell.",
                8,
                7,
                [
                    SharedSegment(byName["X0"], "The short horizontal carrier is available but unused in this cell."),
                    SharedSegment(byName["Y0"], "A bounced vertical carrier that climbs and descends across the stair."),
                    SharedSegment(byName["XLong"], "A long horizontal carrier that advances each landing."),
                ])
            {
                CallPattern = "Y0; XLong across one full up-up-down-down bounce cycle",
                Program = Program(
                    equations,
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
    }

    private static StripOrnamentStrand SharedSegment(StripSegmentDefinition definition, string description) =>
        new(definition.Name, definition.DescribeTraversal(), description, []);

    private static StripEquationProgram Program(
        IReadOnlyList<StripSegmentDefinition> equations,
        IReadOnlyList<StripEquationCommand> loop,
        IReadOnlyList<StripEquationCommand>? prelude = null) =>
        new(equations, loop, prelude);

    private static StripEquationCommand Fire(string equationName) =>
        StripEquationCommand.Fire(equationName);

    private static StripEquationCommand Commit() =>
        StripEquationCommand.Commit();

    private static StripEquationCommand SetLaw(string equationName, BoundaryContinuationLaw law) =>
        StripEquationCommand.SetLaw(equationName, law);
}
