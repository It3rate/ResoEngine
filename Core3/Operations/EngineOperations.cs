using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// One-shot framed operations for cases where the family does not need to
/// persist after the fold is complete.
/// </summary>
public static class EngineOperations
{
    private delegate bool FamilyAction<TResult>(EngineFamily family, out TResult? result)
        where TResult : class;

    public static bool TryAdd(
        EngineOperationContext context,
        out GradedElement? sum) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out GradedElement? result) => family.TryAddAll(out result),
            out sum);

    public static bool TryAdd(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? sum) =>
        TryAdd(EngineOperationContext.Create(frame, members), out sum);

    public static bool TryAddWithProvenance(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryAddAllWithProvenance(out value),
            out result);

    public static bool TryAddWithTension(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryAddAllWithTension(out value),
            out result);

    public static bool TryAddWithProvenance(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryAddWithProvenance(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryAddWithTension(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryAddWithTension(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryMultiply(
        EngineOperationContext context,
        out GradedElement? product) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out GradedElement? result) => family.TryMultiplyAll(out result),
            out product);

    public static bool TryMultiply(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? product) =>
        TryMultiply(EngineOperationContext.Create(frame, members), out product);

    public static bool TryMultiplyWithProvenance(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryMultiplyAllWithProvenance(out value),
            out result);

    public static bool TryMultiplyWithTension(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryMultiplyAllWithTension(out value),
            out result);

    public static bool TryMultiplyWithProvenance(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryMultiplyWithProvenance(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryMultiplyWithTension(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result)
    {
        return TryMultiplyWithTension(EngineOperationContext.Create(frame, members), out result);
    }

    public static bool TryBoolean(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBoolean(payload, out value),
            operation,
            out result);

    public static bool TryBooleanWithTension(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBooleanWithTension(payload, out value),
            operation,
            out result);

    public static bool TryBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        return TryBoolean(EngineOperationContext.Create(frame, members), operation, out result);
    }

    public static bool TryBooleanWithTension(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result)
    {
        return TryBooleanWithTension(EngineOperationContext.Create(frame, members), operation, out result);
    }

    public static bool TryOccupancyBoolean(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBoolean(payload, out value),
            operation,
            out result);

    public static bool TryOccupancyBooleanWithTension(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBooleanWithTension(payload, out value),
            operation,
            out result);

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

    public static bool TryOccupancyBooleanWithTension(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result,
        bool isOrdered = false)
    {
        return TryOccupancyBooleanWithTension(
            EngineOperationContext.Create(frame, members, isOrdered),
            operation,
            out result);
    }

    public static bool TryBooleanAdjacentPairs(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairs(payload, out value),
            operation,
            out results);

    public static bool TryBooleanAdjacentPairsWithTension(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairsWithTension(payload, out value),
            operation,
            out results);

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

    public static bool TryBooleanAdjacentPairsWithTension(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results)
    {
        return TryBooleanAdjacentPairsWithTension(
            EngineOperationContext.Create(frame, members, isOrdered: true),
            operation,
            out results);
    }

    private static bool TryWithFamily<TResult>(
        EngineOperationContext context,
        FamilyAction<TResult> action,
        out TResult? result)
        where TResult : class =>
        action(new EngineFamily(context), out result);

    private static bool TryWithCompositeFamily<TResult, TPayload>(
        EngineOperationContext context,
        CompositeFamilyAction<TResult, TPayload> action,
        TPayload payload,
        out TResult? result)
        where TResult : class
    {
        if (context.Frame is not CompositeElement)
        {
            result = null;
            return false;
        }

        return action(new EngineFamily(context), payload, out result);
    }

    private delegate bool CompositeFamilyAction<TResult, in TPayload>(
        EngineFamily family,
        TPayload payload,
        out TResult? result)
        where TResult : class;
}
