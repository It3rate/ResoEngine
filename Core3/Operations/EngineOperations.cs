using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// One-shot framed operations for cases where the family does not need to
/// persist after the fold is complete.
/// </summary>
public static class EngineOperations
{
    public static bool TryAdd(
        EngineOperationContext context,
        out GradedElement? sum) =>
        new EngineFamily(context).TryAddAll(out sum);

    public static bool TryAdd(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? sum)
    {
        return TryAdd(EngineOperationContext.Create(frame, members), out sum);
    }

    public static bool TryAddWithProvenance(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        new EngineFamily(context).TryAddAllWithProvenance(out result);

    public static bool TryAddWithProvenance(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryAddWithProvenance(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryMultiply(
        EngineOperationContext context,
        out GradedElement? product) =>
        new EngineFamily(context).TryMultiplyAll(out product);

    public static bool TryMultiply(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? product)
    {
        return TryMultiply(EngineOperationContext.Create(frame, members), out product);
    }

    public static bool TryMultiplyWithProvenance(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        new EngineFamily(context).TryMultiplyAllWithProvenance(out result);

    public static bool TryMultiplyWithProvenance(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryMultiplyWithProvenance(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryBoolean(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        if (context.Frame is not CompositeElement)
        {
            result = null;
            return false;
        }

        return new EngineFamily(context).TryBoolean(operation, out result);
    }

    public static bool TryBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        return TryBoolean(EngineOperationContext.Create(frame, members), operation, out result);
    }

    public static bool TryOccupancyBoolean(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result)
    {
        if (context.Frame is not CompositeElement)
        {
            result = null;
            return false;
        }

        return new EngineFamily(context).TryOccupancyBoolean(operation, out result);
    }

    public static bool TryOccupancyBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result,
        bool isOrdered = false)
    {
        return TryOccupancyBoolean(
            EngineOperationContext.Create(frame, members, isOrdered),
            operation,
            out result);
    }

    public static bool TryBooleanAdjacentPairs(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        if (context.Frame is not CompositeElement)
        {
            results = null;
            return false;
        }

        return new EngineFamily(context).TryBooleanAdjacentPairs(operation, out results);
    }

    public static bool TryBooleanAdjacentPairs(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        return TryBooleanAdjacentPairs(
            EngineOperationContext.Create(frame, members, isOrdered: true),
            operation,
            out results);
    }
}
