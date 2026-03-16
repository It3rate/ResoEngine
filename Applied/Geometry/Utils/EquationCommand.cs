using Core2.Repetition;

namespace Applied.Geometry.Utils;

public sealed record EquationCommand(
    CommandKind Kind,
    string? EquationName = null,
    BoundaryContinuationLaw? Law = null)
{
    public static EquationCommand Fire(string equationName) =>
        new(CommandKind.Fire, equationName);

    public static EquationCommand Commit() =>
        new(CommandKind.Commit);

    public static EquationCommand SetLaw(string equationName, BoundaryContinuationLaw law) =>
        new(CommandKind.SetLaw, equationName, law);
}
