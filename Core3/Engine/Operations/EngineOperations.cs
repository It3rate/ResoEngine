using Core3.Engine;

namespace Core3.Engine.Operations;

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

    public static bool TryMultiply(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? product)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryMultiplyAll(out product);
    }

    public static bool TryMultiplyWithProvenance(
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

        return family.TryMultiplyAllWithProvenance(out result);
    }

    public static bool TryBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryBoolean(operation, out result);
    }

    public static bool TryOccupancyBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result,
        bool isOrdered = false)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame, isOrdered);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryOccupancyBoolean(operation, out result);
    }

    public static bool TryBooleanAdjacentPairs(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(members);

        var family = new EngineFamily(frame, isOrdered: true);

        foreach (var member in members)
        {
            family.AddMember(member);
        }

        return family.TryBooleanAdjacentPairs(operation, out results);
    }
}
