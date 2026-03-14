namespace Core2.Geometry;

public sealed record StripEquationProgram(
    IReadOnlyList<StripEquationDefinition> Equations,
    IReadOnlyList<StripEquationCommand> Loop,
    IReadOnlyList<StripEquationCommand>? Prelude = null);
