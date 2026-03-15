namespace Core2.Propagation;

public sealed record CouplingRule(
    CouplingKind Kind,
    decimal Strength = 1m,
    decimal Radius = 0m,
    decimal Priority = 1m,
    string? Channel = null,
    string? Note = null);
