using Core2.Repetition;

namespace Core2.Geometry;

public sealed record StripEquationProgram(
    IReadOnlyList<StripSegmentDefinition> Equations,
    IReadOnlyList<StripEquationCommand> Loop,
    IReadOnlyList<StripEquationCommand>? Prelude = null);
