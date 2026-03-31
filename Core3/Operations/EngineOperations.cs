using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Friendly one-shot operation shell over the current family pipeline for cases
/// where the family does not need to persist after the read is complete.
/// </summary>
public static class EngineOperations
{
    private delegate bool FamilyAction<TResult>(EngineFamily family, out TResult? result)
        where TResult : class;

    private delegate bool CompositeFamilyAction<TResult, in TPayload>(
        EngineFamily family,
        TPayload payload,
        out TResult? result)
        where TResult : class;

    public static bool TryAdd(EngineOperationContext context, out GradedElement? sum) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out GradedElement? result) => family.TryAddAll(out result),
            out sum);

    public static bool TryAdd(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? sum) =>
        TryWithFamily(
            frame,
            members,
            static (EngineFamily family, out GradedElement? result) => family.TryAddAll(out result),
            out sum);

    public static bool TryAddResult(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryAddAllResult(out value),
            out result);

    public static bool TryAddResult(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result) =>
        TryWithFamily(
            frame,
            members,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryAddAllResult(out value),
            out result);

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
        TryWithFamily(
            frame,
            members,
            static (EngineFamily family, out GradedElement? result) => family.TryMultiplyAll(out result),
            out product);

    public static bool TryMultiplyResult(
        EngineOperationContext context,
        out EngineOperationResult? result) =>
        TryWithFamily(
            context,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryMultiplyAllResult(out value),
            out result);

    public static bool TryMultiplyResult(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out EngineOperationResult? result) =>
        TryWithFamily(
            frame,
            members,
            static (EngineFamily family, out EngineOperationResult? value) => family.TryMultiplyAllResult(out value),
            out result);

    public static bool TryBoolean(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBoolean(payload, out value),
            operation,
            out result);

    public static bool TryBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBoolean(payload, out value),
            out result);

    public static bool TryBooleanResult(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBooleanResult(payload, out value),
            operation,
            out result);

    public static bool TryBooleanResult(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out EngineBooleanResult? result) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (EngineFamily family, EngineBooleanOperation payload, out EngineBooleanResult? value) => family.TryBooleanResult(payload, out value),
            out result);

    public static bool TryOccupancyBoolean(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBoolean(payload, out value),
            operation,
            out result);

    public static bool TryOccupancyBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result,
        bool isOrdered = false) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBoolean(payload, out value),
            out result);

    public static bool TryOccupancyBooleanResult(
        EngineOperationContext context,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBooleanResult(payload, out value),
            operation,
            out result);

    public static bool TryOccupancyBooleanResult(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineOccupancyOperation operation,
        out EngineFamilyBooleanResult? result,
        bool isOrdered = false) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered,
            static (EngineFamily family, EngineOccupancyOperation payload, out EngineFamilyBooleanResult? value) => family.TryOccupancyBooleanResult(payload, out value),
            out result);

    public static bool TryBooleanAdjacentPairs(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairs(payload, out value),
            operation,
            out results);

    public static bool TryBooleanAdjacentPairs(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairs(payload, out value),
            out results);

    public static bool TryBooleanAdjacentPairResults(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairResults(payload, out value),
            operation,
            out results);

    public static bool TryBooleanAdjacentPairResults(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        EngineBooleanOperation operation,
        out IReadOnlyList<EngineBooleanResult>? results) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (EngineFamily family, EngineBooleanOperation payload, out IReadOnlyList<EngineBooleanResult>? value) => family.TryBooleanAdjacentPairResults(payload, out value),
            out results);

    private static bool TryWithFamily<TResult>(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        FamilyAction<TResult> action,
        out TResult? result)
        where TResult : class =>
        TryWithFamily(EngineOperationContext.Create(frame, members), action, out result);

    private static bool TryWithFamily<TResult>(
        EngineOperationContext context,
        FamilyAction<TResult> action,
        out TResult? result)
        where TResult : class =>
        action(new EngineFamily(context), out result);

    private static bool TryWithCompositeFamily<TResult, TPayload>(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        TPayload payload,
        bool isOrdered,
        CompositeFamilyAction<TResult, TPayload> action,
        out TResult? result)
        where TResult : class =>
        TryWithCompositeFamily(
            EngineOperationContext.Create(frame, members, isOrdered),
            action,
            payload,
            out result);

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
}
