namespace Core2.Symbolics.Dynamic;

public readonly record struct DynamicTension(
    string Kind,
    string Message,
    decimal Magnitude = 1m);
