using Core2.Repetition;

namespace Core2.Geometry;

public sealed record StripEquationCommand(
    StripEquationCommandKind Kind,
    string? EquationName = null,
    BoundaryContinuationLaw? Law = null)
{
    public static StripEquationCommand Fire(string equationName) =>
        new(StripEquationCommandKind.Fire, equationName);

    public static StripEquationCommand Commit() =>
        new(StripEquationCommandKind.Commit);

    public static StripEquationCommand SetLaw(string equationName, BoundaryContinuationLaw law) =>
        new(StripEquationCommandKind.SetLaw, equationName, law);
}
