using Applied.Geometry.Utils;
using Core2.Repetition;

namespace Applied.Geometry.Frieze;

public sealed record StripEquationProgram(
    IReadOnlyList<StripSegmentDefinition> Equations,
    IReadOnlyList<EquationCommand> Loop,
    IReadOnlyList<EquationCommand>? Prelude = null);
