namespace Core2.Geometry;

public sealed record StripEquationDefinition(
    string Name,
    StripDelta Delta,
    StripEquationMode Mode,
    int HoldCount);
