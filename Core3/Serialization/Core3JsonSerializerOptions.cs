namespace Core3.Serialization;

/// <summary>
/// Small manual serialization options for the current Core3 JSON writer.
/// </summary>
public sealed record Core3JsonSerializerOptions
{
    public static Core3JsonSerializerOptions Default { get; } = new();

    public bool Indented { get; init; } = true;
    public bool IncludeDerived { get; init; }
}
