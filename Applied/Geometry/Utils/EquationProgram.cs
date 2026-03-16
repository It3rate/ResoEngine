namespace Applied.Geometry.Utils;

public sealed record EquationProgram(
    IReadOnlyList<PlanarSegmentDefinition> Equations,
    IReadOnlyList<EquationCommand> Loop,
    IReadOnlyList<EquationCommand>? Prelude = null);
