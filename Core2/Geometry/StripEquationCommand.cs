namespace Core2.Geometry;

public sealed record StripEquationCommand(
    StripEquationCommandKind Kind,
    string? EquationName = null,
    StripEquationMode? Mode = null)
{
    public static StripEquationCommand Fire(string equationName) =>
        new(StripEquationCommandKind.Fire, equationName);

    public static StripEquationCommand Commit() =>
        new(StripEquationCommandKind.Commit);

    public static StripEquationCommand SetMode(string equationName, StripEquationMode mode) =>
        new(StripEquationCommandKind.SetMode, equationName, mode);
}
