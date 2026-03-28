namespace Core3.Engine;

/// <summary>
/// One-shot framed operations for cases where the family does not need to
/// persist after the fold is complete.
/// </summary>
public static class EngineOperations
{
    public static bool TryAdd(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? sum)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryAddAll(out sum);
    }

    public static bool TryAddWithProvenance(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryAddAllWithProvenance(out result);
    }
}
