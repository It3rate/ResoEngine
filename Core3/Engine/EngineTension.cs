namespace Core3.Engine;

/// <summary>
/// Small shared helpers for carrying held tension and explanatory note text
/// across Core3 layers without re-implementing the same merge rules locally.
/// </summary>
public static class EngineTension
{
    public static GradedElement? CombineTension(params GradedElement?[] tensions)
    {
        foreach (var tension in tensions)
        {
            if (tension is not null)
            {
                return tension;
            }
        }

        return null;
    }

    public static string? CombineNotes(params string?[] notes)
    {
        var present = notes
            .Where(note => !string.IsNullOrWhiteSpace(note))
            .Distinct()
            .ToArray();

        return present.Length switch
        {
            0 => null,
            1 => present[0],
            _ => string.Join(" | ", present)
        };
    }
}
