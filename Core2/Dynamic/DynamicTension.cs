namespace Core2.Dynamic;

public readonly record struct DynamicTension(
    string Kind,
    string Message,
    decimal Magnitude = 1m);
