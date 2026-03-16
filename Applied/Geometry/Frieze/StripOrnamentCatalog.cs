using Applied.Geometry.Utils;
using Core2.Elements;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public static class StripOrnamentCatalog
{
    public static IReadOnlyList<StripOrnamentPattern> GalleryPatterns { get; } = CreateGalleryPatterns();

    public static IReadOnlyList<StripSegmentDefinition> CreateDefaultSegments() =>
    [
        new StripSegmentDefinition(
            "X0",
            Axis.FromCoordinates(Scalar.Zero, Scalar.One),
            BoundaryContinuationLaw.ReflectiveBounce,
            Directions2D.Right,
            Scalar.One),
        new StripSegmentDefinition(
            "Y0",
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.ReflectiveBounce,
            Directions2D.Up,
            Scalar.One),
        new StripSegmentDefinition(
            "X1",
            Axis.FromCoordinates(Scalar.Zero, 2),
            BoundaryContinuationLaw.TensionPreserving,
            Directions2D.Right,
            new Scalar(2m),
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
                    SharedSegment(byName["X1"], "The continuous long carrier advances the next square block."),
                ])
            {
                CallPattern = "Y0, Y0; X1",
                Program = Program(
                    equations,
                    [
                        Fire("Y0"),
                        Fire("Y0"), Commit(),
                        Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "The continuous long carrier closes each diagonal stride."),
                ])
            {
                CallPattern = "Y0, Y0, X1",
                Program = Program(
                    equations,
                    [
                        Fire("Y0"),
                        Fire("Y0"),
                        Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "A long horizontal carrier that spaces the next cell."),
                ])
            {
                CallPattern = "X0, Y0, X0, Y0, X1, then the mirrored descent before repeating",
                Program = Program(
                    equations,
                    [
                        Fire("X0"), Commit(),
                        Fire("Y0"), Commit(),
                        Fire("X0"), Commit(),
                        Fire("Y0"), Commit(),
                        Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "A long horizontal carrier that closes the rails."),
                ])
            {
                CallPattern = "X0; Y0 + X0 + Y0; X1, then the mirrored descent half",
                Program = Program(
                    equations,
                    [
                        Fire("X0"), Commit(),
                        Fire("Y0"),
                        Fire("X0"),
                        Fire("Y0"), Commit(),
                        Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "A long horizontal carrier that spaces the next point."),
                ])
            {
                CallPattern = "X0 + Y0; X0 + Y0; X1, then the opposite-slope half",
                Program = Program(
                    equations,
                    [
                        Fire("X0"),
                        Fire("Y0"), Commit(),
                        Fire("X0"),
                        Fire("Y0"), Commit(),
                        Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "A long horizontal carrier that bridges the next tooth."),
                ])
            {
                CallPattern = "X0; Y0; X0; Y0; X0; Y0; X1",
                Program = Program(
                    equations,
                    [
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X0"), Commit(),
                    Fire("Y0"), Commit(),
                    Fire("X1"), Commit(),
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
                    SharedSegment(byName["X1"], "A long horizontal carrier that advances each landing."),
                ])
            {
                CallPattern = "Y0; X1 across one full up-up-down-down bounce cycle",
                Program = Program(
                    equations,
                    [
                        Fire("Y0"), Commit(),
                        Fire("X1"), Commit(),
                    ]),
            },
        ];
    }

    private static StripOrnamentStrand SharedSegment(StripSegmentDefinition definition, string description) =>
        new(definition.Name, definition.DescribeTraversal(), description, []);

    private static StripEquationProgram Program(
        IReadOnlyList<StripSegmentDefinition> equations,
        IReadOnlyList<EquationCommand> loop,
        IReadOnlyList<EquationCommand>? prelude = null) =>
        new(equations, loop, prelude);

    private static EquationCommand Fire(string equationName) =>
        EquationCommand.Fire(equationName);

    private static EquationCommand Commit() =>
        EquationCommand.Commit();

    private static EquationCommand SetLaw(string equationName, BoundaryContinuationLaw law) =>
        EquationCommand.SetLaw(equationName, law);
}
